/*
=========================================================================================================
  Module      : LP検索結果ビューモデル(LandingPageSearchResultModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.Cms.Manager.ViewModels.AbTest
{
	/// <summary>
	/// LP検索結果ビューモデル
	/// </summary>
	public class LandingPageSearchResultModel
	{
		/// <summary>
		/// JSONに変換
		/// </summary>
		/// <returns>JSON</returns>
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		/// <summary>ページID</summary>
		public string PageId { get; set; }
		/// <summary>ページタイトル</summary>
		public string PageTitle { get; set; }
		/// <summary>ページURL</summary>
		public string PageUrl { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>公開状態</summary>
		public bool PublicStatus { get; set; }
		/// <summary>総件数HTML</summary>
		public string CountHtml { get; set; }

	}
}