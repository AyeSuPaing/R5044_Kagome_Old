/*
=========================================================================================================
  Module      : 実行結果クラス(ActionResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.LiaiseAmazonMall.Amazon
{
	/// <summary>
	/// 実行結果クラス
	/// </summary>
	public class ActionResult
	{
		/// <summary>処理件数</summary>
		public int ExecuteCount { get; set; }
		/// <summary>成功件数</summary>
		public int SuccessCount { get; set; }
		/// <summary>エラー件数</summary>
		public int ErrorCount { get; set; }
	}
}
