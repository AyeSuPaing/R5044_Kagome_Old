<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Page Language="C#" ResponseEncoding="Shift_JIS" Inherits="BasePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

	string m_orderUrl = "https://stbfep.sps-system.com/f01/FepBuyInfoReceive.do";	// 試験環境
	//string m_orderUrl = "https://stbfep.sps-system.com/Extra/BuyRequestAction.do";	// 詳細検証用
	//string m_orderUrl = "../SBPS_TEST/SBPS_ReceiveMultiPaymentOrder.aspx";	// ローカル

#if DEBUG
		string m_siteRootUrl = "http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/_Dev/SBPS/";
#else
	string m_siteRootUrl = "https://tryout.w2solution.com/SoftbankPayment/SBPS/";
#endif
		
	
	protected void Page_Load(object sender, EventArgs e)
	{
		PaymentSBPSMultiPaymentExecOrder payment = new PaymentSBPSMultiPaymentExecOrder();

		string userId = "1234567890123456";
		string orderId = "1234567890" + DateTime.Now.ToString("yyyyMMddHHmmss");
		string itemId = "110";
		string itemName = "ご購入商品";
		List<PaymentSBPSCreditAuthApi.ProductItem> productItems = new List<PaymentSBPSCreditAuthApi.ProductItem>();
		productItems.Add(new PaymentSBPSCreditAuthApi.ProductItem("S-111", "商品名１", 1, 150));
		productItems.Add(new PaymentSBPSCreditAuthApi.ProductItem("S-222", "商品名２", 2, 100));
		Decimal priceTotal = new Decimal(350);
		bool isMobile = false;	// ケータイか
		string orderCompleUrl = m_siteRootUrl + "SBPS/PaymentSBPSMultiPaymentReceiveOrderResult.aspx";
		string orderCancelUrl = m_siteRootUrl + "SBPS/PaymentSBPSMultiPaymentReceiveOrderCancel.aspx";
		string errorUrl = m_siteRootUrl + "SBPS/PaymentSBPSMultiPaymentReceiveOrderError.aspx";
		string paymentNoticeUrl = m_siteRootUrl + "SBPS/PaymentSBPSMultiPaymentReceiveOrderNotice.aspx";

		lFormInputs.Text = payment.CreateOrderFromInputs(
			PaymentSBPSTypes.PayMethodTypes.softbank2,
			userId, orderId, itemId, itemName, productItems, priceTotal, isMobile, orderCompleUrl, orderCancelUrl, errorUrl, paymentNoticeUrl);
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>

<form name="form" action='<%= m_orderUrl %>' method="post">
<asp:Literal ID="lFormInputs" runat="server"></asp:Literal>
<input type="submit" value="ソフトバンクケータイプロジェクト内テスト" style="width:200px;height:100px" />
</form>

</body>
</html>
