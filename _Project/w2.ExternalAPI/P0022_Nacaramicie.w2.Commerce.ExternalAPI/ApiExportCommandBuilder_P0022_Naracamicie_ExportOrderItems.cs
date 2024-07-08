/*
=========================================================================================================
  Module      : ナラカミ 受注商品情報出力クラス(ApiExportCommandBuilder_P0022_Naracamicie_ExportOrderItems.cs)
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
    public class ApiExportCommandBuilder_P0022_Naracamicie_ExportOrderItems : ApiExportCommandBuilder
    {
        #region #Export 出力処理
        /// <summary>出力処理</summary>
        protected override object[] Export(IDataRecord record)
        {
			// ギフト商品なら出力しない
			if (record["ProductID"].ToString() == CustomConstants.GIFT_NO_WRAPPING_PRODUCT_ID
				|| record["ProductID"].ToString() == CustomConstants.GIFT_WRAPPING_PRODUCT_ID)
				return null;

            return record.ToArray();
        }
        #endregion

        #region #Init 初期化処理
        /// <summary>初期化処理</summary>
        public override DataTable GetDataTable()
        {
            //APIコマンド作る
            GetOrderItems cmd = new GetOrderItems();

            GetOrderItemsArg getOrderItemsArg = new GetOrderItemsArg()
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
            GetOrderItemsResult getOrderItemsResult = (GetOrderItemsResult)cmd.Do(getOrderItemsArg);

            return getOrderItemsResult.ResultTable;
        }
        #endregion

        #region #Switch flags
        /// <summary>
        /// 書き込み完了したらフラグをたてる
        /// </summary>
        /// <param name="objects"></param>
        public override void PostExport(object[] objects)
        {
            if ((objects == null) || (objects.Length == 0)) return;
			
			// 連携フラグをONに
            var cmd = new UpdateOrderExtendedStatus();
			var arg = new UpdateOrderExtendedStatusArg(objects[0].ToString(), int.Parse(Properties["IntgFlag"]), true);
            cmd.Do(arg);

			// 連携フラグをOFFに
			arg = new UpdateOrderExtendedStatusArg(objects[0].ToString(), int.Parse(Properties["IntgWorkingFlag"]), false);
			cmd.Do(arg);
        }
        #endregion
    }
}
