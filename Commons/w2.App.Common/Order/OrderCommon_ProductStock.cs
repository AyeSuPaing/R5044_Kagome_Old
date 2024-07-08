/*
=========================================================================================================
  Module      : 注文共通処理クラス 商品在庫部分(OrderCommon_OrderCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.ProductStock;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order
{
	public partial class OrderCommon
	{
		/// <summary>実行結果</summary>
		public enum ResultKbn
		{
			/// <summary>更新なし</summary>
			NoUpdate,
			/// <summary>更新OK</summary>
			UpdateOK,
			/// <summary>一部更新</summary>
			UpdatePart,
			/// <summary>更新NG</summary>
			UpdateNG
		}

		/// <summary>
		/// 実在庫の引当戻しを行う
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		public static ResultKbn UpdateOrderItemRealStockCanceled(
			string orderId,
			string shopId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// トランザクション中のテーブル更新順を守るため
			//  商品在庫更新前に該当注文情報の更新ロックを取得
			//------------------------------------------------------
			GetUpdlockFromOrderTables(orderId, sqlAccessor);

			//------------------------------------------------------
			// 実在庫引当戻し処理処理
			//------------------------------------------------------
			foreach (DataRowView orderItem in GetOrderItems(orderId, shopId, sqlAccessor))
			{
				int itemRealStockReserved = ((int)orderItem[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED]);

				// 返品商品・引き当て無し対象外
				if (((string)orderItem[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)
					|| (itemRealStockReserved == 0))
				{
					continue;
				}

				// 商品在庫情報セット
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, orderItem[Constants.FIELD_ORDERITEM_SHOP_ID]);
				input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]);
				input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]);
				input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, itemRealStockReserved * -1);
				// 商品在庫履歴情報セット
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, orderItem[Constants.FIELD_ORDERITEM_ORDER_ID]);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, itemRealStockReserved * -1);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_FORWARD_CANCEL);	// 注文在庫引当キャンセル
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, "");
				// 注文情報セット
				input.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS, Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_UNRESERVED);				// 在庫引当ステータスを在庫未引当に更新
				// 注文商品情報セット
				input.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, orderItem[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO]);
				input.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED, itemRealStockReserved * -1);
				// 最終更新者
				input.Add(Constants.FIELD_ORDER_LAST_CHANGED, lastChanged);

				// 注文情報引当ステータス更新
				using (SqlStatement sqlStatement = new SqlStatement("Order", "UpdateOrderStockReservedStatus"))
				{
					int updated = sqlStatement.ExecStatement(sqlAccessor, input);
				}
				// 実在庫引当済み商品数を更新
				using (SqlStatement sqlStatement = new SqlStatement("Order", "UpdateItemRealStockReserved"))
				{
					int updated = sqlStatement.ExecStatement(sqlAccessor, input);
				}
				// 最終更新日時更新
				new OrderService().UpdateOrderDateChangedByOrderId((string)input[Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID], sqlAccessor);

				// Continue if not manage stock
				if (IsStockManagement(StringUtility.ToEmpty(orderItem[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN])) == false) continue;

				// 実在庫更新
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "AddProductStock"))
				{
					int updated = sqlStatement.ExecStatement(sqlAccessor, input);
				}
				// 商品在庫履歴情報追加
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "InsertProductStockHistory"))
				{
					int updated = sqlStatement.ExecStatement(sqlAccessor, input);
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, sqlAccessor);
			}

			return ResultKbn.UpdateOK;	// 引当戻しはOK固定
		}

		/// <summary>
		/// 論理在庫のキャンセルを行う
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="productStockHistoryActionStatus">商品在庫履歴アクションステータス</param>
		/// <param name="loginOperatorName"></param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void UpdateProductStockCancel(
			DataRowView order,
			string productStockHistoryActionStatus,
			string loginOperatorName,
			SqlAccessor sqlAccessor)
		{
			var orderItems = new OrderService().Get((string)order[Constants.FIELD_ORDER_ORDER_ID], sqlAccessor).Items;
			new ProductStockService().UpdateProductStockCancel(
				orderItems,
				productStockHistoryActionStatus,
				loginOperatorName,
				sqlAccessor);
		}

		/// <summary>
		/// 実在庫の引当を行う(注文商品情報の実在庫引当済み商品数に引当、商品在庫情報の引当済実在庫数を加算する)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		public static ResultKbn UpdateOrderItemRealStockReserved(
			string orderId,
			string shopId,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			bool reserved = false;

			//------------------------------------------------------
			// トランザクション中のテーブル更新順を守るため
			//  商品在庫更新前に該当注文情報の更新ロックを取得
			//------------------------------------------------------
			GetUpdlockFromOrderTables(orderId, sqlAccessor);

			//------------------------------------------------------
			// 実在庫引当処理
			//------------------------------------------------------
			DataView orderItems = GetOrderItems(orderId, shopId, sqlAccessor);
			foreach (DataRowView orderItem in orderItems)
			{
				// 返品注文商品は対象外
				if ((string)orderItem[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)
				{
					continue;
				}
				// 注文商品情報をハッシュテーブルに格納
				int allItemRealStockReserved = 0;
				int itemRealStockReserved = 0;

				//------------------------------------------------------
				// 在庫引当が行えるかチェック
				//------------------------------------------------------
				bool canReserveStock = false;

				// 商品在庫情報取得
				DataView productStock = null;
				using (SqlStatement SqlStatement = new SqlStatement("ProductStock", "GetProductStock"))
				{
					Hashtable input = new Hashtable();
					input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, orderItem[Constants.FIELD_ORDERITEM_SHOP_ID]);
					input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]);
					input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]);

					productStock = SqlStatement.SelectSingleStatement(sqlAccessor, input);
				}
				if (productStock.Count != 0)
				{
					// 引当する商品数取得 (注文数 - 実在庫引当済み商品数)
					itemRealStockReserved = (int)orderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] - (int)orderItem[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED];

					// 引当可能な最大商品数取得 (実在庫数 - 引当済実在庫数)
					allItemRealStockReserved = (int)productStock[0][Constants.FIELD_PRODUCTSTOCK_REALSTOCK] - (int)productStock[0][Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED];

					// 引当する商品が存在するかつ引当可能な商品が存在する場合
					if ((itemRealStockReserved > 0) && (allItemRealStockReserved > 0))
					{
						// [引当可能な最大商品数]が[引当する商品数]より大きければ、[引当する商品数]を
						// [引当する商品数]が[引当可能な最大商品数]以下、[引当可能な最大商品数]を設定
						itemRealStockReserved = (allItemRealStockReserved > itemRealStockReserved) ? itemRealStockReserved : allItemRealStockReserved;
						canReserveStock = true;
					}
				}

				// 在庫引当が行える場合
				if (canReserveStock)
				{
					// 注文商品情報セット
					Hashtable input = new Hashtable();
					input.Add(Constants.FIELD_ORDERITEM_ORDER_ID, orderItem[Constants.FIELD_ORDERITEM_ORDER_ID]);
					input.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, orderItem[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO]);
					input.Add(Constants.FIELD_ORDERITEM_SHOP_ID, orderItem[Constants.FIELD_ORDERITEM_SHOP_ID]);
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]);
					input.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]);
					input.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED, itemRealStockReserved);
					// 商品在庫情報セット
					input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, itemRealStockReserved);
					// 商品在庫履歴情報セット
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, itemRealStockReserved);
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_RESERVED);	// 注文在庫引当
					input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, "");
					// 最終更新者セット
					input.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, loginOperatorName);

					// 注文商品情報更新
					using (SqlStatement sqlStatement = new SqlStatement("Order", "UpdateItemRealStockReserved"))
					{
						int updated = sqlStatement.ExecStatement(sqlAccessor, input);
					}
					// 最終更新日時更新
					new OrderService().UpdateOrderDateChangedByOrderId((string)input[Constants.FIELD_ORDERITEM_ORDER_ID], sqlAccessor);

					// 商品在庫情報更新
					using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "AddProductStock"))
					{
						int updated = sqlStatement.ExecStatement(sqlAccessor, input);
					}

					// 商品在庫履歴情報追加
					using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "InsertProductStockHistory"))
					{
						int updated = sqlStatement.ExecStatement(sqlAccessor, input);
					}

					// 1商品以上引当がされた
					reserved = true;
				}
			}

			//------------------------------------------------------
			// 注文商品在庫引当チェック
			//------------------------------------------------------
			bool reservedAll = true;
			foreach (DataRowView orderItem in GetOrderItems(orderId, shopId, sqlAccessor))
			{
				// 返品商品以外、かつ全て引当されていない場合はフラグを落とす
				if (((string)orderItem[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] != Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)
					&& ((int)orderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] != (int)orderItem[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED]))
				{
					reservedAll = false;
					break;
				}
			}
			// 全ての注文商品が引当済？
			string orderStockReservedStatus = null;
			ResultKbn resultKbn = ResultKbn.UpdateNG;
			if (reservedAll)
			{
				resultKbn = ResultKbn.UpdateOK;
				orderStockReservedStatus = Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_RESERVED;
			}
			// 一部？
			else if (reserved)
			{
				resultKbn = ResultKbn.UpdatePart;
				orderStockReservedStatus = Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_PARTRESERVED;
			}
			// 何も引当無し？
			else
			{
				resultKbn = ResultKbn.UpdateNG;
				orderStockReservedStatus = Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_UNRESERVED;
			}

			//------------------------------------------------------
			// 注文情報引当ステータス更新
			// （トランザクション中のテーブル更新順とは異なるためトランザクション先頭で更新ロック取得処理が必要）
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("Order", "UpdateOrderStockReservedStatus"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_ORDER_ORDER_ID, orderId);
				input.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS, orderStockReservedStatus);
				input.Add(Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName);

				int updated = sqlStatement.ExecStatement(sqlAccessor, input);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, loginOperatorName, sqlAccessor);
			}

			return resultKbn;
		}

		/// <summary>
		/// Check stock management kbn
		/// </summary>
		/// <param name="stockManagementKbn">The stock management kbn</param>
		/// <returns>
		/// true: manage stock
		/// false: not manage stock
		/// </returns>
		private static bool IsStockManagement(string stockManagementKbn)
		{
			return ((string.IsNullOrEmpty(stockManagementKbn) == false) && (stockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED));
		}
	}
}
