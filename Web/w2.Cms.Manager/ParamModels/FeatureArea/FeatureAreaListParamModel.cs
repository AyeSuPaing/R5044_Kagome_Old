/*
=========================================================================================================
  Module      : 特集エリア一覧パラメタモデル(FeatureAreaListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;

namespace w2.Cms.Manager.ParamModels.FeatureArea
{
	/// <summary>
	/// 特集エリア一覧リストパラメタモデル
	/// </summary>
	[Serializable]
	public class FeatureAreaListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureAreaListParamModel()
		{
			this.FreeWord = string.Empty;
			this.AreaType = string.Empty;
			this.PublicDateKbn = Constants.DateSelectType.Unselected;
			this.UseType = string.Empty;
			this.PagerNo = 1;
		}

		/// <summary>フリーワード</summary>
		public string FreeWord { get; set; }
		/// <summary>タイプ</summary>
		public string AreaType { get; set; }
		/// <summary>公開日区分</summary>
		public Constants.DateSelectType PublicDateKbn { get; set; }
		/// <summary>ページ利用状態</summary>
		public string UseType { get; set; }
		/// <summary>ページ番号</summary>
		public int PagerNo { get; set; }
	}
}
