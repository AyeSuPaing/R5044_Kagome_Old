<%--
=========================================================================================================
  Module      : ベリトランスPaypay決済結果受信 (VeriTransPayPayReceive.aspx)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VeriTransPayPayReceive.aspx.cs" Inherits="Payment_VeriTransPayPayReceive" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
<body onload="<% if (this.RequestOrderId != null) { %>document.postdata.submit();<% } %>">
	<form name="postdata" method="post" action="<%= Request.RawUrl %>">
		ただいま決済処理中です。<br />画面が切り替わるまでそのままお待ちください . . .
		<input type="hidden" name="<%= Constants.REQUEST_KEY_ORDER_ID %>" value="<%: this.RequestOrderId %>" />
		<input type="hidden" name="exec" value="1" />
	</form>
</body>
</html>
