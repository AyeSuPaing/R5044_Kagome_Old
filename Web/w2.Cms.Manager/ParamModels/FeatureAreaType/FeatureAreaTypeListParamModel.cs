/*
=========================================================================================================
  Module      : 特集エリアタイプ一覧パラメタモデル(FeatureAreaTypeListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;

namespace w2.Cms.Manager.ParamModels.FeatureAreaType
{
	/// <summary>
	/// 特集エリアタイプ一覧リストパラメタモデル
	/// </summary>
	[Serializable]
	public class FeatureAreaTypeListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureAreaTypeListParamModel()
		{
			this.FreeWord = string.Empty;
			this.DateChanged = Constants.DateSelectType.Unselected;
		}

		/// <summary>フリーワード</summary>
		public string FreeWord { get; set; }
		/// <summary>更新日</summary>
		public Constants.DateSelectType DateChanged { get; set; }
	}
}