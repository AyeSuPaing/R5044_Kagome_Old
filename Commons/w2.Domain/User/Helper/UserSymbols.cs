/*
=========================================================================================================
  Module      : ユーザーシンボルモデル (UserSymbols.cs)
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
	/// ユーザーシンボルモデル
	/// </summary>
	[Serializable]
	public partial class UserSymbols : ModelBase<UserSymbols>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserSymbols()
		{
			this.UserId = string.Empty;
			this.UserMemo = string.Empty;
			this.OrderCount = 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserSymbols(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserSymbols(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_USER_ID] = value; }
		}
		/// <summary>ユーザメモ</summary>
		public string UserMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_MEMO]; }
			set { this.DataSource[Constants.FIELD_USER_USER_MEMO] = value; }
		}
		/// <summary>注文の件数</summary>
		public int OrderCount
		{
			get { return (int)this.DataSource["order_count"]; }
			set { this.DataSource["order_count"] = value; }
		}
		#endregion
	}
}
