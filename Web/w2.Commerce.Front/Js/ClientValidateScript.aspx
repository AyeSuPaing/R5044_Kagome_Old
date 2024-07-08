<%--
=========================================================================================================
  Module      : クライアント検証スクリプト出力画面(ClientValidateScript.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" ContentType="application/javascript"%>
<%@ OutputCache Duration="300" VaryByParam="*" %>
<%@ Import Namespace="w2.App.Common.Global.Region" %>

// クライアント検証
function ClientValidate(cvalidator, e)
{
	// Replaces delimiter in phone number format if phone number has contain delimiter
	replaceAHyphenInTelephone(cvalidator.controltovalidate, e);

	var checked_message_html = "";
	var languageLocaleId = getParamFromCss(cvalidator, '<%= Constants.CLIENTVALIDATE_CSS_HEAD_LANGUAGE_LOCALE_ID %>');
	var countyIsoCode = getParamFromCss(cvalidator, '<%= Constants.CLIENTVALIDATE_CSS_HEAD_COUNTRY_ISO_CODE %>');

	//alert($("#" + cvalidator.controltovalidate).attr('class'));

	$.ajax({
		url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_VALIDATE_MODULE %>",
		type: "post",
		data: {
			'group': cvalidator.validationGroup,
			'control': document.getElementById(cvalidator.controltovalidate).name,
			'value': e.Value,
			'languageLocaleId' : languageLocaleId,
			'countyIsoCode' : countyIsoCode
		},
		async: false,
		success: function(data)
		{
			exec_submit_flg = 0;	// 二重押しチェック用
			
			checked_message_html = data.replace('<%= Constants.CONST_INPUT_ERROR_XML_HEADER %>', '').replace("@@ 1 @@", "： " + e.Value);
			if (checked_message_html.length != 0)
			{
				e.IsValid = false;
				
				document.getElementById(cvalidator.id).innerHTML = checked_message_html;
				
				if (document.getElementById(cvalidator.controltovalidate).className.match(/<%= Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING %>/i) == null)
				{
					document.getElementById(cvalidator.controltovalidate).className += "<%= Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING %>";
				}

				// iosのSafariのfocus()の代用処理
				var isIosSafari =
					((navigator.userAgent.toLowerCase().indexOf("iphone") > -1)
					|| (navigator.userAgent.toLowerCase().indexOf("ipad") > -1))
					&& (navigator.userAgent.toLowerCase().indexOf("safari") > -1);
				if (isIosSafari && (cvalidator.focusOnError == 't')){
					var controllOffset = window.pageYOffset + document.getElementById(cvalidator.id).getBoundingClientRect().top;
					if(window.pageYOffset > controllOffset)
					{
						document.getElementById(cvalidator.id).scrollIntoView();
					}
				}

				resetErrorMessage(cvalidator);
			}
			else
			{
				e.IsValid = true;

				document.getElementById(cvalidator.id).innerHTML = "";
				
				document.getElementById(cvalidator.controltovalidate).className =
					document.getElementById(cvalidator.controltovalidate).className.replace("<%= Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING %>", "");
			}
		},
		error: function(data)
		{
			e.IsValid = false;
			
			checked_message_html = "システムエラーが発生しました";			
			document.getElementById(cvalidator.id).innerHTML = checked_message_html;
		}
	});

	function getParamFromCss(cvalidator, header)
	{
		if ($("#" + cvalidator.controltovalidate).attr('class'))
		{
			var myregex = new RegExp(" " + header + ":.+? ");
			var match = $("#" + cvalidator.controltovalidate).attr('class').match(myregex);
			if (match) return match[0].replace(header + ":", "").trim();
		}
		return "";
	}

	// Replaces delimiter in phone number format if phone number has contain delimiter
	function replaceAHyphenInTelephone(id, e) {
		$(".shortTel").each(function (index, element) {
			var tel = $(".shortTel")[index].value;
			if ((tel != undefined) && (tel != "")) {
				if ((tel.split("-").length <= 2) && (id == element.id)) {
					var value = e.Value.replace("-", "");
					$("#" + element.id).val(value);
					e.Value = value;
				}
				if ((tel.split("ｰ").length <= 2) && (id == element.id)) {
					var value = e.Value.replace("ｰ", "");
					$("#" + element.id).val(value);
					e.Value = value;
				}
			}
		});
	}

	// Reset the error message
	function resetErrorMessage(customValidator) {
		if (customValidator.id.toLowerCase().indexOf("zip") > -1) {
			if (customValidator.classList.length >= 2) {
				switch (customValidator.classList[1]) {
					case "zip_input_error_message":
						hideErrorMessage(cvalidator.className, 'shortZipInputErrorMessage');
						break;

					case "cvOwnerZipShortInput":
						hideErrorMessage(cvalidator.className, 'sOwnerZipError');
						break;

					case "cvShippingZipShortInput":
						hideErrorMessage(cvalidator.className, 'sShippingZipError');
						break;

					case "cvSenderZipShortInput":
						hideErrorMessage(cvalidator.className, 'sSenderZipError');
						break;

					case "cvShippingZipShortInput":
						hideErrorMessage(cvalidator.className, 'sShippingZipError');
				}
			}
		}
	}

	// Hide unnecessary error messages
	function hideErrorMessage(classNameCustomValidate, classNameZip) {
		var zipErrorMessage = document.getElementsByClassName(classNameCustomValidate);
		if (zipErrorMessage[0].innerHTML !== "")
		{
			if (document.getElementsByClassName(classNameZip)[0].innerHTML !== "")
			document.getElementsByClassName(classNameZip)[0].innerHTML = "";
		}
	}
}

// Bind remove custom validator error on input change value
function bindRemoveCustomValidateErrorOnInputChangeValue(controls) {
	if ((controls === undefined) || (typeof controls !== "object")) return;

	for (var i = 0; i < controls.length; i++) {
		// Get input control and custom validate control ids (separated by one space)
		var controlInfos = controls[i].split(' ');
		if ((controlInfos.length !== 2) || (controlInfos[0] === '')) continue;

		var inputControl = $('#' + controlInfos[0]);
		var errorClass = '<%= Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING.Trim() %>';
		if (inputControl.length) {
			// Remove class custom error from input control (when the page is post back event)
			if ($(inputControl).hasClass(errorClass)) {
				$(inputControl).removeClass(errorClass)
			}

			// Set attribute custom validate id (use for finding and removing error message)
			$(inputControl).data('custom-validator-control-id', controlInfos[1]);
			$(inputControl).bind('change', function () {
				// Remove class custom error from input control (when the input change value)
				if ($(inputControl).hasClass(errorClass)) {
					$(inputControl).removeClass(errorClass)
				}

				// Remove display error text
				if ($(this).data('custom-validator-control-id')) {
					// Get custom validator ids (separated by plus(+))
					var customValidatorIds = $(this).data('custom-validator-control-id').split('+');
					for (var j = 0; j < customValidatorIds.length; j++) {
						var customValidatorControl = $('#' + customValidatorIds[j]);
						if (customValidatorControl.length) {
							$(customValidatorControl).html('');
						}
					}
				}
			});
		}
	}
}

// Bind remove custom validate error when no error display
function bindRemoveCustomValidateErrorWhenNoErrorDisplay(controls) {
	if ((controls === undefined) || (typeof controls !== "object")) return;

	for (var i = 0; i < controls.length; i++) {
		// Get input control and custom validate control ids (separated by one space)
		var controlInfos = controls[i].split(' ');
		if ((controlInfos.length !== 2) || (controlInfos[0] === '')) continue;

		var inputControl = $('#' + controlInfos[0]);
		var errorClass = '<%= Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING.Trim() %>';
		if (inputControl.length) {

			// Set attribute custom validate id (use for finding and removing error message)
			$(inputControl).data('custom-validator-control-id', controlInfos[1]);

			// Remove display error text
			if ($(inputControl).data('custom-validator-control-id')) {
				// Get custom validator ids (separated by plus(+))
				var customValidatorIds = $(inputControl).data('custom-validator-control-id').split('+');
				for (var j = 0; j < customValidatorIds.length; j++) {
					var customValidatorControl = $('#' + customValidatorIds[j]);
					if ($(customValidatorControl).css("visibility") === 'hidden') {
						// Remove error message
						$(customValidatorControl).empty();
						// Remove class custom error from input control
						if ($(inputControl).hasClass(errorClass)) {
							$(inputControl).removeClass(errorClass)
						}
					} else if (($(customValidatorControl).length == 0)
						&& ($(inputControl).prev().is("select")
						&& ($(inputControl).val() !== ''))) {

						// Remove class custom error from input control when the drop down list control has value and no error display
						// Sush as: ddlOwnerBirthDay, ...
						if ($(inputControl).hasClass(errorClass)) {
							$(inputControl).removeClass(errorClass)
						}
					}
				}
			}
		}
	}
}

var validators = {};
// Client validate for order shipping select page
function ClientValidateForOrderShippingSelectPage(cvalidator, e) {
	ClientValidate(cvalidator, e);
	let hiddenIndex = $(cvalidator.closest('.orderBoxLarge')).find('.hiddenIndex');
	let cartBox = $(cvalidator.closest('.cartBox')).find('.invalidInputError');
	let control = cvalidator.id + '|' + hiddenIndex[0].innerHTML;
	let isGift = hiddenIndex[0].getAttribute("gift") == "True";

	if (isGift == false) {
		cartBox.css("display", "none");
		return
	}
	let cartBoxId = cartBox[0].id;
	if (validators[cartBoxId] == undefined) {
		validators[cartBoxId] = [];
	}
	if (e.IsValid == false) {
		validators[cartBoxId].push(control);
	} else {
		validators[cartBoxId] = validators[cartBoxId]
			.filter((value) => value !== control);
	}
	if (validators[cartBoxId].length > 0) {
		cartBox[0].innerHTML = getValidatorErrorMessage(cartBoxId);
		cartBox[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
	} else {
		cartBox[0].innerHTML = '';
	}
}

// Get validator error message
function getValidatorErrorMessage(cartBoxId) {
	let groupValidatorsForCheck = validators[cartBoxId];
	let subErrorMessages = [];
	groupValidatorsForCheck.forEach((value) => {
		let splitedControls = value.split('|');
		let subErrorMessage = '配送先' + splitedControls[1] + 'で入力に不備があります';
		if (subErrorMessages.includes(subErrorMessage) == false) {
			subErrorMessages.push(subErrorMessage);
		}
	});
	let errorMessage = subErrorMessages.sort().join('<br />');
	return errorMessage;
}
