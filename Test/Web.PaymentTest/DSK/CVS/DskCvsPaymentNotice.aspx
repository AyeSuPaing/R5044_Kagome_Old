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
			tbUrl.Text = @"http://localhost/V5.14-Release/Web/w2.Commerce.Front/Payment/GetDskCvsPayInfo.aspx";
			rParams.DataSource = new[] {
				new KeyValuePair<string, string>("dskid", ""),
				new KeyValuePair<string, string>("paydate", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new KeyValuePair<string, string>("orderid", ""),
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
		webRequest.ContentType = @"application/x-www-form-urlencoded";
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
			var result = (responseText == "<SENBDATA>STATUS=800</SENBDATA>");
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
	<title>電算システム コンビニ前払い 入金テスト</title>
</head>
<body>
<form id="form1" runat="server">
	<div>
		電算システム コンビニ前払い 入金テスト
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
		備考:
		<pre>
dskid:注文管理ID(w2の決済取引ID) 任意
paydate:店舗入金日時 yyyyMMddHHmmss
orderid:お客様注文番号(w2の注文ID)
		</pre>
	</div>
</form>
</body>
</html>