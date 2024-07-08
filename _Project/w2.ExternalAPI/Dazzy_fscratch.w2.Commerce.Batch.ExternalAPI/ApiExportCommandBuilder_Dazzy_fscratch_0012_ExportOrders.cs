/*
=========================================================================================================
  Module      : 注文ID及びステータスの指定は無しで、1日前から5前の間に作成された注文情報(ApiExportCommandBuilder_Dazzy_fscratch_0012_ExportOrders.cs)
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
	/// 注文ID及びステータスの指定は無しで、1日前から5前の間に作成された注文情報を取得する汎用コマンドビルダ
	/// </summary>
	public class ApiExportCommandBuilder_Dazzy_fscratch_0012_ExportOrders : ApiExportCommandBuilder
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
				record["OrderDate"],
				record["DateUpdated"],
				record["OrderStatus"],
				record["PaymentStatus"],
				record["ItemCount"],
				record["ProductCount"],
				record["PriceSubtotal"].ToPriceString(),
				"" , // PricePack
				"" , // PriceTax
				record["ShippingFee"].ToPriceString(),
				record["PaymentFee"].ToPriceString(),
				record["PriceOther"].ToPriceString(),
				record["MemberRankDiscountPrice"].ToPriceString(),
				record["PointUse"],
				record["PointUseYen"].ToPriceString(),
				record["CouponUse"].ToPriceString(),
				record["PriceTotal"].ToPriceString(),
				record["PaymentID"],
				record["CardTransactionID"],
				record["UserID"],
				record["GiftFlg"],
				record["Memo"],
				record["ManagementMemo"],
				record["APIMemo"],
				record["RegulationMemo"],
				record["MallId"],
				record["SetpromotionProductDiscount"].ToPriceString(),
				record["SetpromotionShippingChargeDiscount"].ToPriceString(),
				record["SetpromotionPaymentChargeDiscount"].ToPriceString()
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
			var cmd = new GetOrders();

			var arg = new GetOrdersArg
			{
				// 注文情報作成日　開始時間、終了時間を指定
				CreatedTimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddDays(-1).AddMinutes(-5)),
				// 注文情報更新日は指定なし
				// 注文ステータス
				OrderStatus = "",

				// 注文ID
				OrderId = ""
			};

			// コマンド実行
			var result = (GetOrdersResult)cmd.Do(arg);

			return result.ResultTable;
		}
		#endregion
	}
}