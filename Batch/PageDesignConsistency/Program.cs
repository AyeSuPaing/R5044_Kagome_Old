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

namespace w2.Commerce.Batch.PageDesignConsistency
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
			try
			{
				var program = new Program();
				AppLogger.WriteInfo("起動");
				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (executeSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private Program()
		{
			Console.WriteLine("設定ファイル読み込み開始");
			Initialize.ReadCommonConfig();
		}

		/// <summary>
		/// 処理開始
		/// </summary>
		private void Start()
		{
			var pageDesign = new PageDesign();
			FileLogger.WriteInfo("ページ管理 ページ調整開始");
			Console.WriteLine("ページ管理 ページ調整開始");
			pageDesign.PageConsistency();
			FileLogger.WriteInfo("ページ管理 ページグループ調整開始");
			Console.WriteLine("ページ管理 ページグループ調整開始");
			pageDesign.GroupConsistency();

			var partsDesign = new PartsDesign();
			FileLogger.WriteInfo("パーツ管理 パーツ調整開始");
			Console.WriteLine("パーツ管理 パーツ調整開始");
			partsDesign.PartsConsistency();
			FileLogger.WriteInfo("パーツ管理 パーツグループ調整開始");
			Console.WriteLine("パーツ管理 パーツグループ調整開始");
			partsDesign.GroupConsistency();
		}
	}
}