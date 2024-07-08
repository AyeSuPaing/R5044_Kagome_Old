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

namespace w2.App.Common.Util
{
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
		/// <returns>エラーメッセージ(チェック区分で分けられたディクショナリ)</returns>
		public static Dictionary<string, string> ValidateAndGetErrorContainer(string strCheckKbn, IDictionary dicParam)
		{
			Dictionary<string, string> dicErrorContainer = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> kvpMessage in w2.Common.Util.Validator.Validate(strCheckKbn, dicParam))
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
		/// 入力＆重複コントロールチェック
		/// </summary>
		/// <param name="strCheckKbn">対象チェック区分</param>
		/// <param name="strControlName">チェック対象コントロール名（コントロールチェックの場合）</param>
		/// <param name="objValue">チェック値（コントロールチェックの場合）</param>
		public static string ValidateControl(string strCheckKbn, string strControlName, object objValue)
		{
			return ChangeToValidate(w2.Common.Util.Validator.ValidateControl(strCheckKbn, strControlName, objValue));
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
		public static string ChangeToValidate(Validator.ErrorMessageList emlErrorMessages)
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
	}
}
