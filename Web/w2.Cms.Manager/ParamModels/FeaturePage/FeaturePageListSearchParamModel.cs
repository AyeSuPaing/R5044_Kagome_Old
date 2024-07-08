/*
=========================================================================================================
  Module      : 特集ページ情報 特集ページ検索パラメータ (FeaturePageListSearchParamModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Cms;
using w2.Cms.Manager.Codes.Util;
using w2.Common.Util;
using w2.Domain.FeaturePage;

namespace w2.Cms.Manager.ParamModels.FeaturePage
{
	/// <summary>
	/// 特集ページ情報 特集ページ検索パラメータ
	/// </summary>
	public class FeaturePageListSearchParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageListSearchParamModel()
		{
			this.Keyword = string.Empty;

			this.DateChangedDropDown = new[]
			{
				new SelectListItem
				{
					Value = string.Empty,
					Text = string.Empty,
					Selected = true,
				}
			}.Concat(
				ValueTextForCms.GetValueSelectListItems(
					Constants.TABLE_FEATUREPAGE,
					Constants.FIELD_FEATUREPAGE_DATE_CHANGED))
			.ToArray();

			this.ParentCategoryItemsDropDown = new []
			{
				new SelectListItem()
				{
					Value = string.Empty,
					Text = string.Empty,
					Selected = true,
				}
			}.Concat(
				new FeaturePageService()
					.GetRootCategoryItem()
					.Select(
						s => new SelectListItem
						{
							Value = s.CategoryId,
							Text = s.CategoryName,
						})).ToArray();

			this.BrandItemsDropDown = new[]
			{
				new SelectListItem()
				{
					Value = string.Empty,
					Text = string.Empty,
					Selected = true,
				}
			}.Concat(
				new FeaturePageService()
					.GetBrand()
					.Select(
						s => new SelectListItem
						{
							Value = s.DataSource["brand_id"].ToString(),
							Text = s.DataSource["brand_name"].ToString(),
						})).ToArray();

			this.Types = new[]
			{
				new CheckBoxModel(
					ValueText.GetValueText(
						Constants.TABLE_FEATUREPAGE,
						Constants.FIELD_FEATUREPAGE_PUBLISH,
						Constants.FLG_FEATUREPAGE_PUBLISH_PUBLIC),
					Constants.FLG_FEATUREPAGE_PUBLISH_PUBLIC,
					true),
				new CheckBoxModel(
					ValueText.GetValueText(
						Constants.TABLE_FEATUREPAGE,
						Constants.FIELD_FEATUREPAGE_PUBLISH,
						Constants.FLG_FEATUREPAGE_PUBLISH_PRIVATE),
					Constants.FLG_FEATUREPAGE_PUBLISH_PRIVATE,
					true),
			};

			this.SortType = Constants.FLG_SORT_TYPE_ASC;
			this.SortField = Constants.FIELD_FEATUREPAGE_MANAGEMENT_TITLE;
			this.PagerNo = 1;
		}

		/// <summary>検索キーワード</summary>
		public string Keyword { get; set; }
		/// <summary>公開期間開始 : 開始日</summary>
		public string PublishBeginDateFromDate { get; set; }
		/// <summary>公開期間開始 : 開始日</summary>
		public string PublishBeginDateFromTime { get; set; }
		/// <summary>公開期間開始 : 終了日</summary>
		public string PublishBeginDateToDate { get; set; }
		/// <summary>公開期間開始 : 終了時間</summary>
		public string PublishBeginDateToTime { get; set; }
		/// <summary>公開期間終了 : 開始日</summary>
		public string PublishEndDateFromDate { get; set; }
		/// <summary>公開期間終了 : 開始日</summary>
		public string PublishEndDateFromTime { get; set; }
		/// <summary>公開期間終了 : 終了日</summary>
		public string PublishEndDateToDate { get; set; }
		/// <summary>公開期間終了 : 終了時間</summary>
		public string PublishEndDateToTime { get; set; }
		/// <summary>公開タイプ</summary>
		public CheckBoxModel[] Types { get; set; }
		/// <summary>更新日区分</summary>
		public string DateChangedKbn { get; set; }
		/// <summary>更新日ドロップダウン</summary>
		public SelectListItem[] DateChangedDropDown { get; set; }
		/// <summary>並び替えタイプ</summary>
		public string SortType { get; set; }
		/// <summary>並び替えフィールド</summary>
		public string SortField { get; set; }
		/// <summary>親カテゴリ</summary>
		public string ParentCategoryId { get; set; }
		/// <summary>親カテゴリID検索用ドロップダウンリスト</summary>
		public SelectListItem[] ParentCategoryItemsDropDown { get; set; }
		/// <summary>ブランドID</summary>
		public string BrandIds { get; set; }
		/// <summary>ブランドIDリスト</summary>
		public string BrandIdList { get; set; }
		/// <summary>ブランド検索用ドロップダウンリスト</summary>
		public SelectListItem[] BrandItemsDropDown { get; set; }
		/// <summary>ページ番号</summary>
		public int PagerNo { get; set; }
	}
}
