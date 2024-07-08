/*
=========================================================================================================
  Module      : メッセージレポートリポジトリ(MessageReportRepository.cs)
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

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// メッセージレポートリポジトリ
	/// </summary>
	public class MessageReportRepository : ReportRepositoryBase
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MessageReportRepository()
			: base("CsReportMessage")
		{
		}
		#endregion
	}
}
