/*
=========================================================================================================
  Module      :  File Upload Cooperation Action (FileUploadCooperationAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using w2.App.Common.Elogit;
using w2.Commerce.Batch.WmsShippingBatch.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.WmsShippingBatch.Action
{
	/// <summary>
	/// File upload cooperation action
	/// </summary>
	public class FileUploadCooperationAction : ExportActionBase
	{
		#region Methods
		/// <summary>
		/// On execute
		/// </summary>
		public override void Execute()
		{
			try
			{
				base.Execute();

				CreateInstructionFile();

				// Delay a few minutes after the file upload API execution is completed, this process is performed
				Thread.Sleep(Constants.TIME_DELAY_MILLISECONDS);

				UpdateOrderInfo();
			}
			catch (Exception ex)
			{
				// Set information to send mail to the operator
				CreateAndSendMailToOperator(ex.Message, string.Empty);

				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Create instruction File
		/// </summary>
		private void CreateInstructionFile()
		{
			var elogitApiService = new ElogitApiService();
			var orderService = new OrderService();

			// The file array in the path
			var files = base.GetFilesPath(Constants.DIR_PATH_WAITING_FOR_PROCESSING);
			var orderIdTitle = base.ExportSetting.FieldSettings
				.First(field => (field.ExportName == Constants.FIELD_ORDER_ORDER_ID))
				.ExportHeaderName;

			foreach (var file in files)
			{
				var orderIds = base.GetOrderFromFileCSV(file)
					.Select(item => StringUtility.ToEmpty(item[orderIdTitle]))
					.Distinct()
					.ToArray();

				// Call api to upload file
				var result = elogitApiService.UploadFile(file);

				if (result.IsSuccess)
				{
					// Execute move file
					MoveFile(
						file,
						Constants.DIR_PATH_UPLOADING,
						string.Format("{0}.csv", result.Response.IfHistoryKey));

					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						// Update order extend status 42
						foreach (var orderId in orderIds)
						{
							orderService.UpdateOrderExtendStatus(
								orderId,
								42,
								Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
								DateTime.Now,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.Insert,
								accessor);
						}

						accessor.CommitTransaction();
					}

					// Write log
					WriteLogSucess(GetMessage(ElogitConstants.KEY_FILE_UPLOAD_LINKAGE), result.Response.IfHistoryKey);

					continue;
				}

				// If the status history result has error, set information to send mail to the operator
				var ifHistoryKey = (result.Response != null)
					? StringUtility.ToEmpty(result.Response.IfHistoryKey)
					: string.Empty;
				CreateAndSendMailToOperator(
					GetMessage(ElogitConstants.KEY_FILE_UPLOAD_LINKAGE_FAIL),
					ifHistoryKey,
					result.GetStatusCodeAndErrorMessage,
					orderIds);

				// Write log error
				WriteLogError(result.GetErrorMessage, ifHistoryKey);
			}
		}

		/// <summary>
		/// Update order information after upload file
		/// </summary>
		private void UpdateOrderInfo()
		{
			var orderService = new OrderService();
			var elogitApiService = new ElogitApiService();
			var files = base.GetFilesPath(Constants.DIR_PATH_UPLOADING);
			var orderIdTitle = base.ExportSetting.FieldSettings
				.First(field => (field.ExportName == Constants.FIELD_ORDER_ORDER_ID)).ExportHeaderName
				.ToString();

			foreach (var file in files)
			{
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				var orderIds = base.GetOrderFromFileCSV(file)
					.Select(item => StringUtility.ToEmpty(item[orderIdTitle]))
					.Distinct()
					.ToArray();
				var result = elogitApiService.GetIfHistoryKeyUpload(fileNameWithoutExtension);

				if (result.IsSuccess)
				{
					// Move file form folder「Uploading」to folder「Processed」
					var fileUploadingPath = Path.Combine(
						Constants.DIR_PATH_UPLOADING,
						string.Format("{0}.csv", fileNameWithoutExtension));
					MoveFile(fileUploadingPath, Constants.DIR_PATH_UPLOADED);

					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						// Update order status ship arranged
						foreach (var orderId in orderIds)
						{
							orderService.UpdateOrderStatus(
								orderId,
								Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED,
								DateTime.Now,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.Insert,
								accessor);
						}

						accessor.CommitTransaction();
					}

					// Create and send mail to operator
					CreateAndSendMailToOperator(
						GetMessage(ElogitConstants.KEY_UPLOAD_SUCCESS),
						fileNameWithoutExtension,
						result.GetStatusCodeAndErrorMessage,
						orderIds,
						result.GetLogText);

					// Write log
					WriteLogSucess(GetMessage(ElogitConstants.KEY_UPLOAD_SUCCESS), fileNameWithoutExtension);

					continue;
				}
				else if (result.IsInProcess)
				{
					// Write log inprocess
					base.WriteLogInProcess(fileNameWithoutExtension);
					continue;
				}

				// If the status history result has error, set information to send mail to the operator
				CreateAndSendMailToOperator(
					GetMessage(ElogitConstants.KEY_IF_HISTORY_STATUS_FAILED),
					fileNameWithoutExtension,
					result.GetStatusCodeAndErrorMessage,
					orderIds,
					result.GetLogText);

				// Write log error
				WriteLogError(GetMessage(ElogitConstants.KEY_IF_HISTORY_STATUS_FAILED), fileNameWithoutExtension);
			}
		}

		/// <summary>
		/// Create and send mail to operator
		/// </summary>
		/// <param name="messages">Messages</param>
		/// <param name="ifHistoryKey">IF history key</param>
		/// <param name="orderIds">Order ids</param>
		/// <param name="statusCodeAndMessage">Response message</param>
		/// <param name="logText">Log text</param>
		protected override void CreateAndSendMailToOperator(
			string messages,
			string ifHistoryKey,
			string statusCodeAndMessage = "",
			string[] orderIds = null,
			string logText = "")
		{
			// Create data to send mail
			var input = new Hashtable
			{
				{ ElogitConstants.KEY_IF_HISTORY_KEY, GetMessage(ElogitConstants.KEY_IF_HISTORY_KEY).Replace("@@ 1 @@", ifHistoryKey)},
				{ MailSendUtility.CONST_KEY_MESSAGE, messages },
			};
			if (orderIds != null)
			{
				input[Constants.FIELD_ORDER_ORDER_ID] = GetMessage(ElogitConstants.KEY_TARGET_ORDER_ID)
					.Replace("@@ 1 @@", string.Join(Environment.NewLine, orderIds));
			}
			if (string.IsNullOrEmpty(statusCodeAndMessage) == false) input[ElogitConstants.KEY_RESPONSE_MESSAGE] = statusCodeAndMessage;
			if (string.IsNullOrEmpty(logText) == false)
			{
				input[ElogitConstants.KEY_LOG_TEXT] = GetMessage(ElogitConstants.KEY_LOG_TEXT)
					.Replace("@@ 1 @@", logText);
			}

			// Send mail
			new MailSendUtility().SendMailToOperator(input);
		}

		/// <summary>
		/// Export log success
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="ifHistoryKey">IF history key</param>
		private void WriteLogSucess(string message, string ifHistoryKey)
		{
			var result = string.Format(
				"{0}\r\n{1}",
				message,
				GetMessage(ElogitConstants.KEY_IF_HISTORY_KEY).Replace("@@ 1 @@", ifHistoryKey));
			FileLogger.Write("FileUpload", result);
		}

		/// <summary>
		/// Export log error
		/// </summary>
		/// <param name="errorMesssage">Error message</param>
		/// <param name="ifHistoryKey">IF history key</param>
		private void WriteLogError(string errorMesssage, string ifHistoryKey)
		{
			// Write log error
			var message = string.Format(
				"{0}\r\n{1}",
				errorMesssage,
				GetMessage(ElogitConstants.KEY_IF_HISTORY_KEY).Replace("@@ 1 @@", ifHistoryKey));
			FileLogger.WriteError(message);
		}

		/// <summary>
		/// Get export data
		/// </summary>
		/// <returns>A collection of export models</returns>
		public override IModel[] GetExportData()
		{
			var orders = new OrderService().GetOrdersForElogitWmsCooperation();
			var result = ConvertData(orders);
			return result;
		}

		/// <summary>
		/// Convert data
		/// </summary>
		/// <param name="models">Models</param>
		/// <returns>Convert Models</returns>
		protected OrderModel[] ConvertData(OrderModel[] models)
		{
			foreach (var model in models)
			{
				// No.2
				model.DataSource[Constants.SHIPPER_CODE] = Constants.ELOGIT_WMS_CODE;

				// No.12
				var shippingZip = StringUtility.ToEmpty(model.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP])
					.Replace("-", string.Empty);
				model.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] = shippingZip;

				// No.13
				var shippingAddr1 = string.Format(
					"{0}{1}{2}",
					model.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1],
					model.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2],
					model.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3]);
				model.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1] = shippingAddr1;

				// No.22
				var orderShipping = model.Shippings.FirstOrDefault();
				var orderOwner = model.Owner;
				var isOwnerSameAsShipping = ((StringUtility.ToEmpty(orderOwner.OwnerName) == StringUtility.ToEmpty(orderShipping.ShippingName))
					&& (StringUtility.ToEmpty(orderOwner.OwnerNameKana) == StringUtility.ToEmpty(orderShipping.ShippingNameKana))
					&& (StringUtility.ToEmpty(orderOwner.OwnerZip) == StringUtility.ToEmpty(orderShipping.ShippingZip))
					&& (StringUtility.ToEmpty(orderOwner.ConcatenateAddressWithoutCountryName()) == StringUtility.ToEmpty(orderShipping.ConcatenateAddressWithoutCountryName()))
					&& (StringUtility.ToEmpty(orderOwner.OwnerTel1) == StringUtility.ToEmpty(orderShipping.ShippingTel1)));
				model.DataSource[Constants.CLIENT_CHANGE_FLAG] = isOwnerSameAsShipping
					? Constants.FLG_CLIENT_CHANGE_FLG_INVALID
					: Constants.FLG_CLIENT_CHANGE_FLG_VALID;

				// No.25
				var ownerZip = StringUtility.ToEmpty(model.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP])
					.Replace("-", string.Empty);
				model.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP] = ownerZip;

				// No.26
				var ownerAddr1 = string.Format(
					"{0}{1}{2}",
					model.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR1],
					model.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR2],
					model.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR3]);
				model.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR1] = ownerAddr1;

				// No.32
				var cashOnDeliveryFlag = (StringUtility.ToEmpty(model.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])
						== Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
					? Constants.FLG_CASH_ON_DELIVERY_FLG_VALID
					: Constants.FLG_CASH_ON_DELIVERY_FLG_INVALID;
				model.DataSource[Constants.CASH_ON_DELIVERY_FLG] = cashOnDeliveryFlag;

				// No.39
				model.DataSource[Constants.TOTAL_ITEM_AMOUNT] = model.OrderPriceSubtotal;

				// No.40
				var shippingTaxExcluded = (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING]
					- (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX];
				model.DataSource[Constants.SHIPPING_TAX_EXCLUDED] = shippingTaxExcluded;

				// No.41
				var paymentTaxExcluded = (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE]
					- (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX];
				model.DataSource[Constants.PAYMENT_TAX_EXCLUDED] = paymentTaxExcluded;

				// No.43
				var taxRate = (decimal)model.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE];
				var orderPriceDiscountTotal10 = (decimal)model.DataSource[Constants.PRICE_TOTAL_ITEM_10];
				var orderPriceDiscountTotal8 = (decimal)model.DataSource[Constants.PRICE_TOTAL_ITEM_8];
				var totalDiscount
					= model.OrderPointUseYen
					+ model.OrderCouponUse
					+ model.OrderDiscountSetPrice
					+ model.FixedPurchaseDiscountPrice
					+ model.MemberRankDiscountPrice
					+ model.FixedPurchaseMemberDiscountAmount
					+ model.SetpromotionProductDiscountAmount
					+ model.SetpromotionShippingChargeDiscountAmount
					+ model.SetpromotionPaymentChargeDiscountAmount;

				if (model.OrderPriceRegulation > 0)
				{
					if (taxRate == Constants.PRODUCT_TAX_RATE_8)
					{
						var priceSubTotalByRate8 = (decimal)model.DataSource[Constants.PRICE_SUBTOTAL_BY_RATE_8];
						orderPriceDiscountTotal10 = (priceSubTotalByRate8 - model.OrderPriceTotal) + model.OrderPriceShipping + model.OrderPriceExchange;
						orderPriceDiscountTotal8 = Math.Abs(orderPriceDiscountTotal10) - model.OrderPriceRegulation;
						var discount = model.OrderPriceRegulation - totalDiscount;
						model.DataSource[Constants.DISCOUNT_1] = discount;
					}
					else
					{
						orderPriceDiscountTotal10 = totalDiscount - model.OrderPriceRegulation;
						model.DataSource[Constants.DISCOUNT_1] = orderPriceDiscountTotal10;
					}
				}
				else
				{
					var discount = totalDiscount - model.OrderPriceRegulation;
					var priceDiscountTotal = (taxRate == Constants.PRODUCT_TAX_RATE_8)
						? orderPriceDiscountTotal8 = discount
						: orderPriceDiscountTotal10 = discount;
					model.DataSource[Constants.DISCOUNT_1] = priceDiscountTotal;
				}

				// No.50
				var consumptionTax = ((decimal)model.DataSource[Constants.CONSUMPTION_TAX]
					- (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX]
					- (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX]);
				model.DataSource[Constants.CONSUMPTION_TAX] = consumptionTax;

				// No.52
				var totalAmountRate10 = (taxRate == Constants.PRODUCT_TAX_RATE_10) ? model.OrderPriceSubtotal : 0;
				model.DataSource[Constants.TOTAL_ITEM_AMOUNT_RATE_10] = totalAmountRate10;

				// No.53
				model.DataSource[Constants.ORDER_PRICE_DISCOUNT_TOTAL_RATE_10] = orderPriceDiscountTotal10;

				// No.58
				var taxAmountRate10 = ((decimal)model.DataSource[Constants.TAX_PRICE_BY_RATE_10]
					- (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX]
					- (decimal)model.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX]);
				model.DataSource[Constants.TAX_AMOUNT_RATE_10] = taxAmountRate10;

				// No.59
				var totalAmountRate8 = (taxRate == Constants.PRODUCT_TAX_RATE_8) ? model.OrderPriceSubtotal : 0;
				model.DataSource[Constants.TOTAL_ITEM_AMOUNT_RATE_8] = totalAmountRate8;

				// No.60
				model.DataSource[Constants.ORDER_PRICE_DISCOUNT_TOTAL_RATE_8] = orderPriceDiscountTotal8;

				// No.65
				var taxAmountRate8 = (decimal)model.DataSource[Constants.TAX_PRICE_BY_RATE_8];
				model.DataSource[Constants.TAX_AMOUNT_RATE_8] = taxAmountRate8;

				// No.69
				var wrappingPaperType = StringUtility.ToEmpty(model.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]);
				var paperProcessingFlag = (string.IsNullOrEmpty(wrappingPaperType) == false)
					? Constants.FLG_PAPER_PROCESSING_FLG_VALID
					: Constants.FLG_PAPER_PROCESSING_FLG_INVALID;
				model.DataSource[Constants.PAPER_PROCESSING_FLAG] = paperProcessingFlag;

				// No.70
				var remarks = (string.IsNullOrEmpty(wrappingPaperType) == false)
					? wrappingPaperType
					: string.Empty;
				model.DataSource[Constants.REMARKS] = remarks;

				// No.103
				var taxAmountItem = ((decimal)model.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX]
					/ (int)model.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]);
				model.DataSource[Constants.TAX_AMOUNT_ITEM] = taxAmountItem;

				// No.104
				var itemAmount = ((decimal)model.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]
					* (int)model.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]);
				model.DataSource[Constants.ITEM_AMOUNT] = itemAmount;

				//No.117
				string orderPaymentKbn = StringUtility.ToEmpty(model.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
				var isDeferredPayments = base.DeferredPayment.Any(dp => dp == orderPaymentKbn);
				if (isDeferredPayments == false)
				{
					model.DataSource[Constants.FIELD_ORDER_CARD_TRAN_ID] = "";
				}
			}
			return models;
		}
		#endregion
	}
}
