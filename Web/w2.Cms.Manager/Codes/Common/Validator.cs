/*
=========================================================================================================
  Module      : 検証クラス(Validator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace w2.Cms.Manager.Codes.Common
{
	/// <summary>
	/// 検証クラス
	/// </summary>
	public class Validator : w2.App.Common.Util.Validator
	{
		/// <summary>
		/// 入力＆重複コントロールチェック
		/// </summary>
		/// <param name="checkKbn">対象チェック区分</param>
		/// <param name="controlName">チェック対象コントロール名（コントロールチェックの場合）</param>
		/// <param name="value">チェック値（コントロールチェックの場合）</param>
		public new static string ValidateControl(string checkKbn, string controlName, object value)
		{
			return ChangeToDisplay(w2.Common.Util.Validator.ValidateControl(checkKbn, controlName, value));
		}

		/// <summary>
		/// 入力＆重複一括チェック
		/// </summary>
		/// <param name="checkKbn">対象チェック区分</param>
		/// <param name="param">チェック値</param>
		/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
		/// <returns>エラーメッセージ</returns>
		public static string Validate(string checkKbn, IDictionary param, string countryIsoCode = "")
		{
			return ChangeToDisplay(
				w2.Common.Util.Validator.Validate(
					checkKbn,
					param,
					Constants.GLOBAL_OPTION_ENABLE ? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE : "",
					countryIsoCode));
		}

		/// <summary>
		/// 入力＆重複一括チェック（ValidateXml利用）
		/// </summary>
		/// <param name="checkKbn">対象チェック区分</param>
		/// <param name="validateXml">バリデータXML文字列</param>
		/// <param name="param">チェック値</param>
		/// <returns>エラーメッセージ</returns>
		public new static string Validate(string checkKbn, string validateXml, IDictionary param)
		{
			return ChangeToDisplay(w2.Common.Util.Validator.Validate(checkKbn, validateXml, param));
		}

		/// <summary>
		/// 表示用変換
		/// </summary>
		/// <param name="errorMessages">変換元</param>
		/// <returns>変換後文字列</returns>
		public new static string ChangeToDisplay(Validator.ErrorMessageList errorMessages)
		{
			var dist = new StringBuilder();
			foreach (var kvpMessage in errorMessages)
			{
				dist.Append(kvpMessage.Value).Append("<br />");
			}

			return dist.ToString();
		}
	}
}