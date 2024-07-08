/*
=========================================================================================================
  Module      : ナラカミ 受注情報出力クラス(ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;

namespace P0022_Naracamicie.w2.Commerce.ExternalAPI
{
	class ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders : ApiExportCommandBuilder
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

			if(Properties.ContainsKey("OrderID"))
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
			DataTable orderItem = GetOrderItem(record["OrderID"].ToString());

			return new object[]{
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
					+ (isW2Gift(orderItem) ? GetGiftPrice(orderItem) : 0)
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
				isGift(record) || isW2Gift(orderItem),
				string.Empty, // ユーザー用メモ(Memo)は空白で出力する
				record["ManagementMemo"],
				string.Empty, // 外部連携メモ(ApiMemo)は空白で出力する
				record["MallId"]
			};
		}

		/// <summary>
		/// ギフトかどうかを判定
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private bool isGift(IDataRecord record)
		{
			bool result = false;

			switch (record["MallId"].ToString())
			{
				case "bidders1":
					result = isBiddersGift(record["APIMemo"].ToString());
					break;
				case "rakuten1":
					result = isRakutenGift(record["APIMemo"].ToString());
					break;
				case "yahoo_1":
					result = isYahooGift(record["RegulationMemo"].ToString());
					break;
			}

			return result;
		}

		/// <summary>
		/// ギフト商品の価格を取得
		/// </summary>
		/// <param name="orderItem"></param>
		/// <returns></returns>
		public decimal GetGiftPrice(DataTable orderItem)
		{
			foreach (DataRow row in orderItem.Rows)
			{
				if (row["ProductID"].ToString() == CustomConstants.GIFT_WRAPPING_PRODUCT_ID)
					return (decimal)row["ProductPrice"];
			}
			return 0;
		}

		/// <summary>
		/// 注文商品情報取得
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public DataTable GetOrderItem(string orderId)
		{
			GetOrderItems cmd = new GetOrderItems();

			GetOrderItemsArg getOrderItemsArg = new GetOrderItemsArg()
			{
				CreatedTimeSpan = new PastAbsoluteTimeSpan(0,
															DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])),
															DateTime.Now),
				OrderId = orderId
			};

			// コマンド実行
			return ((GetOrderItemsResult)cmd.Do(getOrderItemsArg)).ResultTable;
		}

		/// <summary>
		/// ｗ２のギフトかどうか
		/// </summary>
		/// <param name="giftFlag"></param>
		/// <returns></returns>
		public bool isW2Gift(DataTable orderItem)
		{
			foreach(DataRow row in orderItem.Rows)
			{
				 if (row["ProductID"].ToString() == CustomConstants.GIFT_WRAPPING_PRODUCT_ID
					 || row["ProductID"].ToString() == CustomConstants.GIFT_NO_WRAPPING_PRODUCT_ID)
					 return true;
			}
			return false;
		}

		/// <summary>
		/// 楽天ギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool isRakutenGift(string apiMemo)
		{
			return apiMemo.Contains("[ラッピング]");
		}

		/// <summary>
		/// ビッダーズのギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool isBiddersGift(string apiMemo)
		{
			return apiMemo.Contains("[ラッピング]ラッピングする");
		}

		/// <summary>
		/// ヤフーのギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool isYahooGift(string regulationMemo)
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
			var arg = new UpdateOrderExtendedStatusArg(objects[0].ToString(),int.Parse(Properties["IntgWorkingFlag"]), true);
			cmd.Do(arg);
		}
		#endregion
	}
}

