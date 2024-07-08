<%--
=========================================================================================================
  Module      : Authentication 3DSecure (Authentication3DSecure.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Authentication3DSecure.aspx.cs" Inherits="Zcom_Authentication3DSecure" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<style>
		html,
		body {
			height: 100%;
			margin: 0;
			box-sizing: border-box;
		}

		h4,
		h5 {
			margin: 0;
		}

		.container {
			display: flex;
			justify-content: center;
			align-items: center;
			width: auto;
			height: 100%;
		}

		.container-header {
			border-top: 1px solid #bbbbbb;
		}

		.container-authentication {
			width: 700px;
			padding: 10px;
		}

		.authentication-confirm {
			padding: 15px 0;
			margin-bottom: 30px;
			border-top: 1px solid #bbbbbb;
			border-bottom: 1px solid #bbbbbb;
		}

		.authentication-info-jp {
			margin-bottom: 30px;
		}

		.authentication-info-en {
			margin-bottom: 15px;
		}

		.authentication-action {
			max-width: 130px;
		}

		.authentication-error {
			width: 300px;
		}

		.control {
			display: flex;
			align-items: center;
		}

		.authentication-error .control span {
			flex: 300px 1;
		}

		.control input {
			width: 100%;
			height: 30px;
			box-sizing: border-box;
			margin: 5px 5px 5px 0px;
			border-radius: 5px;
			border: 1px solid #afafaf;
		}

		input.btn {
			height: 30px;
			min-width: 60px;
			font-size: 20px;
			margin-right: 10px;
			text-align: center;
			cursor: pointer;
		}

		.btn-error {
			margin-top: 20px;
		}
	</style>
</head>
<body>
	<form class="container" runat="server">
		<asp:ScriptManager ID="scriptManager" runat="server" />
		<asp:UpdatePanel ID="updatePanel" runat="server">
			<ContentTemplate>
				<% if (Constants.PAYMENT_CREDIT_USE_ZCOM3DS_MOCK 
					&& (string.IsNullOrEmpty(this.TransCode) == false)) { %>
				<div class="container-authentication">
					<div class="container-header">
						<h1>3D-Secure Authentication dummy page</h1>
					</div>
					<div class="authentication-confirm">
						<div class="authentication-info-jp">
							<h5>消費者はこのタイミングで本人認証のための情報（パスワード等）を入力します。</h5>
							<h5>これはダミーサイトのため、認証情報の入力は省略します。</h5>
							<h5>ボタンを押して次のページに進んで下さい。</h5>
						</div>
						<div class="authentication-info-en">
							<h4>In this sequence, consumers will enter the password for authentication.</h4>
							<h4>Please press the button. Password is omitted Because this is a dummy site.</h4>
						</div>
						<div class="authentication-action control">
							<asp:Button ID="btnConfirm" Text="OK" class="btn" runat="server" OnClick="btnConfirm_Click" />
							<asp:Button ID="btnBack" Text="戻る" class="btn" runat="server" OnClick="btnBack_Click" />
						</div>
					</div>
					<div class="authentication-error">
						<div class="control">
							<asp:Label Text="エラーコード：" runat="server" />
							<asp:TextBox ID="tbErrorCode" type="text" MaxLength="10" runat="server" />
						</div>
						<div class="control">
							<asp:Label Text="エラーメッセージ：" runat="server" />
							<asp:TextBox ID="tbErrorDetail" type="text" runat="server" />
						</div>
						<div>
							<asp:Label ID="lbMessage" Text="" style="color: red" runat="server" />
							<asp:Button ID="btnError" Text="エラー" class="btn btn-error" runat="server" OnClick="btnError_Click" />
						</div
					</div>
				</div>
				<% } else { %>
				Invalid parameter ...
				<% } %>
			</ContentTemplate>
		</asp:UpdatePanel>
	</form>
</body>
</html>
