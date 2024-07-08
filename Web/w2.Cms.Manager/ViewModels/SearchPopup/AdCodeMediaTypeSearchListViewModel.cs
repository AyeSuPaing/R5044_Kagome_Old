/*
=========================================================================================================
  Module      : 広告媒体区分ポップアップ リストViewモデル(ProductTagManagerListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.SearchPopup;
using w2.Common.Util;
using w2.Domain.AdvCode.Helper;

namespace w2.Cms.Manager.ViewModels.SearchPopup
{
	/// <summary>
	/// 広告媒体区分ポップアップ リストViewモデル
	/// </summary>
	public class AdCodeMediaTypeSearchListViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdCodeMediaTypeSearchListViewModel()
		{
			this.ParamModel = new AdCodeMediaTypeSearchListParamModel();
			this.AdCodeMediaTypeSearchResultListViewModel = new AdCodeMediaTypeSearchResultListViewModel[0];
			this.SortKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_ADVCODE, Constants.REQUEST_KEY_SORT_KBN)
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text
				}).ToArray();
		}

		/// <summary>検索 パラメタモデル</summary>
		public AdCodeMediaTypeSearchListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public AdCodeMediaTypeSearchResultListViewModel[] AdCodeMediaTypeSearchResultListViewModel { get; set; }
		/// <summary>並び順ドロップダウンリスト</summary>
		public SelectListItem[] SortKbnItems { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// 広告媒体区分ポップアップ検索結果 Viewモデル
	/// </summary>
	public class AdCodeMediaTypeSearchResultListViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">広告媒体検索結果</param>
		public AdCodeMediaTypeSearchResultListViewModel(AdvCodeMediaTypeListSearchResult result)
		{
			this.AdvcodeMediaTypeId = result.AdvcodeMediaTypeId;
			this.AdvcodeMediaTypeName = result.AdvcodeMediaTypeName;
		}

		/// <summary>広告媒体区分ID</summary>
		public string AdvcodeMediaTypeId { get; set; }
		/// <summary>広告媒体区分の名称</summary>
		public string AdvcodeMediaTypeName { get; set; }
	}
}