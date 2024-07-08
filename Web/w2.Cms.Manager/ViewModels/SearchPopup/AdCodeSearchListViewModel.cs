/*
=========================================================================================================
  Module      : 広告コードポップアップ リストViewモデル(AdCodeSearchListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.SearchPopup;
using w2.Common.Util;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;

namespace w2.Cms.Manager.ViewModels.SearchPopup
{
	/// <summary>
	/// 広告コードポップアップ リストViewモデル
	/// </summary>
	public class AdCodeSearchListViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdCodeSearchListViewModel()
		{
			this.ParamModel = new AdCodeSearchListParamModel();
			this.AdCodeSearchSearchResultListViewModel = new AdCodeSearchSearchResultListViewModel[0];
			this.SortKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_ADVCODE, Constants.REQUEST_KEY_SORT_KBN)
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text
				}).ToArray();
			this.AdvCodeMediaTypeItems = new AdvCodeService().GetAdvCodeMediaTypeListAll()
				.Select(s => new SelectListItem
				{
					Value = s.AdvcodeMediaTypeId,
					Text = string.Format(
						"{0}(ID:{1})",
						s.AdvcodeMediaTypeName,
						s.AdvcodeMediaTypeId)
				}).ToArray();
		}

		/// <summary>検索 パラメタモデル</summary>
		public AdCodeSearchListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public AdCodeSearchSearchResultListViewModel[] AdCodeSearchSearchResultListViewModel { get; set; }
		/// <summary>並び順ドロップダウンリスト</summary>
		public SelectListItem[] SortKbnItems { get; set; }
		/// <summary>広告媒体区分ドロップダウンリスト</summary>
		public SelectListItem[] AdvCodeMediaTypeItems { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// 広告コードポップアップ検索結果 Viewモデル
	/// </summary>
	public class AdCodeSearchSearchResultListViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">広告コード検索結果</param>
		public AdCodeSearchSearchResultListViewModel(AdvCodeListSearchResult result)
		{
			this.AdvcodeNo = result.AdvcodeNo.ToString();
			this.AdvcodeMediaTypeName = result.AdvcodeMediaTypeName;
			this.AdvcodeMediaTypeNameShort = StringUtility.AbbreviateString(this.AdvcodeMediaTypeName, 10);
			this.MediaName = result.MediaName;
			this.MediaNameShort = StringUtility.AbbreviateString(this.MediaName, 30);
			this.AdvertisementCode = result.AdvertisementCode;
			this.MediaCost = result.MediaCost.ToPriceString(true);
			this.AdvertisementDate = DateTimeUtility.ToStringForManager(
				result.AdvertisementDate,
				DateTimeUtility.FormatType.ShortDate2Letter);
			this.PublicationDate = GetPublicationDateString(result.PublicationDateFrom, result.PublicationDateTo);
			this.ValidFlg = ValueText.GetValueText(
				Constants.TABLE_ADVCODE,
				Constants.FIELD_ADVCODE_VALID_FLG,
				result.ValidFlg);
		}

		/// <summary>
		/// 媒体掲載期間取得
		/// </summary>
		/// <param name="dateFrom">開始日</param>
		/// <param name="dateTo">終了日</param>
		/// <returns>媒体掲載期間</returns>
		private string GetPublicationDateString(DateTime? dateFrom, DateTime? dateTo)
		{
			if (dateFrom.HasValue || dateTo.HasValue)
			{
				var result = string.Format(
					"{0}～{1}",
					DateTimeUtility.ToStringForManager(dateFrom, DateTimeUtility.FormatType.ShortDate2Letter),
					DateTimeUtility.ToStringForManager(dateTo, DateTimeUtility.FormatType.ShortDate2Letter));
				return result;
			}

			return string.Empty;
		}

		/// <summary>広告コードID</summary>
		public string AdvcodeNo { get; set; }
		/// <summary>広告媒体区分 名称</summary>
		public string AdvcodeMediaTypeName { get; set; }
		/// <summary>広告媒体区分 名称 省略</summary>
		public string AdvcodeMediaTypeNameShort { get; set; }
		/// <summary>メディア名</summary>
		public string MediaName { get; set; }
		/// <summary>メディア名 省略</summary>
		public string MediaNameShort { get; set; }
		/// <summary>広告費用</summary>
		public string MediaCost { get; set; }
		/// <summary>広告コード</summary>
		public string AdvertisementCode { get; set; }
		/// <summary>出稿日</summary>
		public string AdvertisementDate { get; set; }
		/// <summary>媒体掲載期間</summary>
		public string PublicationDate { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
	}
}