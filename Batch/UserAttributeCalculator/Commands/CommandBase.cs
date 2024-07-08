/*
=========================================================================================================
  Module      : コマンド基底クラス(CommandBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Commerce.Batch.UserAttributeCalculator.Commands
{
	/// <summary>
	/// コマンド基底クラス
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>処理件数</returns>
		public abstract int Exec(DateTime targetStart, DateTime targetEnd);

		/// <summary>文字列表現取得</summary>
		public override string ToString()
		{
			return this.GetType().Name;
		}
	}
}
