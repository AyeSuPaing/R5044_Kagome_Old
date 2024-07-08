/*
=========================================================================================================
  Module      : コマンド基底クラス(CommandBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Reauth.Commands
{
	/// <summary>
	/// コマンド基底クラス
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功件数/全体件数</returns>
		public abstract Tuple<int, int> Exec(UpdateHistoryAction updateHistoryAction);

		/// <summary>文字列表現取得</summary>
		public override string ToString()
		{
			return this.GetType().Name;
		}
	}
}
