<%--
=========================================================================================================
  Module      : AmazonPayCv2住所変更遷移画面(AmazonPayCv2ChangeActionRedirect.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AmazonPayCv2ChangeActionRedirect.aspx.cs" Inherits="Form_OrderHistory_AmazonPayCv2_AmazonPayCv2ChangeActionRedirect" %>
ページが自動的に遷移しない場合は<input type="button" id="btnChangeAddress" value="こちら" onclick="return false;" />をクリックしてください。
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<script type="text/javascript" charset="utf-8">
	amazon.Pay.bindChangeAction('#btnChangeAddress',
		{
			amazonCheckoutSessionId: '<%= this.AmazonCheckoutSessionId %>',
			changeAction: 'changeAddress'
		});
	document.getElementById("btnChangeAddress").click();
</script>
