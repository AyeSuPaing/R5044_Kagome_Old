/*
=========================================================================================================
  Module      : 実行結果クラス(ActionResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.LiaiseFacebookMall.Actions
{
	/// <summary>
	/// 実行結果クラス
	/// </summary>
	public class ActionResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ActionResult()
		{
			this.RequestSuccessCount = 0;
			this.RequestErrorCount = 0;
			this.RequestErrorMessage = string.Empty;
		}

		/// <summary>Request success count</summary>
		public int RequestSuccessCount { get; set; }
		/// <summary>Request error count</summary>
		public int RequestErrorCount { get; set; }
		/// <summary>Request error message</summary>
		public string RequestErrorMessage { get; set; }
	}
}