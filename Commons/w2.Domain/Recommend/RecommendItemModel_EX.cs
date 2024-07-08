/*
=========================================================================================================
  Module      : レコメンドアイテムモデル (RecommendItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンドアイテムモデル
	/// </summary>
	public partial class RecommendItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>拡張項目_通常商品？</summary>
		public bool IsNormal
		{
			get { return this.RecommendItemType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_NORMAL; }
		}
		/// <summary>拡張項目_定期商品？</summary>
		public bool IsFixedPurchase
		{
			get { return this.RecommendItemType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_FIXED_PURCHASE; }
		}
		/// <summary>拡張項目_頒布会商品？</summary>
		public bool IsSubscriptionBox
		{
			get { return this.RecommendItemType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_SUBSCRIPTION_BOX; }
		}
		/// <summary>拡張項目_指定した数？</summary>
		public bool IsRecommendItemAddQuantityTypeSpecifyQuantity
		{
			get { return this.RecommendItemAddQuantityType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SPECIFY_QUANTITY; }
		}
		/// <summary>拡張項目_更新対象と同じ数？</summary>
		public bool IsRecommendItemAddQuantityTypeSameQuantity
		{
			get { return this.RecommendItemAddQuantityType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SAME_QUANTITY; }
		}
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return StringUtility.ToEmpty(_subscriptionBoxCourseId); }
			set { _subscriptionBoxCourseId = value; }
		}
		private string _subscriptionBoxCourseId;
		#endregion
	}
}
