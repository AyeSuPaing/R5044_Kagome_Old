/*
=========================================================================================================
  Module      : 特集ページ一覧画面処理(FeaturePageList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.FeatureImage;
using w2.Domain.FeaturePageCategory;
using w2.Domain.FeaturePage;

public partial class Form_FeaturePage_FeaturePageList : BasePage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済コントロール宣言
	WrappedRepeater WrFeaturePageCategoryList { get { return GetWrappedControl<WrappedRepeater>("rFeaturePageCategoryList"); } }
	WrappedRepeater WrFeaturePageList { get { return GetWrappedControl<WrappedRepeater>("rFeaturePageList"); } }
	WrappedLabel WrPager1 { get { return GetWrappedControl<WrappedLabel>("lPager1"); } }
	WrappedLabel WrPager2 { get { return GetWrappedControl<WrappedLabel>("lPager2"); } }
	WrappedLabel WrAlertMessage { get { return GetWrappedControl<WrappedLabel>("lAlertMessage"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (Constants.FEATUREPAGESETTING_OPTION_ENABLED == false) return;

		if (!this.IsPostBack)
		{
			var featurePageDesignList = GetFeaturePageList();
			this.FeaturePageList = featurePageDesignList.Item2;

			var pagerUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FEATUREPAGE_FEATUREPAGELIST)
				.AddParam(Constants.REQUEST_KEY_FEATURE_PAGE_CATEGORY_ID, this.FeaturePageCategoryId)
				.CreateUrl();

			this.WrPager1.Text = this.WrPager2.Text = WebPager.CreateDefaultListPager(featurePageDesignList.Item1, this.PageNumber, pagerUrl);

			if (this.FeaturePageList.Length == 0)
			{
				this.WrAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FEATUREPAGE_NO_ITEM);
			}

			this.WrFeaturePageList.DataSource = this.FeaturePageList;
			this.WrFeaturePageList.DataBind();
		}
	}

	/// <summary>
	/// 特集ページ一覧取得
	/// </summary>
	/// <returns>総件数、特集ページ一覧</returns>
	private Tuple<int, FeaturePageModel[]> GetFeaturePageList()
	{
		var cache = DataCacheControllerFacade.GetFeaturePageCacheController().CacheData;

		// アクセスユーザ情報取得
		var accessUser = new ReleaseRangeAccessUser
		{
			Now = this.ReferenceDateTime,
			MemberRankInfo = this.LoginMemberRankInfo,
			IsLoggedIn = this.IsLoggedIn,
			HitTargetListId = this.LoginUserHitTargetListIds
		};

		var featurePageDesignListAll = cache
			.Where(model =>
			{
				if (string.IsNullOrEmpty(this.FeaturePageCategoryId)) return true;

				return (model.CategoryId.StartsWith(this.FeaturePageCategoryId, StringComparison.OrdinalIgnoreCase));
			})
			.Where(model =>
			{
				if (string.IsNullOrEmpty(this.BrandId)) return true;

				return (string.IsNullOrEmpty(model.PermittedBrandIds)
					|| model.PermittedBrandIds.Split(',').Any(brandId => (brandId == this.BrandId)));
			})
			.Where(model => new ReleaseRangeFeaturePage(model).IsPublish(accessUser))
			.OrderByDescending(model => model.ConditionPublishDateFrom)
			.ThenByDescending(model => model.DateChanged)
			.ToArray();

		var featurePageList = featurePageDesignListAll
			.Skip((this.PageNumber - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
			.Take(Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
			.ToArray();

		return new Tuple<int, FeaturePageModel[]>(featurePageDesignListAll.Count(), featurePageList);
	}

	/// <summary>ページ番号</summary>
	public int PageNumber
	{
		get
		{
			int pageNumber;
			if (int.TryParse((string)Request.QueryString[Constants.REQUEST_KEY_PAGE_NO], out pageNumber) == false) return 1;
			return pageNumber;
		}
	}
	/// <summary>特集ページリスト</summary>
	public FeaturePageModel[] FeaturePageList { get; set; }
	/// <summary>特集ページカテゴリID</summary>
	protected string FeaturePageCategoryId
	{
		get { return (string)Request.QueryString[Constants.REQUEST_KEY_FEATURE_PAGE_CATEGORY_ID]; }
	}
}