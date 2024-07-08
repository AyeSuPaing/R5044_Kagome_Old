/*
=========================================================================================================
  Module      : P0028_Lagrad用モール側のImportクラス(ApiImportCommandBuilder_P0028_Lagrad_ImportStock_Mall.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;

namespace P0028_Lagrad.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// モール側のImportコマンド
	/// </summary>
	/// <remarks>取込ファイルを元に在庫絶対数を更新</remarks>
	public class ApiImportCommandBuilder_P0028_Lagrad_ImportStock_Mall : ApiImportCommandBuilder
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
			// 在庫数（絶対値）更新
			// 在庫履歴の同期フラグは同期済の状態で作成
			StockCommon stockCommon = new StockCommon();
			stockCommon.SetStockQuantity(productID, variationID, stock, true);
		}
	}
}
