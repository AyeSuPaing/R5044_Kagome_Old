/*
=========================================================================================================
  Module      : 注文系情報計算コマンド(CalculateOrderInfoCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.Commerce.Batch.UserAttributeCalculator.Commands
{
	/// <summary>
	/// 注文系情報計算コマンド
	/// </summary>
	class CalculateOrderInfoCommand : CommandBase
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>処理件数</returns>
		public override int Exec(DateTime targetStart, DateTime targetEnd)
		{
			var userService = new UserService();
			var userIds = userService.GetUserIdsByOrderChanged(targetStart, targetEnd);
			var totalCount = userIds.Length;
			ConsoleLogger.WriteInfo(@" 対象ユーザー：{0}件", StringUtility.ToNumeric(totalCount));

			var resultCount = 0;
			foreach (var userId in userIds)
			{
				resultCount++;

				var update = userService.InsertUpdateUserAttributeOrderInfo(userId, Constants.FLG_LASTCHANGED_BATCH);
				Console.WriteLine(@"  ユーザーID：{0} 完了 ({1}/{2})" + (update ? "" : "(更新なし)"), userId, StringUtility.ToNumeric(resultCount), StringUtility.ToNumeric(totalCount));
			}
			ConsoleLogger.WriteInfo(@"  -> {0}件の処理が完了しました。", StringUtility.ToNumeric(resultCount));

			return resultCount;
		}
	}
}
