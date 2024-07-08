/*
=========================================================================================================
  Module      : 新着情報一覧画面処理(NewsList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.Domain.News;

public partial class Form_NewsList : NewsPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 新着情報取得
			//------------------------------------------------------
			var newsList = new NewsService().GetNewsList(this.ShopId, this.BrandId, Constants.FLG_NEWS_MOBILE_DISP_FLG_PC);

			// 翻訳情報設定
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				newsList = NameTranslationCommon.SetNewsTranslationData(
					newsList,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);
			}

			//------------------------------------------------------
			// 新着情報表示
			//------------------------------------------------------
			if (this.FindControl("rNewsList") != null)
			{
				if ((newsList != null) && (newsList.Length != 0))
				{
					((Repeater)this.FindControl("rNewsList")).DataSource = newsList;
				}
				else
				{
					((Repeater)this.FindControl("rNewsList")).Visible = false;
				}
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			this.DataBind();
		}
	}
}
