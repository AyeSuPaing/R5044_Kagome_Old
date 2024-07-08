/*
=========================================================================================================
  Module      : Import New Yahoo Order(ImportNewYahooOrder.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Mall.Yahoo.YahooMallOrders;
using w2.App.Common.MallCooperation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.CountryLocation;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;
using w2.Domain.ProductTaxCategory;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Commerce.Batch.ExternalFileImport.Imports
{
	/// <summary>
	/// Import new yahoo order
	/// </summary>
	class ImportNewYahooOrder : ImportBase
	{
		#region Constants
		/// <summary>Setting key: order payment kbn and payment status</summary>
		private const string CONST_SETTING_KEY_ORDER_PAYMENT_KBN_AND_PAYMENT_STATUS_SETTINGS = "OrderPaymentKbnAndPaymentStatusSettings";
		/// <summary>Setting key: order status</summary>
		private const string CONST_SETTING_KEY_ORDER_STATUS_SETTINGS = "OrderStatusSettings";
		/// <summary>Setting key: payment kbn</summary>
		private const string CONST_SETTING_KEY_PAYMENT_KBN = "payment_kbn";
		/// <summary>Setting key: payment status</summary>
		private const string CONST_SETTING_KEY_PAYMENT_STATUS = "payment_status";
		/// <summary>Setting key: value</summary>
		private const string CONST_SETTING_KEY_VALUE = "value";

		/// <summary>Device type: PC : 1</summary>
		private const string CONST_DEVICE_TYPE_PC = "1";
		/// <summary>Device type: Tablet : 4</summary>
		private const string CONST_DEVICE_TYPE_TABLET = "4";

		/// <summary>Const field order id</summary>
		private const string CONST_FIELD_ORDER_ID = "OrderId";
		/// <summary>Const field bill mail address</summary>
		private const string CONST_FIELD_BILL_MAILADDRESS = "BillMailAddress";
		/// <summary>Const field gift wrap charge</summary>
		private const string CONST_FIELD_GIFT_WRAP_CHARGE = "GiftWrapCharge";
		/// <summary>Const field use point</summary>
		private const string CONST_FIELD_USE_POINT = "UsePoint";
		/// <summary>Const field device type</summary>
		private const string CONST_FIELD_DEVICE_TYPE = "DeviceType";
		/// <summary>Const field order status</summary>
		private const string CONST_FIELD_ORDER_STATUS = "OrderStatus";
		/// <summary>Const field bill last name</summary>
		private const string CONST_FIELD_BILL_LAST_NAME = "BillLastName";
		/// <summary>Const field bill first name</summary>
		private const string CONST_FIELD_BILL_FIRST_NAME = "BillFirstName";
		/// <summary>Const field bill last name kana</summary>
		private const string CONST_FIELD_BILL_LAST_NAME_KANA = "BillLastNameKana";
		/// <summary>Const field bill first name kana</summary>
		private const string CONST_FIELD_BILL_FIRST_NAME_KANA = "BillFirstNameKana";
		/// <summary>Const field bill zipcode</summary>
		private const string CONST_FIELD_BILL_ZIPCODE = "BillZipCode";
		/// <summary>Const field bill prefecture</summary>
		private const string CONST_FIELD_BILL_PREFECTURE = "BillPrefecture";
		/// <summary>Const field bill city</summary>
		private const string CONST_FIELD_BILL_CITY = "BillCity";
		/// <summary>Const field bill address1</summary>
		private const string CONST_FIELD_BILL_ADDRESS1 = "BillAddress1";
		/// <summary>Const field bill address2</summary>
		private const string CONST_FIELD_BILL_ADDRESS2 = "BillAddress2";
		/// <summary>Const field bill phone number</summary>
		private const string CONST_FIELD_BILL_PHONENUMBER = "BillPhoneNumber";
		/// <summary>Const field ship last name</summary>
		private const string CONST_FIELD_SHIP_LAST_NAME = "ShipLastName";
		/// <summary>Const field ship first name</summary>
		private const string CONST_FIELD_SHIP_FIRST_NAME = "ShipFirstName";
		/// <summary>Const field ship last name kana</summary>
		private const string CONST_FIELD_SHIP_LAST_NAME_KANA = "ShipLastNameKana";
		/// <summary>Const field ship first name kana</summary>
		private const string CONST_FIELD_SHIP_FIRST_NAME_KANA = "ShipFirstNameKana";
		/// <summary>Const field ship zipcode</summary>
		private const string CONST_FIELD_SHIP_ZIPCODE = "ShipZipCode";
		/// <summary>Const field ship prefecture</summary>
		private const string CONST_FIELD_SHIP_PREFECTURE = "ShipPrefecture";
		/// <summary>Const field ship city</summary>
		private const string CONST_FIELD_SHIP_CITY = "ShipCity";
		/// <summary>Const field ship address1</summary>
		private const string CONST_FIELD_SHIP_ADDRESS1 = "ShipAddress1";
		/// <summary>Const field ship address2</summary>
		private const string CONST_FIELD_SHIP_ADDRESS2 = "ShipAddress2";
		/// <summary>Const field ship phone number</summary>
		private const string CONST_FIELD_SHIP_PHONE_NUMBER = "ShipPhoneNumber";
		/// <summary>Const field ship request date</summary>
		private const string CONST_FIELD_SHIP_REQUEST_DATE = "ShipRequestDate";
		/// <summary>Const field ship no date request</summary>
		private const string CONST_FIELD_SHIP_NO_DATE_REQUEST = "お届け日指定なし";
		/// <summary>Const field ship request time</summary>
		private const string CONST_FIELD_SHIP_REQUEST_TIME = "ShipRequestTime";
		/// <summary>Const field pay method</summary>
		private const string CONST_FIELD_PAY_METHOD = "PayMethod";
		/// <summary>Const field order time</summary>
		private const string CONST_FIELD_ORDER_TIME = "OrderTime";
		/// <summary>Const field ship charge</summary>
		private const string CONST_FIELD_SHIP_CHARGE = "ShipCharge";
		/// <summary>Const field pay charge</summary>
		private const string CONST_FIELD_PAY_CHARGE = "PayCharge";
		/// <summary>Const field total price</summary>
		private const string CONST_FIELD_TOTAL_PRICE = "TotalPrice";
		/// <summary>Const field item coupon discount</summary>
		private const string CONST_FIELD_ITEM_COUPON_DISCOUNT = "ItemCouponDiscount";
		/// <summary>Const field card pay count</summary>
		private const string CONST_FIELD_CARD_PAY_COUNT = "CardPayCount";
		/// <summary>Const field buyer comments</summary>
		private const string CONST_FIELD_BUYER_COMMENTS = "BuyerComments";
		/// <summary>Const field gift wrap message</summary>
		private const string CONST_FIELD_GIFT_WRAP_MESSAGE = "GiftWrapMessage";
		/// <summary>Const field ship method name</summary>
		private const string CONST_FIELD_SHIP_METHOD_NAME = "ShipMethodName";
		/// <summary>Const field item id</summary>
		private const string CONST_FIELD_ITEM_ID = "ItemId";
		/// <summary>Const field sub code</summary>
		private const string CONST_FIELD_SUB_CODE = "SubCode";
		/// <summary>Const field title</summary>
		private const string CONST_FIELD_TITLE = "Title";
		/// <summary>Const field unit price</summary>
		private const string CONST_FIELD_UNIT_PRICE = "UnitPrice";
		/// <summary>Const field quantity detail</summary>
		private const string CONST_FIELD_QUANTITY_DETAIL = "QuantityDetail";
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">Mall id</param>
		public ImportNewYahooOrder(string shopId, string mallId)
			: base(shopId, Constants.FLG_EXTERNALIMPORT_FILE_TYPE_NEW_YAHOO_ORDER)
		{
			this.OrderPriceByTaxRates = new List<OrderPriceByTaxRateModel>();
			this.Countries = DomainFacade.Instance.CountryLocationService.GetCountryNames();
			this.UserService = DomainFacade.Instance.UserService;
			this.OrderService = DomainFacade.Instance.OrderService;
			this.ProductService = DomainFacade.Instance.ProductService;
			this.ProductTaxCategoryService = DomainFacade.Instance.ProductTaxCategoryService;
			this.ProductStockService = DomainFacade.Instance.ProductStockService;
			this.ProductStockHistoryService = DomainFacade.Instance.ProductStockHistoryService;
			this.ShopShippingService = DomainFacade.Instance.ShopShippingService;
			this.DeliveryCompanyService = DomainFacade.Instance.DeliveryCompanyService;
			this.UpdateHistoryService = DomainFacade.Instance.UpdateHistoryService;
			this.MallWatchingLogManager = new MallWatchingLogManager();
			this.MallId = mallId;
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		/// <param name="activeFilePath">取り込みファイルパス</param>
		/// <returns>取込件数</returns>
		public override int Import(string activeFilePath)
		{
			var importCount = 0;

			// Check mall setting can import action
			if (CanImport() == false) return importCount;

			// 現在対象行設定（１行目＝ヘッダフィールド）
			this.CurrentLineCount = 1;

			using (var fs = new FileStream(activeFilePath, FileMode.Open))
			using (var sr = new StreamReader(fs, Encoding.GetEncoding(Constants.CONST_ENCODING_DEFAULT)))
			{
				var allHeaders = new ArrayList(StringUtility.SplitCsvLine(sr.ReadLine()));
				this.CurrentLineCount++;

				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					while (sr.EndOfStream == false)
					{
						var orderId = string.Empty;
						var lineBuffer = sr.ReadLine();

						while ((((lineBuffer.Length - lineBuffer.Replace("\"", string.Empty).Length) % 2) != 0)
							&& (sr.EndOfStream == false))
						{
							lineBuffer += (Environment.NewLine + sr.ReadLine());
						}

						var lineDatas = StringUtility.SplitCsvLine(lineBuffer);
						if (allHeaders.Count != lineDatas.Length)
						{
							FileLogger.WriteError(
								string.Format(
									"ヘッダのフィールド数とフィールド数が一致しません({0}行目)",
									this.CurrentLineCount++));
							continue;
						}

						var orderIdTemp = lineDatas[allHeaders.IndexOf(CONST_FIELD_ORDER_ID)];
						if (string.IsNullOrEmpty(orderIdTemp))
						{
							FileLogger.WriteError(
								string.Format(
									"{0}行目:注文IDを取得できません。",
									this.CurrentLineCount++));
							continue;
						}

						if (orderId != orderIdTemp)
						{
							var isNewUser = false;
							var user = new UserModel();
							var order = new OrderModel();
							var orderItems = new List<OrderItemModel>();
							var orderOwner = new OrderOwnerModel();
							var orderShipping = new OrderShippingModel();
							var userId = string.Empty;
							orderId = orderIdTemp;

							try
							{
								// Check order exists
								var orderForCheck = this.OrderService.GetOrderInfoByOrderId(orderId, accessor);
								if (orderForCheck != null)
								{
									FileLogger.WriteError(
										string.Format(
											"{0}行目:注文IDは既に登録済みです。(order_id:{1})",
											this.CurrentLineCount++,
											orderId));
									continue;
								}

								// Check user exists
								userId = StringUtility.ToEmpty(
									this.UserService.GetUserId(
										this.MallId,
										StringUtility.ToEmpty(lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_MAILADDRESS)]),
										accessor));
								isNewUser = string.IsNullOrEmpty(userId);
								if (isNewUser)
								{
									userId = Domain.User.UserService.CreateNewUserId(
										Constants.CONST_DEFAULT_SHOP_ID,
										Constants.NUMBER_KEY_USER_ID,
										Constants.CONST_USER_ID_HEADER,
										Constants.CONST_USER_ID_LENGTH);
									user.UserId = userId;
								}
								else
								{
									user = this.UserService.Get(userId, accessor);
								}

								// Set user data
								SetUserData(orderId, user, allHeaders, lineDatas);

								// Set order items data
								SetOrderItemsData(
									orderId,
									orderItems,
									allHeaders,
									lineDatas,
									accessor);

								// Set order data
								SetOrderData(
									orderId,
									order,
									orderItems,
									allHeaders,
									lineDatas,
									userId,
									accessor);

								// Set order owner data
								SetOrderOwnerData(orderId, orderOwner, user);

								// Set order shipping data
								SetOrderShippingData(
									orderId,
									orderOwner,
									orderShipping,
									allHeaders,
									lineDatas,
									accessor);
							}
							catch (Exception ex)
							{
								var errorMessage = string.Format(
									"{0}行目:値チェック処理で例外エラーが発生しました。(order_id:{1})",
									this.CurrentLineCount++,
									orderId);

								this.MallWatchingLogManager.Insert(
									Constants.FLG_MALLWATCHINGLOG_BATCH_ID_EXTERNALFILEIMPORT,
									string.Empty,
									Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
									errorMessage);
								FileLogger.WriteError(errorMessage, ex);
								continue;
							}

							// Update database
							try
							{
								var isSuccess = false;
								if (isNewUser)
								{
									// Insert new user
									isSuccess = this.UserService.InsertWithUserExtend(
										user,
										Constants.FLG_LASTCHANGED_BATCH,
										UpdateHistoryAction.DoNotInsert,
										accessor);
									if (isSuccess == false)
									{
										throw new ApplicationException("In w2_User, Insert error.");
									}
								}
								else
								{
									// Update user
									isSuccess = this.UserService.UpdateWithUserExtend(
										user,
										UpdateHistoryAction.DoNotInsert,
										accessor);
									if (isSuccess == false)
									{
										throw new ApplicationException("In w2_User, Update error.");
									}
								}

								// Insert order
								order.Items = orderItems.ToArray();
								order.Owner = orderOwner;
								order.Shippings = new[] { orderShipping };
								order.OrderPriceByTaxRates = this.OrderPriceByTaxRates.ToArray();
								isSuccess = this.OrderService.InsertOrder(
									order,
									UpdateHistoryAction.DoNotInsert,
									accessor);
								if (isSuccess == false)
								{
									throw new ApplicationException("In w2_Order, Insert error.");
								}

								// Update product stock
								foreach (var item in order.Items)
								{
									UpdateProductStock(item, accessor);
								}

								var input = new Hashtable
								{
									{ Constants.FIELD_ORDER_USER_ID, userId },
									{
										Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME,
										this.UserService.Get(userId, accessor).OrderCountOrderRealtime
									},
								};
								OrderCommon.UpdateRealTimeOrderCount(
									input,
									Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER,
									accessor);

								this.UpdateHistoryService.InsertForOrder(orderId, Constants.FLG_LASTCHANGED_BATCH, accessor);
								this.UpdateHistoryService.InsertForUser(user.UserId, Constants.FLG_LASTCHANGED_BATCH, accessor);

								accessor.CommitTransaction();
							}
							catch (Exception ex)
							{
								accessor.RollbackTransaction();

								FileLogger.WriteError(
									string.Format(
										"{0}行目:DB更新処理で例外エラーが発生しました。(order_id:{1})",
										this.CurrentLineCount++,
										orderId),
									ex);
								continue;
							}
						}
						importCount++;
						this.CurrentLineCount++;
					}
				}
			}
			return importCount;
		}

		/// <summary>
		/// Can import
		/// </summary>
		/// <returns>True: execute import action</returns>
		private bool CanImport()
		{
			var setting = DomainFacade.Instance.MallCooperationSettingService.Get(
				m_strShopId,
				this.MallId);
			if (setting == null)
			{
				FileLogger.WriteError(
					string.Format(
						"モールIDの取得に失敗しました mall_id: {0}",
						this.MallId));
				return false;
			}

			if (setting.MallKbn != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO)
			{
				FileLogger.WriteError(
					string.Format(
						"モール連携設定でのモール区分がYahooではありません。mall_id: {0}, mall_kbn: {1}",
						this.MallId,
						setting.MallKbn));
				return false;
			}
			return true;
		}

		/// <summary>
		/// Set user data
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="user">User</param>
		/// <param name="allHeaders">All headers</param>
		/// <param name="lineDatas">Line datas</param>
		private void SetUserData(
			string orderId,
			UserModel user,
			ArrayList allHeaders,
			string[] lineDatas)
		{
			user.MallId = this.MallId;
			var mailAddress = lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_MAILADDRESS)];
			switch (lineDatas[allHeaders.IndexOf(CONST_FIELD_DEVICE_TYPE)])
			{
				case CONST_DEVICE_TYPE_PC:
				case CONST_DEVICE_TYPE_TABLET:
					user.UserKbn = Constants.FLG_USER_USER_KBN_PC_USER;
					user.MailAddr = mailAddress;
					user.MailAddr2 = string.Empty;
					break;

				default:
					FileLogger.WriteError(
						string.Format(
						"{0}行目:ユーザ区分が取得できません。(order_id:{1})",
						this.CurrentLineCount,
						orderId));
					break;
			}

			var name1 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_LAST_NAME)]);
			var name2 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_FIRST_NAME)]);
			user.Name = string.Format("{0}{1}", name1, name2);
			user.Name1 = name1;
			user.Name2 = name2;
			var nameKana1 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_LAST_NAME_KANA)]);
			var nameKana2 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_FIRST_NAME_KANA)]);
			user.NameKana = string.Format("{0}{1}", nameKana1, nameKana2);
			user.NameKana1 = nameKana1;
			user.NameKana2 = nameKana2;
			var billZip = ProcessingZip(orderId, lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_ZIPCODE)]);
			if (string.IsNullOrEmpty(billZip))
			{
				FileLogger.WriteError(
					string.Format(
						"{0}行目:郵便番号が正しくありません。(order_id:{1})",
						this.CurrentLineCount,
						orderId));
				throw new ApplicationException();
			}
			user.Zip = billZip;
			user.Zip1 = billZip.Substring(0, 3);
			user.Zip2 = billZip.Substring(4);
			var billPrefecture = lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_PREFECTURE)];
			var billCity = lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_CITY)];
			var billAddress1 = lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_ADDRESS1)];
			var billAddress2 = lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_ADDRESS2)];
			var billPhoneNumber = lineDatas[allHeaders.IndexOf(CONST_FIELD_BILL_PHONENUMBER)];
			user.Addr = StringUtility.ToZenkaku(
				string.Format(
					"{0}　{1}　{2}　{3}",
					billPrefecture,
					billCity,
					billAddress1,
					billAddress2));
			var prefecturesList = Constants.STR_PREFECTURES_LIST;
			if (prefecturesList.Contains(billPrefecture) == false)
			{
				FileLogger.WriteError(
					string.Format(
						"{0}行目:都道府県が正しくありません。(order_id:{1})",
						this.CurrentLineCount,
						orderId));
				throw new ApplicationException();
			}
			user.Addr1 = StringUtility.ToZenkaku(billPrefecture);
			user.Addr2 = StringUtility.ToZenkaku(billCity);
			user.Addr3 = StringUtility.ToZenkaku(billAddress1);
			user.Addr4 = StringUtility.ToZenkaku(billAddress2);
			user.Tel1 = ProcessingTelFormated(orderId, billPhoneNumber);
			var billPhones = ProcessingTel(orderId, billPhoneNumber);
			if (billPhones.Any() == false)
			{
				FileLogger.WriteError(
					string.Format(
						"{0}行目:電話番号が正しくありません。(order_id:{1})",
						this.CurrentLineCount,
						orderId));
				throw new ApplicationException();
			}
			user.Tel1_1 = billPhones[0];
			user.Tel1_2 = billPhones[1];
			user.Tel1_3 = billPhones[2];
			var japanCode = Constants.COUNTRY_ISO_CODE_JP;
			user.AddrCountryIsoCode = japanCode;
			user.AddrCountryName = this.Countries
				.Where(item => (item.CountryIsoCode == japanCode))
				.Select(item => item.CountryName)
				.FirstOrDefault() ?? "Japan";
			if (user.UserExtend == null)
			{
				user.UserExtend = UserExtendModel.CreateEmpty(
					user.UserId,
					DomainFacade.Instance.UserService.GetUserExtendSettingList());
			}
			user.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
		}

		/// <summary>
		/// Set order owner data
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderOwner">Order owner</param>
		/// <param name="user">User</param>
		private void SetOrderOwnerData(
			string orderId,
			OrderOwnerModel orderOwner,
			UserModel user)
		{
			orderOwner.OrderId = orderId;
			orderOwner.OwnerKbn = user.UserKbn;
			orderOwner.OwnerMailAddr = user.MailAddr;
			orderOwner.OwnerMailAddr2 = user.MailAddr2;
			orderOwner.OwnerName = user.Name;
			orderOwner.OwnerName1 = user.Name1;
			orderOwner.OwnerName2 = user.Name2;
			orderOwner.OwnerNameKana = user.NameKana;
			orderOwner.OwnerNameKana1 = user.NameKana1;
			orderOwner.OwnerNameKana2 = user.NameKana2;
			orderOwner.OwnerZip = user.Zip;
			orderOwner.OwnerAddr1 = user.Addr1;
			orderOwner.OwnerAddr2 = user.Addr2;
			orderOwner.OwnerAddr3 = user.Addr3;
			orderOwner.OwnerAddr4 = user.Addr4;
			orderOwner.OwnerTel1 = user.Tel1;
			orderOwner.OwnerAddrCountryName = user.AddrCountryName;
			orderOwner.OwnerAddrCountryIsoCode = user.AddrCountryIsoCode;
			orderOwner.DateChanged = DateTime.Now;
			orderOwner.DateCreated = DateTime.Now;
		}

		/// <summary>
		/// Set order shipping data
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderOwner">Order owner</param>
		/// <param name="orderShipping">Order shipping</param>
		/// <param name="allHeaders">All headers</param>
		/// <param name="lineDatas">Line datas</param>
		/// <param name="accessor">Sql accessor</param>
		private void SetOrderShippingData(
			string orderId,
			OrderOwnerModel orderOwner,
			OrderShippingModel orderShipping,
			ArrayList allHeaders,
			string[] lineDatas,
			SqlAccessor accessor)
		{
			var name1 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_LAST_NAME)]);
			var name2 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_FIRST_NAME)]);
			var name = string.Format("{0}{1}", name1, name2);
			var nameKana1 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_LAST_NAME_KANA)]);
			var nameKana2 = StringUtility.ToZenkaku(lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_FIRST_NAME_KANA)]);
			var nameKana = string.Format("{0}{1}", nameKana1, nameKana2);
			var shippingZip = ProcessingZip(orderId, lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_ZIPCODE)]);
			if (string.IsNullOrEmpty(shippingZip))
			{
				FileLogger.WriteError(
					string.Format("{0}行目:郵便番号が正しくありません。(order_id:{1})",
						this.CurrentLineCount,
						orderId));
				throw new ApplicationException();
			}
			var shippingAddr1 = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_PREFECTURE)];
			var shippingAddr2 = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_CITY)];
			var shippingAddr3 = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_ADDRESS1)];
			var shippingAddr4 = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_ADDRESS2)];
			var shippingTel1 = ProcessingTelFormated(orderId, lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_PHONE_NUMBER)]);
			DateTime? shippingDate = null;
			var shipRequestDate = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_REQUEST_DATE)];
			if ((string.IsNullOrEmpty(shipRequestDate) == false)
				&& (shipRequestDate != CONST_FIELD_SHIP_NO_DATE_REQUEST))
			{
				var shippingDateString = ProcessingDateTime(shipRequestDate);
				DateTime date;
				if (DateTime.TryParse(shippingDateString, out date))
				{
					shippingDate = date;
				}
			}

			var shopShipping = this.ShopShippingService.Get(m_strShopId, Constants.MALL_DEFAULT_SHIPPING_ID, accessor);
			if (shopShipping == null)
			{
				FileLogger.WriteError(
					string.Format("{0}行目:存在しない配送種別です。(order_id:{1}, shipping_id:{2})",
						this.CurrentLineCount,
						orderId,
						Constants.MALL_DEFAULT_SHIPPING_ID));
				throw new ApplicationException();
			}
			var defaultDeliveryCompany = shopShipping.GetDefaultDeliveryCompany(true);
			var shippingTime = string.Empty;
			var shipRequestTime = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_REQUEST_TIME)].Trim();
			if (string.IsNullOrEmpty(shipRequestTime) == false)
			{
				var deliveryCompany = this.DeliveryCompanyService.Get(defaultDeliveryCompany.DeliveryCompanyId, accessor);
				foreach (var item in deliveryCompany.GetShippingTimeList())
				{
					if (shipRequestTime.TrimStart('0').Replace("-", "～") == item.Value.TrimStart('0').Replace("-", "～"))
					{
						shippingTime = item.Key;
						break;
					}
				}

				if (string.IsNullOrEmpty(shippingTime))
				{
					FileLogger.WriteError(
						string.Format(
							"{0}行目:配送種別IDを取得できません。配送種別に配送希望時間帯を登録してください。({1} order_id:{2} shipping_time_message:{3})",
							this.CurrentLineCount,
							m_strFileType,
							orderId,
							shipRequestTime));
					throw new ApplicationException();
				}
			}

			var isAnotherShipping = ((name1 != orderOwner.OwnerName1)
				|| (name2 != orderOwner.OwnerName2)
				|| (shippingZip != orderOwner.OwnerZip)
				|| (shippingTel1 != orderOwner.OwnerTel1)
				|| (shippingAddr1 != orderOwner.OwnerAddr1)
				|| (shippingAddr2 != orderOwner.OwnerAddr2)
				|| (shippingAddr3 != orderOwner.OwnerAddr3)
				|| (shippingAddr4 != orderOwner.OwnerAddr4));
			var anotherShippingFlag = isAnotherShipping
				? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID
				: Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID;

			// Set data
			orderShipping.OrderId = orderId;
			orderShipping.ShippingName = name;
			orderShipping.ShippingName1 = name1;
			orderShipping.ShippingName2 = name2;
			orderShipping.ShippingNameKana = nameKana;
			orderShipping.ShippingNameKana1 = nameKana1;
			orderShipping.ShippingNameKana2 = nameKana2;
			orderShipping.ShippingZip = shippingZip;
			orderShipping.ShippingAddr1 = shippingAddr1;
			orderShipping.ShippingAddr2 = shippingAddr2;
			orderShipping.ShippingAddr3 = shippingAddr3;
			orderShipping.ShippingAddr4 = shippingAddr4;
			orderShipping.ShippingTel1 = shippingTel1;
			orderShipping.AnotherShippingFlg = anotherShippingFlag;
			orderShipping.ShippingDate = shippingDate;
			orderShipping.ShippingTime = shippingTime;
			orderShipping.DeliveryCompanyId = defaultDeliveryCompany.DeliveryCompanyId;
			orderShipping.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			orderShipping.ShippingCountryIsoCode = orderOwner.OwnerAddrCountryIsoCode;
			orderShipping.ShippingCountryName = orderOwner.OwnerAddrCountryName;
			orderShipping.DateCreated = DateTime.Now;
			orderShipping.DateChanged = DateTime.Now;
		}

		/// <summary>
		/// Set order data
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="order">Order</param>
		/// <param name="orderItems">Order items</param>
		/// <param name="allHeaders">All headers</param>
		/// <param name="lineDatas">Line datas</param>
		/// <param name="userId">User id</param>
		/// <param name="accessor">Sql accessor</param>
		private void SetOrderData(
			string orderId,
			OrderModel order,
			List<OrderItemModel> orderItems,
			ArrayList allHeaders,
			string[] lineDatas,
			string userId,
			SqlAccessor accessor)
		{
			switch (lineDatas[allHeaders.IndexOf(CONST_FIELD_DEVICE_TYPE)])
			{
				case CONST_DEVICE_TYPE_PC:
				case CONST_DEVICE_TYPE_TABLET:
					order.OrderKbn = Constants.FLG_USER_USER_KBN_PC_USER;
					break;

				default:
					FileLogger.WriteError(
						string.Format(
							"{0}行目:注文者区分が取得できません。(order_id:{1})",
							this.CurrentLineCount,
							orderId));
					throw new ApplicationException();
			}

			order.ShopId = m_strShopId;
			order.OrderId = orderId;
			order.MallId = this.MallId;

			var orderStatusValue = lineDatas[allHeaders.IndexOf(CONST_FIELD_ORDER_STATUS)];
			try
			{
				order.OrderStatus = ConvertValue(
					CONST_SETTING_KEY_ORDER_STATUS_SETTINGS,
					orderStatusValue,
					CONST_SETTING_KEY_VALUE).Trim();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(
					string.Format(
						"{0}行目:注文ステータス が取得できません。(order_id:{1})",
						this.CurrentLineCount,
						orderId),
					ex);
				throw new ApplicationException();
			}

			order.ShippingId = Constants.MALL_DEFAULT_SHIPPING_ID;
			order.CardInstruments = lineDatas[allHeaders.IndexOf(CONST_FIELD_CARD_PAY_COUNT)];

			// Get and check payment method setting
			var paymentKbn = string.Empty;
			var paymentStatus = string.Empty;
			var paymentMethod = lineDatas[allHeaders.IndexOf(CONST_FIELD_PAY_METHOD)];
			var payMethodAmount = lineDatas[allHeaders.IndexOf("PayMethodAmount")];
			var combinedPayMethodAmount = lineDatas[allHeaders.IndexOf("CombinedPayMethodAmount")];
			var combinedPayMethod = lineDatas[allHeaders.IndexOf("CombinedPayMethod")];
			var totalMallCouponDiscount = lineDatas[allHeaders.IndexOf("TotalMallCouponDiscount")];
			try
			{
				paymentKbn = ConvertValue(
					CONST_SETTING_KEY_ORDER_PAYMENT_KBN_AND_PAYMENT_STATUS_SETTINGS,
					paymentMethod,
					CONST_SETTING_KEY_PAYMENT_KBN).Trim();
				paymentStatus = ConvertValue(
					CONST_SETTING_KEY_ORDER_PAYMENT_KBN_AND_PAYMENT_STATUS_SETTINGS,
					paymentMethod,
					CONST_SETTING_KEY_PAYMENT_STATUS).Trim();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(
					string.Format(
						"{0}行目:決済種別が取得できません。(order_id:{1})",
						this.CurrentLineCount,
						orderId),
					ex);
				throw new ApplicationException();
			}

			order.OrderPaymentKbn = paymentKbn;
			order.OrderPaymentStatus = paymentStatus;

			// Get and check order date time
			var orderTime = lineDatas[allHeaders.IndexOf(CONST_FIELD_ORDER_TIME)];
			if (string.IsNullOrEmpty(orderTime) == false)
			{
				DateTime date;
				orderTime = ProcessingDateTime(orderTime);
				order.OrderDate = (DateTime.TryParse(orderTime, out date))
					? date
					: DateTime.Now;
			}
			else
			{
				FileLogger.WriteError(
					string.Format(
						"{0}行目:注文日を取得できません。(order_id:{1})",
						this.CurrentLineCount,
						orderId));
				throw new ApplicationException();
			}

			// Set order price and tax informations
			order.OrderItemCount = orderItems.Count;
			order.OrderProductCount = orderItems.Sum(oi => oi.ItemQuantity);
			order.OrderPriceSubtotal = orderItems.Sum(oi => oi.ItemPrice);
			order.OrderPriceSubtotalTax = orderItems.Sum(oi => oi.ItemPriceTax);
			order.OrderPriceShipping = decimal.Parse(lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_CHARGE)]);
			order.OrderPriceExchange = decimal.Parse(lineDatas[allHeaders.IndexOf(CONST_FIELD_PAY_CHARGE)]);
			order.ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE;
			order.PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE;

			var priceTotal = lineDatas[allHeaders.IndexOf(CONST_FIELD_TOTAL_PRICE)];
			var giftWrapCharge = lineDatas[allHeaders.IndexOf(CONST_FIELD_GIFT_WRAP_CHARGE)];
			var itemCouponDiscount = lineDatas[allHeaders.IndexOf(CONST_FIELD_ITEM_COUPON_DISCOUNT)];
			var usePoint = lineDatas[allHeaders.IndexOf(CONST_FIELD_USE_POINT)];

			var priceTotalValue = ConvertValueToDecimal(priceTotal);
			var giftWrapChargeValue = ConvertValueToDecimal(giftWrapCharge);
			var itemCouponDiscountValue = ConvertValueToDecimal(itemCouponDiscount);
			var usePointValue = ConvertValueToDecimal(usePoint);

			// 決済
			var orderPayment = new YahooMallOrderPayment(
				paymentMethod,
				combinedPayMethod,
				payMethodAmount,
				combinedPayMethodAmount,
				priceTotal,
				usePoint,
				totalMallCouponDiscount,
				giftWrapCharge,
				new YahooMallOrderPaymentMapper());
			order.OrderPriceRegulation = orderPayment.OrderPriceRegulation;
			order.OrderPriceTotal = priceTotalValue;
			order.LastBilledAmount = priceTotalValue;

			order.OrderTaxRoundType = Constants.TAX_ROUNDTYPE;
			this.OrderPriceByTaxRates = GetOrderPriceByTaxRates(
				order,
				orderItems,
				allHeaders,
				lineDatas);
			order.OrderPriceTax = this.OrderPriceByTaxRates.Sum(item => item.TaxPriceByRate);

			// Set user
			order.UserId = userId;
			var user = this.UserService.Get(userId, accessor);
			var countOrder = ((user == null) ? 0 : user.OrderCountOrderRealtime);
			order.OrderCountOrder = (countOrder + 1);

			// Set memo informations
			var buyerComments = lineDatas[allHeaders.IndexOf(CONST_FIELD_BUYER_COMMENTS)];
			var giftWrapMessage = lineDatas[allHeaders.IndexOf(CONST_FIELD_GIFT_WRAP_MESSAGE)];
			var shipMethodName = lineDatas[allHeaders.IndexOf(CONST_FIELD_SHIP_METHOD_NAME)];
			var memo = new StringBuilder();
			memo.Append(string.IsNullOrEmpty(buyerComments)
				? string.Empty
				: string.Format("\r\n－－コメント－－\r\n{0}", buyerComments));
			memo.Append(string.IsNullOrEmpty(giftWrapMessage)
				? string.Empty
				: string.Format("\r\n－－ギフトメッセージ－－\r\n{0}", giftWrapMessage));
			order.Memo = memo.ToString().Trim();
			var relationMemo = new StringBuilder();
			relationMemo.Append(string.IsNullOrEmpty(shipMethodName)
				? string.Empty
				: string.Format("\r\n－－お届け方法－－\r\n{0}", shipMethodName));
			order.RelationMemo = relationMemo.ToString().Trim();
			var regulationMemo = new StringBuilder();
			regulationMemo.Append((giftWrapChargeValue != 0)
				? string.Format("\r\n－－ギフト手数料－－\r\n{0}円", giftWrapChargeValue)
				: string.Empty);
			regulationMemo.Append((itemCouponDiscountValue != 0)
				? string.Format("－－\r\nクーポン利用分－－\r\n{0}円", itemCouponDiscountValue)
				: string.Empty);
			regulationMemo.Append((usePointValue != 0)
				? string.Format("－－\r\nポイント利用分－－\r\n{0}円", usePointValue)
				: string.Empty);
			regulationMemo.Append((paymentKbn == Constants.PAYMENT_METHOD_PAYPAY)
				? orderPayment.RegulationMemo
				: string.Empty);
			order.RegulationMemo = regulationMemo.ToString().Trim();

			order.DateChanged = DateTime.Now;
			order.DateCreated = DateTime.Now;
			order.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
		}

		/// <summary>
		/// Set order items data
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderItems">Order items</param>
		/// <param name="allHeaders">All headers</param>
		/// <param name="lineDatas">Line datas</param>
		/// <param name="accessor">Sql accessor</param>
		void SetOrderItemsData(
			string orderId,
			List<OrderItemModel> orderItems,
			ArrayList allHeaders,
			string[] lineDatas,
			SqlAccessor accessor)
		{
			// Get list data of items
			var productIdList = GetListData(lineDatas[allHeaders.IndexOf(CONST_FIELD_ITEM_ID)]);
			var variationIdList = GetListData(lineDatas[allHeaders.IndexOf(CONST_FIELD_SUB_CODE)]);
			var productNameList = GetListData(lineDatas[allHeaders.IndexOf(CONST_FIELD_TITLE)]);
			var productPricePreTaxList = GetListData(lineDatas[allHeaders.IndexOf(CONST_FIELD_UNIT_PRICE)]);
			var itemQualityList = GetListData(lineDatas[allHeaders.IndexOf(CONST_FIELD_QUANTITY_DETAIL)]);

			// Create order items
			var productService = DomainFacade.Instance.ProductService;
			var productTaxCategoryService = DomainFacade.Instance.ProductTaxCategoryService;
			for (var index = 0; index < productIdList.Length; index++)
			{
				var orderItemNo = (index + 1);
				var productId = productIdList[index];
				var variationId = variationIdList.Any()
					? variationIdList[index]
					: string.Empty;
				var productVariationId = (productId + variationId);
				var productName = productNameList[index];
				var productPricePretax = decimal.Parse(productPricePreTaxList[index]);
				var itemQuality = int.Parse(itemQualityList[index]);

				// Get and check product information
				var product = productService.GetProductVariation(
					m_strShopId,
					productId,
					productVariationId,
					string.Empty,
					accessor);
				if (product == null)
				{
					FileLogger.WriteError(
						string.Format(
							"{0}行目:商品情報が見つかりませんでした。(order_id: {1}, product_id: {2}, variation_id: {3})",
							this.CurrentLineCount,
							orderId,
							productId,
							productVariationId));
					throw new ApplicationException();
				}

				// Get product tax information
				var taxCategoryId = product.TaxCategoryId;
				var taxRate = productTaxCategoryService.Get(taxCategoryId).TaxRate;
				var productPriceTax = TaxCalculationUtility.GetTaxPrice(
					product.Price,
					taxRate,
					Constants.TAX_ROUNDTYPE,
					true);
				var productPrice = TaxCalculationUtility.GetPrescribedPrice(product.Price, productPriceTax, true);
				var productTaxIncludedFlag = TaxCalculationUtility.GetPrescribedProductTaxIncludedFlag();
				var itemPrice = (productPrice * itemQuality);
				var itemPriceTax = (productPriceTax * itemQuality);
				var orderItem = new OrderItemModel
				{
					ShopId = m_strShopId,
					OrderId = orderId,
					OrderItemNo = orderItemNo,
					ProductId = productId,
					VariationId = productVariationId,
					ProductName = productName,
					ProductPrice = productPrice,
					ProductTaxIncludedFlg = productTaxIncludedFlag,
					ProductTaxRate = taxRate,
					ProductTaxRoundType = Constants.TAX_ROUNDTYPE,
					ProductPricePretax = productPricePretax,
					ItemQuantity = itemQuality,
					ItemQuantitySingle = itemQuality,
					ItemPrice = itemPrice,
					ItemPriceTax = itemPriceTax,
					ItemPriceSingle = itemPrice,
					BrandId = product.BrandId1,
					CooperationId1 = product.CooperationId1,
					CooperationId2 = product.CooperationId2,
					CooperationId3 = product.CooperationId3,
					CooperationId4 = product.CooperationId4,
					CooperationId5 = product.CooperationId5,
					CooperationId6 = product.CooperationId6,
					CooperationId7 = product.CooperationId7,
					CooperationId8 = product.CooperationId8,
					CooperationId9 = product.CooperationId9,
					CooperationId10 = product.CooperationId10,
					DateChanged = DateTime.Now,
					DateCreated = DateTime.Now,
				};
				orderItems.Add(orderItem);
			}
		}

		/// <summary>
		/// Update product stock
		/// </summary>
		/// <param name="item">Item</param>
		/// <param name="accessor">Sql accessor</param>
		private void UpdateProductStock(OrderItemModel item, SqlAccessor accessor)
		{
			var shopId = item.ShopId;
			var orderId = item.OrderId;
			var productId = item.ProductId;
			var variationId = item.VariationId;
			var itemQantity = item.ItemQuantity;
			var product = this.ProductService.GetProductVariation(
				shopId,
				productId,
				variationId,
				string.Empty,
				accessor);
			if (product.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
			{
				var stockUpdated = this.ProductStockService.AddProductStock(
					shopId,
					productId,
					variationId,
					itemQantity * -1,
					0,
					0,
					0,
					0,
					Constants.FLG_LASTCHANGED_BATCH,
					accessor);
				if (stockUpdated <= 0)
				{
					FileLogger.WriteError(
						string.Format(
							"{0}行目:商品バリエーションID:{1}の在庫を更新できませんでした。商品番号が誤っているか、在庫数が取得できなかった可能性があります。注文ID:{2}",
							this.CurrentLineCount,
							variationId,
							orderId));
					throw new ApplicationException();
				}

				var stockHistory = new ProductStockHistoryModel
				{
					ShopId = shopId,
					OrderId = orderId,
					ProductId = productId,
					VariationId = variationId,
					ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER,
					AddStock = (-1 * itemQantity),
					LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				};
				var stockHistoryUpdated = this.ProductStockHistoryService.Insert(stockHistory, accessor);
				if (stockHistoryUpdated <= 0)
				{
					FileLogger.WriteError(
						string.Format(
							"{0}行目:商品バリエーションID:{1}の在庫を更新できませんでした。商品番号が誤っているか、在庫数が取得できなかった可能性があります。注文ID:{2}",
							this.CurrentLineCount,
							variationId,
							orderId));
					throw new ApplicationException();
				}
			}
		}

		/// <summary>
		/// Get order price by tax rates
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="orderItems">Order items</param>
		/// <param name="allHeaders">All headers</param>
		/// <param name="lineDatas">Line datas</param>
		/// <returns>List order price by tax rate model</returns>
		private List<OrderPriceByTaxRateModel> GetOrderPriceByTaxRates(
			OrderModel order,
			List<OrderItemModel> orderItems,
			ArrayList allHeaders,
			string[] lineDatas)
		{
			var stackedDiscountAmount = 0m;
			var priceTotal = orderItems.Sum(oi => (oi.ProductPricePretax * oi.ItemQuantity));
			var paymentPrice = order.OrderPriceExchange;
			var shippingPrice = order.OrderPriceShipping;

			var giftWrapCharge = lineDatas[allHeaders.IndexOf(CONST_FIELD_GIFT_WRAP_CHARGE)];
			var itemCouponDiscount = lineDatas[allHeaders.IndexOf(CONST_FIELD_ITEM_COUPON_DISCOUNT)];
			var usePoint = lineDatas[allHeaders.IndexOf(CONST_FIELD_USE_POINT)];

			var giftWrapChargeValue = ConvertValueToDecimal(giftWrapCharge);
			var itemCouponDiscountValue = ConvertValueToDecimal(itemCouponDiscount);
			var usePointValue = ConvertValueToDecimal(usePoint);

			if (usePointValue > 0) usePointValue = (-1 * usePointValue);
			var totalRegulation = giftWrapChargeValue + usePointValue + itemCouponDiscountValue;

			priceTotal += paymentPrice;
			priceTotal += shippingPrice;
			var shippingRegulationPrice = 0m;
			var paymentRegulationPrice = 0m;
			if ((priceTotal != 0) && (totalRegulation != 0))
			{
				foreach (var item in orderItems)
				{
					item.ItemPriceRegulation = (item.ProductPricePretax * item.ItemQuantity) / (priceTotal * totalRegulation);
					stackedDiscountAmount += item.ItemPriceRegulation;
				}

				shippingRegulationPrice = Math.Floor(shippingPrice / priceTotal * totalRegulation);
				stackedDiscountAmount += shippingRegulationPrice;

				paymentRegulationPrice = Math.Floor(paymentPrice / priceTotal * totalRegulation);
				stackedDiscountAmount += paymentRegulationPrice;
			}

			var fractionAmount = totalRegulation - stackedDiscountAmount;
			if (fractionAmount != 0)
			{
				var weightItem = orderItems.FirstOrDefault(item => (item.ProductPricePretax * item.ItemQuantity) > 0);
				if (weightItem != null)
				{
					weightItem.ItemPriceRegulation += fractionAmount;
				}
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += fractionAmount;
				}
				else
				{
					paymentRegulationPrice += fractionAmount;
				}
			}

			var priceInfo = new List<Hashtable>();
			priceInfo.AddRange(orderItems
				.Select(item =>
					new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, item.ProductTaxRate },
						{
							Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE,
							(item.ProductPricePretax * item.ItemQuantity) + item.ItemPriceRegulation
						},
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
					})
				.ToList());

			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, order.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, shippingPrice + shippingRegulationPrice },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
			});
			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, order.PaymentTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, paymentPrice + paymentRegulationPrice },
			});

			var groupedItem = priceInfo.GroupBy(item => new
			{
				taxRate = (decimal)item[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
			});
			var priceByTaxRates = groupedItem
				.Select(item =>
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = item.Key.taxRate,
						PriceSubtotalByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
						PriceShippingByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
						PricePaymentByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
						PriceTotalByRate = item.Sum(itemKey =>
							((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(item.Sum(itemKey =>
								((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
									+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
									+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
							(decimal)item.Key.taxRate,
							Constants.TAX_ROUNDTYPE,
							true)
					})
				.ToList();
			foreach (var orderPriceByTaxRateModel in priceByTaxRates)
			{
				orderPriceByTaxRateModel.OrderId = order.OrderId;
				orderPriceByTaxRateModel.DateChanged = DateTime.Now;
				orderPriceByTaxRateModel.DateCreated = DateTime.Now;
			}
			return priceByTaxRates;
		}

		/// <summary>
		/// Get list data
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>List data</returns>
		private string[] GetListData(string data)
		{
			var listData = string.IsNullOrEmpty(data)
				? new List<string>().ToArray()
				: data
					.Split('&')
					.Select(item => item.Substring(item.IndexOf("=") + 1))
					.ToArray();
			return listData;
		}

		/// <summary>
		/// Convert value to decimal
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Number</returns>
		private decimal ConvertValueToDecimal(string input)
		{
			var parttern = @"^\d+$";
			if (Regex.IsMatch(input, parttern)) return Convert.ToDecimal(input);
			return 0;
		}

		#region Properties
		/// <summary>Order price by tax rates</summary>
		private List<OrderPriceByTaxRateModel> OrderPriceByTaxRates { get; set; }
		/// <summary>Countries</summary>
		private CountryLocationModel[] Countries { get; set; }
		/// <summary>User service</summary>
		private IUserService UserService { get; set; }
		/// <summary>Order service</summary>
		private IOrderService OrderService { get; set; }
		/// <summary>Product service</summary>
		private IProductService ProductService { get; set; }
		/// <summary>Product tax category service</summary>
		private IProductTaxCategoryService ProductTaxCategoryService { get; set; }
		/// <summary>Product stock service</summary>
		private IProductStockService ProductStockService { get; set; }
		/// <summary>Product stock history service</summary>
		private IProductStockHistoryService ProductStockHistoryService { get; set; }
		/// <summary>Shop shipping service</summary>
		private IShopShippingService ShopShippingService { get; set; }
		/// <summary>Delivery company service</summary>
		private IDeliveryCompanyService DeliveryCompanyService { get; set; }
		/// <summary>Update history service</summary>
		private IUpdateHistoryService UpdateHistoryService { get; set; }
		/// <summary>Mall watching log manager</summary>
		private MallWatchingLogManager MallWatchingLogManager { get; set; }
		/// <summary>Current line count</summary>
		private int CurrentLineCount { get; set; }
		/// <summary>Mall id</summary>
		private string MallId { get; set; }
		#endregion
	}
}
