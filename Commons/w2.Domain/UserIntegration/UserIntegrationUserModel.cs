/*
=========================================================================================================
  Module      : ユーザー統合ユーザ情報モデル (UserIntegrationUserModel.cs)
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
	/// ユーザー統合ユーザ情報モデル
	/// </summary>
	[Serializable]
	public partial class UserIntegrationUserModel : ModelBase<UserIntegrationUserModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserIntegrationUserModel()
		{
			this.RepresentativeFlg = Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_OFF;
			this.Histories = new UserIntegrationHistoryModel[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationUserModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationUserModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザー統合No</summary>
		public long UserIntegrationNo
		{
			get { return (long)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO] = value; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_ID] = value; }
		}
		/// <summary>代表フラグ</summary>
		public string RepresentativeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_REPRESENTATIVE_FLG]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_REPRESENTATIVE_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
