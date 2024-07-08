/*
=========================================================================================================
  Module      : 管理画面メニューキャッシュ(ManagerMenuCache.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.App.Common.Manager.Menu;

namespace w2.Cms.Manager.Codes.Menu
{
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
				{ "support", Constants.COOPERATION_SUPPORT_SITE },
				{ "sitemap", Constants.SITEMAP_OPTION_ENABLED },
				{ "coordinate_with_staff", Constants.COORDINATE_WITH_STAFF_OPTION_ENABLED },
				{ "product_ranking", Constants.PRODUCTRANKING_OPTION_ENABLED },
				{ "product_group", Constants.PRODUCTGROUP_OPTION_ENABLED },
				{ "product_list_disp_setting", Constants.PRODUCTLISTDISPSETTING_OPTION_ENABLED },
				{ "feature_area", Constants.FEATUREAREASETTING_OPTION_ENABLED },
				{ "news_list_disp_setting", Constants.NEWSLISTDISPSETTING_OPTION_ENABLED },
				{ "seo_tag_disp_setting", Constants.SEOTAGDISPSETTING_OPTION_ENABLED },
				{ "shorturl_setting", Constants.SHORTURL_OPTION_ENABLE },
				{ "ab_test", Constants.AB_TEST_OPTION_ENABLED },
				{ "scoring_sale_setting", Constants.SCORINGSALE_OPTION },
				{ "feature_page", Constants.FEATUREPAGESETTING_OPTION_ENABLED },
			};
		}

		/// <summary>機能一覧URL</summary>
		public override string PageIndexListUrl
		{
			get
			{
				if (m_pageIndexListUrl == null) m_pageIndexListUrl = UrlUtil.CreatepageIndexListUrl();
				return m_pageIndexListUrl;
			}
		}
		private string m_pageIndexListUrl = null;
		/// <summary>ログインオペレータメニュー</summary>
		public override IEnumerable<MenuLarge> LoginOperatorMenus { get { return new SessionWrapper().LoginOperatorMenus; } }
	}
}