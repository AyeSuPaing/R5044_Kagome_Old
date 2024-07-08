<%--
=========================================================================================================
  Module      : Receiving Store For EC Pay Response(ReceivingStoreForEcPayResponse.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Order.Payment.ECPay" %>
<%@ Import Namespace="w2.App.Common.Pdf" %>
<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" ValidateRequest="False" %>

<script>
	// Set convenience store ecpay data
	<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
		&& (string.IsNullOrEmpty(this.Request.Form[ECPayConstants.PARAM_MERCHANT_ID]) == false)) { %>

	// Set Convenience Store Ec Pay Data
	window.opener.setConvenienceStoreEcPayData(
		'<%= this.Request.Form[ECPayConstants.PARAM_CVSSTOREID] %>',
		'<%= this.Request.Form[ECPayConstants.PARAM_CVSSTORENAME] %>',
		'<%= this.Request.Form[ECPayConstants.PARAM_CVSADDRESS] %>',
		'<%= this.Request.Form[ECPayConstants.PARAM_CVSTELEPHONE] %>');
	<% } %>

	window.close();
</script>