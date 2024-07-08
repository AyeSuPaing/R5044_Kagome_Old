/*
=========================================================================================================
  Module      : ユーザー情報更新データ（前後）リスト作成クラス (BeforeAndAfterUpdateDataListCreatorUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper.UpdateData;
using w2.Domain.UpdateHistory.Setting;
using w2.Domain.UserCreditCard;

/// <summary>
/// ユーザー情報更新データ（前後）リスト作成クラス
/// </summary>
public class BeforeAndAfterUpdateDataListCreatorUser : BeforeAndAfterUpdateDataListCreatorBase
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BeforeAndAfterUpdateDataListCreatorUser()
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

		var before = UpdateDataUser.Deserialize(beforeUpdateHistory.UpdateData);
		var after = UpdateDataUser.Deserialize(afterUpdateHistory.UpdateData);
		foreach (
			var field in UpdateHistorySetting.GetFields(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER).FieldList.ToArray())
		{
			// ポイントOPがOFFの場合追加しない
			if ((Constants.W2MP_POINT_OPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_USERPOINT_POINT) || (field.Name == "point_and_point_temp")
					|| (field.Name == Constants.FIELD_USERPOINT_POINT_EXP))) continue;

			// クーポンOPがOFFの場合追加しない
			if ((Constants.W2MP_COUPON_OPTION_ENABLED == false) && (field.Name == "user_coupons")) continue;

			// 法人項目がOFFの場合追加しない
			if ((Constants.DISPLAY_CORPORATION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_USER_COMPANY_NAME) || (field.Name == Constants.FIELD_USER_COMPANY_POST_NAME)))
				continue;

			// モバイルOPがOFFの場合追加しない
			if ((Constants.MOBILEOPTION_ENABLED == false)
				&& ((field.Name == Constants.FIELD_USER_CAREER_ID) || (field.Name == Constants.FIELD_USER_MOBILE_UID))) continue;

			// 広告コードOPがOFFの場合追加しない
			if ((Constants.W2MP_AFFILIATE_OPTION_ENABLED == false) && (field.Name == Constants.FIELD_USER_ADVCODE_FIRST))
				continue;

			// 会員ランクOPがOFFの場合追加しない
			if ((Constants.MEMBER_RANK_OPTION_ENABLED == false) && (field.Name == Constants.FIELD_USER_MEMBER_RANK_ID)) continue;

			// ユーザー統合OPがOFFの場合追加しない
			if ((Constants.USERINTEGRATION_OPTION_ENABLED == false) && (field.Name == Constants.FIELD_USER_INTEGRATED_FLG))
				continue;

			var beforeValue = "";
			var afterValue = "";
			switch (field.Name)
			{
				case "point_and_point_temp":
					SetUserPoint(result, field, before, after);
					break;
				case "user_coupons":
					SetUserCouponList(result, field, before, after);
					break;
				case Constants.FIELD_USERPOINT_POINT_EXP:
					beforeValue = ConvertValue(field, before.UserPoint);
					afterValue = ConvertValue(field, after.UserPoint);
					result.Add(new BeforeAndAfterUpdateData(field, beforeValue, afterValue));
					break;
				case "user_extend":
					SetUserExtendList(result, field, before, after);
					break;
				case "user_credit_cards":
					SetUserCreditcardList(result, field, before, after);
					break;
				case "user_shippings":
					SetUserShippingList(result, field, before, after);
					break;

				case "tw_user_invoices":
					SetUserInvoiceList(result, field, before, after);
					break;

				default:
					beforeValue = ConvertValue(field, before);
					afterValue = ConvertValue(field, after);
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
			case Constants.FIELD_USER_MALL_ID:
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
			case Constants.FIELD_USER_MEMBER_RANK_ID:
				return MemberRankOptionUtility.GetMemberRankName(value);
			case Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID:
				return UserManagementLevelUtility.GetUserManagementLevelName(value);
			default:
				return value;
		}
	}

	/// <summary>
	/// ユーザーポイントセット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetUserPoint(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataUser beforeUpdateData,
		UpdateDataUser afterUpdateData)
	{
		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_USER_POINT);

		var before = "";
		var after = "";
		foreach (var updateData in new [] { beforeUpdateData, afterUpdateData })
		{
			var userPoint = updateData.UserPoint;
			decimal point = 0;
			decimal pointTemp = 0;
			var value = (updateData.KeyValues.Length != 0)
				? string.Format(
					valueFormat,
					decimal.TryParse(userPoint[Constants.FIELD_USERPOINT_POINT], out point) ? point : 0,
					decimal.TryParse(userPoint[Constants.FIELD_USERPOINT_POINT + "_temp"], out pointTemp) ? pointTemp : 0)
				: "";
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
	/// ユーザークーポンリストセット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetUserCouponList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataUser beforeUpdateData,
		UpdateDataUser afterUpdateData)
	{
		var beforeUserCouponList = beforeUpdateData.UserCoupons ?? new UpdateDataUserCoupon[0];
		var afterUserCouponList = afterUpdateData.UserCoupons ?? new UpdateDataUserCoupon[0];

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_USER_COUPON);

		// 枝番リスト作成
		var couponNos =
			beforeUserCouponList.Select(
				c => new Tuple<string, string>(c[Constants.FIELD_USERCOUPON_COUPON_ID], c[Constants.FIELD_USERCOUPON_COUPON_NO]))
				.ToArray();
		couponNos = couponNos.Union(
			afterUserCouponList.Select(
				c => new Tuple<string, string>(c[Constants.FIELD_USERCOUPON_COUPON_ID], c[Constants.FIELD_USERCOUPON_COUPON_NO])))
			.ToArray();

		foreach (var couponNo in couponNos)
		{
			var tempField = new Field
			{
				JName = string.Format(field.JName, couponNo.Item1 + "-" + couponNo.Item2),
				Name = field.Name
			};
			var before = "";
			var after = "";
			foreach (var updateData in new[] { beforeUpdateData, afterUpdateData })
			{
				if (updateData.UserCoupons == null) continue;
				var temp =
					updateData.UserCoupons.Where(
						uc =>
							(uc[Constants.FIELD_USERCOUPON_COUPON_ID] == couponNo.Item1)
								&& (uc[Constants.FIELD_USERCOUPON_COUPON_NO] == couponNo.Item2));
				if (temp.Any() == false) continue;
				var userCoupon = temp.First();
				var value = string.Format(
					valueFormat,
					userCoupon[Constants.FIELD_COUPON_COUPON_DISP_NAME],
					userCoupon[Constants.FIELD_COUPON_COUPON_CODE],
					ValueText.GetValueText(Constants.TABLE_USERCOUPON, Constants.FIELD_USERCOUPON_USE_FLG, userCoupon[Constants.FIELD_USERCOUPON_USE_FLG]));
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
	/// ユーザー拡張項目リストセット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetUserExtendList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataUser beforeUpdateData,
		UpdateDataUser afterUpdateData)
	{
		// ユーザー拡張項目設定取得
		var userExtendSettings = DataCacheControllerFacade.GetUserExtendSettingCacheController().CacheData;

		// ユーザー拡張項目IDを結合
		var beforeUserExtend = beforeUpdateData.UserExtend;
		var afterUserExtend = afterUpdateData.UserExtend;
		var settingIds = beforeUserExtend.KeyValues.Select(ue => ue.Key).ToArray();
		settingIds = settingIds.Union(afterUserExtend.KeyValues.Select(ue => ue.Key)).ToArray();
		foreach (
			var userExtendSetting in
				userExtendSettings.Items.Where(ues => settingIds.Contains(ues.SettingId)).OrderBy(ues => ues.SortOrder))
		{
			// ユーザー拡張項目名取得
			var tempField = new Field
			{
				JName = string.Format(field.JName, userExtendSetting.SettingName),
				Name = field.Name
			};

			// ユーザー拡張項目値取得
			var before = "";
			var after = "";
			foreach (var updateData in new UpdateDataUser[] { beforeUpdateData, afterUpdateData })
			{
				var value = "";
				var userExtend = updateData.UserExtend.KeyValues.Where(ue => ue.Key == userExtendSetting.SettingId);
				if (userExtend.Any())
				{
					value = userExtend.First().Value;
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
	/// ユーザークレジットカードリストセット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetUserCreditcardList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataUser beforeUpdateData,
		UpdateDataUser afterUpdateData)
	{
		var beforeUserCreditcardList = beforeUpdateData.UserCreditCards;
		var afterUserCreditcardList = afterUpdateData.UserCreditCards;

		// 枝番リスト作成
		var branchNos = beforeUserCreditcardList.Select(uc => uc[Constants.FIELD_USERCREDITCARD_BRANCH_NO]).ToArray();
		branchNos =
			branchNos.Union(afterUserCreditcardList.Select(uc => uc[Constants.FIELD_USERCREDITCARD_BRANCH_NO]).ToArray())
				.ToArray();

		int no = 0;
		foreach (var branchNo in branchNos)
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
				var tempUserCreditCard =
					updateData.UserCreditCards.Where(uc => uc[Constants.FIELD_USERCREDITCARD_BRANCH_NO] == branchNo);
				if (tempUserCreditCard.Any())
				{
					var userCreditCard = tempUserCreditCard.First();
					var value = UserCreditCardHelper.CreateCreditCardInfo(new UserCreditCardModel(userCreditCard.ToHashtable()));
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
	/// ユーザー配送先リストセット
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">更新データ（前後）リスト</param>
	/// <param name="field">更新履歴出力設定情報</param>
	/// <param name="beforeUpdateData">更新前データ</param>
	/// <param name="afterUpdateData">更新後データ</param>
	private void SetUserShippingList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataUser beforeUpdateData,
		UpdateDataUser afterUpdateData)
	{
		var beforeUserShippingList = beforeUpdateData.UserShippings;
		var afterUserShippingList = afterUpdateData.UserShippings;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_USER_SHIPPING_LIST);

		// 枝番リスト作成
		var shippingNos = beforeUserShippingList.Select(us => us[Constants.FIELD_USERSHIPPING_SHIPPING_NO]).ToArray();
		shippingNos =
			shippingNos.Union(afterUserShippingList.Select(us => us[Constants.FIELD_USERSHIPPING_SHIPPING_NO]).ToArray())
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
			foreach (var updateData in new UpdateDataUser[] { beforeUpdateData, afterUpdateData })
			{
				var tempUserShipping =
					updateData.UserShippings.Where(us => us[Constants.FIELD_USERSHIPPING_SHIPPING_NO] == shippingNo);
				if (tempUserShipping.Any())
				{
					var userShipping = tempUserShipping.First();
					var value = string.Format(
						valueFormat,
						userShipping[Constants.FIELD_USERSHIPPING_NAME],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_NAME],
						userShipping[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA]);
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
	/// Set User Invoice List
	/// </summary>
	/// <param name="beforeAfterUpdateDataList">Before After Update Data List</param>
	/// <param name="field">Field</param>
	/// <param name="beforeUpdateData">Before Update Data</param>
	/// <param name="afterUpdateData">After Update Data</param>
	private void SetUserInvoiceList(
		List<BeforeAndAfterUpdateData> beforeAfterUpdateDataList,
		Field field,
		UpdateDataUser beforeUpdateData,
		UpdateDataUser afterUpdateData)
	{
		var beforeUserShippingList = beforeUpdateData.UserInvoices;
		var afterUserShippingList = afterUpdateData.UserInvoices;

		var valueFormat = UpdateHistoryDisplayFormatSetting.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_USER_INVOICE_LIST);

		var invoiceNos = beforeUserShippingList.Select(us => us[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO]).ToArray();
		invoiceNos =
			invoiceNos.Union(afterUserShippingList.Select(us => us[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO]).ToArray())
				.ToArray();

		var no = 0;
		foreach (var invoiceNo in invoiceNos)
		{
			no++;
			var tempField = new Field
			{
				JName = string.Format(field.JName, no),
				Name = field.Name
			};

			var before = string.Empty;
			var after = string.Empty;
			foreach (var updateData in new UpdateDataUser[] { beforeUpdateData, afterUpdateData })
			{
				var tempUserInvoice =
					updateData.UserInvoices.Where(invoice => invoice[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO] == invoiceNo);
				if (tempUserInvoice.Any())
				{
					var userInvoice = tempUserInvoice.First();
					var value = string.Format(
						valueFormat,
						userInvoice[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME],
						userInvoice[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE],
						userInvoice[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1],
						userInvoice[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2],
						userInvoice[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE],
						userInvoice[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION],
						userInvoice[Constants.FIELD_TWUSERINVOICE_DATE_CREATED],
						userInvoice[Constants.FIELD_TWUSERINVOICE_DATE_CHANGED]);
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
	#endregion
}
