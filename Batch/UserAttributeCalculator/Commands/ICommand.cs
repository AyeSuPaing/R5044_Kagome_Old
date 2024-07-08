/*
=========================================================================================================
  Module      : コマンドのインターフェース(ICommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.Batch.UserAttributeCalculator.Commands
{
	/// <summary>
	/// コマンドのインターフェース
	/// </summary>
	interface ICommand
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>処理件数</returns>
		int Exec(DateTime targetStart, DateTime targetEnd);

		/// <summary>文字列表現取得</summary>
		string ToString();
	}
}
