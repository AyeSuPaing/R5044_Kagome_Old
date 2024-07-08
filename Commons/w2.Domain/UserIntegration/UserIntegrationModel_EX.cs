/*
=========================================================================================================
  Module      : ユーザー統合情報モデル (UserIntegrationModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.UserIntegration
{
	/// <summary>
	/// ユーザー統合情報モデル
	/// </summary>
	public partial class UserIntegrationModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>ステータスが未確定か</summary>
		public bool IsNoneStatus
		{
			get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_NONE); }
		}
		/// <summary>ステータスが保留か</summary>
		public bool IsSuspendStatus
		{
			get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_SUSPEND); }
		}
		/// <summary>ステータスが確定か</summary>
		public bool IsDoneStatus
		{
			get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_DONE); }
		}
		/// <summary>ステータスが除外か</summary>
		public bool IsExcludedStatus
		{
			get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_EXCLUDED); }
		}
		/// <summary>ユーザリスト</summary>
		public UserIntegrationUserModel[] Users
		{
			get { return (UserIntegrationUserModel[])this.DataSource["EX_Users"]; }
			set { this.DataSource["EX_Users"] = value; }
		}
		#endregion
	}
}
