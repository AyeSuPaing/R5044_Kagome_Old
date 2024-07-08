/*
=========================================================================================================
  Module      : 更新データ（前後）リスト作成基底クラス (BeforeAndAfterUpdateDataListCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper.UpdateData;
using w2.Domain.UpdateHistory.Setting;
using w2.Domain.User.Helper;
using w2.App.Common.Order;
using w2.App.Common.OrderExtend;
using w2.App.Common.Web.Page;
using w2.Common.Util;
using w2.Domain.UserCreditCard;

/// <summary>
/// 更新データ（前後）リスト作成基底クラス
/// </summary>
public abstract class BeforeAndAfterUpdateDataListCreatorBase
{
	/// <summary>
	/// 更新データ（前後）リスト作成
	/// </summary>
	/// <param name="beforeUpdateHistory">更新前データ</param>
	/// <param name="afterUpdateHistory">更新後データ</param>
	/// <returns>更新データ（前後）リスト</returns>
	public abstract BeforeAndAfterUpdateData[] CreateBeforeAfterUpdateDataList(
		UpdateHistoryModel beforeUpdateHistory,
		UpdateHistoryModel afterUpdateHistory);

	/// <summary>
	/// 拡張ステータスセット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	protected void SetExtendStatus(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataBase beforeUpdateData,
		UpdateDataBase afterUpdateData)
	{
		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_EXTEND_STATUS);

		var orderExtendStatusSettings = OrderPage.GetOrderExtendStatusSettingList(Constants.CONST_DEFAULT_SHOP_ID);
		foreach (DataRowView setting in orderExtendStatusSettings)
		{
			var no = (int)setting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO];
			var before = (beforeUpdateData.KeyValues.Length != 0)
				? string.Format(
					valueFormat,
					no,
					ValueText.GetValueText(Constants.TABLE_ORDER, "extend_status", beforeUpdateData["extend_status" + no]),
					DateTimeUtility.ToStringForManager(
						beforeUpdateData["extend_status_date" + no],
						DateTimeUtility.FormatType.ShortDate2Letter,
						"-"))
				: "";
			var after = (afterUpdateData.KeyValues.Length != 0)
				? string.Format(
					valueFormat,
					no,
					ValueText.GetValueText(Constants.TABLE_ORDER, "extend_status", afterUpdateData["extend_status" + no]),
					DateTimeUtility.ToStringForManager(
						afterUpdateData["extend_status_date" + no],
						DateTimeUtility.FormatType.ShortDate2Letter,
						"-"))
				: "";
			var fieldNew = (Field)field.Clone();
			fieldNew.Name += no.ToString();
			fieldNew.JName += no.ToString() + "\r\n" + (string)setting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME];
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(fieldNew, before, after));
		}
	}

	/// <summary>
	/// クレジットカード情報セット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	protected void SetCreditCardInfo(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataBase beforeUpdateData,
		UpdateDataBase afterUpdateData)
	{
		var before = "";
		var after = "";
		var isSet = false;
		foreach (var updateData in new [] { beforeUpdateData, afterUpdateData })
		{
			if ((updateData[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (string.IsNullOrEmpty(updateData[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO]) == false))
			{
				var userCreditCard = (updateData is UpdateDataOrder)
					? ((UpdateDataOrder)updateData).UserCreditCard
					: (updateData is UpdateDataFixedPurchase) ? ((UpdateDataFixedPurchase)updateData).UserCreditCard : null;
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

	/// <summary>
	/// 値変換
	/// </summary>
	/// <param name="field">出力項目設定情報</param>
	/// <param name="updateData">更新データ</param>
	/// <returns>値</returns>
	protected string ConvertValue(Field field, UpdateDataBase updateData)
	{
		if (updateData.KeyValues.Length == 0) return "";
		var value = StringUtility.ToEmpty(updateData[field.Name]);
		decimal decimalValueTmp;
		switch (field.Convert)
		{
			case "valuetext":
				return ValueText.GetValueText(field.ValueText[0], field.ValueText[1], value);

			case "password":
				if (string.IsNullOrEmpty(value)) return "";
				return UserPassowordCryptor.PasswordDecrypt(value);

			case "format":
				switch (field.Type)
				{
					case "datetime":
						DateTime dateValue;
						return DateTime.TryParse(value, out dateValue) ? dateValue.ToString(field.Format) : "";

					case "decimal":
						return decimal.TryParse(value, out decimalValueTmp) ? string.Format(field.Format, decimalValueTmp) : "";

					case "int":
						int intValue;
						return int.TryParse(value, out intValue) ? string.Format(field.Format, intValue) : "";

					default:
						return value;
				}

			case "system":
				return ConvertToValueForMaster(field, updateData);

			case "price":
				return value.ToPriceString(true);

			case "settlement_amount":
				if (decimal.TryParse(value, out decimalValueTmp) == false) return "";
				return CurrencyManager.ToSettlementCurrencyNotation(decimalValueTmp, updateData["settlement_currency"]);

			case "settlement_rate":
				if (decimal.TryParse(value, out decimalValueTmp) == false) return "";
				return DecimalUtility.DecimalRound(decimalValueTmp, DecimalUtility.Format.RoundDown, 6).ToString();

			default:
				return value;
		}
	}

	/// <summary>
	/// 注文拡張項目
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	protected void SetOrderExtend(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataBase beforeUpdateData,
		UpdateDataBase afterUpdateData)
	{
		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_EXTEND);

		foreach (var orderExtendField in Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST)
		{
			var settingModel = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
				.SettingModels.FirstOrDefault(m => m.SettingId == orderExtendField);

			if (settingModel == null) continue;

			var before = "";
			var after = "";
			var value = "";
			var isSet = false;
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				var temp = updateData.KeyValues.FirstOrDefault(kv => kv.Key == orderExtendField);
				if (temp == null) continue;

				if (updateData.KeyValues.Length != 0)
				{
					value = string.Format(
						valueFormat,
						OrderExtendCommon.GetValueDisplayName(settingModel.InputType, settingModel.InputDefault, temp.Value));
				}
				if (updateData == beforeUpdateData)
				{
					before = value;
				}
				else
				{
					isSet = true;
					after = value;
				}
			}

			if (isSet)
			{
				var temp = field.JName;
				field.JName = string.Format(field.JName, settingModel.SettingName, settingModel.SettingId);
				beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(field, before, after));
				field.JName = temp;
			}
		}
	}

	/// <summary>
	/// 全ポイント継続利用フラグ
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	protected void SetUsePoint(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataBase beforeUpdateData,
		UpdateDataBase afterUpdateData)
	{
		var before = "";
		var after = "";
		var afterValue = "";
		var beforeValue = "";
		var isSet = false;
		var beforeUsePoint = "";
		var afterUsePoint = "";
		var beforeUseAllPointFlg = "";
		var afterUseAllPointFlg = "";
		foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
		{
			var tempFlg = updateData.KeyValues.FirstOrDefault(kv => kv.Key == Constants.FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG);
			var tempPoint = updateData.KeyValues.FirstOrDefault(kv => kv.Key == Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT);
			if (tempPoint == null || tempFlg == null) continue;

			if (updateData.KeyValues.Length != 0)
			{
				// 更新前データが利用ポイントか
				if ((tempPoint.Key == Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT)
					&& (updateData == beforeUpdateData))
				{
					beforeUsePoint = tempPoint.Value;
				}
				// 更新後データが利用ポイントか
				else if ((tempPoint.Key == Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT)
					&& (updateData == afterUpdateData))
				{
					afterUsePoint = tempPoint.Value;
				}

				// 更新前データが全ポイント継続利用か
				if ((tempFlg.Key == Constants.FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG)
					&& (updateData == beforeUpdateData))
				{
					beforeUseAllPointFlg = tempFlg.Value;
				}
				// 更新後データが全ポイント継続利用か
				else if ((tempFlg.Key == Constants.FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG)
					&& (updateData == afterUpdateData))
				{
					afterUseAllPointFlg = tempFlg.Value;
				}

				// 更新前データの値、格納
				if (beforeUseAllPointFlg.Contains(Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON))
				{
					beforeValue = CommonPage.ReplaceTag("@@DispText.fixed_purchase.UseAllPointFlg@@");
				}
				else if (beforeUseAllPointFlg.Contains(Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF))
				{
					beforeValue = beforeUsePoint;
				}

				// 更新後データの値、格納
				if (afterUseAllPointFlg.Contains(Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON))
				{
					afterValue = CommonPage.ReplaceTag("@@DispText.fixed_purchase.UseAllPointFlg@@");
				}
				else if (afterUseAllPointFlg.Contains(Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF))
				{
					afterValue = afterUsePoint;
				}
			}

			if (updateData == beforeUpdateData)
			{
				before = beforeValue;
			}
			else
			{
				isSet = true;
				after = afterValue;
			}
		}
		if (isSet)
		{
			beforeAfterUpdateDataList.Add(new BeforeAndAfterUpdateData(field, before, after));
		}
	}

	/// <summary>
	/// 値変換（各クラスで実装）
	/// </summary>
	/// <param name="field">出力項目設定情報</param>
	/// <param name="updateData">更新データ</param>
	/// <returns>値</returns>
	protected abstract string ConvertToValueForMaster(Field field, UpdateDataBase updateData);
}