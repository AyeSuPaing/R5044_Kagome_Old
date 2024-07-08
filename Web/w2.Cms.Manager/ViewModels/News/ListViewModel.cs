/*
=========================================================================================================
  Module      : お知らせ Listビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.News;
using w2.Common.Util;
using w2.Domain.News.Helper;

namespace w2.Cms.Manager.ViewModels.News
{
	/// <summary>
	/// お知らせ Listビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.SearchResultListViewModel = new SearchResultListViewModel[0];
			this.SortKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_NEWS, "sort")
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text,
					// 並び順の初期値を設定
					Selected = (s.Value == "2")
				}).ToArray();
			this.ValidItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_NEWS, Constants.FIELD_NEWS_VALID_FLG);
			this.DispItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_NEWS, Constants.FIELD_NEWS_DISP_FLG);

			this.YearItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.MonthItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.DayItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
		}

		/// <summary>パラメタモデル</summary>
		public NewsListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public SearchResultListViewModel[] SearchResultListViewModel { get; set; }
		/// <summary>選択肢群 並び順</summary>
		public SelectListItem[] SortKbnItems { get; private set; }
		/// <summary>選択肢群 有効</summary>
		public SelectListItem[] ValidItems { get; private set; }
		/// <summary>選択肢群 Top表示</summary>
		public SelectListItem[] DispItems { get; private set; }
		/// <summary>選択肢群 年</summary>
		public SelectListItem[] YearItems { get; private set; }
		/// <summary>選択肢群 月</summary>
		public SelectListItem[] MonthItems { get; private set; }
		/// <summary>選択肢群 日</summary>
		public SelectListItem[] DayItems { get; private set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// 検索結果 Viewモデル
	/// </summary>
	public class SearchResultListViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="searchResult">検索結果</param>
		/// <param name="urlHelper">URLヘルパー</param>
		public SearchResultListViewModel(NewsListSearchResult searchResult, UrlHelper urlHelper)
		{
			this.NewsId = searchResult.NewsId;
			this.NewsText = StringUtility.AbbreviateString(searchResult.NewsText, 50);
			this.BrandId = searchResult.BrandId;
			this.ValidFlg = ValueText.GetValueText(Constants.TABLE_NEWS, Constants.FIELD_NEWS_VALID_FLG, searchResult.ValidFlg);
			this.DisplayOrder = searchResult.DisplayOrder.ToString();
			this.RegisterUrl = urlHelper.Action(
				"Register",
				Constants.CONTROLLER_W2CMS_MANAGER_NEWS,
				new
				{
					ActionStatus = ActionStatus.Update,
					NewsId = this.NewsId
				});
			this.DisplayDateFrom = DateTimeUtility.ToStringForManager(searchResult.DisplayDateFrom, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.DisplayDateTo = DateTimeUtility.ToStringForManager(searchResult.DisplayDateTo, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
				ValueText.GetValueText(Constants.TABLE_NEWS, "display_date_unspecified", string.Empty));

			this.DispFlg = (searchResult.DispFlg == Database.Common.Constants.FLG_NEWS_DISP_FLG_ALL);
		}

		/// <summary>新着ID</summary>
		public string NewsId { get; private set; }
		/// <summary>本文</summary>
		public string NewsText { get; private set; }
		/// <summary>ブランドID</summary>
		public string BrandId { get; private set; }
		/// <summary>有効</summary>
		public string ValidFlg { get; private set; }
		/// <summary>表示順</summary>
		public string DisplayOrder { get; private set; }
		/// <summary>表示日付 From</summary>
		public string DisplayDateFrom { get; private set; }
		/// <summary>表示日付 To</summary>
		public string DisplayDateTo { get; private set; }
		/// <summary>Top表示</summary>
		public bool DispFlg { get; private set; }
		/// <summary>登録遷移URL</summary>
		public string RegisterUrl { get; private set; }
	}
}