/*
=========================================================================================================
  Module      : レコメンド適用条件アイテムモデル (RecommendApplyConditionItemModel.cs)
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
	/// レコメンド適用条件アイテムモデル
	/// </summary>
	[Serializable]
	public partial class RecommendApplyConditionItemModel : ModelBase<RecommendApplyConditionItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RecommendApplyConditionItemModel()
		{
			this.RecommendApplyConditionType = Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY;
			this.RecommendApplyConditionItemType = Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_NORMAL;
			this.RecommendApplyConditionItemUnitType = Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_PRODUCT;
			this.RecommendApplyConditionItemSortNo = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendApplyConditionItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendApplyConditionItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID] = value; }
		}
		/// <summary>レコメンド適用条件種別</summary>
		public string RecommendApplyConditionType
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE] = value; }
		}
		/// <summary>レコメンド適用条件アイテム種別</summary>
		public string RecommendApplyConditionItemType
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE] = value; }
		}
		/// <summary>レコメンド適用条件アイテム単位種別</summary>
		public string RecommendApplyConditionItemUnitType
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE] = value; }
		}
		/// <summary>レコメンド適用条件アイテム商品ID</summary>
		public string RecommendApplyConditionItemProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_PRODUCT_ID] = value; }
		}
		/// <summary>レコメンドアイテム商品バリエーションID</summary>
		public string RecommendApplyConditionItemVariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_VARIATION_ID] = value; }
		}
		/// <summary>レコメンド適用条件アイテム並び順</summary>
		public int RecommendApplyConditionItemSortNo
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_SORT_NO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
