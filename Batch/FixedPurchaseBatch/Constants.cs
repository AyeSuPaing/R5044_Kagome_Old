/*
=========================================================================================================
  Module      : FixedPurchaseBatch固有の定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.App.Common.Order.FixedPurchase;

namespace w2.Commerce.Batch.FixedPurchaseBatch
{
	/// <summary>
	/// FixedPurchaseBatch固有の定数定義
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		/// <summary>ユーザ向けメール送信可否</summary>
		public static bool SEND_USER_MAIL_FLG = false;
		/// <summary>定期購入メールの送信タイミング管理設定</summary>
		public static string FIXED_PURCHASE_MAIL_SEND_TIMING = string.Empty;
		/// <summary>送信するメールのテンプレートID</summary>
		public static string SEND_CHANGE_DEADLINE_MAIL_TEMPLATE_ID = "00000131";
		/// <summary>定期便変更案内メール送信日（配送キャンセル期限の何日前に送信するか）</summary>
		public static int NEXT_FIXED_PURCHASE_CHANGE_DEADLINE_SEND_DATE = 5;
		/// <summary>定期便変更案内メール送信バッチの最終実行日を保持するファイル</summary>
		public static string FILE_LAST_SEND_DATE_FOR_CHANGE_DEADLINE_MAIL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LastDate.Properties");
		/// <summary>日付けフォーマット</summary>
		public static string FORMAT_DATE = "yyyy/MM/dd";
	}
}
