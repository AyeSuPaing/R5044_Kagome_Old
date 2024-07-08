/*
=========================================================================================================
  Module      : 商品グループ一覧ビューモデル (ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.ProductGroup;
using w2.Common.Util;
using w2.Domain.ProductGroup.Helper;
using Constants = w2.App.Common.Constants;

namespace w2.Cms.Manager.ViewModels.ProductGroup
{
	/// <summary>
	/// 商品グループ一覧ビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.ParamModel = new ProductGroupListParamModel();
			this.SearchResultListViewModel = new SearchResultListViewModel[0];
			this.BeginYearFromItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginMonthFromItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDayFromItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginYearToItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginMonthToItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDayToItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndYearFromItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndMonthFromItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDayFromItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndYearToItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndMonthToItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDayToItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.ValidItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PRODUCTGROUP, Constants.FIELD_PRODUCTGROUP_VALID_FLG);
			this.PagerHtml = "";
			this.ErrorMessage = "";
		}

		/// <summary>パラメタモデル</summary>
		public ProductGroupListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public SearchResultListViewModel[] SearchResultListViewModel { get; set; }
		/// <summary>選択肢群 開始年From</summary>
		public SelectListItem[] BeginYearFromItems { get; private set; }
		/// <summary>選択肢群 開始月From</summary>
		public SelectListItem[] BeginMonthFromItems { get; private set; }
		/// <summary>選択肢群 開始日From</summary>
		public SelectListItem[] BeginDayFromItems { get; private set; }
		/// <summary>選択肢群 開始年To</summary>
		public SelectListItem[] BeginYearToItems { get; private set; }
		/// <summary>選択肢群 開始月To</summary>
		public SelectListItem[] BeginMonthToItems { get; private set; }
		/// <summary>選択肢群 開始日To</summary>
		public SelectListItem[] BeginDayToItems { get; private set; }
		/// <summary>選択肢群 終了年From</summary>
		public SelectListItem[] EndYearFromItems { get; private set; }
		/// <summary>選択肢群 終了月From</summary>
		public SelectListItem[] EndMonthFromItems { get; private set; }
		/// <summary>選択肢群 終了日From</summary>
		public SelectListItem[] EndDayFromItems { get; private set; }
		/// <summary>選択肢群 終了年To</summary>
		public SelectListItem[] EndYearToItems { get; private set; }
		/// <summary>選択肢群 終了月To</summary>
		public SelectListItem[] EndMonthToItems { get; private set; }
		/// <summary>選択肢群 終了日To</summary>
		public SelectListItem[] EndDayToItems { get; private set; }
		/// <summary>選択肢群 有効</summary>
		public SelectListItem[] ValidItems { get; private set; }
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
		public SearchResultListViewModel(ProductGroupListSearchResult searchResult, UrlHelper urlHelper)
		{
			this.ProductGroupId = searchResult.ProductGroupId;
			this.ProductGroupName = searchResult.ProductGroupName;
			this.BeginDate = DateTimeUtility.ToStringForManager(searchResult.BeginDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.EndDate = searchResult.EndDate.HasValue
				? DateTimeUtility.ToStringForManager(searchResult.EndDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)
				: "";
			this.ValidFlg = ValueText.GetValueText(Constants.TABLE_PRODUCTGROUP, Constants.FIELD_PRODUCTGROUP_VALID_FLG, searchResult.ValidFlg);
			this.RegisterUrl = urlHelper.Action(
				"Edit",
				Constants.CONTROLLER_W2CMS_MANAGER_PRODUCT_GROUP,
				new
				{
					ActionStatus = ActionStatus.Update,
					ProductGroupId = this.ProductGroupId,
				});
		}

		/// <summary>商品グループID</summary>
		public string ProductGroupId { get; set; }
		/// <summary>商品グループ名</summary>
		public string ProductGroupName { get; set; }
		/// <summary>開始日時</summary>
		public string BeginDate { get; set; }
		/// <summary>終了日時</summary>
		public string EndDate { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>更新URL</summary>
		public string RegisterUrl { get; set; }
	}
}