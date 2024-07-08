<%@ Page Language="C#" %>

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
			tbUrl.Text = @"http://localhost/V5.14/Web/w2.Commerce.Front/Payment/Card3DSecureAuthVeriTrans/GetCard3DSecureAuthResultReceive.aspx";
			rParams.DataSource = new[] {
				new KeyValuePair<string, string>("numberOfNotify", "1"),
				new KeyValuePair<string, string>("pushTime", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new KeyValuePair<string, string>("pushId", "00000001"),
				new KeyValuePair<string, string>("orderId0000", "PDEVR0000000001"),
				new KeyValuePair<string, string>("vResultCode0000", "A00100000000"),
				new KeyValuePair<string, string>("txnType0000", "AuthorizeConfirm "),
				new KeyValuePair<string, string>("mpiMstatus0000", "success"),
				new KeyValuePair<string, string>("cardMstatus0000", "success"),
				new KeyValuePair<string, string>("dummy0000", "1"),
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
			var result = (responseText == "0");
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
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title>Veritrans結果通知テスト</title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			Veritrans結果通知テスト
		<hr />
			URL：<asp:TextBox ID="tbUrl" Width="800" runat="server"></asp:TextBox><br />
			PARAM<br />
			<asp:Repeater ID="rParams" runat="server">
				<ItemTemplate>
					<asp:TextBox ID="tbKey" Width="100" Text='<%# Eval("Key") %>' runat="server"></asp:TextBox>
					=
					<asp:TextBox ID="tbValue" Width="200" Text='<%# Eval("Value") %>' runat="server"></asp:TextBox><br />
				</ItemTemplate>
			</asp:Repeater>
			<br />
			<asp:Button Text="　送信　" runat="server" OnClick="btnPost_Click" />
			<br />
			<hr />
			<asp:Literal ID="lSendOrderNoticeMessage" runat="server"></asp:Literal>
			<br />
			<hr />
			備考:
		<pre>
OrderID:決済注文ID
		</pre>
		</div>
	</form>
</body>
</html>
