/*
=========================================================================================================
  Module      : ナラカミ 在庫情報入力クラス(ApiImportCommandBuilder_P0022_Naracamicie_ImportStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;

namespace P0022_Naracamicie.w2.Commerce.ExternalAPI
{

    public class ApiImportCommandBuilder_P0022_Naracamicie_ImportStock : ApiImportCommandBuilder
    {

        #region #Import インポート処理の実装
        /// <summary>
        /// インポート処理の実装
        /// </summary>
        /// <param name="apiEntry"></param>
        protected override void Import(ApiEntry apiEntry)
        {
			//分割したデータを元にコマンド用引数クラス生成
            AddStockQuantityArg addStockQuantityArg = new AddStockQuantityArg
            {
                ProductID = (string)apiEntry.Data[0],
                VariationID = (string)apiEntry.Data[1],
                Stock = Convert.ToInt32(apiEntry.Data[2])
            };

			// コマンド実行
            AddStockQuantity addStockQuantity = new AddStockQuantity();
            AddStockQuantityResult addStockQuantityResult = (AddStockQuantityResult)addStockQuantity.Do(addStockQuantityArg);
        }
        #endregion
    }
}
