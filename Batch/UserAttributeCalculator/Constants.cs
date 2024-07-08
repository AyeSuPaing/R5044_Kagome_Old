/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Commerce.Batch.UserAttributeCalculator
{
	/// <summary>
	/// 定数定義
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		/// <summary>コマンド指定ファイル名</summary>
		public const string FILENAME_CALCULATE_ALL_COMMANDS = "CalculateAllCommands.txt";
		/// <summary>最終実行ファイル名プリフィックス</summary>
		public const string FILENAME_LASTEXEC_PREFIX = "_LastExec";
		/// <summary>最終実行ファイル名サフィックス日付フォーマット</summary>
		public const string FILENAME_LASTEXEC_SUFFIX_DATEFORMAT = "yyyyMMdd";
	}
}
