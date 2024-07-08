/*
=========================================================================================================
  Module      : コマンドインターフェース(ICommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Reauth.Commands
{
	/// <summary>
	/// コマンドのインターフェース
	/// </summary>
	interface ICommand
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功件数/全体件数</returns>
		Tuple<int, int> Exec(UpdateHistoryAction updateHistoryAction);

		/// <summary>文字列表現取得</summary>
		string ToString();
	}
}
