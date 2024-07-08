/*
=========================================================================================================
  Module      : 広告媒体区分ポップアップ 検索用パラメタモデル(AdCodeMediaTypeSearchListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.SearchPopup
{
	/// <summary>
	/// 広告媒体区分ポップアップ 検索用パラメタモデル
	/// </summary>
	[Serializable]
	public class AdCodeMediaTypeSearchListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdCodeMediaTypeSearchListParamModel()
		{
			this.PagerNo = 1;
			this.AdvCodeMediaTypeId = string.Empty;
			this.AdvCodeMediaTypeName = string.Empty;
			this.SortKbn = "2";
			this.UsableMediaTypeIds = string.Empty;
		}

		/// <summary>広告媒体区分ID</summary>
		public string AdvCodeMediaTypeId { get; set; }
		/// <summary>広告媒体区分の名称</summary>
		public string AdvCodeMediaTypeName { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
		/// <summary>閲覧可能な広告媒体区分配列</summary>
		public string[] UsableMediaTypeIdsArray
		{
			get
			{
				var splited = this.UsableMediaTypeIds.Split(',');

				return splited
					.Where(mediaTypeId => (string.IsNullOrEmpty(mediaTypeId) == false))
					.Distinct()
					.ToArray();
			}
		}
		/// <summary>閲覧可能な広告媒体区分</summary>
		[BindAlias("")]
		protected string UsableMediaTypeIds { get; set; }
	}
}