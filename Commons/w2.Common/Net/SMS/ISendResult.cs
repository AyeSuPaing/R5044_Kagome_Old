/*
=========================================================================================================
  Module      : SMS送信結果インタフェース(ISendResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Net.SMS
{
	/// <summary>
	/// SMS送信結果インタフェース
	/// </summary>
	public interface ISendResult
	{
		/// <summary>成功したかどうか</summary>
		bool IsSucccess { get; }
		/// <summary>結果</summary>
		string ResultMessage { get; }
		/// <summary>成功したメッセージのID</summary>
		string SuccessMessageID { get; }
	}
}
