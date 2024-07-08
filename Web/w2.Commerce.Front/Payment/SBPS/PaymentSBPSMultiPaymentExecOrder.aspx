<%--
=========================================================================================================
  Module      : SBPS マルチ決済 購入要求ページ(PaymentSBPSMultiPaymentExecOrder.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaymentSBPSMultiPaymentExecOrder.aspx.cs" Inherits="Payment_SBPS_PaymentSBPSMultiPaymentExecOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>SBPSマルチ決済(自動ポスト)</title>
	<%-- キャッシュ無効設定：ここから（消さないでください） --%>
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="Cache-Control" content="no-cache" />
	<meta http-equiv="Expires" content="-1" />
	<%	
		Response.AddHeader("cache-control", "no-store");
	%>
	<%-- キャッシュ無効設定：ここまで --%>
</head>
<body onload="document.postdata.submit();">

<form name="postdata" action='<%= this.OrderUrl %>' method="post">
<asp:Literal ID="lFormInputs" runat="server"></asp:Literal>
</form>
<div>
	転送中です。しばらくお待ちください。
	<p style="margin: 0 auto;">
		<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" alt="転送中です" />
	</p>
</div>
</body>
</html>
