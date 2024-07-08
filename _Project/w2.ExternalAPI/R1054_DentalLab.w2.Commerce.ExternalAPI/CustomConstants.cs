/*
=========================================================================================================
  Module      : DentalLab カスタマイズ定数定義(CustomConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace R1054_DentalLab.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// カスタマイズ定数定義
	/// </summary>
	internal static class CustomConstants
	{
		// メールテンプレート
		public const string IMPORT_MAILTEMPLATE_KEY_FILE_TYPE = "filetype";
		public const string IMPORT_MAILTEMPLATE_KEY_FILE_NAME = "file_name";
		public const string IMPORT_MAILTEMPLATE_KEY_RESULT = "result";
		public const string IMPORT_MAILTEMPLATE_KEY_MSG = "message";
		public const string IMPORT_MAILTEMPLATE_FILE_TYPE = "出荷確定情報";
		public const string IMPORT_MAILTEMPLATE_RESULT_SUCCSESS = "成功";
		public const string IMPORT_MAILTEMPLATE_RESULT_SUCCSESS_MSG = "ファイルの取込が完了しました。（@@ 1 @@件/全@@ 2 @@件）";
		public const string IMPORT_MAILTEMPLATE_RESULT_FAIL = "失敗";
		public const string IMPORT_MAILTEMPLATE_FILE_FORMAT_ERRMSG = "ファイルフォーマット不正のエラーが発生しました。";
	}
}
