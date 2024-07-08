/*
=========================================================================================================
  Module      : 特集エリアタイプ一覧ビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.ParamModels.FeatureAreaType;
using w2.Domain.FeatureArea.Helper;

namespace w2.Cms.Manager.ViewModels.FeatureAreaType
{
	/// <summary>
	/// 特集エリアタイプ一覧ビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.List = new FeatureAreaTypeListSearchResultViewModel[0];
			this.DateChanged = DateTimeUtility.DateTermUpTo3Month().Select(
				item => new SelectListItem()
				{
					Text = item.Text,
					Value = item.Value
				}).ToArray();
		}

		/// <summary>パラメタモデル</summary>
		public FeatureAreaTypeListParamModel ParamModel { get; set; }
		/// <summary>選択肢群 更新日</summary>
		public SelectListItem[] DateChanged { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public FeatureAreaTypeListSearchResultViewModel[] List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// 特集エリアタイプ検索結果表示クラス
	/// </summary>
	public class FeatureAreaTypeListSearchResultViewModel : FeatureAreaTypeListSearchResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaTypeListSearchResultViewModel(Hashtable source)
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