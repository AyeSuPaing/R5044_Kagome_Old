<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred.GetInvoice" %>
<script runat="server">
	void Main()
	{
		try
		{
			var reader = new StreamReader(Request.InputStream);
			var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<DskDeferredGetInvoiceRequest>(xml);
			var response = new DskDeferredGetInvoiceResponse();

			response.Result = "OK";
			response.InvoiceDataResult = new InvoiceDataResultElement();
			response.InvoiceDataResult.InvoiceBarCode = "91550056999202010010000000002099999900005001";
			response.InvoiceDataResult.InvoiceCode = "2010010000000002";
			response.InvoiceDataResult.InvoiceKbn = "1";
			response.InvoiceDataResult.HistorySeq = "0";
			response.InvoiceDataResult.RemindedKbn = "01";
			response.InvoiceDataResult.CompanyName = "";
			response.InvoiceDataResult.Department = "";
			response.InvoiceDataResult.CustomerName = "ダブル テスト";
			response.InvoiceDataResult.CustomerZip = "1040061";
			response.InvoiceDataResult.CustomerAddress1 = "東京都";
			response.InvoiceDataResult.CustomerAddress2 = "中央区銀座４ー１４ー１１";
			response.InvoiceDataResult.CustomerAddress3 = "七十七銀座ビル７階";
			response.InvoiceDataResult.ShopZip = "5410056";
			response.InvoiceDataResult.ShopAddress1 = "大阪府";
			response.InvoiceDataResult.ShopAddress2 = "大阪市中央区久太郎町３ー３ー８";
			response.InvoiceDataResult.ShopAddress3 = "カラバサ本社ビル4F";
			response.InvoiceDataResult.ShopTel = "0330001234";
			response.InvoiceDataResult.ShopFax = "0330001234";
			response.InvoiceDataResult.BilledAmount = "1080";
			response.InvoiceDataResult.Tax = "80";
			response.InvoiceDataResult.TimeOfReceipts = "2030年10月15日";
			response.InvoiceDataResult.InvoiceStartDate = "2030年10月01日";
			response.InvoiceDataResult.InvoiceTitle = "請求書";
			response.InvoiceDataResult.Message1 = "ＤＳＫ後払いをご利用いただきありがとうございます。";
			response.InvoiceDataResult.Message2 = "支払期限日までにコンビニエンスストア等でお支払いください。";
			response.InvoiceDataResult.Message3 = "商品より先に本状が到着する場合もございますが、";
			response.InvoiceDataResult.Message4 = "その際は商品をご確認いただきました後にお支払いください。";
			response.InvoiceDataResult.InvoiceShopsiteName = "サイト名１";
			response.InvoiceDataResult.ShopEmail = "w2test@example.com";
			response.InvoiceDataResult.Name = "ＤＳＫ後払いサポートセンター";
			response.InvoiceDataResult.QaUrl = "058-279-3474 受付時間 平日 9:00～17:00";
			response.InvoiceDataResult.ShopOrderDate = "2030年10月01日";
			response.InvoiceDataResult.ShopCode = request.ShopInfo.ShopCode;
			response.InvoiceDataResult.TransactionId = request.Transaction.TransactionId;
			response.InvoiceDataResult.ShopTransactionId1 = request.Transaction.ShopTransactionId;
			response.InvoiceDataResult.ShopTransactionId2 = "";
			response.InvoiceDataResult.ShopMessage1 = "";
			response.InvoiceDataResult.ShopMessage2 = "";
			response.InvoiceDataResult.ShopMessage3 = "";
			response.InvoiceDataResult.ShopMessage4 = "";
			response.InvoiceDataResult.ShopMessage5 = "";
			response.InvoiceDataResult.Yobi1 = "2";
			response.InvoiceDataResult.Yobi2 = "00140-5-901697";
			response.InvoiceDataResult.Yobi3 = "ＤＳＫ 後払い";
			response.InvoiceDataResult.Yobi4 = "000014090169700000000500200000000099920";
			response.InvoiceDataResult.Yobi5 = "24201001000000000200000000005005698700000000";
			response.InvoiceDataResult.Yobi6 = "住所等非表示払込書";
			response.InvoiceDataResult.Yobi7 = "X";

			response.DetailList = new DetailListElement();
			response.DetailList.GoodsDetail = new GoodsDetailElement[4];
			response.DetailList.GoodsDetail[0] = new GoodsDetailElement();
			response.DetailList.GoodsDetail[0].GoodsName = "ご購入商品";
			response.DetailList.GoodsDetail[0].GoodsNum = "4";
			response.DetailList.GoodsDetail[0].GoodsPrice = "200";
			response.DetailList.GoodsDetail[1] = new GoodsDetailElement();
			response.DetailList.GoodsDetail[1].GoodsName = "送料";
			response.DetailList.GoodsDetail[1].GoodsNum = "1";
			response.DetailList.GoodsDetail[1].GoodsPrice = "100";
			response.DetailList.GoodsDetail[1] = new GoodsDetailElement();
			response.DetailList.GoodsDetail[1].GoodsName = "決済手数料";
			response.DetailList.GoodsDetail[1].GoodsNum = "1";
			response.DetailList.GoodsDetail[1].GoodsPrice = "100";
			response.DetailList.GoodsDetail[1] = new GoodsDetailElement();
			response.DetailList.GoodsDetail[1].GoodsName = "割引等";
			response.DetailList.GoodsDetail[1].GoodsNum = "1";
			response.DetailList.GoodsDetail[1].GoodsPrice = "0";

			response.Errors = null;

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new DskDeferredGetInvoiceResponse();
			response.Result = "NG";
			response.Errors = new BaseDskDeferredResponse.ErrorsElement();
			if (ex.InnerException != null)
				response.Errors.Error = new [] { new BaseDskDeferredResponse.ErrorElement()
				{
					ErrorCode = "9999",
					ErrorMessage = ex.Message + " " + ((ex.InnerException == null) ? "" :  ex.InnerException.Message),
				} };
			else
			{
				response.Errors.Error = new[] { new BaseDskDeferredResponse.ErrorElement()
				{
					ErrorCode = "9999",
					ErrorMessage = "予期せぬエラーが発生しました。",
				}};
			}
			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
	}
</script>
<% this.Main(); %>
