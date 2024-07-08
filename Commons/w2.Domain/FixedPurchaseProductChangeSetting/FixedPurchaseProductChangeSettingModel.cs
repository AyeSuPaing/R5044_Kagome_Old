/*
=========================================================================================================
  Module      : 定期商品変更設定モデル (FixedPurchaseProductChangeSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseProductChangeSetting
{
	/// <summary>
	/// 定期商品変更設定モデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseProductChangeSettingModel : ModelBase<FixedPurchaseProductChangeSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseProductChangeSettingModel()
		{
			this.FixedPurchaseProductChangeId = string.Empty;
			this.FixedPurchaseProductChangeName = string.Empty;
			this.Priority = 0;
			this.ValidFlg = Constants.FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseProductChangeSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseProductChangeSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>定期商品変更ID</summary>
		public string FixedPurchaseProductChangeId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID] = value; }
		}
		/// <summary>定期商品変更名</summary>
		public string FixedPurchaseProductChangeName
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME] = value; }
		}
		/// <summary>適用優先順</summary>
		public int Priority
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_PRIORITY] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
