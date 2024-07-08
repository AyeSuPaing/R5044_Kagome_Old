/*
=========================================================================================================
  Module      : ユーザーメッセージ履歴リポジトリクラス(UserHistoryMessageRepository.cs)
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
	/// ユーザーメッセージ履歴リポジトリクラス
	/// </summary>
	public class UserHistoryMessageRepository
	{
		private const string XML_KEY_NAME = "CsUserSearch";

		/// <summary>
		/// ユーザー履歴（メッセージ）取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>履歴一覧</returns>
		public DataView GetUserHistoryMessages(string userId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetUserHistoryMessages"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_USER_USER_ID, userId);

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}
	}
}