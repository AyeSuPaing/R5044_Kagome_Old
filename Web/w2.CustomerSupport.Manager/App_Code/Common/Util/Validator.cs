/*
=========================================================================================================
  Module      : Validatorクラス(Validator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Validator の概要の説明です
/// </summary>
public class Validator : w2.Common.Util.Validator
{
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
	/// ＤＢ重複一括チェック
	/// </summary>
	/// <param name="strCheckKbn">対象チェック区分</param>
	/// <param name="dicParam">チェック値</param>
	/// <returns>エラーメッセージ</returns>
	public static new string ValidteDuplication(string strCheckKbn, IDictionary dicParam)
	{
		return ChangeToDisplay(w2.Common.Util.Validator.ValidteDuplication(strCheckKbn, dicParam));
	}

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
