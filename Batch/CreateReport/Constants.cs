/*
=========================================================================================================
  Module      : 共通定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace w2.MarketingPlanner.Batch.CreateReport
{
	class Constants : w2.App.Common.Constants
	{
		/// <summary>バッチの最終実行日を保持するファイル</summary>
		public static string FILE_LAST_EXEC_DATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LastDate.Properties");
		/// <summary>日付けフォーマット</summary>
		public static string FORMAT_DATE = "yyyy/MM/dd";
	}
}
