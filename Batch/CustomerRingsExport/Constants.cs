/*
=========================================================================================================
  Module      : CustomerRingsExport用定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.Batch.CustomerRingsExport
{
	public class Constants : w2.App.Common.Constants
	{
		// 出力先
		public static string EXPORT_DIRECTORY = "";

		// カスタマーリングスの設定
		public static string CUSTOMER_RINGS_HOST = "";
		public static string CUSTOMER_RINGS_USER_NAME = "";
		public static string CUSTOMER_RINGS_PASSWORD = "";

		// FTPSの設定
		public static bool USE_PASSIVE = true;
		public static bool CHECK_SSL = true;
	}
}
