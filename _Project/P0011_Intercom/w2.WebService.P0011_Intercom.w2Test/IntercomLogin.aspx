<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntercomLogin.aspx.cs" Inherits="w2.Plugin.P0011_Intercom.Webservice.w2Test.IntercomLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
		Text="リダイレクトURL作成" />
	<p>
		<asp:Label ID="Label1" runat="server" Text="w2id"></asp:Label>
		<asp:TextBox ID="txtw2id" runat="server" style="margin-bottom: 0px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label3" runat="server" Text="icid"></asp:Label>
		<asp:TextBox ID="txticid" runat="server"></asp:TextBox>
	</p>
	<p>
		&nbsp;</p>
    <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
		Text="w2SSO画面へリダイレクト" />
	<asp:TextBox ID="TextBox1" runat="server" Width="688px"></asp:TextBox>
    </form>
</body>
</html>
