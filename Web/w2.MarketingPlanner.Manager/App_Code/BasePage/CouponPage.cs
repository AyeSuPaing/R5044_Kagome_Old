/*
=========================================================================================================
  Module      : クーポン設定共通ページ(Coupon.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Text;
using System.Web;
using w2.Common.Web;

/// <summary>
/// クーポン設定共通ページ
/// </summary>
public class CouponPage :BasePage
{
	#region #CreateCouponDetailUrl データバインド用クーポン詳細URL作成
	/// <summary>
	/// データバインド用クーポン詳細URL作成
	/// </summary>
	/// <param name="couponId">クーポンID</param>
	/// <returns>クーポン詳細URL</returns>
	protected string CreateCouponDetailUrl(string couponId)
	{
		var url = new StringBuilder();

		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_COUPON_CONFIRM)
		.Append("?").Append(Constants.REQUEST_KEY_COUPON_COUPON_ID).Append("=").Append(HttpUtility.UrlEncode(couponId))
		.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return url.ToString();
	}
	#endregion

	#region -CreateCouponListUrl クーポン一覧遷移URL作成
	/// <summary>
	/// クーポン一覧遷移URL作成
	/// </summary>
	/// <param name="searchParams">検索情報</param>
	/// <returns>クーポン一覧遷移URL</returns>
	protected string CreateCouponListUrl(Hashtable searchParams)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPONT_LIST)
			.AddParam(Constants.REQUEST_KEY_COUPON_COUPON_CODE, (string)searchParams["cc"])
			.AddParam(Constants.REQUEST_KEY_COUPON_COUPON_TYPE, (string)searchParams["ct"])
			.AddParam(Constants.REQUEST_KEY_COUPON_COUPON_NAME, (string)searchParams["cn"])
			.AddParam(Constants.REQUEST_KEY_COUPON_PUBLISH_DATE, (string)searchParams["cpd"])
			.AddParam(Constants.REQUEST_KEY_COUPON_VALID_FLG, (string)searchParams["cvf"])
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)searchParams["sort"])
			.CreateUrl();

		return url;
	}
	#endregion
}