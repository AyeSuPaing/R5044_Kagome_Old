/*
=========================================================================================================
  Module      : 定数クラス(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Commerce.Batch.OrderCancelBatch
{
	/// <summary>
	/// 定数
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>注文キャンセルインターバル分</summary>
		public static int ORDER_CANCEL_INTERVAL_MINUTES = 30;

		/// <summary>注文キャンセルを許可しない決済種別</summary>
		public static string[] ORDER_CANCEL_DISALLOW_PAYMENT_KBNS =
			{
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF
			};
	}
}
