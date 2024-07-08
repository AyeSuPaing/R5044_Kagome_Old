/*
=========================================================================================================
  Module      : 定数(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Net.Mail;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>POPアカウントの最大数</summary>
		public static int POP_ACCOUNT_MAX_COUNT = 10;
		/// <summary>POPアカウントの設定リスト</summary>
		public static List<Pop3Client.PopAccountSetting> POP_ACCOUNT_LIST;
		/// <summary>Gmail Account List</summary>
		public static List<GmailAccountSetting> GMAIL_ACCOUNT_LIST;
		/// <summary>Gmail Account Max Count</summary>
		public static int GMAIL_ACCOUNT_MAX_COUNT = 10;
		/// <summary>Exchange account list</summary>
		public static List<ExchangeAccountSetting> EXCHANGE_ACCOUNT_LIST;
		/// <summary>Exchange account max count</summary>
		public static int EXCHANGE_ACCOUNT_MAX_COUNT = 10;
	}
}
