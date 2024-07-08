/*
=========================================================================================================
  Module      : 管理画面メニューキャッシュ(ManagerMenuCache.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Web;
using w2.App.Common.Manager.Menu;

/// <summary>
/// 管理画面メニューキャッシュ
/// </summary>
public class ManagerMenuCache : ManagerMenuCacheBase<ManagerMenuCache>
{
	/// <summary>
	/// オプションステータス取得
	/// </summary>
	/// <returns>メニュー有効状態</returns>
	protected override Dictionary<string, bool> GetOptionStatus()
	{
		return new Dictionary<string, bool>
		{
			{ "coupon", Constants.MARKETINGPLANNER_COUPON_OPTION_ENABLE},	// クーポンオプション用
			{ "mail", Constants.MARKETINGPLANNER_MAIL_OPTION_ENABLE},	// メールオプション用
			{ "point", Constants.MARKETINGPLANNER_POINT_OPTION_ENABLE},	// ポイントオプション用
			{ "affiliate", Constants.MARKETINGPLANNER_AFFILIATE_OPTION_ENABLE},	// 広告コード
			{ "multipurpose_affiliate", Constants.MARKETINGPLANNER_MULTIPURPOSE_AFFILIATE_OPTION_ENABLE},	// 汎用アフィリエイト
			{ "memberrank", Constants.MEMBER_RANK_OPTION_ENABLED},	// 会員ランクオプション用
			{ "support", Constants.COOPERATION_SUPPORT_SITE},	// サポートサイト用
			{ "cpm", Constants.CPM_OPTION_ENABLED},  // CPM（顧客ポートフォリオマネジメント）オプション用
			{ "fixedpurchase", Constants.FIXEDPURCHASE_OPTION_ENABLED}, // 定期購入オプション用
			{ "targetlist_setting", Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE}, // ターゲットリストオプション用
			{ "recommend", (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg)}, // レコメンドオプション用
			{ "fixedpurchase_forecast", Constants.FIXED_PURCHASE_FORECAST_REPORT_OPTION },
			{
				"memberrankrulelist",
				(Constants.MEMBER_RANK_OPTION_ENABLED
					&& (Constants.CROSS_POINT_OPTION_ENABLED == false))
			},
			{
				"usermemberrankhistorylist",
				(Constants.MEMBER_RANK_OPTION_ENABLED
					&& (Constants.CROSS_POINT_OPTION_ENABLED == false))
			},
			{ "recommend_report", Constants.RECOMMEND_OPTION_ENABLED },
			{ 
				"shipment_forecast_report",
				(Constants.SHIPTMENT_FORECAST_BY_DAYS_ENABLED && Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE)
			},
		};
	}

	/// <summary>機能一覧URL</summary>
	public override string PageIndexListUrl
	{
		get { return Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAGE_INDEX_LIST; }
	}
	/// <summary>ログインオペレータメニューセッションキー</summary>
	public override IEnumerable<MenuLarge> LoginOperatorMenus
	{
		get { return (List<MenuLarge>)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
}
