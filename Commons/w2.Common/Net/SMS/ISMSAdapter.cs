/*
=========================================================================================================
  Module      : SMS送信アダプタインタフェース(ISMSAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Net.SMS
{
	/// <summary>
	/// SMS送信アダプタインタフェース
	/// </summary>
	public interface ISMSAdapter
	{
		/// <summary>
		/// パラメタセット
		/// </summary>
		/// <param name="message">SMS送信するメッセージ</param>
		/// <param name="to">SMS送信先</param>
		/// <param name="from">SMS送信元</param>
		/// <returns>アダプタ</returns>
		ISMSAdapter SetParams(string message, string to, string from);

		/// <summary>
		/// SMS送信実施
		/// </summary>
		/// <returns>送信結果</returns>
		ISendResult Send();
	}
}
