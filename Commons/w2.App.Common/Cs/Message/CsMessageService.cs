/*
=========================================================================================================
  Module      : メッセージサービス(CsMessageService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.App.Common.Cs.Search;
using w2.Common.Sql;
using w2.App.Common.Cs.Incident;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージサービス
	/// </summary>
	public class CsMessageService
	{
		/// <summary>レポジトリ</summary>
		private CsMessageRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsMessageService(CsMessageRepository repository)
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
		/// <returns>メッセージ情報</returns>
		public CsMessageModel Get(string deptId, string incidentId, int messageNo)
		{
			var dv = this.Repository.Get(deptId, incidentId, messageNo);
			if (dv.Count == 0) return null;

			var message = new CsMessageModel(dv[0]);
			return message;
		}
		#endregion

		#region +GetWithMail メール含めて取得
		/// <summary>
		/// メール含めて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>メッセージ情報</returns>
		public CsMessageModel GetWithMail(string deptId, string incidentId, int messageNo)
		{
			var message = Get(deptId, incidentId, messageNo);
			if (message == null) return message;

			message.EX_Mail = GetMailWithAttachment(message);

			return message;
		}
		#endregion

		#region +GetCount 件数取得
		/// <summary>
		/// 件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] GetCount(string deptId, string operatorId)
		{
			var dv = this.Repository.GetCount(deptId, operatorId);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetCountByRequest 件数取得（メッセージ依頼単位）
		/// <summary>
		/// 件数取得（メッセージ依頼単位）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] GetCountByRequest(string deptId, string operatorId)
		{
			var dv = this.Repository.GetCountByRequest(deptId, operatorId);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetCountByRequestItem 件数取得（メッセージ依頼アイテム単位）
		/// <summary>
		/// 件数取得（メッセージ依頼アイテム単位）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] GetCountByRequestItem(string deptId, string operatorId)
		{
			var dv = this.Repository.GetCountByRequestItem(deptId, operatorId);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +SearchValidByCreateOperatorAndStatus 作成オペレータとステータスを指定して有効なものを検索
		/// <summary>
		/// 作成オペレータとステータスを指定して有効なものを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">依頼オペレータID</param>
		/// <param name="messageStatus">メッセージステータス</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] SearchValidByCreateOperatorAndStatus(string deptId, string operatorId, string messageStatus, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			var dv = this.Repository.SearchValidByCreateOperatorAndStatus(deptId, operatorId, messageStatus, beginRow, endRow, sortField, sortType, searchParameter);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();

			SetMailAndAttachmentToMessages(models);

			return models;
		}
		#endregion

		#region +SearchByCreateOperatorAndStatusAndValidFlg ステータスと有効フラグを指定して検索
		/// <summary>
		/// ステータスと有効フラグを指定して検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="status">メッセージステータス</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>メッセージ情報</returns>
		public CsMessageModel[] SearchByCreateOperatorAndStatusAndValidFlg(string deptId, string operatorId, string[] status, string validFlg, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			var dv = this.Repository.SearchByCreateOperatorAndStatusAndValidFlg(deptId, operatorId, status, validFlg, beginRow, endRow, sortField, sortType, false, searchParameter);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +SearchValidByReplyOperator 回答オペレータを指定して有効なものを検索（回答済みページ用）
		/// <summary>
		/// 回答オペレータを指定して有効なものを検索（回答済みページ用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="replyOperatorId">返信オペレータID</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] SearchValidByReplyOperator(string deptId, string replyOperatorId, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			var dv = this.Repository.SearchValidByReplyOperator(
				deptId,
				replyOperatorId,
				beginRow,
				endRow,
				sortField,
				sortType,
				searchParameter);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();

			SetMailAndAttachmentToMessages(models);

			return models;
		}
		#endregion

		#region +SearchValidRequestByApprovalOperatorAndStatus 承認オペレータとステータスを指定して有効な依頼メッセージを検索
		/// <summary>
		/// 承認オペレータとステータスを指定して有効な依頼メッセージを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="apprOperatorId">承認オペレータID</param>
		/// <param name="messageStatus">メッセージステータス</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] SearchValidRequestByApprovalOperatorAndStatus(string deptId, string apprOperatorId, string messageStatus, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			var dv = this.Repository.SearchValidRequestByApprovalOperatorAndStatus(deptId, apprOperatorId, messageStatus, beginRow, endRow, sortField, sortType, searchParameter);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();

			SetMailAndAttachmentToMessages(models);
			SetMessageRequestToMessages(models);

			return models;
		}
		#endregion

		#region +SearchValidRequestByRequestOperator 依頼オペレータを指定して有効な依頼メッセージを検索
		/// <summary>
		/// 依頼オペレータを指定して有効な依頼メッセージを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">依頼オペレータID</param>
		/// <param name="messageStatus">メッセージステータス</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public CsMessageModel[] SearchValidRequestByRequestOperator(string deptId, string operatorId, string[] messageStatus, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			var dv = this.Repository.SearchValidRequestByRequestOperator(deptId, operatorId, messageStatus, beginRow, endRow, sortField, sortType, searchParameter);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();

			SetMailAndAttachmentToMessages(models);
			SetMessageRequestToMessages(models);

			return models;
		}
		#endregion

		#region +SearchAdvanced メッセージを詳細検索
		/// <summary>
		/// メッセージを詳細検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="sp">検索パラメータ</param>
		/// <param name="beginRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>検索結果の一覧</returns>
		public CsMessageModel[] SearchAdvanced(string deptId, SearchParameter sp, int beginRow, int endRow, string sortField, string sortType)
		{
			var dv = this.Repository.SearchAdvanced(deptId, sp, beginRow, endRow, sortField, sortType);
			var models = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();

			SetMailAndAttachmentToMessages(models);

			return models;
		}
		#endregion

		#region +SearchAdvancedByIncident メッセージを詳細検索（インシデント単位で取得）
		/// <summary>
		/// メッセージを詳細検索（インシデント単位で取得）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="sp">検索パラメータ</param>
		/// <param name="beginRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>検索結果の一覧</returns>
		public CsIncidentModel[] SearchAdvancedByIncident(string deptId, SearchParameter sp, int beginRow, int endRow, string sortField, string sortType)
		{
			var dv = this.Repository.SearchAdvanced(deptId, sp, beginRow, endRow, sortField, sortType, true);
			var result = (from DataRowView drv in dv select new CsIncidentModel(drv)).ToArray();

			// 表示文言の組み立て
			foreach (CsIncidentModel incident in result)
			{
				incident.EX_SetInvalidLavel(deptId);
			}

			return result;
		}
		#endregion

		#region -SetMailAndAttachmentToMessages メッセージへメール・添付セット
		/// <summary>
		/// メッセージへメール・添付セット
		/// </summary>
		/// <param name="models">メッセージリスト</param>
		private void SetMailAndAttachmentToMessages(CsMessageModel[] models)
		{
			models.ToList().ForEach(m => m.EX_Mail = GetMailWithAttachment(m));
		}
		#endregion

		#region -GetMailWithAttachment メッセージメール・添付取得
		/// <summary>
		/// メッセージメール・添付取得
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <returns>メール</returns>
		public CsMessageMailModel GetMailWithAttachment(CsMessageModel message)
		{
			var service = new CsMessageMailService(new CsMessageMailRepository());

			CsMessageMailModel mail = null;
			if (string.IsNullOrEmpty(message.SendMailId) == false)
			{
				mail = service.GetWithAttachment(message.DeptId, message.SendMailId);
			}
			else if (string.IsNullOrEmpty(message.ReceiveMailId) == false)
			{
				mail = service.GetWithAttachment(message.DeptId, message.ReceiveMailId);
			}
			return mail;
		}
		#endregion

		#region -SetMessageRequestToMessages メッセージへメッセージリクエストセット
		/// <summary>
		/// メッセージへメッセージリクエストセット
		/// </summary>
		/// <param name="models">メッセージリスト</param>
		private void SetMessageRequestToMessages(CsMessageModel[] models)
		{
			models.Where(m => m.EX_RequestNo != 0).ToList().ForEach(m => m.EX_Request = GetMessageRequest(m));
		}
		#endregion

		#region -GetMessageRequest メッセージリクエスト取得
		/// <summary>
		/// メッセージリクエスト取得
		/// </summary>
		/// <param name="models">メッセージ</param>
		private CsMessageRequestModel GetMessageRequest(CsMessageModel message)
		{
			var service = new CsMessageRequestService(new CsMessageRequestRepository());
			CsMessageRequestModel req = service.Get(message.DeptId, message.IncidentId, message.MessageNo, message.EX_RequestNo);
			return req;
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>メッセージリスト情報</returns>
		public CsMessageModel[] GetAll(string deptId, string incidentId)
		{
			var dv = this.Repository.GetAll(deptId, incidentId);
			var list = (from DataRowView drv in dv select new CsMessageModel(drv)).ToArray();
			return list;
		}
		/// <summary>
		/// 全て取得 (Sort)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>メッセージリスト情報</returns>
		public CsMessageModel[] GetAll(string deptId, string incidentId, string sortField, string sortType)
		{
			var data = this.Repository.GetAll(deptId, incidentId, sortField, sortType);
			var list = (from DataRowView row in data select new CsMessageModel(row)).ToArray();
			return list;
		}
		#endregion

		#region +GetLastMessageByIncidentIds 複数インシデントIDでインシデント最後のメッセージを取得
		/// <summary>
		/// 複数インシデントIDでインシデント最後のメッセージを取得
		/// </summary>
		/// <param name="deptId"></param>
		/// <param name="incidentIdList"></param>
		/// <returns>インシデント最後のメッセージリスト</returns>
		public CsMessageModel[] GetLastMessageByIncidentIds(string deptId, string[] incidentIdList)
		{
			var messages = this.Repository.GetLastMessageByIncidentIds(deptId, incidentIdList);
			return messages;
		}
		#endregion

		#region +GetAllReceiveMail
		/// <summary>
		/// Get all receive mail
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>メッセージリスト情報</returns>
		public CsMessageModel[] GetAllReceiveMail(string deptId)
		{
			var data = this.Repository.GetAllReceiveMail(deptId);

			return (from DataRowView row in data select new CsMessageModel(row)).ToArray();
		}
		#endregion

		#region +GetValidAll 有効なもの全て取得
		/// <summary>
		/// 有効なもの全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>メッセージリスト情報</returns>
		public CsMessageModel[] GetValidAll(string deptId, string incidentId)
		{
			var list = (from message in GetAll(deptId, incidentId)
						where (message.ValidFlg == Constants.FLG_CSMESSAGE_VALID_FLG_VALID)
						select message).ToArray();
			return list;
		}
		/// <summary>
		/// 有効なもの全て取得 (Sort)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>メッセージリスト情報</returns>
		public CsMessageModel[] GetValidAll(string deptId, string incidentId, string sortField, string sortType)
		{
			var list = (from message in GetAll(deptId, incidentId, sortField, sortType)
				where (message.ValidFlg == Constants.FLG_CSMESSAGE_VALID_FLG_VALID)
				select message).ToArray();
			return list;
		}
		#endregion

		#region +GetLastMail 最後のメッセージメールを取得（メッセージID取得などのため）
		/// <summary>
		/// 最後のメッセージメールを取得（メッセージID取得などのため）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>メッセージメール情報</returns>
		public CsMessageMailModel GetLastMail(string deptId, string incidentId)
		{
			var list = GetValidAll(deptId, incidentId);

			var mailIds = (from i in list
						   where (string.IsNullOrEmpty(i.SendMailId) == false) || (string.IsNullOrEmpty(i.ReceiveMailId) == false)
						   orderby i.MessageNo descending
						   select (string.IsNullOrEmpty(i.ReceiveMailId) == false) ? i.ReceiveMailId : i.SendMailId).ToArray();
			if (mailIds.Length == 0) return null;

			var mailService = new CsMessageMailService(new CsMessageMailRepository());
			var lastMail = mailService.GetWithAttachment(deptId, mailIds[0]);
			return lastMail;
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="messageModel">モデル</param>
		/// <returns>メッセージNO</returns>
		public int Register(CsMessageModel messageModel)
		{
			int messageNo = 0;

			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// メッセージNO取得
				messageNo = this.Repository.GetRegisterMessageNo(messageModel.DeptId, messageModel.IncidentId, accessor);

				// 登録
				messageModel.MessageNo = messageNo;
				this.Repository.Register(messageModel.DataSource, accessor);

				accessor.CommitTransaction();
			}
			return messageNo;
		}
		#endregion

		#region +CreateDefaultReceivedMailModel 空のメール受信用メッセージモデル作成
		/// <summary>
		/// 空のメール受信用メッセージモデル作成
		/// </summary>
		/// <returns>メッセージモデル</returns>
		public static CsMessageModel CreateDefaultReceivedMailModel()
		{
			var model = new CsMessageModel();
			model.DeptId = Constants.CONST_DEFAULT_DEPT_ID;
			model.IncidentId = "";
			model.MediaKbn = Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL;
			model.DirectionKbn = Constants.FLG_CSMESSAGE_DIRECTION_KBN_RECEIVE;
			model.OperatorId = "";
			model.MessageStatus = Constants.FLG_CSMESSAGE_MESSAGE_STATUS_RECEIVED;
			model.UserName1 = "";
			model.UserName2 = "";
			model.UserNameKana1 = "";
			model.UserNameKana2 = "";
			model.UserMailAddr = "";
			model.UserTel1 = "";
			model.InquiryTitle = "";
			model.InquiryText = "";
			model.ReplyText = "";
			model.ReceiveMailId = "";
			model.SendMailId = "";
			model.ValidFlg = Constants.FLG_CSMESSAGE_VALID_FLG_VALID;
			model.LastChanged = "";
			return model;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="message">メッセージモデル</param>
		public void Update(CsMessageModel message)
		{
			this.Repository.Update(message.DataSource);
		}
		#endregion

		#region +UpdateStatusFromRequestResult 依頼結果からステータス更新
		/// <summary>
		/// 依頼結果からステータス更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>変更後ステータス（null：更新なし）</returns>
		public string UpdateStatusFromRequestResult(string deptId, string incidentId, int messageNo, int requestNo, string lastChanged)
		{
			var messageBefore = Get(deptId, incidentId, messageNo);

			// 結果判定
			var requestService = new CsMessageRequestService(new CsMessageRequestRepository());
			var requestResult = requestService.GetResultFromItems(deptId, incidentId, messageNo, requestNo);
			if (requestResult == null) return null;

			// 更新
			var message = new CsMessageModel();
			var request = new CsMessageRequestModel();
			request.DeptId = message.DeptId = deptId;
			request.IncidentId = message.IncidentId = incidentId;
			request.MessageNo = message.MessageNo = messageNo;
			request.RequestNo = requestNo;
			request.LastChanged = message.LastChanged = lastChanged;
			if (messageBefore.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ)
			{
				message.MessageStatus = requestResult.Value ? Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK : Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG;
				UpdateMessageStatus(message);

				request.RequestStatus = requestResult.Value ? Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_OK : Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_NG;
				requestService.UpdateRequestStatus(request);
			}
			else if (messageBefore.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ)
			{
				message.MessageStatus = requestResult.Value ? Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE : Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG;
				UpdateMessageStatus(message);

				request.RequestStatus = requestResult.Value ? Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_OK : Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_NG;
				requestService.UpdateRequestStatus(request);
			}

			return message.MessageStatus;
		}

		#endregion

		#region +UpdateMessageStatus メッセージステータス更新
		/// <summary>
		/// メッセージステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateMessageStatus(CsMessageModel model)
		{
			this.Repository.UpdateMessageStatus(model.DataSource);
		}
		#endregion

		#region +UpdateValidFlg 有効フラグ全て更新
		/// <summary>
		/// 有効フラグ更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateValidFlg(CsMessageModel model)
		{
			this.Repository.UpdateValidFlg(model.DataSource);
		}
		#endregion

		#region +UpdateIncidentIdWithRequests リクエスト情報含めてインシデントID更新
		/// <summary>
		/// イリクエスト情報含めてインシデントID更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beforeIncidentId">更新前インシデントID</param>
		/// <param name="beforeMessegeNo">更新前メッセージNO</param>
		/// <param name="afterIncidentId">更新後インシデントID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新後メッセージNO</returns>
		public int UpdateIncidentIdWithRequests(string deptId, string beforeIncidentId, int beforeMessegeNo, string afterIncidentId, string lastChanged)
		{
			int afterMessageNo;

			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// メッセージNO取得
				afterMessageNo = this.Repository.GetRegisterMessageNo(deptId, afterIncidentId, accessor);

				var afterMessage = Get(deptId, beforeIncidentId, beforeMessegeNo);
				afterMessage.IncidentId = afterIncidentId;
				afterMessage.MessageNo = afterMessageNo;
				afterMessage.LastChanged = lastChanged;
				
				// メッセージ更新
				this.Repository.Register(afterMessage.DataSource, accessor);

				// メッセージリクエスト更新
				var requestService = new CsMessageRequestService(new CsMessageRequestRepository());
				requestService.UpdateIncidentIdAllWithItems(deptId, beforeIncidentId, beforeMessegeNo, afterIncidentId, afterMessageNo, lastChanged, accessor);


				accessor.CommitTransaction();
			}
			return afterMessageNo;
		}
		#endregion

		#region +DeleteWithRequestAndMails リクエスト＆メール含めて削除
		/// <summary>
		/// リクエスト＆メール含めて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		public void DeleteWithRequestAndMails(string deptId, string incidentId, int messageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				DeleteWithRequestAndMails(deptId, incidentId, messageNo, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion
		#region +DeleteWithRequestAndMails リクエスト＆メール含めて削除
		/// <summary>
		/// リクエスト＆メール含めて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteWithRequestAndMails(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			var message = this.Get(deptId, incidentId, messageNo);

			// メッセージ削除
			this.Repository.Delete(deptId, incidentId, messageNo, accessor);

			// リクエスト削除
			var messageRequestService = new CsMessageRequestService(new CsMessageRequestRepository());
			messageRequestService.DeleteAllWithItems(deptId, incidentId, messageNo, accessor);

			// メール削除
			var messageMailService = new CsMessageMailService(new CsMessageMailRepository());
			if (string.IsNullOrEmpty(message.SendMailId) == false)
			{
				messageMailService.DeleteWithAtachmentAndDatas(deptId, message.SendMailId, accessor);
			}
			if (string.IsNullOrEmpty(message.ReceiveMailId) == false)
			{
				messageMailService.DeleteWithAtachmentAndDatas(deptId, message.ReceiveMailId, accessor);
			}
		}
		#endregion

		#region +DeleteAllWithMails メール含めて全て削除
		/// <summary>
		/// メール含めて全て削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		public void DeleteAllWithMails(string deptId, string incidentId)
		{
			var list = GetAll(deptId, incidentId);

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				foreach (var msg in list)
				{
					DeleteWithRequestAndMails(msg.DeptId, msg.IncidentId, msg.MessageNo);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}
