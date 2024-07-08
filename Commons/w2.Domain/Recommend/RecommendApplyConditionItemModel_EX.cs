/*
=========================================================================================================
  Module      : レコメンド適用条件アイテムモデル (RecommendApplyConditionItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド適用条件アイテムモデル
	/// </summary>
	public partial class RecommendApplyConditionItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// 拡張項目_過去注文もしくはカートで購入している？
		/// </summary>
		public bool IsRecommendApplyConditionTypeBuy
		{
			get { return (this.RecommendApplyConditionType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY); }
		}
		/// <summary>
		/// 拡張項目_過去注文もしくはカートで購入していない？
		/// </summary>
		public bool IsRecommendApplyConditionTypeNotBuy
		{
			get { return (this.RecommendApplyConditionType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY); }
		}
		/// <summary>拡張項目_通常商品？</summary>
		public bool IsNormal
		{
			get { return this.RecommendApplyConditionItemType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_NORMAL; }
		}
		/// <summary>拡張項目_定期商品？</summary>
		public bool IsFixedPurchase
		{
			get { return this.RecommendApplyConditionItemType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_FIXED_PURCHASE; }
		}
		/// <summary>拡張項目_頒布会商品？</summary>
		public bool IsSubscriptionBox
		{
			get { return this.RecommendApplyConditionItemType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_SUBSCRIPTION_BOX; }
		}
		/// <summary>
		/// 拡張項目_商品指定？
		/// </summary>
		public bool IsRecommendApplyConditionItemTypeProduct
		{
			get { return (this.RecommendApplyConditionItemUnitType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_PRODUCT); }
		}
		/// <summary>
		/// 拡張項目_商品バリエーション指定？
		/// </summary>
		public bool IsRecommendApplyConditionItemTypeVariation
		{
			get { return (this.RecommendApplyConditionItemUnitType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_VARIATION); }
		}
		#endregion
	}
}
