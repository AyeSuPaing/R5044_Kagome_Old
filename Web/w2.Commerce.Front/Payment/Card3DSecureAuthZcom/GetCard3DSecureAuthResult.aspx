<%--
=========================================================================================================
  Module      : Get Card 3DSecure Auth Result (GetCard3DSecureAuthResult.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetCard3DSecureAuthResult.aspx.cs" Inherits="Payment_Card3DSecureAuthZcom_GetCard3DSecureAuthResult" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>決済処理中</title>
		<meta http-equiv="Pragma" content="no-cache" />
		<meta http-equiv="Cache-Control" content="no-cache" />
		<meta http-equiv="Expires" content="-1" />
		<script type="text/javascript">
		<!--
		history.forward();
		//-->
		</script>
	</head>
	<body onload="<% if (string.IsNullOrEmpty(this.OrderNumber) == false) { %>document.postdata.submit();<% } %>">
		<form name="postdata" method="post" action="<%= this.Request.RawUrl %>">
			ただいま決済処理中です。<br />
			画面が切り替わるまでそのままお待ちください . . .
		</form>
	</body>
</html>
