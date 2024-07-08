/*
=========================================================================================================
  Module      : ユーザー統合履歴情報モデル (UserIntegrationHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserIntegration
{
	/// <summary>
	/// ユーザー統合履歴情報モデル
	/// </summary>
	[Serializable]
	public partial class UserIntegrationHistoryModel : ModelBase<UserIntegrationHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserIntegrationHistoryModel()
		{
			this.BranchNo = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザー統合No</summary>
		public long UserIntegrationNo
		{
			get { return (long)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO] = value; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_BRANCH_NO] = value; }
		}
		/// <summary>テーブル名</summary>
		public string TableName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_TABLE_NAME]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_TABLE_NAME] = value; }
		}
		/// <summary>主キー1</summary>
		public string PrimaryKey1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY1]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY1] = value; }
		}
		/// <summary>主キー2</summary>
		public string PrimaryKey2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY2]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY2] = value; }
		}
		/// <summary>主キー3</summary>
		public string PrimaryKey3
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY3]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY3] = value; }
		}
		/// <summary>主キー4</summary>
		public string PrimaryKey4
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY4]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY4] = value; }
		}
		/// <summary>主キー5</summary>
		public string PrimaryKey5
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY5]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY5] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
