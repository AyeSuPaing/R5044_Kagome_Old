/*
=========================================================================================================
  Module      : SMSステータスモデル (GlobalSMSStatusModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.GlobalSMS
{
	/// <summary>
	/// SMSステータスモデル
	/// </summary>
	public partial class GlobalSMSStatusModel
	{
		/// <summary>SMSステータス：なし</summary>
		public const string SMS_STATUS_NONE = "";
		/// <summary>SMSステータス：要求済み</summary>
		public const string SMS_STATUS_REQUEST = "REQUEST";
		/// <summary>SMSステータス：送信済み</summary>
		public const string SMS_STATUS_DELIVERED = "DELIVERED";

		#region メソッド
		#endregion

		#region プロパティ
		#endregion
	}
}
