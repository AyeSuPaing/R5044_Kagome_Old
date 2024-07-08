<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestLogin.aspx.cs" Inherits="w2.Plugin.P0011_Intercom.Webservice.w2Test.TestLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="SS_IN" />
	<p>
		<asp:TextBox ID="TextBox1" runat="server" Width="858px"></asp:TextBox>
	</p>
	<p>
		<asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="SS_OFF" />
	</p>
	<asp:TextBox ID="TextBox2" runat="server" Width="846px"></asp:TextBox>
    </form>
</body>
</html>
