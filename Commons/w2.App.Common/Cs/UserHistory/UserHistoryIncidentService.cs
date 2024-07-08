/*
=========================================================================================================
  Module      : ユーザーインシデント履歴サービスクラス(UserHistoryIncidentService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	///ユーザーインシデント履歴サービスクラス
	/// </summary>
	public class UserHistoryIncidentService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">レポジトリ</param>
		public UserHistoryIncidentService(UserHistoryIncidentRepository repository)
		{
			this.Repository = repository;
		}

		/// <summary>
		/// ユーザー履歴（インシデント）取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>履歴一覧</returns>
		public UserHistoryBase[] GetList(string userId)
		{
			List<UserHistoryIncident> list = new List<UserHistoryIncident>();
			var historyIncidents = this.Repository.GetUserHistoryIncidents(userId);
			foreach (DataRowView drv in historyIncidents)
			{
				if ((list.Count == 0) || (list[list.Count - 1].IncidentId != (string)drv[Constants.FIELD_CSINCIDENT_INCIDENT_ID]))
				{
					list.Add(new UserHistoryIncident(drv));
					list.Last().SetLastMessageInfo(drv);
				}
				else
				{
					list.Last().SetLastMessageInfo(drv);
				}
			}
			return list.ToArray();
		}

		/// <summary>レポジトリ</summary>
		private UserHistoryIncidentRepository Repository;
	}
}