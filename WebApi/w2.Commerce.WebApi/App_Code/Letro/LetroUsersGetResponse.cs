/*
=========================================================================================================
  Module      : Letro Users Get Response (LetroUsersGetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Data;
using w2.Common.Util;

namespace Letro
{
	/// <summary>
	/// Letro users get response
	/// </summary>
	[Serializable]
	public class LetroUsersGetResponse
	{
		/// <summary>顧客情報</summary>
		[JsonProperty("users")]
		public UserDetail[] Users { get; set; }
	}

	/// <summary>
	/// 顧客情報
	/// </summary>
	[Serializable]
	public class UserDetail
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data</param>
		public UserDetail(DataRowView data)
		{
			this.UserId = StringUtility.ToEmpty(data[Constants.FIELD_USER_USER_ID]);
			this.Name1 = StringUtility.ToEmpty(data[Constants.FIELD_USER_NAME1]);
			this.Name2 = StringUtility.ToEmpty(data[Constants.FIELD_USER_NAME2]);
			this.MailAddr = StringUtility.ToEmpty(data[Constants.FIELD_USER_MAIL_ADDR]);
			this.MailFlg = StringUtility.ToEmpty(data[Constants.FIELD_USER_MAIL_FLG]);
			this.DateCreated = StringUtility.ToDateFormat(data[Constants.FIELD_USER_DATE_CREATED]);
		}

		/// <summary>ユーザーID</summary>
		[JsonProperty(Constants.FIELD_USER_USER_ID)]
		public string UserId { get; set; }
		/// <summary>氏名1</summary>
		[JsonProperty(Constants.FIELD_USER_NAME1)]
		public string Name1 { get; set; }
		/// <summary>氏名2</summary>
		[JsonProperty(Constants.FIELD_USER_NAME2)]
		public string Name2 { get; set; }
		/// <summary>メールアドレス</summary>
		[JsonProperty(Constants.FIELD_USER_MAIL_ADDR)]
		public string MailAddr { get; set; }
		/// <summary>メール配信フラグ</summary>
		[JsonProperty(Constants.FIELD_USER_MAIL_FLG)]
		public string MailFlg { get; set; }
		/// <summary>作成日</summary>
		[JsonProperty(Constants.FIELD_USER_DATE_CREATED)]
		public string DateCreated { get; set; }
	}
}
