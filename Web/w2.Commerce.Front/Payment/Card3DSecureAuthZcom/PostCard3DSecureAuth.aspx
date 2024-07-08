<!--
=========================================================================================================
  Module      : Post Card 3DSecure Auth (PostCard3DSecureAuth.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostCard3DSecureAuth.aspx.cs" Inherits="Payment_Card3DSecureAuthZcom_PostCard3DSecureAuth" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title></title>
	</head>
	<body onload="document.downLoadFrom.submit();">
		<noscript>
			<br /><br />
			<center>
				<h2>
					You continue to implement payment processing.<br />
					Please click the following the "OK" button.
				</h2>
				<input type="submit" value="OK" />
			</center>
		</noscript>
		<form name="downLoadFrom" action="<%= this.AccessUrl %>" method="post" >
			<input type="hidden" name="payment_code" value="<%= this.PaymentCode %>" />
			<input type="hidden" name="trans_code" value="<%= this.TransCode %>" />
			<input type="hidden" name="mode" value="<%= this.Mode %>" />
		</form>
	</body>
</html>
