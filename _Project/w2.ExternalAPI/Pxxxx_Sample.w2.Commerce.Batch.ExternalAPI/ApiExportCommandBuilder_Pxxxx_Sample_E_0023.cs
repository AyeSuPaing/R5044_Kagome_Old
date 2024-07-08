﻿using System;
using System.Data;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;

using w2.ExternalAPI.Common.Export;
using w2.App.Common.Util;
namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiExportCommandBuilder_Pxxxx_Sample_E_0023 : ApiExportCommandBuilder
	{
		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
    	{
			//APIコマンド作る
			GetOrderItems cmd = new GetOrderItems();

			GetOrderItemsArg arg = new GetOrderItemsArg
			                   		{
										// 注文情報作成日　開始時間、終了時間を指定
										CreatedTimeSpan = new PastAbsoluteTimeSpan(-5,
																				   DateTime.Parse("2012-02-16 17:21:55.000"),
																				   DateTime.Parse("2012-02-17 10:42:07.220")),
										// 注文ID
										OrderId = "2012021600006"
			                   		};

			// コマンド実行
			GetOrderItemsResult result = (GetOrderItemsResult)cmd.Do(arg);

			return result.ResultTable;
    	}
		#endregion

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			return record.ToArray();
		}
		#endregion
	}
}
