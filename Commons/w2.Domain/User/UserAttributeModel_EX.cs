/*
=========================================================================================================
  Module      : ユーザー属性マスタモデル (UserAttributeModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.User.Helper;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザー属性マスタモデル
	/// </summary>
	public partial class UserAttributeModel
	{
		#region メソッド
		/// <summary>
		/// CPMクラスタ名取得
		/// </summary>
		/// <param name="settings">CPMクラスタ設定</param>
		/// <returns>CPMクラスタ名</returns>
		public string GetCpmClusterName(CpmClusterSettings settings)
		{
			return settings.ClusterNames.ContainsKey(this.CpmClusterId) ? settings.ClusterNames[this.CpmClusterId] : null;
		}
		/// <summary>
		/// 過去のCPMクラスタ名取得
		/// </summary>
		/// <param name="settings">CPMクラスタ設定</param>
		/// <returns>過去のCPMクラスタ名</returns>
		public string GetCpmClusterNameBefore(CpmClusterSettings settings)
		{
			return settings.ClusterNames.ContainsKey(this.CpmClusterIdBefore) ? settings.ClusterNames[this.CpmClusterIdBefore] : null;
		}
		#endregion

		#region プロパティ
		#endregion
	}
}
