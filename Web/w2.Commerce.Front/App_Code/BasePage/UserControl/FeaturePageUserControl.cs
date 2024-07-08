/*
=========================================================================================================
  Module      : 特集ページユーザコントロール(FeaturePageUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;

/// <summary>
/// 特集ページユーザコントロール
/// </summary>
public class FeaturePageUserControl : BaseUserControl
{
	/// <summary>
	/// 特集ページカテゴリIDと対象カテゴリIDが一致するか
	/// </summary>
	/// <param name="targetCategoryId">対象カテゴリID</param>
	/// <returns>True：特集ページカテゴリIDと対象カテゴリIDが一致、False：特集ページカテゴリIDと対象カテゴリIDが一致しない</returns>
	protected bool IsActiveCategory(string targetCategoryId)
	{
		var result = (targetCategoryId == this.FeaturePageCategoryId);
		return result;
	}

	/// <summary>
	/// 特集ページカテゴリ用リンク生成
	/// </summary>
	/// <param name="featurePageCategoryId">特集ページカテゴリID</param>
	protected string CreateFeaturePageCategoryLink(string featurePageCategoryId = null)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FEATUREPAGE_FEATUREPAGELIST);

		if (string.IsNullOrEmpty(featurePageCategoryId) == false)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_FEATURE_PAGE_CATEGORY_ID, featurePageCategoryId);
		}

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>特集ページカテゴリID</summary>
	protected string FeaturePageCategoryId
	{
		get { return (string)Request.QueryString[Constants.REQUEST_KEY_FEATURE_PAGE_CATEGORY_ID]; }
	}
	/// <summary>特集ページ親カテゴリID</summary>
	protected string FeaturePageParentCategoryId
	{
		get
		{
			return ((this.FeaturePageCategoryId != null) && (this.FeaturePageCategoryId.Length > 3))
				? this.FeaturePageCategoryId.Substring(0, 3)
				: this.FeaturePageCategoryId;
		}
	}
}