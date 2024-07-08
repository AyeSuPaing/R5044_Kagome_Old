/*
=========================================================================================================
  Module      : SMSステータス更新バッチ(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common;
using w2.Common.Logger;

namespace UpdateSMSState
{
	/// <summary>
	/// SMSステータス更新バッチ
	/// </summary>
	class Program
	{
		/// <summary>
		/// エントリポイント
		/// </summary>
		/// <param name="args">引数（未使用）</param>
		static void Main(string[] args)
		{
			try
			{
				BatchInit();
				FileLogger.WriteInfo("開始");

				var proc = new BatchProc();
				proc.Execute();

				FileLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Concat("異常終了しました。\r\n", ex));
				try
				{
					FileLogger.WriteError(Constants.APPLICATION_NAME + " エラー発生", ex);
					BatchMailer.SendAlertMail(ex);
				}
				catch (Exception ex2)
				{
					Console.WriteLine("エラーログ出力、エラーメール送信に失敗");
					Console.WriteLine(ex2.ToString());
				}
			}
		}

		#region -BatchInit バッチ初期処理
		/// <summary>
		/// バッチ初期処理
		/// </summary>
		static void BatchInit()
		{
			Initializer.Initialize();
		}
		#endregion

	}
}
