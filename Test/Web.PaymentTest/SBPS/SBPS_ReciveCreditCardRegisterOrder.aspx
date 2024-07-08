<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SBPS_ReciveCreditCardRegisterOrder.aspx.cs" Inherits="SBPS_SBPS_ReciveCreditCardRegisterOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<h1>ソフトバンクペイメント(リンク式）テスト</h1>
		<hr />
		<h3>■決済成功を試す</h3>
		<h4>１．まず「購入結果通知」を送信してください。（バックグラウンドポスト）<br />
			　　ボタン押下後、下に「OK,」が出れば結果通知成功</h4>
		<asp:TextBox ID="tbUrl" Width="800" runat="server" Visible="true"></asp:TextBox><br />
		<asp:Button ID="SendOrderNotice" Text="(購入結果通知送信)" runat="server" onclick="SendOrderNotice_Click" />
		<asp:CheckBox ID="cbChangeHttp" Text="httpに補正して送信 (WebRequestは正しい証明書のhttpsでないとエラーになるため)" Checked="true" runat="server" /><br />
		<asp:Literal ID="lSendOrderNoticeMessage" runat="server"></asp:Literal>	<br />
    </div>
    </form>
	<h4>２．次に、下記ボタンで画面遷移を行ってください。</h4>
	<form method="POST" action='<%= m_form["success_url"] %>'>
		<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS")) %>
		<input id="Submit1" type="submit" value="購入完了結果遷移" runat="server" /><br />
	</form>
	<br />
	<hr />

	<div id="divButtonsForCredit3DSecure" runat="server">
	<h3>■キャンセルを試す</h3>
	<form method="post" action='<%= m_form["cancel_url"] %>'>
		<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"), "7717")%>
		<input id="Submit3" type="submit" value="キャンセル遷移（クレジット3Dセキュア）" runat="server" /><br />
	</form>
	<br />
	<h3>■カードエラーを試す</h3>
	<form method="post" action='<%= m_form["error_url"] %>'>
		<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"), "7702")%>
		<input id="Submit6" type="submit" value="カードエラー遷移（クレジット3Dセキュア）" runat="server" /><br />
	</form>
	<br />
	<h3>■その他エラーを試す</h3>
	<form method="post" action='<%= m_form["error_url"] %>'>
		<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"))%>
		<input id="Submit5" type="submit" value="その他エラー遷移（クレジット3Dセキュア）" runat="server" /><br />
	</form>
	<br />
	</div>

	<div id="divButtonsForCreditEMV3DSecure" runat="server">
		<h3>■キャンセルを試す</h3>
		<form method="post" action='<%= m_form["error_url"] %>'>
			<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"), "7717")%>
			<input id="Submit7" type="submit" value="キャンセル遷移（クレジット3Dセキュア2.0）" runat="server" /><br />
		</form>
		<br />
		<h3>■カードエラーを試す</h3>
		<form method="post" action='<%= m_form["error_url"] %>'>
			<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"), "7702")%>
			<input id="Submit8" type="submit" value="カードエラー遷移（クレジット3Dセキュア2.0）" runat="server" /><br />
		</form>
		<br />
		<h3>■その他エラーを試す</h3>
		<form method="post" action='<%= m_form["error_url"] %>'>
			<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"))%>
			<input id="Submit9" type="submit" value="その他エラー遷移（クレジット3Dセキュア2.0）" runat="server" /><br />
		</form>
		<br />
	</div>

	<div id="divButtonsForOthers" runat="server">
	<h3>■キャンセルを試す</h3>
	<form method="post" action='<%= m_form["cancel_url"] %>'>
		<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"))%>
		<input id="Submit2" type="submit" value="キャンセル遷移（キャリア決済）" runat="server" /><br />
	</form>
	<br />
	<h3>■エラーを試す</h3>
	<form method="post" action='<%= m_form["error_url"] %>'>
		<%= CreateFormInputs(Encoding.GetEncoding("Shift_JIS"))%>
		<input id="Submit4" type="submit" value="エラー遷移（キャリア決済）" runat="server" /><br />
	</form>
	<br />
	</div>

</body>
</html>