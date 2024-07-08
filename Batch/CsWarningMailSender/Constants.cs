/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.CustomerSupport.Batch.CsWarningMailSender
{
	class Constants : w2.App.Common.Constants
	{
		/// <summary>送信時間（毎日○時）</summary>
		public static int MAIL_SEND_TIME = 0;
		/// <summary>オペレータ：未対応滞留警告</summary>
		public static bool WARNING_NO_ACTION = false;
		/// <summary>オペレータ：未対応滞留警告日数</summary>
		public static int WARNING_NO_ACTION_LIMIT_DAYS = 0;
		/// <summary>管理者：担当未設定滞留警告</summary>
		public static bool WARNING_NO_ASSIGN = false;
		/// <summary>管理者：担当未設定滞留警告日数</summary>
		public static int WARNING_NO_ASSIGN_LIMIT_DAYS = 0;
	}
}
