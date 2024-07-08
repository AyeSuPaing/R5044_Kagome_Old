<!--
=========================================================================================================
Module      : GMO3Dセキュア認証データ送信ページ処理(PostCard3DSecureAuth.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostCard3DSecureAuth.aspx.cs" Inherits="Payment_GMO_PostCard3DSecureAuth" %>

<html>
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=Windows-31J">
</head>

<body Onload="OnLoadEvent();">
<form name="ACSCall" action="<%: WebSanitizer.UrlAttrHtmlEncode(this.Card3DSecureAuthUrl) %>" method="POST">
	<input type="hidden" name="PaReq" value="<%: this.Card3DSecureAuthKey %>">
	<input type="hidden" name="TermUrl" value="<%: this.ReturnUrl %>">
	<input type="hidden" name="MD" value="<%: this.Card3DSecureTranId %>">
</form>
<script type="text/javascript">
	function OnLoadEvent() {
		document.ACSCall.submit();
	}
</script>
</body>
</html>
