<%@ Page Language="C#" %>
<%@ Import Namespace="w2.App.Common.Order" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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
			tbUrl.Text = Request["requrl"] ?? "https://tryout.w2solution.com/SoftbankPaymentTest/_m/Payment/SBPS/PaymentSBPSCvsPaymentRecv.aspx";
			tbOrderId.Text = Request["orderid"] ?? "SBPSTEST2012053000015";
			tbTrackingId.Text = Request["trackid"] ?? "35107140161933";
		}
	}
	
	/// <summary>
	/// 送信ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void send_Click(object sender, EventArgs e)
	{
		var settings = new PaymentSBPSSetting(
			tbUrl.Text,
			Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL,
			Constants.PAYMENT_SETTING_SBPS_MERCHANT_ID,
			Constants.PAYMENT_SETTING_SBPS_SERVICE_ID,
			Constants.PAYMENT_SETTING_SBPS_HASHKEY,
			Constants.PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_ID,
			Constants.PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_PASSWORD,
			Constants.PAYMENT_SETTING_SBPS_3DES_KEY,
			Constants.PAYMENT_SETTING_SBPS_3DES_IV);

		PaymentSBPSCvsPaymentRevc_ReqApi api = new PaymentSBPSCvsPaymentRevc_ReqApi(settings);
		XDocument result = api.Exec(tbOrderId.Text, tbTrackingId.Text);

		message.Text = HttpUtility.HtmlEncode(result).Replace("\n", "<br />");
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
	<h1>コンビニ入金テスト</h1>
    <div>
		送信先URL：<asp:TextBox ID="tbUrl" Text="" Width="800" runat="server"></asp:TextBox><br />
		注文ID：<asp:TextBox ID="tbOrderId" Width="400" Text="" runat="server"></asp:TextBox><br />
		トラッキングID：<asp:TextBox ID="tbTrackingId" Width="400" Text="" runat="server"></asp:TextBox><br />
		<br />
		<asp:Button id="send" Text="　送信　" Width="200" Height="200" runat="server" onclick="send_Click" />
		<br />
		<asp:Literal ID="message" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
