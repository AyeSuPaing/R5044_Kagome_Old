/*
=========================================================================================================
  Module      : API引数の抽象クラス(ApiCommandArg.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Util;

namespace w2.ExternalAPI.Common.Command
{
	/// <summary>
	///	API引数の抽象クラス
	/// </summary>
	/// <remarks>
	/// 各コマンドで利用する引数はこのクラスを実装すること
	/// </remarks>
	public abstract class ApiCommandArgBase
	{
		#region +Validate 作成日チェック
		/// <summary>
		/// 作成日チェック
		/// </summary>
		public void Validate(PastAbsoluteTimeSpan createdTimeSpan)
		{
			if (createdTimeSpan == null)
			{
				throw new Exception("作成日の指定が必要です。");
			}
		}
		#endregion

		#region +Validate 作成日、更新日チェック
		/// <summary>
		/// 作成日、更新日チェック
		/// </summary>
		public void Validate(PastAbsoluteTimeSpan createdTimeSpan, PastAbsoluteTimeSpan updatedTimeSpan)
		{
			if (createdTimeSpan == null && updatedTimeSpan == null)
			{
				throw new Exception("作成日と更新日どちらかの指定が必要です。");
			}
			if (createdTimeSpan != null && updatedTimeSpan != null)
			{
				throw new Exception("作成日と更新日の両方を指定はできません。");
			}
		}
		#endregion
	}
}
