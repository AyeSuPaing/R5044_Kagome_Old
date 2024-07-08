/*
=========================================================================================================
  Module      : ShippingReceivingStoreStatusAction(ShippingReceivingStoreStatusAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Flaps;
using w2.Commerce.Batch.ShippingReceivingStoreStatusImportBatch.Util;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.ExternalAPI.Common.Ftp;

namespace w2.Commerce.Batch.ShippingReceivingStoreStatusImportBatch.Action
{
	/// <summary>
	/// ShippingReceivingStoreStatusAction
	/// </summary>
	public class ShippingReceivingStoreStatusAction
	{
		#region Fields
		/// <summary>Lenght Max</summary>
		private const int LENGHT_MAX = 576;
		/// <summary>Deadline Day</summary>
		private const int DEADLINE_DAY = 6;
		/// <summary>Flag Shipping Status Finished</summary>
		private const string FLG_SHIPPING_STATUSFINISHED = "3";
		/// <summary>Shipping External Deliverty Status Complete At Convenience Store</summary>
		private const string SHIPPING_EXTERNAL_DELIVERTY_STATUS_COMPLETE_AT_CONVENIENCE_STORE = "0810H";
		/// <summary>Day 3</summary>
		private readonly DateTime m_day3 = DateTime.Now.AddDays(-3).Date;
		/// <summary>Day 6</summary>
		private readonly DateTime m_day6 = DateTime.Now.Date;
		/// <summary>Shipping Status Delivering</summary>
		private const string SHIPPING_STATUS_DELIVERING = "6";
		/// <summary>Shipping Status Abnormal</summary>
		private const string SHIPPING_STATUS_ABNORMAL = "23";
		/// <summary>Shipping Status Rejection</summary>
		private const string SHIPPING_STATUS_REJECTED = "11";
		/// <summary>Shipping Status Missed Or Fixed</summary>
		private const string SHIPPING_STATUS_MISSED_OR_FIXED = "5";
		/// <summary>Shipping Status Returned</summary>
		private const string SHIPPING_STATUS_RETURNED = "10";
		/// <summary>Shipping Status Absenced</summary>
		private const string SHIPPING_STATUS_ABSENCED = "7";
		/// <summary>Shipping Status Changed Address</summary>
		private const string SHIPPING_STATUS_CHANGED_ADDRESS = "8";
		/// <summary>Shipping Status Demaged Product</summary>
		private const string SHIPPING_STATUS_DEMAGED_PRODUCT = "9";
		/// <summary>Format Date</summary>
		private const string FORMAT_DATE = "yyyyMMdd";
		/// <summary>Format Date For File Import</summary>
		private const string FORMAT_DATE_FOR_FILE_IMPORT = "yyyyMM_dd";
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ftp">FTP</param>
		public ShippingReceivingStoreStatusAction(FluentFtpUtility ftp)
		{
			this.FileExtension = ".txt";
			this.Ftp = ftp;
		}
		#endregion

		#region Method
		/// <summary>
		/// On Execute
		/// </summary>
		public void OnExecute()
		{
			var encoding = Encoding.GetEncoding(950);
			var fileName = (string.Format(
				"{0}_{1}{2}",
				Constants.TWPELICANEXPRESS_CLIENT.Split(',')[0],
				DateTime.Now.ToString(FORMAT_DATE_FOR_FILE_IMPORT),
				this.FileExtension)).ToLower();
			var files = this.Ftp.FileNameListDownload(string.Empty).Where(file => (file.ToLower() == fileName));
			foreach (var file in files)
			{
				if (Directory.Exists(Constants.SHIPPINGRECEIVINGSTORESTATUSIMPORTBATCH_TEMP_DIR_PATH) == false)
				{
					Directory.CreateDirectory(Constants.SHIPPINGRECEIVINGSTORESTATUSIMPORTBATCH_TEMP_DIR_PATH);
				}

				var path = Path.Combine(Constants.SHIPPINGRECEIVINGSTORESTATUSIMPORTBATCH_TEMP_DIR_PATH, file);
				this.Ftp.Download(
					file,
					path);

				using (var streamReader = new StreamReader(path, encoding))
				{
					Import(streamReader, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert);
				}
				File.Delete(path);
			}

			SendMailAndUpdateOrder();
		}

		/// <summary>
		/// Import
		/// </summary>
		/// <param name="streamReader">Stream Reader</param>
		/// <param name="lastChanged">last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		public void Import(StreamReader streamReader, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction(IsolationLevel.ReadCommitted);
				var orderService = new OrderService();

				while (streamReader.Peek() != -1)
				{
					var lineBuffer = streamReader.ReadLine();

					var encoding = Encoding.GetEncoding(950);
					var orderId = StringUtility.GetByteLengthString(lineBuffer, 11, 22, encoding);
					var shippingCheckNo = StringUtility.GetByteLengthString(lineBuffer, 26, 57, encoding);
					var shippingStatusDate = StringUtility.GetByteLengthString(lineBuffer, 342, 349, encoding);
					var twPelicanNo = StringUtility.GetByteLengthString(lineBuffer, 543, 547, encoding);

					if (encoding.GetBytes(lineBuffer).Length < LENGHT_MAX) return;

					var order = orderService.GetOrderInfoByShippingCheckNo(orderId, shippingCheckNo, accessor);

					if (order == null) continue;

					var shippingModel = orderService.GetShipping(orderId, 1, accessor);

					if (shippingModel == null) continue;

					shippingModel.ShippingExternalDelivertyStatus = twPelicanNo;
					shippingModel.ShippingStatus = GetShippingStatusByPelicanNo(twPelicanNo);

					shippingModel.ShippingStatusUpdateDate = DateTime.ParseExact(
						shippingStatusDate,
						FORMAT_DATE,
						CultureInfo.InvariantCulture);

					if (orderService.UpdateOrderShipping(shippingModel, accessor) > 0)
					{
						if (shippingModel.ShippingStatus == FLG_SHIPPING_STATUSFINISHED)
						{
							orderService.UpdateOrderStatus(
								orderId,
								Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP,
								DateTime.Now,
								lastChanged,
								UpdateHistoryAction.DoNotInsert,
								accessor);

							// FLAPS連携済みフラグをオンに更新
							if (Constants.FLAPS_OPTION_ENABLE)
							{
								new FlapsIntegrationFacade().TurnOnErpIntegrationFlg(
									orderId,
									Constants.FLG_LASTCHANGED_BATCH,
									accessor);
							}

							if (order.PaymentOrderId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
							{
								orderService.UpdateExternalPaymentStatusPayComplete(
									orderId,
									lastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
							if (updateHistoryAction == UpdateHistoryAction.Insert)
							{
								new UpdateHistoryService().InsertAllForOrder(
									orderId,
									lastChanged,
									accessor);
							}
						}
						accessor.CommitTransaction();
					}
				}
			}
		}

		/// <summary>
		/// Send Mail And Update Order
		/// </summary>
		private void SendMailAndUpdateOrder()
		{
			var orderService = new OrderService();
			var orderShippings = orderService.GetOrdersByExternalDelivertyStatusAndUpdateDate(
				DEADLINE_DAY,
				SHIPPING_EXTERNAL_DELIVERTY_STATUS_COMPLETE_AT_CONVENIENCE_STORE);

			foreach (var shipping in orderShippings)
			{
				var extendStatus = StringUtility.ToEmpty(
					shipping.DataSource["extend_status" + Constants.RECEIVINGSTORE_TWPELICAN_MAILORDEREXTENDNO]);

				var user = new UserService().Get(
					StringUtility.ToEmpty(shipping.DataSource[Constants.FIELD_ORDER_USER_ID]));

				// Sent email when ShippingReceivingMailDate is day 1, day 3 and day 6
				var isSentMail = ((shipping.ShippingReceivingMailDate.HasValue == false)
					|| (shipping.ShippingReceivingMailDate.Value.Date == m_day3)
					|| (shipping.ShippingReceivingMailDate.Value.Date == m_day6));

				switch (extendStatus)
				{
					case Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF:
						if (isSentMail && (user != null))
						{
							// Send Mail
							new MailSendUtil().SendMail(shipping, user.UserId, user.MailAddr);

							shipping.ShippingReceivingMailDate = DateTime.Now;
							orderService.UpdateOrderShipping(shipping);

							orderService.Modify(
								shipping.OrderId,
								order =>
								{
									order.DataSource["extend_status" + Constants.RECEIVINGSTORE_TWPELICAN_MAILORDEREXTENDNO] =
										Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON;
								},
								UpdateHistoryAction.DoNotInsert,
								null);
						}
						break;

					case Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON:
						if (isSentMail == false)
						{
							orderService.Modify(
								shipping.OrderId,
								order =>
								{
									order.DataSource["extend_status" + Constants.RECEIVINGSTORE_TWPELICAN_MAILORDEREXTENDNO] =
										Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
								},
								UpdateHistoryAction.DoNotInsert,
								null);
						}
						break;
				}
			}
		}

		/// <summary>
		/// Get Convert Shipping Pelican Status
		/// </summary>
		/// <param name="twPelicanNo">Pelican No</param>
		/// <returns>Shipping Status</returns>
		private string GetShippingStatusByPelicanNo(string twPelicanNo)
		{
			var inputConvertShippingStatus = new Hashtable()
			{
				{ "07301", SHIPPING_STATUS_ABSENCED },
				{ "07302", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "07303", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "07305", SHIPPING_STATUS_REJECTED },
				{ "07306", SHIPPING_STATUS_REJECTED },
				{ "07307", SHIPPING_STATUS_REJECTED },
				{ "07308", SHIPPING_STATUS_REJECTED },
				{ "07309", SHIPPING_STATUS_REJECTED },
				{ "07311", SHIPPING_STATUS_REJECTED },
				{ "07312", SHIPPING_STATUS_REJECTED },
				{ "07313", SHIPPING_STATUS_REJECTED },
				{ "07314", SHIPPING_STATUS_REJECTED },
				{ "07315", SHIPPING_STATUS_REJECTED },
				{ "07316", SHIPPING_STATUS_REJECTED },
				{ "07317", SHIPPING_STATUS_REJECTED },
				{ "07321", SHIPPING_STATUS_REJECTED },
				{ "07322", SHIPPING_STATUS_DELIVERING },
				{ "07323", SHIPPING_STATUS_DELIVERING },
				{ "07324", SHIPPING_STATUS_DELIVERING },
				{ "07325", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07331", SHIPPING_STATUS_DELIVERING },
				{ "07333", SHIPPING_STATUS_DELIVERING },
				{ "07361", SHIPPING_STATUS_REJECTED },
				{ "07391", SHIPPING_STATUS_DELIVERING },
				{ "07411", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07421", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07422", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "07431", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07432", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07441", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07442", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07443", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07444", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07451", SHIPPING_STATUS_DELIVERING },
				{ "07452", SHIPPING_STATUS_DELIVERING },
				{ "07453", SHIPPING_STATUS_DELIVERING },
				{ "07454", SHIPPING_STATUS_DELIVERING },
				{ "07456", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07457", SHIPPING_STATUS_MISSED_OR_FIXED },
				{ "07461", SHIPPING_STATUS_DELIVERING },
				{ "07462", SHIPPING_STATUS_DELIVERING },
				{ "07463", SHIPPING_STATUS_DELIVERING },
				{ "07465", SHIPPING_STATUS_DELIVERING },
				{ "07466", SHIPPING_STATUS_DELIVERING },
				{ "07467", SHIPPING_STATUS_ABNORMAL },
				{ "07468", SHIPPING_STATUS_DELIVERING },
				{ "07469", SHIPPING_STATUS_ABNORMAL },
				{ "07471", SHIPPING_STATUS_DELIVERING },
				{ "07501", SHIPPING_STATUS_ABNORMAL },
				{ "07502", SHIPPING_STATUS_ABNORMAL },
				{ "07503", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07504", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07505", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07511", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07512", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07513", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07514", SHIPPING_STATUS_ABNORMAL },
				{ "08101", SHIPPING_STATUS_DELIVERING },
				{ "08102", SHIPPING_STATUS_DELIVERING },
				{ "08103", SHIPPING_STATUS_DELIVERING },
				{ "08104", SHIPPING_STATUS_DELIVERING },
				{ "08105", SHIPPING_STATUS_DELIVERING },
				{ "08106", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "08107", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "08108", SHIPPING_STATUS_ABNORMAL },
				{ "08109", SHIPPING_STATUS_ABNORMAL },
				{ "0810A", SHIPPING_STATUS_REJECTED },
				{ "0810B", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "0810C", SHIPPING_STATUS_CHANGED_ADDRESS },
				{ "0810D", SHIPPING_STATUS_DELIVERING },
				{ "0810E", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "0810F", SHIPPING_STATUS_ABNORMAL },
				{ "0810G", SHIPPING_STATUS_DELIVERING },
				{ "0810H", SHIPPING_STATUS_DELIVERING },
				{ "0810I", SHIPPING_STATUS_RETURNED },
				{ "08201", SHIPPING_STATUS_DELIVERING },
				{ "08202", SHIPPING_STATUS_DELIVERING },
				{ "08203", SHIPPING_STATUS_DELIVERING },
				{ "08204", SHIPPING_STATUS_DELIVERING },
				{ "08205", SHIPPING_STATUS_ABNORMAL },
				{ "08206", SHIPPING_STATUS_ABNORMAL },
				{ "07601", SHIPPING_STATUS_RETURNED },
				{ "07602", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07603", SHIPPING_STATUS_DEMAGED_PRODUCT },
				{ "07605", SHIPPING_STATUS_REJECTED },
				{ "07607", SHIPPING_STATUS_REJECTED },
				{ "07608", SHIPPING_STATUS_REJECTED },
				{ "07609", SHIPPING_STATUS_RETURNED }
			};

			return ((inputConvertShippingStatus.ContainsKey(twPelicanNo) == false)
				? string.Empty
				: StringUtility.ToEmpty(inputConvertShippingStatus[twPelicanNo]));
		}
		#endregion

		#region Properties
		/// <summary>The File Transfer Protocol object</summary>
		protected FluentFtpUtility Ftp { get; private set; }
		/// <summary>The file name</summary>
		public string FileExtension { get; set; }
		#endregion
	}
}