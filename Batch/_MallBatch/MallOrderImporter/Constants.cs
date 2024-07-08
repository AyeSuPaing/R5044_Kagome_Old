/*
=========================================================================================================
  Module      : 定数クラス(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace w2.Commerce.MallBatch.MallOrderImporter
{
	class Constants : w2.App.Common.Constants
	{
		public const string MASTER_IMPORT_KBN_ADD_YAHOO_ORDER = "AddYahooOrder";
		public const string MASTER_IMPORT_KBN_ADD_YAHOO_CUSTOM_FIELDS = "AddYahooCustomFields";
		public static string PATH_CHECK_SEND_MAIL = Directory.GetCurrentDirectory() + "\\CheckSendMail.txt";
	}
}
