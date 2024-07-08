/*
=========================================================================================================
  Module      : セットプロモーションキャッシュコントローラ(SetPromotionCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.Product;
using w2.Domain.SetPromotion;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// セットプロモーションキャッシュプロバイダ
	/// </summary>
	public class SetPromotionCacheController : DataCacheControllerBase<SetPromotionModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal SetPromotionCacheController()
			: base(RefreshFileType.SetPromotion)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.SetPromotionService.GetUsable();
		}

		/// <summary>
		/// 商品割引セットプロモーション設定を取得
		/// </summary>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="userTargetListIds">ユーザーターゲットリスト</param>
		/// <returns>セットプロモーション設定</returns>
		public SetPromotionModel[] GetProductDiscountSetPromotion(
			string memberRankId,
			string orderKbn,
			string[] userTargetListIds)
		{
			var result = GetApplicableSetPromotions(memberRankId, orderKbn, userTargetListIds)
				.Where(item => item.IsDiscountTypeProductDiscount)
				.OrderBy(setting => setting.ApplyOrder)
				.ThenBy(setPromotion => setPromotion.SetpromotionId)
				.ToArray();
			return result;
		}

		/// <summary>
		/// 配送料無料セットプロモーション設定を取得
		/// </summary>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="userTargetListIds">ユーザーターゲットリスト</param>
		/// <returns>セットプロモーション設定</returns>
		public SetPromotionModel[] GetShippingDiscountSetPromotion(
			string memberRankId,
			string orderKbn,
			string[] userTargetListIds)
		{
			var result = GetApplicableSetPromotions(memberRankId, orderKbn, userTargetListIds)
				.Where(item => item.IsDiscountTypeShippingChargeFree).ToArray();
			return result;
		}

		/// <summary>
		/// 決済手数料無料セットプロモーション設定を取得
		/// </summary>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="userTargetListIds">ユーザーターゲットリスト</param>
		/// <returns>セットプロモーション設定</returns>
		public SetPromotionModel[] GetPaymentDiscountSetPromotion(
			string memberRankId,
			string orderKbn,
			string[] userTargetListIds)
		{
			var result = GetApplicableSetPromotions(memberRankId, orderKbn, userTargetListIds)
				.Where(item => item.IsDiscountTypePaymentChargeFree).ToArray();
			return result;
		}

		// HACK:本来はここに記述したくない
		/// <summary>
		/// 有効なセットプロモーション設定を取得
		/// </summary>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="targetLists">ターゲットリスト</param>
		/// <returns>有効なセットプロモーション設定</returns>
		public SetPromotionModel[] GetApplicableSetPromotions(string memberRankId, string orderKbn, string[] targetLists)
		{
			return this.CacheData.Where(sp => (
				(sp.BeginDate <= DateTime.Now)
				&& ((sp.EndDate == null) || (sp.EndDate >= DateTime.Now))
				&& ((sp.TargetMemberRank == "") || (MemberRankOptionUtility.CheckMemberRankPermission(memberRankId, sp.TargetMemberRank)))
				&& (sp.TargetOrderKbn.Contains(orderKbn))
				&& ((string.IsNullOrEmpty(sp.TargetIds)) || targetLists.Any(targetList => sp.TargetIds.Contains(targetList)))
				)).ToArray();
		}

		/// <summary>
		/// 商品情報から、その商品を含むセットプロモーション設定を取得
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="isVariation">バリエーションを考慮するかどうか</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="targetLists">ターゲットリスト</param>
		/// <returns>該当商品を含むセットプロモーション設定</returns>
		public SetPromotionModel[] GetSetPromotionByProduct(object product, bool isVariation, string memberRankId, string orderKbn, string[] targetLists)
		{
			if (Constants.SETPROMOTION_OPTION_ENABLED == false) return null;

			if (product is ProductModel)
			{
				return GetApplicableSetPromotions(memberRankId, orderKbn, targetLists).Where(item => item.ContainsProduct((ProductModel)product, isVariation)).ToArray();
			}

			if (product is DataRowView)
			{
				return GetApplicableSetPromotions(memberRankId, orderKbn, targetLists).Where(item => item.ContainsProduct((DataRowView)product, isVariation)).ToArray();
			}

			return null;
		}
		/// <summary>
		/// 商品情報から、その商品を含むセットプロモーション設定を取得
		/// </summary>
		/// <param name="product">カート商品</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="targetLists">ターゲットリスト</param>
		/// <returns>該当商品を含むセットプロモーション設定</returns>
		public SetPromotionModel[] GetSetPromotionByProduct(CartProduct product, string memberRankId, string orderKbn, string[] targetLists)
		{
			if (Constants.SETPROMOTION_OPTION_ENABLED == false) return null;

			return GetApplicableSetPromotions(memberRankId, orderKbn, targetLists).Where(item =>
				item.ContainsProduct(product.ProductId, product.VariationId, product.CategoryId1, product.CategoryId2, product.CategoryId3, product.CategoryId4, product.CategoryId5)).ToArray();
		}

	}
}
