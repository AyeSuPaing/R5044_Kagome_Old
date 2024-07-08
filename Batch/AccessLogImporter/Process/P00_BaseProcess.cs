/*
=========================================================================================================
  Module      : 基底プロセスクラス(P00_BaseProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Net.Mail;

namespace w2.MarketingPlanner.Batch.AccessLogImporter.Process
{
	abstract class P00_BaseProcess
	{
		/// <summary>プロセス実行結果列挙体</summary>
		public enum PROCESS_RESULT
		{
			SUCCESS,
			FAILED_A_PART,
			FAILED,
			ALERT,
			NONE
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public P00_BaseProcess()
		{
			this.ProcessResult = PROCESS_RESULT.NONE;
			this.ProcessException = null;
		}

		/// <summary>プロセス実行結果</summary>
		public PROCESS_RESULT ProcessResult { get; protected set; }

		/// <summary>プロセス実行例外</summary>
		public Exception ProcessException { get; protected set; }

		/// <summary>
		/// プロセス実行結果文字列を取得する
		/// </summary>
		/// <returns>プロセス実行結果文字列</returns>
		public string GetProcessResultString()
		{
			return GetProcessResultString(this.ProcessResult);
		}
		/// <summary>
		/// プロセス結果よりプロセス実行結果文字列を取得する
		/// </summary>
		/// <param name="prProcessResult">プロセス結果</param>
		/// <returns>プロセス実行結果文字列</returns>
		public static string GetProcessResultString(PROCESS_RESULT prProcessResult)
		{
			switch (prProcessResult)
			{
				case PROCESS_RESULT.SUCCESS:

					return "成功";

				case PROCESS_RESULT.FAILED_A_PART:

					return "一部成功";

				case PROCESS_RESULT.FAILED:

					return "失敗";

				case PROCESS_RESULT.ALERT:

					return "警告";

				case PROCESS_RESULT.NONE:

					return "未処理";
			}

			return "";
		}
	}
}
