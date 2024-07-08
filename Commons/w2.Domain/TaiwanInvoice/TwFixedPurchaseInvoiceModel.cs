/*
=========================================================================================================
  Module      : 定期購入電子発票情報モデル (TwFixedPurchaseInvoiceModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TwFixedPurchaseInvoice
{
	/// <summary>
	/// 定期購入電子発票情報モデル
	/// </summary>
	[Serializable]
	public partial class TwFixedPurchaseInvoiceModel : ModelBase<TwFixedPurchaseInvoiceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwFixedPurchaseInvoiceModel()
		{
			this.FixedPurchaseShippingNo = 1;
			this.TwUniformInvoice = Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwFixedPurchaseInvoiceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwFixedPurchaseInvoiceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>定期購入電子発票枝番</summary>
		public int FixedPurchaseShippingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_SHIPPING_NO] = value; }
		}
		/// <summary>電子発票種別</summary>
		public string TwUniformInvoice
		{
			get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE] = value; }
		}
		/// <summary>電子発票項目1</summary>
		public string TwUniformInvoiceOption1
		{
			get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1] = value; }
		}
		/// <summary>電子発票項目2</summary>
		public string TwUniformInvoiceOption2
		{
			get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION2]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION2] = value; }
		}
		/// <summary>載具種別</summary>
		public string TwCarryType
		{
			get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE] = value; }
		}
		/// <summary>載具項目</summary>
		public string TwCarryTypeOption
		{
			get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
