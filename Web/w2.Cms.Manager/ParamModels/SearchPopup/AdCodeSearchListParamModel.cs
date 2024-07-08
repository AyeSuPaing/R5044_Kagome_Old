/*
=========================================================================================================
  Module      : 広告コード 検索用パラメタモデル(AdCodeSearchListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.SearchPopup
{
	/// <summary>
	/// 広告コード 検索用パラメタモデル
	/// </summary>
	[Serializable]
	public class AdCodeSearchListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdCodeSearchListParamModel()
		{
			this.PagerNo = 1;
			this.AdvCodeMediaType = string.Empty;
			this.AdvCode = string.Empty;
			this.MediaName = string.Empty;
			this.SortKbn = "2";
		}

		/// <summary>広告媒体区分ID</summary>
		public string AdvCodeMediaType { get; set; }
		/// <summary>広告コード</summary>
		public string AdvCode { get; set; }
		/// <summary>広告媒体名称</summary>
		public string MediaName { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}