/*
=========================================================================================================
  Module      : ユーザー注文履歴リポジトリクラス(UserHistoryOrderRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.Common.Sql;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー注文履歴リポジトリクラス
	/// </summary>
	public class UserHistoryOrderRepository
	{
		private const string XML_KEY_NAME = "CsUserSearch";

		/// <summary>
		/// ユーザー履歴（注文）取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>履歴一覧</returns>
		public DataView GetUserHistoryOrders(string userId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetUserHistoryOrders"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_USER_USER_ID, userId);

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}
	}
}