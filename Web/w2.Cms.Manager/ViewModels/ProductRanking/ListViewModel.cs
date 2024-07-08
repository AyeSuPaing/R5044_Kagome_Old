/*
=========================================================================================================
  Module      : 商品ランキング一覧ビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.ProductRanking;
using w2.Common.Util;
using w2.Domain.ProductRanking;

namespace w2.Cms.Manager.ViewModels.ProductRanking
{
	/// <summary>
	/// 商品ランキング一覧ビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.List = new SearchResultListViewModel[0];
			this.SortKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PRODUCTRANKING, "sort")
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text,
					// 並び順の初期値を設定
					Selected = (s.Value == "0")
				}).ToArray();
			this.ValidItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PRODUCTRANKING, Constants.FIELD_PRODUCTRANKING_VALID_FLG);
			this.TotalTypes = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PRODUCTRANKING, Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE);

		}
		/// <summary>パラメタモデル</summary>
		public ProductRankingListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public SearchResultListViewModel[] List { get; set; }
		/// <summary>選択肢群 並び順</summary>
		public SelectListItem[] SortKbnItems { get; private set; }
		/// <summary>選択肢群 有効</summary>
		public SelectListItem[] ValidItems { get; private set; }
		/// <summary>選択肢群 集計タイプ</summary>
		public SelectListItem[] TotalTypes { get; private set; }
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
		public SearchResultListViewModel()
		{
			this.RankingId = string.Empty;
			this.TotalType = string.Empty;
			this.TotalKbn = string.Empty;
			this.TotalDays = string.Empty;
			this.TotalDate = string.Empty;
			this.CategoryId = string.Empty;
			this.ExcludeCategoryIds = string.Empty;
			this.BrandId = string.Empty;
			this.BrandKbn = string.Empty;
			this.StockKbn = string.Empty;
			this.ValidFlg = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="searchResult">検索結果</param>
		/// <param name="categoryIds">表示用カテゴリID</param>
		/// <param name="excludeCategoryIds">表示用含まないカテゴリID</param>
		public SearchResultListViewModel(
			ProductRankingModel searchResult,
			string categoryIds,
			string excludeCategoryIds) : this()
		{
			this.RankingId = searchResult.RankingId;
			this.TotalType = searchResult.TotalType;
			this.TotalKbn = searchResult.TotalKbn;
			this.TotalDays = searchResult.TotalDays;
			var totalDate = DisplayTotalDate(searchResult);
			this.TotalDate = string.IsNullOrEmpty(totalDate) ? "-" : totalDate;
			this.CategoryId = categoryIds;
			this.ExcludeCategoryIds = excludeCategoryIds;
			this.BrandId = searchResult.BrandId;
			this.BrandKbn = searchResult.BrandKbn;
			this.StockKbn = searchResult.StockKbn;
			this.ValidFlg = searchResult.ValidFlg;
		}

		/// <summary>
		/// 集計タイプと集計期間区分に合わせた日時表記
		/// </summary>
		/// <param name="searchResult">検索結果情報</param>
		/// <returns>集計日時</returns>
		protected string DisplayTotalDate(ProductRankingModel searchResult)
		{
			if (this.IsTotalTypeAuto != true) return string.Empty;

			var date = string.Empty;
			// 日数指定
			if (searchResult.TotalKbn == Constants.FLG_PRODUCTRANKING_TOTAL_KBN_DAYS)
			{
				date = string.Format(
					"{0}～{1}",
					DateTimeUtility.ToStringForManager(
						DateTime.Now.Date.AddDays(-1 * double.Parse(searchResult.TotalDays)),
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter),
					DateTimeUtility.ToStringForManager(
						DateTime.Now.Date.AddSeconds(-1),
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			}
			// 期間指定
			else
			{
				date = string.Format(
					"{0}～{1}",
					DateTimeUtility.ToStringForManager(
						searchResult.TotalFrom,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter),
					DateTimeUtility.ToStringForManager(
						searchResult.TotalTo,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			}
			return date;
		}

		/// <summary>商品ランキングID</summary>
		public string RankingId { get; private set; }
		/// <summary>集計タイプ</summary>
		public string TotalType { get; private set; }
		/// <summary>集計期間区分</summary>
		public string TotalKbn { get; private set; }
		/// <summary>集計期間</summary>
		public string TotalDate { get; private set; }
		/// <summary>集計日数</summary>
		public string TotalDays { get; private set; }
		/// <summary>カテゴリ指定</summary>
		public string CategoryId { get; private set; }
		/// <summary>カテゴリ除外指定</summary>
		public string ExcludeCategoryIds { get; private set; }
		/// <summary>ブランドID</summary>
		public string BrandId { get; private set; }
		/// <summary>ブランド指定</summary>
		public string BrandKbn { get; private set; }
		/// <summary>在庫切れ商品</summary>
		public string StockKbn { get; private set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; private set; }
		/// <summary>集計タイプが自動か</summary>
		public bool IsTotalTypeAuto
		{
			get
			{
				return (this.TotalType == Constants.FLG_PRODUCTRANKING_TOTAL_TYPE_AUTO)
					|| string.IsNullOrEmpty(this.TotalType);
			}
		}
		/// <summary>ブランド指定ありか</summary>
		public bool IsBrandKbnValid
		{
			get { return (this.BrandKbn == Constants.FLG_PRODUCTRANKING_BRAND_KBN_VALID); }
		}
		/// <summary>集計期間指定か</summary>
		public bool IsTotalKbnPeriod
		{
			get
			{
				return (this.TotalKbn == Constants.FLG_PRODUCTRANKING_TOTAL_KBN_PERIOD)
					|| string.IsNullOrEmpty(this.TotalKbn);
			}
		}
	}
}