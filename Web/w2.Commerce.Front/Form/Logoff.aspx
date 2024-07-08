<%--
=========================================================================================================
  Module      : ログオフ画面(Logoff.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page language="c#" Inherits="Form_Logoff" CodeFile="~/Form/Logoff.aspx.cs" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>Logout...</title>
</head>
<body>
	<form  runat="server">
	</form>
	<% if (this.IsAmazonLoggedIn)
	{ %>
	<script>
		<%--Amazon連携解除時処理--%>
		document.body.onload = function () {
			amazon.Login.logout();
			document.forms[0].submit();
		}
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<% } %>
</body>
</html>