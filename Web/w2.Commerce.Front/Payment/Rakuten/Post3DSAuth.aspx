<!--
=========================================================================================================
  Module      : 楽天3Dセキュア認証データ送信ページ処理(Post3DSAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Post3DSAuth.aspx.cs" Inherits="Payment_Rakuten_Post3DSAuth" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body onload="document.postdata.submit();">
	<form name="postdata" method="POST" action="<%: this.Card3DSecureAuthUrl %>" >
		<input type="hidden" name="paymentinfo" value="<%: this.PaymentInfo %>" />
		<input type="hidden" name="signature" value="<%: this.Signature %>" />
		<input type="hidden" name="key_version" value="<%: this.KeyVersion %>" />
	</form>
</body>
</html>
