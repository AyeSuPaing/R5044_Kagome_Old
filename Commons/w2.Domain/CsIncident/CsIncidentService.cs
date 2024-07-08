/*
=========================================================================================================
  Module      : インシデントサービス (CsIncidentService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.CsOperatorGroup;

namespace w2.Domain.CsIncident
{
	/// <summary>
	/// インシデントサービス
	/// </summary>
	public class CsIncidentService : ServiceBase
	{
		#region +GetCsIncidentByUserId ユーザーIDからインシデント情報取得
		/// <summary>
		/// ユーザーIDからインシデント情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデルリスト</returns>
		public CsIncidentModel[] GetCsIncidentByUserId(string deptId, string userId, SqlAccessor accessor = null)
		{
			using (var repository = new CsIncidentRepository(accessor))
			{
				var models = repository.GetCsIncidentByUserId(deptId, userId);
				return models;
			}
		}
		#endregion

		#region +GetCsGroupIdByIncidentId インシデントIDから担当グループID取得
		/// <summary>
		/// インシデントIDから担当グループID取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>担当グループID</returns>
		public string GetCsGroupIdByIncidentId(string deptId, string incidentId)
		{
			using (var repository = new CsIncidentRepository())
			{
				var csGroupId = repository.GetCsGroupIdByIncidentId(deptId, incidentId);
				return csGroupId;
			}
		}
		#endregion

		#region +UpdateUserId ユーザーID更新
		/// <summary>
		/// ユーザーID更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateUserId(string deptId, string incidentId, string userId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new CsIncidentRepository(accessor))
			{
				// 最新インシデント取得
				var model = repository.Get(deptId, incidentId);
				// ユーザーID、最終更新者セット
				model.UserId = userId;
				model.LastChanged = lastChanged;
				return repository.Update(model);
			}
		}
		#endregion

		#region +UpdateIncidentByIncidentIds
		/// <summary>
		/// Update Incident By Incident Ids
		/// </summary>
		/// <param name="incidentIds">Incident id</param>
		/// <param name="input">Input data</param>
		public void UpdateIncidentByIncidentIds(string[] incidentIds, Hashtable input)
		{
			using (var repository = new CsIncidentRepository())
			{
				repository.UpdateIncidentByIncidentIds(incidentIds, input);
			}
		}
		#endregion

		#region +CountTask 個人/グループタスクの件数集計
		/// <summary>
		/// 個人/グループタスクの件数集計
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="includeUnassigned">グループタスクで未振り分けを含めるかどうか</param>
		/// <returns>インシデント配列のペア</returns>
		public Pair<CsIncidentModel[], CsIncidentModel[]> CountTask(
			string deptId,
			string operatorId,
			bool includeUnassigned)
		{
			var csGroupIdList = new CsOperatorGroupService()
				.GetGroups(deptId, operatorId)
				.Select(csOparatorGroup => csOparatorGroup.CsGroupId)
				.ToList();
			using (var repository = new CsIncidentRepository())
			{
				var result = repository.CountTask(
					deptId,
					operatorId,
					includeUnassigned,
					csGroupIdList);
				return result;
			}
		}
		#endregion
	}
}
