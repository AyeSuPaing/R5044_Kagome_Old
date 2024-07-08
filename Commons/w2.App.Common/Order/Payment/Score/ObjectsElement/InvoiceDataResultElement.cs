/*
=========================================================================================================
  Module      : 印字データ情報要素(InvoiceDataResultElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 印字データ要素
	/// </summary>
	public class InvoiceDataResultElement
	{
		/// <summary>請求書バーコード</summary>
		[XmlElement("invoiceBarCode")]
		public string InvoiceBarCode { get; set; }
		/// <summary>請求書コード</summary>
		[XmlElement("invoiceCode")]
		public string InvoiceCode { get; set; }
		/// <summary>発行区分</summary>
		[XmlElement("invoiceKbn")]
		public string InvoiceKbn { get; set; }
		/// <summary>履歴番号</summary>
		[XmlElement("historySeq")]
		public string HistorySeq { get; set; }
		/// <summary>督促区分</summary>
		[XmlElement("remindedKbn")]
		public string RemindedKbn { get; set; }
		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		[XmlElement("department")]
		public string Department { get; set; }
		/// <summary>購入者氏名</summary>
		[XmlElement("customerName")]
		public string CustomerName { get; set; }
		/// <summary>購入者郵便番号</summary>
		[XmlElement("customerZip")]
		public string CustomerZip { get; set; }
		/// <summary>購入者住所都道府県</summary>
		[XmlElement("customerAddress1")]
		public string CustomerAddress1 { get; set; }
		/// <summary>購入者住所市区町村</summary>
		[XmlElement("customerAddress2")]
		public string CustomerAddress2 { get; set; }
		/// <summary>購入者住所それ以降の住所</summary>
		[XmlElement("customerAddress3")]
		public string CustomerAddress3 { get; set; }
		/// <summary>加盟店郵便番号</summary>
		[XmlElement("shopZip")]
		public string ShopZip { get; set; }
		/// <summary>加盟店住所都道府県</summary>
		[XmlElement("shopAddress1")]
		public string ShopAddress1 { get; set; }
		/// <summary>購入者住所市区町村</summary>
		[XmlElement("shopAddress2")]
		public string ShopAddress2 { get; set; }
		/// <summary>加盟店住所それ以降の住所</summary>
		[XmlElement("shopAddress3")]
		public string ShopAddress3 { get; set; }
		/// <summary>加盟店電話</summary>
		[XmlElement("shopTel")]
		public string ShopTel { get; set; }
		/// <summary>加盟店FAX番号</summary>
		[XmlElement("shopFax")]
		public string ShopFax { get; set; }
		/// <summary>顧客請求金額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount { get; set; }
		/// <summary>消費税</summary>
		[XmlElement("tax")]
		public string Tax { get; set; }
		/// <summary>購入者払込期限日</summary>
		[XmlElement("timeOfReceipts")]
		public string TimeOfReceipts { get; set; }
		/// <summary>請求書発行日付</summary>
		[XmlElement("invoiceStartDate")]
		public string InvoiceStartDate { get; set; }
		/// <summary>帳票タイトル</summary>
		[XmlElement("invoiceTitle")]
		public string InvoiceTitle { get; set; }
		/// <summary>通信欄1</summary>
		[XmlElement("nissenMessage1")]
		public string Message1 { get; set; }
		/// <summary>通信欄2</summary>
		[XmlElement("nissenMessage2")]
		public string Message2 { get; set; }
		/// <summary>通信欄3</summary>
		[XmlElement("nissenMessage3")]
		public string Message3 { get; set; }
		/// <summary>通信欄4</summary>
		[XmlElement("nissenMessage4")]
		public string Message4 { get; set; }
		/// <summary>加盟店サイト名称</summary>
		[XmlElement("invoiceShopsiteName")]
		public string InvoiceShopsiteName { get; set; }
		/// <summary>加盟店メールアドレス</summary>
		[XmlElement("shopEmail")]
		public string ShopEmail { get; set; }
		/// <summary>スコア社名 </summary>
		[XmlElement("nissenName")]
		public string NissenName { get; set; }
		/// <summary>スコア連絡先</summary>
		[XmlElement("nissenQaUrl")]
		public string NissenQaUrl { get; set; }
		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate { get; set; }
		/// <summary>加盟店ID</summary>
		[XmlElement("shopCode")]
		public string ShopCode { get; set; }
		/// <summary>スコア注文ID</summary>
		[XmlElement("nissenTransactionId")]
		public string NissenTransactionId { get; set; }
		/// <summary>加盟店注文ID1</summary>
		[XmlElement("shopTransactionId1")]
		public string ShopTransactionId1 { get; set; }
		/// <summary>加盟店注文ID2</summary>
		[XmlElement("shopTransactionId2")]
		public string ShopTransactionId2 { get; set; }
		/// <summary>加盟店通信欄1</summary>
		[XmlElement("shopMessage1")]
		public string ShopMessage1 { get; set; }
		/// <summary>加盟店通信欄2</summary>
		[XmlElement("shopMessage2")]
		public string ShopMessage2 { get; set; }
		/// <summary>加盟店通信欄3</summary>
		[XmlElement("shopMessage3")]
		public string ShopMessage3 { get; set; }
		/// <summary>加盟店通信欄4</summary>
		[XmlElement("shopMessage4")]
		public string ShopMessage4 { get; set; }
		/// <summary>加盟店通信欄5</summary>
		[XmlElement("shopMessage5")]
		public string ShopMessage5 { get; set; }
		/// <summary>請求書形式</summary>
		[XmlElement("yobi1")]
		public string InvoiceForm { get; set; }
		/// <summary>郵便口座番号</summary>
		[XmlElement("yobi2")]
		public string PostalAccountNumber { get; set; }
		/// <summary>郵便口座名義 </summary>
		[XmlElement("yobi3")]
		public string PostalAccountHolderName { get; set; }
		/// <summary>郵便OCR-Bフォント上段</summary>
		[XmlElement("yobi4")]
		public string PostalFontTopRow { get; set; }
		/// <summary>郵便OCR-Bフォント下段</summary>
		[XmlElement("yobi5")]
		public string PostalFontBottomRow { get; set; }
		/// <summary>払込取扱用購入者住所</summary>
		[XmlElement("yobi6")]
		public string PremittanceAddress { get; set; }
		/// <summary>X印</summary>
		[XmlElement("yobi7")]
		public string XSymbol { get; set; }
		/// <summary>予備項目1</summary>
		[XmlElement("yobi8")]
		public string Reserve1 { get; set; }
		/// <summary>予備項目2</summary>
		[XmlElement("yobi9")]
		public string Reserve2 { get; set; }
		/// <summary>予備項目3</summary>
		[XmlElement("yobi10")]
		public string Reserve3 { get; set; }
	}
}
