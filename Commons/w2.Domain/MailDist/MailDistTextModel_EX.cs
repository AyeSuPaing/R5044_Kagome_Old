/*
=========================================================================================================
  Module      : メール配信文章マスタモデル (MailDistTextModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MailDist
{
	/// <summary>
	/// メール配信文章マスタモデル
	/// </summary>
	public partial class MailDistTextModel
	{
		/// <summary>SMS利用フラグ：利用しない</summary>
		public const string SMS_USE_FLG_OFF = "0";
		/// <summary>SMS利用フラグ：利用する</summary>
		public const string SMS_USE_FLG_ON = "1";

		/// <summary>LINE利用フラグ：利用しない</summary>
		public const string LINE_USE_FLG_OFF = "0";
		/// <summary>LINE利用フラグ：利用する</summary>
		public const string LINE_USE_FLG_ON = "1";

		#region メソッド
		#endregion

		#region プロパティ
		#endregion
	}
}
