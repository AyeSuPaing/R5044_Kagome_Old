<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemoPage.aspx.cs" Inherits="w2.Plugin.P0011_Intercom.Webservice.w2Test.DemoPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Label ID="lblUserID" runat="server" Text="ユーザーID"></asp:Label>
	<asp:TextBox ID="txtUserID" runat="server"></asp:TextBox>
	<asp:Label ID="Label1" runat="server" Text="処理区分"></asp:Label>
	<asp:DropDownList ID="DropDownList1" runat="server">
		<asp:ListItem Value="UserRegist">登録</asp:ListItem>
		<asp:ListItem Value="UserModify">変更</asp:ListItem>
		<asp:ListItem Value="UserDelete">退会</asp:ListItem>
	</asp:DropDownList>
	<asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="会員情報連携" />
	<asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
		Text="会員情報連携（エラー発生）" />
	<asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
		Text="会員情報連携（テスト環境のwebサービス）" />
	<p>
		<asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
			Text="ワンタイムパス発行" />
	</p>
    <p>
		<asp:Label ID="Label2" runat="server" Text="webサービス戻り結果"></asp:Label>
	</p>
	<asp:Label ID="lblRtDs" runat="server"></asp:Label>
    </form>
</body>
</html>
