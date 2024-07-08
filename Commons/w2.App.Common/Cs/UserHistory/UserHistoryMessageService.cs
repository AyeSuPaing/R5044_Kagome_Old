/*
=========================================================================================================
  Module      : ユーザーメッセージ履歴サービスクラス(UserHistoryMessageService.cs)
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
	///ユーザーメッセージ履歴サービスクラス
	/// </summary>
	public class UserHistoryMessageService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">レポジトリ</param>
		public UserHistoryMessageService(UserHistoryMessageRepository repository)
		{
			this.Repository = repository;
		}

		/// <summary>
		/// ユーザー履歴（メッセージ）取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>履歴一覧</returns>
		public UserHistoryBase[] GetList(string userId)
		{
			return (from DataRowView drv in this.Repository.GetUserHistoryMessages(userId) select new UserHistoryMessage(drv)).ToArray();
		}

		/// <summary>レポジトリ</summary>
		private UserHistoryMessageRepository Repository;
	}
}