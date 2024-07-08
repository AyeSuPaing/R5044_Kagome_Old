/*
=========================================================================================================
  Module      : ユーザー管理レベルマスタモデル (UserManagementLevelModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserManagementLevel
{
	/// <summary>
	/// ユーザー管理レベルマスタモデル
	/// </summary>
	[Serializable]
	public partial class UserManagementLevelModel : ModelBase<UserManagementLevelModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserManagementLevelModel()
		{
			this.UserManagementLevelId = "";
			this.UserManagementLevelName = "";
			this.DisplayOrder = 1;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserManagementLevelModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserManagementLevelModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>シーケンス番号</summary>
		public long SeqNo
		{
			get { return (long)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_SEQ_NO]; }
		}
		/// <summary>ユーザー管理レベルID</summary>
		public string UserManagementLevelId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID]; }
			set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID] = value; }
		}
		/// <summary>ユーザー管理レベル名</summary>
		public string UserManagementLevelName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME]; }
			set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
