/*
=========================================================================================================
  Module      : レコメンドアイテムモデル (RecommendItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンドアイテムモデル
	/// </summary>
	[Serializable]
	public partial class RecommendItemModel : ModelBase<RecommendItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RecommendItemModel()
		{
			this.RecommendItemType = Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_NORMAL;
			this.RecommendItemAddQuantityType = Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SPECIFY_QUANTITY;
			this.RecommendItemAddQuantity = 1;
			this.RecommendItemSortNo = 1;
			this.FixedPurchaseKbn = string.Empty;
			this.FixedPurchaseSetting1 = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_SHOP_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID] = value; }
		}
		/// <summary>レコメンドアイテム種別</summary>
		public string RecommendItemType
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_TYPE]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_TYPE] = value; }
		}
		/// <summary>レコメンドアイテム商品ID</summary>
		public string RecommendItemProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_PRODUCT_ID] = value; }
		}
		/// <summary>レコメンドアイテム商品バリエーションID</summary>
		public string RecommendItemVariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_VARIATION_ID] = value; }
		}
		/// <summary>レコメンドアイテム投入数種別</summary>
		public string RecommendItemAddQuantityType
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE] = value; }
		}
		/// <summary>レコメンドアイテム投入数</summary>
		public int RecommendItemAddQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY] = value; }
		}
		/// <summary>レコメンドアイテム並び順</summary>
		public int RecommendItemSortNo
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_SORT_NO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_LAST_CHANGED] = value; }
		}
		/// <summary>定期購入区分</summary>
		public string FixedPurchaseKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_KBN]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_KBN] = value; }
		}
		/// <summary>定期購入設定1</summary>
		public string FixedPurchaseSetting1
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_SETTING1]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_SETTING1] = value; }
		}
		#endregion
	}
}
