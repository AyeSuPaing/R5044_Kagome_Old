<%--
=========================================================================================================
  Module      : PayPalログインスクリプト (PayPalScriptsForm.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="PayPalScriptsFormBase" %>

<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
<asp:HiddenField id="hfPayPalNonce" runat="server"/>
<asp:HiddenField id="hfPayPalPayerId" runat="server"/>
<asp:HiddenField id="hfPayPalDeviceData" runat="server"/>
<asp:HiddenField id="hfPayPalShippingAddress" runat="server"/>

<!-- Load the required components. -->
<script src="https://www.paypalobjects.com/api/checkout.min.js"></script>
<script src="https://js.braintreegateway.com/web/3.28.1/js/client.min.js"></script>
<script src="https://js.braintreegateway.com/web/3.28.1/js/paypal-checkout.min.js"></script>
<script src="https://js.braintreegateway.com/web/3.28.1/js/data-collector.min.js"></script>
<script type="text/javascript">
	var CLIENT_AUTHORIZATION = '<%= this.ClientToken %>';

	// PayPal初期化
	function InitializePaypal(sender, args) {

		if (!$("#paypal-button").length) return;
		$("#paypal-button").html("");

		braintree.client.create({
			authorization: CLIENT_AUTHORIZATION
		}, function (clientErr, clientInstance) {

			// フォーカスそのままにしておくとバグるので退避
			var $focused = $(':focus');
			// ブラウザもアクティブにしておかないとバグる
			$(window).focus();
			for (var i = 0; i < $focused.length; i++) {
				$focused[i].blur();
			}

			if (clientErr) {
				console.error('Error creating client:', clientErr);
				return;
			}

			braintree.dataCollector.create({
				client: clientInstance,
				paypal: true
			}, function(err, dataCollectorInstance) {
				if (err) {
					return;
				}
				$("#<%= hfPayPalDeviceData.ClientID %>").val(dataCollectorInstance.deviceData);
			});

			braintree.paypalCheckout.create({
				client: clientInstance
			}, function (paypalCheckoutErr, paypalCheckoutInstance) {

				if (paypalCheckoutErr) {
					console.error('Error creating PayPal Checkout:', paypalCheckoutErr);
					return;
				}
				paypal.Button.render({
					env: '<%= Constants.PAYMENT_PAYPAL_GATEWAY.Environment.EnvironmentName.ToLower() %>',
					commit: false,
					<%if (this.LogoDesign == "Login") {%>
					style: {
						label: 'paypal',
						size: 'responsive',    // small | medium | large | responsive
						shape: 'rect',     // pill | rect
						color: 'silver',     // gold | blue | silver | black
						tagline: false
					},
					<%} %>
					<%if (this.LogoDesign == "Payment") {%>
					style: {
						label: 'paypal',
						shape: 'pill',     // pill | rect
						color: 'gold',     // gold | blue | silver | black
						tagline: false
					},
					<%} %>
					<%if (this.LogoDesign == "Cart") {%>
					style: {
						label: 'paypal',
						size: 'responsive',    // small | medium | large | responsive
						shape: 'rect',     // pill | rect
						color: 'gold',     // gold | blue | silver | black
						tagline: true
					},
					<%} %>
					payment: function () {
						return paypalCheckoutInstance.createPayment({
							flow: 'vault',
							amount: 0,
							billingAgreementDescription: 'こちらのアカウントで支払いの登録を行います。（まだ決済はされません）',
							displayName: '<%= ShopMessage.GetMessage("ShopName") %>',
							enableShippingAddress: <%= this.GetShippingAddress.ToString().ToLower() %>
							<%if (this.GetShippingAddress && this.IsLoggedIn) { %>
								,shippingAddressOverride: {
									line1 : '<%= this.LoginUser.Addr3 %>',
									line2 : '<%= this.LoginUser.Addr4 %>',
									city : '<%= this.LoginUser.Addr2 %>',
									state : '<%= (this.LoginUser.AddrCountryIsoCode == "JP") ? this.LoginUser.Addr1 : this.LoginUser.Addr5 %>',
									postalCode : '<%= this.LoginUser.Zip %>',
									countryCode : '<%= this.LoginUser.AddrCountryIsoCode %>'
							}
							<%} %>
						});
					},

					onAuthorize: function (data, actions) {
						return paypalCheckoutInstance.tokenizePayment(data)
							.then(function (payload) {
								paypalPostAction(payload);
							});
					},

					onCancel: function (data) {
						console.log('checkout.js payment cancelled', JSON.stringify(data, 0, 2));
					},

					onError: function (err) {
						alert("onError" + err);
						console.error('checkout.js error', err);
					}
				}, '#paypal-button').then(function() {
					// レンダのコールバックで退避したフォーカスセット
					// ブラウザもアクティブにしておかないとバグる
					$(window).focus();
					for (var i = 0; i < $focused.length; i++) {
						$focused[i].blur();
						$focused[i].focus();
					}
					// The PayPal button will be rendered in an html element with the id
					// `paypal-button`. This function will be called when the PayPal button
					// is set up and ready to be used.
				});

			});
		});
	}

	// PayPal POST
	var paypalPostAction = function(payload) {
		$("#<%= hfPayPalNonce.ClientID %>").val(payload.nonce);
		$("#<%= hfPayPalPayerId.ClientID %>").val(payload.details.payerId);
		$("#<%= hfPayPalShippingAddress.ClientID %>").val(
			JSON.stringify({
				email: payload.details.email,
				firstName: payload.details.firstName,
				lastName: payload.details.lastName,
				phone: payload.details.phone,
				recipientName: payload.details.shippingAddress ? payload.details.shippingAddress.recipientName : "",
				line1: payload.details.shippingAddress ? payload.details.shippingAddress.line1 : "",
				line2: payload.details.shippingAddress ? payload.details.shippingAddress.line2 : "",
				city: payload.details.shippingAddress ? payload.details.shippingAddress.city : "",
				state: payload.details.shippingAddress ? payload.details.shippingAddress.state : "",
				postalCode: payload.details.shippingAddress ? payload.details.shippingAddress.postalCode : "",
				countryCode: payload.details.shippingAddress ? payload.details.shippingAddress.countryCode : ""
			}));
		<% if (this.AuthCompleteActionControl != null) { %>
			<%-- setTimeoutしないとIEでPostBackが実行できない --%>
			setTimeout(function() { <%= this.Page.ClientScript.GetPostBackEventReference(this.AuthCompleteActionControl, String.Empty) %> }, 100);
		<% }%>
	}
</script>

<%--
<a href="javascript:paypalTest();">ペイパルTEST</a>
<script type="text/javascript">
	// テスト用
	var paypalTest = function() {
		$("#<%= hfPayPalNonce.ClientID %>").val("PAYPALTEST");
	$("#<%= hfPayPalPayerId.ClientID %>").val("Z7PSNJHGQRNNY");
	$("#<%= hfPayPalShippingAddress.ClientID %>").val(
	JSON.stringify({
	email: "<%= string.Format("bh+{0}@w2s.xyz", DateTime.Now.ToString("yyMMddHHmmss")) %>",
	firstName: "だぶるつ",
	lastName: "テスト",
	phone: "00-1111-2222",
	recipientName: "なんだっけ",
	line1: "1234-5678",
	line2: "KINGSMAN",
	city: "ゴッサムシティ",
	state: "ニューヨーク",
	postalCode: "152-0003",
	countryCode: "JPN"
	}));
	<% if (this.HasAuthCompleteActionPostBack) { %>
		setTimeout(function() { <%= this.Page.ClientScript.GetPostBackEventReference(this.AuthCompleteActionControl, String.Empty) %> }, 100);
	<% }%>
	}
</script>
--%>

<%} %>