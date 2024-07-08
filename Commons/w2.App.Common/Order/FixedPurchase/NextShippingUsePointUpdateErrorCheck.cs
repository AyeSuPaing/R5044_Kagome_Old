/*
=========================================================================================================
  Module      : 次回購入の利用ポイントを更新時エラーチェック (NextShippingUsePointUpdateErrorCheck.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Point;

namespace w2.App.Common.Order.FixedPurchase
{
	/// <summary>
	/// 次回購入の利用ポイントを更新時エラーチェック
	/// </summary>
	public class NextShippingUsePointUpdateErrorCheck
	{
		/// <summary>
		/// 次回購入の利用ポイントを更新時エラーチェック
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="newUsePoint">更新後の利用ポイント数</param>
		/// <param name="inputUseAllPointFlg">入力された全ポイント継続利用</param>
		/// <param name="priceSubtotalForCampaign">クーポン利用可能額</param>
		/// <returns>エラーメッセージ（エラーがない場合、空に返却）</returns>
		public static string CheckNextShippingUsePoint(FixedPurchaseContainer fixedPurchase, decimal newUsePoint, string inputUseAllPointFlg, decimal priceSubtotalForCampaign)
		{
			// 利用ポイント数の変更があるかどうかチェック(全ポイント継続利用中に更新がない場合もチェック）
			if (((newUsePoint == fixedPurchase.NextShippingUsePoint)
					&& ((inputUseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF) && (inputUseAllPointFlg == fixedPurchase.UseAllPointFlg)))
				|| ((inputUseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON) && (inputUseAllPointFlg == fixedPurchase.UseAllPointFlg)))
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NEXTSHIPPINGUSEPOINT_NO_CHANGE_ERROR);

			// ポイント利用可能単位チェック
			var service = new PointService();
			var pointMaster = service.GetPointMaster().Where(x => x.PointKbn == Constants.FLG_POINT_POINT_KBN_BASE);
			if (pointMaster.Any())
			{
				var usableUnit = pointMaster.First().UsableUnit;
				if ((newUsePoint % usableUnit) != 0) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_POINT_USABLE_UNIT_ERROR).Replace("@@ 1 @@", usableUnit.ToString());
			}

			// 利用ポイント数と注文の利用可能ポイント数の比較
			// セットプロモーション、タイムセールは非適用
			var orderPointUseablePrice = fixedPurchase.Shippings[0].Items.Select(x => x.GetItemPrice()).Sum();
			orderPointUseablePrice += fixedPurchase.Shippings[0].Items
				.Select(item => ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts(item.ProductOptionTexts))
				.Sum();
			var usePointPrice = service.GetOrderPointUsePrice(newUsePoint, Constants.FLG_POINT_POINT_KBN_BASE);
			if (usePointPrice > orderPointUseablePrice) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR).Replace("@@ 1 @@", CurrencyManager.ToPrice(orderPointUseablePrice));

			//割引適応後小計からポイント利用数を引いてマイナスにならないかチェック
			if (fixedPurchase.NextShippingUseCouponDetail != null)
			{
				var discountable = FixedPurchaseHelper.CheckDiscountableForNextFixedPurchase(fixedPurchase.NextShippingUseCouponDetail, newUsePoint, priceSubtotalForCampaign);
				if (discountable == false) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_EXCEEDED_AVAILABLE_POINTS_ERROR);
			}

			// ユーザの本ポイント取得
			var userCompPoint = PointOptionUtility.GetUserPoint(fixedPurchase.UserId, Constants.FLG_USERPOINT_POINT_KBN_BASE);
			// 利用ポイント数とユーザの利用可能ポイント数の比較
			if (userCompPoint.PointUsable < (newUsePoint - fixedPurchase.NextShippingUsePoint)) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NEXTSHIPPINGUSEPOINT_CHANGE_OVER_USER_POINT);

			return string.Empty;
		}
	}
}
