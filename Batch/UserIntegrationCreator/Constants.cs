/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.UserIntegrationCreator
{
	/// <summary>
	/// 定数定義
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		/// <summary>並列処理ワーカースレッド数</summary>
		public static int PARALLEL_WORKERTHREADS = 1;
		/// <summary>最終実行ファイル名プリフィックス</summary>
		public const string FILENAME_LASTEXEC_PREFIX = "_LastExec";
		/// <summary>最終実行ファイル名サフィックス日付フォーマット</summary>
		public const string FILENAME_LASTEXEC_SUFFIX_DATEFORMAT = "yyyyMMddHHmmss";
	}
}
