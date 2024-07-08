<%--
=========================================================================================================
  Module      : Amazon Pay詳細ウィジェット画面(AmazonPayDetailWidget.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/Form/Common/HeaderScriptDeclaration.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/AmazonPayDetailWidget.aspx.cs" Inherits="Form_Common_AmazonPayDetailWidget" %>
<%-- 各種Js読み込み --%>
<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server"></uc:HeaderScriptDeclaration>
<style>
</style>

<% if (this.IsAmazonLoggedIn) { %>

	<%-- ▼▼Amazonアドレス帳ウィジェット▼▼ --%>
	<div id="shippingAddressBookWidgetDiv" style="width:100%;height:150px;"></div>
	<%-- ▲▲Amazonアドレス帳ウィジェット▲▲ --%>

	<div style="margin: 10px;"></div>

	<%-- ▼▼Amazon決済ウィジェット▼▼ --%>
	<div id="walletWidgetDiv" style="width:100%;height:150px;"></div>
	<%-- ▲▲Amazon決済ウィジェット▲▲ --%>

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
	<input type="hidden" id="amazonState" value="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetCallbackUrl(this.IsSmartPhone, true, orderId: this.RequestOrderId, fixedPurchaseId: this.RequestFixedPurchaseId) %>" />

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

	window.onAmazonLoginReady = function () {
		amazon.Login.setClientId(w2amzn.clientId);
		if (w2amzn.isSmartPhone) amazon.Login.setUseCookie(true);
	};

	window.onAmazonPaymentsReady = function () {
		if (w2amzn.isLoggedIn == false) showAmazonPayButton();
		showAddressBookWidget();
		$('#AmazonDetailWidget', window.parent.document).load(function() {
			$('#AmazonDetailWidget', window.parent.document).height($('html').height());
		});
	};

	<%-- Amazonアドレス帳表示ウィジェット --%>
	function showAddressBookWidget() {
		var param = {
			sellerId: w2amzn.sellerId,
			onReady: function (arg) {
				showWalletWidget();
			},
			displayMode: 'Read',
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

		new OffAmazonPayments.Widgets.AddressBook(param)
			.bind("shippingAddressBookWidgetDiv");
	}

	<%-- Amazon決済方法表示ウィジェット --%>
	function showWalletWidget() {
		var param = {
			sellerId: w2amzn.sellerId,
			displayMode: 'Read',
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

	<%-- Amazonボタン表示ウィジェット --%>
	function showAmazonPayButton() {
		var authRequest;
		OffAmazonPayments.Button("AmazonPayButton", w2amzn.sellerId, {
			type: "LwA",
			color: "Gold",
			size: "medium",
			authorization: function () {
				loginOptions = { scope: "payments:widget payments:shipping_address profile", popup: (w2amzn.isSmartPhone === false), state: w2amzn.state};
				authRequest = amazon.Login.authorize(loginOptions, w2amzn.callbackUrl);
			},
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		});
	};

//# sourceURL=http://form/common/amazonpaydetailwidget.js
</script>
<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>