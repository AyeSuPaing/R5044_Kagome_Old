/*
=========================================================================================================
  Module      : レコメンドアップセル対象アイテムモデル (RecommendUpsellTargetItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンドアップセル対象アイテムモデル
	/// </summary>
	public partial class RecommendUpsellTargetItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>拡張項目_通常商品？</summary>
		public bool IsNormal
		{
			get { return this.RecommendUpsellTargetItemType == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_NORMAL; }
		}
		/// <summary>拡張項目_定期商品？</summary>
		public bool IsFixedPurchase
		{
			get { return this.RecommendUpsellTargetItemType == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_FIXED_PURCHASE; }
		}
		/// <summary>拡張項目_頒布会商品？</summary>
		public bool IsSubscriptionBox
		{
			get { return this.RecommendUpsellTargetItemType == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_SUBSCRIPTION_BOX; }
		}
		#endregion
	}
}
