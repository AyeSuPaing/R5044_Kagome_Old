/*
=========================================================================================================
  Module      : ショートURL Listビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ShortUrl;
using w2.Common.Util;

namespace w2.Cms.Manager.ViewModels.ShortUrl
{
	/// <summary>
	/// ショートURL Listビューモデル
	/// </summary>
	[Serializable]
	public class ShortUrlListViewModel : ViewModelBase
	{
		#region 列挙体
		/// <summary>
		/// 表示モード
		/// </summary>
		public enum Mode
		{
			/// <summary>参照</summary>
			Display,
			/// <summary>編集</summary>
			Edit,
			/// <summary>結果</summary>
			Result
		}
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pageLayout">ページレイアウト</param>
		public ShortUrlListViewModel(string pageLayout = Constants.LAYOUT_PATH_DEFAULT)
		{
			this.SearchResult = new List<ShortUrlInput>();
			this.SortKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_SHORTURL, "sort")
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text,
					// 並び順の初期値を設定
					Selected = (s.Value == "2")
				}).ToArray();
			this.ExportFiles = MasterExportHelper.CreateExportFilesDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL);
			this.DisplayMode = Mode.Display;
			this.PageLayout = pageLayout;
		}

		public bool IsShowEditButton()
		{
			var result = (this.DisplayMode == Mode.Display) && (this.SearchResult.Count != 0);
			return result;
		}

		/// <summary>パラメタモデル</summary>
		public ShortUrlListParamModel ParamModel { get; set; }
		/// <summary>検索結果</summary>
		public List<ShortUrlInput> SearchResult { get; set; }
		/// <summary>選択肢群 並び順</summary>
		public SelectListItem[] SortKbnItems { get; private set; }
		/// <summary>選択肢群 マスタ出力</summary>
		public SelectListItem[] ExportFiles { get; private set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>表示モード</summary>
		public Mode DisplayMode { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
	}
}