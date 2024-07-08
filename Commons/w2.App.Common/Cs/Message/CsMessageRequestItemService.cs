/*
=========================================================================================================
  Module      : メッセージ依頼アイテムサービス(CsMessageRequestItemService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Sql;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージ依頼アイテムサービス
	/// </summary>
	public class CsMessageRequestItemService
	{
		/// <summary>レポジトリ</summary>
		private CsMessageRequestItemRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsMessageRequestItemService(CsMessageRequestItemRepository repository)
		{
			this.Repository = repository;
		}

		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <returns>リスト</returns>
		public CsMessageRequestItemModel[] GetAll(string deptId, string incidentId, int messageNo, int requestNo)
		{
			var dv = this.Repository.GetAll(deptId, incidentId, messageNo, requestNo);
			var models = (from DataRowView drv in dv select new CsMessageRequestItemModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetByApprovalOeratorAndResultStatus 承認オペレータと結果から依頼アイテム取得
		/// <summary>
		/// 承認オペレータと結果ステータスから依頼アイテム取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">承認オペレータID</param>
		/// <param name="resultStatus">結果ステータス</param>
		/// <returns>リスト</returns>
		public CsMessageRequestItemModel[] GetByApprovalOeratorAndResultStatus(string deptId, string operatorId, string resultStatus)
		{
			var dv = this.Repository.GetByApprovalOeratorAndResultStatus(deptId, operatorId, resultStatus);
			var models = (from DataRowView drv in dv select new CsMessageRequestItemModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="itemModel">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>枝番</returns>
		public int Register(CsMessageRequestItemModel itemModel, SqlAccessor accessor)
		{
			// 依頼枝番取得
			int branchNo = this.Repository.GetRegisterBranchNo(itemModel.DeptId, itemModel.IncidentId, itemModel.MessageNo, itemModel.RequestNo, accessor);

			// 登録
			itemModel.BranchNo = branchNo;
			this.Repository.Register(itemModel.DataSource, accessor);

			return branchNo;
		}
		#endregion

		#region +UpdateResult 結果更新
		/// <summary>
		/// 結果更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateResult(CsMessageRequestItemModel model)
		{
			this.Repository.UpdateResult(model.DataSource);
		}
		#endregion

		#region +UpdateIncidentIdAll インシデントIDすべて更新
		/// <summary>
		/// インシデントIDすべて更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beforeIncidentId">更新前インシデントID</param>
		/// <param name="beforeMessegeNo">更新前メッセージNO</param>
		/// <param name="afterIncidentId">更新後インシデントID</param>
		/// <param name="afterMessegeNo">更新後メッセージNO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateIncidentIdAll(string deptId, string beforeIncidentId, int beforeMessegeNo, string afterIncidentId, int afterMessegeNo, string lastChanged, SqlAccessor accessor)
		{
			this.Repository.UpdateIncidentIdAll(deptId, beforeIncidentId, beforeMessegeNo, afterIncidentId, afterMessegeNo, lastChanged, accessor);
		}
		#endregion

		#region +DeleteAll すべて削除
		/// <summary>
		/// すべて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string incidentId, int messageNo, int requestNo, SqlAccessor accessor)
		{
			this.Repository.DeleteAll(deptId, incidentId, messageNo, requestNo, accessor);
		}
		#endregion

		#region +DeleteAllMessageRequestItems メッセージのリクエストアイテムすべて削除
		/// <summary>
		/// メッセージのリクエストアイテムすべて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAllMessageRequestItems(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			this.Repository.DeleteAllMessageRequestItems(deptId, incidentId, messageNo, accessor);
		}
		#endregion
	}
}
