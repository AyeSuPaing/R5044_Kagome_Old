<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SerialInput.aspx.cs" Inherits="w2.Plugin.P0011_Intercom.Webservice.w2Test.SerialInput" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    	<asp:Label ID="Label1" runat="server" Text="対象商品ID："></asp:Label>
&nbsp;<asp:Label ID="lblpid" runat="server"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    
    </div>
    <asp:Label ID="Label3" runat="server" Text="対象バリエーションID："></asp:Label>
	<asp:Label ID="lblva" runat="server"></asp:Label>
	<br />
    <asp:Label ID="Label2" runat="server" Text="対象個数："></asp:Label>
	<asp:Label ID="lblko" runat="server"></asp:Label>
	<p>
		&nbsp;</p>
	<p>
		<asp:TextBox ID="txtSeri1" runat="server"></asp:TextBox>
	</p>
	<p align="left">
		<asp:TextBox ID="txtSeri2" runat="server"></asp:TextBox>
	</p>
	<asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="入力オッケー" />
    </form>
</body>
</html>
