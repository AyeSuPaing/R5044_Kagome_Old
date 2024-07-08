/*
=========================================================================================================
  Module      : CPMレポート計算コマンド(CreateCpmReportCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using w2.Common.Util;
using w2.Domain.DispSummaryAnalysis;
using w2.Domain.User;
using w2.Domain.User.Helper;

namespace w2.Commerce.Batch.UserAttributeCalculator.Commands
{
	/// <summary>
	/// CPMレポート計算コマンド
	/// </summary>
	class CreateCpmReportCommand : CommandBase
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">前回実行日時</param>
		/// <param name="targetEnd">今回実行日時</param>
		/// <returns>1（固定）</returns>
		public override int Exec(DateTime targetStart, DateTime targetEnd)
		{
			// 現在のレポート取得
			var report = new UserService().GetUserCpmClusterReport(Constants.CPM_CLUSTER_SETTINGS);

			// 登録
			new DispSummaryAnalysisService().RegisterForCpmReport(Constants.CONST_DEFAULT_DEPT_ID, DateTime.Now, report);

			return 1;
		}
	}
}
