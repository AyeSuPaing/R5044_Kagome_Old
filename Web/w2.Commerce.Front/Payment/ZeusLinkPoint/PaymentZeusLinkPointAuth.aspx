<!--
=========================================================================================================
  Module      : ゼウス（LinkPoint）決済ページ処理(PaymentZeusLinkPointAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaymentZeusLinkPointAuth.aspx.cs" Inherits="Payment_ZeusLinkPoint_PaymentZeusLinkPointAuth" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body onload="document.postdata.submit();">
	<form name="postdata" method="POST" action="<%= this.ZeusLinkPointUrl %>" >
		<input type="hidden" name="clientip" value="<%: this.ClientIp %>" />
		<input type="hidden" name="money" value="<%: this.Money %>" />
		<input type="hidden" name="telno" value="<%: this.TelNo %>" />
		<input type="hidden" name="email" value="<%: this.Email %>" />
		<input type="hidden" name="sendid" value="<%: this.SendId %>" />
		<input type="hidden" name="sendpoint" value="<%: this.SendPoint %>" />
		<input type="hidden" name="success_url" value="<%: this.SuccessUrl %>" />
		<input type="hidden" name="success_str" value="<%: this.SuccessStr %>" />
		<input type="hidden" name="failure_url" value="<%: this.FailureUrl %>" />
		<input type="hidden" name="failure_str" value="<%: this.FailureStr %>" />
	</form>
</body>
</html>
