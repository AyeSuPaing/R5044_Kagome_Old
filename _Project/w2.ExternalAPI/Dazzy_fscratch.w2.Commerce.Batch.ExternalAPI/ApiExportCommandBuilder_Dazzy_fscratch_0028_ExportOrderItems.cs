/*
=========================================================================================================
  Module      : 注文ID及びステータスの指定は無しで、5分前から1日前の間に作成された注文商品情報(ApiExportCommandBuilder_Dazzy_fscratch_0028_ExportOrderItems.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;

namespace Dazzy_fscratch.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 注文ID及びステータスの指定は無しで、5分前から1日前の間に作成された注文商品情報を取得する汎用コマンドビルダ
	/// </summary>
	public class ApiExportCommandBuilder_Dazzy_fscratch_0028_ExportOrderItems : ApiExportCommandBuilder
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
				record["ItemNo"],
				record["ProductID"],
				record["VariationID"],
				record["CooperationID1"],
				"", // VariationCooperationID1
				record["CooperationID2"],
				"", // VariationCooperationID2
				record["CooperationID3"],
				"", // VariationCooperationID3
				record["CooperationID4"],
				"", // VariationCooperationID4
				record["CooperationID5"],
				"", // VariationCooperationID5
				record["ProductName"],
				record["BrandID"],
				"", // SupplierID
				record["ProductNameKana"],
				record["ProductPrice"].ToPriceString(),
				record["ProductPriceOrg"].ToPriceString(),
				"" , // TaxRate
				"" , // PricePreTax
				"" , // PriceShip
				"" , // PriceCost
				record["ItemQuantity"],
				record["ItemPrice"].ToPriceString(),
				record["OptionText"],
				record["ProductSaleID"],
				record["DownloadUrl"]
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
			var cmd = new GetOrderItems();

			var arg = new GetOrderItemsArg
			{
				// 注文情報作成日　開始時間、終了時間を指定
				CreatedTimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddDays(-1).AddMinutes(-5)),
				// 注文ステータス
				OrderStatus = "",

				// 注文ID
				OrderId = ""
			};

			// コマンド実行
			var result = (GetOrderItemsResult)cmd.Do(arg);

			return result.ResultTable;
		}
		#endregion
	}
}