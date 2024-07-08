/*
=========================================================================================================
  Module      : 価格計算クラス(PriceCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.ProductFixedPurchaseDiscountSetting;

namespace w2.App.Common.Util
{
	/// <summary>
	/// 価格計算クラス
	/// </summary>
	public static class PriceCalculator
	{
		/// <summary>
		/// 明細金額を計算する
		/// </summary>
		/// <param name="unitPrice">単価</param>
		/// <param name="itemQuantity">数量</param>
		/// <returns>明細金額</returns>
		public static decimal GetItemPrice(decimal unitPrice, int itemQuantity)
		{
			var result = unitPrice * itemQuantity;
			return result;
		}

		/// <summary>
		/// 最終利用ポイントを計算する
		/// </summary>
		/// <param name="lastOrderPointUseOld">最終利用ポイント数(旧)</param>
		/// <param name="orderPointUseOld">利用ポイント数(旧)</param>
		/// <param name="orderPointUse">利用ポイント数</param>
		/// <returns>最終利用ポイント数</returns>
		public static decimal GetLastOrderPointUse(
			decimal lastOrderPointUseOld,
			decimal orderPointUseOld,
			decimal orderPointUse)
		{
			var result = lastOrderPointUseOld
				- orderPointUseOld
				+ orderPointUse;
			return result;
		}

		/// <summary>
		/// 受注編集時の利用可能ポイント数を計算する
		/// </summary>
		/// <param name="userPointUsableNow">現在の利用可能ポイント数</param>
		/// <param name="orderPointUseOld">利用ポイント数（変更前）</param>
		/// <param name="orderPointAddNew">ポイント付与数（変更後）</param>
		/// <param name="orderPointAddOld">ポイント付与数（変更前）</param>
		/// <param name="pointType">付与ポイント種別</param>
		/// <returns>利用可能ポイント数</returns>
		public static decimal GetUserPointUsable(
			decimal userPointUsableNow,
			decimal orderPointUseOld,
			decimal orderPointAddNew,
			decimal orderPointAddOld,
			string pointType)
		{
			// 利用ポイントチェック
			// ・利用可ポイント取得（現在の利用可能ポイント + 利用ポイント(変更前)）
			var userPointUsable = orderPointUseOld
				+ userPointUsableNow;

			// 発行ポイントが本ポイントの場合、発行ポイントの減算を考慮する必要がある。
			// そのため「付与ポイント(変更後) - 付与ポイント(変更前)」がマイナスの場合、利用可能なポイントに含める必要がある。
			// 但し、原則として、ポイントを付与した後に初めてポイントが利用できることとし、
			// 付与ポイントの減算があった場合のみ、以下処理を実行する。
			if (pointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
			{
				// 利用可ポイントに「付与ポイント(変更後) - 付与ポイント(変更前)」を加える
				var orderPointAdd = orderPointAddNew - orderPointAddOld;
				if (orderPointAdd < 0)
				{
					userPointUsable += orderPointAdd;
				}
			}
			return userPointUsable;
		}

		/// <summary>
		/// 割引額合計を計算する
		/// </summary>
		/// <param name="memberRankDiscountPrice">会員ランク割引</param>
		/// <param name="setpromotionProductDiscountAmount">セットプロモーション商品割引額</param>
		/// <param name="setpromotionShippingChargeDiscountAmount">セットプロモーション配送料割引額</param>
		/// <param name="setpromotionPaymentChargeDiscountAmount">セットプロモーション決済手数料割引額</param>
		/// <param name="orderPointUseYen">ポイント利用額</param>
		/// <param name="orderCouponUse">クーポン割引額</param>
		/// <param name="fixedPurchaseMemberDiscountAmount">定期会員割引額</param>
		/// <param name="fixedPurchaseDiscountPrice">定期購入割引額</param>
		/// <returns>割引額合計</returns>
		public static decimal GetOrderPriceDiscountTotal(
			decimal memberRankDiscountPrice,
			decimal setpromotionProductDiscountAmount,
			decimal setpromotionShippingChargeDiscountAmount,
			decimal setpromotionPaymentChargeDiscountAmount,
			decimal orderPointUseYen,
			decimal orderCouponUse,
			decimal fixedPurchaseMemberDiscountAmount,
			decimal fixedPurchaseDiscountPrice
		)
		{
			var result = memberRankDiscountPrice
				+ setpromotionProductDiscountAmount
				+ setpromotionShippingChargeDiscountAmount
				+ setpromotionPaymentChargeDiscountAmount
				+ orderPointUseYen
				+ orderCouponUse
				+ fixedPurchaseMemberDiscountAmount
				+ fixedPurchaseDiscountPrice;
			return result;
		}

		/// <summary>
		/// 価格を按分計算する
		/// </summary>
		/// <param name="targetPrice">按分対象の価格</param>
		/// <param name="denominatorPrice">割合の分子となる価格</param>
		/// <param name="numeratorPrice">割合の分母となる価格</param>
		/// <returns>按分後の価格（ゼロ除算の場合は0を返却）</returns>
		public static decimal GetDistributedPrice(
			decimal targetPrice,
			decimal denominatorPrice,
			decimal numeratorPrice)
		{
			// ゼロ除算の場合は、0を返却
			if (numeratorPrice == 0m) return 0m;

			var result = targetPrice * denominatorPrice / numeratorPrice;
			return result;
		}

		/// <summary>
		/// 商品定期購入割引設定から割引金額を取得
		/// </summary>
		/// <param name="discountSettingType">商品定期割引設定(値)</param>
		/// <param name="discountSettingValue">商品定期割引設定(種別)</param>
		/// <param name="itemPrice">商品価格</param>
		/// <param name="itemQuantity">商品個数</param>
		/// <returns>定期購入割引価格</returns>
		public static decimal GetFixedPurchaseDiscountPrice(
			string discountSettingType,
			decimal? discountSettingValue,
			decimal itemPrice,
			int itemQuantity)
		{
			var discountValue = discountSettingValue ?? 0;

			if (string.IsNullOrEmpty(discountSettingType)
				|| (discountValue <= 0)) return 0;

			var discountPriceSetting =
				(discountSettingType
					== Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN)
					? discountValue * itemQuantity
					: RoundingCalculationUtility.GetRoundPercentDiscountFraction(itemPrice, discountValue);

			var discountPrice = (discountPriceSetting < itemPrice)
				? discountPriceSetting
				: itemPrice;

			return discountPrice;
		}
	}
}