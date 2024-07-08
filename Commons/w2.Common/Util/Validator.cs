/*
=========================================================================================================
  Module      : 入力チェックモジュール(Validator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// さまざまな入力チェックを行う
	/// </summary>
	/// <remarks>
	/// インスタンス作成不要のため抽象クラスとして定義する
	/// </remarks>
	///**************************************************************************************
	public abstract partial class Validator
	{
		/*
		XMLサンプル UserRegistInput.xml
		
		<?xml version="1.0" encoding="utf-8" ?>
		<UserRegistInput>
			<Column name="Name1">
				<name>氏名(姓)</name>
				<necessary>1</necessary>
				<type>FULLWIDTH</type>
				<length></length>
				<length_max>10</length_max>
				<length_min></length_min>
			</Column>
			・・・
		</UserRegistInput>
		*/

		// 文字タイプ
		public const string STRTYPE_FULLWIDTH = "FULLWIDTH";
		public const string STRTYPE_FULLWIDTH_HIRAGANA = "FULLWIDTH_HIRAGANA";
		public const string STRTYPE_FULLWIDTH_KATAKANA = "FULLWIDTH_KATAKANA";
		public const string STRTYPE_HALFWIDTH = "HALFWIDTH";
		public const string STRTYPE_HALFWIDTH_ALPHNUM = "HALFWIDTH_ALPHNUM";
		public const string STRTYPE_HALFWIDTH_ALPHNUMSYMBOL = "HALFWIDTH_ALPHNUMSYMBOL";
		public const string STRTYPE_HALFWIDTH_NUMBER = "HALFWIDTH_NUMBER";
		public const string STRTYPE_HALFWIDTH_DECIMAL = "HALFWIDTH_DECIMAL";
		public const string STRTYPE_HALFWIDTH_DATE = "HALFWIDTH_DATE";
		public const string STRTYPE_HALFWIDTH_INT = "HALFWIDTH_INT";
		public const string STRTYPE_DATE = "DATE";
		public const string STRTYPE_DATE_FUTURE = "DATE_FUTURE";
		public const string STRTYPE_DATE_PAST = "DATE_PAST";
		public const string STRTYPE_MAILADDRESS = "MAILADDRESS";
		public const string STRTYPE_PREFECTURE = "PREFECTURE";
		public const string STRTYPE_TAX_RATE = "TAX_RATE";
		public const string STRTYPE_CURRENCY = "CURRENCY";
		public const string STRTYPE_DROPDOWN_TIME = "DROPDOWN_TIME";
		
		// 入力チェック区分
		private const string CHECK_NECESSARY = "CHECK_NECESSARY";
		private const string CHECK_LENGTH = "CHECK_LENGTH";
		private const string CHECK_LENGTH_MAX = "CHECK_LENGTH_MAX";
		private const string CHECK_LENGTH_MIN = "CHECK_LENGTH_MIN";
		private const string CHECK_BYTE_LENGTH = "CHECK_BYTE_LENGTH";
		private const string CHECK_BYTE_LENGTH_MAX = "CHECK_BYTE_LENGTH_MAX";
		private const string CHECK_BYTE_LENGTH_MIN = "CHECK_BYTE_LENGTH_MIN";
		private const string CHECK_NUMBER_MAX = "CHECK_NUMBER_MAX";
		private const string CHECK_NUMBER_MIN = "CHECK_NUMBER_MIN";
		private const string CHECK_STRTYPE_FULLWIDTH = "CHECK_STRTYPE_FULLWIDTH";
		private const string CHECK_STRTYPE_FULLWIDTH_HIRAGANA = "CHECK_STRTYPE_FULLWIDTH_HIRAGANA";
		private const string CHECK_STRTYPE_FULLWIDTH_KATAKANA = "CHECK_STRTYPE_FULLWIDTH_KATAKANA";
		private const string CHECK_STRTYPE_HALFWIDTH = "CHECK_STRTYPE_HALFWIDTH";
		private const string CHECK_STRTYPE_HALFWIDTH_ALPHNUM = "CHECK_STRTYPE_HALFWIDTH_ALPHNUM";
		private const string CHECK_STRTYPE_HALFWIDTH_ALPHNUMSYMBOL = "CHECK_STRTYPE_HALFWIDTH_ALPHNUMSYMBOL";
		private const string CHECK_STRTYPE_HALFWIDTH_NUMBER = "CHECK_STRTYPE_HALFWIDTH_NUMBER";
		private const string CHECK_STRTYPE_HALFWIDTH_DECIMAL = "CHECK_STRTYPE_HALFWIDTH_DECIMAL";
		private const string CHECK_STRTYPE_HALFWIDTH_DATE = "CHECK_STRTYPE_HALFWIDTH_DATE";
		private const string CHECK_STRTYPE_HALFWIDTH_INT = "CHECK_STRTYPE_HALFWIDTH_INT";
		private const string CHECK_STRTYPE_DATE = "CHECK_STRTYPE_DATE";
		private const string CHECK_STRTYPE_DATE_FUTURE = "CHECK_STRTYPE_DATE_FUTURE";
		private const string CHECK_STRTYPE_DATE_PAST = "CHECK_STRTYPE_DATE_PAST";
		private const string CHECK_STRTYPE_MAILADDRESS = "CHECK_STRTYPE_MAILADDRESS";
		private const string CHECK_STRTYPE_PREFECTURE = "CHECK_STRTYPE_PREFECTURE";
		private const string CHECK_STRTYPE_TAX_RATE = "CHECK_STRTYPE_TAX_RATE";
		private const string CHECK_STRTYPE_CURRENCY = "CHECK_STRTYPE_CURRENCY";
		private const string CHECK_PROHIBITED_CHAR = "CHECK_PROHIBITED_CHAR";
		private const string CHECK_OUTOFSHIFTJISCODE = "CHECK_OUTOFSHIFTJISCODE";
		private const string CHECK_REGEXP = "CHECK_REGEXP";
		private const string CHECK_EXCEPT_REGEXP = "CHECK_EXCEPT_REGEXP";
		private const string CHECK_CONFIRM = "CHECK_CONFIRM";
		private const string CHECK_EQUIVALENCE = "CHECK_EQUIVALENCE";
		private const string CHECK_DIFFERENT_VALUE = "CHECK_DIFFERENT_VALUE";
		private const string CHECK_DUPLICATION = "CHECK_DUPLICATION";
		private const string CHECK_DUPLICATION_DATERANGE = "CHECK_DUPLICATION_DATERANGE";
		private const string CHECK_DATERANGE = "CHECK_DATERANGE";
		private const string CHECK_STRTYPE_DROPDOWN_TIME = "CHECK_DROPDOWN_TIME";
		private const string CHECK_PRICE_MAX = "CHECK_PRICE_MAX";

		/// <summary>ＸＭＬチェック用区分</summary>
		private static List<string> XmlCheckTypes { get; set; }

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static Validator()
		{
			//------------------------------------------------------
			// チェックタイプ設定
			//------------------------------------------------------
			string[] XML_CHECK_TYPES = new string[] {
				"name", "necessary", "type", "accept", "prohibit",
				"length", "length_max", "length_min",
				"byte_length", "byte_length_max", "byte_length_min",
				"number_max", "number_min",
				"except_sjis", "except_jis",
				"except_unable_convert_sjis", "except_unable_convert_sjis_add",
				"regexp",
				"except_regexp",
				"confirm",
				"equivalence",
				"different_value",
				"duplication",
				"duplication_daterange",
				"currency_localeId",
				"currency_decimal_digits",
				"price_max"
			};
			XmlCheckTypes = new List<string>(XML_CHECK_TYPES);

			//------------------------------------------------------
			// ログディレクトリ名補正
			//------------------------------------------------------
			if (Constants.PHYSICALDIRPATH_VALIDATOR.EndsWith(@"\") == false)
			{
				// 排他制御
				lock (Constants.PHYSICALDIRPATH_VALIDATOR)
				{
					// 念のため二重チェック
					if (Constants.PHYSICALDIRPATH_VALIDATOR.EndsWith(@"\") == false)
					{
						Constants.PHYSICALDIRPATH_VALIDATOR += @"\";
					}
				}
			}
		}

		/// <summary>
		/// 入力＆重複コントロールチェック
		/// </summary>
		/// <param name="strCheckKbn">対象チェック区分（ハイフン"-"区切り複数指定可）</param>
		/// <param name="strControlName">チェック対象コントロール名（コントロールチェックの場合）</param>
		/// <param name="objValue">チェック値（コントロールチェックの場合）</param>
		/// <param name="languageLocaleId">国ロケールID（項目名切替用）</param>
		/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
		/// <remarks>
		/// 設定ファイルXML（入力チェック仕様）を読み込み、それに従ってデータの入力チェックを行う。
		/// ここではXMLの記述ミス解消のためXMLの記述ミスチェックも行っている。
		/// </remarks>
		public static ErrorMessageList ValidateControl(
			string strCheckKbn,
			string strControlName,
			object objValue,
			string languageLocaleId = "",
			string countryIsoCode = "")
		{
			return Validate(
				strCheckKbn,
				GetValidateXmlDocument(strCheckKbn, languageLocaleId: languageLocaleId, countryIsoCode: countryIsoCode),
				null,
				strControlName,
				objValue,
				false);
		}

		/// <summary>
		/// ＤＢ重複一括チェック
		/// </summary>
		/// <param name="strCheckKbn">対象チェック区分（ハイフン"-"区切り複数指定可）</param>
		/// <param name="dicParam">チェック値</param>
		/// <returns>エラーメッセージ</returns>
		public static ErrorMessageList ValidteDuplication(string strCheckKbn, IDictionary dicParam)
		{
			return Validate(strCheckKbn, GetValidateXmlDocument(strCheckKbn), dicParam, null, null, true);
		}

		/// <summary>
		/// 入力＆重複一括チェック
		/// </summary>
		/// <param name="strCheckKbn">対象チェック区分（ハイフン"-"区切り複数指定可）</param>
		/// <param name="dicParam">チェック値</param>
		/// <param name="languageLocaleId">国ロケールID（項目名切替用）</param>
		/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 設定ファイルXML（入力チェック仕様）を読み込み、それに従ってデータの入力チェックを行う。
		/// ここではXMLの記述ミス解消のためXMLの記述ミスチェックも行っている。
		/// </remarks>
		public static ErrorMessageList Validate(
			string strCheckKbn,
			IDictionary dicParam,
			string languageLocaleId = "",
			string countryIsoCode = "")
		{
			return Validate(
				strCheckKbn,
				GetValidateXmlDocument(strCheckKbn, languageLocaleId: languageLocaleId, countryIsoCode: countryIsoCode),
				dicParam,
				null,
				null,
				false);
		}

		/// <summary>
		/// 入力＆重複一括チェック（ValidateXml利用）
		/// </summary>
		/// <param name="strCheckKbn">対象チェック区分</param>
		/// <param name="strValidateXml">バリデータXML文字列</param>
		/// <param name="dicParam">チェック値</param>
		/// <param name="languageLocaleId">国ロケールID（項目名切替用）</param>
		/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 設定ファイルXML（入力チェック仕様）を読み込み、それに従ってデータの入力チェックを行う。
		/// ここではXMLの記述ミス解消のためXMLの記述ミスチェックも行っている。
		/// </remarks>
		public static ErrorMessageList Validate(string strCheckKbn, string strValidateXml, IDictionary dicParam, string languageLocaleId = "", string countryIsoCode = "")
		{
			return Validate(
				strCheckKbn,
				GetValidateXmlDocument(strCheckKbn, strValidateXml, languageLocaleId, countryIsoCode),
				dicParam,
				null,
				null,
				false);
		}

		/// <summary>
		/// 入力＆重複一括チェック
		/// </summary>
		/// <param name="strCheckKbn">対象チェック区分</param>
		/// <param name="xdValidatorXml">バリデータXMLドキュメント</param>
		/// <param name="dicParam">チェック値（一括チェックの場合）</param>
		/// <param name="strControlName">チェック対象コントロール名（コントロールチェックの場合）</param>
		/// <param name="objValue">チェック値（コントロールチェックの場合）</param>
		/// <param name="blCheckOnlyDuplicaion">重複チェックのみ実行フラグ（trueの場合重複チェックのみ）</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		///	設定ファイルXML（入力チェック仕様）を読み込み、それに従ってデータの入力チェックを行う。
		/// ここではXMLの記述ミス解消のためXMLの記述ミスチェックも行っている。
		/// また、チェックには２種類ある
		/// ・一括チェック（複数の値を一度にチェックを行う。サーバー側検証用）
		/// ・コントロールチェック（コントロールの値のチェックを行う。クライアント側検証用）
		/// </remarks>
		protected static ErrorMessageList Validate(
			string strCheckKbn,
			XmlDocument xdValidatorXml,
			IDictionary dicParam,
			string strControlName,
			object objValue,
			bool blCheckOnlyDuplicaion)
		{
			ErrorMessageList emlErrorMessages = new ErrorMessageList();
			bool blChecked = false;

			//------------------------------------------------------
			// 入力チェック実行
			//------------------------------------------------------
			foreach (XmlNode xnColumnNode in xdValidatorXml.SelectNodes(strCheckKbn + "/Column"))
			{
				//------------------------------------------------------
				// チェック値・表示名ー取得
				//------------------------------------------------------
				string strValueKey = xnColumnNode.Attributes["name"].InnerText;
				string strDisplayName = xnColumnNode.SelectSingleNode("name").InnerText;

				string strValue = null;

				//------------------------------------------------------
				// コントロールチェックの場合の共通処理
				//------------------------------------------------------
				if (strControlName != null)
				{
					// 対象コントロールでない場合はcontinue
					if ((xnColumnNode.Attributes["control"] == null)
						|| (xnColumnNode.Attributes["control"].InnerText != strControlName))
					{
						continue;
					}

					// 値変換
					strValue = StringUtility.ToEmpty(objValue);
				}
				//------------------------------------------------------
				// 一括チェックの場合の共通処理
				//------------------------------------------------------
				else
				{
					if (dicParam.Contains(strValueKey))
					{
						strValue = (string)dicParam[strValueKey];
					}
					if (strValue == null)
					{
						continue;
					}
				}

				//------------------------------------------------------
				// 対象カラムの入力チェックを行う
				//------------------------------------------------------
				try
				{
					emlErrorMessages.AddRange(
						ValidateColumn(strCheckKbn, xnColumnNode, strValueKey, strDisplayName, strValue, dicParam, blCheckOnlyDuplicaion));

					blChecked = true;
				}
				catch (Exception ex)
				{
					throw new w2Exception(strValueKey + "の入力チェックで例外が発生しました。", ex);
				}

			} // foreach (XmlNode xnColumnNode in xnRootNode.SelectNodes("Column"))

			//------------------------------------------------------
			// コントロールチェックでは対象のチェックが見つからない場合エラーとする
			//------------------------------------------------------
			if ((blChecked == false)
				&& (strControlName != null))
			{
				throw new w2Exception(strCheckKbn + " の " + strControlName + " コントロールチェックが見つかりませんでした。");
			}

			return emlErrorMessages;
		}

		/// <summary>
		/// バリデータXMLドキュメント取得（内容チェックも行う）
		/// </summary>
		/// <param name="checkKbn">対象チェック区分</param>
		/// <param name="validateXmlText">バリデータXML文字列</param>
		/// <param name="languageLocaleId">国ローケールID（項目名切替用）</param>
		/// <param name="countryIsoCode">国ISOコード（項目名切替用）</param>
		public static XmlDocument GetValidateXmlDocument(string checkKbn, string validateXmlText = "", string languageLocaleId = "", string countryIsoCode = "")
		{
			// テキスト文字取得
			if (string.IsNullOrEmpty(validateXmlText)) validateXmlText = GetValidateXmlText(checkKbn);

			// タグ置換処理
			// HACK:置換タグが見つからない時にエラーになっていたのを、置換しないようにした。（テストコードだと置換タグねつ造が手間）この方法でいいか要検討。
			var strReplacedValidateXml = (Constants.TAG_REPLACER_DATA_SCHEMA != null)
				? Constants.TAG_REPLACER_DATA_SCHEMA.ReplaceTextAll(validateXmlText, languageLocaleId, countryIsoCode)
				: validateXmlText;

			//------------------------------------------------------
			// XML読み込みチェック
			//------------------------------------------------------
			XmlDocument xdValidateXml = new XmlDocument();
			try
			{
				xdValidateXml.LoadXml(strReplacedValidateXml);
			}
			catch (System.IO.IOException)
			{
				throw new w2Exception("XML「" + checkKbn + "」の読み込みに失敗しました。");
			}

			//------------------------------------------------------
			// ルートノードチェック
			//------------------------------------------------------
			XmlNode xnRootNode = xdValidateXml.SelectSingleNode(checkKbn);
			if (xnRootNode == null)
			{
				throw new w2Exception("Validation「" + checkKbn + "」のルートノードの読み込みに失敗しました。");
			}

			//------------------------------------------------------
			// 入力チェック実行
			//------------------------------------------------------
			foreach (XmlNode xnColumnNode in xnRootNode.SelectNodes("Column"))
			{
				// チェック値のキー取得
				string strValueKey = null;
				try
				{
					strValueKey = xnColumnNode.Attributes["name"].InnerText;
				}
				catch (Exception ex)
				{
					throw new w2Exception("Validation「" + checkKbn + "」の入力チェックXMLに誤りがあります。", ex);
				}

				//------------------------------------------------------
				// チェックXMLチェック（チェック仕様名に合致しているか）
				//------------------------------------------------------
				foreach (XmlNode xn in xnColumnNode.ChildNodes)
				{
					if (xn.NodeType == XmlNodeType.Comment)
					{
						continue;
					}

					if (XmlCheckTypes.Contains(xn.Name) == false)
					{
						throw new w2Exception("Validation「" + checkKbn + "」の項目「" + xn.Name + "」は仕様にありません。");
					}
				}
			}

			return xdValidateXml;
		}

		/// <summary>
		/// バリデータXMLドキュメント文字列取得
		/// </summary>
		/// <param name="checkKbn">対象チェック区分（ハイフン"-"区切り複数指定可）</param>
		private static string GetValidateXmlText(string checkKbn)
		{
			// ルートノード作成
			var validXmlFileFullPath = "";
			try
			{
				var xdoc = new XDocument(new XElement(checkKbn));

				// 指定バリデータXMLドキュメント数分ループし、入力チェックカラムを結合
				// ＜例＞対象チェック区分が「OrderShipping-OrderPayment」の場合、OrderShipping.xml、OrderPayment.xmlを読込
				foreach (var validXmlFile in checkKbn.Split('-'))
				{
					validXmlFileFullPath = Constants.PHYSICALDIRPATH_VALIDATOR + validXmlFile + ".xml";
					xdoc.Element(checkKbn).Add(XElement.Load(validXmlFileFullPath).Elements("Column"));
				}

				var result = xdoc.ToString();
				return result;
			}
			catch (System.IO.IOException)
			{
				throw new w2Exception("ファイル「" + validXmlFileFullPath + "」の読み込みに失敗しました。");
			}
		}

		/// <summary>
		/// カラムのValidate
		/// </summary>
		/// <param name="strCheckKbn">チェック区分</param>
		/// <param name="xnColumnNode">対象カラムチェックXML</param>
		/// <param name="strValueKey">値キー</param>
		/// <param name="strDisplayName">表示名</param>
		/// <param name="strValue">値</param>
		/// <param name="dicParam">パラメタ</param>
		/// <param name="blCheckOnlyDuplicaion">重複チェックのみチェックフラグ</param>
		/// <returns>エラーメッセージリスト</returns>
		private static ErrorMessageList ValidateColumn(
			string strCheckKbn,
			XmlNode xnColumnNode,
			string strValueKey,
			string strDisplayName,
			string strValue,
			IDictionary dicParam,
			bool blCheckOnlyDuplicaion)
		{
			ErrorMessageList emlErrorMessages = new ErrorMessageList();

			string strCheckType = null;
			//------------------------------------------------------
			// 入力チェック開始
			//------------------------------------------------------
			if (blCheckOnlyDuplicaion == false)
			{
				//------------------------------------------------------
				// 必須チェック？
				//------------------------------------------------------
				string strErrorMessageBuff = "";
				if ((xnColumnNode.SelectSingleNode("necessary") != null) &&
					((strCheckType = xnColumnNode.SelectSingleNode("necessary").InnerText) == "1"))
				{
					strErrorMessageBuff = CheckNecessaryError(strDisplayName, strValue);
					emlErrorMessages.Add(strValueKey, strErrorMessageBuff);
				}

				// 必須チェックエラーでない人のみ以降のチェック実行
				if (strErrorMessageBuff == "")
				{
					//------------------------------------------------------
					// 同値チェック？
					//------------------------------------------------------
					if (dicParam != null)
					{
						if ((xnColumnNode.SelectSingleNode("equivalence") != null)
							&& ((strCheckType = xnColumnNode.SelectSingleNode("equivalence").InnerText) != ""))
						{
							string strEquivalenceName = xnColumnNode.SelectSingleNode("equivalence").Attributes["target_name"].InnerText;
							strErrorMessageBuff += CheckEquivalence(strDisplayName, strEquivalenceName, strValue, dicParam[strCheckType].ToString());
							emlErrorMessages.Add(strValueKey, strErrorMessageBuff);
						}
					}

					//------------------------------------------------------
					// 異値チェック？
					//------------------------------------------------------
					// 同値チェックエラーのときは以降の形式チェックは行わない
					if ((dicParam != null)
						&& (strErrorMessageBuff == ""))
					{
						if ((xnColumnNode.SelectSingleNode("different_value") != null)
							&& ((strCheckType = xnColumnNode.SelectSingleNode("different_value").InnerText) != ""))
						{
							string strDifferentValueName = xnColumnNode.SelectSingleNode("different_value").Attributes["target_name"].InnerText;
							strErrorMessageBuff += CheckDifferentValue(strDisplayName, strDifferentValueName, strValue, dicParam[strCheckType].ToString());
							emlErrorMessages.Add(strValueKey, strErrorMessageBuff);
						}
					}

					//------------------------------------------------------
					// 確認用入力チェック？
					//------------------------------------------------------
					// 異値チェック、同値チェックエラーのときは以降の形式チェックは行わない
					if ((dicParam != null)
						&& (strErrorMessageBuff == ""))
					{
						if ((xnColumnNode.SelectSingleNode("confirm") != null) &&
							((strCheckType = xnColumnNode.SelectSingleNode("confirm").InnerText) != ""))
						{
							strErrorMessageBuff += CheckConfirmError(strDisplayName, strValue, dicParam[strCheckType].ToString());
							emlErrorMessages.Add(strValueKey, strErrorMessageBuff);
						}
					}

					// 確認入力チェックエラーのときは以降の形式チェックは行わない
					if (strErrorMessageBuff == "")
					{
						// 文字列が空の時は文字数チェックは行わない
						if (strValue != "")
						{
							//------------------------------------------------------
							// 文字数チェック？
							//------------------------------------------------------
							if ((xnColumnNode.SelectSingleNode("length") != null) &&
								((strCheckType = xnColumnNode.SelectSingleNode("length").InnerText) != ""))
							{
								emlErrorMessages.Add(strValueKey, CheckLengthError(strDisplayName, strValue, int.Parse(strCheckType)));
							}
							else
							{
								//------------------------------------------------------
								// 最大文字数・最小文字数チェック？
								//------------------------------------------------------
								if ((xnColumnNode.SelectSingleNode("length_max") != null) &&
									((strCheckType = xnColumnNode.SelectSingleNode("length_max").InnerText) != ""))
								{
									emlErrorMessages.Add(strValueKey, CheckLengthMaxError(strDisplayName, strValue, int.Parse(strCheckType)));
								}
								if ((xnColumnNode.SelectSingleNode("length_min") != null) &&
									((strCheckType = xnColumnNode.SelectSingleNode("length_min").InnerText) != ""))
								{
									emlErrorMessages.Add(strValueKey, CheckLengthMinError(strDisplayName, strValue, int.Parse(strCheckType)));
								}
							}

							//------------------------------------------------------
							// 文字バイト数チェック？
							//------------------------------------------------------
							if ((xnColumnNode.SelectSingleNode("byte_length") != null) &&
								((strCheckType = xnColumnNode.SelectSingleNode("byte_length").InnerText) != ""))
							{
								emlErrorMessages.Add(strValueKey, CheckByteLengthError(strDisplayName, strValue, int.Parse(strCheckType)));
							}
							else
							{
								//------------------------------------------------------
								// 最大文字バイト数・最小文字バイト数チェック？
								//------------------------------------------------------
								if ((xnColumnNode.SelectSingleNode("byte_length_max") != null) &&
									((strCheckType = xnColumnNode.SelectSingleNode("byte_length_max").InnerText) != ""))
								{
									emlErrorMessages.Add(strValueKey, CheckByteLengthMaxError(strDisplayName, strValue, int.Parse(strCheckType)));
								}
								if ((xnColumnNode.SelectSingleNode("byte_length_min") != null) &&
									((strCheckType = xnColumnNode.SelectSingleNode("byte_length_min").InnerText) != ""))
								{
									emlErrorMessages.Add(strValueKey, CheckByteLengthMinError(strDisplayName, strValue, int.Parse(strCheckType)));
								}
							}
						}

						//------------------------------------------------------
						// 入力許可文字列設定取得
						//------------------------------------------------------
						string strAcceptChars = "";
						if (xnColumnNode.SelectSingleNode("accept") != null)
						{
							strAcceptChars = xnColumnNode.SelectSingleNode("accept").InnerText;
						}

						//------------------------------------------------------
						// 通貨判定用のロケールIDを取得
						//------------------------------------------------------
						var currencyLocaleId = "";
						if (xnColumnNode.SelectSingleNode("currency_localeId") != null)
						{
							currencyLocaleId = xnColumnNode.SelectSingleNode("currency_localeId").InnerText;
						}

						//------------------------------------------------------
						// 通貨判定用の補助単位 小数点以下の有効桁数を取得
						//------------------------------------------------------
						int? currencyDecimalDigits = null;
						if ((xnColumnNode.SelectSingleNode("currency_decimal_digits") != null)
							&& string.IsNullOrEmpty(xnColumnNode.SelectSingleNode("currency_decimal_digits").InnerText) == false)
						{
							currencyDecimalDigits = int.Parse(xnColumnNode.SelectSingleNode("currency_decimal_digits").InnerText);
						}

						//------------------------------------------------------
						// 入力文字形式チェック？
						//	※コントロールチェック(dicParam == null)であれば型チェックは簡易でOK
						//------------------------------------------------------
						strErrorMessageBuff = "";
						if ((xnColumnNode.SelectSingleNode("type") != null) &&
							((strCheckType = xnColumnNode.SelectSingleNode("type").InnerText) != ""))
						{
							string strTarget = strValue;

							// 各種文字形式チェック
							switch (strCheckType)
							{
								case STRTYPE_FULLWIDTH:
									strTarget = (dicParam == null) ? StringUtility.ToZenkaku(strValue) : strValue;
									strErrorMessageBuff = CheckFullwidthError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_FULLWIDTH_HIRAGANA:
									strTarget = (dicParam == null) ? StringUtility.ToZenkakuHiragana(strValue) : strValue;
									strErrorMessageBuff = CheckFullwidthHiraganaError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_FULLWIDTH_KATAKANA:
									strTarget = (dicParam == null) ? StringUtility.ToZenkakuKatakana(strValue) : strValue;
									strErrorMessageBuff = CheckFullwidthKatakanaError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_HALFWIDTH:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_HALFWIDTH_ALPHNUM:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthAlphNumError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthAlphNumSymbolError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_HALFWIDTH_NUMBER:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthNumberError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_HALFWIDTH_DECIMAL:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthDecimalError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_HALFWIDTH_DATE:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthDateError(strDisplayName, strTarget);
									break;

								case STRTYPE_HALFWIDTH_INT:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckHalfwidthIntError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_DATE:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckDateError(strDisplayName, strTarget);
									break;

								case STRTYPE_DATE_FUTURE:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckDateFutureError(strDisplayName, strTarget);
									break;

								case STRTYPE_DATE_PAST:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckDatePastError(strDisplayName, strTarget);
									break;

								case STRTYPE_MAILADDRESS:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckMailAddressError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_PREFECTURE:
									strErrorMessageBuff = CheckPrefectureError(strDisplayName, RemoveChars(strTarget, strAcceptChars));
									break;

								case STRTYPE_TAX_RATE:
									strErrorMessageBuff = CheckTaxRateError(strDisplayName, strTarget);
									break;

								case STRTYPE_CURRENCY:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckCurrency(strDisplayName, strTarget, currencyLocaleId, currencyDecimalDigits);
									break;

								case STRTYPE_DROPDOWN_TIME:
									strTarget = (dicParam == null) ? StringUtility.ToHankaku(strValue) : strValue;
									strErrorMessageBuff = CheckTimeError(strDisplayName, strTarget);
									break;

								default:
									throw new w2Exception("Validation「" + strCheckKbn + "」で存在しない入力チェックの型：" + strCheckType);
							}
						}
						emlErrorMessages.Add(strValueKey, strErrorMessageBuff);

						// 形式チェックがＯＫの時のみ、下記チェック実行
						if (strErrorMessageBuff == "")
						{
							//------------------------------------------------------
							// 入力禁止文字チェック
							//------------------------------------------------------
							if (xnColumnNode.SelectSingleNode("prohibit") != null)
							{
								char[] chProhibitChars = xnColumnNode.SelectSingleNode("prohibit").InnerText.ToCharArray();
								emlErrorMessages.Add(strValueKey, CheckProhibitedCharError(strDisplayName, RemoveChars(strValue, strAcceptChars), chProhibitChars));
							}

							//------------------------------------------------------
							// 最大数値・最小数値チェック？
							//------------------------------------------------------
							if ((xnColumnNode.SelectSingleNode("number_max") != null) &&
								((strCheckType = xnColumnNode.SelectSingleNode("number_max").InnerText) != ""))
							{
								emlErrorMessages.Add(strValueKey, CheckNumberMaxError(strDisplayName, strValue, decimal.Parse(strCheckType), xnColumnNode.SelectSingleNode("type").InnerText));
							}
							if ((xnColumnNode.SelectSingleNode("number_min") != null) &&
								((strCheckType = xnColumnNode.SelectSingleNode("number_min").InnerText) != ""))
							{
								emlErrorMessages.Add(strValueKey, CheckNumberMinError(strDisplayName, strValue, decimal.Parse(strCheckType), xnColumnNode.SelectSingleNode("type").InnerText));
							}
							// 上限額チェック
							if ((xnColumnNode.SelectSingleNode("price_max") != null) &&
								((strCheckType = xnColumnNode.SelectSingleNode("price_max").InnerText) != ""))
							{
								emlErrorMessages.Add(strValueKey, CheckPriceMaxError(strDisplayName, strValue, decimal.Parse(strCheckType), xnColumnNode.SelectSingleNode("type").InnerText));
							}
						}

						//------------------------------------------------------
						// SJIS文字コードチェック？（タグ複数ある可能性あり）
						//------------------------------------------------------
						// 形式チェックがＯＫの時のみ、数値大小チェック実行
						if (strErrorMessageBuff == "")
						{
							if (xnColumnNode.SelectSingleNode("except_sjis") != null)
							{
								foreach (XmlNode xn in xnColumnNode.SelectNodes("except_sjis"))
								{
									if (xn.InnerText != "")
									{
										string strTrueValueName = xn.Attributes["exept_name"].Value;
										string[] strCode = xn.InnerText.Split('-');
										uint uiExceptCodeBgn = uint.Parse(strCode[0].Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
										uint uiExceptCodeEnd = uint.Parse(strCode[1].Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);

										emlErrorMessages.Add(strValueKey, CheckOutOfShiftJISCode(strDisplayName, strTrueValueName, strValue, uiExceptCodeBgn, uiExceptCodeEnd));
									}
								}
							}
						}

						//------------------------------------------------------
						// SJIS変換チェック？
						//------------------------------------------------------
						// 形式チェックがＯＫの時のみ
						if (strErrorMessageBuff == "")
						{
							if ((xnColumnNode.SelectSingleNode("except_unable_convert_sjis") != null) 
								&& (xnColumnNode.SelectSingleNode("except_unable_convert_sjis").InnerText == "1"))
							{
								string addCheckType = string.Empty;
								if (xnColumnNode.SelectSingleNode("except_unable_convert_sjis_add") != null)
								{
									//追加で除外する条件がある場合
									addCheckType = xnColumnNode.SelectSingleNode("except_unable_convert_sjis_add").InnerText;
								}

								emlErrorMessages.Add(strValueKey, CheckShiftJisConvertError(strDisplayName, strValue, addCheckType));
							}
						}

						//------------------------------------------------------
						// JIS(iso-2022-jp)文字コードチェック？（タグ複数ある可能性あり）
						//------------------------------------------------------
						// 形式チェックがＯＫの時のみ、数値大小チェック実行
						if (strErrorMessageBuff == "")
						{
							if (xnColumnNode.SelectSingleNode("except_jis") != null)
							{
								foreach (XmlNode xn in xnColumnNode.SelectNodes("except_jis"))
								{
									if (xn.InnerText != "")
									{
										string strTrueValueName = xn.Attributes["exept_name"].Value;
										string[] strCode = xn.InnerText.Split('-');
										uint uiExceptCodeBgn = uint.Parse(strCode[0].Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
										uint uiExceptCodeEnd = uint.Parse(strCode[1].Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);

										emlErrorMessages.Add(strValueKey, CheckOutOfISO2202JPCode(strDisplayName, strTrueValueName, strValue, uiExceptCodeBgn, uiExceptCodeEnd));
									}
								}
							}
						}

						//------------------------------------------------------
						// 正規表現チェック？
						//------------------------------------------------------
						if ((xnColumnNode.SelectSingleNode("regexp") != null) &&
							((strCheckType = xnColumnNode.SelectSingleNode("regexp").InnerText) != ""))
						{
							string strPattern = "";
							try
							{
								strPattern = xnColumnNode.SelectSingleNode("regexp").Attributes["ptn"].Value;
							}
							catch
							{
								// なにもしない
							}

							emlErrorMessages.Add(strValueKey, CheckRegExpError(strDisplayName, strValue, strCheckType, strPattern));
						}

						//------------------------------------------------------
						// 正規表現（除外）チェック？
						//------------------------------------------------------
						if ((xnColumnNode.SelectSingleNode("except_regexp") != null) &&
							((strCheckType = xnColumnNode.SelectSingleNode("except_regexp").InnerText) != ""))
						{
							string strPattern = "";
							try
							{
								strPattern = xnColumnNode.SelectSingleNode("except_regexp").Attributes["ptn"].Value;
							}
							catch
							{
								// なにもしない
							}

							emlErrorMessages.Add(strValueKey, CheckExceptRegExpError(strDisplayName, strValue, strCheckType, strPattern));
						}
					}
				}
			} //if (blCheckOnlyDuplicaion == false)

			//------------------------------------------------------
			// ＤＢ重複チェック
			//------------------------------------------------------
			if (dicParam != null)
			{
				// エラーがある場合には重複チェックを行わない
				if (emlErrorMessages.Count == 0)
				{
					if ((xnColumnNode.SelectSingleNode("duplication") != null) &&
						((strCheckType = xnColumnNode.SelectSingleNode("duplication").InnerText) != ""))
					{
						string strFileName = xnColumnNode.SelectSingleNode("duplication").Attributes["page"].InnerText;
						string strStatementName = xnColumnNode.SelectSingleNode("duplication").Attributes["statement"].InnerText;

						emlErrorMessages.Add(strValueKey, CheckDbDuplication(strDisplayName, strFileName, strStatementName, dicParam, strValue));
					}

					if ((xnColumnNode.SelectSingleNode("duplication_daterange") != null) &&
						((strCheckType = xnColumnNode.SelectSingleNode("duplication_daterange").InnerText) != ""))
					{
						string strFileName = xnColumnNode.SelectSingleNode("duplication_daterange").Attributes["page"].InnerText;
						string strStatementName = xnColumnNode.SelectSingleNode("duplication_daterange").Attributes["statement"].InnerText;

						emlErrorMessages.Add(strValueKey, CheckDbDuplicationDateRange(strDisplayName, strFileName, strStatementName, dicParam, strValue));
					}
				}
			}

			return emlErrorMessages;
		}

		/// <summary>
		/// 必須チェックがあるか
		/// </summary>
		/// <param name="checkKbn">対象チェック区分</param>
		/// <param name="valueKey">チェック値</param>
		/// <returns>必須チェックがあるか</returns>
		public static bool HasNecessary(string checkKbn, string valueKey)
		{
			foreach (XmlNode columnNode in GetValidateXmlDocument(checkKbn).SelectNodes(checkKbn + "/Column"))
			{
				if(valueKey != columnNode.Attributes["name"].InnerText) continue;

				return (columnNode.SelectSingleNode("necessary") != null) && ((columnNode.SelectSingleNode("necessary").InnerText) == "1");
			}
			return false;
		}

		///*********************************************************************************************
		/// <summary>
		/// エラーメッセージリスト
		/// </summary>
		///*********************************************************************************************
		public class ErrorMessageList : List<KeyValuePair<string, string>>
		{
			/// <summary>
			/// アイテム追加
			/// </summary>
			/// <param name="strKey">追加アイテムキー</param>
			/// <param name="strValue">追加アイテム</param>
			/// <remarks>空文字が格納できないようにする</remarks>
			public void Add(string strKey, string strValue)
			{
				if (strValue != "")
				{
					base.Add(new KeyValuePair<string, string>(strKey, strValue));
				}
			}
		}
	}
}