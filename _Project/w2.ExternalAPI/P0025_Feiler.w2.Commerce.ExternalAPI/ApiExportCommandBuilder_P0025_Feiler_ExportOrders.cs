/*
=========================================================================================================
  Module      : Feiler 受注情報出力クラス(ApiExportCommandBuilder_P0025_Feiler_ExportOrders.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	class ApiExportCommandBuilder_P0025_Feiler_ExportOrders : ApiExportCommandBuilder
	{
		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			var cmd = new GetOrders();

			var getOrdersArg = new GetOrdersArg
			{
				CreatedTimeSpan = new PastAbsoluteTimeSpan(0,
															DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])),
															DateTime.Now),
				OrderPaymentStatus = (string.IsNullOrEmpty(Properties["PaymentStatus"]) || Properties["PaymentStatus"] == "true") ?
										"" : Properties["PaymentStatus"], // 入金ステータス
				OrderStatus = Properties["OrderStatus"], // 注文ステータス
				OrderExtendedStatusSpecification =  // 連携フラグ、連携作業中フラグともにOFF
					OrderExtendedStatusSpecification.GenByString(string.Format("({0}F & {1}F)", Properties["IntgFlag"], Properties["IntgWorkingFlag"]))
			};

			if (Properties.ContainsKey("OrderID"))
			{
				getOrdersArg.OrderId = Properties["OrderID"];
			}

			// コマンド実行
			var getOrdersResult = (GetOrdersResult)cmd.Do(getOrdersArg);

			return getOrdersResult.ResultTable;
		}
		#endregion

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			DataTable orderItem = ApiCommon.GetOrderItem(record["OrderID"].ToString(), new PastAbsoluteTimeSpan(0, DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])), DateTime.Now));
			DataRowView orderData = OrderCommon.GetOrder(StringUtility.ToEmpty(record["OrderID"]))[0];

			return new []{
				record["OrderID"],
				record["OrderDate"],
				record["DateUpdated"],
				record["OrderStatus"],
				record["PaymentStatus"],
				record["ItemCount"],
				record["ProductCount"],
				record["PriceSubtotal"],
				record["PricePack"],
				record["PriceTax"],
				record["ShippingFee"],
				record["PaymentFee"],
				(decimal)record["PriceOther"]
					+ (IsW2Gift(orderItem) ? GetGiftPrice(orderItem) : 0)
					- (decimal)record["SetpromotionProductDiscount"]
					- (decimal)record["SetpromotionShippingChargeDiscount"]
					- (decimal)record["SetpromotionPaymentChargeDiscount"], // セットプロモーション割引額は＋の値のため、－する。
				record["MemberRankDiscountPrice"],
				record["PointUse"],
				record["PointUseYen"],
				record["CouponUse"],
				record["PriceTotal"],
				record["PaymentID"],
				record["CardTransactionID"],
				record["UserID"],
				IsW2Gift(orderItem) || IsGift(record),
				record["Memo"].ToString().Length > 200 
					? record["Memo"].ToString().Substring(0, 200).Replace("\r","").Replace("\n","")
					: record["Memo"].ToString().Replace("\r","").Replace("\n",""),										// メモ(Memo)
				record["ManagementMemo"].ToString().Length > 30 
					? record["ManagementMemo"].ToString().Substring(0, 30).Replace("\r","").Replace("\n","")
					: record["ManagementMemo"].ToString().Replace("\r","").Replace("\n",""),								// Management Memo
				string.Empty,												// 外部連携メモ(ApiMemo)は空白で出力する
				record["MallId"],											// Mall ID
				orderData["payment_memo"].ToString().Length > 200 
					? orderData["payment_memo"].ToString().Substring(0, 200).Replace("\r","").Replace("\n","")
					: orderData["payment_memo"].ToString().Replace("\r","").Replace("\n","")								// Payment Memo
			};
		}

		/// <summary>
		/// ギフトかどうかを判定
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private bool IsGift(IDataRecord record)
		{
			switch (record["MallId"].ToString())
			{
				case CustomConstants.FLG_ORDER_MALL_ID_RAKUTEN:
					return IsRakutenGift(record["APIMemo"].ToString());

				case CustomConstants.FLG_ORDER_MALL_ID_YAHOO:
					return IsYahooGift(record["RegulationMemo"].ToString());

				default:
					return false;
			}
		}

		/// <summary>
		/// ギフト商品の価格を取得
		/// </summary>
		/// <param name="orderItem"></param>
		/// <returns></returns>
		public decimal GetGiftPrice(DataTable orderItem)
		{
			return (from DataRow row in orderItem.Rows
			        where row["ProductID"].ToString() == CustomConstants.GIFT_WRAPPING_PRODUCT_ID
			        select (decimal) row["ProductPrice"]).FirstOrDefault();
		}

		/// <summary>
		/// ｗ２のギフトかどうか
		/// </summary>
		/// <param name="orderItem"></param>
		/// <returns></returns>
		public bool IsW2Gift(DataTable orderItem)
		{
			return orderItem.Rows.Cast<DataRow>().Any(row => row["ProductID"].ToString().EndsWith(CustomConstants.FLG_SUFFIX_PRODUCT_ID));
		}

		/// <summary>
		/// 楽天ギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool IsRakutenGift(string apiMemo)
		{
			return apiMemo.Contains("[ラッピング]");
		}

		/// <summary>
		/// ヤフーのギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool IsYahooGift(string regulationMemo)
		{
			return regulationMemo.Contains("－－ギフト手数料－－");
		}
		#endregion

		#region #Switch flags
		/// <summary>
		/// 出力開始のため、作業中フラグをONに設定する
		/// </summary>
		/// <param name="objects"></param>
		public override void PostExport(object[] objects)
		{
			if (objects.Length == 0) return;

			var cmd = new UpdateOrderExtendedStatus();
			var arg = new UpdateOrderExtendedStatusArg(objects[0].ToString(), int.Parse(Properties["IntgWorkingFlag"]), true);
			cmd.Do(arg);
		}
		#endregion
	}
}

