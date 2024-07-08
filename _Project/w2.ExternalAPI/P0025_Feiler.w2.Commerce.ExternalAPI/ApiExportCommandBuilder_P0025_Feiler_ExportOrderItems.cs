/*
=========================================================================================================
  Module      : Feiler 受注商品情報出力クラス(ApiExportCommandBuilder_P0025_Feiler_ExportOrderItems.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;
using System.Data;
using System.Linq;

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	public class ApiExportCommandBuilder_P0025_Feiler_ExportOrderItems : ApiExportCommandBuilder
	{
		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			// 数量がマイナスのものは除外する
			if ((int)record["ItemQuantity"] < 0) return null;

			DataTable orderItems = ApiCommon.GetOrderItem(record["OrderID"].ToString(),
														  new PastAbsoluteTimeSpan(0,
																				   DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])),
																				   DateTime.Now));

			string productId = record["ProductID"].ToString();
			bool isIgnoreGiftFlagProduct = productId.EndsWith(CustomConstants.FLG_SUFFIX_PRODUCT_ID)
											&& IsGiftFlagProduct(orderItems, productId.Remove(productId.Length - CustomConstants.FLG_SUFFIX_PRODUCT_ID.Length))
											&& CheckGiftFlagQuantity(record, orderItems);

			// ギフト商品なら出力しない
			if (productId == CustomConstants.GIFT_NO_WRAPPING_PRODUCT_ID
				|| productId == CustomConstants.GIFT_WRAPPING_PRODUCT_ID
				|| isIgnoreGiftFlagProduct)
				return null;

			DataRowView orderData = OrderCommon.GetOrder(StringUtility.ToEmpty(record["OrderID"]))[0];
			DataRow originalProduct = GetOriginalProduct(productId, record["VariationID"].ToString(), orderItems);
			int itemQuantityExport = GetItemQuantityExport(record, orderItems);

			return new []{
				record["OrderID"],
				record["ItemNo"],
				originalProduct["ProductID"],
				originalProduct["VariationID"],
				originalProduct["CooperationID1"],
				originalProduct["VariationCooperationID1"],
				originalProduct["CooperationID2"],
				originalProduct["VariationCooperationID2"],
				originalProduct["CooperationID3"],
				originalProduct["VariationCooperationID3"],
				originalProduct["CooperationID4"],
				originalProduct["VariationCooperationID4"],
				originalProduct["CooperationID5"],
				originalProduct["VariationCooperationID5"],
				originalProduct["ProductName"],
				originalProduct["BrandID"],
				originalProduct["SupplierID"],
				originalProduct["ProductNameKana"],
				originalProduct["ProductPrice"],
				originalProduct["ProductPriceOrg"],
				originalProduct["TaxRate"],
				originalProduct["PricePreTax"],
				originalProduct["PriceShip"],
				originalProduct["PriceCost"],
				itemQuantityExport,
				decimal.Parse(originalProduct["ProductPrice"].ToString()) * itemQuantityExport,
				originalProduct["OptionText"],
				originalProduct["ProductSaleID"],
				originalProduct["DownloadUrl"],
				GetWrappingBagType(orderData["memo"].ToString(),
								   orderData["mall_id"].ToString(),
								   IsGift(orderData) || IsW2Gift(orderItems),
								   GetProductIdForWrapping(record, orderItems),
								   orderItems)																			// Wrapping Bag Type
			};
		}
		#endregion

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			var cmd = new GetOrderItems();

			var getOrderItemsArg = new GetOrderItemsArg
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
			var getOrderItemsResult = (GetOrderItemsResult)cmd.Do(getOrderItemsArg);

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

		#region Get Wrapping Bag Type
		/// <summary>
		/// Gets the type of the wrapping bag.
		/// </summary>
		/// <param name="memo">The memo.</param>
		/// <param name="mallId">The mall id.</param>
		/// <param name="giftFlag">if set to <c>true</c> [gift flag].</param>
		/// <param name="productId">The product id.</param>
		/// <param name="orderItems">The order items.</param>
		/// <returns></returns>
		private string GetWrappingBagType(string memo, string mallId, bool giftFlag, string productId, DataTable orderItems)
		{
			// Get wrapping bag type of shipping (No.34)
			var wrappingBagType = ApiCommon.GetWrappingBagType(memo, mallId, giftFlag);

			if (wrappingBagType == CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_INDIVIDUAL				// 2：個別
				|| wrappingBagType == CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_OTHERS)				// 3：その他
			{
				wrappingBagType = (productId.EndsWith(CustomConstants.FLG_SUFFIX_PRODUCT_ID)) 
									&& IsGiftFlagProduct(orderItems, productId.Remove(productId.Length - CustomConstants.FLG_SUFFIX_PRODUCT_ID.Length))
												? CustomConstants.FLG_ORDERITEMS_WRAPPING_BAG_TYPE_VALID		// 有り
												: CustomConstants.FLG_ORDERITEMS_WRAPPING_BAG_TYPE_INVALID;		// 無し
			}

			return wrappingBagType;
		}
		#endregion

		#region Check Ignore Wrapping Flag Product
		/// <summary>
		/// Checks the gift flag quantity.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <param name="orderItems">The order items.</param>
		/// <returns></returns>
		private bool CheckGiftFlagQuantity(IDataRecord record, DataTable orderItems)
		{
			return ((int) record["ItemQuantity"] >= GetItemQuantity(GetOriginalProductId(record["ProductID"].ToString(), orderItems),
																	GetOriginalProductId(record["VariationID"].ToString(), orderItems), orderItems));
		}
		#endregion

		#region Gets Original Product Id
		/// <summary>
		/// Gets the original product id.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <param name="orderItems">The order items.</param>
		/// <returns></returns>
		private string GetOriginalProductId(string productId, DataTable orderItems)
		{
			string originalProductId = productId.EndsWith(CustomConstants.FLG_SUFFIX_PRODUCT_ID)
				                           ? productId.Substring(0, productId.Length - CustomConstants.FLG_SUFFIX_PRODUCT_ID.Length)
				                           : productId;

			return IsGiftFlagProduct(orderItems, originalProductId)
					? originalProductId
					: productId;
		}

		/// <summary>
		/// Determines whether [is gift flag product] [the specified order items].
		/// </summary>
		/// <param name="orderItems">The order items.</param>
		/// <param name="productId">The product id.</param>
		/// <returns></returns>
		private bool IsGiftFlagProduct(DataTable orderItems, string productId)
		{
			return orderItems.Rows.Cast<DataRow>().Any(row => (row["ProductID"].ToString() == productId) || (row["VariationID"].ToString() == productId));
		}

		#endregion

		#region Gets Original Product
		/// <summary>
		/// Gets the original Product.
		/// </summary>
		/// <param name="productId">The product ID.</param>
		/// <param name="variationId">The variation ID.</param>
		/// <param name="orderItems">The order items.</param>
		/// <returns></returns>
		private DataRow GetOriginalProduct(string productId, string variationId, DataTable orderItems)
		{
			return orderItems.Rows.Cast<DataRow>().FirstOrDefault(row => (row["ProductID"].ToString() == GetOriginalProductId(productId, orderItems))
																& row["VariationID"].ToString() == GetOriginalProductId(variationId, orderItems));
		}
		#endregion

		#region Get Item Quantity For Export
		/// <summary>
		/// Gets the item quantity for export.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <param name="orderItems">The order items.</param>
		/// <returns></returns>
		private int GetItemQuantityExport(IDataRecord record, DataTable orderItems)
		{
			string productId = record["ProductID"].ToString();
			string variationId = record["VariationID"].ToString();
			var quantity = (int)record["ItemQuantity"];
			var giftFlagQuantity = GetItemQuantity(productId + CustomConstants.FLG_SUFFIX_PRODUCT_ID, variationId + CustomConstants.FLG_SUFFIX_PRODUCT_ID, orderItems);
			if (giftFlagQuantity > quantity) giftFlagQuantity = quantity;

			bool isExistsGiftFlagProduct = orderItems.Rows.Cast<DataRow>().Any(row => (row["ProductID"].ToString() == (productId + CustomConstants.FLG_SUFFIX_PRODUCT_ID)));

			return ((isExistsGiftFlagProduct == false) || (quantity == giftFlagQuantity))
					   ? quantity
					   : quantity - giftFlagQuantity;
		}
		#endregion

		#region Get Item Quantity
		/// <summary>
		/// Get Item Quantity
		/// </summary>
		/// <param name="productId">The product ID.</param>
		/// <param name="variationId">The variation id.</param>
		/// <param name="orderItems">The order items</param>
		/// <returns></returns>
		private int GetItemQuantity(string productId, string variationId, DataTable orderItems)
		{
			return (from DataRow row in orderItems.Rows
					where (row["ProductID"].ToString() == productId) & (row["VariationID"].ToString() == variationId & (int)row["ItemQuantity"] > 0)
					select (int)row["ItemQuantity"]).FirstOrDefault();
		}
		#endregion

		#region Get Product ID For Check Wrapping
		/// <summary>
		/// Gets the product id for wrapping.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <param name="orderItems">The order items.</param>
		/// <returns></returns>
		private string GetProductIdForWrapping(IDataRecord record, DataTable orderItems)
		{
			string productId = record["ProductID"].ToString();
			string variationId = record["VariationID"].ToString();
			var quantity = (int) record["ItemQuantity"];

			return productId.EndsWith(CustomConstants.FLG_SUFFIX_PRODUCT_ID) == false
			       && quantity <= GetItemQuantity(productId + CustomConstants.FLG_SUFFIX_PRODUCT_ID,
												  variationId + CustomConstants.FLG_SUFFIX_PRODUCT_ID, orderItems)
				       ? productId + CustomConstants.FLG_SUFFIX_PRODUCT_ID
				       : productId;
		}
		#endregion

		/// <summary>
		/// ギフトかどうかを判定
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private bool IsGift(DataRowView record)
		{
			switch (record["mall_id"].ToString())
			{
				case CustomConstants.FLG_ORDER_MALL_ID_RAKUTEN:
					return IsRakutenGift(record["relation_memo"].ToString());

				case CustomConstants.FLG_ORDER_MALL_ID_YAHOO:
					return IsYahooGift(record["relation_memo"].ToString());

				default:
					return false;
			}
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

	}
}
