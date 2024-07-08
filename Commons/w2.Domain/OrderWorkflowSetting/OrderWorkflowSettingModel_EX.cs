/*
=========================================================================================================
  Module      : 注文ワークフロー設定モデル (OrderWorkflowSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.Domain.OrderWorkflowSetting
{
	/// <summary>
	/// 注文ワークフロー設定モデル
	/// </summary>
	public partial class OrderWorkflowSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>有効か？</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID); }
		}
		/// <summary>Workflow type</summary>
		public string WorkflowType
		{
			get { return StringUtility.ToEmpty(this.DataSource["workflow_type"]); }
			set { this.DataSource["workflow_type"] = value; }
		}
		#endregion
	}
}
