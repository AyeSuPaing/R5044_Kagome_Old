/*
=========================================================================================================
  Module      : ABテスト振り分け画面(AbTestAllotoment.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.ABTest.Util;
using w2.App.Common.DataCacheController;
using w2.Common.Web;
using w2.Domain.AbTest;
using w2.Domain.LandingPage;

public partial class Form_AbTest_AbTestAllotoment : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//クエリパラメータがないとき、エラーページへ
		if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_AB_TEST_ID]))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var abTestId = Request.QueryString[Constants.REQUEST_KEY_AB_TEST_ID];

		//キャッシュがあるかの判定
		var filePath = string.Empty;
		var cacheLpId = CookieManager.GetValue(abTestId);
		if (string.IsNullOrEmpty(cacheLpId) == false)
		{
			var lp = new LandingPageService().Get(cacheLpId);

			filePath = CreateUrlWithRequestParameters(
				Constants.PATH_ROOT + Constants.CMS_LANDING_PAGE_DIR_URL_PC + lp.PageFileName + ".aspx");
		}

		if (Constants.AB_TEST_OPTION_ENABLED)
		{
			//有効性の確認
			var errorNo = AbTestUtil.ValidateAbTest(abTestId);
			if (errorNo != AbTestUtil.ValidateStatus.NoError)
			{
				if ((errorNo == AbTestUtil.ValidateStatus.NoPage404) || string.IsNullOrEmpty(filePath))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			var abTestItems = new AbTestService().GetAllItemByAbTestId(abTestId);

			//ABテストの参照LPとキャッシュが一致しているか
			var isAgree = abTestItems.Any(item => (item.PageId == cacheLpId));

			if (isAgree == false)
			{
				filePath = string.Empty;
				CookieManager.RemoveCookie(abTestId, Constants.PATH_ROOT_FRONT_PC);
			}

			Url = string.IsNullOrEmpty(filePath) ? DecisionPageByDistributionRate(abTestItems, abTestId) : filePath;

			Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_ABTEST_ALLOTOMENT] = Url;
			Response.Redirect(Url);
			return;
		}

		if (string.IsNullOrEmpty(filePath) == false)
		{
			Response.Redirect(filePath);
			return;
		}
		
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		
	}

	/// <summary>
	/// 出し分け比率によって遷移先ページを決定する
	/// </summary>
	/// <param name="abTestItems">AbTestItems</param>
	/// <param name="abTestId">AbテストID</param>
	/// <returns>遷移先ページのURL</returns>
	private string DecisionPageByDistributionRate(AbTestItemModel[] abTestItems, string abTestId)
	{
		var landingPageId = string.Empty;
		
		if (abTestItems.Length == 1)
		{
			landingPageId = abTestItems[0].PageId;
		}
		else
		{
			var randomValue = new Random().Next(1, 100);
			foreach (var item in abTestItems)
			{
				if (randomValue <= item.DistributionRate)
				{
					landingPageId = item.PageId;
					break;
				}

				randomValue -= item.DistributionRate;
			}
		}

		// 遷移先LPの取得
		var lpModel = new LandingPageService().Get(landingPageId);

		Url = CreateUrlWithRequestParameters(
			Constants.PATH_ROOT + Constants.CMS_LANDING_PAGE_DIR_URL_PC + lpModel.PageFileName + ".aspx");
		return Url;
	}

	/// <summary>
	/// リクエストのパラメータを引き継いだURLを作成
	/// </summary>
	/// <param name="urlPath">ページパス</param>
	/// <returns>URL</returns>
	private string CreateUrlWithRequestParameters(string urlPath)
	{
		var urlCreator = new UrlCreator(urlPath);
		var parameterKeys = (Constants.AB_TEST_OPTION_ENABLED)
			? Request.QueryString.AllKeys
			: Request.QueryString.AllKeys.Where(key => (key == Constants.REQUEST_KEY_AB_TEST_ID) == false).ToArray();

		foreach (var key in parameterKeys)
		{
			urlCreator.AddParam(key, Request.QueryString[key]);
		}

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>遷移先URL</summary>
	private string Url { get; set; }
}
