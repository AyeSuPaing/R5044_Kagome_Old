/*
=========================================================================================================
  Module      : Import yahoo order item(ImportAddYahooOrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Option;
using w2.App.Common.Stock;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;
using w2.Domain.UpdateHistory;

namespace w2.Commerce.Batch.ExternalFileImport.Imports
{
	/// <summary>
	/// Import yahoo order item
	/// </summary>
	class ImportAddYahooOrderItem : ImportBase
	{
		// Constants of csv field order item
		private const string CSV_FIELD_ORDER_ID = "OrderId";
		private const string CSV_FIELD_QUANTITY = "Quantity";
		private const string CSV_FIELD_ITEM_ID = "ItemId";
		private const string CSV_FIELD_SUBCODE = "SubCode";
		private const string CSV_FIELD_TITLE = "Title";
		private const string CSV_FIELD_ITEM_OPTION_NAME = "ItemOptionName";
		private const string CSV_FIELD_ITEM_OPTION_VALUE = "ItemOptionValue";
		private const string CSV_FIELD_UNIT_PRICE = "UnitPrice";
		private const string CSV_FIELD_LINE_SUB_TOTAL = "LineSubTotal";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="shopId">The login shop id</param>
		public ImportAddYahooOrderItem(string shopId)
			: base(shopId, "AddYahooOrderItem")
		{
		}

		/// <summary>
		/// Import yahoo order item
		/// </summary>
		/// <param name="activeFilePath">The active file path</param>
		/// <returns>The number of row inport success</returns>
		public override int Import(string activeFilePath)
		{
			int importCount = 0;
			List<string> lines = new List<string>();

			Dictionary<string, List<string>> orderList;
			ArrayList headers;

			try
			{
				using (FileStream stream = new FileStream(activeFilePath, FileMode.Open))
				using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(Constants.CONST_ENCODING_DEFAULT)))
				{
					headers = new ArrayList(StringUtility.SplitCsvLine(reader.ReadLine()));
					while (reader.EndOfStream == false)
					{
						string line = reader.ReadLine();

						if (headers.Count != StringUtility.SplitCsvLine(line).Count())
						{
							throw new Exception("ヘッダのフィールド数とフィールド数が一致しません(" + (importCount + 1) + "行目)");
						}

						lines.Add(line);
					}
				}

				orderList = lines.Where(line => line.Length > 0)
								.GroupBy(line => StringUtility.SplitCsvLine(line)[headers.IndexOf(CSV_FIELD_ORDER_ID)])
								.ToDictionary(item => item.Key, item => item.ToList());
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("csvファイル形式エラー", ex);
				return 0;
			}

			foreach (var orderId in orderList.Keys)
			{
				// Check existed order
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "CheckOrderId"))
				{
					Hashtable parameters = new Hashtable();
					parameters.Add(Constants.FIELD_ORDER_SHOP_ID, this.m_strShopId);
					parameters.Add(Constants.FIELD_ORDER_ORDER_ID, orderId);

					DataView orderView = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, parameters);

					if (orderView.Count == 0)
					{
						FileLogger.WriteError("注文IDが存在しません。この注文情報はスキップされます。(order_id:" + orderId + ")");
						continue;
					}
				}

				// 仮登録ではない商品が含まれていればスキップ
				DataView orderItem = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "CheckOrderItem"))
				{
					Hashtable parameters = new Hashtable();
					parameters.Add(Constants.FIELD_ORDER_ORDER_ID, orderId);

					orderItem = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, parameters);
				}

				if (orderItem.Count > 0)
				{
					FileLogger.WriteError("本登録済みの注文商品が含まれています。この注文情報はスキップされます。(order_id:" + orderId + ")");
					continue;
				}

				//------------------------------------------------------
				// Import order item
				//------------------------------------------------------
				string errorMessage = string.Empty;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				{
					try
					{
						sqlAccessor.OpenConnection();
						sqlAccessor.BeginTransaction();

						// Delete item of order
						DeleteOrderItem(sqlAccessor, orderId);

						int index = 0;
						foreach (string orderItemData in orderList[orderId])
						{
							string[] datas = StringUtility.SplitCsvLine(orderItemData);
							ValidateData(headers, datas);

							string productId = datas[headers.IndexOf(CSV_FIELD_ITEM_ID)];
							string variationId = (datas[headers.IndexOf(CSV_FIELD_SUBCODE)] != string.Empty)
								? datas[headers.IndexOf(CSV_FIELD_SUBCODE)]
								: productId;
							int orderItemQuantity = Convert.ToInt32(datas[headers.IndexOf(CSV_FIELD_QUANTITY)]);
							int productInStock = 0;

							DataView viewProduct = GetExistsProduct(sqlAccessor, productId, variationId);
							if (viewProduct.Count == 0)
							{
								errorMessage = string.Format("商品情報が見つかりませんでした。(order_id: {0}, product_id: {1}, variation_id: {2})", orderId, productId, variationId);
								throw new Exception(errorMessage);
							}

							string productStockManagementKbn = viewProduct[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN].ToString();

							// Add order item
							AddOrderItem(sqlAccessor, headers, datas, index);
							if ((productStockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED))
							{
								// Check product quantity in stock
								try
								{
									DataView viewStock = (new StockCommon()).GetStock(productId, variationId);
									productInStock = Convert.ToInt32(viewStock[0][Constants.FIELD_PRODUCTSTOCK_STOCK]);
								}
								catch (Exception ex)
								{
									FileLogger.WriteError(ex);
								}

								if ((productStockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK) &&
									(productInStock < orderItemQuantity))
								{
									errorMessage = string.Format("ただいま在庫がありません。(order_id: {0}, productId: {1}, VariationId : {2})", orderId,
										productId, variationId);
									throw new Exception(errorMessage);
								}

								// Update product stock for order
								if (UpdateProductStock(sqlAccessor, productId, variationId, orderItemQuantity) <= 0)
								{
									errorMessage = string.Format("商品バリエーションID:{0}の在庫を更新できませんでした。商品番号が誤っているか、在庫数が取得できなかった可能性があります。注文ID:{1}", variationId, orderId);
									throw new Exception(errorMessage);
								}

								// Update product stock history
								if (InsertProductStockHistory(sqlAccessor, datas[headers.IndexOf(CSV_FIELD_ORDER_ID)], productId, variationId, orderItemQuantity) <= 0)
								{
									errorMessage = string.Format("商品バリエーションID:{0}の在庫を更新できませんでした。商品番号が誤っているか、在庫数が取得できなかった可能性があります。注文ID:{1}", variationId, orderId);
									throw new Exception(errorMessage);
								}
							}
							
							index++;
						}

						// 更新履歴登録
						new UpdateHistoryService().InsertForOrder(orderId, Constants.FLG_LASTCHANGED_BATCH, sqlAccessor);

						sqlAccessor.CommitTransaction();

						importCount++;
					}
					catch (Exception ex)
					{
						FileLogger.WriteError(ex);
						sqlAccessor.RollbackTransaction();

						if (string.IsNullOrEmpty(errorMessage) == false)
						{
							// モール監視ログ登録
							w2.App.Common.MallCooperation.MallWatchingLogManager mallWatchingLogManager = new App.Common.MallCooperation.MallWatchingLogManager();
							mallWatchingLogManager.Insert(
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_EXTERNALFILEIMPORT,
								"",
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
								ex.ToString());
						}

						continue;
					}
				}
			}

			return importCount;
		}

		/// <summary>
		/// Adds the order item.
		/// </summary>
		/// <param name="sqlAccessor">The SQL accessor.</param>
		/// <param name="headers">The csv line headers.</param>
		/// <param name="datas">The csv line datas.</param>
		private void AddOrderItem(SqlAccessor sqlAccessor, ArrayList headers, string[] datas, int index)
		{
			using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "InsertOrderItem"))
			{
				Hashtable htInput = new Hashtable();

				var product = new ProductService().GetProductVariation(
					m_strShopId,
					datas[headers.IndexOf(CSV_FIELD_ITEM_ID)],
					(datas[headers.IndexOf(CSV_FIELD_SUBCODE)] != string.Empty) ? datas[headers.IndexOf(CSV_FIELD_SUBCODE)] : datas[headers.IndexOf(CSV_FIELD_ITEM_ID)],
					string.Empty);
				var productTaxCategory = new ProductTaxCategoryService().Get(product.TaxCategoryId);

				var productPriceOrg = decimal.Parse(datas[headers.IndexOf(CSV_FIELD_UNIT_PRICE)]);
				var itemQuantity = decimal.Parse(datas[headers.IndexOf(CSV_FIELD_QUANTITY)]);
				var productPriceTax = TaxCalculationUtility.GetTaxPrice(productPriceOrg, productTaxCategory.TaxRate, Constants.TAX_ROUNDTYPE, true);
				var itemPriceTax = productPriceTax * itemQuantity;
				var productPrice = TaxCalculationUtility.GetPrescribedPrice(productPriceOrg, productPriceTax, true);

				htInput.Add(Constants.FIELD_ORDERITEM_SHOP_ID, m_strShopId);
				htInput.Add(Constants.FIELD_ORDERITEM_ORDER_ID, datas[headers.IndexOf(CSV_FIELD_ORDER_ID)]);
				htInput.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, index);
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, datas[headers.IndexOf(CSV_FIELD_ITEM_ID)]);
				htInput.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, (datas[headers.IndexOf(CSV_FIELD_SUBCODE)] != string.Empty) ? datas[headers.IndexOf(CSV_FIELD_SUBCODE)] : datas[headers.IndexOf(CSV_FIELD_ITEM_ID)]);
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME, SetProductName(datas[headers.IndexOf(CSV_FIELD_TITLE)], datas[headers.IndexOf(CSV_FIELD_ITEM_OPTION_NAME)], datas[headers.IndexOf(CSV_FIELD_ITEM_OPTION_VALUE)]));
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE, productPrice);
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_POINT, new decimal(0));
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG, TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag());
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, productTaxCategory.TaxRate);
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE, Constants.TAX_ROUNDTYPE);
				// 税込み価格
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX, productPriceOrg);
				htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP, decimal.Zero);
				htInput.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, itemQuantity);
				htInput.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE, itemQuantity);
				htInput.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE, datas[headers.IndexOf(CSV_FIELD_LINE_SUB_TOTAL)]);
				htInput.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX, itemPriceTax);
				htInput.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE, datas[headers.IndexOf(CSV_FIELD_LINE_SUB_TOTAL)]);

				int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
				if (iUpdated <= 0)
				{
					// Todo:モール連携監視ログ出力
					throw new ApplicationException("In w2_OrderItem, Insert error.");
				}
			}
		}

		/// <summary>
		/// Deletes the order item.
		/// </summary>
		/// <param name="sqlAccessor">The SQL accessor.</param>
		/// <param name="orderId">The order id.</param>
		private void DeleteOrderItem(SqlAccessor sqlAccessor, string orderId)
		{
			using (SqlStatement sqlStatement = new SqlStatement(this.m_strFileType, "DeleteOrderItem"))
			{
				Hashtable parameters = new Hashtable();
				parameters.Add(Constants.FIELD_ORDERITEM_ORDER_ID, orderId);

				sqlStatement.ExecStatement(sqlAccessor, parameters);
			}
		}

		/// <summary>
		/// Update product stock for order
		/// </summary>
		/// <param name="sqlAccessor">The SQL accessor.</param>
		/// <param name="productId">The Id of product</param>
		/// <param name="variationId">The product variation id</param>
		/// <param name="itemQuantity">Order item quantity</param>
		/// <returns>Update count</returns>
		private int UpdateProductStock(SqlAccessor sqlAccessor, string productId, string variationId, int itemQuantity)
		{
			using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "UpdateProductStockForOrder"))
			{
				Hashtable parameters = new Hashtable();
				parameters.Add(Constants.FIELD_ORDERITEM_SHOP_ID, this.m_strShopId);
				parameters.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, productId);
				parameters.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, variationId);
				parameters.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, itemQuantity);
				return sqlStatement.ExecStatement(sqlAccessor, parameters);
			}
		}

		/// <summary>
		/// Insert product stock history
		/// </summary>
		/// <param name="sqlAccessor">The SQL accessor.</param>
		/// <param name="orderId">The order id</param>
		/// <param name="productId">The Id of product</param>
		/// <param name="variationId">The variation id</param>
		/// <param name="itemQuantity">Order item quantity</param>
		/// <returns>Update count</returns>
		private int InsertProductStockHistory(SqlAccessor sqlAccessor, string orderId, string productId, string variationId, int itemQuantity)
		{
			using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "InsertProductStockHistoryForOrder"))
			{
				Hashtable parameters = new Hashtable();
				parameters.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, orderId);
				parameters.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, this.m_strShopId);
				parameters.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productId);
				parameters.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationId);
				parameters.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, -1 * itemQuantity);

				return sqlStatement.ExecStatement(sqlAccessor, parameters);
			}
		}

		/// <summary>
		/// Sets the name of the product.
		/// </summary>
		/// <param name="productName">Product's name.</param>
		/// <param name="optionName">The option name.</param>
		/// <param name="optionValue">The option value.</param>
		/// <returns>Product name</returns>
		private string SetProductName(string productName, string optionName, string optionValue)
		{
			if ((optionName != string.Empty) || (optionValue != string.Empty))
			{
				string[] optionNames = optionName.Split(';');
				string[] optionValues = optionValue.Split(';');

				StringBuilder appendProductName = new StringBuilder();

				int lenght = Math.Max(optionNames.Length, optionValues.Length);
				for (int index = 0; index < lenght; index++)
				{
					string name = (index < optionNames.Length) ? optionNames[index] : string.Empty;
					string value = (index < optionValues.Length) ? optionValues[index] : string.Empty;

					appendProductName.Append(string.Format("{0}：{1}", name, value)).Append("、");
				}

				if (appendProductName.Length > 0) appendProductName.Remove(appendProductName.Length - 1, 1);

				productName += string.Format("({0})", appendProductName);
			}

			if (productName.Length > 200) productName = productName.Substring(0, 200);

			return productName;
		}

		/// <summary>
		/// Checks the exists product.
		/// </summary>
		/// <param name="sqlAccessor">The SQL accessor.</param>
		/// <param name="productId">The product identifier.</param>
		/// <param name="variationId">The variation identifier.</param>
		/// <returns>Product info if it exists</returns>
		private DataView GetExistsProduct(SqlAccessor sqlAccessor, string productId, string variationId)
		{
			using (SqlStatement sqlStatement = new SqlStatement("Product", "CheckExistsProduct"))
			{
				Hashtable parameters = new Hashtable();
				parameters.Add(Constants.FIELD_ORDERITEM_SHOP_ID, this.m_strShopId);
				parameters.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, productId);
				parameters.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, variationId);

				return sqlStatement.SelectSingleStatement(sqlAccessor, parameters);
			}
		}

		/// <summary>
		/// Validates the data before import order item.
		/// </summary>
		/// <param name="headers">The csv line headers.</param>
		/// <param name="datas">The csv line datas.</param>
		private void ValidateData(ArrayList headers, string[] datas)
		{
			string variationId = (datas[headers.IndexOf(CSV_FIELD_SUBCODE)] != string.Empty) ? datas[headers.IndexOf(CSV_FIELD_SUBCODE)] : datas[headers.IndexOf(CSV_FIELD_ITEM_ID)];
			if ((Validator.IsHalfwidthNumber(datas[headers.IndexOf(CSV_FIELD_QUANTITY)]) == false)
				|| (Validator.IsHalfwidthNumber(datas[headers.IndexOf(CSV_FIELD_UNIT_PRICE)]) == false)
				|| (Validator.IsHalfwidthNumber(datas[headers.IndexOf(CSV_FIELD_LINE_SUB_TOTAL)]) == false))
			{
				throw new Exception(string.Format("[Input Quantity][UnitPrice][LineSubTota]は正数ではありません。(order_id: {0}, productId: {1}, VariationId : {2})"
										, datas[headers.IndexOf(CSV_FIELD_ORDER_ID)], datas[headers.IndexOf(CSV_FIELD_ITEM_ID)], variationId));
			}
		}
	}
}
