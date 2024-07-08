/*
=========================================================================================================
  Module      : メッセージ依頼サービス(CsMessageRequestService.cs)
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
	/// メッセージ依頼サービス
	/// </summary>
	public class CsMessageRequestService
	{
		/// <summary>レポジトリ</summary>
		private CsMessageRequestRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsMessageRequestService(CsMessageRequestRepository repository)
		{
			this.Repository = repository;
		}

		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <returns>モデル</returns>
		public CsMessageRequestModel Get(string deptId, string incidentId, int messageNo, int requestNo)
		{
			var dv = this.Repository.Get(deptId, incidentId, messageNo, requestNo);
			if (dv.Count == 0) return null;
			var request = new CsMessageRequestModel(dv[0]);
			return request;
		}
		#endregion

		#region +GetDraft 下書き依頼取得
		/// <summary>
		/// 下書き依頼取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>モデル</returns>
		public CsMessageRequestModel GetDraft(string deptId, string incidentId, int messageNo)
		{
			var dv =　this.Repository.GetDraft(deptId, incidentId, messageNo);
			if (dv.Count == 0) return null;
			var request = new CsMessageRequestModel(dv[0]);
			return request;
		}
		#endregion

		#region +GetAllWithItems 指定メッセージのリクエストをアイテム含めて全て取得
		/// <summary>
		/// 指定メッセージのリクエストをアイテム含めて全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>メッセージリクエスト配列</returns>
		public CsMessageRequestModel[] GetAllWithItems(string deptId, string incidentId, int messageNo)
		{
			var requests = GetAll(deptId, incidentId, messageNo);

			var itemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
			foreach (var req in requests)
			{
				req.EX_Items = itemService.GetAll(req.DeptId, req.IncidentId, req.MessageNo, req.RequestNo);
			}
			return requests;
		}
		#endregion

		#region +GetAll 指定メッセージのリクエストをすべて取得
		/// <summary>
		/// 最終リクエスト取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>メッセージリクエスト配列</returns>
		public CsMessageRequestModel[] GetAll(string deptId, string incidentId, int messageNo)
		{
			var dv = this.Repository.GetAll(deptId, incidentId, messageNo);
			var models = (from DataRowView drv in dv select new CsMessageRequestModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetDraftWithAllItems 全てのアイテム含めて下書き依頼取得
		/// <summary>
		/// 全てのアイテム含めて下書き依頼取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>モデル</returns>
		public CsMessageRequestModel GetDraftWithAllItems(string deptId, string incidentId, int messageNo)
		{
			var request = GetDraft(deptId, incidentId, messageNo);
			if (request == null) return null;

			var itemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
			var items = itemService.GetAll(request.DeptId, request.IncidentId, request.MessageNo, request.RequestNo);
			request.EX_Items = items;

			return request;
		}
		#endregion

		#region +GetWithAllItems 全てのアイテム含めて取得
		/// <summary>
		/// 全てのアイテム含めて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <returns>モデル</returns>
		public CsMessageRequestModel GetWithAllItems(string deptId, string incidentId, int messageNo, int requestNo)
		{
			var request = Get(deptId, incidentId, messageNo, requestNo);
			if (request == null) return null;

			var itemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
			var items = itemService.GetAll(request.DeptId, request.IncidentId, request.MessageNo, request.RequestNo);
			request.EX_Items = items;

			return request;
		}
		#endregion

		#region +GetResultFromItems 依頼アイテムから結果を取得
		/// <summary>
		/// 依頼アイテムから結果を取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <returns>判定結果（true：全てOK、false：全てNG、null：変更無し）</returns>
		public bool? GetResultFromItems(string deptId, string incidentId, int messageNo, int requestNo)
		{
			var request = GetWithAllItems(deptId, incidentId, messageNo, requestNo);
			if (request == null) return null;

			switch (request.ApprovalType)
			{
				// 合議
				case Constants.FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_CONSULTATION:
					// 全てOKならOK
					if (request.EX_Items.All(i => i.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK))
					{
						return true;
					}
					// １つでもNGならNG
					else if (request.EX_Items.Any(i => i.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG))
					{
						return false;
					}
					// 上記以外であれば変更無し
					return null;

				// 並行
				case Constants.FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_PARALLEL:
					// １つでもOKならOK
					if (request.EX_Items.Any(i => (i.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK)))
					{
						return true;
					}
					// １つでもNGならNG
					else if (request.EX_Items.Any(i => (i.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG)))
					{
						return false;
					}
					//　上記以外であれば変更無し
					return null;

				default:
					throw new Exception("未対応の承認方法です：" + request.ApprovalType);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="requestModel">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>メッセージ依頼NO</returns>
		private int Register(CsMessageRequestModel requestModel, SqlAccessor accessor)
		{
			// 依頼NO取得
			int requestNo = this.Repository.GetRegisterRequestNo(requestModel.DeptId, requestModel.IncidentId, requestModel.MessageNo, accessor);

			// 登録
			requestModel.RequestNo = requestNo;
			this.Repository.Register(requestModel.DataSource, accessor);

			return requestNo;
		}
		#endregion

		#region +RegisterWithAllItems アイテムも含めて登録
		/// <summary>
		/// アイテムも含めて登録
		/// </summary>
		/// <param name="requestModel">モデル</param>
		public void RegisterWithAllItems(CsMessageRequestModel requestModel)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				try
				{
					// 要求登録
					int requestNo = Register(requestModel, accessor);

					// 要求アイテム登録
					var itemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
					foreach (var item in requestModel.EX_Items)
					{
						item.RequestNo = requestNo;
						itemService.Register(item, accessor);
					}
					accessor.CommitTransaction();
				}
				catch (Exception)
				{
					accessor.RollbackTransaction();
					throw;
				}
			}
		}
		#endregion

		#region +UpdateWorkingOperator 作業中オペレータ更新
		/// <summary>
		/// 作業中オペレータ更新
		/// </summary>
		/// <param name="request">モデル</param>
		/// <returns>成功したか</returns>
		public bool UpdateWorkingOperator(CsMessageRequestModel request)
		{
			var updated = this.Repository.UpdateWorkingOperator(request.DataSource);
			return (updated > 0);
		}
		#endregion

		#region +UpdateRequestStatus 依頼ステータス更新
		/// <summary>
		/// 依頼ステータス更新
		/// </summary>
		/// <param name="request">モデル</param>
		/// <returns>成功したか</returns>
		public bool UpdateRequestStatus(CsMessageRequestModel request)
		{
			var updated = this.Repository.UpdateRequestStatus(request.DataSource);
			return (updated > 0);
		}
		#endregion

		#region +UpdateIncidentIdAllWithItems アイテム含めてインシデントIDすべて更新
		/// <summary>
		///  アイテム含めてインシデントIDすべて更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beforeIncidentId">更新前インシデントID</param>
		/// <param name="beforeMessegeNo">更新前メッセージNO</param>
		/// <param name="afterIncidentId">更新後インシデントID</param>
		/// <param name="afterMessegeNo">更新後メッセージNO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateIncidentIdAllWithItems(string deptId, string beforeIncidentId, int beforeMessegeNo, string afterIncidentId, int afterMessegeNo, string lastChanged, SqlAccessor accessor)
		{
			var requestItemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
			var requests = GetAll(deptId, beforeIncidentId, beforeMessegeNo);
			foreach (var request in requests)
			{
				var items = requestItemService.GetAll(deptId, beforeIncidentId, beforeMessegeNo, request.RequestNo);

				request.IncidentId = afterIncidentId;
				request.MessageNo = afterMessegeNo;
				request.LastChanged = lastChanged;
				Register(request, accessor);

				foreach (var item in items)
				{
					item.IncidentId = afterIncidentId;
					item.MessageNo = afterMessegeNo;
					item.LastChanged = lastChanged;
					requestItemService.Register(item, accessor);
				}
			}
		}
		#endregion

		#region +DeleteWithItems メッセージリクエストをアイテム含め削除
		/// <summary>
		/// メッセージリクエストをアイテム含め削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteWithItems(string deptId, string incidentId, int messageNo, int requestNo, SqlAccessor accessor)
		{
			// リクエスト削除
			this.Repository.Delete(deptId, incidentId, messageNo, requestNo, accessor);

			// 要求アイテム登録
			var itemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
			itemService.DeleteAll(deptId, incidentId, messageNo, requestNo, accessor);
		}
		#endregion

		#region +DeleteAllWithItems メッセージのすべてのリクエストをアイテム含め削除
		/// <summary>
		/// メッセージのすべてのリクエストをアイテム含め削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAllWithItems(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			// リクエスト削除
			this.Repository.DeleteAll(deptId, incidentId, messageNo, accessor);

			// 要求アイテム登録
			var itemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
			itemService.DeleteAllMessageRequestItems(deptId, incidentId, messageNo, accessor);
		}
		#endregion

		#region +DeleteDraftRequestWithItems メッセージのすべての下書き依頼情報をアイテム含め削除
		/// <summary>
		/// メッセージのすべての下書き依頼情報をアイテム含め削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		public void DeleteDraftRequestWithItems(string deptId, string incidentId, int messageNo)
		{
			// 対象取得
			var models = GetAll(deptId, incidentId, messageNo);
			var targets = models.Where(m => (m.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_DRAFT)
				|| (m.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_DRAFT)).ToArray();

			// 削除
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				try
				{
					foreach (var target in targets)
					{
						// リクエスト削除
						DeleteWithItems(target.DeptId, target.IncidentId, target.MessageNo, target.RequestNo, accessor);
					}
					accessor.CommitTransaction();
				}
				catch (Exception)
				{
					accessor.RollbackTransaction();
					throw;
				}
			}

		}
		#endregion

	}
}
