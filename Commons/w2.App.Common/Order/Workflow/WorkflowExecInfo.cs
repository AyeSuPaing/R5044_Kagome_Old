/*
=========================================================================================================
  Module      : ワークフロー実行情報クラス(WorkflowExecInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// ワークフロー実行情報クラス
	/// </summary>
	public class WorkflowExecInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public WorkflowExecInfo()
		{
			this.DoExec = true;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="itemIndex">アイテムインデックス</param>
		/// <param name="masterId">マスターID（注文ID/定期購入ID）</param>
		/// <param name="doExec">実行しているか</param>
		/// <param name="updateCassetteStatus">アップデートカセットステータス</param>
		public WorkflowExecInfo(int itemIndex, string masterId, bool doExec, string updateCassetteStatus)
		{
			this.ItemIndex = itemIndex;
			this.MasterId = masterId;
			this.DoExec = doExec;
			this.UpdateCassetteStatus = updateCassetteStatus;
		}

		/// <summary>アイテムインデックス</summary>
		public int ItemIndex { get; set; }
		/// <summary>マスターID（注文ID/定期購入ID）</summary>
		public string MasterId { get; set; }
		/// <summary>実行しているか</summary>
		public bool DoExec { get; set; }
		/// <summary>実行カセットステータス</summary>
		public string UpdateCassetteStatus { get; set; }
	}
}