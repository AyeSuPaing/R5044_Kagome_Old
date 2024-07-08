<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestUserSearch.aspx.cs" Inherits="w2.Plugin.P0011_Intercom.WebService.TestUserSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>検証用会員情報検索</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		
    </div>
    <asp:Label ID="Label1" runat="server" Text="ログインID："></asp:Label>
	<asp:TextBox ID="TextBox1" runat="server" Width="332px"></asp:TextBox>
	<asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
		Text="ログインIDで検索" />
	<br />
	<br />
	<asp:Label ID="Label3" runat="server" Text="w2ユーザID："></asp:Label>
	<asp:TextBox ID="txtw2" runat="server" Width="332px"></asp:TextBox>
	<asp:Button ID="btnw2" runat="server" onclick="btnw2_Click" Text="w2ユーザIDで検索" />
	<br />
	<br />
	<asp:Label ID="Label4" runat="server" Text="IntercomユーザID："></asp:Label>
	<asp:TextBox ID="txtic" runat="server" Width="332px"></asp:TextBox>
	<asp:Button ID="btnic" runat="server" onclick="btnic_Click" 
		Text="IntercomユーザIDで検索" />
	<br />
	<p>
		<asp:Label ID="Label2" runat="server" Text="検索結果1"></asp:Label>
	</p>
	<asp:Label ID="lblref" runat="server"></asp:Label>
    </form>
</body>
</html>
