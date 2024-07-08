/*
=========================================================================================================
  Module      : 会員ランク情報ユーティリティクラス(MemberRankUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global.Region.Currency;
using w2.Common.Util;
using w2.Domain.MemberRank;

/// <summary>
/// 会員ランク情報ユーティリティクラス
/// </summary>
public class MemberRankUtility
{
	/// <summary>
	/// 注文金額の割引文言取得
	/// </summary>
	/// <returns>注文金額の割引文言</returns>
	public static string GetBenefitOrderDiscount(MemberRankModel memberRank, string format)
	{
		if (IsBenefitOrderDiscount(memberRank) == false) return "";

		var result = "";
		if ((memberRank.OrderDiscountType != Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE)
			&& (memberRank.OrderDiscountValue != 0))
		{
			// 割引部分の文言作成
			result = string.Format("{0} {1}",
				(memberRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE)
					? (memberRank.OrderDiscountValue != null)
						? DecimalUtility.DecimalRound(memberRank.OrderDiscountValue.Value, DecimalUtility.Format.RoundDown).ToString() 
						: string.Empty
					: CurrencyManager.ToPrice(memberRank.OrderDiscountValue),
				ValueText.GetValueText(Constants.TABLE_MEMBERRANK,
					Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE,
					memberRank.OrderDiscountType));

			// 閾値設定部分の文言作成
			if ((memberRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED)
				&& (memberRank.OrderDiscountThresholdPrice > 0))
			{
				result = string.Format(format, CurrencyManager.ToPrice(memberRank.OrderDiscountThresholdPrice), result);
			}
		}

		return result;
	}

	/// <summary>
	/// ポイント加算率の文言取得
	/// </summary>
	/// <returns>ポイント加算率の文言</returns>
	public static string GetBenefitPointAdd(MemberRankModel memberRank)
	{
		if (IsBenefitPointAdd(memberRank) == false) return "";

		var result = string.Format(" {0}{1}",
			memberRank.PointAddValue,
			ValueText.GetValueText(Constants.TABLE_MEMBERRANK,
				Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE,
				memberRank.PointAddType));
		return result;
	}

	/// <summary>
	/// 配送料割引の文言取得
	/// </summary>
	/// <returns>配送料割引の文言</returns>
	public static string GetBenefitShippingDiscount(MemberRankModel memberRank)
	{
		if (IsBenefitShippingDiscount(memberRank) == false) return "";

		var result = string.Format(" {0}{1}",
			(memberRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE)
				? ""
				: CurrencyManager.ToPrice(memberRank.ShippingDiscountValue),
			ValueText.GetValueText(Constants.TABLE_MEMBERRANK,
				Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE,
				memberRank.ShippingDiscountType));
		return result;
	}

	/// <summary>
	/// 定期会員割引の文言取得
	/// </summary>
	/// <returns>定期会員割引の文言</returns>
	public static string GetBenefitFixedPurchaseDiscountRate(MemberRankModel memberRank, string format)
	{
		if (IsBenefitFixedFuchaseDiscountRate(memberRank) == false) return "";

		var result = string.Format(format, memberRank.FixedPurchaseDiscountRate);
		return result;
	}

	/// <summary>
	/// 注文割引の特典有無
	/// </summary>
	/// <returns>注文割引の特典有無</returns>
	public static bool IsBenefitOrderDiscount(MemberRankModel memberRank)
	{
		if (memberRank.OrderDiscountValue == 0) return false;
		return ((memberRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE)
			|| (memberRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED));
	}

	/// <summary>
	/// ポイント加算の特典有無
	/// </summary>
	/// <returns>ポイント加算の特典有無</returns>
	public static bool IsBenefitPointAdd(MemberRankModel memberRank)
	{
		if (memberRank.PointAddValue == 0) return false;
		return (memberRank.PointAddType == Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_RATE);
	}

	/// <summary>
	/// 配送料割引の特典有無
	/// </summary>
	/// <returns>配送料割引の特典有無</returns>
	public static bool IsBenefitShippingDiscount(MemberRankModel memberRank)
	{
		if ((memberRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE)
			|| (memberRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD))
			return true;
		return ((memberRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED)
			&& (memberRank.ShippingDiscountValue > 0));
	}

	/// <summary>
	/// 定期会員割引の特典有無
	/// </summary>
	/// <returns>定期会員割引の特典有無</returns>
	public static bool IsBenefitFixedFuchaseDiscountRate(MemberRankModel memberRank)
	{
		return (memberRank.FixedPurchaseDiscountRate > 0);
	}
}
