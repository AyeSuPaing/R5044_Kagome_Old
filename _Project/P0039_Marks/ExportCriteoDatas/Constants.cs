/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Specialized;

namespace w2.Commerce.Batch.ExportCriteoDatas
{
	/// <summary>
	/// Criteo連携ファイル出力用定数
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		//========================================================================
		// ディレクトリ・パス系
		//========================================================================
		public static string FILE_NAME = "";
		public static string PHYSICALDIRPATH_UPLOAD_FILE = "";

		//========================================================================
		// メール設定
		//========================================================================
		public static string CRITEO_MAIL_FROM = "";
		public static string CRITEO_MAIL_TITLE = "";
		public static StringCollection CRITEO_MAIL_TO = null;
		public static StringCollection CRITEO_MAIL_CC = null;
		public static StringCollection CRITEO_MAIL_BCC = null;		
	}
}
