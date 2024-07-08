/*
=========================================================================================================
  Module      : Validatorクラス(Validator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
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
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <returns>エラーメッセージ</returns>
	public static string Validate(string strCheckKbn, IDictionary dicParam)
	{
		return ChangeToDisplay(w2.Common.Util.Validator.Validate(strCheckKbn, dicParam));
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
	/// バリデータカラム名リスト取得
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <returns>バリデータカラム名リスト</returns>
	public static List<string> GetValidatorColumns(string strCheckKbn)
	{
		XmlDocument xdValidateXml = new XmlDocument();
		string strValidXmlFilePath = w2.Common.Constants.PHYSICALDIRPATH_VALIDATOR + strCheckKbn + ".xml";
		try
		{
			xdValidateXml.Load(strValidXmlFilePath);
		}
		catch (Exception ex)
		{
			throw new System.ApplicationException("ファイル「" + strValidXmlFilePath + "」の読み込みに失敗しました。\r\n" + ex.ToString());
		}

		return GetValidatorColumns(strCheckKbn, xdValidateXml);
	}

	/// <summary>
	/// バリデータカラム名リスト取得
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="xdValidateXml">Validator用のXMLドキュメント</param>
	/// <returns>バリデータカラム名リスト</returns>
	public static List<string> GetValidatorColumns(string strCheckKbn, XmlDocument xdValidateXml)
	{
		List<string> result = new List<string>();
		try
		{
			foreach (XmlNode xnColumn in xdValidateXml.SelectSingleNode(strCheckKbn).SelectNodes("Column"))
			{
				if (xnColumn.NodeType == XmlNodeType.Comment) continue;
				result.Add(xnColumn.Attributes[0].Value);
			}
		}
		catch (Exception ex)
		{
			throw new System.ApplicationException("読み込みに失敗しました。\r\n" + ex.ToString());
		}

		return result;
	}

	/// <summary>
	/// 表示用変換（表示用）
	/// </summary>
	/// <param name="emlErrorMessages">エラーメッセージ</param>
	/// <returns>変換後文字列</returns>
	public static string ChangeToDisplay(Validator.ErrorMessageList emlErrorMessages)
	{
		StringBuilder sbDist = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpMessage in emlErrorMessages)
		{
			if (sbDist.Length != 0)
			{
				sbDist.Append("\r\n");
			}
			sbDist.Append(kvpMessage.Value);
		}

		return sbDist.ToString();
	}
}
