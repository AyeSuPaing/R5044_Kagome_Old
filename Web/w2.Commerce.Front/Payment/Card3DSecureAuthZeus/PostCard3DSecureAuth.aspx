<!--
=========================================================================================================
  Module      : ゼウス3Dセキュア認証データ送信ページ処理(PostCard3DSecureAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostCard3DSecureAuth.aspx.cs" Inherits="Payment_Card3DSecureAuthZeus_PostCard3DSecureAuth" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
	<title></title>
</head>
<body
	onload="<%= (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2 == false)
		? "dodument.postdata.submit()"
		: string.Format(
			"setPareqParams('{0}','{1}','{2}','{3}','{4}')",
			this.Card3DSecureTranId,
			this.Card3DSecureAuthKey,
			this.ReturnUrl,
			this.Card3DSecure2Flag,
			this.Card3DSecureIframeUrl) %>" >
	<form name="postdata" method="POST" action="<%: this.Card3DSecureAuthUrl %>" >
		<input type="hidden" name="MD" value="<%= WebSanitizer.HtmlEncode(this.Card3DSecureTranId) %>" />
		<input type="hidden" name="PaReq" value="<%= WebSanitizer.HtmlEncode(this.Card3DSecureAuthKey) %>" />
		<input type="hidden" name="TermUrl" value="<%= WebSanitizer.HtmlEncode(this.ReturnUrl) %>" />
		<input type="hidden" name="threeDSMethod" value="<%= WebSanitizer.HtmlEncode(this.Card3DSecure2Flag) %>" />
		<input type="hidden" name="iframeUrl" value="<%= WebSanitizer.HtmlEncode(this.Card3DSecureIframeUrl) %>" />
	</form>

	<% if (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2) { %>
		<div id="challenge_wait">ただいま決済処理中です。<br />画面が切り替わるまでそのままお待ちください . . .</div>
		<div id="3dscontainer" style="height: 90vh"></div>
		<script type="text/javascript" charset="UTF-8" src="<%: Constants.PATH_ROOT %>Js/CredtTokenZeus2.0.js"></script>
	<% } %>
</body>
</html>
