/*
=========================================================================================================
  Module      : ユーザ拡張項目マスタモデル (UserExtendModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.User.Helper;
using w2.Domain.UserExtendSetting;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザ拡張項目マスタモデル
	/// </summary>
	[Serializable]
	public partial class UserExtendModel : ModelBase<UserExtendModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="userExtendSettingList">ユーザー拡張項目設定リスト</param>
		public UserExtendModel(UserExtendSettingList userExtendSettingList)
		{
			this.UserId = "";
			this.DataSource = new Hashtable();
			this.UserExtendDataValue = new UserExtendData();
			this.UserExtendDataText = new UserExtendData();
			this.UserExtendColumns = new List<string>();
			this.UserExtendSettings = userExtendSettingList;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		/// <param name="userExtendSettingList">ユーザー拡張項目設定リスト</param>
		public UserExtendModel(Hashtable source, UserExtendSettingList userExtendSettingList)
			: this(userExtendSettingList)
		{
			this.DataSource = source;

			SetUserExtendDataValue();
			SetUserExtendDataText();
			SetUserExtendColumns();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		/// <param name="userExtendSettingList">ユーザー拡張項目設定リスト</param>
		public UserExtendModel(DataRowView source, UserExtendSettingList userExtendSettingList)
			: this(source.ToHashtable(), userExtendSettingList)
		{

		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTEND_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USEREXTEND_USER_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USEREXTEND_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USEREXTEND_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USEREXTEND_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USEREXTEND_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTEND_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USEREXTEND_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
