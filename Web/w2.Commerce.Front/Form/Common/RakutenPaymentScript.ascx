<%--
=========================================================================================================
  Module      : Rakuten Payment Script(RakutenPaymentScript.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RakutenPaymentScript.ascx.cs" Inherits="Form_Common_RakutenPaymentScript" %>
<script type="text/javascript">
	<% if (this.IsUserCreditcardInputPage) { %>
		if ($('[id$=divCreditCardForTokenAcquired]').length === 0) {
			$('[id$=lbConfirm]').first().css('pointer-events', 'none');
		}
	<% } %>
	<% if (this.IsEdit) { %>
		$('input[id$=tbUserCreditCardName]').blur(ableButton);
	<% } %>
	<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
		$('[id$=lbEditCreditCardNoForRakutenToken]').click(ableButton);
		$('[id$=lbEditCreditCardNoForToken]').click(ableButton);
	<% } %>

	var _isPayvaultSetup = false;
	var current_scrolly;

	var RakutenPaymentPayvault = (function () {

		var PAYMENT_RAKUTEN_CREDIT_SERVICE_ID = '<%= Constants.PAYMENT_RAKUTEN_CREDIT_SERVICE_ID %>';
		var RAKUTEN_INTERGRATION_CASE = '<%= Constants.RAKUTEN_INTERGRATION_CASE %>';
		var CREDIT_CARD_STYLES = {
			base: {
				'color': 'black',
				'border': '1px solid #aaaaaa',
				'font-size': '13px'
			},
			valid: {
				'color': 'green',
				'border': '1px solid green',
				'font-size': '13px'
			},
			invalid: {
				'color': 'red',
				'border': '1px solid red',
				'font-size': '13px'
			},
			focus: {
				'color': 'black',
				'border': '1px solid violet',
				'font-size': '13px'
			}
		};

		var SELECTOR_PREFIX_CARD_NUMBER = '#rakuten-card-mount-';
		var SELECTOR_PREFIX_EXPIRATION_MONTH = '#rakuten-expiration-month-mount-';
		var SELECTOR_PREFIX_EXPIRATION_YEAR = '#rakuten-expiration-year-mount-';
		var SELECTOR_PREFIX_CVV = '#rakuten-cvv-mount-';

		var CLASS_NAME_PAYVAULT_LABEL = 'temp-payvault';

		var _isExpirationYearSelected = false;
		var _targetCartIndex = 0;

		function _initialize(index) {
			$('[id$=btnClose]').first().css('pointer-events', '');
			
            current_scrolly = $(window).scrollTop();
           $('#aspnetForm').css({
                position: 'fixed',
                width: '100%'
			});
			$('[id^=rakuten-payment-').each(function () {
				var cartIndex = $(this).data('cart-index');
				if (index != cartIndex) {
					return true;
				}
				var $modal = $("#modal-rakuten-payment-" + cartIndex);
				$modal.show();

				// マスクされたカード情報をクリアする
				var $maskedLabelLastFourDigit = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lLastFourDigitForTokenAcquiredRakuten]');
				var $maskedLabelMonth = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lExpirationMonthForTokenAcquiredRakuten]');
				var $maskedLabelYear = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lExpirationYearForTokenAcquiredRakuten]');
				var $maskedLabelAuthorName = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lCreditAuthorNameForTokenAcquiredRakuten]');
				$maskedLabelLastFourDigit.text('');
				$maskedLabelMonth.text('');
				$maskedLabelYear.text('');
				$maskedLabelAuthorName.text('');
				$('.' + CLASS_NAME_PAYVAULT_LABEL).remove();

				var $card = $(this).find(SELECTOR_PREFIX_CARD_NUMBER + cartIndex);
				var $expirationMonth = $(this).find(SELECTOR_PREFIX_EXPIRATION_MONTH + cartIndex);
				var $expirationYear = $(this).find(SELECTOR_PREFIX_EXPIRATION_YEAR + cartIndex);
				var $cvv = $(this).find(SELECTOR_PREFIX_CVV + cartIndex);

				var $errorCardNumber = $(this).find('#cvCreditCardNo1Rakuten-' + cartIndex);
				var $errorCardExp = $(this).find('#cvCreditExpire-' + cartIndex);
				var $errorCardCvv = $(this).find('#cvCreditSecurityCode-' + cartIndex);

				// 要素が存在しないか、既に入力項目が作られている場合はスキップ
				if (!($card.length && $expirationMonth.length && $expirationYear.length && $cvv.length)
					|| _isPayvaultSetup) {
					return true;
				}

				var cardElements = {
					cardNumber: {
						mount: SELECTOR_PREFIX_CARD_NUMBER + cartIndex,
						styles: CREDIT_CARD_STYLES,
						height: '30px'
					},
					expirationMonth: {
						mount: SELECTOR_PREFIX_EXPIRATION_MONTH + cartIndex,
						styles: CREDIT_CARD_STYLES,
						inputType: 'select',
						placeholder: '月',
						height: '25px'
					},
					expirationYear: {
						mount: SELECTOR_PREFIX_EXPIRATION_YEAR + cartIndex,
						styles: CREDIT_CARD_STYLES,
						inputType: 'select',
						placeholder: '年',
						height: '25px'
					},
					cvv: {
						mount: SELECTOR_PREFIX_CVV + cartIndex,
						styles: CREDIT_CARD_STYLES,
						height: '30px'
					}
				};
				var $creditToken = $('[id$=hfCreditTokenSameAs1]').eq(cartIndex - 1);
				var $cardToken = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=hfCreditToken]');
				$creditToken.val('');
				$cardToken.val('');
				payvault.setup(cardElements);

				payvault.card.addEventListener('cardNumber', 'input', function (event) {
					if (!$errorCardNumber.length) return;

					_getPayVaultToken(
						cartIndex,
						$errorCardNumber,
						event.valid,
						'正しいカード番号を入力してください。');
				});
				payvault.card.addEventListener('expirationMonth', 'input', function (event) {
					if (!$errorCardExp.length) return;

					_getPayVaultToken(
						cartIndex,
						$errorCardExp,
						event.validDate || !_isExpirationYearSelected,
						'正しい有効期限を入力してください。');
				});
				payvault.card.addEventListener('expirationYear', 'input', function (event) {
					if (!$errorCardExp.length) return;

					_getPayVaultToken(
						cartIndex,
						$errorCardExp,
						event.validDate,
						'正しい有効期限を入力してください。');

					_isExpirationYearSelected = event.valid;
				});
				payvault.card.addEventListener('cvv', 'input', function (event) {
					if (!$errorCardCvv.length) return;
					if (event.length < 3) return;

					_getPayVaultToken(
						cartIndex,
						$errorCardCvv,
						event.valid && event.validCvvForCardNumber,
						'正しいセキュリティコードを入力してください。');
				});

				$('input[id$=tbUserCreditCardName]').blur(ableButton);
				$('input[id$=tbCreditAuthorNameRakuten]').blur(ableButton);

				_isPayvaultSetup = true;
			});
		}

		function _appendLabel($element, text) {
			var $appendElement = $('<span>', { class: CLASS_NAME_PAYVAULT_LABEL, text: text });
			$element.append($appendElement);
		}

		function _setCardDetailsToHidden(maskedCardDetails) {
			var cartIndex = RakutenPaymentPayvault.targetCartIndex;
			var $rakutenPayment = $('#rakuten-payment-' + cartIndex);

			var elementIndex = RakutenPaymentPayvault.targetCartIndex - 1;

			var $creditToken = $('[id$=hfCreditTokenSameAs1]').eq(elementIndex);
			var $cardToken = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfCreditToken]');
			var $cardMount = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfMyCardMount]');
			var $expMonth = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfMyExpirationMonthMount]');
			var $expYear = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfMyExpirationYearMount]');
			var $cvvMount = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfMyCvvMount]');
			var $authorName = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfAuthorNameCard]');
			var $creditCardCompany = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=hfCreditCardCompany]');

			if ($cardMount.length) $cardMount.val(maskedCardDetails.last4digits);
			if ($expMonth.length) $expMonth.val(maskedCardDetails.expirationMonth);
			if ($expYear.length) $expYear.val(maskedCardDetails.expirationYear);
			if ($cvvMount.length) $cvvMount.val(maskedCardDetails.cvvToken);
			if ($creditToken.length) $creditToken.val(maskedCardDetails.cardToken);
			if ($cardToken.length) $cardToken.val(maskedCardDetails.cardToken);
			if ($creditCardCompany) $creditCardCompany.val(maskedCardDetails.brandCode);
			if ($authorName.length) $authorName.val($rakutenPayment.find('input[id$=tbCreditAuthorNameRakuten]').val());
		}

		function _switchToMaskedCardDetails(maskedCardDetails) {
			var cartIndex = RakutenPaymentPayvault.targetCartIndex;

			var $rakutenPayment = $('#rakuten-payment-' + cartIndex);

			var $card = $rakutenPayment.find(SELECTOR_PREFIX_CARD_NUMBER + cartIndex);
			var $expirationMonth = $rakutenPayment.find(SELECTOR_PREFIX_EXPIRATION_MONTH + cartIndex);
			var $expirationYear = $rakutenPayment.find(SELECTOR_PREFIX_EXPIRATION_YEAR + cartIndex);
			var $cvv = $rakutenPayment.find(SELECTOR_PREFIX_CVV + cartIndex);
			var $toEditMode = $('.lbEditCreditCardNoForRakutenToken');
			var $dispCreditInfo = $('#temp-card-info-' + cartIndex);

			// マスクされたカード情報を表示する
			$dispCreditInfo.show();
			$card.find('iframe').hide();
			$expirationMonth.find('iframe').hide();
			$expirationYear.find('iframe').hide();
			$cvv.find('iframe').hide();
			// マスクされたカード情報を設定する
			_appendLabel($card, 'XXXXXXXXXXXX'.concat(maskedCardDetails.last4digits));
			_appendLabel($expirationMonth, maskedCardDetails.expirationMonth);
			_appendLabel($expirationYear, maskedCardDetails.expirationYear);
			_appendLabel($cvv, 'XXX');
			var $maskedLabelLastFourDigit = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=lLastFourDigitForTokenAcquiredRakuten]');
			var $maskedLabelMonth = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=lExpirationMonthForTokenAcquiredRakuten]');
			var $maskedLabelYear = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=lExpirationYearForTokenAcquiredRakuten]');
			var $maskedLabelAuthorName = $rakutenPayment.closest('[id$=divCreditCardInputForm]').find('[id$=lCreditAuthorNameForTokenAcquiredRakuten]');
			if ($maskedLabelLastFourDigit.length) $maskedLabelLastFourDigit.text(maskedCardDetails.last4digits);
			if ($maskedLabelMonth.length) $maskedLabelMonth.text(maskedCardDetails.expirationMonth);
			if ($maskedLabelYear.length) $maskedLabelYear.text(maskedCardDetails.expirationYear);
			if ($maskedLabelAuthorName.length) $maskedLabelAuthorName.text($rakutenPayment.find('input[id$=tbCreditAuthorNameRakuten]').val());

			// 「入力」ボタンを非活性に、「再入力」ボタンを活性可する
			$('#edit-button-' + cartIndex).hide();
			$toEditMode.show();
			$toEditMode.click(function (e) {
				$card.find('iframe').show();
				$expirationMonth.find('iframe').show();
				$expirationYear.find('iframe').show();
				$cvv.find('iframe').show();

				_initialize(cartIndex);
				$dispCreditInfo.hide();
				e.preventDefault();
			});
		}

		function _createPayVaultToken($element) {
			LoadingShow();

			payvault.card.createToken({
				serviceId: PAYMENT_RAKUTEN_CREDIT_SERVICE_ID,
				attachResponseTarget: $('#rakuten-payvault-form-' + RakutenPaymentPayvault.targetCartIndex),
				integrationCase: RAKUTEN_INTERGRATION_CASE
			}, function (response) {
				if (response.resultType === 'failure') {
					LoadingHide();
					return;
				}

				$element.empty();
				_setCardDetailsToHidden(response.maskedCardDetails);
				_switchToMaskedCardDetails(response.maskedCardDetails);
				ableButton();
				$('[id$=btnClose]').first().css('pointer-events', '');
				LoadingHide();
			});
		}

		function _getPayVaultToken(cartIndex, $element, isValid, errorMessage) {

			if (!_displayErrorMessage($element, isValid, errorMessage)) return;

			RakutenPaymentPayvault.targetCartIndex = cartIndex;

			_createPayVaultToken($element);
		}

		function _displayErrorMessage($element, isValid, errorMessage) {

			if (!isValid) {
				$element.html(errorMessage)
					.css('visibility', 'visible');
				return false;
			}

			$element.css('visibility', 'hidden');
			return true;
		}

		return {
			targetCartIndex: _targetCartIndex,
			initialize: _initialize,
		}
	}());

	if (Sys && Sys.Application) { Sys.Application.add_load(EnableEditPayvaultForm); } else { EnableEditPayvaultForm(); }
	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(EnableEditPayvaultForm);

	function GetDivUserCreditCardName() {
		var creditCardNameCheck = $('[id$=cbRegistCreditCard]')[0];
		if (creditCardNameCheck.checked) {
			$('[id$=divUserCreditCardName]')[0].style['display'] = 'block';
		} else {
			$('[id$=divUserCreditCardName]')[0].style['display'] = 'none';
		}
	}

	function CloseCreditCardInputModal(cartIndex) {
		if (typeof payvault.destroy == 'function') payvault.destroy();
		_isPayvaultSetup = false;
		var $modal = $('#modal-rakuten-payment-' + cartIndex);
        $('#aspnetForm').attr({ style: '' });
        $('html, body').prop({ scrollTop: current_scrolly });
		$modal.hide();
	}

	function EnableEditPayvaultForm() {
		$('[id^=rakuten-payment-').each(function () {
			var cartIndex = $(this).data('cart-index');
			var elementIndex = cartIndex - 1;

			var $tempData = $(this).find('.temp-payvault');
			var $dispCreditInfo = $('#temp-card-info-' + cartIndex);
			var $creditToken = $('[id$=hfCreditTokenSameAs1]').eq(elementIndex);
			var $cardToken = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=hfCreditToken]');
			var $buttonEdit = $('#edit-button-' + cartIndex);
			var $buttonReEdit = $(this).closest('[id$=divCreditCardInputForm]').find('.lbEditCreditCardNoForRakutenToken');

			var $maskedLabelLastFourDigit = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lLastFourDigitForTokenAcquiredRakuten]');
			var $maskedLabelMonth = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lExpirationMonthForTokenAcquiredRakuten]');
			var $maskedLabelYear = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lExpirationYearForTokenAcquiredRakuten]');
			var $maskedLabelAuthorName = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=lCreditAuthorNameForTokenAcquiredRakuten]');
			var $cardMount = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=hfMyCardMount]');
			var $expMonth = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=hfMyExpirationMonthMount]');
			var $expYear = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=hfMyExpirationYearMount]');
			var $authorName = $(this).closest('[id$=divCreditCardInputForm]').find('[id$=hfAuthorNameCard]');


			// 入力ボタンが存在しない（モーダルではない）場合は先頭の入力フォームを活性化する
			if ($buttonEdit.length == 0) {
				RakutenPaymentPayvault.initialize(1);
				return;
			}

			if ($cardToken.val() || $creditToken.val()) {
				$maskedLabelLastFourDigit.text($cardMount.val());
				$maskedLabelMonth.text($expMonth.val());
				$maskedLabelYear.text($expYear.val());
				$maskedLabelAuthorName.text($authorName.val());
				$buttonEdit.hide();
				$buttonReEdit.show();
				$dispCreditInfo.show();

				$buttonReEdit.off();
				$buttonReEdit.click(function (e) {
					$tempData.remove();
					RakutenPaymentPayvault.initialize(cartIndex);
					e.preventDefault();
				});
			}
			else {
				$buttonEdit.show();
				$buttonReEdit.hide();
				$dispCreditInfo.hide();
			}
		});
	}

	function ableButton() {
		LoadingShow();

		var $cardName = $('input[id$=tbUserCreditCardName]');
		var $authorName = $('input[id$=tbCreditAuthorNameRakuten]');
		var isErrorCardName = $cardName.val() == false;
		var isErrorAuthorName = new RegExp(/^[a-zA-Z ]+$/).test($authorName.val()) == false;
		var isErrorCardToken = ($('[id$=hfCreditToken]').val() == false);
		var targetId = event.target.id;

		if (targetId != undefined) {
			if (targetId.match(/tbUserCreditCardName/)) {
				displayErrorMessage($cardName, $('span[id$=cvUserCreditCardNameRakuten]'), isErrorCardName, 'クレジットカード登録名は入力必須項目です。');
			}
			if (targetId.match(/tbCreditAuthorNameRakuten/)) {
				displayErrorMessage($authorName, $('span[id$=cvCreditAuthorNameRakuten]'), isErrorAuthorName, 'カード名義人は半角英字で入力して下さい。');
			}
			if (targetId.match(/lbEditCreditCardNoForRakutenToken/) || targetId.match(/lbEditCreditCardNoForToken/)) {
				isErrorCardToken = true;
			}
		}

		var isError = (isErrorCardName<% if (this.IsEdit == false) { %> || isErrorAuthorName || isErrorCardToken<% } %>);
		$('[id$=lbConfirm]').first().css('pointer-events', isError ? 'none' : '');

		LoadingHide();

		function displayErrorMessage($tb, $cv, isValid, errorMessage) {
			if (isValid) {
				$tb.addClass('error_input');
				$cv.css('visibility', 'visible').html(errorMessage);
			}
			else {
				$tb.removeClass('error_input');
				$cv.css('visibility', 'hidden');
			}
		}
	}
</script>
