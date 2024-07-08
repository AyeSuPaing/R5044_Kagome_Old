/*
=========================================================================================================
  Module      : 送料無料表示用基底ユーザコントロール(FreeShippingUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.Common.Util;

/// <summary>
/// FreeShippingUserControl の概要の説明です
/// </summary>
public class FreeShippingUserControl : BaseUserControl
{
	/// <summary>
	/// OnPreRenderイベント
	/// </summary>
	/// <param name="e"></param>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		if (this.IsDisplayAnnounceFreeShipping == false)
		{
			this.Visible = false;
		}
	}

	/// <summary>対象のカート</summary>
	public CartObject TargetCart
	{
		set
		{
			// 各プロパティに設定する
			decimal dFreeShippingPrice = value.GetDifferenceToFreeShippingPrice();
			this.ShopId = value.ShopId;
			this.CartId = value.CartId;
			this.ShippingId = value.ShippingType; // カート分割基準1
			this.DigitalContentsFlg = value.IsDigitalContentsOnly ? Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID : Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID; // カート分割基準2
			this.DifferenceFreeShippingPrice = dFreeShippingPrice.ToPriceDecimal().Value;
			// 送料無料とする設定金額が0円以外の場合、百分率を設定する
			if (value.ShippingFreePrice != 0)
			{
				this.DifferenceFreeShippingPercent = (dFreeShippingPrice / value.ShippingFreePrice * 100).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
			}
			this.IsFreeShipping = value.IsFreeShipping();
			this.IsUseFreeShippingSetting = (value.ShippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID);
			this.UseAnnounceFreeShippingFlg = (value.AnnounceFreeShippingFlg == Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID);
			// 配送無料クーポンの利用
			this.UseFreeShippingCouponFlg = value.IsFreeShippingCouponUse();
			this.IsFreeShippingBySetPromotion = value.SetPromotions.IsShippingChargeFree;
			this.IsUseFreeShippingFee = Constants.FREE_SHIPPING_FEE_OPTION_ENABLED
				&& value.Items.Any(item => item.ExcludeFreeShippingFlg ==
					Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID);

			// 送料無料案内メッセージを表示可能かを判断
			// ・配送先指定なし
			// ・配送先・特別配送先ごと配送料無料最低金額設定利用なし
			// ・配送先・特別配送先ごと配送料無料最低金額設定利用、かつ条件付き配送料が無料設定
			// のいずれかの場合、送料無料案内メッセージを表示する
			var shipping = value.Shippings.FirstOrDefault();
			var isDisplayable = ((shipping == null)
					|| (shipping.ConditionalShippingPriceThreshold == null)
					|| shipping.IsConditionalShippingPriceFree)
				&& (value.IsMemberRankFreeShippingThresholdUse() == false);
			this.Visible = ((value.IsGift == false) && isDisplayable);
		}
	}

	/// <summary>送料無料案内表示判定</summary>
	public bool IsDisplayAnnounceFreeShipping
	{
		get
		{
			if (ViewState["IsDisplayAnnounceFreeShipping"] == null)
			{
				ViewState["IsDisplayAnnounceFreeShipping"]
					= this.IsUseFreeShippingFee
						|| (this.UseAnnounceFreeShippingFlg
							&& (this.IsFreeShipping == false
									&& this.IsUseFreeShippingSetting
									&& this.UseFreeShippingCouponFlg == false
									&& this.IsFreeShippingBySetPromotion == false));
			}
			return (bool)ViewState["IsDisplayAnnounceFreeShipping"];
		}
	}

	/// <summary>店舗ID</summary>
	protected string ShopId { get; set; }
	/// <summary>カートID</summary>
	protected string CartId { get; set; }
	/// <summary>配送種別ID</summary>
	protected string ShippingId { get; set; }
	/// <summary>デジタルコンテンツフラグ</summary>
	protected string DigitalContentsFlg { get; private set; }
	/// <summary>送料無料判定</summary>
	protected bool IsFreeShipping { get; set; }
	/// <summary>送料無料設定有効フラグ</summary>
	protected bool IsUseFreeShippingSetting { get; set; }
	/// <summary>送料無料案内表示フラグ</summary>
	protected bool UseAnnounceFreeShippingFlg { get; set; }
	/// <summary>配送無料クーポン利用フラグ</summary>
	protected bool UseFreeShippingCouponFlg { get; set; }
	/// <summary>セットプロモーションによる配送料無料</summary>
	protected bool IsFreeShippingBySetPromotion { get; set; }
	/// <summary>送料無料までの差額</summary>
	public decimal DifferenceFreeShippingPrice { get; protected set; }
	/// <summary>送料無料までの差額(百分率)</summary>
	public decimal DifferenceFreeShippingPercent { get; protected set; }
	/// <summary>最低配送料金利用フラグ</summary>
	protected bool IsUseFreeShippingFee { get; set; }
}
