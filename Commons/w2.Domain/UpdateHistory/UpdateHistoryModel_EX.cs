/*
=========================================================================================================
  Module      : 更新履歴情報モデル (UpdateHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.UpdateHistory
{
	/// <summary>
	/// 更新履歴情報モデル
	/// </summary>
	public partial class UpdateHistoryModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// 拡張項目_更新履歴区分表示テキスト
		/// </summary>
		public string UpdateHistoryKbnText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_UPDATEHISTORY, Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN, this.UpdateHistoryKbn);
			}
		}
		#endregion
	}
}