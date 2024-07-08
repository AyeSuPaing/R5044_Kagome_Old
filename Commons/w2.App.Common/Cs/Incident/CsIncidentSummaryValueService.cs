/*
=========================================================================================================
  Module      : インシデント集計区分値サービス(CsIncidentSummaryValueService.cs)
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

namespace w2.App.Common.Cs.Incident
{
	public class CsIncidentSummaryValueService
	{
		/// <summary>レポジトリ</summary>
		private CsIncidentSummaryValueRepository Repository;

		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsIncidentSummaryValueService(CsIncidentSummaryValueRepository repository)
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
		/// <param name="summaryNo">集計区分NO</param>
		/// <param name="asccesor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public CsIncidentSummaryValueModel Get(string deptId, string incidentId, int summaryNo, SqlAccessor asccesor)
		{
			var dv = this.Repository.Get(deptId, incidentId, summaryNo, asccesor);
			if (dv.Count == 0) return null;

			return new CsIncidentSummaryValueModel(dv[0]);
		}
		#endregion

		#region +GetList リスト取得
		/// <summary>
		/// リスト取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>リスト</returns>
		public CsIncidentSummaryValueModel[] GetList(string deptId, string incidentId)
		{
			var dv = this.Repository.GetList(deptId, incidentId);
			return (from DataRowView drv in dv select new CsIncidentSummaryValueModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(CsIncidentSummaryValueModel model, SqlAccessor accessor)
		{
			this.Repository.Register(model.DataSource, accessor);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(CsIncidentSummaryValueModel model, SqlAccessor accessor)
		{
			this.Repository.Update(model.DataSource, accessor);
		}
		#endregion

		#region +DeleteAll 全て削除
		/// <summary>
		/// 全て削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="asccesor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string incidentId, SqlAccessor asccesor)
		{
			this.Repository.DeleteAll(deptId, incidentId, asccesor);
		}
		#endregion
	}
}
