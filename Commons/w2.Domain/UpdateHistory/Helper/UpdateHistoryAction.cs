/*
=========================================================================================================
  Module      : 更新履歴アクション種別(UpdateHistoryAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.UpdateHistory.Helper
{
	/// <summary>更新履歴アクション種別</summary>
	public enum UpdateHistoryAction
	{
		/// <summary>実行</summary>
		Insert,
		/// <summary>実行しない</summary>
		DoNotInsert,
	}
}
