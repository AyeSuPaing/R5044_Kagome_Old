/*
=========================================================================================================
  Module      : FLAPS API 基底モデル(ResultBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Net;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// 基底モデル
	/// </summary>
	public class ResultBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected ResultBase()
		{
			this.StatusCode = new HttpStatusCode();
			this.ErrorMessage = "";
		}
		/// <summary>ステータスコード</summary>
		public HttpStatusCode StatusCode { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}
