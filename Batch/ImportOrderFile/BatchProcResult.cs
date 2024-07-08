/*
=========================================================================================================
  Module      : バッチ処理結果クラス(BatchProcResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.ImportOrderFile
{
	public class BatchProcResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private BatchProcResult() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">結果</param>
		/// <param name="resultMessage">結果メッセージ</param>
		/// <param name="resultSuccessCount">結果処理成功件数</param>
		public BatchProcResult(bool result, string resultMessage, int resultSuccessCount = 0)
			: this()
		{
			this.Result = result;
			this.ResultMessage = resultMessage;
			this.ResultSuccessCount = resultSuccessCount;
		}

		/// <summary>結果</summary>
		public bool Result { get; private set; }
		/// <summary>結果メッセージ</summary>
		public string ResultMessage { get; private set; }
		/// <summary>結果処理成功件数</summary>
		public int ResultSuccessCount { get; private set; }
	}
}
