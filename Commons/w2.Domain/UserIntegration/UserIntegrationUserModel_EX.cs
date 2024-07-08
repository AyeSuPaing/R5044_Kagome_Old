/*
=========================================================================================================
  Module      : ユーザー統合ユーザ情報モデル (UserIntegrationUserModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.UserIntegration
{
	/// <summary>
	/// ユーザー統合ユーザ情報モデル
	/// </summary>
	public partial class UserIntegrationUserModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>代表フラグが「代表である」か</summary>
		public bool IsOnRepresentativeFlg
		{
			get { return (this.RepresentativeFlg == Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_ON); }
		}
		/// <summary>履歴リスト</summary>
		public UserIntegrationHistoryModel[] Histories
		{
			get { return (UserIntegrationHistoryModel[])this.DataSource["EX_Histories"]; }
			set { this.DataSource["EX_Histories"] = value; }
		}
		#endregion
	}
}
