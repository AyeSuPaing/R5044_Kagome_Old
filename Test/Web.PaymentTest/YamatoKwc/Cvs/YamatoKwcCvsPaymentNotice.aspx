<%@ Page Language="C#"%>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>

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
			tbUrl.Text = @"http://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/Payment/YamatoKwc/PaymentYamatoKwcCvsPaymentRecv.ashx";
			rParams.DataSource = new[] {
				new KeyValuePair<string, string>("trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE),
				new KeyValuePair<string, string>("order_no", ""),
				new KeyValuePair<string, string>("settle_price", ""),
				new KeyValuePair<string, string>("settle_date", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new KeyValuePair<string, string>("settle_result", "1"),	// 正常
				new KeyValuePair<string, string>("settle_detail", "2"),	// 入金完了（速報）
				new KeyValuePair<string, string>("settle_method", "21"),// セブン-イレブン
			};
			rParams.DataBind();
		}
	}

	/// <summary>
	/// POST実行
	/// </summary>
	private bool DoPost()
	{
		var paramString = string.Join("&", rParams.Items.Cast<RepeaterItem>().Select(ri =>
		{
			var key = ((TextBox)ri.FindControl("tbKey")).Text.Trim();
			var value = ((TextBox)ri.FindControl("tbValue")).Text.Trim();
			return string.Format("{0}={1}", key, value, Encoding.UTF8);
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

	/// <summary>
	/// POSTボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPost_Click(object sender, EventArgs e)
	{
		var result = DoPost();
		{
			lSendOrderNoticeMessage.Text += "[ " + (result ? "OK" : "NG") + " ] " + "(" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ")<br/>";
		}
	}

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>ヤマトKWC コンビニ前払い 入金テスト</title>
</head>
<body>
<form id="form1" runat="server">
	<div>
		ヤマトKWC コンビニ前払い 入金テスト
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
		<asp:Button Text="　送信　" runat="server" OnClick="btnPost_Click"/>
		<br/>
		<hr/>
		<asp:Literal id="lSendOrderNoticeMessage" runat="server"></asp:Literal>
		<br/>
		<hr/>
		備考:
		<pre>
trader_code:加盟店コード
order_no:注文番号
settle_price:入金金額　差分がある場合は入金ステータスは更新されません
settle_date:yyyyMMddHHmmss
settle_result:
正常時：1
異常時：2 

settle_detail:
 正常時
2：入金完了（速報）
3：入金完了（確報）
 異常時
11：購入者都合エラー（支払期限切れ、コンビニエンススト
アから入金取消の通知が発生した場合等）
13：決済機関都合エラー（コンビニエンスストアより応答が
 無い場合、異常の応答を受けた場合等）
14：その他システムエラー

settle_method:
クレジットカード会社
1：ＵＣ
2：ダイナース
3：ＪＣＢ
4：ＤＣ
5：三井住友クレジット
6：ＵＦＪ
7：クレディセゾン
8：ＮＩＣＯＳ
9：ＶＩＳＡ
10：ＭＡＳＴＥＲ
11：イオンクレジット
12：アメックス
13：ＴＯＰ＆カード
14：楽天カード

コンビニエンスストア
21：セブン-イレブン
22：ローソン
23：ファミリーマート
24：セイコーマート
25：ミニストップ
26：サークルＫサンクス

・組み合わせ
			switch (settleMethod)
			{
				// クレカは精算確定になっていたら入金OK
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "10":
				case "11":
				case "12":
				case "13":
				case "14":
					return (settleDetail == "31");

				// セブンイレブン・ファミリーマートは速報で入金OKとする
				case "21":
				case "23":
					return (settleDetail == "2");

				// その他コンビニは速報未対応なので、確報時に入金OKとする
				case "22":
				case "24":
				case "25":
				case "26":
					return (settleDetail == "3");
			}
			return false;
		</pre>
	</div>
</form>
</body>
</html>