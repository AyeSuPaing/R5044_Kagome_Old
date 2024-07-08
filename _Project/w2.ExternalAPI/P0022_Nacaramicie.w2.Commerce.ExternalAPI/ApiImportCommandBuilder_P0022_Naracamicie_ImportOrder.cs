/*
=========================================================================================================
  Module      : ナラカミ 受注情報入力クラス(ApiImportCommandBuilder_P0022_Naracamicie_ImportOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;
using System.Globalization;

namespace P0022_Naracamicie.w2.Commerce.ExternalAPI
{
    public class ApiImportCommandBuilder_P0022_Naracamicie_ImportOrder : ApiImportCommandBuilder
    {
        #region #Import インポート処理の実装
        /// <summary>
        /// インポート処理の実装
        /// </summary>
        /// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
        protected override void Import(ApiEntry apiEntry)
        {
            //分割したデータを元にコマンド用引数クラス生成
			ShipOrderArg shipOrderArg = GetArg(apiEntry);

			// コマンド実行
            ShipOrder shipOrder = new ShipOrder();
            ShipOrderResult shipOrderResult = (ShipOrderResult)shipOrder.Do(shipOrderArg);
        }

		/// <summary>
		/// 引数設定
		/// </summary>
		/// <param name="apiEntry"></param>
		/// <returns></returns>
		private static ShipOrderArg GetArg(ApiEntry apiEntry)
		{
			ShipOrderArg shipOrderArg = new ShipOrderArg
			{
				OrderId = apiEntry.Data[0].ToString(),
				ShippingNo = apiEntry.Data[1].ToString() == "" ? 1 : Convert.ToInt32(apiEntry.Data[1]),
				ShippingCheckNo = apiEntry.Data[2].ToString(),
				DoesMail = apiEntry.GetData<bool>(3) ?? true,
				ApiMemo = apiEntry.Data[4].ToString(),
				IsOverwriteMemo = apiEntry.GetData<bool>(5) ?? false,
				ShippedDate = apiEntry.Data[6].ToString() != "" ? Convert.ToDateTime(apiEntry.Data[6]) : DateTime.Today
			};
			return shipOrderArg;
		}
        #endregion
    }
}
