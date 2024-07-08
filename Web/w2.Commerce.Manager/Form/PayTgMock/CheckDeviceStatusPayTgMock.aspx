<%--
=========================================================================================================
  Module      : PayTg：端末状態確認モックページ(CheckDeviceStatusPayTgMock.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckDeviceStatusPayTgMock.aspx.cs" Inherits="Form_PayTgMock_CheckDeviceStatusPayTgMock" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>端末状態確認モック</title>
	<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/jquery-1.6.4.min.js") %>"></script>
</head>
<body>

<form id="form1" runat="server">
	<div>
		<asp:Literal ID="lMessage" Text="該当のボタンをクリックしてください" runat="server" /><br />
		<asp:Button ID="btnSendOkOk" runat="server" Text="  端末OK　接続OK  " OnClick="btnSendOkOk_Click" />
		<asp:Button ID="btnSendOkNg" runat="server" Text="  端末OK　接続NG  " OnClick="btnSendOkNg_Click" />
		<asp:Button ID="btnSendNgOk" runat="server" Text="  端末NG　接続OK  " OnClick="btnSendNgOk_Click" />
		<asp:Button ID="btnSendNgNg" runat="server" Text="  端末NG　接続NG  " OnClick="btnSendNgNg_Click" />
	</div>
	<script type="text/javascript">
		// レスポンス返す
		function returnResponse(canUseDeviceValue, stateMessageValue) {
			// 親画面が存在する場合、親画面でのレスポンス処理メソッドを呼び出す
			if (window.opener != null) {
				var res = {
					canUseDevice: canUseDeviceValue,
					stateMessage: stateMessageValue
				};
				var result = JSON.stringify(res);
				console.log(result);
				window.opener.getResponseFromCheckDeviceStatusMock(result);
			}
		}
	</script>
</form>
</body>
</html>
