/*
=========================================================================================================
  Module      : 会員ランク情報文言作成クラス(MemberRankBenefitWording.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Common.Util;
using w2.Domain.MemberRank;

/// <summary>
/// 会員ランク情報文言作成クラス
/// </summary>
public class MemberRankBenefitWording
{
	/// <summary>
	/// 会員ランク特典情報作成
	/// </summary>
	/// <param name="rankId">会員ランクID</param>
	/// <returns>会員ランク特典情報文言</returns>
	public static string CreateBenefitString(string rankId)
	{
		if (string.IsNullOrEmpty(rankId)) return "";

		var userRank = GetMemberRank(rankId);
		if (userRank == null) return "";

		var benefit = "";
		// 注文割引
		// ポイント加算
		// 配送料
		// 定期購入
		benefit = ConcatAddNewLine(
			CreateBenefitStringDiscount(userRank),
			CreateBenefitStringPoint(userRank),
			CreateBenefitStringShipping(userRank),
			CreateBenefitStringFixedPurchase(userRank));

		return benefit;
	}

	/// <summary>
	/// 文字列結合
	/// </summary>
	/// <param name="words">結合文字列</param>
	/// <returns>結合結果文字列</returns>
	private static string ConcatAddNewLine(params string[] words)
	{
		var result = string.Join("", words.Select(word => string.IsNullOrEmpty(word) ? "" : word + System.Environment.NewLine));
		return result.ToString();
	}

	/// <summary>
	/// 会員ランク情報取得
	/// </summary>
	/// <param name="rankId">会員ランクID</param>
	/// <returns>会員ランク情報</returns>
	private static MemberRankModel GetMemberRank(string rankId)
	{
		var result = MemberRankOptionUtility.GetMemberRankList().FirstOrDefault(memberRank => memberRank.MemberRankId == rankId);
		return result;
	}

	/// <summary>
	/// 注文割引文言作成
	/// </summary>
	/// <param name="userRank">会員ランク情報</param>
	/// <returns>注文割引文言</returns>
	private static string CreateBenefitStringDiscount(MemberRankModel userRank)
	{
		var result = "";
		if ((userRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE)
			|| (userRank.OrderDiscountValue.HasValue == false)
			|| (userRank.OrderDiscountValue == 0)) return result;

		// 閾値設定があれば購入条件の文字列を表示
		var threshold = ((userRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED)
			&& (userRank.OrderDiscountThresholdPrice > 0))
				? string.Format(
					//「{0} 以上購入時」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_MEMBER_RANK,
						Constants.VALUETEXT_PARAM_BENEFIT_WORDING,
						Constants.VALUETEXT_PARAM_MEMBER_RANK_AT_PURCHASE),
					userRank.OrderDiscountThresholdPrice.ToPriceString(true))
				: string.Empty;

		result = string.Format(
			//「注文金額割引：{0}{1}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MEMBER_RANK,
				Constants.VALUETEXT_PARAM_BENEFIT_WORDING,
				Constants.VALUETEXT_PARAM_MEMBER_RANK_ORDER_DISCOUNT),
			threshold,
			(userRank.OrderDiscountType == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE)
				? DecimalUtility.DecimalRound(userRank.OrderDiscountValue.Value, DecimalUtility.Format.RoundDown).ToString()
				: userRank.OrderDiscountValue.ToPriceString(true))
			+ ValueText.GetValueText(
				Constants.TABLE_MEMBERRANK,
				Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE,
				userRank.OrderDiscountType);

		return result;
	}

	/// <summary>
	/// ポイント加算率文言作成
	/// </summary>
	/// <param name="userRank">会員ランク情報</param>
	/// <returns>ポイント加算率文言</returns>
	private static string CreateBenefitStringPoint(MemberRankModel userRank)
	{
		var result = "";
		if ((userRank.PointAddType != Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_NONE) && (userRank.PointAddValue > 0))
		{
			result = string.Format(
				//「ポイント加算： {0}{1}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MEMBER_RANK,
					Constants.VALUETEXT_PARAM_BENEFIT_WORDING,
					Constants.VALUETEXT_PARAM_MEMBER_RANK_ADD_POINT),
				userRank.PointAddValue,
				ValueText.GetValueText(
					Constants.TABLE_MEMBERRANK,
					Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE,
					userRank.PointAddType));
		}
		return result;
	}

	/// <summary>
	/// 配送料文言作成
	/// </summary>
	/// <param name="userRank">会員ランク情報</param>
	/// <returns>配送料文言</returns>
	private static string CreateBenefitStringShipping(MemberRankModel userRank)
	{
		var result = "";
		if ((userRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE)
			|| (userRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD)
			|| ((userRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED)
				&& (userRank.ShippingDiscountValue > 0)))
		{
			result = string.Format(
				//「配送料割引： {0}{1}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MEMBER_RANK,
					Constants.VALUETEXT_PARAM_BENEFIT_WORDING,
					Constants.VALUETEXT_PARAM_MEMBER_RANK_SHIPPING_DISCOUNT),
				(userRank.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE)
					? string.Empty
					: userRank.ShippingDiscountValue.ToPriceString(true),
				ValueText.GetValueText(
					Constants.TABLE_MEMBERRANK,
					Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE,
					userRank.ShippingDiscountType));
		}
		return result;
	}

	/// <summary>
	/// 定期割引文言作成
	/// </summary>
	/// <param name="userRank">会員ランク情報</param>
	/// <returns>定期割引文言</returns>
	private static string CreateBenefitStringFixedPurchase(MemberRankModel userRank)
	{
		var result = (userRank.FixedPurchaseDiscountRate == 0)
			? string.Empty
			: string.Format(
			//「定期会員割引率： {0} % 割引」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MEMBER_RANK,
					Constants.VALUETEXT_PARAM_BENEFIT_WORDING,
					Constants.VALUETEXT_PARAM_MEMBER_RANK_DISCOUNT_RATE),
				userRank.FixedPurchaseDiscountRate);
		return result;
	}

	/// <summary>
	/// 会員ランクメモ文言作成
	/// </summary>
	/// <param name="rankId">会員ランク情報</param>
	/// <returns>会員ランクメモ文言</returns>
	public static string CreateBenefitStringMemo(string rankId)
	{
		if (string.IsNullOrEmpty(rankId)) return "";

		var userRank = GetMemberRank(rankId);
		if (userRank == null) return "";

		var result = GetMemberRank(rankId).MemberRankMemo;
		return result;
	}
}