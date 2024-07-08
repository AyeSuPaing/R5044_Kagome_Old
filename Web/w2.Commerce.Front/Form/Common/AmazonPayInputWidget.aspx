<%--
=========================================================================================================
  Module      : Amazon Pay入力ウィジェット画面(AmazonPayInputWidget.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/Form/Common/HeaderScriptDeclaration.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/AmazonPayInputWidget.aspx.cs" Inherits="Form_Common_AmazonPayInputWidget" %>
<%-- 各種Js読み込み --%>
<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server"></uc:HeaderScriptDeclaration>

<% if (this.IsAmazonLoggedIn) { %>

	<%-- ▼▼Amazonアドレス帳ウィジェット▼▼ --%>
	<div id="shippingAddressBookWidgetDiv" style="width:100%;height:250px"></div>
	<%-- ▲▲Amazonアドレス帳ウィジェット▲▲ --%>

	<div id="shippingAddressBookErrorMessage" style="color:red;padding:5px;font-size:13px;" ClientIDMode="Static" runat="server"></div>

	<%-- ▼▼Amazon決済ウィジェット▼▼ --%>
	<div id="walletWidgetDiv" style="width:100%;height:250px"></div>
	<%-- ▲▲Amazon決済ウィジェット▲▲ --%>

	<% if (this.HasFixedPurchase) { %>
		<div style="margin: 10px 0;font-size: 13px;">下記のお支払い契約に同意してください。</div>
		<%-- ▼▼Amazon支払契約同意ウィジェット▼▼ --%>
		<div id="consentWidgetDiv" style="width:100%;height:105px;margin-top: 0.5em;"></div>
		<%-- ▲▲Amazon支払契約同意ウィジェット▲▲ --%>
	<% } %>

<% } else { %>
	<%-- ▼▼Amazonログインボタンウィジェット▼▼ --%>
	<div id="AmazonPayButton"></div>
	<%-- ▲▲Amazonログインボタンウィジェット▲▲ --%>
<% } %>

	<input type="hidden" id="isSmartPhone" value="<%: this.IsSmartPhone %>" />
	<input type="hidden" id="hasOrder" value="<%: this.HasOrder %>" />
	<input type="hidden" id="hasFixedPurchase" value="<%: this.HasFixedPurchase %>" />
	<input type="hidden" id="amazonClientId" value="<%: Constants.PAYMENT_AMAZON_CLIENTID %>" />
	<input type="hidden" id="amazonSellerId" value="<%: Constants.PAYMENT_AMAZON_SELLERID %>" />
	<input type="hidden" id="amazonIsLoggedIn" value="<%: this.IsAmazonLoggedIn %>" />
	<input type="hidden" id="amazonBillingAgreementId" value="<%: this.HasFixedPurchase ? this.FixedPurchaseModel.ExternalPaymentAgreementId : "" %>" />
	<input type="hidden" id="amazonOrderReferenceId" value="<%: this.HasOrder ? this.OrderModel.PaymentOrderId : "" %>" />
	<input type="hidden" id="amazonCallbackUrl" value="<%: w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_AMAZON_PAY_WIDGET_CALLBACK) %>" />
	<input type="hidden" id="amazonState" value="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetCallbackUrl(this.IsSmartPhone, false, orderId: this.RequestOrderId, fixedPurchaseId: this.RequestFixedPurchaseId) %>" />
	<input type="hidden" id="amazonGetAmazonAddressUrl" value="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_COMMON_AMAZON_PAY_INPUT_WIDGET %>/GetAmazonAddress" />

<%-- ▼▼Amazonウィジェット用スクリプト▼▼ --%>
<script type="text/javascript">
	var w2amzn = w2amzn || {};
	w2amzn.isSmartPhone = ($('#isSmartPhone').val() === 'True');
	w2amzn.hasOrder = ($('#hasOrder').val() === 'True');
	w2amzn.hasFixedPurchase = ($('#hasFixedPurchase').val() === 'True');
	w2amzn.clientId = $('#amazonClientId').val();
	w2amzn.sellerId = $('#amazonSellerId').val();
	w2amzn.isLoggedIn = ($('#amazonIsLoggedIn').val() === 'True');
	w2amzn.orderReferenceId = $('#amazonOrderReferenceId').val();
	w2amzn.billingAgreementId = $('#amazonBillingAgreementId').val();
	w2amzn.callbackUrl = $('#amazonCallbackUrl').val();
	w2amzn.state = $('#amazonState').val();
	w2amzn.getAmazonAddressUrl = $('#amazonGetAmazonAddressUrl').val();

	window.onAmazonLoginReady = function () {
		amazon.Login.setClientId(w2amzn.clientId);
		if (w2amzn.isSmartPhone) amazon.Login.setUseCookie(true);
	};

	window.onAmazonPaymentsReady = function () {
		if (w2amzn.isLoggedIn == false) showAmazonPayButton();
		showAddressBookWidget();
		$('#AmazonInputWidget', window.parent.document).load(function() {
			$('#AmazonInputWidget', window.parent.document).height($('html').height());
		});
	};

	<%-- Amazonアドレス帳表示ウィジェット --%>
	function showAddressBookWidget() {

		var param = {
			sellerId: w2amzn.sellerId,
			onAddressSelect: function (arg) {
				var $shippingAddressBookErrorMessage = $('#shippingAddressBookErrorMessage');
				$shippingAddressBookErrorMessage.empty();
				getAmazonAddress(
					w2amzn.orderReferenceId,
					w2amzn.billingAgreementId,
					w2amzn.hasFixedPurchase
						? '<%= w2.App.Common.Amazon.AmazonConstants.OrderType.AutoPay %>'
						: '<%= w2.App.Common.Amazon.AmazonConstants.OrderType.OneTime %>'
					,
					function (response) {
						var data = JSON.parse(response.d);
						if (data.Error) $shippingAddressBookErrorMessage.html(data.Error);
					});
			},
			design: { designMode: 'responsive' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		};

		if (w2amzn.hasFixedPurchase) {
			param.agreementType = 'BillingAgreement';
			param.onReady = function (arg) {
				w2amzn.billingAgreementId = arg.getAmazonBillingAgreementId();
				$('#hfAmazonBillingAgreementId', window.parent.document).val(w2amzn.billingAgreementId);
				showWalletWidget();
				showConsentWidget();
			}
		}
		if (w2amzn.hasOrder) {
			param.onReady = function (arg) {
				w2amzn.orderReferenceId = arg.getAmazonOrderReferenceId();
				$('#hfAmazonOrderRefID', window.parent.document).val(w2amzn.orderReferenceId);
				showWalletWidget();
			}
		}

		new OffAmazonPayments.Widgets.AddressBook(param)
			.bind("shippingAddressBookWidgetDiv");
	}

	<%-- Amazon決済方法表示ウィジェット --%>
	function showWalletWidget() {
		var param = {
			sellerId: w2amzn.sellerId,
			design: { designMode: 'responsive' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		};

		if (w2amzn.hasFixedPurchase) {
			param.agreementType = 'BillingAgreement';
			param.amazonBillingAgreementId = w2amzn.billingAgreementId;
		}
		if (w2amzn.hasOrder) {
			param.amazonOrderReferenceId = w2amzn.orderReferenceId;
		}

		new OffAmazonPayments.Widgets.Wallet(param)
			.bind("walletWidgetDiv");
	}

	<%-- Amazon支払契約同意ウィジェット --%>
	function showConsentWidget() {
		new OffAmazonPayments.Widgets.Consent({
			sellerId: w2amzn.sellerId,
			amazonBillingAgreementId: w2amzn.billingAgreementId,
			onConsent: function (billingAgreementConsentStatus) {
				buyerBillingAgreementConsentStatus = billingAgreementConsentStatus.getConsentStatus();
				if (buyerBillingAgreementConsentStatus) {
					$('#constraintErrorMessage', window.parent.document).empty();
				}
			},
			design: { designMode: 'responsive' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		}).bind("consentWidgetDiv");
	}

	<%-- Amazon住所取得関数 --%>
	function getAmazonAddress(orderReferenceId, billingAgreementId, orderType, callback) {
		$.ajax({
			type: "POST",
			url: w2amzn.getAmazonAddressUrl,
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				orderReferenceIdOrBillingAgreementId: orderReferenceId || billingAgreementId,
				orderType: orderType
			}),
			success: callback
		});
	}

	<%-- Amazonボタン表示ウィジェット --%>
	function showAmazonPayButton() {
		var authRequest;
		OffAmazonPayments.Button("AmazonPayButton", w2amzn.sellerId, {
			type: "LwA",
			color: "Gold",
			size: "medium",
			authorization: function () {
				loginOptions = { scope: "payments:widget payments:shipping_address profile", popup: (w2amzn.isSmartPhone === false), state: w2amzn.state };
				authRequest = amazon.Login.authorize(loginOptions, w2amzn.callbackUrl);
			},
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		});
	};

//# sourceURL=http://form/common/amazonpayinputwidget.js
</script>
<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
