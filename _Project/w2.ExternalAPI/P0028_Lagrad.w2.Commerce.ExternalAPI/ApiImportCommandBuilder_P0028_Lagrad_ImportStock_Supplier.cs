/*
=========================================================================================================
  Module      : P0028_Lagrad用店舗側（サプライア）のImportクラス(ApiImportCommandBuilder_P0028_Lagrad_ImportStock_Supplier.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;

namespace P0028_Lagrad.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// 店舗側（サプライア）のImportコマンド
	/// </summary>
	/// <remarks>取込ファイルを元に相対値で更新</remarks>
	public class ApiImportCommandBuilder_P0028_Lagrad_ImportStock_Supplier : ApiImportCommandBuilder
	{
		/// <summary>
		/// Import実行
		/// </summary>
		/// <param name="apiEntry"></param>
		protected override void Import(ApiEntry apiEntry)
		{
			// 分割データから必要な値とってくる
			string productID = (string)apiEntry.Data[0];
			string variationID = (string)apiEntry.Data[1];
			int stock = Convert.ToInt32(apiEntry.Data[2]);
			// 在庫数（相対値）更新
			// 在庫履歴の同期フラグは同期済の状態で作成
			StockCommon stockCommon = new StockCommon();
			stockCommon.AddStockQuantity(productID, variationID, stock, true);
		}
	}
}
