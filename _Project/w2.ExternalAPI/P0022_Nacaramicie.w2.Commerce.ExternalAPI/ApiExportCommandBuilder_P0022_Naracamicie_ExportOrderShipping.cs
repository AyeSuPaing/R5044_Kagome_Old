/*
=========================================================================================================
  Module      : ナラカミ 受注配送情報出力クラス(ApiExportCommandBuilder_P0022_Naracamicie_ExportOrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;
using System.Data;

namespace P0022_Naracamicie.w2.Commerce.ExternalAPI
{
    public class ApiExportCommandBuilder_P0022_Naracamicie_ExportOrderShipping : ApiExportCommandBuilder
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
            var cmd = new GetOrderShippings();

            var getOrderShippingsArg = new GetOrderShippingsArg
                    {
						CreatedTimeSpan = new PastAbsoluteTimeSpan(0,
															DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])),
															DateTime.Now),
						OrderPaymentStatus = (string.IsNullOrEmpty(Properties["PaymentStatus"]) || Properties["PaymentStatus"] == "true") ?
										"" : Properties["PaymentStatus"], // 入金ステータス
						OrderStatus = Properties["OrderStatus"], // 注文ステータス
						OrderExtendedStatusSpecification =  // 連携フラグがOFFかつ連携作業中フラグがON
							OrderExtendedStatusSpecification.GenByString(string.Format("({0}F & {1}T)", Properties["IntgFlag"], Properties["IntgWorkingFlag"]))
                    };

            // コマンド実行
            var getOrderShippingsResult = (GetOrderShippingsResult)cmd.Do(getOrderShippingsArg);

            return getOrderShippingsResult.ResultTable;
        }
        #endregion
    }
}
