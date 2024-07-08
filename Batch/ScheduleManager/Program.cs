/*
=========================================================================================================
  Module      : メインクラス(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using w2.App.Common.Util;

namespace w2.MarketingPlanner.Win.ScheduleManager
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// 実行（Mutexで二重起動防止）
			w2.App.Common.Util.ProcessUtility.ExcecWithProcessMutex(Run);
		}

		/// <summary>
		/// フォーム起動
		/// </summary>
		static void Run()
		{
			Application.Run(new Form1());
		}
	}
}
