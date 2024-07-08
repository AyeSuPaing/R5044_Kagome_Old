/*
=========================================================================================================
  Module      : パスワードリマインダーモデル (PasswordReminderModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.User
{
	/// <summary>
	/// パスワードリマインダーモデル
	/// </summary>
	[Serializable]
	public partial class PasswordReminderModel : ModelBase<PasswordReminderModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PasswordReminderModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PasswordReminderModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PasswordReminderModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_PASSWORDREMINDER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_USER_ID] = value; }
		}
		/// <summary>認証キー</summary>
		public string AuthenticationKey
		{
			get { return (string)this.DataSource[Constants.FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY]; }
			set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY] = value; }
		}
		/// <summary>変更試行回数制限</summary>
		public int ChangeTrialLimitCount
		{
			get { return (int)this.DataSource[Constants.FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT]; }
			set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PASSWORDREMINDER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_DATE_CREATED] = value; }
		}
		#endregion
	}
}
