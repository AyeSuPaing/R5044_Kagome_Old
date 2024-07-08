/*
=========================================================================================================
  Module      : 特集ページリスト出力コントローラ処理(BodyFeaturePageList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.FeaturePageCategory;
using w2.Domain.FeaturePage;

public partial class Form_Common_FeaturePage_BodyFeaturePageList : FeaturePageUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrFeaturePageList { get { return GetWrappedControl<WrappedRepeater>("rFeaturePageList"); } }
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
			this.FeaturePageCategoryData = new FeaturePageCategoryService().Get(this.FeaturePageParentCategoryId);
			this.FeaturePageList = GetFeaturePageList();

			if (this.FeaturePageList.Length == 0) return;

			this.WrFeaturePageList.DataSource = this.FeaturePageList;
			this.WrFeaturePageList.DataBind();
		}
	}

	/// <summary>
	/// 特集ページ一覧取得
	/// </summary>
	/// <returns>特集ページ一覧</returns>
	private FeaturePageModel[] GetFeaturePageList()
	{
		var cache = DataCacheControllerFacade.GetFeaturePageCacheController().CacheData;

		var accessUser = new ReleaseRangeAccessUser
		{
			Now = this.ReferenceDateTime,
			MemberRankInfo = this.LoginMemberRankInfo,
			IsLoggedIn = this.IsLoggedIn,
			HitTargetListId = this.LoginUserHitTargetListIds
		};

		var FeaturePageList = cache
			.Where(model =>
			{
				if (string.IsNullOrEmpty(this.FeaturePageParentCategoryId)) return true;

				return (model.CategoryId.StartsWith(this.FeaturePageParentCategoryId, StringComparison.OrdinalIgnoreCase));
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
			.Take(this.MaxDispCount)
			.ToArray();

		return FeaturePageList;
	}

	/// <summary>特集ページカテゴリ情報</summary>
	protected FeaturePageCategoryModel FeaturePageCategoryData { get; set; }
	/// <summary>総件数</summary>
	protected int FeatureCount { get; set; }
	/// <summary>最大表示数</summary>
	protected int MaxDispCount { get; set; }
	/// <summary>特集ページ親カテゴリID</summary>
	protected new string FeaturePageParentCategoryId { get; set; }
	/// <summary>特集ページリスト</summary>
	protected FeaturePageModel[] FeaturePageList { get; set; }
	/// <summary>もっとみるURL</summary>
	protected string ViewMoreUrl
	{
		get
		{
			return CreateFeaturePageCategoryLink(this.FeaturePageParentCategoryId);
		}
	}
}