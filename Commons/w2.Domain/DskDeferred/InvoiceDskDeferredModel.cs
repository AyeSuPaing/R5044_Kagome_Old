/*
=========================================================================================================
  Module      : DSK後払い請求書モデル (InvoiceDskDeferredModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.InvoiceDskDeferred
{
	/// <summary>
	/// DSK後払い請求書モデル
	/// </summary>
	[Serializable]
	public partial class InvoiceDskDeferredModel : ModelBase<InvoiceDskDeferredModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceDskDeferredModel()
		{
			this.OrderId = "";
			this.InvoiceBarCode = "";
			this.InvoiceCode = "";
			this.InvoiceKbn = "";
			this.HistorySeq = "";
			this.RemindedKbn = "";
			this.CompanyName = "";
			this.Department = "";
			this.CustomerName = "";
			this.CustomerZip = "";
			this.CustomerAddress1 = "";
			this.CustomerAddress2 = "";
			this.CustomerAddress3 = "";
			this.ShopZip = "";
			this.ShopAddress1 = "";
			this.ShopAddress2 = "";
			this.ShopAddress3 = "";
			this.ShopTel = "";
			this.ShopFax = "";
			this.BilledAmount = "";
			this.Tax = "";
			this.TimeOfReceipts = "";
			this.InvoiceStartDate = "";
			this.InvoiceTitle = "";
			this.Message1 = "";
			this.Message2 = "";
			this.Message3 = "";
			this.Message4 = "";
			this.InvoiceShopsiteName = "";
			this.ShopEmail = "";
			this.Name = "";
			this.QaUrl = "";
			this.ShopOrderDate = "";
			this.ShopCode = "";
			this.TransactionId = "";
			this.ShopTransactionId1 = "";
			this.ShopTransactionId2 = "";
			this.ShopMessage1 = "";
			this.ShopMessage2 = "";
			this.ShopMessage3 = "";
			this.ShopMessage4 = "";
			this.ShopMessage5 = "";
			this.Yobi1 = "";
			this.Yobi2 = "";
			this.Yobi3 = "";
			this.Yobi4 = "";
			this.Yobi5 = "";
			this.Yobi6 = "";
			this.Yobi7 = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceDskDeferredModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceDskDeferredModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_ORDER_ID] = value; }
		}
		/// <summary>請求書バーコード</summary>
		public string InvoiceBarCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_BAR_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_BAR_CODE] = value; }
		}
		/// <summary>請求書コード</summary>
		public string InvoiceCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_CODE] = value; }
		}
		/// <summary>発行区分</summary>
		public string InvoiceKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_KBN]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_KBN] = value; }
		}
		/// <summary>履歴番号</summary>
		public string HistorySeq
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_HISTORY_SEQ]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_HISTORY_SEQ] = value; }
		}
		/// <summary>督促区分</summary>
		public string RemindedKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_REMINDED_KBN]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_REMINDED_KBN] = value; }
		}
		/// <summary>会社名</summary>
		public string CompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		public string Department
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_DEPARTMENT]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_DEPARTMENT] = value; }
		}
		/// <summary>購入者氏名</summary>
		public string CustomerName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_NAME] = value; }
		}
		/// <summary>購入者郵便番号</summary>
		public string CustomerZip
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ZIP]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ZIP] = value; }
		}
		/// <summary>購入者住所都道府県</summary>
		public string CustomerAddress1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS1]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS1] = value; }
		}
		/// <summary>購入者住所市区町村</summary>
		public string CustomerAddress2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS2]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS2] = value; }
		}
		/// <summary>購入者住所それ以降の住所</summary>
		public string CustomerAddress3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS3]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS3] = value; }
		}
		/// <summary>加盟店郵便番号</summary>
		public string ShopZip
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ZIP]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ZIP] = value; }
		}
		/// <summary>加盟店住所都道府県</summary>
		public string ShopAddress1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS1]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS1] = value; }
		}
		/// <summary>購入者住所市区町村</summary>
		public string ShopAddress2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS2]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS2] = value; }
		}
		/// <summary>加盟店住所それ以降の住所</summary>
		public string ShopAddress3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS3]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS3] = value; }
		}
		/// <summary>加盟店電話</summary>
		public string ShopTel
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_TEL]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_TEL] = value; }
		}
		/// <summary>加盟店FAX番号</summary>
		public string ShopFax
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_FAX]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_FAX] = value; }
		}
		/// <summary>顧客請求金額</summary>
		public string BilledAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_BILLED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_BILLED_AMOUNT] = value; }
		}
		/// <summary>消費税</summary>
		public string Tax
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_TAX]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_TAX] = value; }
		}
		/// <summary>購入者払込期限日</summary>
		public string TimeOfReceipts
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_TIME_OF_RECEIPTS]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_TIME_OF_RECEIPTS] = value; }
		}
		/// <summary>請求書発行日付</summary>
		public string InvoiceStartDate
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_START_DATE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_START_DATE] = value; }
		}
		/// <summary>帳票タイトル</summary>
		public string InvoiceTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_TITLE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_TITLE] = value; }
		}
		/// <summary>通信欄1</summary>
		public string Message1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE1] = value; }
		}
		/// <summary>通信欄2</summary>
		public string Message2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE2] = value; }
		}
		/// <summary>通信欄3</summary>
		public string Message3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE3] = value; }
		}
		/// <summary>通信欄4</summary>
		public string Message4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_MESSAGE4] = value; }
		}
		/// <summary>加盟店サイト名称</summary>
		public string InvoiceShopsiteName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_SHOPSITE_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_INVOICE_SHOPSITE_NAME] = value; }
		}
		/// <summary>加盟店メールアドレス</summary>
		public string ShopEmail
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_EMAIL]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_EMAIL] = value; }
		}
		/// <summary>固定文言1</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_NAME] = value; }
		}
		/// <summary>固定文言2</summary>
		public string QaUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_QA_URL]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_QA_URL] = value; }
		}
		/// <summary>加盟店注文日</summary>
		public string ShopOrderDate
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ORDER_DATE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_ORDER_DATE] = value; }
		}
		/// <summary>加盟店ID</summary>
		public string ShopCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_CODE] = value; }
		}
		/// <summary>注文ID</summary>
		public string TransactionId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_TRANSACTION_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_TRANSACTION_ID] = value; }
		}
		/// <summary>加盟店注文ID1</summary>
		public string ShopTransactionId1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_TRANSACTION_ID1]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_TRANSACTION_ID1] = value; }
		}
		/// <summary>加盟店注文ID2</summary>
		public string ShopTransactionId2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_TRANSACTION_ID2]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_TRANSACTION_ID2] = value; }
		}
		/// <summary>加盟店通信欄1</summary>
		public string ShopMessage1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE1] = value; }
		}
		/// <summary>加盟店通信欄2</summary>
		public string ShopMessage2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE2] = value; }
		}
		/// <summary>加盟店通信欄3</summary>
		public string ShopMessage3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE3] = value; }
		}
		/// <summary>加盟店通信欄4</summary>
		public string ShopMessage4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE4] = value; }
		}
		/// <summary>加盟店通信欄5</summary>
		public string ShopMessage5
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE5]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE5] = value; }
		}
		/// <summary>請求書形式</summary>
		public string Yobi1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI1]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI1] = value; }
		}
		/// <summary>郵便口座番号</summary>
		public string Yobi2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI2]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI2] = value; }
		}
		/// <summary>郵便口座名義</summary>
		public string Yobi3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI3]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI3] = value; }
		}
		/// <summary>郵便OCR-Bフォント上段</summary>
		public string Yobi4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI4]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI4] = value; }
		}
		/// <summary>郵便OCR-Bフォント下段</summary>
		public string Yobi5
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI5]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI5] = value; }
		}
		/// <summary>払込取扱用購入者住所</summary>
		public string Yobi6
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI6]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI6] = value; }
		}
		/// <summary>X印</summary>
		public string Yobi7
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI7]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_YOBI7] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERRED_DATE_CREATED] = value; }
		}
		#endregion
	}
}
