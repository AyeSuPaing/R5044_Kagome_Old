/*
=========================================================================================================
  Module      : Validatorクラス(Validator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;

///*********************************************************************************************
/// <summary>
/// Validatorクラス
/// </summary>
///*********************************************************************************************
public class Validator : w2.Common.Util.Validator
{
	/// <summary>
	/// 入力＆重複一括チェック
	/// </summary>
	/// <param name="checkKbn">対象チェック区分</param>
	/// <param name="parameters">チェック値</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ(チェック区分で分けられたディクショナリ)</returns>
	public static Dictionary<string, string> ValidateAndGetErrorContainer(
		string checkKbn,
		IDictionary parameters,
		string countryIsoCode = "")
	{
		var errorContainer = new Dictionary<string, string>();
		var errorMessages = w2.Common.Util.Validator.Validate(
			checkKbn,
			parameters,
			Constants.GLOBAL_OPTION_ENABLE
				? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
				: string.Empty,
			countryIsoCode);
		foreach (var errorMessage in errorMessages)
		{
			if (errorContainer.ContainsKey(errorMessage.Key))
			{
				errorContainer[errorMessage.Key] += ("\r\n" + errorMessage.Value);
				continue;
			}

			errorContainer.Add(errorMessage.Key, errorMessage.Value);
		}
		return errorContainer;
	}

	/// <summary>
	/// 入力＆重複コントロールチェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="strControlName">チェック対象コントロール名（コントロールチェックの場合）</param>
	/// <param name="objValue">チェック値（コントロールチェックの場合）</param>
	public static string ValidateControl(string strCheckKbn, string strControlName, object objValue)
	{
		return ChangeToDisplay(w2.Common.Util.Validator.ValidateControl(strCheckKbn, strControlName, objValue));
	}

	/// <summary>
	/// 入力＆重複一括チェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ</returns>
	public static string Validate(string strCheckKbn, IDictionary dicParam, XmlDocument xml, string countryIsoCode = "")
	{
		return ChangeToDisplay(
			Validator.Validate(
				strCheckKbn,
				xml.InnerXml,
				dicParam,
				Constants.GLOBAL_OPTION_ENABLE ? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE : "",
				countryIsoCode));
	}
	/// <summary>
	/// 入力＆重複一括チェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ</returns>
	public static string Validate(string strCheckKbn, IDictionary dicParam, string countryIsoCode = "")
	{
		return ChangeToDisplay(
			w2.Common.Util.Validator.Validate(
				strCheckKbn,
				dicParam,
				Constants.GLOBAL_OPTION_ENABLE ? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE : "",
				countryIsoCode));
	}

	/// <summary>
	/// 入力＆重複一括チェック（ValidateXml利用）
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="strValidateXml">バリデータXML文字列</param>
	/// <param name="dicParam">チェック値</param>
	/// <returns>エラーメッセージ</returns>
	public static string Validate(string strCheckKbn, string strValidateXml, IDictionary dicParam)
	{
		return ChangeToDisplay(w2.Common.Util.Validator.Validate(strCheckKbn, strValidateXml, dicParam));
	}

	/// <summary>
	/// 表示用変換
	/// </summary>
	/// <param name="strSrc">変換元</param>
	/// <returns>変換後文字列</returns>
	public static string ChangeToDisplay(Validator.ErrorMessageList emlErrorMessages)
	{
		StringBuilder sbDist = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpMessage in emlErrorMessages)
		{
			sbDist.Append(kvpMessage.Value).Append("<br />");
		}

		return sbDist.ToString();
	}
}
