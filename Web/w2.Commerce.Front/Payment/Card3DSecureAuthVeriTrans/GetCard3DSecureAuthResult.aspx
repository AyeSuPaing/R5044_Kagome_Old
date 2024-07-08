<%--
=========================================================================================================
  Module      : ベリトランス3Dセキュア認証結果取得ページ(GetCard3DSecureAuthResult.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetCard3DSecureAuthResult.aspx.cs" Inherits="Payment_Card3DSecureAuthVeriTrans_GetCard3DSecureAuthResult" %>

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
<body onload="<%if (Request[Constants.REQUEST_KEY_ORDER_ID] != null) {%>document.postdata.submit();<%} %>">
	<form name="postdata" method="post" action="<%= Request.RawUrl %>" >
		ただいま決済処理中です。<br />画面が切り替わるまでそのままお待ちください . . .
		<input type="hidden" name="cardMstatus" value="<%= WebSanitizer.HtmlEncode(Request.Form["cardMstatus"]) %>" />
		<input type="hidden" name="mpiMstatus" value="<%= WebSanitizer.HtmlEncode(Request.Form["mpiMstatus"]) %>" />
		<input type="hidden" name="<%= Constants.REQUEST_KEY_ORDER_ID %>" value="<%= WebSanitizer.HtmlEncode(Request[Constants.REQUEST_KEY_ORDER_ID]) %>" />
	</form>
</body>
</html>
