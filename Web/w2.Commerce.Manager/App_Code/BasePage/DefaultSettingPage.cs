/*
=========================================================================================================
  Module      : Default Setting Page (DefaultSettingPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.DefaultSetting;

/// <summary>
/// Default setting page
/// </summary>
public class DefaultSettingPage : BasePage
{
	#region +Method
	/// <summary>
	/// Get default setting value
	/// </summary>
	/// <param name="defaultSetting">Default setting</param>
	/// <param name="classification">Classification</param>
	/// <returns>Default setting values</returns>
	public static Hashtable GetDefaultSettingValue(DefaultSetting defaultSetting, string classification)
	{
		var defautSettingInfo = new Hashtable();
		if (defaultSetting.DefaultSettingTables.ContainsKey(classification) == false) return defautSettingInfo;

		switch (classification)
		{
			case Constants.TABLE_USER:
				return GetUserDefaultSettingValue(defaultSetting.DefaultSettingTables[Constants.TABLE_USER]);

			case Constants.TABLE_USER_BUSINESS_OWNER:
				return GetUserDefaultSettingValueGmo(defaultSetting.DefaultSettingTables[Constants.TABLE_USER_BUSINESS_OWNER]);

			default:
				return defautSettingInfo;
		}
	}

	/// <summary>
	/// ユーザーのGMOデフォルト設定値の取得
	/// </summary>
	/// <param name="userDefaultSetting">ユーザーのデフォルト設定値</param>
	/// <returns>ユーザーのデフォルト設定情報</returns>
	private static Hashtable GetUserDefaultSettingValueGmo(DefaultSettingTable userDefaultSetting)
	{
		var parameters = new Hashtable
		{
			{ Constants.FIELD_USER_BUSINESS_OWNER_NAME1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BUSINESS_OWNER_NAME1) },
			{ Constants.FIELD_USER_BUSINESS_OWNER_NAME2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BUSINESS_OWNER_NAME2) },
			{ Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1) },
			{ Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2) },
			{ Constants.FIELD_USER_BUSINESS_OWNER_BIRTH, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BUSINESS_OWNER_BIRTH) },
			{ Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET) }
		};
		return parameters;
	}

	/// <summary>
	/// Get user default setting value
	/// </summary>
	/// <param name="userDefaultSetting">User default setting</param>
	/// <returns>User default setting informattion</returns>
	private static Hashtable GetUserDefaultSettingValue(DefaultSettingTable userDefaultSetting)
	{
		var parameters = new Hashtable
		{
			{ Constants.FIELD_USER_USER_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_USER_ID) },
			{ Constants.FIELD_USER_MALL_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_MALL_ID) },
			{ Constants.FIELD_USER_USER_KBN, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_USER_KBN) },
			{ Constants.FIELD_USER_NAME1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_NAME1) },
			{ Constants.FIELD_USER_NAME2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_NAME2) },
			{ Constants.FIELD_USER_NAME_KANA1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_NAME_KANA1) },
			{ Constants.FIELD_USER_NAME_KANA2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_NAME_KANA2) },
			{ Constants.FIELD_USER_NICK_NAME, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_NICK_NAME) },
			{ Constants.FIELD_USER_SEX, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_SEX) },
			{ Constants.FIELD_USER_BIRTH, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_BIRTH) },
			{ Constants.FIELD_USER_MAIL_ADDR, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_MAIL_ADDR) },
			{ Constants.FIELD_USER_MAIL_ADDR2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_MAIL_ADDR2) },
			{ Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE) },
			{ Constants.FIELD_USER_ZIP, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ZIP) },
			{ Constants.FIELD_USER_ZIP1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ZIP1) },
			{ Constants.FIELD_USER_ZIP2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ZIP2) },
			{ Constants.FIELD_USER_ADDR1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADDR1) },
			{ Constants.FIELD_USER_ADDR2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADDR2) },
			{ Constants.FIELD_USER_ADDR3, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADDR3) },
			{ Constants.FIELD_USER_ADDR4, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADDR4) },
			{ Constants.FIELD_USER_ADDR5, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADDR5) },
			{ Constants.FIELD_USER_TEL1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL1) },
			{ Constants.FIELD_USER_TEL1_1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL1_1) },
			{ Constants.FIELD_USER_TEL1_2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL1_2) },
			{ Constants.FIELD_USER_TEL1_3, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL1_3) },
			{ Constants.FIELD_USER_TEL2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL2) },
			{ Constants.FIELD_USER_TEL2_1, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL2_1) },
			{ Constants.FIELD_USER_TEL2_2, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL2_2) },
			{ Constants.FIELD_USER_TEL2_3, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_TEL2_3) },
			{ Constants.FIELD_USER_MAIL_FLG, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_MAIL_FLG) },
			{ Constants.FIELD_USER_EASY_REGISTER_FLG, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_EASY_REGISTER_FLG) },
			{ Constants.FIELD_USER_LOGIN_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_LOGIN_ID) },
			{ Constants.FIELD_USER_PASSWORD, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_PASSWORD) },
			{ Constants.FIELD_USER_ADVCODE_FIRST, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ADVCODE_FIRST) },
			{ Constants.FIELD_USER_USER_MEMO, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_USER_MEMO) },
			{ Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID) },
			{ Constants.FIELD_USER_MEMBER_RANK_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_MEMBER_RANK_ID) },
			{ Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE) },
			{ Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID) },
			{ Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID) },
			{ Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR) },
			{ Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR) },
			{ Constants.FIELD_USER_COMPANY_NAME, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_COMPANY_NAME) },
			{ Constants.FIELD_USER_COMPANY_POST_NAME, userDefaultSetting.GetDefaultSettingValue(Constants.FIELD_USER_COMPANY_POST_NAME) }
		};
		return parameters;
	}

	/// <summary>
	/// Get default setting comment
	/// </summary>
	/// <param name="defaultSetting">Default setting</param>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>Comment</returns>
	public static string GetDefaultSettingComment(
		DefaultSetting defaultSetting,
		string tableName,
		string fieldName)
	{
		if (defaultSetting.DefaultSettingTables.ContainsKey(tableName))
		{
			return defaultSetting.DefaultSettingTables[tableName].GetDefaultSettingCommentField(fieldName);
		}
		return string.Empty;
	}

	/// <summary>
	/// Is default setting display field
	/// </summary>
	/// <param name="defaultSetting">Default setting</param>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>TRUE: display / False: not display</returns>
	public static bool IsDefaultSettingDisplayField(
		DefaultSetting defaultSetting,
		string tableName,
		string fieldName)
	{
		if (defaultSetting.DefaultSettingTables.ContainsKey(tableName))
		{
			return (defaultSetting.DefaultSettingTables[tableName].GetDefaultSettingDisplayField(fieldName));
		}
		return true;
	}
	#endregion

	#region +Properties
	/// <summary>Fields</summary>
	public Dictionary<string, DefaultSettingField> Fields { get; set; }
	#endregion
}
