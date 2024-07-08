/*
=========================================================================================================
  Module      : 会員ランク更新履歴モデル (UserMemberRankHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserMemberRankHistory
{
	/// <summary>
	/// 会員ランク更新履歴モデル
	/// </summary>
	[Serializable]
	public partial class UserMemberRankHistoryModel : ModelBase<UserMemberRankHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserMemberRankHistoryModel()
		{
			this.UserId = null;
			this.BeforeRankId = null;
			this.AfterRankId = null;
			this.MailId = null;
			this.ChangedBy = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserMemberRankHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserMemberRankHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>履歴No</summary>
		public int HistoryNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_HISTORY_NO]; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID] = value; }
		}
		/// <summary>更新前ランクID</summary>
		public string BeforeRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_BEFORE_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_BEFORE_RANK_ID] = value; }
		}
		/// <summary>更新後ランクID</summary>
		public string AfterRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_AFTER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_AFTER_RANK_ID] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_MAIL_ID] = value; }
		}
		/// <summary>変更者</summary>
		public string ChangedBy
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_CHANGED_BY]; }
			set { this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_CHANGED_BY] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED] = value; }
		}
		#endregion
	}
}