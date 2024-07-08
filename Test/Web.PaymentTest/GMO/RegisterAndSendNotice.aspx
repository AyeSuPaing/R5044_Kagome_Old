<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>

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
			tbUrl.Text = @"http://localhost/R5044_Kagome.Develop//Web/w2.Commerce.Front/Payment/Gmo/PaymentGmoCreditCardRegisterResponseReceiver.ashx";
			rParams.DataSource = new []
			{
				new KeyValuePair<string, string>("SiteID", "tsite00005618"),
				new KeyValuePair<string, string>("MemberID", ""),
				new KeyValuePair<string, string>("CardSeq", "0"),
				new KeyValuePair<string, string>("CardSeqLogical", "0"),
				new KeyValuePair<string, string>("ProcessDate", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new KeyValuePair<string, string>("ProcessType", "I"),
				new KeyValuePair<string, string>("CardNo", "************1111"),
				new KeyValuePair<string, string>("Expire", "2406"),
				new KeyValuePair<string, string>("Forward", ""),
				new KeyValuePair<string, string>("DefaultFlag", ""),
				new KeyValuePair<string, string>("PayType", "0"),// クレジットカード
				new KeyValuePair<string, string>("ErrCode", ""),
				new KeyValuePair<string, string>("ErrInfo", ""),
			};
			rParams.DataBind();
		}
	}

	/// <summary>
	/// 登録してPOSTするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterAndPost_Click(object sender, EventArgs e)
	{
		var gmoMemberId = tbGmoMemberId.Text.Trim().Replace("-", "").Replace(" ", "");
		var paymentGmoCredit = new PaymentGmoCredit();
		var result = paymentGmoCredit.SaveMemberAndCard(
			gmoMemberId,
			"USERNAME",
			tbToken.Text.Trim(),
			"AUTHER");
		if (result)
		{
			DoPost(gmoMemberId);
		}
		else
		{
			lSendOrderNoticeMessage.Text += paymentGmoCredit.ErrorMessages + "<br/>";
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
	private void DoPost(string gmoMemberId = null)
	{
		var paramString = string.Join(
			"&",
			rParams.Items.Cast<RepeaterItem>().Select(
				ri =>
				{
					var key = ((TextBox)ri.FindControl("tbKey")).Text.Trim();
					var value = ((TextBox)ri.FindControl("tbValue")).Text.Trim();
					if ((key == "MemberID") && (gmoMemberId != null)) value = gmoMemberId;
					if ((key == "MemberID") && (gmoMemberId != null)) value = gmoMemberId;

					return string.Format(
						"{0}={1}",
						key,
						HttpUtility.UrlEncode((key == "MemberID") ? value.Replace("-", "") : value, Encoding.UTF8));
				}));
		var postData = Encoding.UTF8.GetBytes(paramString);

		var webRequest = (HttpWebRequest)WebRequest.Create(tbUrl.Text);
		webRequest.Method = WebRequestMethods.Http.Post;
		webRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
		webRequest.ContentLength = postData.Length;

		// 送信データの書き込み
		var stPostStream = webRequest.GetRequestStream();
		stPostStream.Write(postData, 0, postData.Length); // 送信するデータを書き込む
		stPostStream.Close();

		// レスポンス取得
		using (var responseStream = webRequest.GetResponse().GetResponseStream())
		using (var sr = new StreamReader(responseStream, Encoding.UTF8))
		{
			var responseText = sr.ReadToEnd();
			lSendOrderNoticeMessage.Text += "[ " + responseText + " ] " + tbGmoMemberId.Text + "(" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ")<br/>";
			if (responseText == "0")
			{
				tbGmoMemberId.Text = "";
			}
		}
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
		<h1>GMOテレフォンオーダー向け</h1>
		<h2>GMO会員の登録を行い、POST通知する。</h2>
		カード番号：<asp:TextBox id="tbCardNo" Text="4111111111111111" Width="200" runat="server"></asp:TextBox>
			<input type="button" value="トークン取得" onclick="GetToken();"/>　トークン：<asp:TextBox id="tbToken" Width="600" runat="server"></asp:TextBox><br/>
		１．GMO会員ID入力：<asp:TextBox id="tbGmoMemberId" Width="200" runat="server"></asp:TextBox>（ハイフンは削除されます）<br/>
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
		PARAM<br/>
		<asp:Repeater id="rParams" runat="server">
		<ItemTemplate>
			<asp:TextBox id="tbKey" Width="100" Text='<%# Eval("Key") %>' runat="server"></asp:TextBox>
			=
			<asp:TextBox id="tbValue" Width="200" Text='<%# Eval("Value") %>' runat="server"></asp:TextBox><%# ((string)Eval("Key") == "Expire") ? " (YYMM指定)" : "" %><br/>
		</ItemTemplate>
		</asp:Repeater>
		<br/>
		<asp:Button id="btnPost" Text="　送信　" runat="server" OnClick="btnPost_Click"/><br/>
		※GMO会員IDはハイフンありでもOK<br/>
		<br/>
	</div>
	</form>
<script>
	Multipayment.init("<%:Constants.PAYMENT_SETTING_GMO_SHOP_ID %>");

	function GetToken() {
		Multipayment.getToken({
			cardno: document.getElementById('<%= tbCardNo.ClientID %>').value,
			expire: '202211',
			securitycode: '123',
			holdername: 'DOUBLE TARO'
		}, getTokenCallbackGmo);
	}

	function getTokenCallbackGmo(result) {
		var resc = result.resultCode;
		if (resc == "000") {
			document.getElementById('<%= tbToken.ClientID %>').value = result.tokenObject.token;// + " " + result.tokenObject.toBeExpiredAt;
		} else {
			alert("error:" + resc);
		}
	}
</script>
</body>
</html>
