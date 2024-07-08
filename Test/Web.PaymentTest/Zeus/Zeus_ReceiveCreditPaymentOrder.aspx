<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Zeus_ReceiveCreditPaymentOrder.aspx.cs" Inherits="Zeus_Zeus_ReceiveCreditPaymentOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<h1>Zeus決済(リンク式）テスト</h1>
		<hr />
		<h3>■決済成功を試す</h3>
		<h4>１．まず「購入結果通知」を送信してください。（バックグラウンドポスト）<br />
			　　ボタン押下後、下に「OK,」が出れば結果通知成功</h4>
		<asp:TextBox ID="tbOrderNoticeUrl" Width="800" runat="server" Text='<%# CreateNoticeUrl() %>'></asp:TextBox><br />
		<asp:Button ID="btnSendOrderNotice" Text="(購入結果通知送信)" runat="server" onclick="btnSendOrderNotice_Click" />
		<asp:CheckBox ID="cbChangeOrderNoticeUrlHttp" Text="httpに補正して送信 (WebRequestは正しい証明書のhttpsでないとエラーになるため)" Checked="true" runat="server" /><br />
		<asp:Literal ID="lbtnSendOrderNoticeResultMessage" runat="server"></asp:Literal>	<br />
    </div>
    </form>
	<h4>２．次に、下記ボタンで画面遷移を行ってください。</h4>
	<form method="post" action='<%#: Request.Form["success_url"] %>'>
		<input id="Submit1" type="submit" value="購入完了結果遷移" runat="server" /><br />
	</form>
	<br />
	<hr />

	<div id="divButtonsForCredit3DSecure" Visible='<%# (Request.Form["pay_method"] == "credit3d") %>' runat="server">
		<h3>■キャンセルを試す</h3>
		<form method="get" action='<%#: Request.Form["failure_url"] %>'>
			<input id="Submit3" type="submit" value="キャンセル遷移（クレジット3Dセキュア）" runat="server" /><br />
		</form>
		<br />
		<h3>■カードエラーを試す</h3>
		<form method="get" action='<%#: Request.Form["failure_url"] %>'>
			<input id="Submit6" type="submit" value="カードエラー遷移（クレジット3Dセキュア）" runat="server" /><br />
		</form>
		<br />
		<h3>■その他エラーを試す</h3>
		<form method="get" action='<%#: Request.Form["failure_url"] %>'>
			<input id="Submit5" type="submit" value="その他エラー遷移（クレジット3Dセキュア）" runat="server" /><br />
		</form>
		<br />
	</div>

	<div id="divButtonsForOthers" Visible='<%# (Request.Form["pay_method"] != "credit3d") %>' runat="server">
		<h3>■エラーを試す</h3>
		<p><a href="<%#: Request.Form["failure_url"] %>">エラー遷移</a></p>
		<br />
	</div>

</body>
</html>
