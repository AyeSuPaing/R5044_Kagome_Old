<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.GetinvoicePrintData" %>
<script runat="server">
void Main()
{
	StreamReader reader = new StreamReader(Request.InputStream);
	string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

	try
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestGetinvoicePrintData>(xml);
		var response = new GmoResponseGetinvoicePrintData();
		response.Result = ResultCode.OK;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "", ErrorMessage = "" } };
		response.InvoiceDataResult = new InvoiceDataResultElement();
		response.InvoiceDataResult.AccountNumber = "123456";
		response.InvoiceDataResult.BankAccountHolder = "テスト名義人";
		response.InvoiceDataResult.BankCode = "9999";
		response.InvoiceDataResult.BankName = "テスト銀行";
		response.InvoiceDataResult.BankNoteWording = "注意文言";
		response.InvoiceDataResult.BilledAmount = "9999";
		response.InvoiceDataResult.BilledAmountTax = "10";
		response.InvoiceDataResult.BranchCode = "01";
		response.InvoiceDataResult.BranchName = "銀座支店";
		response.InvoiceDataResult.DeliveryAddress1 = "東京都中央区銀座";
		response.InvoiceDataResult.DeliveryAddress2 = "4-14-11";
		response.InvoiceDataResult.DeliveryZip = "104-0061";
		response.InvoiceDataResult.DepositType = "普通";
		response.InvoiceDataResult.DocketBilledAmount = "10009";
		response.InvoiceDataResult.DocketPurchaseAddress = "東京都中央区銀座";
		response.InvoiceDataResult.DocketPurchaseCompanyName = "テストカンパニー";
		response.InvoiceDataResult.DocketPurchaseDepartmentName = "テスト部";
		response.InvoiceDataResult.DocketPurchaseUserName = "てすてすと";
		response.InvoiceDataResult.DocketTrackingNumber = "1234567890";
		response.InvoiceDataResult.DocketX = "";
		response.InvoiceDataResult.GmoCompanyName = "GmoPS";
		response.InvoiceDataResult.GmoInfo1 = "";
		response.InvoiceDataResult.GmoInfo2 = "";
		response.InvoiceDataResult.GmoInfo3 = "";
		response.InvoiceDataResult.GmoInfo4 = "";
		response.InvoiceDataResult.GmoTransactionId = request.Transaction.GmoTransactionId;
		response.InvoiceDataResult.InvoiceGreeting1 = "";
		response.InvoiceDataResult.InvoiceGreeting2 = "";
		response.InvoiceDataResult.InvoiceGreeting3 = "";
		response.InvoiceDataResult.InvoiceGreeting4 = "";
		response.InvoiceDataResult.InvoiceIssueDate = DateTime.Now.ToString("yyyy/MM/dd");
		response.InvoiceDataResult.InvoiceMatter1 = "";
		response.InvoiceDataResult.InvoiceMatter2 = "";
		response.InvoiceDataResult.InvoiceMatter3 = "";
		response.InvoiceDataResult.InvoiceMatter4 = "";
		response.InvoiceDataResult.InvoiceMatter5 = "";
		response.InvoiceDataResult.InvoiceTitle = "後払い請求書";
		response.InvoiceDataResult.OrderDate = DateTime.Now.ToString("yyyy/MM/dd");
		response.InvoiceDataResult.PaymentDueDate = DateTime.Now.AddDays(7).ToString("yyyy/MM/dd");
		response.InvoiceDataResult.PurchaseCompanyName = "";
		response.InvoiceDataResult.PurchaseDepartmentName = "";
		response.InvoiceDataResult.PurchaseUserName = "テスト購入者";
		response.InvoiceDataResult.ReceiptAmount = "10009";
		response.InvoiceDataResult.ReceiptPrintWord = "印紙文言";
		response.InvoiceDataResult.ReceiptPurchaseCompanyName = "テスト123";
		response.InvoiceDataResult.ReceiptPurchaseDepartmentName = "部署";
		response.InvoiceDataResult.ReceiptPurchaseUserName = "テスト太郎";
		response.InvoiceDataResult.ReceiptShopName = "テスト加盟店";
		response.InvoiceDataResult.ReceiptTax = "9";
		response.InvoiceDataResult.ReceiptTrackingNumber1 = "1234567890";
		response.InvoiceDataResult.ReceiptTrackingNumber2 = "9876543210";
		response.InvoiceDataResult.ShopName = "テストショップ";
		response.InvoiceDataResult.ShopTransactionId = "123456";
		response.InvoiceDataResult.String = "";
		response.InvoiceDataResult.TrackingNumber = "11223344";
		response.InvoiceDataResult.VotesBarCode = "";
		response.InvoiceDataResult.VotesBilledAmount = "10009";
		response.InvoiceDataResult.VotesFontKiwerInfo = "";
		response.InvoiceDataResult.VotesFontUpperInfo = "";
		response.InvoiceDataResult.VotesPaymentDueDate = DateTime.Now.AddDays(7).ToString("yyyy/MM/dd");
		response.InvoiceDataResult.VotesPurchaseUserName = "テスト";
		response.InvoiceDataResult.VotesTrackingNumber = "123456";
		response.InvoiceDataResult.Yobi1 = "";
		response.InvoiceDataResult.Yobi2 = "";
		response.InvoiceDataResult.Yobi3 = "";
		response.InvoiceDataResult.Yobi4 = "";
		response.InvoiceDataResult.Yobi5 = "";
		response.InvoiceDataResult.Yobi6 = "";
		response.InvoiceDataResult.Yobi7 = "";
		response.InvoiceDataResult.Yobi8 = "";
		response.InvoiceDataResult.Yobi9 = "";
		response.InvoiceDataResult.Yobi10 = "";
		response.InvoiceDataResult.Yobi11 = "";
		response.InvoiceDataResult.Yobi12 = "";
		response.InvoiceDataResult.Yobi13 = "";
		response.InvoiceDataResult.Yobi14 = "";
		response.InvoiceDataResult.Yobi15 = "";

		response.InvoiceDataResult.DetailList = new DetailListElement();
		response.InvoiceDataResult.DetailList.GoodsDetail = new GoodsDetailElement[]{new GoodsDetailElement()};
		response.InvoiceDataResult.DetailList.GoodsDetail[0].GoodsAmount = "9999";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].GoodsAmountTax = "10";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].GoodsName = "商品X";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].GoodsNum = "1";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].GoodsPrice = "9999";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].Yobi16 = "";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].Yobi17 = "";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].Yobi18 = "";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].Yobi19 = "";
		response.InvoiceDataResult.DetailList.GoodsDetail[0].Yobi20 = "";
		
		response.Result = ResultCode.OK;
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new GmoResponseGetinvoicePrintData();
		response.Result = ResultCode.NG;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "9999", ErrorMessage = ex.Message } };
		response.InvoiceDataResult = new InvoiceDataResultElement();
		response.InvoiceDataResult.DetailList = new DetailListElement();
		response.InvoiceDataResult.DetailList.GoodsDetail = new GoodsDetailElement[]{new GoodsDetailElement()};
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>