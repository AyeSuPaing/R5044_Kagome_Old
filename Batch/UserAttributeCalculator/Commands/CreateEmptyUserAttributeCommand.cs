/*
=========================================================================================================
  Module      : 空のユーザー属性作成コマンド(CreateEmptyUserAttributeCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.Commerce.Batch.UserAttributeCalculator.Commands
{
	/// <summary>
	/// 空のユーザー属性作成コマンド
	/// </summary>
	public class CreateEmptyUserAttributeCommand : CommandBase
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">未利用（ダミー）</param>
		/// <param name="targetEnd">未利用（ダミー）</param>
		/// <returns>処理件数</returns>
		public override int Exec(DateTime targetStart, DateTime targetEnd)
		{
			ConsoleLogger.WriteInfo(@" 空のユーザー属性作成...", this.ToString());

			var result = new UserService().CreateEmptyUserAttribute(Constants.FLG_LASTCHANGED_BATCH);

			ConsoleLogger.WriteInfo(@"  -> {0}件の処理が完了しました。", StringUtility.ToNumeric(result));

			return result ? 1 : 0;
		}
	}
}
