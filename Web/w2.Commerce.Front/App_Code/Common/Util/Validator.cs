/*
=========================================================================================================
  Module      : Validatorクラス(StringUtility.cs)
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
using w2.App.Common.Global.Region;

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
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ(チェック区分で分けられたディクショナリ)</returns>
	public static Dictionary<string, string> ValidateAndGetErrorContainer(
		string strCheckKbn,
		IDictionary dicParam,
		string countryIsoCode = "")
	{
		var dicErrorContainer = new Dictionary<string, string>();
		foreach (var kvpMessage in w2.Common.Util.Validator.Validate(
			strCheckKbn,
			dicParam,
			Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
			countryIsoCode))
		{
			if (dicErrorContainer.ContainsKey(kvpMessage.Key) == false)
			{
				dicErrorContainer.Add(kvpMessage.Key, kvpMessage.Value);
			}
			else
			{
				dicErrorContainer[kvpMessage.Key] += ("\r\n" + kvpMessage.Value);
			}
		}
		return dicErrorContainer;
	}
	/// <summary>
	/// 入力＆重複一括チェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <param name="xml">バリデーションXML</param>
	/// <returns>エラーメッセージ(チェック区分で分けられたディクショナリ)</returns>
	public static Dictionary<string, string> ValidateAndGetErrorContainer(
		string strCheckKbn,
		IDictionary dicParam,
		XmlDocument xml)
	{
		var dicErrorContainer = new Dictionary<string, string>();
		foreach (var kvpMessage in Validate(
			strCheckKbn,
			xml,
			dicParam,
			null,
			null,
			false))
		{
			if (dicErrorContainer.ContainsKey(kvpMessage.Key) == false)
			{
				dicErrorContainer.Add(kvpMessage.Key, kvpMessage.Value);
			}
			else
			{
				dicErrorContainer[kvpMessage.Key] += ("\r\n" + kvpMessage.Value);
			}
		}
		return dicErrorContainer;
	}

	/// <summary>
	/// 入力＆重複一括チェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <param name="languageLocaleId">国ロケールID（項目名切替用）</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ</returns>
	public new static string Validate(
		string strCheckKbn,
		IDictionary dicParam,
		string languageLocaleId = "",
		string countryIsoCode = "")
	{
		var errorMessageList = w2.Common.Util.Validator.Validate(
			strCheckKbn,
			dicParam,
			languageLocaleId,
			countryIsoCode);
		return ChangeToDisplay(errorMessageList);

	}

	/// <summary>
	/// 入力＆重複一括チェック（ValidateXml利用）
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="strValidateXml">バリデータXML文字列</param>
	/// <param name="dicParam">チェック値</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ</returns>
	public static string Validate(
		string strCheckKbn,
		string strValidateXml,
		IDictionary dicParam,
		string countryIsoCode = "")
	{
		var errorMessageList = w2.Common.Util.Validator.Validate(
			strCheckKbn,
			strValidateXml,
			dicParam,
			LanguageLocaleId,
			countryIsoCode);
		return ChangeToDisplay(errorMessageList);
	}

	/// <summary>
	/// 重複コントロールチェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="strControlName">チェック対象コントロール名（コントロールチェックの場合）</param>
	/// <param name="objValue">チェック値（コントロールチェックの場合）</param>
	/// <param name="languageLocaleId">国ロケールID（項目名切替用）</param>
	/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
	/// <returns>エラーメッセージ</returns>
	public new static string ValidateControl(
		string strCheckKbn,
		string strControlName,
		object objValue,
		string languageLocaleId = "",
		string countryIsoCode = "")
	{
		var errorMessage = ChangeToValidate(
			w2.Common.Util.Validator.ValidateControl(
				strCheckKbn,
				strControlName,
				objValue,
				((languageLocaleId == "") ? LanguageLocaleId : languageLocaleId),
				countryIsoCode));
		return errorMessage;
	}

	/// <summary>
	/// 表示用変換（表示用）
	/// </summary>
	/// <param name="emlErrorMessages">エラーメッセージ</param>
	/// <returns>変換後文字列</returns>
	public static string ChangeToDisplay(ErrorMessageList emlErrorMessages)
	{
		StringBuilder sbDist = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpMessage in emlErrorMessages)
		{
			if (sbDist.Length != 0)
			{
				sbDist.Append("<br />");
			}
			sbDist.Append(kvpMessage.Value);
		}

		return sbDist.ToString();
	}
	/// <summary>
	/// 表示用変換（コントローラ検証用）
	/// </summary>
	/// <param name="emlErrorMessages">エラーメッセージ</param>
	/// <returns>変換後文字列</returns>
	public static string ChangeToValidate(ErrorMessageList emlErrorMessages)
	{
		StringBuilder sbDist = new StringBuilder();
		foreach (KeyValuePair<string, string> kvp in emlErrorMessages)
		{
			if (sbDist.Length != 0)
			{
				sbDist.Append("\r\n");
			}
			sbDist.Append(kvp.Value);
		}

		return sbDist.ToString();
	}


	/// <summary>言語ロケールID</summary>
	private static string LanguageLocaleId
	{
		get { return Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : ""; }
	}
}
