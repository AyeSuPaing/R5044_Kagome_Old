/*
=========================================================================================================
  Module      : インシデントサービス(CsIncidentService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Cs.CsOperator;
using w2.Common.Sql;
using w2.Common.Util;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.Search;

namespace w2.App.Common.Cs.Incident
{
	/// <summary>
	/// インシデントサービス
	/// </summary>
	public class CsIncidentService
	{
		/// <summary>レポジトリ</summary>
		private CsIncidentRepository Repository;

		#region +コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsIncidentService(CsIncidentRepository repository)
		{
			this.Repository = repository;
		}

		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="statuses">ステータス</param>
		/// <param name="categoryId">インシデントカテゴリID</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="tagrgetOperatorGroupType">対象はオペレータかグループか（1:オペレータ、2-1:グループ、2-2:グループだがグループ未振り分けも対象、0:すべて）</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort Type</param>
		/// <param name="csGroupId">グループID</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>インシデント配列</returns>
		public CsIncidentModel[] Search(string deptId, string operatorId, string[] statuses, string categoryId, string validFlg, string tagrgetOperatorGroupType, int beginRow, int endRow, string sortField, string sortType, string csGroupId, SearchParameter searchParameter)
		{
			var none = this.Repository.Search(deptId, operatorId, statuses, categoryId, validFlg, tagrgetOperatorGroupType, beginRow, endRow, sortField, sortType, csGroupId, searchParameter);
			var models = (from DataRowView drv in none select new CsIncidentModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>モデル</returns>
		public CsIncidentModel Get(string deptId, string incidentId)
		{
			var dv = this.Repository.Get(deptId, incidentId);
			if (dv.Count == 0) return null;
			var incident = new CsIncidentModel(dv[0]);

			incident.EX_SetInvalidLavel(deptId);

			return incident;
		}
		#endregion

		#region +GetWithSummaryValues 集計区分値とともに取得
		/// <summary>
		/// 集計区分値とともに取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>モデル</returns>
		public CsIncidentModel GetWithSummaryValues(string deptId, string incidentId)
		{
			// インシデント情報取得
			var incident = Get(deptId, incidentId);
			if (incident == null) return null;

			// 集計区分値取得
			var summaryValueService = new CsIncidentSummaryValueService(new CsIncidentSummaryValueRepository());
			incident.EX_SummaryValues = summaryValueService.GetList(deptId, incidentId);
			return incident;
		}
		#endregion

		#region +GetByMessageId メッセージIDを元にインシデント情報取得
		/// <summary>
		/// 該当メッセージIDのメールを送信/受信したメッセージに紐付くインシデント取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="messageId">メッセージID</param>
		/// <returns>インシデントリスト</returns>
		public CsIncidentModel[] GetByMessageId(string deptId, string messageId)
		{
			var none = this.Repository.GetByMessageId(deptId, messageId);
			CsIncidentModel[] models = (from DataRowView drv in none select new CsIncidentModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +CountWarning 滞留警告メール送信用に件数集計
		/// <summary>
		/// 滞留警告メール送信用に件数集計
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="warningLimitDays">滞留日数</param>
		/// <returns>インシデント配列のペア</returns>
		public Pair<CsIncidentModel[], CsIncidentModel[]> CountWarning(string deptId, int warningLimitDays)
		{
			var none = this.Repository.CountWarning(deptId, warningLimitDays);
			var models1 = (from DataRowView drv in none.Tables[0].DefaultView select new CsIncidentModel(drv)).ToArray();	// 本人
			var models2 = (from DataRowView drv in none.Tables[1].DefaultView select new CsIncidentModel(drv)).ToArray();	// グループ
			return new Pair<CsIncidentModel[], CsIncidentModel[]>(models1, models2);
		}
		#endregion

		#region +UpdateLockStatusForLock ロック取得用ロックステータス更新
		/// <summary>
		/// ロック取得用ロックステータス更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="lockStatusBefore">変更前ロックステータス</param>
		/// <param name="lockStatusAfter">変更後ロックステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>成功したか</returns>
		public bool UpdateLockStatusForLock(string deptId, string incidentId, string operatorId, string lockStatusBefore, string lockStatusAfter, string lastChanged)
		{
			var result = this.Repository.UpdateLockStatusForLock(
				deptId,
				incidentId,
				operatorId,
				lockStatusBefore,
				lockStatusAfter,
				lastChanged);
			return result;
		}
		#endregion

		#region +UpdateLockStatusForUnlock ロック解除用ロックステータス更新
		/// <summary>
		/// ロック解除用ロックステータス更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>成功したか</returns>
		public bool UpdateLockStatusForUnlock(string deptId, string incidentId, string lastChanged)
		{
			var result = this.Repository.UpdateLockStatusForUnlock(
				deptId,
				incidentId,
				lastChanged);
			return result;
		}
		#endregion

		#region +RegisterUpdateWithSummaryValues 集計区分値含めて登録／更新
		/// <summary>
		/// 集計区分値含めて登録／更新
		/// </summary>
		/// <param name="incident">データ</param>
		/// <returns>登録／更新したインシデントID</returns>
		public string RegisterUpdateWithSummaryValues(CsIncidentModel incident)
		{
			string incidentId;
			if (string.IsNullOrEmpty(incident.IncidentId))
			{
				incidentId = RegisterWithSummaryValues(incident);
			}
			else
			{
				UpdateWithSummaryValues(incident);
				incidentId = incident.IncidentId;
			}
			
			return incidentId;
		}
		#endregion

		#region +RegisterEmpty 空のインシデントを登録
		/// <summary>
		/// 空のインシデントを登録
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="beforeCsIncident">元インシデント</param>
		/// <returns>登録したインシデントID</returns>
		public string RegisterEmpty(string deptId, string lastChanged, CsIncidentModel beforeCsIncident)
		{
			var incidnet = CreateDefaultModel();
			incidnet.LastChanged = lastChanged;
			incidnet.IncidentTitle = beforeCsIncident.IncidentTitle;
			incidnet.UserName = beforeCsIncident.UserName;
			incidnet.IncidentCategoryId = beforeCsIncident.IncidentCategoryId;
			incidnet.Status = beforeCsIncident.Status;
			incidnet.OperatorId = beforeCsIncident.OperatorId;
			return this.Register(incidnet);
		}
		#endregion

		#region +CreateDefaultModel 空のインシデントモデル作成
		/// <summary>
		/// 空のインシデントモデル作成
		/// </summary>
		/// <returns>インシデントモデル</returns>
		public static CsIncidentModel CreateDefaultModel()
		{
			var incidnet = new CsIncidentModel();
			incidnet.DeptId = Constants.CONST_DEFAULT_DEPT_ID;
			incidnet.UserId = "";
			incidnet.IncidentCategoryId = "";
			incidnet.IncidentTitle = "";
			incidnet.Status = Constants.FLG_CSINCIDENT_STATUS_NONE;
			incidnet.VocId = "";
			incidnet.VocMemo = "";
			incidnet.Comment = "";
			incidnet.Importance = Constants.FLG_CSINCIDENT_IMPORTANCE_MIDDLE;
			incidnet.UserName = "";
			incidnet.UserContact = "";
			incidnet.CsGroupId = "";
			incidnet.OperatorId = "";
			incidnet.ValidFlg = Constants.FLG_CSINCIDENT_VALID_FLG_VALID;
			incidnet.LastChanged = "";
			return incidnet;
		}
		#endregion

		#region -RegisterWithSummaryValues 集計区分値含めて登録
		/// <summary>
		/// 集計区分値含めて登録
		/// </summary>
		/// <param name="incident">データ</param>
		/// <returns>登録したインシデントID</returns>
		private string RegisterWithSummaryValues(CsIncidentModel incident)
		{
			string incidentId = "";

			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 登録
				incidentId = Register(incident, accessor);

				// 集計区分登録
				var summaryValueService = new CsIncidentSummaryValueService(new CsIncidentSummaryValueRepository());
				foreach (var summaryValue in incident.EX_SummaryValues)
				{
					summaryValue.IncidentId = incidentId;
					summaryValueService.Register(summaryValue, accessor);
				}

				// 対応完了日更新
				if (incident.Status == Constants.FLG_CSINCIDENT_STATUS_COMPLETE) UpdateCompleteDate(incident, accessor);

				accessor.CommitTransaction();
			}
			return incidentId;
		}
		#endregion

		#region -UpdateWithSummaryValues 集計区分値含めて更新
		/// <summary>
		/// 集計区分値含めて更新
		/// </summary>
		/// <param name="incident">データ</param>
		private void UpdateWithSummaryValues(CsIncidentModel incident)
		{
			// 対応完了日更新判定
			bool doUpdateCompleteDate = false;
			if (incident.Status == Constants.FLG_CSINCIDENT_STATUS_COMPLETE)
			{
				var old = GetWithSummaryValues(incident.DeptId, incident.IncidentId);
				doUpdateCompleteDate = (old.Status != Constants.FLG_CSINCIDENT_STATUS_COMPLETE);
			}
	
			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				Update(incident, accessor);

				// 集計区分更新（無ければ登録）
				var summaryValueService = new CsIncidentSummaryValueService(new CsIncidentSummaryValueRepository());
				foreach (var summaryValue in incident.EX_SummaryValues)
				{
					if (summaryValueService.Get(summaryValue.DeptId, summaryValue.IncidentId, summaryValue.SummaryNo, accessor) != null)
					{
						summaryValueService.Update(summaryValue, accessor);
					}
					else
					{
						summaryValueService.Register(summaryValue, accessor);
					}
				}

				// 対応完了日更新
				if (doUpdateCompleteDate) UpdateCompleteDate(incident, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region -Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録したインシデントID</returns>
		private string Register(CsIncidentModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				string incidentId = Register(model, accessor);

				accessor.CommitTransaction();

				return incidentId;
			}
		}
		#endregion
		#region -Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録したインシデントID</returns>
		private string Register(CsIncidentModel model, SqlAccessor accessor)
		{
			// 登録
			model.IncidentId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_INCIDENT_ID, 8);
			this.Repository.Register(model.DataSource, accessor);

			return model.IncidentId;
		}
		#endregion

		#region +UpdateWithMessages メッセージも全て更新
		/// <summary>
		/// メッセージも全て更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		/// <param name="messages">メッセージリスト</param>
		public void UpdateWithMessages(CsIncidentModel incident, CsMessageModel[] messages)
		{
			var messageRepository = new CsMessageRepository();

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				this.Repository.Update(incident.DataSource, accessor);

				foreach (var msg in messages)
				{
					messageRepository.Update(msg.DataSource, accessor);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		private void Update(CsIncidentModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				this.Repository.Update(model.DataSource, accessor);
			}
		}
		#endregion
		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void Update(CsIncidentModel incident, SqlAccessor accessor)
		{
			this.Repository.Update(incident.DataSource, accessor);
		}
		#endregion

		#region +UpdateValidFlg 有効フラグ更新
		/// <summary>
		/// 有効フラグ更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		public void UpdateValidFlg(CsIncidentModel incident)
		{
			this.Repository.UpdateValidFlg(incident.DataSource);
		}
		#endregion

		#region +UpdateValidFlgWithMessage メッセージ含めて有効フラグ更新
		/// <summary>
		/// メッセージ含めて有効フラグ更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		public void UpdateValidFlgWithMessage(CsIncidentModel incident)
		{
			UpdateValidFlg(incident);
	
			var messageService = new CsMessageService(new CsMessageRepository());
			var messages = messageService.GetAll(incident.DeptId, incident.IncidentId);
			foreach (var m in messages)
			{
				m.ValidFlg = incident.ValidFlg;
				m.LastChanged = incident.LastChanged;
				messageService.UpdateValidFlg(m);
			}
		}
		#endregion

		#region +UpdateStatusAndCompleteDate ステータス・対応完了日更新
		/// <summary>
		/// ステータス・対応完了日更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		public void UpdateStatusAndCompleteDate(CsIncidentModel incident)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateStatus(incident, accessor);
				UpdateCompleteDate(incident, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateStatus ステータス更新
		/// <summary>
		/// ステータス更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateStatus(CsIncidentModel incident, SqlAccessor accessor)
		{
			this.Repository.UpdateStatus(incident.DataSource, accessor);
		}
		#endregion

		#region +UpdateLastReceivedDate 最終受信日時更新
		/// <summary>
		/// 最終受信日時更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		public void UpdateLastReceivedDate(CsIncidentModel incident)
		{
			this.Repository.UpdateLastReceivedDate(incident.DataSource);
		}
		#endregion

		#region +UpdateLastReceivedDate 最終送信日時更新
		/// <summary>
		/// 最終送信日時更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		public void UpdateLastSendDate(CsIncidentModel incident)
		{
			this.Repository.UpdateLastSendDate(incident.DataSource);
		}
		#endregion

		#region +UpdateCompleteDate 完了日更新
		/// <summary>
		/// 完了日更新
		/// </summary>
		/// <param name="incident">インシデント</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateCompleteDate(CsIncidentModel incident, SqlAccessor accessor)
		{
			this.Repository.UpdateCompleteDate(incident.DataSource, accessor);
		}
		#endregion

		#region -UpdateIncidentByMailAssignSetting
		/// <summary>
		/// Update incident by mail assign setting
		/// </summary>
		/// <param name="incident">Incident</param>
		public void UpdateIncidentByMailAssignSetting(CsIncidentModel incident)
		{
			this.Repository.UpdateIncidentByMailAssignSetting(incident.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		public void Delete(string deptId, string incidentId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// インシデント削除
				this.Repository.Delete(deptId, incidentId, accessor);

				// 集計区分値削除
				var summaryValueService = new CsIncidentSummaryValueService(new CsIncidentSummaryValueRepository());
				summaryValueService.DeleteAll(deptId, incidentId, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}
