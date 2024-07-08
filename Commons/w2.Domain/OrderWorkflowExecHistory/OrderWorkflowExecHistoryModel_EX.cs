/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴モデル (OrderWorkflowExecHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴モデル
	/// </summary>
	public partial class OrderWorkflowExecHistoryModel
	{
		#region プロパティ
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		#endregion
	}
}
