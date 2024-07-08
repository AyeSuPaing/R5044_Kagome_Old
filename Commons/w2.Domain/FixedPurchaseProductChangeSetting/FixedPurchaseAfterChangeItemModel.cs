/*
=========================================================================================================
  Module      : 定期変更後商品モデル (FixedPurchaseAfterChangeItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseProductChangeSetting
{
	/// <summary>
	/// 定期変更後商品モデル
	/// </summary>
	[Serializable]
	public class FixedPurchaseAfterChangeItemModel : ModelBase<FixedPurchaseAfterChangeItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseAfterChangeItemModel()
		{
			this.FixedPurchaseProductChangeId = string.Empty;
			this.ItemUnitType = string.Empty;
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.VariationId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseAfterChangeItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseAfterChangeItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>定期商品変更ID</summary>
		public string FixedPurchaseProductChangeId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID] = value; }
		}
		/// <summary>商品単位種別</summary>
		public string ItemUnitType
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_ITEM_UNIT_TYPE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_ITEM_UNIT_TYPE] = value; }
		}
		/// <summary>ショップID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_VARIATION_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
