/*
=========================================================================================================
  Module      : ユーザー統合情報モデル (UserIntegrationModel.cs)
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
	/// ユーザー統合情報モデル
	/// </summary>
	[Serializable]
	public partial class UserIntegrationModel : ModelBase<UserIntegrationModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserIntegrationModel()
		{
			this.Status = Constants.FLG_USERINTEGRATION_STATUS_NONE;
			this.Users = new UserIntegrationUserModel[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザー統合No</summary>
		public long UserIntegrationNo
		{
			get { return (long)this.DataSource[Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_STATUS]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATION_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATION_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
