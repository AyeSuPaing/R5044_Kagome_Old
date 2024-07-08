/*
=========================================================================================================
  Module      : 注文ID及びステータスの指定は無しで、5前から1日前の間に作成された注文配送先情報(ApiExportCommandBuilder_Dazzy_fscratch_0020_ExportOrderShippings.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;

namespace Dazzy_fscratch.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 注文ID及びステータスの指定は無しで、5前から1日前の間に作成された注文配送先情報を取得する汎用コマンドビルダ
	/// </summary>
	public class ApiExportCommandBuilder_Dazzy_fscratch_0020_ExportOrderShippings : ApiExportCommandBuilder
	{
		#region #Export 出力処理
		/// <summary>
		/// 出力処理
		/// </summary>
		/// <param name="record">出力レコード</param>
		/// <returns>出力内容</returns>
		protected override object[] Export(IDataRecord record)
		{
			var result = new []{
				record["OrderID"],
				record["ShippingNo"],
				record["SenderName1"],
				record["SenderName2"],
				record["SenderNameKana1"],
				record["SenderNameKana2"],
				record["SenderCompanyName"],
				record["SenderCompanyPostName"],
				record["SenderZip"],
				record["SenderAddr1"],
				record["SenderAddr2"],
				record["SenderAddr3"],
				record["SenderAddr4"],
				record["SenderTel"],
				record["ShippingName1"],
				record["ShippingName2"],
				record["ShippingNameKana1"],
				record["ShippingNameKana2"],
				record["ShippingCompanyName"],
				record["ShippingCompanyPostName"],
				record["ShippingZip"],
				record["ShippingAddr1"],
				record["ShippingAddr2"],
				record["ShippingAddr3"],
				record["ShippingAddr4"],
				record["ShippingTel"],
				"" , // ShippingCompany
				record["ShippingDate"],
				record["ShippingTime"],
				record["ShippingCheckNo"],
				record["WrappingPaperType"],
				record["WrappingPaperName"],
				record["WrappingBagType"]
			};
			return result;
		}
		#endregion

		#region #GetDataTable テーブル取得
		/// <summary>
		/// テーブル取得
		/// </summary>
		/// <returns>テーブル内容</returns>
		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			var cmd = new GetOrderShippings();

			var arg = new GetOrderShippingsArg
			{
				// 注文情報作成日　開始時間、終了時間を指定
				CreatedTimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddDays(-1).AddMinutes(-5)),
				// 注文ステータス
				OrderStatus = "",

				// 注文ID
				OrderId = ""
			};

			// コマンド実行
			var result = (GetOrderShippingsResult)cmd.Do(arg);

			return result.ResultTable;
		}
		#endregion
	}
}