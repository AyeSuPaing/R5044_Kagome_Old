/*
=========================================================================================================
  Module      : ABテストリストパラメタモデル(AbTestListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Cms.Manager.ParamModels.AbTest
{
	/// <summary>
	/// LPリストパラメタモデル
	/// </summary>
	[Serializable]
	public class AbTestListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AbTestListParamModel()
		{
			this.SearchWord = "";
		}

		/// <summary>検索ワード</summary>
		public string SearchWord { get; set; }
	}
}