/*
=========================================================================================================
  Module      : 電子発票情報モデル (TwInvoiceModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TwInvoice
{
	/// <summary>
	/// 電子発票情報モデル
	/// </summary>
	[Serializable]
	public partial class TwInvoiceModel : ModelBase<TwInvoiceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwInvoiceModel()
		{
			this.TwInvoiceNo = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwInvoiceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwInvoiceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>電子発票管理Id</summary>
		public string TwInvoiceId
		{
			get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_ID]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_ID] = value; }
		}
		/// <summary>発番開始日</summary>
		public DateTime TwInvoiceDateStart
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_START]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_START] = value; }
		}
		/// <summary>発番終了日</summary>
		public DateTime TwInvoiceDateEnd
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_END]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_END] = value; }
		}
		/// <summary>発票種別コード</summary>
		public string TwInvoiceCode
		{
			get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE] = value; }
		}
		/// <summary>発票種別名</summary>
		public string TwInvoiceTypeName
		{
			get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_TYPE_NAME]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_TYPE_NAME] = value; }
		}
		/// <summary>発票種別コード名</summary>
		public string TwInvoiceCodeName
		{
			get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE_NAME]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE_NAME] = value; }
		}
		/// <summary>発票開始番号</summary>
		public decimal TwInvoiceNoStart
		{
			get { return (decimal)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_START]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_START] = value; }
		}
		/// <summary>発票終了番号</summary>
		public decimal TwInvoiceNoEnd
		{
			get { return (decimal)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_END]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_END] = value; }
		}
		/// <summary>最終発票番号</summary>
		public decimal? TwInvoiceNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO];
			}
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO] = value; }
		}
		/// <summary>アラート値</summary>
		public int TwInvoiceAlertCount
		{
			get { return (int)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_ALERT_COUNT]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_ALERT_COUNT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWINVOICE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWINVOICE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TWINVOICE_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
