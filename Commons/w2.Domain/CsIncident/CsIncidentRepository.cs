/*
=========================================================================================================
  Module      : インシデントリポジトリ (CsIncidentRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Domain.CsIncident
{
	/// <summary>
	/// インシデントリポジトリ
	/// </summary>
	public class CsIncidentRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "CsIncident";
		/// <summary>Incident ids</summary>
		private const string INCIDENT_IDS = "@@incident_ids@@";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsIncidentRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CsIncidentRepository(SqlAccessor accessor)
			: base(accessor)
		{
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
			var ht = new Hashtable
			{
				{Constants.FIELD_CSINCIDENT_DEPT_ID, deptId},
				{Constants.FIELD_CSINCIDENT_INCIDENT_ID, incidentId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new CsIncidentModel(dv[0]);
		}
		#endregion

		#region +GetCsIncidentByUserId ユーザーIDからインシデント情報取得
		/// <summary>
		/// ユーザーIDからインシデント情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデルリスト</returns>
		public CsIncidentModel[] GetCsIncidentByUserId(string deptId, string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_CSINCIDENT_DEPT_ID, deptId},
				{Constants.FIELD_CSINCIDENT_USER_ID, userId}
			};
			var dv = Get(XML_KEY_NAME, "GetCsIncidentByUserId", ht);
			if (dv.Count == 0) return new CsIncidentModel[0];
			var models = dv.Cast<DataRowView>().Select(drv => new CsIncidentModel(drv)).ToArray();

			return models;
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
			var ht = new Hashtable
			{
				{Constants.FIELD_CSINCIDENT_DEPT_ID, deptId},
				{Constants.FIELD_CSINCIDENT_INCIDENT_ID, incidentId}
			};
			var dv = Get(XML_KEY_NAME, "GetCsGroupIdByIncidentId", ht);
			return (dv.Count != 0) ? (string)dv[0][0] : null;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(CsIncidentModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateIncidentByIncidentIds
		/// <summary>
		/// Update Incident By Incident Ids
		/// </summary>
		/// <param name="incidentIds">Incident ids</param>
		/// <param name="input">Input data</param>
		public void UpdateIncidentByIncidentIds(string[] incidentIds, Hashtable input)
		{
			Exec(
				XML_KEY_NAME,
				"UpdateIncidentByIncidentIds",
				input,
				new KeyValuePair<string, string>(
					INCIDENT_IDS,
					string.Join(",", incidentIds.Select(id => string.Format("'{0}'", id.Replace("'", "''").Trim())))));
		}
		#endregion

		#region +CountTask 個人/グループタスクの件数集計
		/// <summary>
		/// 個人/グループタスクの件数集計
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="includeGroupUnassigned"></param>
		/// <param name="csGroupIdList">オペレータが所属しているCSグループIDリスト</param>
		/// <returns>個人/グループタスクの件数</returns>
		internal Pair<CsIncidentModel[], CsIncidentModel[]> CountTask(
			string deptId,
			string operatorId,
			bool includeGroupUnassigned,
			List<string> csGroupIdList)
		{
			var csGroupIds = csGroupIdList.Select(csGroupId =>
				string.Format("'{0}'", csGroupId.Replace("'", "''")))
				.JoinToString(",");
			var csGroupIdCondition = csGroupIdList.Any()
				? string.Format("OR w2_CsIncident.cs_group_id IN ({0})", csGroupIds)
				: string.Empty;
			var replaceTag = new[]
			{
				new KeyValuePair<string, string>("@@ cs_group_ids_condition @@", csGroupIdCondition)
			};
			var input = new Hashtable
			{
				{ Constants.FIELD_CSINCIDENT_DEPT_ID, deptId },
				{ Constants.FIELD_CSINCIDENT_OPERATOR_ID, operatorId },
				{ "include_unassigned_group_task", includeGroupUnassigned ? "1" : "0" },
			};
			var statementInfos = new[]
			{
				new KeyValuePair<string, string>(XML_KEY_NAME, "CountTask")
			};
			var ds = GetWithChilds(statementInfos, input, replaceTag);
			var firstTask = ds.Tables[0].DefaultView
				.Cast<DataRowView>()
				.Select(drv => new CsIncidentModel(drv))
				.ToArray();
			var secondTask = ds.Tables[1].DefaultView
				.Cast<DataRowView>()
				.Select(drv => new CsIncidentModel(drv))
				.ToArray();
			return new Pair<CsIncidentModel[], CsIncidentModel[]>(firstTask, secondTask);
		}
		#endregion
	}
}
