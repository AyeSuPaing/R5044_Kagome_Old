/*
=========================================================================================================
  Module      : Atodene請求書印字データ取得レスポンス(AtodeneGetInvoiceResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Xml.Serialization;
using w2.Common.Sql;
using w2.Domain.Atodene;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice
{
	/// <summary>
	/// Atodene請求書印字データ取得レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AtodeneGetInvoiceResponse : BaseAtodeneResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneGetInvoiceResponse()
			: base()
		{

		}

		/// <summary>
		/// 請求書を登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">トランザクションを内包するSQLアクセサ</param>
		public void InsertInvoice(string orderId, SqlAccessor accessor)
		{
			var model = ToModel();
			model.OrderId = orderId;
			foreach (var detail in model.Details)
			{
				detail.OrderId = orderId;
			}

			new AtodeneService().Insert(model, accessor);
		}

		/// <summary>
		/// モデルにする
		/// </summary>
		/// <returns>モデル</returns>
		public InvoiceAtodeneModel ToModel()
		{
			return this.InvoiceInfo.ToDbModel();
		}

		/// <summary>請求書印字情報</summary>
		[XmlElement("invoiceInfo")]
		public InvoiceInfoElement InvoiceInfo { get; set; }

		/// <summary>
		/// 請求書印字情報要素
		/// </summary>
		public class InvoiceInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public InvoiceInfoElement()
			{
			}

			/// <summary>
			/// モデルに変換
			/// </summary>
			/// <returns>モデル</returns>
			public InvoiceAtodeneModel ToDbModel()
			{
				var model = new InvoiceAtodeneModel
				{
					OrderId = string.Empty,
					Zip = this.Zip,
					Address1 = this.Address1,
					Address2 = this.Address2,
					Companyname = this.Companyname,
					Sectionname = this.Sectionname,
					Name = this.Name,
					Sitenametitle = this.Sitenametitle,
					Sitename = this.Sitename,
					Shoporderidtitle = this.Shoporderidtitle,
					Shoporderid = this.Shoporderid,
					Descriptiontext1 = this.Descriptiontext1,
					Descriptiontext2 = this.Descriptiontext2,
					Descriptiontext3 = this.Descriptiontext3,
					Descriptiontext4 = this.Descriptiontext4,
					Descriptiontext5 = this.Descriptiontext5,
					Billservicename = this.Billservicename,
					Billserviceinfo1 = this.Billserviceinfo1,
					Billserviceinfo2 = this.Billserviceinfo2,
					Billserviceinfo3 = this.Billserviceinfo3,
					Billserviceinfo4 = this.Billserviceinfo4,
					Billstate1 = this.Billstate1,
					Billfirstgreet1 = this.Billfirstgreet1,
					Billfirstgreet2 = this.Billfirstgreet2,
					Billfirstgreet3 = this.Billfirstgreet3,
					Billfirstgreet4 = this.Billfirstgreet4,
					Expand1 = this.Expand1,
					Expand2 = this.Expand2,
					Expand3 = this.Expand3,
					Expand4 = this.Expand4,
					Expand5 = this.Expand5,
					Expand6 = this.Expand6,
					Expand7 = this.Expand7,
					Expand8 = this.Expand8,
					Expand9 = this.Expand9,
					Expand10 = this.Expand10,
					Billedamounttitle = this.Billedamounttitle,
					Billedamount = this.Billedamount,
					Billedfeetax = this.Billedfeetax,
					Billorderdaytitle = this.Billorderdaytitle,
					Shoporderdate = this.Shoporderdate,
					Billsenddatetitle = this.Billsenddatetitle,
					Billsenddate = this.Billsenddate,
					Billdeadlinedatetitle = this.Billdeadlinedatetitle,
					Billdeadlinedate = this.Billdeadlinedate,
					Transactionidtitle = this.Transactionidtitle,
					Transactionid = this.Transactionid,
					Billbankinfomation = this.Billbankinfomation,
					Banknametitle = this.Banknametitle,
					Bankname = this.Bankname,
					Bankcode = this.Bankcode,
					Branchnametitle = this.Branchnametitle,
					Branchname = this.Branchname,
					Branchcode = this.Branchcode,
					Bankaccountnumbertitle = this.Bankaccountnumbertitle,
					Bankaccountkind = this.Bankaccountkind,
					Bankaccountnumber = this.Bankaccountnumber,
					Bankaccountnametitle = this.Bankaccountnametitle,
					Bankaccountname = this.Bankaccountname,
					Receiptbilldeadlinedate = this.Receiptbilldeadlinedate,
					Receiptname = this.Receiptname,
					Invoicebarcode = this.Invoicebarcode,
					Receiptcompanytitle = this.Receiptcompanytitle,
					Receiptcompany = this.Receiptcompany,
					Docketbilledamount = this.Docketbilledamount,
					Docketcompanyname = this.Docketcompanyname,
					Docketsectionname = this.Docketsectionname,
					Docketname = this.Docketname,
					Dockettransactionidtitle = this.Dockettransactionidtitle,
					Dockettransactionid = this.Dockettransactionid,
					Vouchercompanyname = this.Vouchercompanyname,
					Vouchersectionname = this.Vouchersectionname,
					Vouchercustomerfullname = this.Vouchercustomerfullname,
					Vouchertransactionidtitle = this.Vouchertransactionidtitle,
					Vouchertransactionid = this.Vouchertransactionid,
					Voucherbilledamount = this.Voucherbilledamount,
					Voucherbilledfeetax = this.Voucherbilledfeetax,
					Revenuestamprequired = this.Revenuestamprequired,
					Goodstitle = this.Goodstitle,
					Goodsamounttitle = this.Goodsamounttitle,
					Goodspricetitle = this.Goodspricetitle,
					Goodssubtotaltitle = this.Goodssubtotaltitle,
					Detailinfomation = this.Detailinfomation,
					Expand11 = this.Expand11,
					Expand12 = this.Expand12,
					Expand13 = this.Expand13,
					Expand14 = this.Expand14,
					Expand15 = this.Expand15,
					Expand16 = this.Expand16,
					Expand17 = this.Expand17,
					Expand18 = this.Expand18,
					Expand19 = this.Expand19,
					Expand20 = this.Expand20,
					Details = this.Details.ToDbModel()
				};

				return model;
			}

			/// <summary>郵便番号</summary>
			[XmlElement("zip")]
			public string Zip { get; set; }

			/// <summary>住所1</summary>
			[XmlElement("address1")]
			public string Address1 { get; set; }

			/// <summary>住所2</summary>
			[XmlElement("address2")]
			public string Address2 { get; set; }

			/// <summary>会社名</summary>
			[XmlElement("companyName")]
			public string Companyname { get; set; }

			/// <summary>部署名</summary>
			[XmlElement("sectionName")]
			public string Sectionname { get; set; }

			/// <summary>氏名</summary>
			[XmlElement("name")]
			public string Name { get; set; }

			/// <summary>加盟店名タイトル</summary>
			[XmlElement("siteNameTitle")]
			public string Sitenametitle { get; set; }

			/// <summary>請求書記載店舗名</summary>
			[XmlElement("siteName")]
			public string Sitename { get; set; }

			/// <summary>加盟店取引IDタイトル</summary>
			[XmlElement("shopOrderIdTitle")]
			public string Shoporderidtitle { get; set; }

			/// <summary>ご購入店受注番号</summary>
			[XmlElement("shopOrderId")]
			public string Shoporderid { get; set; }

			/// <summary>請求書記載事項1</summary>
			[XmlElement("descriptionText1")]
			public string Descriptiontext1 { get; set; }

			/// <summary>請求書記載事項2</summary>
			[XmlElement("descriptionText2")]
			public string Descriptiontext2 { get; set; }

			/// <summary>請求書記載事項3</summary>
			[XmlElement("descriptionText3")]
			public string Descriptiontext3 { get; set; }

			/// <summary>請求書記載事項4</summary>
			[XmlElement("descriptionText4")]
			public string Descriptiontext4 { get; set; }

			/// <summary>請求書記載事項5</summary>
			[XmlElement("descriptionText5")]
			public string Descriptiontext5 { get; set; }

			/// <summary>請求書発行元企業名</summary>
			[XmlElement("billServiceName")]
			public string Billservicename { get; set; }

			/// <summary>請求書発行元情報1</summary>
			[XmlElement("billServiceInfo1")]
			public string Billserviceinfo1 { get; set; }

			/// <summary>請求書発行元情報2</summary>
			[XmlElement("billServiceInfo2")]
			public string Billserviceinfo2 { get; set; }

			/// <summary>請求書発行元情報3</summary>
			[XmlElement("billServiceInfo3")]
			public string Billserviceinfo3 { get; set; }

			/// <summary>請求書発行元情報4</summary>
			[XmlElement("billServiceInfo4")]
			public string Billserviceinfo4 { get; set; }

			/// <summary>請求書ステータス</summary>
			[XmlElement("billState1")]
			public string Billstate1 { get; set; }

			/// <summary>宛名欄挨拶文欄1</summary>
			[XmlElement("billFirstGreet1")]
			public string Billfirstgreet1 { get; set; }

			/// <summary>宛名欄挨拶文欄2</summary>
			[XmlElement("billFirstGreet2")]
			public string Billfirstgreet2 { get; set; }

			/// <summary>宛名欄挨拶文欄3</summary>
			[XmlElement("billFirstGreet3")]
			public string Billfirstgreet3 { get; set; }

			/// <summary>宛名欄挨拶文欄4</summary>
			[XmlElement("billFirstGreet4")]
			public string Billfirstgreet4 { get; set; }

			/// <summary>予備項目1</summary>
			[XmlElement("expand1")]
			public string Expand1 { get; set; }

			/// <summary>予備項目2</summary>
			[XmlElement("expand2")]
			public string Expand2 { get; set; }

			/// <summary>予備項目3</summary>
			[XmlElement("expand3")]
			public string Expand3 { get; set; }

			/// <summary>予備項目4</summary>
			[XmlElement("expand4")]
			public string Expand4 { get; set; }

			/// <summary>予備項目5</summary>
			[XmlElement("expand5")]
			public string Expand5 { get; set; }

			/// <summary>予備項目6</summary>
			[XmlElement("expand6")]
			public string Expand6 { get; set; }

			/// <summary>予備項目7</summary>
			[XmlElement("expand7")]
			public string Expand7 { get; set; }

			/// <summary>予備項目8</summary>
			[XmlElement("expand8")]
			public string Expand8 { get; set; }

			/// <summary>予備項目9</summary>
			[XmlElement("expand9")]
			public string Expand9 { get; set; }

			/// <summary>予備項目10</summary>
			[XmlElement("expand10")]
			public string Expand10 { get; set; }

			/// <summary>請求金額タイトル</summary>
			[XmlElement("billedAmountTitle")]
			public string Billedamounttitle { get; set; }

			/// <summary>請求金額</summary>
			[XmlElement("billedAmount")]
			public string Billedamount { get; set; }

			/// <summary>請求金額消費税</summary>
			[XmlElement("billedFeeTax")]
			public string Billedfeetax { get; set; }

			/// <summary>注文日タイトル</summary>
			[XmlElement("billOrderdayTitle")]
			public string Billorderdaytitle { get; set; }

			/// <summary>注文日</summary>
			[XmlElement("shopOrderDate")]
			public string Shoporderdate { get; set; }

			/// <summary>請求書発行日タイトル</summary>
			[XmlElement("billSendDateTitle")]
			public string Billsenddatetitle { get; set; }

			/// <summary>請求書発行日</summary>
			[XmlElement("billSendDate")]
			public string Billsenddate { get; set; }

			/// <summary>お支払期限日タイトル</summary>
			[XmlElement("billDeadlineDateTitle")]
			public string Billdeadlinedatetitle { get; set; }

			/// <summary>お支払期限日</summary>
			[XmlElement("billDeadlineDate")]
			public string Billdeadlinedate { get; set; }

			/// <summary>お問合せ番号タイトル</summary>
			[XmlElement("transactionIdTitle")]
			public string Transactionidtitle { get; set; }

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string Transactionid { get; set; }

			/// <summary>銀行振込注意文言</summary>
			[XmlElement("billBankInfomation")]
			public string Billbankinfomation { get; set; }

			/// <summary>銀行名タイトル</summary>
			[XmlElement("bankNameTitle")]
			public string Banknametitle { get; set; }

			/// <summary>銀行名漢字</summary>
			[XmlElement("bankName")]
			public string Bankname { get; set; }

			/// <summary>銀行コード</summary>
			[XmlElement("bankCode")]
			public string Bankcode { get; set; }

			/// <summary>支店名タイトル</summary>
			[XmlElement("branchNameTitle")]
			public string Branchnametitle { get; set; }

			/// <summary>支店名漢字</summary>
			[XmlElement("branchName")]
			public string Branchname { get; set; }

			/// <summary>支店コード</summary>
			[XmlElement("branchCode")]
			public string Branchcode { get; set; }

			/// <summary>口座番号タイトル</summary>
			[XmlElement("bankAccountNumberTitle")]
			public string Bankaccountnumbertitle { get; set; }

			/// <summary>預金種別</summary>
			[XmlElement("bankAccountKind")]
			public string Bankaccountkind { get; set; }

			/// <summary>口座番号</summary>
			[XmlElement("bankAccountNumber")]
			public string Bankaccountnumber { get; set; }

			/// <summary>口座名義タイトル</summary>
			[XmlElement("bankAccountNameTitle")]
			public string Bankaccountnametitle { get; set; }

			/// <summary>銀行口座名義</summary>
			[XmlElement("bankAccountName")]
			public string Bankaccountname { get; set; }

			/// <summary>払込取扱用支払期限日</summary>
			[XmlElement("receiptBillDeadlineDate")]
			public string Receiptbilldeadlinedate { get; set; }

			/// <summary>払込取扱用購入者氏名</summary>
			[XmlElement("receiptName")]
			public string Receiptname { get; set; }

			/// <summary>バーコード情報</summary>
			[XmlElement("invoiceBarcode")]
			public string Invoicebarcode { get; set; }

			/// <summary>収納代行会社名タイトル</summary>
			[XmlElement("receiptCompanyTitle")]
			public string Receiptcompanytitle { get; set; }

			/// <summary>収納代行会社名</summary>
			[XmlElement("receiptCompany")]
			public string Receiptcompany { get; set; }

			/// <summary>請求金額</summary>
			[XmlElement("docketbilledAmount")]
			public string Docketbilledamount { get; set; }

			/// <summary>受領証用購入者会社名</summary>
			[XmlElement("docketCompanyName")]
			public string Docketcompanyname { get; set; }

			/// <summary>受領証用購入者部署名</summary>
			[XmlElement("docketSectionName")]
			public string Docketsectionname { get; set; }

			/// <summary>受領証用購入者氏名</summary>
			[XmlElement("docketName")]
			public string Docketname { get; set; }

			/// <summary>お問い合せ番号タイトル</summary>
			[XmlElement("docketTransactionIdTitle")]
			public string Dockettransactionidtitle { get; set; }

			/// <summary>お問い合せ番号</summary>
			[XmlElement("docketTransactionId")]
			public string Dockettransactionid { get; set; }

			/// <summary>払込受領書用購入者会社名</summary>
			[XmlElement("voucherCompanyName")]
			public string Vouchercompanyname { get; set; }

			/// <summary>払込受領書用購入者部署名</summary>
			[XmlElement("voucherSectionName")]
			public string Vouchersectionname { get; set; }

			/// <summary>払込受領書用購入者氏名</summary>
			[XmlElement("voucherCustomerFullName")]
			public string Vouchercustomerfullname { get; set; }

			/// <summary>払込受領書用お問い合せ番号タイトル</summary>
			[XmlElement("voucherTransactionIdTitle")]
			public string Vouchertransactionidtitle { get; set; }

			/// <summary>払込受領書用お問い合せ番号</summary>
			[XmlElement("voucherTransactionId")]
			public string Vouchertransactionid { get; set; }

			/// <summary>払込受領書用請求金額</summary>
			[XmlElement("voucherBilledAmount")]
			public string Voucherbilledamount { get; set; }

			/// <summary>払込受領書用消費税金額</summary>
			[XmlElement("voucherBilledFeeTax")]
			public string Voucherbilledfeetax { get; set; }

			/// <summary>収入印紙文言</summary>
			[XmlElement("revenueStampRequired")]
			public string Revenuestamprequired { get; set; }

			/// <summary>明細内容タイトル</summary>
			[XmlElement("goodsTitle")]
			public string Goodstitle { get; set; }

			/// <summary>注文数タイトル</summary>
			[XmlElement("goodsAmountTitle")]
			public string Goodsamounttitle { get; set; }

			/// <summary>単価タイトル</summary>
			[XmlElement("goodsPriceTitle")]
			public string Goodspricetitle { get; set; }

			/// <summary>金額タイトル</summary>
			[XmlElement("goodsSubtotalTitle")]
			public string Goodssubtotaltitle { get; set; }

			/// <summary>明細詳細項目</summary>
			[XmlElement("details")]
			public DetailsElement Details { get; set; }

			/// <summary>明細注意事項</summary>
			[XmlElement("detailInfomation")]
			public string Detailinfomation { get; set; }

			/// <summary>ゆうちょ口座番号</summary>
			[XmlElement("expand11")]
			public string Expand11 { get; set; }

			/// <summary>ゆうちょ加入者名</summary>
			[XmlElement("expand12")]
			public string Expand12 { get; set; }

			/// <summary>OCR-Bフォント印字項目上段情報</summary>
			[XmlElement("expand13")]
			public string Expand13 { get; set; }

			/// <summary>OCR-Bフォント印字項目下段情報</summary>
			[XmlElement("expand14")]
			public string Expand14 { get; set; }

			/// <summary>払込取扱用購入者住所</summary>
			[XmlElement("expand15")]
			public string Expand15 { get; set; }

			/// <summary>印字ズレチェックマーク</summary>
			[XmlElement("expand16")]
			public string Expand16 { get; set; }

			/// <summary>予備項目17</summary>
			[XmlElement("expand17")]
			public string Expand17 { get; set; }

			/// <summary>予備項目18</summary>
			[XmlElement("expand18")]
			public string Expand18 { get; set; }

			/// <summary>予備項目19</summary>
			[XmlElement("expand19")]
			public string Expand19 { get; set; }

			/// <summary>予備項目20</summary>
			[XmlElement("expand20")]
			public string Expand20 { get; set; }
		}

		/// <summary>
		/// 明細詳細項目要素
		/// </summary>
		public class DetailsElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public DetailsElement()
			{
				this.Detail = new DetailElement[] { };
			}

			/// <summary>
			/// モデルにする
			/// </summary>
			/// <returns>モデル配列</returns>
			public InvoiceAtodeneDetailModel[] ToDbModel()
			{
				var models = this.Detail
					.Select(
						(detail, i) =>
						{
							var model = detail.ToModel();
							model.DetailNo = (i + 1);
							return model;
						})
					.ToArray();

				return models;
			}

			/// <summary>明細詳細情報</summary>
			[XmlElement("detail")]
			public DetailElement[] Detail { get; set; }
		}

		/// <summary>
		/// 明細詳細情報要素
		/// </summary>
		public class DetailElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public DetailElement()
			{
				this.Goods = "";
				this.GoodsPrice = "0";
				this.GoodsAmount = "0";
				this.GoodsSubtotal = "0";
				this.GoodsExpand = "";
			}

			/// <summary>
			/// モデルにする
			/// </summary>
			/// <returns></returns>
			public InvoiceAtodeneDetailModel ToModel()
			{
				var model = new InvoiceAtodeneDetailModel
				{
					Goods = this.Goods,
					Goodsamount = this.GoodsAmount,
					Goodsprice = this.GoodsPrice,
					Goodssubtotal = this.GoodsSubtotal,
					Goodsexpand = this.GoodsExpand
				};

				return model;
			}

			/// <summary>明細名（商品名）</summary>
			[XmlElement("goods")]
			public string Goods { get; set; }

			/// <summary>単価（税込）</summary>
			[XmlElement("goodsPrice")]
			public string GoodsPrice { get; set; }

			/// <summary>数量</summary>
			[XmlElement("goodsAmount")]
			public string GoodsAmount { get; set; }

			/// <summary>数量</summary>
			[XmlElement("goodsSubtotal")]
			public string GoodsSubtotal { get; set; }

			/// <summary>明細予備項目</summary>
			[XmlElement("goodsExpand")]
			public string GoodsExpand { get; set; }
		}
	}
}
