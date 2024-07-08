/*
=========================================================================================================
  Module      : 新着情報出力コントローラ処理(BodyNews.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;

public partial class Form_Common_BodyNews : BaseUserControl
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
			//------------------------------------------------------
			// 新着情報表示
			//------------------------------------------------------
			if (this.FindControl("rTopNewsList") != null)
			{
				// Get news list cache
				var newsLists = DataCacheControllerFacade.GetNewsCacheController().GetApplyNewsList(this.ShopId, this.BrandId, Constants.FLG_NEWS_MOBILE_DISP_FLG_PC);

				// 翻訳情報設定
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					newsLists = NameTranslationCommon.SetNewsTranslationData(
						newsLists,
						RegionManager.GetInstance().Region.LanguageCode,
						RegionManager.GetInstance().Region.LanguageLocaleId);
				}

				if ((newsLists != null) && (newsLists.Length != 0))
				{
					((Repeater)this.FindControl("rTopNewsList")).DataSource = newsLists;
				}
				else
				{
					((Repeater)this.FindControl("rTopNewsList")).Visible = false;
				}
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			this.DataBind();
		}
	}

	/// <summary>店舗ID（外部から設定可能）</summary>
	/// <remarks>モールカスタマイズを想定し、プロパティで変更できるように対応</remarks>
	public string ShopId
	{
		get { return string.IsNullOrEmpty(m_shopId) ? Constants.CONST_DEFAULT_SHOP_ID : m_shopId; }
		set { m_shopId = value; }
	}
	private string m_shopId = null;
}
