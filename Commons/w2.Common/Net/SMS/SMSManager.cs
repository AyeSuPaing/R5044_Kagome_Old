/*
=========================================================================================================
  Module      : SMS送信を司るクラス(SMSManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Net.SMS.MicroKiosk;

namespace w2.Common.Net.SMS
{
	/// <summary>
	/// SMS送信を司るクラス
	/// </summary>
	public class SMSManager
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SMSManager()
		{
		}

		/// <summary>
		/// SMSメッセージ送信
		/// </summary>
		/// <param name="message">SMS送信するメッセージ</param>
		/// <param name="to">SMS送信先</param>
		/// <param name="from">SMS送信元</param>
		/// <returns>送信結果</returns>
		public ISendResult SendMessage(string message, string to, string from)
		{
			var adp = CreateAdapter();
			adp.SetParams(message, to, from);
			var res = adp.Send();
			return res;
		}

		/// <summary>
		/// アダプタ生成
		/// </summary>
		/// <returns>生成したアダプタ</returns>
		private ISMSAdapter CreateAdapter()
		{
			// SMSの種類によってアダプタを分ける
			if (Constants.GLOBAL_SMS_TYPE == Constants.GLOBAL_SMS_TYPE_MACROKIOSK)
			{
				return new AdapterMicroKiosk();
			}

			// 上記以外は未定義
			throw new Exception(string.Format("{0}は未定義のSMS種別です。", Constants.GLOBAL_SMS_TYPE));
		}
	}
}
