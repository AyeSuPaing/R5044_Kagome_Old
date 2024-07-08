/*
=========================================================================================================
  Module      : 注文電子発票情報モデル (TwOrderInvoiceModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.TwOrderInvoice
{
	/// <summary>
	/// 注文電子発票情報モデル
	/// </summary>
	[Serializable]
	public partial class TwOrderInvoiceModel : ModelBase<TwOrderInvoiceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwOrderInvoiceModel()
		{
			this.OrderId = string.Empty;
			this.OrderShippingNo = 1;
			this.TwUniformInvoice = Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL;
			this.TwInvoiceDate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwOrderInvoiceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwOrderInvoiceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		[UpdateData(1, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_ID] = value; }
		}
		/// <summary>電子発票枝番</summary>
		[UpdateData(2, "order_shipping_no")]
		public int OrderShippingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO] = value; }
		}
		/// <summary>電子発票種別</summary>
		[UpdateData(3, "tw_uniform_invoice")]
		public string TwUniformInvoice
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE] = value; }
		}
		/// <summary>電子発票項目1</summary>
		[UpdateData(4, "tw_uniform_invoice_option1")]
		public string TwUniformInvoiceOption1
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1] = value; }
		}
		/// <summary>電子発票項目2</summary>
		[UpdateData(5, "tw_uniform_invoice_option2")]
		public string TwUniformInvoiceOption2
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2] = value; }
		}
		/// <summary>載具種別</summary>
		[UpdateData(6, "tw_carry_type")]
		public string TwCarryType
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE] = value; }
		}
		/// <summary>載具項目</summary>
		[UpdateData(7, "tw_carry_type_option")]
		public string TwCarryTypeOption
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION] = value; }
		}
		/// <summary>発票番号</summary>
		[UpdateData(8, "tw_invoice_no")]
		public string TwInvoiceNo
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO] = value; }
		}
		/// <summary>発票日時</summary>
		[UpdateData(9, "tw_invoice_date")]
		public DateTime? TwInvoiceDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE];
			}
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE] = value; }
		}
		/// <summary>発票ステータス</summary>
		[UpdateData(10, "tw_invoice_status")]
		public string TwInvoiceStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(11, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(12, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
