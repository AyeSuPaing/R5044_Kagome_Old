/*
=========================================================================================================
  Module      : 頒布会キャッシュコントローラ (SubscriptionBoxCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.SubscriptionBox;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 頒布会キャッシュコントローラ
	/// </summary>
	public class SubscriptionBoxCacheController : DataCacheControllerBase<SubscriptionBoxModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SubscriptionBoxCacheController()
			: base(RefreshFileType.SubscriptionBox)
		{
		}

		/// <summary>
		/// キャッシュリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.SubscriptionBoxService.GetValidOnesWithChildren();
		}

		/// <summary>
		/// コースIDから取得
		/// </summary>
		/// <param name="courseId">コースID</param>
		/// <returns></returns>
		public SubscriptionBoxModel Get(string courseId)
		{
			var result = this.CacheData.FirstOrDefault(s => (s.CourseId == courseId));

			return result ?? new SubscriptionBoxModel();
		}

		/// <summary>
		/// 有効な頒布会を商品IDで取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns></returns>
		public SubscriptionBoxModel[] GetSubscriptionBoxesByProductId(string shopId, string productId, string variationId)
		{
			var today = DateTime.Now.Date;

			var result = this.CacheData.Where(
				cource =>
				{
					switch (cource.OrderItemDeterminationType)
					{
						case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
							return cource.SelectableProducts
								.Where(
									i => (i.ShopId == shopId)
										&& (i.ProductId == productId)
										&& (i.VariationId == variationId))
								.Any(i => i.IsInTerm(today));

						case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
							return cource.SelectableProducts
								.Where(
									i => (i.ShopId == shopId)
									&& (i.ProductId == productId)
									&& (i.VariationId == variationId))
								.Any(i => i.IsInTerm(today));

						default:
							return false;
					}
				}).ToArray();

			return result;
		}
	}
}
