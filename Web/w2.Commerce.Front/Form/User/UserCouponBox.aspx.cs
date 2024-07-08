/*
=========================================================================================================
  Module      : クーポンBOX画面処理(UserCouponBox.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Web;
using w2.Domain.Coupon.Helper;
using w2.App.Common.Option;
using w2.App.Common.Web.WrappedContols;

public partial class Form_User_CouponBox : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			var currentPageNo = GetCurrentPageNo();

			var now = DateTime.Now;
			var expireDate = now.AddDays(-Constants.COUPONBOX_DISPLAY_PASSED_DAYS_FROM_EXPIREDATE);

			string languageCode = null;
			string languageLocaleId = null;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				languageCode = RegionManager.GetInstance().Region.LanguageCode;
				languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
			}

			var userCoupons = CouponOptionUtility.GetUserUsableCouponsSpecificExpireDate(this.LoginUserId, this.LoginUserMail, now, expireDate, languageCode, languageLocaleId);

			BindCouponList(userCoupons, currentPageNo);
			CreateCouponListPager(userCoupons.Count(), currentPageNo);
		}
	}

	/// <summary>
	/// 現在のページ番号取得
	/// </summary>
	/// <returns>ページ番号</returns>
	private int GetCurrentPageNo()
	{
		int parsedCurrentPageNo;
		var currentPageNo = ((StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == "")
			|| (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO].ToString(), out parsedCurrentPageNo) == false))
			? 1
			: parsedCurrentPageNo;

		return currentPageNo;
	}

	/// <summary>
	/// 現ページの利用可能クーポンリストをバインド
	/// </summary>
	/// <param name="userCoupons">ユーザクーポン情報</param>
	/// <param name="currentPageNo">現在のページ番号</param>
	private void BindCouponList(UserCouponDetailInfo[] userCoupons, int currentPageNo)
	{
		// ユーザー表示用のクーポン名が空である場合、管理用のクーポン名を表示するようにする
		foreach (var cp in userCoupons)
		{
			if (string.IsNullOrEmpty(cp.CouponDispName) == false) continue;
			cp.CouponDispName = cp.CouponName;
		}

		// 画面上で、利用可能なクーポンの後に利用期限切れを表示
		var now = DateTime.Now;
		var userCouponsUnexpired = userCoupons.Where(cp => (cp.ExpireEnd >= now)).OrderBy(cp => cp.ExpireEnd).ToArray();
		var userCouponsExpired = userCoupons.Where(cp => (cp.ExpireEnd < now)).OrderBy(cp => cp.ExpireEnd).ToArray();
		var userCouponsForDisp = userCouponsUnexpired.Concat(userCouponsExpired);

		// クーポン情報にインデックスを採番し、現ページ分のみ抽出する
		var userUsableCouponsPage = userCouponsForDisp.Select((cp, index) => new { Coupon = cp, Index = index + 1 })
			.Where(cp => ((cp.Index >= (Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (currentPageNo - 1)) + 1)
				&& (cp.Index <= (Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * currentPageNo))))
			.Select(cp => cp.Coupon)
			.ToArray();

		this.WrCouponList.Visible = true;
		this.WrCouponList.DataSource = userUsableCouponsPage;
		this.WrCouponList.DataBind();
	}

	/// <summary>
	/// クーポンリストのページャ作成
	/// </summary>
	/// <param name="couponCount">クーポン件数</param>
	/// <param name="currentPageNo">現在のページ番号</param>
	private void CreateCouponListPager(int couponCount, int currentPageNo)
	{
		if (couponCount == 0)
		{
			this.WrCouponList.Visible = false;
			this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COUPONBOX_NO_ITEM);
			return;
		}

		var pageUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_COUPON_BOX;
		this.PagerHtml = WebPager.CreateDefaultListPager(couponCount, currentPageNo, pageUrl);
	}

	/// <summary>
	/// 利用可能回数表示文字列取得
	/// </summary>
	/// <param name="coupon">ユーザークーポン詳細情報</param>
	/// <returns>利用可能回数表示</returns>
	public static string GetCouponCount(UserCouponDetailInfo coupon)
	{
		return CouponOptionUtility.GetCouponCount(coupon);
	}
	
	/// <summary>
	/// クーポン割引文字列取得
	/// </summary>
	/// <param name="coupon">ユーザークーポン詳細情報</param>
	/// <returns>クーポン割引文字列</returns>
	public static string GetCouponDiscountString(UserCouponDetailInfo coupon)
	{
		var freeShippingMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
		var discountPriceMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);
		if (coupon.DiscountPrice != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + CurrencyManager.ToPrice(coupon.DiscountPrice);
			return CurrencyManager.ToPrice(coupon.DiscountPrice);
		}
		if (coupon.DiscountRate != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%";
			return StringUtility.ToNumeric(coupon.DiscountRate) + "%";
		}
		if (CouponOptionUtility.IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			return freeShippingMessage;
		}
		return "-";
	}

	/// <summary>ページャーHTML</summary>
	protected string PagerHtml { get; set; }
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage { get; private set; }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	#region  ラップ済みコントロール宣言
	WrappedRepeater WrCouponList { get { return GetWrappedControl<WrappedRepeater>("rCouponList"); } }
	#endregion
}
