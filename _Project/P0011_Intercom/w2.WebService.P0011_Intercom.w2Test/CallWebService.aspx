<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallWebService.aspx.cs" Inherits="w2.Plugin.P0011_Intercom.Webservice.w2Test.CallWebService" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
	<asp:DropDownList ID="DropDownList1" runat="server">
		<asp:ListItem Value="UserRegist">登録</asp:ListItem>
		<asp:ListItem Value="UserModify">変更</asp:ListItem>
		<asp:ListItem Value="UserDelete">退会</asp:ListItem>
	</asp:DropDownList>
		<asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="会員情報連携" />
		<asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
			Text="テストバリュー" />
		<asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
			Text="会員情報連携（テスト機webサービス）" />
		<asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
			Text="会員情報連携（インターコム）" />
		<br />
		<asp:Label ID="Label23" runat="server" Text="w2ユーザID"></asp:Label>
		<asp:TextBox ID="txw2id" runat="server" Height="17px" Width="254px"></asp:TextBox>
		<br />
		<br />
		<asp:Label ID="Label24" runat="server" Text="インターコムユーザID"></asp:Label>
		<asp:TextBox ID="txicid" runat="server" Height="17px" Width="254px"></asp:TextBox>
		<br />
    
    </div>
    <asp:Label ID="Label1" runat="server" Text="ログインID"></asp:Label>
	<asp:TextBox ID="txLogin" runat="server" Height="17px" Width="254px"></asp:TextBox>
	<p>
		<asp:Label ID="Label2" runat="server" Text="パスワード"></asp:Label>
		<asp:TextBox ID="txPass" runat="server" Height="17px" Width="254px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label3" runat="server" Text="企業名"></asp:Label>
		<asp:TextBox ID="txkigyo" runat="server" Height="17px" Width="254px"></asp:TextBox>
		<asp:Label ID="Label4" runat="server" Text="部署名"></asp:Label>
		<asp:TextBox ID="txbusyo" runat="server" Height="17px" Width="254px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label5" runat="server" Text="姓名"></asp:Label>
		<asp:TextBox ID="txSei" runat="server" Height="17px" Width="254px"></asp:TextBox>
		<asp:TextBox ID="txMei" runat="server" Height="17px" Width="254px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label7" runat="server" Text="姓名（かな）"></asp:Label>
		<asp:TextBox ID="txSeiKana" runat="server" Height="17px" Width="254px"></asp:TextBox>
		<asp:TextBox ID="txMeiKana" runat="server" Height="17px" Width="254px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label9" runat="server" Text="郵便番号"></asp:Label>
		<asp:TextBox ID="txzip1" runat="server" Height="17px" Width="68px"></asp:TextBox>
		-<asp:TextBox ID="txzip2" runat="server" Height="17px" Width="68px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label11" runat="server" Text="住所1"></asp:Label>
		<asp:TextBox ID="txadr1" runat="server" Height="17px" Width="448px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label12" runat="server" Text="住所2"></asp:Label>
		<asp:TextBox ID="txadr2" runat="server" Height="17px" Width="448px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label13" runat="server" Text="住所3"></asp:Label>
		<asp:TextBox ID="txadr3" runat="server" Height="17px" Width="448px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label14" runat="server" Text="住所4"></asp:Label>
		<asp:TextBox ID="txadr4" runat="server" Height="17px" Width="448px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label15" runat="server" Text="電話番号"></asp:Label>
		<asp:TextBox ID="txtell1" runat="server" Height="22px" Width="80px"></asp:TextBox>
		-<asp:TextBox ID="txtell2" runat="server" Height="22px" Width="80px"></asp:TextBox>
		-<asp:TextBox ID="txtell3" runat="server" Height="22px" Width="80px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label16" runat="server" Text="メールアドレス"></asp:Label>
		<asp:TextBox ID="txMail" runat="server" Height="17px" Width="448px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label17" runat="server" Text="生年月日"></asp:Label>
		<asp:TextBox ID="txNen" runat="server" Height="17px" Width="68px"></asp:TextBox>
		年<asp:TextBox ID="txTuki" runat="server" Height="17px" Width="68px"></asp:TextBox>
		月<asp:TextBox ID="txHi" runat="server" Height="17px" Width="68px"></asp:TextBox>
		日</p>
	<p>
		<asp:Label ID="Label18" runat="server" Text="性別"></asp:Label>
		<asp:DropDownList ID="dpSex" runat="server">
			<asp:ListItem Value="MALE">男</asp:ListItem>
			<asp:ListItem Value="FEMALE">女</asp:ListItem>
		</asp:DropDownList>
	</p>
	<p>
		<asp:Label ID="Label19" runat="server" Text="メール配信フラグ"></asp:Label>
		<asp:DropDownList ID="dpMailFlg" runat="server">
			<asp:ListItem>ON</asp:ListItem>
			<asp:ListItem>OFF</asp:ListItem>
		</asp:DropDownList>
	</p>
	<p>
		<asp:Label ID="Label20" runat="server" Text="最終更新者"></asp:Label>
		<asp:TextBox ID="txkousin" runat="server" Height="17px" Width="448px"></asp:TextBox>
	</p>
	<p>
		<asp:Label ID="Label21" runat="server" Text="削除フラグ"></asp:Label>
		<asp:DropDownList ID="dpDelFlg" runat="server">
			<asp:ListItem Value="0">有効</asp:ListItem>
			<asp:ListItem Value="1">無効</asp:ListItem>
		</asp:DropDownList>
	</p>
	<p>
		<asp:Label ID="Label22" runat="server" Text="webサービス戻り結果"></asp:Label>
	</p>
    </form>
</body>
</html>
