/*
=========================================================================================================
  Module      : ユーザ電子発票管理情報モデル (TwUserInvoiceModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.TwUserInvoice
{
	/// <summary>
	/// ユーザ電子発票管理情報モデル
	/// </summary>
	[Serializable]
	public partial class TwUserInvoiceModel : ModelBase<TwUserInvoiceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwUserInvoiceModel()
		{
			this.TwInvoiceNo = 1;
			this.TwUniformInvoice = Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwUserInvoiceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwUserInvoiceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_USER_ID]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_USER_ID] = value; }
		}
		/// <summary>電子発票管理枝番</summary>
		[UpdateData(2, "tw_invoice_no")]
		public int TwInvoiceNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO] = value; }
		}
		/// <summary>電子発票情報名</summary>
		[UpdateData(3, "tw_invoice_name")]
		public string TwInvoiceName
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME] = value; }
		}
		/// <summary>電子発票種別</summary>
		[UpdateData(4, "tw_uniform_invoice")]
		public string TwUniformInvoice
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE] = value; }
		}
		/// <summary>電子発票項目1</summary>
		[UpdateData(5, "tw_uniform_invoice_option1")]
		public string TwUniformInvoiceOption1
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1] = value; }
		}
		/// <summary>電子発票項目2</summary>
		[UpdateData(6, "tw_uniform_invoice_option2")]
		public string TwUniformInvoiceOption2
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2] = value; }
		}
		/// <summary>載具種別</summary>
		[UpdateData(7, "tw_carry_type")]
		public string TwCarryType
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE] = value; }
		}
		/// <summary>載具項目</summary>
		[UpdateData(8, "tw_carry_type_option")]
		public string TwCarryTypeOption
		{
			get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(9, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(10, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
