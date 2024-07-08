/*
=========================================================================================================
  Module      : 請求書印字データ取得のレスポンス値(GmoResponseGetinvoicePrintData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetinvoicePrintData
{
	/// <summary>
	/// 与信審査結果取得のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseGetinvoicePrintData : BaseGmoResponse
	{
		/// <summary>
		/// 印字データ
		/// </summary>
		[XmlElement("invoiceDataResult")]
		public InvoiceDataResultElement InvoiceDataResult;
	}

	#region InvoiceDataResultElement 印字データ要素
	/// <summary>
	/// 印字データ要素
	/// </summary>
	public class InvoiceDataResultElement
	{
		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>発送先郵便番号</summary>
		[XmlElement("deliveryZip")]
		public string DeliveryZip;

		/// <summary>発送先住所1</summary>
		[XmlElement("deliveryAddress1")]
		public string DeliveryAddress1;

		/// <summary>発送先住所2</summary>
		[XmlElement("deliveryAddress2")]
		public string DeliveryAddress2;

		/// <summary>購入者会社名</summary>
		[XmlElement("purchaseCompanyName")]
		public string PurchaseCompanyName;

		/// <summary>購入者部署名</summary>
		[XmlElement("purchaseDepartmentName")]
		public string PurchaseDepartmentName;

		/// <summary>購入者氏名</summary>
		[XmlElement("purchaseUserName")]
		public string PurchaseUserName;

		/// <summary>請求書記載店舗名</summary>
		[XmlElement("shopName")]
		public string ShopName;

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>請求書記載事項1</summary>
		[XmlElement("invoiceMatter1")]
		public string InvoiceMatter1;

		/// <summary>請求書記載事項2</summary>
		[XmlElement("invoiceMatter2")]
		public string InvoiceMatter2;

		/// <summary>請求書記載事項3</summary>
		[XmlElement("invoiceMatter3")]
		public string InvoiceMatter3;

		/// <summary>請求書記載事項4</summary>
		[XmlElement("invoiceMatter4")]
		public string InvoiceMatter4;

		/// <summary>請求書記載事項5</summary>
		[XmlElement("invoiceMatter5")]
		public string InvoiceMatter5;

		/// <summary>GMO-PS 社名</summary>
		[XmlElement("gmoCompanyName")]
		public string GmoCompanyName;

		/// <summary>GMO-PS 情報1</summary>
		[XmlElement("gmoInfo1")]
		public string GmoInfo1;

		/// <summary>GMO-PS 情報2</summary>
		[XmlElement("gmoInfo2")]
		public string GmoInfo2;

		/// <summary>GMO-PS 情報3</summary>
		[XmlElement("gmoInfo3")]
		public string GmoInfo3;

		/// <summary>GMO-PS 情報4</summary>
		[XmlElement("gmoInfo4")]
		public string GmoInfo4;

		/// <summary>請求書タイトル</summary>
		[XmlElement("invoiceTitle")]
		public string InvoiceTitle;

		/// <summary>宛名欄挨拶文欄1</summary>
		[XmlElement("invoiceGreeting1")]
		public string InvoiceGreeting1;

		/// <summary>宛名欄挨拶文欄2</summary>
		[XmlElement("invoiceGreeting2")]
		public string InvoiceGreeting2;

		/// <summary>宛名欄挨拶文欄3</summary>
		[XmlElement("invoiceGreeting3")]
		public string InvoiceGreeting3;

		/// <summary>宛名欄挨拶文欄4</summary>
		[XmlElement("invoiceGreeting4")]
		public string InvoiceGreeting4;

		/// <summary>予備項目1</summary>
		[XmlElement("yobi1")]
		public string Yobi1;

		/// <summary>予備項目2</summary>
		[XmlElement("yobi2")]
		public string Yobi2;

		/// <summary>予備項目3</summary>
		[XmlElement("yobi3")]
		public string Yobi3;

		/// <summary>予備項目4</summary>
		[XmlElement("yobi4")]
		public string Yobi4;

		/// <summary>予備項目5</summary>
		[XmlElement("yobi5")]
		public string Yobi5;

		/// <summary>予備項目6</summary>
		[XmlElement("yobi6")]
		public string Yobi6;

		/// <summary>予備項目7</summary>
		[XmlElement("yobi7")]
		public string Yobi7;

		/// <summary>予備項目8</summary>
		[XmlElement("yobi8")]
		public string Yobi8;

		/// <summary>予備項目9</summary>
		[XmlElement("yobi9")]
		public string Yobi9;

		/// <summary>予備項目10</summary>
		[XmlElement("yobi10")]
		public string Yobi10;

		/// <summary>請求金額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount;

		/// <summary>請求金額消費税</summary>
		[XmlElement("billedAmountTax")]
		public string BilledAmountTax;

		/// <summary>注文日</summary>
		[XmlElement("orderDate")]
		public string OrderDate;

		/// <summary>請求書発行日</summary>
		[XmlElement("invoiceIssueDate")]
		public string InvoiceIssueDate;

		/// <summary>お支払期限日</summary>
		[XmlElement("paymentDueDate")]
		public string PaymentDueDate;

		/// <summary>お問い合せ番号</summary>
		[XmlElement("trackingNumber")]
		public string TrackingNumber;

		/// <summary>銀行振込注意文言</summary>
		[XmlElement("bankNoteWording")]
		public string BankNoteWording;

		/// <summary>銀行名漢字</summary>
		[XmlElement("bankName")]
		public string BankName;

		/// <summary>銀行コード</summary>
		[XmlElement("bankCode")]
		public string BankCode;

		/// <summary>支店名漢字</summary>
		[XmlElement("branchName")]
		public string BranchName;

		/// <summary>支店コード</summary>
		[XmlElement("branchCode")]
		public string BranchCode;

		/// <summary>預金種別</summary>
		[XmlElement("depositType")]
		public string DepositType;

		/// <summary>口座番号</summary>
		[XmlElement("accountNumber")]
		public string AccountNumber;

		/// <summary>銀行口座名義</summary>
		[XmlElement("bankAccountHolder")]
		public string BankAccountHolder;

		/// <summary>請求金額</summary>
		[XmlElement("votesBilledAmount")]
		public string VotesBilledAmount;

		/// <summary>郵便OCR-B フォント印字項目上段情報</summary>
		[XmlElement("votesFontUpperInfo")]
		public string VotesFontUpperInfo;

		/// <summary>郵便OCR-B フォント印字項目下段情報</summary>
		[XmlElement("votesFontKiwerInfo")]
		public string VotesFontKiwerInfo;

		/// <summary>払込取扱用支払期限日</summary>
		[XmlElement("votesPaymentDueDate")]
		public string VotesPaymentDueDate;

		/// <summary>払込取扱用購入者氏名</summary>
		[XmlElement("votesPurchaseUserName")]
		public string VotesPurchaseUserName;

		/// <summary>お問合せ番号</summary>
		[XmlElement("votesTrackingNumber")]
		public string VotesTrackingNumber;

		/// <summary>バーコード情報</summary>
		[XmlElement("votesBarCode")]
		public string VotesBarCode;

		/// <summary>請求金額</summary>
		[XmlElement("docketBilledAmount")]
		public string DocketBilledAmount;

		/// <summary>受領証用購入者住所</summary>
		[XmlElement("docketPurchaseAddress")]
		public string DocketPurchaseAddress;

		/// <summary>受領証用購入者住所</summary>
		[XmlElement("docketPurchaseCompanyName")]
		public string DocketPurchaseCompanyName;

		/// <summary>受領証用購入者部署名</summary>
		[XmlElement("docketPurchaseDepartmentName")]
		public string DocketPurchaseDepartmentName;

		/// <summary>受領証用購入者氏名</summary>
		[XmlElement("docketPurchaseUserName")]
		public string DocketPurchaseUserName;

		/// <summary>お問合せ番号</summary>
		[XmlElement("docketTrackingNumber")]
		public string DocketTrackingNumber;

		/// <summary>X</summary>
		[XmlElement("docketX")]
		public string DocketX;

		/// <summary>払込受領書用購入者会社名</summary>
		[XmlElement("receiptPurchaseCompanyName")]
		public string ReceiptPurchaseCompanyName;

		/// <summary>払込受領書用購入者部署名</summary>
		[XmlElement("receiptPurchaseDepartmentName")]
		public string ReceiptPurchaseDepartmentName;

		/// <summary>払込受領書用購入者氏名</summary>
		[XmlElement("receiptPurchaseUserName")]
		public string ReceiptPurchaseUserName;

		/// <summary>払込受領書用お問い合せ番号1</summary>
		[XmlElement("receiptTrackingNumber1")]
		public string ReceiptTrackingNumber1;

		/// <summary>払込受領書用お問い合せ番号2</summary>
		[XmlElement("receiptTrackingNumber2")]
		public string ReceiptTrackingNumber2;

		/// <summary>払込受領書用金額</summary>
		[XmlElement("receiptAmount")]
		public string ReceiptAmount;

		/// <summary>払込受領書用消費税金額</summary>
		[XmlElement("receiptTax")]
		public string ReceiptTax;

		/// <summary>払込受領書用加盟店名</summary>
		[XmlElement("receiptShopName")]
		public string ReceiptShopName;

		/// <summary>収入印紙文言</summary>
		[XmlElement("receiptPrintWord")]
		public string ReceiptPrintWord;

		/// <summary>文字列</summary>
		[XmlElement("string")]
		public string String;

		/// <summary>予備項目11</summary>
		[XmlElement("yobi11")]
		public string Yobi11;

		/// <summary>予備項目12</summary>
		[XmlElement("yobi12")]
		public string Yobi12;

		/// <summary>予備項目13</summary>
		[XmlElement("yobi13")]
		public string Yobi13;

		/// <summary>予備項目14</summary>
		[XmlElement("yobi14")]
		public string Yobi14;

		/// <summary>予備項目15</summary>
		[XmlElement("yobi15")]
		public string Yobi15;

		/// <summary>明細リスト</summary>
		[XmlElement("detailList")]
		public DetailListElement DetailList;

	}
	#endregion

	#region DetailListElement 明細リスト要素
	/// <summary>
	/// 明細リスト要素
	/// </summary>
	public class DetailListElement
	{
		/// <summary>明細</summary>
		[XmlElement("goodsDetail")]
		public GoodsDetailElement[] GoodsDetail;
	}
	#endregion

	#region GoodsDetailElement 明細要素
	/// <summary>
	/// 明細要素
	/// </summary>
	public class GoodsDetailElement
	{
		/// <summary>商品名称</summary>
		[XmlElement("goodsName")]
		public string GoodsName;

		/// <summary>商品数量</summary>
		[XmlElement("goodsNum")]
		public string GoodsNum;

		/// <summary>商品単価</summary>
		[XmlElement("goodsPrice")]
		public string GoodsPrice;

		/// <summary>商品金額</summary>
		[XmlElement("goodsAmount")]
		public string GoodsAmount;

		/// <summary>商品金額消費税</summary>
		[XmlElement("goodsAmountTax")]
		public string GoodsAmountTax;

		/// <summary>予備項目16</summary>
		[XmlElement("yobi16")]
		public string Yobi16;

		/// <summary>予備項目17</summary>
		[XmlElement("yobi17")]
		public string Yobi17;

		/// <summary>予備項目18</summary>
		[XmlElement("yobi18")]
		public string Yobi18;

		/// <summary>予備項目19</summary>
		[XmlElement("yobi19")]
		public string Yobi19;

		/// <summary>予備項目20</summary>
		[XmlElement("yobi20")]
		public string Yobi20;

	}
	#endregion
}
