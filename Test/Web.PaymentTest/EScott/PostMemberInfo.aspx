<%--
=========================================================================================================
  Module      : ソニーe-SCOTT 会員情報送信ページ(専用端末のモック)(PostMemberInfo.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.EScott" %>

<!DOCTYPE html>

<script runat="server">

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			tbUrl.Text = @"http://localhost/R5044_Kagome.Develop//Web/w2.Commerce.Front/Payment/EScott/PaymentEScottCreditCardRegisterResponseReceiver.ashx";
		}
	}

	/// <summary>
	/// 登録してPOSTするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterAndPost_Click(object sender, EventArgs e)
	{
		var eScottKaiinId = tbEScottMemberId.Text.Replace("-", "").Replace(" ", "");
		var memberAddApi = EScottMember4MemAddApi.CreateEScottMember4MemAddApiByKaiinId(eScottKaiinId, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_PASSWORD, tbToken.Text.Trim());
		var result = memberAddApi.ExecRequest();
		if (result.IsSuccess)
		{
			DoPost();
		}
		else
		{
			lSendOrderNoticeMessage.Text += result.ResponseMessage + "<br/>";
		}
	}

	/// <summary>
	/// POSTボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPost_Click(object sender, EventArgs e)
	{
		DoPost();
	}

	/// <summary>
	/// POST実行
	/// </summary>
	private void DoPost()
	{
		var api = EScottMemberInfoPostApi.CreateEScottEScottMemberInfoPostApi(
			tbEScottMemberId.Text.Replace("-", "").Replace(" ", ""),
			tbUrl.Text);
		var result = api.ExecRequest();

		lSendOrderNoticeMessage.Text += result + "(" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ")<br/>";
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title></title>
	<script type="text/javascript" charset="UTF-8" src="https://stg.static.mul-pay.jp/ext/js/token.js"></script>
</head>
<body onload="GetToken();">
	<form id="form1" runat="server">
	<div>
		<h1>e-SCOTT専用端末からオフライン注文向け</h1>
		<h2>e-SCOTT会員の登録を行い、POST通知する。</h2>
		カード番号：<asp:TextBox id="tbCardNo" Text="4123000000000100" Width="200" runat="server"></asp:TextBox><br>
		有効期限(月)：<asp:TextBox id="tbExpMM" Text="99" Width="200" runat="server"></asp:TextBox><br>
		有効期限(年)：<asp:TextBox id="tbExpYY" Text="99" Width="200" runat="server"></asp:TextBox><br>
		セキュリティコード：<asp:TextBox id="tbSecurityCode" Text="8888" Width="200" runat="server"></asp:TextBox><br>
		<br>
		<input type="button" value="トークン取得" onclick="getToken();"/><br>
		トークン：<asp:TextBox id="tbToken" Width="600" runat="server"></asp:TextBox><br/>
		<br>
		１．e-SCOTT会員ID入力：<asp:TextBox id="tbEScottMemberId" Width="200" runat="server"></asp:TextBox>（ハイフンは削除されます）<br/>
		<asp:Button id="btnRegisterAndPost" Text="２．登録してPOST送信" runat="server" OnClick="btnRegisterAndPost_Click" /><br/>
		<br/>
		<br/>
		<br/>
		<asp:Literal id="lSendOrderNoticeMessage" runat="server"></asp:Literal><br/>
		<br/>
		<br/>
		以下は触らなくてＯＫ(POSTで利用）
		<hr/>
		URL：<asp:TextBox id="tbUrl" Width="800" runat="server"></asp:TextBox><br/>
		<br/>
		<asp:Button id="btnPost" Text="　送信　" runat="server" OnClick="btnPost_Click"/><br/>
		
		<br/>
	</div>
	</form>
<script type="text/javascript"
	src="<%=Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_GETTOKEN_JS_URL %>?k_TokenNinsyoCode=<%=Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TOKENPAYMENTAUTHCODE %>"
	callBackFunc = "setToken"
	class = "spsvToken">
</script>
<script>
	function getToken() {
		SpsvApi.spsvCreateToken(
			document.getElementById('<%= tbCardNo.ClientID %>').value,
			document.getElementById('<%= tbExpYY.ClientID %>').value,
			document.getElementById('<%= tbExpMM.ClientID %>').value,
			document.getElementById('<%= tbSecurityCode.ClientID %>').value,
			"", "", "", "", "");
	}

	function setToken(token, card) {
		document.getElementById('<%= tbToken.ClientID %>').value = token;
	}
</script>
</body>
</html>
