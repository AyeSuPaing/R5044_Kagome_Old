/*
=========================================================================================================
  Module      : 会員ランク共通ページ(MemberRankPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Extensions.Currency;

/// <summary>
/// 会員ランク共通ページ
/// </summary>
public class MemberRankPage : BasePage
{
	/// <summary>
	/// 注文割引指定を取得する
	/// </summary>
	/// <param name="discountValue">割引する値</param>
	/// <param name="discountText">割引テキスト</param>
	/// <returns>注文割引指定値</returns>
	public string GetOrderDiscountType(decimal? discountValue, string discountText)
	{
		var str = string.Format(
			"{0} {1}",
			discountValue.ToPriceString((discountText == Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED)),
			GetOrderDiscountTypeText(discountText));
		return str;
	}

	/// <summary>
	/// 配送割引指定を取得する
	/// </summary>
	/// <param name="discountValue">割引する値</param>
	/// <param name="discountText">割引テキスト</param>
	/// <returns>配送割引指定値</returns>
	public string GetShippingDiscountType(decimal? discountValue, string discountText)
	{
		var str = string.Format(
			"{0} {1}",
			discountValue.ToPriceString(
				((discountText == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED)
					|| (discountText == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD))),
			GetShippingDiscountTypeText(discountText));
		return str;
	}

	/// <summary>
	/// 注文割引指定のテキストを取得する
	/// </summary>
	/// <param name="orderDiscountType">注文割引指定</param>
	/// <returns>注文割引指定のテキスト</returns>
	private string GetOrderDiscountTypeText(string orderDiscountType)
	{
		var str = ValueText.GetValueText(
			Constants.TABLE_MEMBERRANK,
			Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE,
			orderDiscountType);
		return str;
	}

	/// <summary>
	/// 配送割引指定のテキストを取得する
	/// </summary>
	/// <param name="shippingDiscountType">配送割引指定</param>
	/// <returns>配送割引指定のテキスト</returns>
	private string GetShippingDiscountTypeText(string shippingDiscountType)
	{
			var str = ValueText.GetValueText(
			Constants.TABLE_MEMBERRANK,
			Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE,
			shippingDiscountType);
		return str;
	}
}