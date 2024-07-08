/*
=========================================================================================================
  Module      : メッセージレポートサービス(MessageReportService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.App.Common.Cs.CsOperator;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// メッセージレポートサービス
	/// </summary>
	public class MessageReportService : ReportServiceBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public MessageReportService(MessageReportRepository repository)
			: base(repository)
		{
		}
		#endregion
	}
}
