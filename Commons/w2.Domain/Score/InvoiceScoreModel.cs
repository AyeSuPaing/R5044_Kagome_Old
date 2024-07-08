/*
=========================================================================================================
  Module      : スコア後払い請求書モデル (InvoiceScoreModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Score
{
	/// <summary>
	/// スコア後払い請求書モデル
	/// </summary>
	public partial class InvoiceScoreModel : ModelBase<InvoiceScoreModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceScoreModel()
		{
			this.OrderId = string.Empty;
			this.InvoiceBarCode = string.Empty;
			this.InvoiceCode = string.Empty;
			this.InvoiceKbn = string.Empty;
			this.HistorySeq = string.Empty;
			this.RemindedKbn = string.Empty;
			this.CompanyName = string.Empty;
			this.Department = string.Empty;
			this.CustomerName = string.Empty;
			this.CustomerZip = string.Empty;
			this.CustomerAddress1 = string.Empty;
			this.CustomerAddress2 = string.Empty;
			this.CustomerAddress3 = string.Empty;
			this.ShopZip = string.Empty;
			this.ShopAddress1 = string.Empty;
			this.ShopAddress2 = string.Empty;
			this.ShopAddress3 = string.Empty;
			this.ShopTel = string.Empty;
			this.ShopFax = string.Empty;
			this.BilledAmount = string.Empty;
			this.Tax = string.Empty;
			this.TimeOfReceipts = string.Empty;
			this.InvoiceStartDate = string.Empty;
			this.InvoiceTitle = string.Empty;
			this.Message1 = string.Empty;
			this.Message2 = string.Empty;
			this.Message3 = string.Empty;
			this.Message4 = string.Empty;
			this.InvoiceShopsiteName = string.Empty;
			this.ShopEmail = string.Empty;
			this.Name = string.Empty;
			this.QaUrl = string.Empty;
			this.ShopOrderDate = string.Empty;
			this.ShopCode = string.Empty;
			this.TransactionId = string.Empty;
			this.ShopTransactionId1 = string.Empty;
			this.ShopTransactionId2 = string.Empty;
			this.ShopMessage1 = string.Empty;
			this.ShopMessage2 = string.Empty;
			this.ShopMessage3 = string.Empty;
			this.ShopMessage4 = string.Empty;
			this.ShopMessage5 = string.Empty;
			this.InvoiceForm = string.Empty;
			this.PostalAccountNumber = string.Empty;
			this.PostalAccountHolderName = string.Empty;
			this.PostalFontTopRow = string.Empty;
			this.PostalFontBottomRow = string.Empty;
			this.PremittanceAddress = string.Empty;
			this.XSymbol = string.Empty;
			this.Reserve1 = string.Empty;
			this.Reserve2 = string.Empty;
			this.Reserve3 = string.Empty;
			this.DateCreated = DateTime.Now;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceScoreModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceScoreModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_ORDER_ID] = value; }
		}
		/// <summary>請求書バーコード</summary>
		public string InvoiceBarCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_BAR_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_BAR_CODE] = value; }
		}
		/// <summary>請求書コード</summary>
		public string InvoiceCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_CODE] = value; }
		}
		/// <summary>発行区分</summary>
		public string InvoiceKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_KBN]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_KBN] = value; }
		}
		/// <summary>履歴番号</summary>
		public string HistorySeq
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_HISTORY_SEQ]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_HISTORY_SEQ] = value; }
		}
		/// <summary>督促区分</summary>
		public string RemindedKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_REMINDED_KBN]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_REMINDED_KBN] = value; }
		}
		/// <summary>会社名</summary>
		public string CompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		public string Department
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_DEPARTMENT]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DEPARTMENT] = value; }
		}
		/// <summary>購入者氏名</summary>
		public string CustomerName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_NAME] = value; }
		}
		/// <summary>購入者郵便番号</summary>
		public string CustomerZip
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ZIP]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ZIP] = value; }
		}
		/// <summary>購入者住所都道府県</summary>
		public string CustomerAddress1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS1]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS1] = value; }
		}
		/// <summary>購入者住所市区町村</summary>
		public string CustomerAddress2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS2]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS2] = value; }
		}
		/// <summary>購入者住所それ以降の住所</summary>
		public string CustomerAddress3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS3]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS3] = value; }
		}
		/// <summary>加盟店郵便番号</summary>
		public string ShopZip
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ZIP]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ZIP] = value; }
		}
		/// <summary>加盟店住所都道府県</summary>
		public string ShopAddress1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ADDRESS1]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ADDRESS1] = value; }
		}
		/// <summary>購入者住所市区町村</summary>
		public string ShopAddress2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ADDRESS2]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ADDRESS2] = value; }
		}
		/// <summary>加盟店住所それ以降の住所</summary>
		public string ShopAddress3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ADDRESS3]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ADDRESS3] = value; }
		}
		/// <summary>加盟店電話</summary>
		public string ShopTel
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_TEL]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_TEL] = value; }
		}
		/// <summary>加盟店FAX番号</summary>
		public string ShopFax
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_FAX]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_FAX] = value; }
		}
		/// <summary>顧客請求金額</summary>
		public string BilledAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_BILLED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_BILLED_AMOUNT] = value; }
		}
		/// <summary>消費税</summary>
		public string Tax
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_TAX]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_TAX] = value; }
		}
		/// <summary>購入者払込期限日</summary>
		public string TimeOfReceipts
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_TIME_OF_RECEIPTS]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_TIME_OF_RECEIPTS] = value; }
		}
		/// <summary>請求書発行日付</summary>
		public string InvoiceStartDate
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_START_DATE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_START_DATE] = value; }
		}
		/// <summary>帳票タイトル</summary>
		public string InvoiceTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_TITLE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_TITLE] = value; }
		}
		/// <summary>通信欄1</summary>
		public string Message1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE1] = value; }
		}
		/// <summary>通信欄2</summary>
		public string Message2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE2] = value; }
		}
		/// <summary>通信欄3</summary>
		public string Message3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE3] = value; }
		}
		/// <summary>通信欄4</summary>
		public string Message4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_MESSAGE4] = value; }
		}
		/// <summary>加盟店サイト名称</summary>
		public string InvoiceShopsiteName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_SHOPSITE_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_SHOPSITE_NAME] = value; }
		}
		/// <summary>加盟店メールアドレス</summary>
		public string ShopEmail
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_EMAIL]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_EMAIL] = value; }
		}
		/// <summary>固定文言1</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_NAME] = value; }
		}
		/// <summary>固定文言2</summary>
		public string QaUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_QA_URL]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_QA_URL] = value; }
		}
		/// <summary>加盟店注文日</summary>
		public string ShopOrderDate
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ORDER_DATE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_ORDER_DATE] = value; }
		}
		/// <summary>加盟店ID</summary>
		public string ShopCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_CODE] = value; }
		}
		/// <summary>注文ID</summary>
		public string TransactionId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_TRANSACTION_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_TRANSACTION_ID] = value; }
		}
		/// <summary>加盟店注文ID1</summary>
		public string ShopTransactionId1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_TRANSACTION_ID1]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_TRANSACTION_ID1] = value; }
		}
		/// <summary>加盟店注文ID2</summary>
		public string ShopTransactionId2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_TRANSACTION_ID2]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_TRANSACTION_ID2] = value; }
		}
		/// <summary>加盟店通信欄1</summary>
		public string ShopMessage1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE1] = value; }
		}
		/// <summary>加盟店通信欄2</summary>
		public string ShopMessage2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE2] = value; }
		}
		/// <summary>加盟店通信欄3</summary>
		public string ShopMessage3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE3] = value; }
		}
		/// <summary>加盟店通信欄4</summary>
		public string ShopMessage4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE4] = value; }
		}
		/// <summary>加盟店通信欄5</summary>
		public string ShopMessage5
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE5]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_SHOP_MESSAGE5] = value; }
		}
		/// <summary>請求書形式</summary>
		public string InvoiceForm
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_FORM]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_INVOICE_FORM] = value; }
		}
		/// <summary>郵便口座番号</summary>
		public string PostalAccountNumber
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_ACCOUNT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_ACCOUNT_NUMBER] = value; }
		}
		/// <summary>郵便口座名義</summary>
		public string PostalAccountHolderName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_ACCOUNT_HOLDER_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_ACCOUNT_HOLDER_NAME] = value; }
		}
		/// <summary>郵便OCR-Bフォント上段</summary>
		public string PostalFontTopRow
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_FONT_TOP_ROW]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_FONT_TOP_ROW] = value; }
		}
		/// <summary>郵便OCR-Bフォント下段</summary>
		public string PostalFontBottomRow
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_FONT_BOTTOM_ROW]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_POSTAL_FONT_BOTTOM_ROW] = value; }
		}
		/// <summary>払込取扱用購入者住所</summary>
		public string PremittanceAddress
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_REMITTANCE_ADDRESS]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_REMITTANCE_ADDRESS] = value; }
		}
		/// <summary>X印</summary>
		public string XSymbol
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_X_SYMBOL]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_X_SYMBOL] = value; }
		}
		/// <summary>予備項目1</summary>
		public string Reserve1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_RESERVE1]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_RESERVE1] = value; }
		}
		/// <summary>予備項目2</summary>
		public string Reserve2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_RESERVE2]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_RESERVE2] = value; }
		}
		/// <summary>予備項目3</summary>
		public string Reserve3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_RESERVE3]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_RESERVE3] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_INVOICE_SCORE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DATE_CREATED] = value; }
		}
		#endregion
	}
}
