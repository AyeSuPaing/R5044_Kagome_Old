/*
=========================================================================================================
  Module      : 中川政七商店 在庫情報入力クラス(ApiImportCommandBuilder_P0045_Nakagawa_ImportStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;

namespace P0045_Nakagawa.w2.Commerce.ExternalAPI
{

	public class ApiImportCommandBuilder_P0045_Nakagawa_ImportStock : ApiImportCommandBuilder
	{

		#region #Import インポート処理の実装
		/// <summary>
		/// インポート処理の実装
		/// </summary>
		/// <param name="apiEntry"></param>
		protected override void Import(ApiEntry apiEntry)
		{
			int stock;
			if (Int32.TryParse((string)apiEntry.Data[1], out stock) == false) return;

			// 引数生成
			SetStockQuantityArg arg = new SetStockQuantityArg
			{
				ProductID = GetProductId((string)apiEntry.Data[0]),
				VariationID = (string)apiEntry.Data[0],
				Stock = stock
			};

			if (arg.ProductID == "") return;

			// コマンド実行
			SetStockQuantity cmd = new SetStockQuantity();
			SetStockQuantityResult result = (SetStockQuantityResult)cmd.Do(arg);
		}
		#endregion

		#region
		/// <summary>
		/// 商品ID取得
		/// </summary>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>商品ID</returns>
		private string GetProductId(string variationId)
		{
			if (Stocks == null)
			{
				// コマンド生成
				GetStockQuantitiesNow cmd = new GetStockQuantitiesNow();

				// 引数生成
				GetStockQuantitiesNowArg arg = new GetStockQuantitiesNowArg() { };

				// コマンド実行（全在庫情報取得）
				Stocks = ((GetStockQuantitiesNowResult)cmd.Do(arg)).ResultTable;
			}

			//System.Data.DataTableExtensions.AsEnumerable
			var products = Stocks.AsEnumerable().Where(stock => (string)stock["VariationID"] == variationId);

			if (products.Any())
			{
				return (string)products.First()["ProductID"];
			}

			return "";
		}
		#endregion

		private static DataTable Stocks { get; set; }
	}
}
