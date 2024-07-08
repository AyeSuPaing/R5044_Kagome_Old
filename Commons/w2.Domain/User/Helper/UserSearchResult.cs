/*
=========================================================================================================
  Module      : ユーザーの検索結果クラス (UserSearchResult.cs)
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
	/// ユーザーの検索結果クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class UserSearchResult : ModelBase<UserSearchResult>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private UserSearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserSearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>行番号</summary>
		public int RowNum
		{
			get { return (int)this.DataSource["row_num"]; }
		}

		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_ID]; }
		}

		/// <summary>モールID</summary>
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MALL_ID]; }
		}

		/// <summary>モール名</summary>
		public string MallName
		{
			get
			{
				if (this.DataSource.Contains(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME)
					&& this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME] != DBNull.Value)
				{
					return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME];
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>ユーザー区分</summary>
		public string UserKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_KBN]; }
		}

		/// <summary>氏名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
		}

		/// <summary>ユーザー管理レベルID</summary>
		public string UserManagermentLevelId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]; }
		}

		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CREATED]; }
		}

		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CHANGED]; }
		}

		/// <summary>ユーザーシンボル</summary>
		public string Symbol { get; set; }
		#endregion
	}
}
