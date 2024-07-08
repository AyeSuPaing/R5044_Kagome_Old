/*
=========================================================================================================
  Module      : LINE API API実行結果取得基底モデル(ResultBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;

namespace w2.App.Common.Line
{
	/// <summary>
	/// API実行結果取得基底モデル
	/// </summary>
	[Serializable]
	public class ResultBase
	{
		/// <summary>ステータスコード</summary>
		public HttpStatusCode StatusCode { get; set; }
		/// <summary>実行結果</summary>
		public bool IsSuccess
		{
			get { return (this.StatusCode == HttpStatusCode.OK); }
		}
	}
}
