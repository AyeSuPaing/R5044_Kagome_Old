/*
=========================================================================================================
  Module      : ログインID重複ユーザモデル (DuplicationLoginId.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ログインID重複ユーザモデル
	/// </summary>
	[Serializable]
	public partial class DuplicationLoginId : ModelBase<DuplicationLoginId>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DuplicationLoginId()
		{
			this.LoginId = string.Empty;
			this.Count = 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DuplicationLoginId(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DuplicationLoginId(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ログインＩＤ</summary>
		public string LoginId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_LOGIN_ID]; }
			set { this.DataSource[Constants.FIELD_USER_LOGIN_ID] = value; }
		}
		/// <summary>件数</summary>
		public int Count
		{
			get { return (int)this.DataSource["count"]; }
			set { this.DataSource["count"] = value; }
		}
		#endregion
	}
}
