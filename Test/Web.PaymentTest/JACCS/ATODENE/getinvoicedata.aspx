<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.JACCS.ATODENE" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice" %>
<script runat="server">
	void Main()
	{
		try
		{
			StreamReader reader = new StreamReader(Request.InputStream);
			string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<AtodeneGetInvoiceRequest>(xml);
			var response = new AtodeneGetInvoiceResponse();

			response.Result = "OK";
			response.InvoiceInfo = new AtodeneGetInvoiceResponse.InvoiceInfoElement();

			response.InvoiceInfo.Zip = "104-0061";
			response.InvoiceInfo.Address1 = "東京都中央区銀座４の１４の１１";
			response.InvoiceInfo.Address2 = "七十七銀座ビル７階";
			response.InvoiceInfo.Companyname = "ｗ２ソリューション";
			response.InvoiceInfo.Sectionname = "パッケージサービス";
			response.InvoiceInfo.Name = "ほげほげお";
			response.InvoiceInfo.Sitenametitle = "ご購入店舗";
			response.InvoiceInfo.Sitename = "ほげげ店";
			response.InvoiceInfo.Shoporderidtitle = "注文番号";
			response.InvoiceInfo.Shoporderid = "1234567890";
			response.InvoiceInfo.Descriptiontext1 = "特記事項壱";
			response.InvoiceInfo.Descriptiontext2 = "特記事項弐";
			response.InvoiceInfo.Descriptiontext3 = "特記事項参";
			response.InvoiceInfo.Descriptiontext4 = "特記事項肆";
			response.InvoiceInfo.Descriptiontext5 = "特記事項伍";
			response.InvoiceInfo.Billservicename = "発行元企業名";
			response.InvoiceInfo.Billserviceinfo1 = "発行元情報壱";
			response.InvoiceInfo.Billserviceinfo2 = "発行元情報弐";
			response.InvoiceInfo.Billserviceinfo3 = "発行元情報参";
			response.InvoiceInfo.Billserviceinfo4 = "発行元情報肆";
			response.InvoiceInfo.Billstate1 = "ご請求書";
			response.InvoiceInfo.Billfirstgreet1 = "あいさつ文壱";
			response.InvoiceInfo.Billfirstgreet2 = "あいさつ文弐";
			response.InvoiceInfo.Billfirstgreet3 = "あいさつ文参";
			response.InvoiceInfo.Billfirstgreet4 = "あいさつ文肆";
			response.InvoiceInfo.Expand1 = "予備壱";
			response.InvoiceInfo.Expand2 = "予備弐";
			response.InvoiceInfo.Expand3 = "予備参";
			response.InvoiceInfo.Expand4 = "予備肆";
			response.InvoiceInfo.Expand5 = "予備伍";
			response.InvoiceInfo.Expand6 = "予備陸";
			response.InvoiceInfo.Expand7 = "予備漆";
			response.InvoiceInfo.Expand8 = "予備捌";
			response.InvoiceInfo.Expand9 = "予備玖";
			response.InvoiceInfo.Expand10 = "予備拾";
			response.InvoiceInfo.Billedamounttitle = "ご請求金額";
			response.InvoiceInfo.Billedamount = "1080円";
			response.InvoiceInfo.Billedfeetax = "内消費税：80円";
			response.InvoiceInfo.Billorderdaytitle = "ご注文日";
			response.InvoiceInfo.Shoporderdate = "2030年12月10日";
			response.InvoiceInfo.Billsenddatetitle = "請求書発行日";
			response.InvoiceInfo.Billsenddate = "2030年12月11日";
			response.InvoiceInfo.Billdeadlinedatetitle = "お支払期日";
			response.InvoiceInfo.Billdeadlinedate = "2030年12月21日";
			response.InvoiceInfo.Transactionidtitle = "お問い合わせ番号";
			response.InvoiceInfo.Transactionid = "1234567890";
			response.InvoiceInfo.Billbankinfomation = "銀行にてお支払いをする場合、ご請求金額を確認の上、下記の口座へお振込み下さい。ご注文毎に口座番号が異なります。別注文時の口座へのお振込みや合算はご遠慮下さい。";
			response.InvoiceInfo.Banknametitle = "銀行名";
			response.InvoiceInfo.Bankname = "ぴよよ銀行";
			response.InvoiceInfo.Bankcode = "9999";
			response.InvoiceInfo.Branchnametitle = "支店名";
			response.InvoiceInfo.Branchname = "ぴよ支店";
			response.InvoiceInfo.Branchcode = "999";
			response.InvoiceInfo.Bankaccountnumbertitle = "口座番号";
			response.InvoiceInfo.Bankaccountkind = "（普通）";
			response.InvoiceInfo.Bankaccountnumber = "1234567";
			response.InvoiceInfo.Bankaccountnametitle = "口座名義";
			response.InvoiceInfo.Bankaccountname = "ﾎｹﾞﾎｹﾞﾌｶﾞｶﾞ";
			response.InvoiceInfo.Receiptbilldeadlinedate = "2000年07月01日";
			response.InvoiceInfo.Receiptname = "ほげぴよこ";
			response.InvoiceInfo.Invoicebarcode = "91929023111112222222222222222399999912100009";
			response.InvoiceInfo.Receiptcompanytitle = "収納代行";
			response.InvoiceInfo.Receiptcompany = "株式会社もげげ";
			response.InvoiceInfo.Docketbilledamount = "1080円";
			response.InvoiceInfo.Docketcompanyname = "株式会社ほげほげ";
			response.InvoiceInfo.Docketsectionname = "ほげ";
			response.InvoiceInfo.Docketname = "ぴよ";
			response.InvoiceInfo.Dockettransactionidtitle = "お問い合わせ番号";
			response.InvoiceInfo.Dockettransactionid = "111111111111";
			response.InvoiceInfo.Vouchercompanyname = "株式会社ちょめめ";
			response.InvoiceInfo.Vouchersectionname = "ほげ";
			response.InvoiceInfo.Vouchercustomerfullname = "ぴよ";
			response.InvoiceInfo.Vouchertransactionidtitle = "お問い合わせ番号";
			response.InvoiceInfo.Vouchertransactionid = "111111111";
			response.InvoiceInfo.Voucherbilledamount = "1080円";
			response.InvoiceInfo.Voucherbilledfeetax = "内消費税：80円";
			response.InvoiceInfo.Revenuestamprequired = "収入印紙不要";
			response.InvoiceInfo.Goodstitle = "明細内容";
			response.InvoiceInfo.Goodsamounttitle = "注文数";
			response.InvoiceInfo.Goodspricetitle = "単価";
			response.InvoiceInfo.Goodssubtotaltitle = "金額";

			var details = new AtodeneGetInvoiceResponse.DetailsElement();
			var d1 = new AtodeneGetInvoiceResponse.DetailElement();
			var d2 = new AtodeneGetInvoiceResponse.DetailElement();
			d1.Goods = "ご購入商品";
			d1.GoodsAmount = "4";
			d1.GoodsPrice = "200";
			d1.GoodsSubtotal = "800";
			d1.GoodsExpand = "備考1";
			d2.Goods = "手数料等";
			d2.GoodsAmount = "1";
			d2.GoodsPrice = "200";
			d2.GoodsSubtotal = "200";
			d2.GoodsExpand = "備考2";
			details.Detail = new AtodeneGetInvoiceResponse.DetailElement[] { d1, d2 };
			response.InvoiceInfo.Details = details;

			response.InvoiceInfo.Detailinfomation = "注意事項";
			response.InvoiceInfo.Expand11 = "1234568790";
			response.InvoiceInfo.Expand12 = "ぴよたろう";
			response.InvoiceInfo.Expand13 = "1234567890";
			response.InvoiceInfo.Expand14 = "0987654321";
			response.InvoiceInfo.Expand15 = "ほげほげ";
			response.InvoiceInfo.Expand16 = "*";
			response.InvoiceInfo.Expand17 = "よび１７";
			response.InvoiceInfo.Expand18 = "よび１８";
			response.InvoiceInfo.Expand19 = "予備１９";
			response.InvoiceInfo.Expand20 = "よび２０";

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new AtodeneGetInvoiceResponse();
			response.Result = "NG";
			response.Errors = new BaseAtodeneResponse.ErrorsElement();
			if (ex.InnerException != null)
				response.Errors.Error = new BaseAtodeneResponse.ErrorElement[] { new BaseAtodeneResponse.ErrorElement()
				{
					ErrorCode = "9999",
					ErrorMessage = ex.Message + " " + ((ex.InnerException == null) ? "" :  ex.InnerException.Message),
					ErrorPoint = "exception"
				} };
			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
	}
</script>
<% this.Main(); %>
