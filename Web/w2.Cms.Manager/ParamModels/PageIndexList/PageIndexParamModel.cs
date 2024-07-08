/*
=========================================================================================================
  Module      : 機能一覧パラメタモデル(PageIndexParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Cms.Manager.ParamModels.PageIndexList
{
	/// <summary>
	/// 機能一覧パラメタモデル
	/// </summary>
	[Serializable]
	public class PageIndexParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageIndexParamModel()
		{
		}

		/// <summary>キー</summary>
		public string Key { get; set; }
	}
}