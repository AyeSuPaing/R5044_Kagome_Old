<%--
=========================================================================================================
  Module      : Zeus Cvs Payment Notice (ZeusCvsPaymentNotice.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>

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
			tbUrl.Text = @"http://localhost/V5.14/Web/w2.Commerce.Front/Payment/Zeus/PaymentZeusCvsPaymentRecv.ashx";
			rParams.DataSource = new[] {
				new KeyValuePair<string, string>("order_no", ""),
				new KeyValuePair<string, string>("sendid", ""),
				new KeyValuePair<string, string>("status", "04"),	// 決済状況
			};
			rParams.DataBind();
		}
	}

	/// <summary>
	/// POST実行
	/// </summary>
	/// <returns>Response text: OK | NG</returns>
	private string DoPost()
	{
		var encoding = Encoding.GetEncoding("Shift_JIS");
		var paramString = string.Join("&", rParams.Items.Cast<RepeaterItem>().Select(ri =>
		{
			var key = ((TextBox)ri.FindControl("tbKey")).Text.Trim();
			var value = ((TextBox)ri.FindControl("tbValue")).Text.Trim();
			return string.Format("{0}={1}", key, value, encoding);
		}));
		var postData = encoding.GetBytes(paramString);

		var webRequest = (HttpWebRequest)WebRequest.Create(tbUrl.Text);
		webRequest.Method = WebRequestMethods.Http.Post;
		webRequest.ContentType = @"application/x-www-form-urlencoded";
		webRequest.ContentLength = postData.Length;

		// 送信データの書き込み
		var postStream = webRequest.GetRequestStream();
		// 送信するデータを書き込む
		postStream.Write(postData, 0, postData.Length);
		postStream.Close();

		// レスポンス取得
		using (var responseStream = webRequest.GetResponse().GetResponseStream())
		using (var sr = new StreamReader(responseStream, encoding))
		{
			var responseText = sr.ReadToEnd();
			return responseText;
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
			lSendOrderNoticeMessage.Text += "[ " + result + " ] " + "(" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ")<br/>";
		}
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>ZEUS コンビニ前払い 入金テスト</title>
</head>
<body>
<form id="form1" runat="server">
	<div>
		ZEUS コンビニ前払い 入金テスト
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
		<asp:Button ID="btnPost" Text="　送信　" runat="server" OnClick="btnPost_Click"/>
		<br/>
		<hr/>
		<asp:Literal id="lSendOrderNoticeMessage" runat="server"></asp:Literal>
		<br/>
		<hr/>
		備考:
		<pre>
order_no:決済取引ID
sendid:注文ID

status:
決済状況
01：未入金
02：申込エラー
03：期日切
04：入金済
05：売上確定
06：入金取消
11：キャンセル後入金
12：キャンセル後売上
13：キャンセル後取消

・組み合わせ
			switch (status)
			{
				case "04":
				case "05":
					// 入金処理（未入金→入金済み）
					break;

				case "01":
				case "02":
				case "03":
				case "06":
				case "11":
				case "12":
				case "13":
					// 入金戻し（入金済み→未入金）
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(this.Status), this.Status, null);
			}
		</pre>
	</div>
</form>
</body>
</html>