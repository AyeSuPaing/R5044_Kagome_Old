/*
=========================================================================================================
  Module      : Program (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Util;
using w2.Common.Logger;

namespace w2.Commerce.Batch.TagManagerMigration
{
	/// <summary>
	/// Programクラス
	/// </summary>
	class Program
	{
		/// <summary>
		/// メインスレッド
		/// </summary>
		private static void Main()
		{
			Console.WriteLine("本バッチはアフィリエイトタグデータをV5.13からR5044_Kagome.Developへ移行するためのバッチです。別の用途では利用しないでください。");
			Console.WriteLine("タグマネージャーの移行を実行します。実行する場合はy 実行しない場合はn");
			var input = Console.ReadLine();
			switch (input)
			{
				case "y":
				case "Y":
					Console.WriteLine("実行を開始します。");
					break;

				case "n":
				case "N":
					Console.WriteLine("実行せずに終了します。");
					return;

				default:
					Console.WriteLine("終了します。");
					return;
			}

			try
			{
				var program = new Program();

				FileLogger.WriteInfo("起動");

				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (executeSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				FileLogger.WriteInfo("正常終了");

				Console.WriteLine("タグマネージャーの移行が終了しました。何かキーを押してください。");
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				// Write Log
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private Program()
		{
			// Read Common Setting
			Initialize.ReadCommonConfig();
		}

		/// <summary>
		/// 処理開始
		/// </summary>
		private void Start()
		{
			new Migration().Process();
		}
	}
}