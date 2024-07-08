/*
=========================================================================================================
  Module      : 注文情報更新データ（前後）リスト作成クラス (BeforeAndAfterUpdateDataListCreatorOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Web.Page;
using w2.Domain.DeliveryCompany;
using w2.Domain.Payment;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper.UpdateData;
using w2.Domain.UpdateHistory.Setting;
using w2.Domain.UserCreditCard;

/// <summary>
/// 注文情報更新データ（前後）リスト作成クラス
/// </summary>
public class BeforeAndAfterUpdateDataListCreatorOrder : BeforeAndAfterUpdateDataListCreatorBase
{
	#region
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BeforeAndAfterUpdateDataListCreatorOrder()
	{
	}
	#endregion

	#region メソッド
	/// <summary>
	/// 更新データ（前後）リスト作成
	/// </summary>
	/// <param name="beforeUpdateHistory">更新前データ</param>
	/// <param name="afterUpdateHistory">更新後データ</param>
	/// <returns>更新データ（前後）リスト</returns>
	public override BeforeAndAfterUpdateData[] CreateBeforeAfterUpdateDataList(
		UpdateHistoryModel beforeUpdateHistory,
		UpdateHistoryModel afterUpdateHistory)
	{
		var result = new List<BeforeAndAfterUpdateData>();

		var before = UpdateDataOrder.Deserialize(beforeUpdateHistory.UpdateData);
		var after = UpdateDataOrder.Deserialize(afterUpdateHistory.UpdateData);
		foreach (var field in UpdateHistorySetting.GetFields(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER)
			.FieldList.ToArray())
		{
			// ポイントOPがOFFの場合追加しない
			if ((Constants.W2MP_POINT_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_ORDER_POINT_USE)
					|| (field.Name == Constants.FIELD_ORDER_ORDER_POINT_USE_YEN)
					|| (field.Name == Constants.FIELD_ORDER_ORDER_POINT_ADD))) continue;

			// クーポンOPがOFFの場合追加しない
			if ((Constants.W2MP_COUPON_OPTION_ENABLED == false) && (field.Name == Constants.FIELD_ORDER_ORDER_COUPON_USE))
				continue;

			// 実在庫OPがOFFの場合追加しない
			if ((Constants.REALSTOCK_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS)
					|| (field.Name == Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE))) continue;

			// モバイルOPがOFFの場合追加しない
			if ((Constants.MOBILEOPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_CAREER_ID) || (field.Name == Constants.FIELD_ORDER_MOBILE_UID))) continue;

			// 広告コードOPがOFFの場合追加しない
			if ((Constants.W2MP_AFFILIATE_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_ADVCODE_FIRST) || (field.Name == Constants.FIELD_ORDER_ADVCODE_NEW)))
				continue;

			// 会員ランクOPがOFFの場合追加しない
			if ((Constants.MEMBER_RANK_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_MEMBER_RANK_ID)
					|| (field.Name == Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE))) continue;

			// セットプロモーションOPがOFFの場合追加しない
			if ((Constants.SETPROMOTION_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT)
					|| (field.Name == Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT)
					|| (field.Name == Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT))) continue;

			// デジタルコンテンツOPがOFFの場合追加しない
			if ((Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false)
				&& (field.Name == Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG)) continue;

			// ギフトOPがOFFの場合追加しない
			if ((Constants.GIFTORDER_OPTION_ENABLED == false) && (field.Name == Constants.FIELD_ORDER_GIFT_FLG)) continue;

			// 定期購入OPがOFFの場合追加しない
			if ((Constants.SETPROMOTION_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_ORDER_FIXED_PURCHASE_ID)
					|| (field.Name == Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT)
					|| (field.Name == Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT))) continue;

			// グローバルOPがOFFの場合追加しない
			if ((Constants.GLOBAL_OPTION_ENABLE == false)
				&& ((field.Name == Constants.FIELD_ORDER_SETTLEMENT_CURRENCY)
					|| (field.Name == Constants.FIELD_ORDER_SETTLEMENT_RATE)
					|| (field.Name == Constants.FIELD_ORDER_SETTLEMENT_AMOUNT))) continue;

			// 店舗受取OPがOFFの場合追加しない
			if (((Constants.REALSHOP_OPTION_ENABLED && Constants.STORE_PICKUP_OPTION_ENABLED) == false)
				&& ((field.Name == Constants.FIELD_ORDER_STOREPICKUP_STATUS)
					|| (field.Name == Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE)
					|| (field.Name == Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE)
					|| (field.Name == Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE))) continue;

			switch (field.Name)
			{
				case "extend_status":
					SetExtendStatus(result, field, before, after);
					break;
				case "order_owner":
					SetOrderOwner(result, field, before, after);
					break;
				case "order_shippings":
					SetOrderShippingList(result, field, before, after);
					break;
				case "order_items":
					SetOrderItemList(result, field, before, after);
					break;
				case "order_coupons":
					if (Constants.W2MP_COUPON_OPTION_ENABLED == false) continue;
					SetOrderCouponList(result, field, before, after);
					break;
				case "order_setpromotions":
					if (Constants.SETPROMOTION_OPTION_ENABLED == false) continue;
					SetOrderSetPromotionList(result, field, before, after);
					break;
				case "credit_card_info":
					SetCreditCardInfo(result, field, before, after);
					break;
				case "tw_order_invoice":
					if (OrderCommon.DisplayTwInvoiceInfo() == false) continue;
					SetOrderInvoice(result, field, before, after);
					break;
				case "order_extend":
					if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) continue;
					SetOrderExtend(result, field, before, after);
					break;
				default:
					var beforeValue = ConvertValue(field, before);
					var afterValue = ConvertValue(field, after);
					result.Add(new BeforeAndAfterUpdateData(field, beforeValue, afterValue));
					break;
			}
		}
		return result.ToArray();
	}

	/// <summary>
	/// 値変換（各クラスで実装）
	/// </summary>
	/// <param name="field">出力項目設定情報</param>
	/// <param name="updateData">更新データ</param>
	/// <returns>値</returns>
	protected override string ConvertToValueForMaster(Field field, UpdateDataBase updateData)
	{
		var value = StringUtility.ToEmpty(updateData[field.Name]);
		switch (field.Name)
		{
			case Constants.FIELD_ORDER_MALL_ID:
				if (value == Constants.FLG_ORDER_MALL_ID_OWN_SITE)
				{
					return ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_SITENAME,
						Constants.VALUETEXT_PARAM_OWNSITENAME,
						Constants.FLG_ORDER_MALL_ID_OWN_SITE);
				}
				var mallCooperationSetting = DataCacheControllerFacade.GetMallCooperationSettingCacheController().GetMallCooperationSetting(value);
				if (mallCooperationSetting != null)
				{
					return mallCooperationSetting.MallName;
				}
				return value;
			case Constants.FIELD_ORDER_MEMBER_RANK_ID:
				return MemberRankOptionUtility.GetMemberRankName(value);
			case Constants.FIELD_ORDER_ORDER_PAYMENT_KBN:
				var payment = new PaymentService().Get(Constants.CONST_DEFAULT_SHOP_ID, value);
				if (payment != null)
				{
					var paymentName = payment.PaymentName;
					if (OrderCommon.CreditInstallmentsSelectable && (string.IsNullOrEmpty(updateData[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE]) == false))
					{
						var paymentValueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_PAYMENT_KBN);
						paymentName +=
							string.Format(
								paymentValueFormat,
								ValueText.GetValueText(
									Constants.TABLE_ORDER,
									OrderCommon.CreditInstallmentsValueTextFieldName,
									updateData[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE]));
					}
					return paymentName;
				}
				return value;
			case Constants.FIELD_ORDER_SHIPPING_ID:
				if (updateData.KeyValues.Length == 0) return "";
				var shipping = new ShopShippingService().Get(Constants.CONST_DEFAULT_SHOP_ID, value);
				if (shipping != null) return "[" + value + "]" + shipping.ShopShippingName;
				var shippingTypeValueFormat = UpdateHistoryDisplayFormatSetting
					.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING_TYPE_UNKNOWN);
				return string.Format(shippingTypeValueFormat,value);
			case Constants.FIELD_ORDER_REPAYMENT_MEMO:
				var repaymentBank = OrderCommon.CreateRepaymentBankDictionary(value);
				if (repaymentBank != null)
				{
					var repaymentMemoFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_REPAYMENT_MEMO);
					return string.Format(
						repaymentMemoFormat,
						repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_CODE],
						repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_NAME],
						repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_BRANCH],
						repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NO],
						repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NAME]
					);
				}
				return value;
			default:
				return value;
		}
	}

	/// <summary>
	/// 注文者情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetOrderOwner(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_OWNER);

		var before = "";
		var after = "";
		var value = "";
		foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
		{
			if (updateData.KeyValues.Length != 0)
			{
				var owner = updateData.Owner;
				value = string.Format(
					valueFormat,
					ValueText.GetValueText(
						Constants.TABLE_ORDEROWNER,
						Constants.FIELD_ORDEROWNER_OWNER_KBN,
						owner[Constants.FIELD_ORDEROWNER_OWNER_KBN]),
					owner[Constants.FIELD_ORDEROWNER_OWNER_ZIP],
					owner[Constants.FIELD_ORDEROWNER_OWNER_ADDR1],
					owner[Constants.FIELD_ORDEROWNER_OWNER_ADDR2],
					owner[Constants.FIELD_ORDEROWNER_OWNER_ADDR3],
					owner[Constants.FIELD_ORDEROWNER_OWNER_ADDR4],
					owner[Constants.FIELD_ORDEROWNER_OWNER_NAME],
					owner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA],
					DateTimeUtility.ToStringForManager(
						owner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
						DateTimeUtility.FormatType.ShortDate2Letter,
						"-"),
					ValueText.GetValueText(
						Constants.TABLE_ORDEROWNER,
						Constants.FIELD_ORDEROWNER_OWNER_SEX,
						owner[Constants.FIELD_ORDEROWNER_OWNER_SEX]),
					owner[Constants.FIELD_ORDEROWNER_OWNER_TEL1],
					(string.IsNullOrEmpty(owner[Constants.FIELD_ORDEROWNER_OWNER_TEL2]) == false)
						? " / " + owner[Constants.FIELD_ORDEROWNER_OWNER_TEL2]
						: "",
					owner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR],
					(string.IsNullOrEmpty(owner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]) == false)
						? " / " + owner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]
						: "");
			}
			if (updateData == beforeUpdateData)
			{
				before = value;
			}
			else
			{
				after = value;
			}
		}
		beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(field, before, after));
	}

	/// <summary>
	/// 注文配送先情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetOrderShippingList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var beforeOrderShippingList = beforeUpdateData.Shippings;
		var afterOrderShippingList = afterUpdateData.Shippings;

		// 枝番リスト作成
		var shippingNos = beforeOrderShippingList.Select(os => os[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]).ToArray();
		shippingNos =
			shippingNos.Union(afterOrderShippingList.Select(os => os[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]).ToArray())
				.ToArray();

		var shippingAddressFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING);

		var receivingStoreFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_RECEIVING_STORE);

		// 宅配通連携の場合、配送実績情報も加える
		if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED)
		{
			shippingAddressFormat += UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING_PELICAN_REPORT);
			receivingStoreFormat += UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING_CVS_PELICAN_REPORT);
		}

		int no = 0;
		foreach (var shippingNo in shippingNos)
		{
			no++;
			var tempField = new Field
			{
				JName = string.Format(field.JName, no),
				Name = field.Name
			};

			var before = "";
			var after = "";
			var value = "";
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				if (updateData.KeyValues.Length != 0)
				{
					var tempOrderShipping =
						updateData.Shippings.Where(os => os[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] == shippingNo);
					if (tempOrderShipping.Any())
					{
						var orderShipping = tempOrderShipping.First();
						var shippingDate = DateTimeUtility.ToStringForManager(
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE],
							DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
							CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@"));

						var shippingTime = "";
						var deliveryCompanyName = "";
						var companyShipping =
							new DeliveryCompanyService().Get(orderShipping[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]);
						if (companyShipping != null)
						{
							shippingTime = companyShipping.GetShippingTimeMessage(orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]);
							deliveryCompanyName = companyShipping.DeliveryCompanyName;
						}
						if (string.IsNullOrEmpty(shippingTime)) shippingTime = CommonPage.ReplaceTag("@@DispText.shipping_time_list.none@@");

						var scheduledShippingDate = DateTimeUtility.ToStringForManager(
							orderShipping[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE],
							DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
							CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@"));

						value = string.Format(
							shippingAddressFormat,
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1],
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD,
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]),
							deliveryCompanyName,
							scheduledShippingDate,
							shippingDate,
							shippingTime,
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE],
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE,
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE]),
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS,
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS]),
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME],
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME],
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS,
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS]),
							orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL]);

						// Display History When Buy In Convenience Store
						var shippingReceivingStoreFlg
							= StringUtility.ToEmpty(orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]);
						if (shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						{
							value = string.Format(
								receivingStoreFormat,
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID],
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS],
								ValueText.GetValueText(
									Constants.TABLE_ORDERSHIPPING,
									Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS,
									orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS]),
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE],
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE],
								ValueText.GetValueText(
									Constants.TABLE_ORDERSHIPPING,
									Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE,
									orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE]),
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME],
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME],
								ValueText.GetValueText(
									Constants.TABLE_ORDERSHIPPING,
									Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS,
									orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS]),
								orderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL]);
						}
					}
				}
				if (updateData == beforeUpdateData)
				{
					before = value;
				}
				else
				{
					after = value;
				}
			}
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(tempField, before, after));
		}
	}

	/// <summary>
	/// Set Order Invoice
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetOrderInvoice(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var beforeOrderInvoiceList = beforeUpdateData.Invoices;
		var afterOrderInvoiceList = afterUpdateData.Invoices;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_INVOICE);

		// 枝番リスト作成
		var shippingNos = beforeOrderInvoiceList.Select(os => os[Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO]).ToArray();
		shippingNos =
			shippingNos.Union(afterOrderInvoiceList.Select(os => os[Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO]).ToArray())
				.ToArray();

		int no = 0;
		foreach (var shippingNo in shippingNos)
		{
			no++;
			var tempField = new Field
			{
				JName = string.Format(field.JName, no),
				Name = field.Name
			};

			var before = "";
			var after = "";
			var value = "";
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				if (updateData.KeyValues.Length != 0)
				{
					var tempOrderInvoice =
						updateData.Invoices.Where(os => os[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] == shippingNo);
					if (tempOrderInvoice.Any())
					{
						var orderInvoice = tempOrderInvoice.First();

						value = string.Format(
							valueFormat,
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE],
							ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE,
								Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS,
								orderInvoice[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS]),
							orderInvoice[Constants.FIELD_TWORDERINVOICE_DATE_CREATED],
							orderInvoice[Constants.FIELD_TWORDERINVOICE_DATE_CHANGED]);
					}
				}
				if (updateData == beforeUpdateData)
				{
					before = value;
				}
				else
				{
					after = value;
				}
			}
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(tempField, before, after));
		}
	}

	/// <summary>
	/// 注文商品情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetOrderItemList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var beforeOrderItemList = (beforeUpdateData.Shippings.Length != 0)
			? beforeUpdateData.Shippings[0].Items
			: new UpdateDataOrderItem[0];
		var afterOrderItemList = (afterUpdateData.Shippings.Length != 0)
			? afterUpdateData.Shippings[0].Items
			: new UpdateDataOrderItem[0];

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM);
		if (Constants.PRODUCT_SALE_OPTION_ENABLED) valueFormat +=
			UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM_SALE);
		if (Constants.NOVELTY_OPTION_ENABLED) valueFormat +=
			UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM_NOVELTY);
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			valueFormat += UpdateHistoryDisplayFormatSetting.GetFormat(
				Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM_SUBSCRIPTION_BOX);
		}

		// 枝番リスト作成
		var itemNos = beforeOrderItemList.Select(oi => oi[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO]).ToArray();
		itemNos = itemNos.Union(afterOrderItemList.Select(os => os[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO]).ToArray()).ToArray();

		int no = 0;
		foreach (var itemNo in itemNos)
		{
			no++;
			var tempField = new Field
			{
				JName = string.Format(field.JName, no),
				Name = field.Name
			};

			var before = "";
			var after = "";
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				var value = "";
				if (updateData.Shippings.Length != 0)
				{
					var tempOrderItem = updateData.Shippings[0].Items.Where(os => os[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO] == itemNo);
					if (tempOrderItem.Any())
					{
						var orderItem = tempOrderItem.First();
						value = string.Format(
							valueFormat,
							orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID],
							orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID],
							orderItem[Constants.FIELD_ORDERITEM_PRODUCT_NAME],
							orderItem[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToPriceString(true),
							StringUtility.ToNumeric(orderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]),
							orderItem[Constants.FIELD_ORDERITEM_ITEM_PRICE].ToPriceString(true),
							string.IsNullOrEmpty(orderItem[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT])
								? "0"
								: orderItem[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT],
							string.IsNullOrEmpty(orderItem[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT])
								? "0"
								: orderItem[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT],
							orderItem[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE].ToPriceString(true),
							orderItem[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS],
							orderItem[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID],
							orderItem[Constants.FIELD_ORDERITEM_NOVELTY_ID],
							orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]);

						if (Constants.ORDER_ITEM_DISCOUNTED_PRICE_ENABLE == false)
						{
							value = Regex.Replace(value, "明細金額（割引後価格）.*?\n", "");
						}

						// 定期購入を利用しない場合、または定期購入グラフがオフの場合、定期購入回数履歴を表示しない
						if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
							|| (orderItem[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG] == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF))
						{
							value = Regex.Replace(value, "定期購入回数（注文基準）: .*?\n", "");
							value = Regex.Replace(value, "定期購入回数（出荷基準）: .*?\n", "");
						}
						value = value.Replace(")　", ")<br />");
					}
				}
				if (updateData == beforeUpdateData)
				{
					before = value;
				}
				else
				{
					after = value;
				}
			}
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(tempField, before, after));
		}
	}

	/// <summary>
	/// 注文クーポン情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetOrderCouponList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var beforeOrderCouponList = beforeUpdateData.Coupons;
		var afterOrderCouponList = afterUpdateData.Coupons;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_COUPON);

		// 枝番リスト作成
		var couponNos = beforeOrderCouponList.Select(oi => oi[Constants.FIELD_ORDERCOUPON_COUPON_NO]).ToArray();
		couponNos =
			couponNos.Union(afterOrderCouponList.Select(os => os[Constants.FIELD_ORDERCOUPON_COUPON_NO]).ToArray()).ToArray();

		int no = 0;
		foreach (var couponNo in couponNos)
		{
			no++;
			var tempField = new Field
			{
				JName = string.Format(field.JName, no),
				Name = field.Name
			};

			var before = "";
			var after = "";
			foreach (var updateData in new UpdateDataOrder[] { beforeUpdateData, afterUpdateData })
			{
				var tempOrderCoupon = updateData.Coupons.Where(os => os[Constants.FIELD_ORDERCOUPON_COUPON_NO] == couponNo);
				if (tempOrderCoupon.Any())
				{
					var orderCoupon = tempOrderCoupon.First();
					var value = string.Format(
						valueFormat,
						orderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_CODE],
						updateData[Constants.FIELD_ORDER_ORDER_COUPON_USE].ToPriceString(true),
						orderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME],
						orderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NAME]);
					if (updateData == beforeUpdateData)
					{
						before = value;
					}
					else
					{
						after = value;
					}
				}
			}
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(tempField, before, after));
		}
	}

	/// <summary>
	/// 注文セットプロモーション情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetOrderSetPromotionList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var beforeOrderSetPromotionList = beforeUpdateData.SetPromotions;
		var afterOrderSetPromotionList = afterUpdateData.SetPromotions;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SET_PROMOTION);

		// 枝番リスト作成
		var setPromotionNos =
			beforeOrderSetPromotionList.Select(oi => oi[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO]).ToArray();
		setPromotionNos =
			setPromotionNos.Union(
				afterOrderSetPromotionList.Select(os => os[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO]).ToArray())
				.ToArray();

		int no = 0;
		foreach (var setPromotionNo in setPromotionNos)
		{
			no++;
			var tempField = new Field
			{
				JName = string.Format(field.JName, no),
				Name = field.Name
			};

			var before = "";
			var after = "";
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				var tempsetPromotion =
					updateData.SetPromotions.Where(os => os[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO] == setPromotionNo);
				if (tempsetPromotion.Any())
				{
					var setPromotion = tempsetPromotion.First();
					var promotionType = "";
					var discountPrice = "0";
					if (setPromotion[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG]
						== Constants.FLG_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG_ON)
					{
						promotionType = UpdateHistoryDisplayFormatSetting
							.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_SET_PROMOTION_DISCOUNT_TYPE_PRODUCT);
						discountPrice = setPromotion[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT];
					}
					else if (setPromotion[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG]
						== Constants.FLG_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON)
					{
						promotionType = UpdateHistoryDisplayFormatSetting
							.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_SET_PROMOTION_DISCOUNT_TYPE_SHIPPING_FREE);
						discountPrice = setPromotion[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT];
					}
					else if (setPromotion[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG]
						== Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON)
					{
						promotionType = UpdateHistoryDisplayFormatSetting
							.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_SET_PROMOTION_DISCOUNT_TYPE_PAYMENT_FREE);
						discountPrice = setPromotion[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT];
					}

					var value = string.Format(
						valueFormat,
						setPromotion[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME],
						setPromotion[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID],
						promotionType,
						discountPrice.ToPriceString(true));
					if (updateData == beforeUpdateData)
					{
						before = value;
					}
					else
					{
						after = value;
					}
				}
			}
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(tempField, before, after));
		}
	}

	/// <summary>
	/// クレジットカード情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetCreditCardInfo(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataOrder beforeUpdateData,
		UpdateDataOrder afterUpdateData)
	{
		var before = "";
		var after = "";
		var isSet = false;
		foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
		{
			if ((updateData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (string.IsNullOrEmpty(updateData[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]) == false))
			{
				var userCreditCard = updateData.UserCreditCard;
				var value = UserCreditCardHelper.CreateCreditCardInfo(new UserCreditCardModel(userCreditCard.ToHashtable()));
				if (updateData == beforeUpdateData)
				{
					before = value;
				}
				else
				{
					after = value;
				}
				isSet = true;
			}
		}
		if (isSet)
		{
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(field, before, after));
		}
	}

	#endregion
}
