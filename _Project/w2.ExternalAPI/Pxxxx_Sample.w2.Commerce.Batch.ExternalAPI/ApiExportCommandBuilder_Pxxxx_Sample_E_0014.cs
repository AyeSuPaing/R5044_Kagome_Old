﻿using System;
using System.Data;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;

using w2.ExternalAPI.Common.Export;
using w2.App.Common.Util;
namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiExportCommandBuilder_Pxxxx_Sample_E_0014 : ApiExportCommandBuilder
	{
		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			return record.ToArray();
		}
		#endregion

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
    	{
			//APIコマンド作る
			GetOrders cmd = new GetOrders();

			GetOrdersArg arg = new GetOrdersArg
			                   	{
									// 注文情報作成日　開始時間、終了時間を指定
									CreatedTimeSpan = new PastAbsoluteTimeSpan(-5,
																			   DateTime.Parse("2012-02-16 17:21:55.000"),
																			   DateTime.Parse("2012-02-17 10:42:07.220")),
									// 注文ステータス
									OrderStatus = "ODR",
			                   	};

			// コマンド実行
			GetOrdersResult result = (GetOrdersResult)cmd.Do(arg);

			return result.ResultTable;
    	}
		#endregion
	}
}