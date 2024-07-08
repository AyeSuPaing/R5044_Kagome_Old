/*
=========================================================================================================
  Module      : CPMクラスタ計算コマンド(CalculateCpmClusterCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.Commerce.Batch.UserAttributeCalculator.Commands
{
	/// <summary>
	/// CPMクラスタ計算コマンド
	/// </summary>
	class CalculateCpmClusterCommand : CommandBase
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">前回実行日時</param>
		/// <param name="targetEnd">今回実行日時</param>
		/// <returns>更新したユーザー件数</returns>
		public override int Exec(DateTime targetStart, DateTime targetEnd)
		{
			var service = new UserService();
			var userIds = service.GetUserIdsForSetCpmCluster(targetStart, targetEnd, Constants.CPM_CLUSTER_SETTINGS);
			var totalCount = userIds.Length;
			ConsoleLogger.WriteInfo(@" CPMクラスタセット 対象ユーザー：{0}件", StringUtility.ToNumeric(totalCount));

			var resultCount = 0;
            foreach (var userId in userIds)
			{
				service.CalculateCpmClusterAndSave(userId, Constants.CPM_CLUSTER_SETTINGS, Constants.FLG_LASTCHANGED_BATCH);
				resultCount++;

				Console.WriteLine(@"  ユーザーID：{0} 完了 ({1}/{2})", userId, StringUtility.ToNumeric(resultCount), StringUtility.ToNumeric(totalCount));
			}
			ConsoleLogger.WriteInfo(@"  -> {0}件の処理が完了しました。 ※クラスタ変更のないものは更新していません。", StringUtility.ToNumeric(resultCount));
			return resultCount;
		}
	}
}
