<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderPaymentNotification.aspx.cs" Inherits="GMO_CVS_OrderPaymentNotification" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			URL<br />
			<asp:TextBox ID="tbUrl" Width="800" runat="server" Visible="true"></asp:TextBox><br /><br />
			Params<br />
			<asp:TextBox ID="tbParams" Width="800" runat="server" Visible="true" TextMode="MultiLine"></asp:TextBox><br /><br />
			<asp:Button runat="server" ID="btnCreateParam" Text="Create param" OnClick="btnCreateParam_Click" /><br /><br />
			<asp:Button ID="btnSendOrderNotice" Text="Send" runat="server" onclick="btnSendOrderNotice_Click" />
			<asp:Literal ID="lMessage" runat="server"></asp:Literal><br />
		</div>
	</form>
</body>
</html>