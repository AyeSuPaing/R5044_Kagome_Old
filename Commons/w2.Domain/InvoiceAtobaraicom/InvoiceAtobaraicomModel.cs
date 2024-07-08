/*
=========================================================================================================
  Module      : 後払い.com請求書テーブルモデル (InvoiceAtobaraicomModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.InvoiceAtobaraicom
{
	/// <summary>
	/// 後払い.com請求書テーブルモデル
	/// </summary>
	[Serializable]
	public partial class InvoiceAtobaraicomModel : ModelBase<InvoiceAtobaraicomModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceAtobaraicomModel()
		{
			this.UseAmount = 0m;
			this.TaxAmount = 0m;
			this.LimitDate = null;
			this.NameKj = string.Empty;
			this.CvBarcodeData = string.Empty;
			this.CvBarcodeString1 = string.Empty;
			this.CvBarcodeString2 = string.Empty;
			this.YuMtOcrCode1 = string.Empty;
			this.YuMtOcrCode2 = string.Empty;
			this.YuAccountName = string.Empty;
			this.YuAccountNo = string.Empty;
			this.YuLoadKind = string.Empty;
			this.CvsCompanyName = string.Empty;
			this.CvsUserName = string.Empty;
			this.BkCode = string.Empty;
			this.BkBranchCode = string.Empty;
			this.BkName = string.Empty;
			this.BkBranchName = string.Empty;
			this.BkAccountKind = string.Empty;
			this.BkAccountNo = string.Empty;
			this.BkAccountName = string.Empty;
			this.BkAccountKana = string.Empty;
			this.MypagePwd = string.Empty;
			this.MypageUrl = string.Empty;
			this.CreditDeadline = string.Empty;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceAtobaraicomModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceAtobaraicomModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_ORDER_ID] = value; }
		}
		/// <summary>請求金額</summary>
		public decimal? UseAmount
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_USE_AMOUNT] != DBNull.Value)
					? (decimal?)this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_USE_AMOUNT]
					: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_USE_AMOUNT] = value; }
		}
		/// <summary>うち消費税額</summary>
		public decimal? TaxAmount
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_TAX_AMOUNT] != DBNull.Value)
					? (decimal?)this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_TAX_AMOUNT]
					: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_TAX_AMOUNT] = value; }
		}
		/// <summary>支払期限日</summary>
		public DateTime? LimitDate
		{
			get
			{
				return (this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_LIMIT_DATE] != DBNull.Value)
					? (DateTime?)this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_LIMIT_DATE]
					: null;
			}
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_LIMIT_DATE] = value; }
		}
		/// <summary>顧客氏名</summary>
		public string NameKj
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_NAME_KJ]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_NAME_KJ] = value; }
		}
		/// <summary>バーコードデータ</summary>
		public string CvBarcodeData
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CV_BARCODE_DATA]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CV_BARCODE_DATA] = value; }
		}
		/// <summary>バーコード文字列1</summary>
		public string CvBarcodeString1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CV_BARCODE_STRING1]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CV_BARCODE_STRING1] = value; }
		}
		/// <summary>バーコード文字列2</summary>
		public string CvBarcodeString2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CV_BARCODE_STRING2]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CV_BARCODE_STRING2] = value; }
		}
		/// <summary>ゆうちょ口座 - MT用OCRコード1</summary>
		public string YuMtOcrCode1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_MT_OCR_CODE1]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_MT_OCR_CODE1] = value; }
		}
		/// <summary>ゆうちょ口座 - MT用OCRコード2</summary>
		public string YuMtOcrCode2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_MT_OCR_CODE2]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_MT_OCR_CODE2] = value; }
		}
		/// <summary>ゆうちょ口座 - 加入者名</summary>
		public string YuAccountName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_ACCOUNT_NAME]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_ACCOUNT_NAME] = value; }
		}
		/// <summary>ゆうちょ口座 - 口座番号</summary>
		public string YuAccountNo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_ACCOUNT_NO]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_ACCOUNT_NO] = value; }
		}
		/// <summary>ゆうちょ口座 - 払込負担区分</summary>
		public string YuLoadKind
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_LOAD_KIND]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_YU_LOAD_KIND] = value; }
		}
		/// <summary>CVS収納代行会社名</summary>
		public string CvsCompanyName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CVS_COMPANY_NAME]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CVS_COMPANY_NAME] = value; }
		}
		/// <summary>CVS収納代行加入者名</summary>
		public string CvsUserName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CVS_USER_NAME]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CVS_USER_NAME] = value; }
		}
		/// <summary>銀行口座 - 銀行コード</summary>
		public string BkCode
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_CODE]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_CODE] = value; }
		}
		/// <summary>銀行口座 - 支店コード</summary>
		public string BkBranchCode
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_BRANCH_CODE]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_BRANCH_CODE] = value; }
		}
		/// <summary>銀行口座 - 銀行名</summary>
		public string BkName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_NAME]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_NAME] = value; }
		}
		/// <summary>銀行口座 - 支店名</summary>
		public string BkBranchName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_BRANCH_NAME]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_BRANCH_NAME] = value; }
		}
		/// <summary>銀行口座 - 口座種別</summary>
		public string BkAccountKind
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_KIND]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_KIND] = value; }
		}
		/// <summary>銀行口座 - 口座番号</summary>
		public string BkAccountNo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_NO]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_NO] = value; }
		}
		/// <summary>銀行口座 - 口座名義</summary>
		public string BkAccountName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_NAME]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_NAME] = value; }
		}
		/// <summary>銀行口座 - 口座名義カナ</summary>
		public string BkAccountKana
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_KANA]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_KANA] = value; }
		}
		/// <summary>マイページパスワード</summary>
		public string MypagePwd
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_MYPAGE_PWD]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_MYPAGE_PWD] = value; }
		}
		/// <summary>マイページURL</summary>
		public string MypageUrl
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_MYPAGE_URL]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_MYPAGE_URL] = value; }
		}
		/// <summary>クレジット利用期限日</summary>
		public string CreditDeadline
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CREDIT_DEADLINE]); }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_CREDIT_DEADLINE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_INVOICEATOBARAICOM_DATE_CREATED] = value; }
		}
		#endregion
	}
}
