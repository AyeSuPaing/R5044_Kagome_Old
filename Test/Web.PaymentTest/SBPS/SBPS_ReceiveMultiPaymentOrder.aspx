<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SBPS_ReceiveMultiPaymentOrder.aspx.cs" Inherits="SBPS_TEST_SBPS_ReceiveMultiPaymentOrder" %>

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
		<asp:Button ID="btnSendOrderNotice" Text="(購入結果通知送信)" runat="server" onclick="btnSendOrderNotice_Click" /><br /><br />

		<h4>※楽天ペイの場合<br />
			　決済を成功を試す場合、「楽天ペイ与信結果送信」を送信してください。<br />
			　それ以外の金額変更、売上結果通知を試す場合にはそれぞれを送信してください。(この場合注文の更新は行われません)<br />
		</h4>
		<asp:Button ID="btnRakutenAuthorizedResult" Text="(楽天ペイ与信結果送信)" runat="server" onclick="btnRakutenAuthorizedResult_Click" /><br />
		<asp:Button ID="bRakutenAmountChange" Text="(楽天ペイ金額変更通知送信)" runat="server" onclick="bRakutenAmountChange_Click" /><br />
		<asp:Button ID="bRakutenSalesComplete" Text="(楽天ペイ売上結果通知送信)" runat="server" onclick="bRakutenSalesComplete_Click" /><br />
		<br />
		<asp:CheckBox ID="cbChangeHttp" Text="httpに補正して送信 (WebRequestは正しい証明書のhttpsでないとエラーになるため)" Checked="true" runat="server" /><br />
		<asp:Literal ID="lSendOrderNoticeMessage" runat="server"></asp:Literal><br />
		<br />
	<br />
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
	<form method="post" action='<%= m_form["error_url"] %>'>
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
	<p><a href="<%= m_form["error_url"] %>">エラー遷移（クレジット）</a></p>

</body>
</html>
