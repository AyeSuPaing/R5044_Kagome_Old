<%--
=========================================================================================================
  Module      : Paygent Difference Notification (PaygentDifferenceNotification.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaygentDifferenceNotification.aspx.cs" Inherits="Paygent_CVS_PaygentDifferenceNotification" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>Paygent コンビニ前払い 入金テスト</title>
	<style>
		table, th, td, tr {
			border: none;
		}
		th {
			text-align: left;
		}
	</style>
</head>
<body>
	<form runat="server">
		<div>
			<h2>Paygent コンビニ前払い 入金テスト</h2>
			<hr />
			URL：<asp:TextBox ID="tbUrl" runat="server" Width="800" /><br />
			<hr />
			<table>
				<tr>
					<th>決済通知ID</th>
					<td>
						<asp:TextBox ID="tbPaymentNoticeId" runat="server" Width="200" />
					</td>
				</tr>
				<tr>
					<th>決済ID</th>
					<td>
						<asp:TextBox ID="tbPaymentId" runat="server" Width="200" />
					</td>
				</tr>
				<tr>
					<th>マーチャント取引ID</th>
					<td>
						<asp:TextBox ID="tbTradingId" runat="server" Width="200" />
					</td>
				</tr>
				<tr>
					<th>決済種別CD</th>
					<td>
						<asp:TextBox ID="tbPaymentType" runat="server" Width="200" Text="22" />
					</td>
				</tr>
				<tr>
					<th>決済ステータス</th>
					<td>
						<asp:TextBox ID="tbPaymentStatus" runat="server" Width="200" />
					</td>
				</tr>
				<tr>
					<th>決済金額</th>
					<td>
						<asp:TextBox ID="tbPaymentAmount" runat="server" Width="200" />
					</td>
				</tr>
				<tr>
					<th>Paidy決済ID</th>
					<td>
						<asp:TextBox ID="tbPaidyPaymentId" runat="server" Width="200" />
					</td>
				</tr>
				<tr>
					<th>差分通知ハッシュ値生成キー</th>
					<td>
						<asp:TextBox ID="tbHashKey" runat="server" Width="200" Text="<%# Constants.PAYMENT_PAYGENT_NOTICE_HASHKEY %>" />
					</td>
				</tr>
			</table>
		</div>
		<asp:Button ID="btSend" runat="server" Text=" 　送信 　" OnClick="btSend_Click" />
		<hr />
		<div>
			<asp:Literal ID="lResponse" runat="server" />
		</div>
		<hr />
		備考:
<pre>
決済ID:
	決済取引ID

マーチャント取引ID:
	注文ID

決済種別CD:
	22:Paidy

決済ステータス(payment_status):
	20：オーソリOK
	33：オーソリ期限切
	40：消込済
	41：消込済（売上取消期限切）
	43：速報検知済

	組み合わせ
	switch (payment_status)
	{
		case "20":
			//   + 外部決済ステータス→「与信済み」
			//「外部決済連携ログ」にログを残す
			break;
		case "33":
			//   + 外部決済ステータス→「与信期限切れ」
			//「外部決済連携ログ」にログを残す
			break;
		case "41":
			//   + 外部決済ステータス→「売上取消期限切れ」
			//「外部決済連携ログ」にログを残す
			break;

		case "40":
		case "43":
			// ステータス更新は実施せず、「外部決済連携ログ」にログを残す
			break;
	}
</pre>
	</form>
</body>
</html>
