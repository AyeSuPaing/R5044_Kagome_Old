/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Specialized;

namespace MilkyStop
{
	/// <summary>
	/// ミルキー停止処理用定数
	/// </summary>
	class Constants: w2.App.Common.Constants
	{
		// メール設定
		public new static string MAIL_FROM = "";
		public static string MAIL_TITLE = "";
		public static StringCollection MAIL_TO = null;
		public static StringCollection MAIL_CC = null;
		public static StringCollection MAIL_BCC = null;
	}
}
