/*
=========================================================================================================
  Module      : 特集画像リストパラメタモデル(FeatureImageListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;

namespace w2.Cms.Manager.ParamModels.FeatureImage
{
	/// <summary>
	/// 特集画像リストパラメタモデル
	/// </summary>
	public class FeatureImageListSearchParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureImageListSearchParamModel()
		{
			this.Keyword = string.Empty;
			this.GroupId = string.Empty;
			this.DateCreatedKbn = Constants.DateSelectType.Unselected;
		}

		/// <summary>キーワード</summary>
		public string Keyword { get; set; }
		/// <summary>グループID</summary>
		public string GroupId { get; set; }
		/// <summary>作成日</summary>
		public Constants.DateSelectType DateCreatedKbn { get; set; }
	}
}