/*
=========================================================================================================
  Module      : 定期ワークフロー設定モデル (FixedPurchaseWorkflowSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FixedPurchaseWorkflowSetting
{
	/// <summary>
	/// 定期ワークフロー設定モデル
	/// </summary>
	public partial class FixedPurchaseWorkflowSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>有効か？</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID); }
		}
		#endregion
	}
}
