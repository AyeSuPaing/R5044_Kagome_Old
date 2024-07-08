/*
=========================================================================================================
  Module      : 定期情報情報更新データ（前後）リスト作成クラス (BeforeAndAfterUpdateDataListCreatorFixedPurchase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.Web.Page;
using w2.Domain.DeliveryCompany;
using w2.Domain.Payment;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper.UpdateData;
using w2.Domain.UpdateHistory.Setting;

/// <summary>
/// 定期情報情報更新データ（前後）リスト作成クラス
/// </summary>
public class BeforeAndAfterUpdateDataListCreatorFixedPurchase : BeforeAndAfterUpdateDataListCreatorBase
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BeforeAndAfterUpdateDataListCreatorFixedPurchase()
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

		var before = UpdateDataFixedPurchase.Deserialize(beforeUpdateHistory.UpdateData);
		var after = UpdateDataFixedPurchase.Deserialize(afterUpdateHistory.UpdateData);
		foreach (var field in
			UpdateHistorySetting.GetFields(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE).FieldList.ToArray())
		{
			switch (field.Name)
			{
				case "extend_status":
					SetExtendStatus(result, field, before, after);
					break;

				case "fixed_purchase_shippings":
					SetFixedPurchaseShippingList(result, field, before, after);
					break;

				case "fixed_purchase_items":
					SetFixedPurchaseItemList(result, field, before, after);
					break;

				case "credit_card_info":
					SetCreditCardInfo(result, field, before, after);
					break;

				case "order_extend":
					if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) continue;
					SetOrderExtend(result, field, before, after);
					break;

				case "next_shipping_use_point":
					if (Constants.W2MP_POINT_OPTION_ENABLED
						&& Constants.FIXEDPURCHASE_OPTION_ENABLED
						&& Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
					{
						SetUsePoint(result, field, before, after);
						break;
					}
					result.Add(new BeforeAndAfterUpdateData(field, ConvertValue(field, before), ConvertValue(field, after)));
					break;

				default:
					result.Add(new BeforeAndAfterUpdateData(field, ConvertValue(field, before), ConvertValue(field, after)));
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
			case Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN:
				var payment = new PaymentService().Get(Constants.CONST_DEFAULT_SHOP_ID, value);
				if (payment != null)
				{
					var paymentName = payment.PaymentName;
					if (OrderCommon.CreditInstallmentsSelectable
						&& (string.IsNullOrEmpty(updateData[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE]) == false))
					{
						var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_PAYMENT_KBN);
						paymentName += string.Format(
							valueFormat,
							ValueText.GetValueText(
								Constants.TABLE_ORDER,
								OrderCommon.CreditInstallmentsValueTextFieldName,
								updateData[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE]));
					}
					return paymentName;
				}
				return value;

			case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1:
				return OrderCommon.CreateFixedPurchaseSettingMessage(
					updateData[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN],
					updateData[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]);

			case Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID:
				var reason = DataCacheControllerFacade.GetFixedPurchaseCancelReasonCacheController().GetCancelReason(value);
				if (reason != null) return reason.CancelReasonName;
				return value;

			default:
				return value;
		}
	}

	/// <summary>
	/// 定期購入配送先情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetFixedPurchaseShippingList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataFixedPurchase beforeUpdateData,
		UpdateDataFixedPurchase afterUpdateData)
	{
		var beforeFixedPurchaseShippings = beforeUpdateData.Shippings;
		var afterFixedPurchaseShippings = afterUpdateData.Shippings;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_FIXED_PURCHASE_SHIPPING);

		// 枝番リスト作成
		var shippingNos =
			beforeFixedPurchaseShippings.Select(fs => fs[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO])
				.ToArray();
		shippingNos =
			shippingNos.Union(
				afterFixedPurchaseShippings.Select(fs => fs[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO])
					.ToArray()).ToArray();

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
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				var tempFixedPurchaseShipping =
					updateData.Shippings.Where(
						os => (string)os[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO] == shippingNo);
				if (tempFixedPurchaseShipping.Any())
				{
					var fixedPurchaseShipping = tempFixedPurchaseShipping.First();

					var shippingTime = "";
					var deliveryCompanyName = "";
					if (fixedPurchaseShipping.Items.Length != 0)
					{
						var companyShipping = new DeliveryCompanyService().Get(fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_DELIVERY_COMPANY_ID]);
						if (companyShipping != null)
						{
							shippingTime = companyShipping.GetShippingTimeMessage(fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TIME]);
							deliveryCompanyName = companyShipping.DeliveryCompanyName;
						}
					}
					if (string.IsNullOrEmpty(shippingTime)) shippingTime = CommonPage.ReplaceTag("@@DispText.shipping_time_list.none@@");

					var value = string.Format(
						valueFormat,
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR1],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR2],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR3],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR4],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA],
						fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1],
						ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, fixedPurchaseShipping[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_METHOD]),
						deliveryCompanyName,
						shippingTime);
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
	/// 定期購入商品情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetFixedPurchaseItemList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataFixedPurchase beforeUpdateData,
		UpdateDataFixedPurchase afterUpdateData)
	{
		var beforeFixedPurchaseItemList = (beforeUpdateData.Shippings.Length != 0)
			? beforeUpdateData.Shippings[0].Items
			: new UpdateDataFixedPurchaseItem[0];
		var afterFixedPurchaseItemList = afterUpdateData.Shippings[0].Items;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_FIXED_PURCHASE_ITEM);

		// 枝番リスト作成
		var itemNos =
			beforeFixedPurchaseItemList.Select(fi => fi[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO]).ToArray();
		itemNos =
			itemNos.Union(
				afterFixedPurchaseItemList.Select(fi => fi[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO]).ToArray())
				.ToArray();

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
			var value = "";
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				if (updateData.Shippings.Length != 0)
				{
					var tempFixedPurchaseItem =
						updateData.Shippings[0].Items.Where(
							os => (string)os[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO] == itemNo);

					if (tempFixedPurchaseItem.Any())
					{
						var fixedPurchaseItem = tempFixedPurchaseItem.First();

						var tempFixedPurchaseInput = new FixedPurchaseItemInput();
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]) == false)
							tempFixedPurchaseInput.FixedPurchasePrice = decimal.Parse(fixedPurchaseItem[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]);
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE]) == false)
							tempFixedPurchaseInput.MemberRankPrice = decimal.Parse(fixedPurchaseItem[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE]);
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]) == false)
							tempFixedPurchaseInput.SpecialPrice = decimal.Parse(fixedPurchaseItem[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]);
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_PRODUCTVARIATION_PRICE]) == false)
							tempFixedPurchaseInput.Price = decimal.Parse(fixedPurchaseItem[Constants.FIELD_PRODUCTVARIATION_PRICE]);
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_SHOP_ID]) == false)
							tempFixedPurchaseInput.ShopId = (string)fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_SHOP_ID];
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID]) == false)
							tempFixedPurchaseInput.ProductId = (string)fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID];
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY]) == false)
							tempFixedPurchaseInput.ItemQuantity = (string)fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY];
						if (string.IsNullOrEmpty(fixedPurchaseItem[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]) == false)
							tempFixedPurchaseInput.ProductOptionTexts = (string)fixedPurchaseItem[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS];

						value = string.Format(
							valueFormat,
							(string)fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID],
							(string)fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_VARIATION_ID],
							(string)fixedPurchaseItem["product_name"],
							(tempFixedPurchaseInput.GetValidPrice() + tempFixedPurchaseInput.GetProductOptionPrice()).ToPriceString(true),
							StringUtility.ToNumeric(fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY]),
							tempFixedPurchaseInput.GetItemPrice().ToPriceString(true),
							StringUtility.ToEmpty(fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT]),
							StringUtility.ToEmpty(fixedPurchaseItem[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT]),
							tempFixedPurchaseInput.GetDisplayProductOptionTexts());
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
				value = "";
			}
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(tempField, before, after));
		}
	}
	#endregion
}