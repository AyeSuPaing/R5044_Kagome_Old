<%--
=========================================================================================================
  Module      : Aftee Payment Script(AfteePaymentScript.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/AfteePaymentScript.ascx.cs" Inherits="Form_Common_AfteePaymentScript" %>
<script type="text/javascript">
	var dataInfo = "";
	var isAuthoriesAftee = false;
	var dataAftee = {};
	var currentAfteeAuthoriesIndex = -1;
	var confirmButton;

	<%-- Write Error Message --%>
	function WriteErrorMessageAftee(response) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/WriteLogErrorAftee",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: response,
			async: false
		});
	}

	<%-- Write Success Message --%>
	function WriteSuccessMessageAftee(response) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/WriteLogSuccessAftee",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: response,
			async: false
		});
	}

	<%-- Aftee Authories --%>
	function AfteeAuthories(index) {
		currentAfteeAuthoriesIndex = index;
		GetAfteeDataAuthories(index, function (response) {
			dataAftee = JSON.parse(response.d);

			if (dataAftee.data != "")
			{
				dataInfo = dataAftee.data;
				var token = GetCurrentAfteeToken();
				// Reset Atone Before Config
				<%= this.JavaScriptCode %>
				var configAftee = {
					pre_token: token,
					pub_key: '<%= Constants.PAYMENT_AFTEE_APIKEY %>',
					payment: dataInfo,
					terminal_id: '<%= Constants.PAYMENT_AFTEE_TERMINALID %>',
					// For case authentication
					authenticated: function (authentication_token) {
						SetAfteeTokenFromChildPage(authentication_token);
						token = authentication_token;
					},
					// For case cancel
					cancelled: function () { },
					// For case fail
					failed: function (response) {
						if ((typeof response !== 'undefined') && (response != null))
						{
							var data = JSON.stringify({
								name: response.id,
								message: response.shop_name
							});
							var jsonData = JSON.stringify({ response: data });
							WriteErrorMessageAftee(jsonData);
						}
					},
					// For case success
					succeeded: function (response) {
						var data = JSON.stringify({ id: response.id });
						var jsonData = JSON.stringify({ response: data });
						WriteSuccessMessageAftee(jsonData);

						SetAfteeTransactionId(currentAfteeAuthoriesIndex, response.id, isMyPage);
						// If Call From Order History => Close
						if ((typeof isPageConfirm === "undefined")
							|| (isPageConfirm == null))
						{
							isAuthoriesAftee = true;
							PostBackConfirmAftee();
							return;
						}
						// Set Aftee Transaction Id To Cart And Execute Order
						SetAfteeTransactionIdToCart(currentAfteeAuthoriesIndex, token, response.id);
						if (isLastItemCart)
						{
							ExecuteOrder();
						}
						GetAfteeAuthority();
					},
					// For case error
					error: function (name, message, errors) {
						var data = JSON.stringify({
							name: name,
							message: message,
							errors: errors
						});
						var jsonData = JSON.stringify({ response: data });
						WriteErrorMessageAftee(jsonData);
					}
				}

				// Config Aftee
				Aftee.config(configAftee, function () {
					Aftee.sync();
					Aftee.start();
				});
			}
			return false;
		});
	}

	<%-- Aftee Authories For My page --%>
	function AfteeAuthoriesForMyPage(orderId, paymentId, btnConfirm) {
		confirmButton = btnConfirm;
		GetAfteeDataAuthoriesForMyPage(orderId, paymentId, function (response) {
			dataAftee = JSON.parse(response.d);

			if (dataAftee.data != "")
			{
				dataInfo = dataAftee.data;
				var token = GetCurrentAfteeToken();
				// Reset Atone Before Config
				<%= this.JavaScriptCode %>
				var configAftee = {
					pre_token: token,
					pub_key: '<%= Constants.PAYMENT_AFTEE_APIKEY %>',
					payment: dataInfo,
					terminal_id: '<%= Constants.PAYMENT_AFTEE_TERMINALID %>',
					// For case authentication
					authenticated: function (authentication_token) {
						SetAfteeTokenFromChildPage(authentication_token);
						token = authentication_token;
					},
					// For case cancel
					cancelled: function () { },
					// For case fail
					failed: function (response) {
						if ((typeof response !== 'undefined') && (response != null)) {
							var data = JSON.stringify({
								name: response.id,
								message: response.shop_name
							});
							var jsonData = JSON.stringify({ response: data });
							WriteErrorMessageAftee(jsonData);
						}
					},
					// For case success
					succeeded: function (response) {
						var data = JSON.stringify({ id: response.id });
						var jsonData = JSON.stringify({ response: data });
						WriteSuccessMessageAftee(jsonData);

						SetAfteeTransactionId(currentAfteeAuthoriesIndex, response.id, isMyPage);
						// If Call From Order History => Close
						if ((typeof isPageConfirm === "undefined")
							|| (isPageConfirm == null)) {
							isAuthoriesAftee = true;
							PostBackConfirmAftee();
							return;
						}
						// Set Aftee Transaction Id To Cart And Execute Order
						SetAfteeTransactionIdToCart(currentAfteeAuthoriesIndex, token, response.id);
						if (isLastItemCart) {
							ExecuteOrder();
						}
						GetAfteeAuthority();
					},
					// For case error
					error: function (name, message, errors) {
						var data = JSON.stringify({
							name: name,
							message: message,
							errors: errors
						});
						var jsonData = JSON.stringify({ response: data });
						WriteErrorMessageAftee(jsonData);
					}
				}

				// Config Aftee
				Aftee.config(configAftee, function () {
					Aftee.sync();
					Aftee.start();
				});
			}
			return false;
		});
	}

	<%-- Set Aftee Transaction Id To Cart --%>
	function SetAfteeTransactionIdToCart(index, token, id) {
		var data = JSON.stringify({
			index: index,
			token: token,
			id: id
		});
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/<%= this.IsLandingCartPage
				? "SetTransactionIdToCartLanding"
				: "SetTransactionIdToCart" %>", // Must bind from code behind to get current url
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: data,
			async: false
		});
	}

	<%-- Set Transaction for Aftee --%>
	function SetAfteeTransactionId(index, id, isMyPage) {
		if ((typeof isMyPage === "undefined") || (isMyPage == null))
		{
			var className = "Aftee_" + index;
			var element = $('.' + className + ' input[type=hidden]');
			$(element).val(id);
		}
		else
		{
			SetAfteeTransactionIdFromMypage(id);
		}
	}

	<%-- Get Data Authories --%>
	function GetAfteeDataAuthories(index, callback) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/<%= this.IsLandingCartPage
				? "CreateDataAtoneAfteeTokenLanding"
				: "CreateDataAtoneAfteeToken" %>", // Must bind from code behind to get current url
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				index: index,
				isAtone: false
			}),
			async: false,
			success: callback
		});
	}

	<%-- Get Data Authories --%>
	function GetAfteeDataAuthoriesForMyPage(orderId, paymentId, callback) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/CreateDataAtoneAfteeToken", // Must bind from code behind to get current url
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				orderId: orderId,
				isAtone: false,
				paymentId: paymentId
			}),
			async: false,
			success: callback
		});
	}

	<%-- Post Back Confirm Aftee --%>
	function PostBackConfirmAftee() {
		if (typeof confirmButton !== "undefined" && confirmButton != null) {
			confirmButton.click();
		}
	}
</script>
