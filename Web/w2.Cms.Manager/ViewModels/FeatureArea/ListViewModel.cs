/*
=========================================================================================================
  Module      : 特集エリア一覧ビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.FeatureArea;
using w2.Common.Util;
using w2.Domain.FeatureArea.Helper;

namespace w2.Cms.Manager.ViewModels.FeatureArea
{
	/// <summary>
	/// 特集エリア一覧ビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.List = new ListViewRow[0];
			this.PublicDateKbns = DateTimeUtility.DateTermUpTo3Month().Select(
				item => new SelectListItem()
				{
					Text = item.Text,
					Value = item.Value
				}).ToArray();
			this.UseTypes = ValueText.GetValueItemArray(Constants.VALUE_TEXT_KEY_CMS_COMMON, Constants.VALUE_TEXT_FIELD_USE_TYPE).Select(
				item => new SelectListItem()
				{
					Text = item.Text,
					Value = item.Value
				}).ToArray();
			this.HitCount = 0;
		}
		/// <summary>パラメタモデル</summary>
		public FeatureAreaListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public ListViewRow[] List { get; set; }
		/// <summary>選択肢群 エリアタイプ</summary>
		public SelectListItem[] AreaTypes { get; set; }
		/// <summary>選択肢群 公開日</summary>
		public SelectListItem[] PublicDateKbns { get; set; }
		/// <summary>選択肢群 ページ利用状態</summary>
		public SelectListItem[] UseTypes { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>サムネイルファイル名</summary>
		public string ThumbFileName { get; set; }
		/// <summary>検索ヒット数</summary>
		public int HitCount { get; set; }
	}

	/// <summary>
	/// 特集エリア一覧行ビューモデル
	/// </summary>
	public class ListViewRow : FeatureAreaListSearchResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ListViewRow(Hashtable source)
			: base(source)
		{

		}

		/// <summary>更新日</summary>
		public string DisplayDateChanged
		{
			get
			{
				return DateTimeUtility.ToStringForManager(
					this.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			}
		}
	}

}
