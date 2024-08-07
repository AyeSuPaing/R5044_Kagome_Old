/*
=========================================================================================================
  Module      : レコメンドアップセル対象アイテムモデル (RecommendUpsellTargetItemModel.cs)
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
	/// レコメンドアップセル対象アイテムモデル
	/// </summary>
	[Serializable]
	public partial class RecommendUpsellTargetItemModel : ModelBase<RecommendUpsellTargetItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RecommendUpsellTargetItemModel()
		{
			this.RecommendUpsellTargetItemType = Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_NORMAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendUpsellTargetItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendUpsellTargetItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID] = value; }
		}
		/// <summary>レコメンドアップセル対象アイテム種別</summary>
		public string RecommendUpsellTargetItemType
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE] = value; }
		}
		/// <summary>レコメンドアップセル対象アイテム商品ID</summary>
		public string RecommendUpsellTargetItemProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_PRODUCT_ID] = value; }
		}
		/// <summary>レコメンドアップセル対象アイテム商品バリエーションID</summary>
		public string RecommendUpsellTargetItemVariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_VARIATION_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
