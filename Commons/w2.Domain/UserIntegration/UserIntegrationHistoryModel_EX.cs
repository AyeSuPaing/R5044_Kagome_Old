/*
=========================================================================================================
  Module      : ユーザー統合履歴情報モデル (UserIntegrationHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;
using w2.Domain.Point;

namespace w2.Domain.UserIntegration
{
	/// <summary>
	/// ユーザー統合履歴情報モデル
	/// </summary>
	public partial class UserIntegrationHistoryModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// 拡張項目_テーブル名
		/// </summary>
		public string TableNameText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_USERINTEGRATIONHISTORY, Constants.FIELD_USERINTEGRATIONHISTORY_TABLE_NAME, this.TableName);
			}
		}
		#endregion
	}
}
