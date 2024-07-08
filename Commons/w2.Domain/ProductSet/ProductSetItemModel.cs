/*
=========================================================================================================
  Module      : 商品セットアイテムマスタモデル (ProductSetItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductSet
{
	/// <summary>
	/// 商品セットアイテムマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductSetItemModel : ModelBase<ProductSetItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductSetItemModel()
		{
			this.ShopId = "";
			this.ProductSetId = "";
			this.ProductId = "";
			this.VariationId = "";
			this.SetitemPrice = 0;
			this.CountMin = null;
			this.CountMax = null;
			this.FamilyFlg = "1";
			this.DisplayOrder = 1;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSetItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSetItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSETITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_SHOP_ID] = value; }
		}
		/// <summary>商品セットID</summary>
		public string ProductSetId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSETITEM_PRODUCT_SET_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_PRODUCT_SET_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSETITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSETITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_VARIATION_ID] = value; }
		}
		/// <summary>セット価格</summary>
		public decimal SetitemPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE] = value; }
		}
		/// <summary>個数（下限）</summary>
		public int? CountMin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] = value; }
		}
		/// <summary>個数（上限）</summary>
		public int? CountMax
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] = value; }
		}
		/// <summary>親子フラグ</summary>
		public string FamilyFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSETITEM_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSETITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSETITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSETITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
