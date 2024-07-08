/*
=========================================================================================================
  Module      : ベリトランス請求書モデル (InvoiceVeritransModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.InvoiceVeritrans
{
	/// <summary>
	/// ベリトランス請求書モデル
	/// </summary>
	public partial class InvoiceVeritransModel : ModelBase<InvoiceVeritransModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceVeritransModel()
		{
			this.ServiceType = string.Empty;
			this.MStatus = string.Empty;
			this.VResulCcode = string.Empty;
			this.MErrMsg = string.Empty;
			this.MArchTxn = string.Empty;
			this.PaymentOrderId = string.Empty;
			this.CustTxn = string.Empty;
			this.TxnVersion = string.Empty;
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
			this.TimeOfReceipts = null;
			this.InvoiceStartDate = null;
			this.InvoiceTitle = string.Empty;
			this.NissenMessage1 = string.Empty;
			this.NissenMessage2 = string.Empty;
			this.NissenMessage3 = string.Empty;
			this.NissenMessage4 = string.Empty;
			this.InvoiceShopsiteName = string.Empty;
			this.ShopEmail = string.Empty;
			this.NissenName = string.Empty;
			this.NissenQaUrl = string.Empty;
			this.ShopOrderDate = null;
			this.ShopCode = string.Empty;
			this.NissenTransactionId = string.Empty;
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
			this.RemittanceAddress = string.Empty;
			this.XSymbol = string.Empty;
			this.Reserve1 = string.Empty;
			this.Reserve2 = string.Empty;
			this.Reserve3 = string.Empty;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceVeritransModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceVeritransModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_ORDER_ID] = value; }
		}
		/// <summary>決済サービスタイプ</summary>
		public string ServiceType
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SERVICE_TYPE]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SERVICE_TYPE] = value; }
		}
		/// <summary>処理結果コード</summary>
		public string MStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_M_STATUS]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_M_STATUS] = value; }
		}
		/// <summary>詳細結果コード</summary>
		public string VResulCcode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_V_RESULT_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_V_RESULT_CODE] = value; }
		}
		/// <summary>エラーメッセージ</summary>
		public string MErrMsg
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_M_ERR_MSG]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_M_ERR_MSG] = value; }
		}
		/// <summary>電文 ID</summary>
		public string MArchTxn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_M_ARCH_TXN]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_M_ARCH_TXN] = value; }
		}
		/// <summary>取引 ID</summary>
		public string PaymentOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_PAYMENT_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_PAYMENT_ORDER_ID] = value; }
		}
		/// <summary>取引毎に付く ID</summary>
		public string CustTxn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUST_TXN]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUST_TXN] = value; }
		}
		/// <summary>MDK バージョン</summary>
		public string TxnVersion
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_TXN_VERSION]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_TXN_VERSION] = value; }
		}
		/// <summary>請求書バーコード</summary>
		public string InvoiceBarCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_BAR_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_BAR_CODE] = value; }
		}
		/// <summary>請求書コード</summary>
		public string InvoiceCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_CODE] = value; }
		}
		/// <summary>発行区分</summary>
		public string InvoiceKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_KBN]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_KBN] = value; }
		}
		/// <summary>履歴番号</summary>
		public string HistorySeq
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_HISTORY_SEQ]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_HISTORY_SEQ] = value; }
		}
		/// <summary>督促区分</summary>
		public string RemindedKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_REMINDED_KBN]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_REMINDED_KBN] = value; }
		}
		/// <summary>会社名</summary>
		public string CompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		public string Department
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_DEPARTMENT]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_DEPARTMENT] = value; }
		}
		/// <summary>購入者氏名</summary>
		public string CustomerName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_NAME] = value; }
		}
		/// <summary>購入者郵便番号</summary>
		public string CustomerZip
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ZIP]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ZIP] = value; }
		}
		/// <summary>購入者住所：都道府県</summary>
		public string CustomerAddress1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS1]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS1] = value; }
		}
		/// <summary>購入者住所：市区町村</summary>
		public string CustomerAddress2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS2]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS2] = value; }
		}
		/// <summary>購入者住所：それ以降の住所</summary>
		public string CustomerAddress3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS3]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS3] = value; }
		}
		/// <summary>加盟店郵便番号</summary>
		public string ShopZip
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ZIP]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ZIP] = value; }
		}
		/// <summary>加盟店住所：都道府県</summary>
		public string ShopAddress1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ADDRESS1]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ADDRESS1] = value; }
		}
		/// <summary>加盟店住所：市区町村</summary>
		public string ShopAddress2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ADDRESS2]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ADDRESS2] = value; }
		}
		/// <summary>加盟店住所：それ以降の住所</summary>
		public string ShopAddress3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ADDRESS3]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ADDRESS3] = value; }
		}
		/// <summary>加盟店電話</summary>
		public string ShopTel
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_TEL]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_TEL] = value; }
		}
		/// <summary>加盟店FAX番号</summary>
		public string ShopFax
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_FAX]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_FAX] = value; }
		}
		/// <summary>顧客請求金額</summary>
		public string BilledAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_BILLED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_BILLED_AMOUNT] = value; }
		}
		/// <summary>消費税</summary>
		public string Tax
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_TAX]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_TAX] = value; }
		}
		/// <summary>購入者払込期限日</summary>
		public DateTime? TimeOfReceipts
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEVERITRANS_TIME_OF_RECEIPTS] != DBNull.Value)
				? (DateTime?)this.DataSource[Constants.FIELD_INVOICEVERITRANS_TIME_OF_RECEIPTS]
				: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_TIME_OF_RECEIPTS] = value; }
		}
		/// <summary>請求書発行日付</summary>
		public DateTime? InvoiceStartDate
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_START_DATE] != DBNull.Value)
				? (DateTime?)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_START_DATE]
				: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_START_DATE] = value; }
		}
		/// <summary>帳票タイトル</summary>
		public string InvoiceTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_TITLE]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_TITLE] = value; }
		}
		/// <summary>スコア通信欄1</summary>
		public string NissenMessage1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE1] = value; }
		}
		/// <summary>スコア通信欄2</summary>
		public string NissenMessage2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE2] = value; }
		}
		/// <summary>スコア通信欄3</summary>
		public string NissenMessage3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE3] = value; }
		}
		/// <summary>スコア通信欄4</summary>
		public string NissenMessage4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_MESSAGE4] = value; }
		}
		/// <summary>加盟店サイト名称</summary>
		public string InvoiceShopsiteName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_SHOPSITE_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_SHOPSITE_NAME] = value; }
		}
		/// <summary>加盟店メールアドレス</summary>
		public string ShopEmail
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_EMAIL]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_EMAIL] = value; }
		}
		/// <summary>スコア社名</summary>
		public string NissenName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_NAME] = value; }
		}
		/// <summary>スコア連絡先URL</summary>
		public string NissenQaUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_QA_URL]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_QA_URL] = value; }
		}
		/// <summary>加盟店注文日</summary>
		public DateTime? ShopOrderDate
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CHANGED] != DBNull.Value)
				? (DateTime?)this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CHANGED]
				: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_ORDER_DATE] = value; }
		}
		/// <summary>加盟店コード</summary>
		public string ShopCode
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_CODE]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_CODE] = value; }
		}
		/// <summary>スコア注文ID</summary>
		public string NissenTransactionId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_TRANSACTION_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_NISSEN_TRANSACTION_ID] = value; }
		}
		/// <summary>加盟店注文ID1</summary>
		public string ShopTransactionId1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_TRANSACTION_ID1]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_TRANSACTION_ID1] = value; }
		}
		/// <summary>加盟店注文ID2</summary>
		public string ShopTransactionId2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_TRANSACTION_ID2]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_TRANSACTION_ID2] = value; }
		}
		/// <summary>加盟店通信欄1</summary>
		public string ShopMessage1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE1] = value; }
		}
		/// <summary>加盟店通信欄2</summary>
		public string ShopMessage2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE2] = value; }
		}
		/// <summary>加盟店通信欄3</summary>
		public string ShopMessage3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE3] = value; }
		}
		/// <summary>加盟店通信欄4</summary>
		public string ShopMessage4
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE4] = value; }
		}
		/// <summary>加盟店通信欄5</summary>
		public string ShopMessage5
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE5]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_SHOP_MESSAGE5] = value; }
		}
		/// <summary>請求書形式</summary>
		public string InvoiceForm
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_FORM]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_INVOICE_FORM] = value; }
		}
		/// <summary>郵便口座番号</summary>
		public string PostalAccountNumber
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_ACCOUNT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_ACCOUNT_NUMBER] = value; }
		}
		/// <summary>郵便口座名義</summary>
		public string PostalAccountHolderName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_ACCOUNT_HOLDER_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_ACCOUNT_HOLDER_NAME] = value; }
		}
		/// <summary>郵便 OCR-B フォント：上段</summary>
		public string PostalFontTopRow
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_FONT_TOP_ROW]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_FONT_TOP_ROW] = value; }
		}
		/// <summary>郵便 OCR-B フォント：下段</summary>
		public string PostalFontBottomRow
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_FONT_BOTTOM_ROW]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_POSTAL_FONT_BOTTOM_ROW] = value; }
		}
		/// <summary>払込取扱用 購入者住所</summary>
		public string RemittanceAddress
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_REMITTANCE_ADDRESS]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_REMITTANCE_ADDRESS] = value; }
		}
		/// <summary>X 印</summary>
		public string XSymbol
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_X_SYMBOL]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_X_SYMBOL] = value; }
		}
		/// <summary>予備項目1</summary>
		public string Reserve1
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_RESERVE1]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_RESERVE1] = value; }
		}
		/// <summary>予備項目2</summary>
		public string Reserve2
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_RESERVE2]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_RESERVE2] = value; }
		}
		/// <summary>予備項目3</summary>
		public string Reserve3
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_RESERVE3]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_RESERVE3] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime? DateCreated
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CREATED] != DBNull.Value)
				? (DateTime?)this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CREATED]
				: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime? DateChanged
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CHANGED] != DBNull.Value)
				? (DateTime?)this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CHANGED]
				: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEVERITRANS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_INVOICEVERITRANS_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
