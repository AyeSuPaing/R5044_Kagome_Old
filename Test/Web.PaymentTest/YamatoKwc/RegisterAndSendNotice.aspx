<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.YamatoKwc" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.YamatoKwc.Helper" %>
<%@ Import Namespace="w2.Common.Logger" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Order.UserCreditCardCooperationInfos" %>

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
			tbUrl.Text = @"http://localhost/R5044_Kagome.Develop//Web/w2.Commerce.Front/Payment/YamatoKwc/PaymentYamatoKwcResponseReceiver.ashx";
			rParams.DataSource = new[]
				{
					new KeyValuePair<string, string>("trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE),
					new KeyValuePair<string, string>("order_no", "(別フォームでセット)"),
					new KeyValuePair<string, string>("settle_price", "1"),
					new KeyValuePair<string, string>("settle_date", DateTime.Now.ToString("yyyyMMddHHmmss")),
					new KeyValuePair<string, string>("settle_result", "1"),	// 正常
					new KeyValuePair<string, string>("settle_detail", "4"),	// 与信完了
					new KeyValuePair<string, string>("settle_method", "9"),	// VISA
				};
			rParams.DataBind();
		}
	}

	/// <summary>
	/// チェックサム作成
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCreateCheckSum_Click(object sender, EventArgs e)
	{
		tbYamatoKwcMemberId.Text = tbYamatoKwcMemberId.Text.Trim().Replace("-", "");
		tbYamatoKwcAccessKey.Text = tbYamatoKwcAccessKey.Text.Trim().Replace("-", "");
		tbYamatoKwcOrderNo.Text = tbYamatoKwcOrderNo.Text.Trim().Replace("-", "");
		
		tbCheckSum.Text = PaymentYamatoKwcCheckSumCreater.CreateForToken(
			tbYamatoKwcMemberId.Text,
			tbYamatoKwcAccessKey.Text);
		GetFromKvps("order_no").Text = tbYamatoKwcOrderNo.Text;
	}
	
	/// <summary>
	/// 登録してPOSTするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterAndPost_Click(object sender, EventArgs e)
	{
		// クレジットカード登録実施
		var result = Auth1Yen();
		if (result)
		{
			DoPost();
		}
	}

	/// <summary>
	/// 登録のみ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOnlyRegister_Click(object sender, EventArgs e)
	{
		Auth1Yen();
	}

	/// <summary>
	/// 1円与信実施（キャンセルなし）
	/// </summary>
	/// <returns>与信結果</returns>
	private bool Auth1Yen()
	{
		var telNo = "090-1414-1919";
		var name = "ｗ２太郎モック";
		var mailAddr = "sampe@w2solution.co.jp";
		var companyCode = GetFromKvps("settle_method").Text;
		var paymentOrderId = GetFromKvps("order_no").Text;

		var yamatoApi = new PaymentYamatoKwcCreditAuthApi(true);
		var result = yamatoApi.Exec(
			PaymentYamatoKwcDeviceDiv.Pc,
			paymentOrderId,
			Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
			1,
			name,
			telNo,
			mailAddr,
			1,
			new PaymentYamatoKwcCreditOptionServiceParamOptionReg(
				companyCode,
				tbToken.Text.Trim()));
		lSendOrderNoticeMessage.Text += "[ " + (result.Success ? "OK" : "NG") + " ] " + tbYamatoKwcMemberId.Text + "(" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ")<br/>";
		if (result.Success)
		{
			tbYamatoKwcOrderNo.Text = "";
			tbYamatoKwcMemberId.Text = "";
			tbYamatoKwcAccessKey.Text = "";
			tbCheckSum.Text = "";
			tbToken.Text = "";
		}
		else
		{
			lSendOrderNoticeMessage.Text += "決済エラー：" + result.ErrorInfoForLog + ((string.IsNullOrEmpty(result.CreditErrorMessage) == false) ? "（" + result.CreditErrorMessage + "）" : "") + "<br/>";
		}
		return result.Success;
	}

	/// <summary>
	/// KVPリストから取得
	/// </summary>
	/// <param name="key">key</param>
	/// <returns>value</returns>
	private TextBox GetFromKvps(string key)
	{
		foreach (RepeaterItem ri in rParams.Items)
		{
			if (((TextBox)ri.FindControl("tbKey")).Text == key)
			{
				return ((TextBox)ri.FindControl("tbValue"));
			}
		}
		return null;
	}
	
	/// <summary>
	/// POSTボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPost_Click(object sender, EventArgs e)
	{
		var result = DoPost();
		{
			lSendOrderNoticeMessage.Text += "[ " + (result ? "OK" : "NG") + " ] " + tbYamatoKwcMemberId.Text + "(" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ")<br/>";
			if (result)	
			{
				tbYamatoKwcOrderNo.Text = "";
				tbYamatoKwcMemberId.Text = "";
				tbYamatoKwcAccessKey.Text = "";
				tbCheckSum.Text = "";
				tbToken.Text = "";
			}
		}
	}

	/// <summary>
	/// POST実行
	/// </summary>
	private bool DoPost()
	{
		var paramString = string.Join(
			"&",
			rParams.Items.Cast<RepeaterItem>().Select(
				ri =>
				{
					var key = ((TextBox)ri.FindControl("tbKey")).Text.Trim();
					var value = ((TextBox)ri.FindControl("tbValue")).Text.Trim();
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
			var result = string.IsNullOrEmpty(responseText);
			return result;
		}
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title></title>
	<%-- 本番 --%>
	<%--script type="text/javascript" charset="UTF-8" src="https://api.kuronekoyamato.co.jp/api/token/js/embeddedTokenLib.js"></%script--%>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<h1>YamatoKwcテレフォンオーダー向け</h1>
		<h2>YamatoKwc会員の登録を行い、POST通知する。</h2>
		カード番号：<asp:TextBox id="tbCardNo" Text="0000000000000001" Width="200" runat="server"></asp:TextBox><br/>
		１．YamatoKwc情報入力：<br/>
			受付番号：<asp:TextBox id="tbYamatoKwcOrderNo" Width="400" runat="server"></asp:TextBox>（ハイフンは削除されます）<br/>
			会員ID：<asp:TextBox id="tbYamatoKwcMemberId" Width="400" runat="server"></asp:TextBox>（ハイフンは削除されます）<br/>
			認証キー：<asp:TextBox id="tbYamatoKwcAccessKey" Width="400" runat="server"></asp:TextBox><br/>
		<asp:Button id="btnCreateCheckSum" Text="２．チェックサム作成等" runat="server" OnClick="btnCreateCheckSum_Click" />
			　　　チェックサム<asp:TextBox id="tbCheckSum" Width="600" runat="server"></asp:TextBox><br/>
		<input type="button" value="３．トークン取得" onclick="GetToken();"/>
			　　　　　　トークン：<asp:TextBox id="tbToken" Width="600" runat="server"></asp:TextBox><br/>
		<asp:Button id="btnRegisterAndPost" Text="４．登録してPOST送信" runat="server" OnClick="btnRegisterAndPost_Click" />
		　　　<asp:Button id="btnOnlyRegister" Text="（４．登録のみ）" runat="server" OnClick="btnOnlyRegister_Click" /><br/>
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
			<asp:TextBox id="tbValue" Width="200" Text='<%# Eval("Value") %>' runat="server"></asp:TextBox><br/>
		</ItemTemplate>
		</asp:Repeater>
		<br/>
		<asp:Button id="btnPost" Text="　送信　" runat="server" OnClick="btnPost_Click"/><br/>
		※YamatoKwc会員IDはハイフンありでもOK<br/>
		<br/>
	</div>
	</form>
<script>
	<%-- 保持したい値トークンセットのために保持したい値（ヤマトKWCで利用） --%>
	var paymentInfoForToken = "";

	function GetToken() {
		<%-- 保持しておきたい値をセット --%>
		var createTokenInfo = {
			traderCode: '<%= Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE %>',
			authDiv: "<%= Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE ? "2" : "0" %>",
			optServDiv: "01", // オプションサービス受注
			memberId: document.getElementById('<%= tbYamatoKwcMemberId.ClientID %>').value,
			authKey: document.getElementById('<%= tbYamatoKwcAccessKey.ClientID %>').value,
			checkSum: document.getElementById('<%= tbCheckSum.ClientID %>').value,
			cardNo: document.getElementById('<%= tbCardNo.ClientID %>').value,
			cardOwner: "DOUBLE TARO",
			cardExp: "0524",
			securityCode: "123"
		};
		
		// ｗｅｂコレクトが提供するJavaScript関数を実行し、トークンを発行する。
		WebcollectTokenLib.createToken(createTokenInfo, callbackSuccess, callbackFailure);
	}

	var callbackSuccess = function (response) {
		document.getElementById('<%= tbToken.ClientID %>').value = paymentInfoForToken + " " + response.token;
	}
	<%-- 「異常」 --%>
	var callbackFailure = function (response) {
		for (var i = 0; i < response.errorInfo.length; i++) {
			alert(response.errorInfo[i].errorCode + " : " + response.errorInfo[i].errorMsg);
		}
	};

	function getTokenCallbackYamatoKwc(result) {
		var resc = result.resultCode;
		if (resc == "000") {
			document.getElementById('<%= tbToken.ClientID %>').value = result.tokenObject.token;// + " " + result.tokenObject.toBeExpiredAt;
		} else {
			alert("error:" + resc);
		}
	}
</script>
<%-- テスト環境 --%>
<script type="text/javascript" charset="UTF-8" src="https://ptwebcollect.jp/test_gateway/token/js/embeddedTokenLib.js"></script>
</body>
</html>
