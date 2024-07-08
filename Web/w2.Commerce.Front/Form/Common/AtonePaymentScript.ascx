<%--
=========================================================================================================
  Module      : Atone Payment Script(AtonePaymentScript.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/AtonePaymentScript.ascx.cs" Inherits="Form_Common_AtonePaymentScript" %>
<script type="text/javascript">
	var dataInfo = "";
	var isAuthoriesAtone = false;
	var dataAtone = {};
	var currentAtoneAuthoriesIndex = -1;
	var confirmButton;

	<%-- Write Error Message --%>
	function WriteErrorMessageAtone(response) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/WriteLogErrorAtone",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: response,
			async: false
		});
	}

	<%-- Write Success Message --%>
	function WriteSuccessMessageAtone(response) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/WriteLogSuccessAtone",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: response,
			async: false
		});
	}

	<%-- Atone Authories --%>
	function AtoneAuthories(index) {
		currentAtoneAuthoriesIndex = index;
		GetAtoneDataAuthories(index, function (response) {
			dataAtone = JSON.parse(response.d);

			if (dataAtone.data != "")
			{
				dataInfo = dataAtone.data;
				var token = GetCurrentAtoneToken();
				// Reset Atone Before Config
				<%= this.JavaScriptCode %>
				var configAtone = {
					pre_token: token,
					pub_key: '<%= Constants.PAYMENT_ATONE_APIKEY %>',
					payment: dataInfo,
					terminal_id: '<%= Constants.PAYMENT_ATONE_TERMINALID %>',
					// For case authentication
					authenticated: function (authentication_token) {
						SetAtoneTokenFromChildPage(authentication_token);
						token = authentication_token;
					},
					// For case cancel
					cancelled: function () {},
					// For case fail
					failed: function (response) {
						if ((typeof response !== 'undefined') && (response != null)) {
							var data = JSON.stringify({
								name: response.id,
								message: response.shop_name
							});
							var jsonData = JSON.stringify({ response: data });
							WriteErrorMessageAtone(jsonData);
						}
					},
					// For case success
					succeeded: function (response) {
						var data = JSON.stringify({
							id: response.id,
							authorization_result: response.authorization_result
						});
						var jsonData = JSON.stringify({ response: data });
						WriteSuccessMessageAtone(jsonData);

						SetAtoneTransactionId(currentAtoneAuthoriesIndex, response.id, isMyPage);

						// If Call From Order History => Close
						if ((typeof isPageConfirm === "undefined")
							|| (isPageConfirm == null)) {
							isAuthoriesAtone = true;
							PostBackConfirmAtone();
							return;
						}
						SetAtoneTransactionIdToCart(currentAtoneAuthoriesIndex, token, response.id);
						if (isLastItemCart)
						{
							ExecuteOrder();
						}
						GetAtoneAuthority();
					},
					// For case error
					error: function (name, message, errors) {
						var data = JSON.stringify({
							name: name,
							message: message,
							errors: errors
						});
						var jsonData = JSON.stringify({ response: data });
						WriteErrorMessageAtone(jsonData);
					}
				}

				Atone.config(configAtone, function () {
					Atone.sync();
					Atone.start();
				});
			}
			return false;
		});
	}

	<%-- Atone Authories For My page --%>
	function AtoneAuthoriesForMyPage(orderId, paymentId, btnConfirm) {
		confirmButton = btnConfirm;
		GetAtoneDataAuthoriesForMyPage(orderId, paymentId, function (response) {
			dataAtone = JSON.parse(response.d);

			if (dataAtone.data != "")
			{
				dataInfo = dataAtone.data;
				var token = GetCurrentAtoneToken();
				// Reset Atone Before Config
				<%= this.JavaScriptCode %>
				var configAtone = {
					pre_token: token,
					pub_key: '<%= Constants.PAYMENT_ATONE_APIKEY %>',
					payment: dataInfo,
					terminal_id: '<%= Constants.PAYMENT_ATONE_TERMINALID %>',
					// For case authentication
					authenticated: function (authentication_token) {
						SetAtoneTokenFromChildPage(authentication_token);
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
							WriteErrorMessageAtone(jsonData);
						}
					},
					// For case success
					succeeded: function (response) {
						var data = JSON.stringify({
							id: response.id,
							authorization_result: response.authorization_result
						});
						var jsonData = JSON.stringify({ response: data });
						WriteSuccessMessageAtone(jsonData);

						SetAtoneTransactionId(currentAtoneAuthoriesIndex, response.id, isMyPage);

						// If Call From Order History => Close
						if ((typeof isPageConfirm === "undefined")
							|| (isPageConfirm == null)) {
							isAuthoriesAtone = true;
							PostBackConfirmAtone();
							return;
						}
						SetAtoneTransactionIdToCart(currentAtoneAuthoriesIndex, token, response.id);
						if (isLastItemCart) {
							ExecuteOrder();
						}
						GetAtoneAuthority();
					},
					// For case error
					error: function (name, message, errors) {
						var data = JSON.stringify({
							name: name,
							message: message,
							errors: errors
						});
						var jsonData = JSON.stringify({ response: data });
						WriteErrorMessageAtone(jsonData);
					}
				}

				Atone.config(configAtone, function () {
					Atone.sync();
					Atone.start();
				});
			}
			return false;
		});
	}
	<%-- Get Data Authories --%>
	function GetAtoneDataAuthoriesForMyPage(orderId, paymentId, callback) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/CreateDataAtoneAfteeToken", // Must bind from code behind to get current url
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				orderId: orderId,
				isAtone: true,
				paymentId: paymentId
			}),
			async: false,
			success: callback
		});
	}

	<%-- Set Atone Transaction Id To Cart --%>
	function SetAtoneTransactionIdToCart(index, token, id) {
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

	<%-- Set Transaction for Atone --%>
	function SetAtoneTransactionId(index, id, isMyPage) {
		if ((typeof isMyPage === "undefined") || (isMyPage == null))
		{
			var className = "Atone_" + index;
			var element = $('.' + className + ' input[type=hidden]');
			$(element).val(id);
		}
		else
		{
			SetAtoneTransactionIdFromMypage(id);
		}

	}

	<%-- Get Data Authories --%>
	function GetAtoneDataAuthories(index, callback) {
		$.ajax({
			type: "POST",
			url: "<%= this.CurrentUrl %>/<%= this.IsLandingCartPage
				? "CreateDataAtoneAfteeTokenLanding"
				: "CreateDataAtoneAfteeToken" %>", // Must bind from code behind to get current url
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				index: index,
				isAtone: true
			}),
			async: false,
			success: callback
		});
	}

	<%-- Post Back Confirm Atone --%>
	function PostBackConfirmAtone() {
		if (typeof confirmButton !== "undefined" && confirmButton != null) {
			confirmButton.click();
		}
	}
</script>
