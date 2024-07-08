/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.Batch.ExternalFileImport
{
	class Constants : w2.App.Common.Constants
	{
		public static string FILE_XML_PARAMDEFINE = @"Settings\ParamDefineSetting.xml";

		//=========================================================================================
		// 完了ファイル付加コード（例：成功ファイル・・・200512315959_S_00001.csv）
		//=========================================================================================
		public const string IMPORT_RESULT_KBN_SUCCESS = "_S_";			// 成功
		public const string IMPORT_RESULT_KBN_FAILED = "_F_";			// 失敗

		//=========================================================================================
		// メールテンプレート
		//=========================================================================================
		public const string IMPORT_MAILTEMPLATE_KEY_FILE_TYPE = "filetype";
		public const string IMPORT_MAILTEMPLATE_KEY_FILE_NAME = "file_name";		// "001.csv"など
		public const string IMPORT_MAILTEMPLATE_KEY_RESULT = "result";			// "成功" or "失敗"
		public const string IMPORT_MAILTEMPLATE_MSG = "message";		// エラーメッセージ

		public static string TAX_ROUNDTYPE = null;

		public const string PAYMENT_METHOD_PAYPAY = "M77";

		/// <summary>Mall default shipping id</summary>
		public const string MALL_DEFAULT_SHIPPING_ID = "101";
		/// <summary>Encoding default: Shift_JIS</summary>
		public const string CONST_ENCODING_DEFAULT = "Shift_JIS";
	}
}
