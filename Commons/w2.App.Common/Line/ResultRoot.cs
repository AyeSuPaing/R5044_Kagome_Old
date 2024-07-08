/*
=========================================================================================================
  Module      : LINE API API実行結果取得ルートモデル(ResultRoot.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.Line
{
	/// <summary>
	/// API実行結果取得ルートモデル
	/// </summary>
	[Serializable]
	public class ResultRoot<TData>
	{
		/// <summary>取得内容</summary>
		[JsonProperty("data")]
		public TData Data { get; set; }
	}
}
