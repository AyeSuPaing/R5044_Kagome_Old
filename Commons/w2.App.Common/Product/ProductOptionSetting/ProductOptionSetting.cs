/*
=========================================================================================================
  Module      : 商品付帯情報クラス(ProductOptionSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Product.Helper;

namespace w2.App.Common.Product
{
	///*********************************************************************************************
	/// <summary>
	/// 商品付帯情報クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class ProductOptionSetting
	{
		// テキストボックス用設定のKey
		private const string KEY_DEFAULTVALUE = "DefaultValue";		// 初期値
		private const string KEY_NECESSARY = "Necessary";			// 入力必須チェック
		private const string KEY_TYPE = "Type";						// 入力タイプ
		private const string KEY_LENGTH = "Length";					// 固定文字列長
		private const string KEY_LENGTHMIN = "LengthMin";			// 最低文字列長
		private const string KEY_LENGTHMAX = "LengthMax";			// 最大文字列長
		private const string KEY_MINVALUE = "MinValue";				// 最小値
		private const string KEY_MAXVALUE = "MaxValue";				// 最大値

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductOptionSetting()
		{
			this.DisplayKbn = "";
			this.ValueName = "";
			this.SettingValues = new List<string>();
			this.SelectedSettingValue = "";
			this.OptionPriceDictionary = new Dictionary<string, decimal>();
			this.OptionPriceSettingValues = new List<string>();
			this.MatchesLatestProductMaster = true;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strProductOptionValue">商品付帯情報文字列</param>
		public ProductOptionSetting(string strProductOptionSetting)
		{
			SetProductOptionSetting(strProductOptionSetting);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strDisplayKbn">表示区分</param>
		/// <param name="strValueName">項目名</param>
		/// <param name="lSettingValues">設定値一覧</param>
		public ProductOptionSetting(string strDisplayKbn, string strValueName, List<string> lSettingValues)
		{
			this.DisplayKbn = strDisplayKbn;
			this.ValueName = strValueName;
			this.SettingValues = lSettingValues;
			this.OptionPriceDictionary = new Dictionary<string, decimal>();
			this.MatchesLatestProductMaster = true;
		}

		/// <summary>
		/// コンストラクタ（テキストボックス用）
		/// </summary>
		/// <param name="displayKbn">表示区分</param>
		/// <param name="valueName">項目名</param>
		/// <param name="defaultValue">初期値</param>
		/// <param name="necessary">入力必須チェック</param>
		/// <param name="type">入力タイプ</param>
		/// <param name="length">固定文字列長</param>
		/// <param name="lengthMin">最低文字列長</param>
		/// <param name="lengthMax">最大文字列長</param>
		/// <param name="minValue">最低値</param>
		/// <param name="maxValue">最大値</param>
		public ProductOptionSetting(string displayKbn, string valueName, string defaultValue, string necessary, string type, string length, string lengthMin, string lengthMax, string minValue, string maxValue)
		{
			this.DisplayKbn = displayKbn;
			this.ValueName = valueName;
			this.DefaultValue = defaultValue;
			this.Necessary = necessary;
			this.Type = type;
			this.Length = length;
			this.LengthMin = lengthMin;
			this.LengthMax = lengthMax;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.MatchesLatestProductMaster = true;
		}

		/// <summary>
		/// <para>商品付帯情報文字列取得</para>
		/// <para>商品付帯情報を文字列変換し返却する</para>
		/// </summary>
		/// <param name="strProductOptionValue">商品付帯情報文字列</param>
		public string GetProductOptionSettingString()
		{
			StringBuilder sbProductOptionSetting = new StringBuilder();

			sbProductOptionSetting.Append("[[");
			// 表示区分
			sbProductOptionSetting.Append(this.DisplayKbn);
			// 項目名
			sbProductOptionSetting.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE).Append(this.ValueName);

			switch (this.DisplayKbn)
			{
				// チェックボックス、ドロップダウン
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX:
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU:
					// 設定値
					foreach (string str in this.SettingValues)
					{
						sbProductOptionSetting.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE);
						sbProductOptionSetting.Append(str);
					}
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.Necessary) 
						? ""
						: Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_NECESSARY + "=" + this.Necessary);
					break;

				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX:
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU:
					// 設定値
					foreach (string str in this.OptionPriceSettingValues)
					{
						sbProductOptionSetting.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE);
						sbProductOptionSetting.Append(str);
					}
					break;

				// テキストボックス
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX:
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.DefaultValue) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_DEFAULTVALUE + "=" + this.DefaultValue);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.Necessary) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_NECESSARY + "=" + this.Necessary);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.Type) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_TYPE + "=" + this.Type);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.Length) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_LENGTH + "=" + this.Length);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.LengthMin) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_LENGTHMIN + "=" + this.LengthMin);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.LengthMax) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_LENGTHMAX + "=" + this.LengthMax);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.MinValue) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_MINVALUE + "=" + this.MinValue);
					sbProductOptionSetting.Append(string.IsNullOrEmpty(this.MaxValue) ? "" : Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE + KEY_MAXVALUE + "=" + this.MaxValue);
					break;
			}

			sbProductOptionSetting.Append("]]");

			return sbProductOptionSetting.ToString();
		}

		/// <summary>
		/// <para>商品付帯情報設定</para>
		/// <para>引数を文字列分解し、表示区分、項目名、設定値を設定する</para>
		/// </summary>
		/// <param name="strProductOptionSetting">商品付帯情報文字列</param>
		/// <returns>付帯情報のフォーマット通り出なければfalseを返す</returns>
		public bool SetProductOptionSetting(string strProductOptionSetting)
		{
			// フォーマット以外であればfalseを返す
			if (Regex.IsMatch(strProductOptionSetting, Constants.REGEX_PATTERN_PRODUCT_OPTION_STRICT))
			{
				return false;
			}

			// 区切り文字を元に分解
			List<string> lProductOptionSettings = strProductOptionSetting.Replace("@@", "\n").Split('\n').ToList<string>();

			// 最低でも３つ以上項目が存在するので、それ以下の場合は処理しない
			if (lProductOptionSettings.Count < 3)
			{
				return false;
			}

			// 表示区分
			this.DisplayKbn = lProductOptionSettings[0];
			// 項目名
			lProductOptionSettings[1] = lProductOptionSettings[1].Replace(
				Constants.PRODUCTOPTIONVALUES_ESCAPE_STR,
				Constants.PRODUCTOPTIONVALUES_ESCAPE_TARGET_CHAR);
			this.ValueName = lProductOptionSettings[1];
			// 表示区分・項目名をリストから削除
			lProductOptionSettings.Remove(lProductOptionSettings[0]);
			lProductOptionSettings.Remove(lProductOptionSettings[0]);
			this.SettingValues = new List<string>();
			this.OptionPriceDictionary = new Dictionary<string, decimal>();
			this.OptionPriceSettingValues = new List<string>();
			this.MatchesLatestProductMaster = true;

			switch (this.DisplayKbn)
			{
				// チェックボックス、ドロップダウン
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX:
					// 設定値
					foreach (var productOptionSetting in lProductOptionSettings)
					{
						if (productOptionSetting.Contains("Necessary="))
						{
							var settingString = productOptionSetting.Split('=');
							this.Necessary = settingString[1];
						}
						else
						{
							var replacedValue = productOptionSetting.Replace(
								Constants.PRODUCTOPTIONVALUES_ESCAPE_STR,
								Constants.PRODUCTOPTIONVALUES_ESCAPE_TARGET_CHAR);
							this.SettingValues.Add(replacedValue);
						}
					}
					break;

				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU:
					// 設定値
					foreach (var productOptionSetting in lProductOptionSettings)
					{
						if (productOptionSetting.Contains("Necessary="))
						{
							var settingString = productOptionSetting.Split('=');
							this.Necessary = settingString[1];
						}
						else if (productOptionSetting.Contains("DefaultValue="))
						{
							var settingString = productOptionSetting.Split('=');
							this.DefaultValue = settingString[1];
							this.SettingValues.Add(this.DefaultValue.Replace(
								Constants.PRODUCTOPTIONVALUES_ESCAPE_STR,
								Constants.PRODUCTOPTIONVALUES_ESCAPE_TARGET_CHAR));
						}
						else
						{
							var replacedValue = productOptionSetting.Replace(
								Constants.PRODUCTOPTIONVALUES_ESCAPE_STR,
								Constants.PRODUCTOPTIONVALUES_ESCAPE_TARGET_CHAR);
							this.SettingValues.Add(replacedValue);
						}
					}
					break;

				// チェックボックス(価格)、ドロップダウン(価格)
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX:
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU:
					// 設定値
					foreach (var optionSetting in lProductOptionSettings)
					{
						var price = 0m;
						this.OptionPriceSettingValues.Add(optionSetting);
						var optionKeyValue = Regex.Replace(optionSetting, Constants.REGEX_PATTERN_OPTION_PRICE_SEARCH_PATTERN, match =>
						{
							price = ObjectUtility.TryParseDecimal(Regex.Match(match.Value, @"[0-9]+").Value, 0);
							return "(+" + CurrencyManager.ToPrice(price) + ")";
						});
						this.SettingValues.Add(optionKeyValue);

						if ((string.IsNullOrEmpty(optionKeyValue) == false) && (this.OptionPriceDictionary.ContainsKey(optionKeyValue) == false))
						{
							this.OptionPriceDictionary.Add(optionKeyValue, price);
						}
					}
					break;

				// テキストボックス
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX:
					foreach (string productOptionSetting in lProductOptionSettings)
					{
						string[] settingString = productOptionSetting.Split('=');
						var replacedValue = settingString[1].Replace(
							Constants.PRODUCTOPTIONVALUES_ESCAPE_STR,
							Constants.PRODUCTOPTIONVALUES_ESCAPE_TARGET_CHAR);
						switch (settingString[0])
						{
							case KEY_DEFAULTVALUE:
								this.DefaultValue = replacedValue;
								break;

							case KEY_NECESSARY:
								this.Necessary = replacedValue;
								break;

							case KEY_TYPE:
								this.Type = replacedValue;
								break;

							case KEY_LENGTH:
								this.Length = replacedValue;
								break;

							case KEY_LENGTHMIN:
								this.LengthMin = replacedValue;
								break;

							case KEY_LENGTHMAX:
								this.LengthMax = replacedValue;
								break;

							case KEY_MINVALUE:
								this.MinValue = replacedValue;
								break;

							case KEY_MAXVALUE:
								this.MaxValue = replacedValue;
								break;
						}
					}
					break;
			}

			return true;
		}

		/// <summary>
		/// <para>商品付帯情報html</para>
		/// <para>商品付帯情報選択用htmlを作成し返却する</para>
		/// </summary>
		/// <param name="iNo">設定値番号</param>
		/// <param name="blIsMobile">モバイルか？</param>
		/// <returns>商品付帯情報htmlタグ</returns>
		public string GetHtmlProductOptionSetting(int iNo, bool blIsMobile)
		{
			if (this.DisplayKbn != Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX && this.SettingValues.Count == 0)
			{
				return "";
			}

			// 表示区分がチェックボックスの場合、チェックボックス用のhtmlタグを作成
			StringBuilder sbHtml = new StringBuilder();
			string strName = Constants.PRODUCTOPTIONVALUES_HTML_TAG_VALUE_HEAD + iNo.ToString();
			if (this.IsCheckBox)
			{
				foreach (string strSettingValue in this.SettingValues)
				{
					// モバイルの場合、各表示項目は半角カタカナ制御でカタカナは自動で半角化
					string strSettingValueTmp = (blIsMobile) ? StringUtility.ToHankakuKatakana(strSettingValue) : strSettingValue;

					sbHtml.Append("<input type=\"checkbox\" name=\"")
						  .Append(strName).Append("\"")
						  .Append(" value=\"").Append(strSettingValue).Append("\"")
						  .Append((StringUtility.ToEmpty(this.SelectedSettingValue) != "") ? " checked" : "")
						  .Append(" />")
						  .Append(HtmlSanitizer.HtmlEncode(strSettingValueTmp));
				}
			}
			// 表示区分がセレクトメニューの場合、セレクトメニュー用のhtmlタグを作成
			// モバイルの場合、各表示項目は半角カタカナ制御でカタカナは自動で半角化
			else if (this.IsSelectMenu)
			{
				sbHtml.Append("<select name=\"").Append(strName).Append("\" >");
				foreach (string strSettingValue in this.SettingValues)
				{
					string strSettingValueTmp = (blIsMobile) ? StringUtility.ToHankakuKatakana(strSettingValue) : strSettingValue;

					sbHtml.Append("<option value=\"").Append(strSettingValue).Append("\"")
						.Append((strSettingValueTmp == this.SelectedSettingValue) ? " selected" : "").Append(" >")
						.Append(HtmlSanitizer.HtmlEncode(strSettingValueTmp)).Append("</option>");
				}
				sbHtml.Append("</select>");
			}
			// 表示区分がテキストボックスの場合、テキストボックス用のhtmlタグを作成
			else if (this.IsTextBox)
			{
				sbHtml.Append("<input name=\"").Append(strName).Append("\" type=\"text\" value=\"").Append(this.DefaultValue).Append("\">");
				if (this.IsNecessary) sbHtml.Append("<font color=\"red\">*</font>");
			}
			return sbHtml.ToString();
		}

		/// <summary>
		/// <para>商品付帯情報 表示区分取得</para>
		/// <para>商品付帯情報の表示区分を返却する</para>
		/// </summary>
		/// <returns>表示区分 C：チェックボックス、S：セレクトメニュー</returns>
		public string GetProductOptionSettingDisplayKbn()
		{
			if (this.IsCheckBox)
			{
				return "チェックボックス";
			}
			else if (this.IsSelectMenu)
			{
				return "ドロップダウン";
			}
			else if (this.IsTextBox)
			{
				return "テキストボックス";
			}
			else if (this.IsDropDownPrice)
			{
				return "ドロップダウン(価格)";
			}
			else if (this.IsCheckBoxPrice)
			{
				return "チェックボックス(価格)";
			}
			else
			{
				return "―";
			}
		}

		/// <summary>
		/// <para>商品付帯情報 設定値取得</para>
		/// <para>商品付帯情報の設定値をカンマ区切り文字列で取得</para>
		/// </summary>
		/// <returns>表示区分 C：チェックボックス、S：セレクトメニュー</returns>
		public string GetProductOptionSettingValue()
		{
			if (this.SettingValues.Count == 0)
			{
				return "-";
			}

			StringBuilder sbSettingValues = new StringBuilder();
			foreach (string strSettingValue in this.SettingValues)
			{
				if (sbSettingValues.Length != 0)
				{
					sbSettingValues.Append(",");
				}

				sbSettingValues.Append(strSettingValue);
			}

			return sbSettingValues.ToString();
		}

		/// <summary>
		/// <para>商品付帯情報 </para>
		/// <para>商品付帯情報の設定値をカンマ区切り文字列で取得</para>
		/// </summary>
		/// <returns>表示区分 C：チェックボックス、S：セレクトメニュー</returns>
		public string GetDisplayProductOptionSettingSelectValue()
		{
			// 項目名は空の場合が存在する
			if (StringUtility.ToEmpty(this.SelectedSettingValue) != "")
			{
				var sbSelectedSettingValues = new StringBuilder();
				var strSelectedSettingValues
					= this.SelectedSettingValue.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE, "\n").Split('\n');

				foreach (var strSelectedSettingValue in strSelectedSettingValues)
				{
					if (strSelectedSettingValue == "")
					{
						continue;
					}

					if (sbSelectedSettingValues.Length != 0)
					{
						sbSelectedSettingValues.Append(",");
					}

					sbSelectedSettingValues.Append(strSelectedSettingValue);
				}
				// 項目名が空の場合、入力情報のみ表示する
				return (string.IsNullOrEmpty(this.ValueName) == false)
					? this.ValueName + "：" + sbSelectedSettingValues
					: sbSelectedSettingValues.ToString();
			}
			else if (StringUtility.ToEmpty(this.FixedPurchaseSelectedSettingValue) != "")
			{
				return this.FixedPurchaseSelectedSettingValue;
			}

			return "";
		}

		/// <summary>
		/// 商品付帯情報選択値を取得
		/// </summary>
		/// <param name="includesEmptySelection">選択値が空の付帯情報も含めるか</param>
		/// <returns>商品付帯情報選択値</returns>
		public ProductOptionSelectedValue GetProductOptionSelectedValue(bool includesEmptySelection = false)
		{
			var poSelectedValue = new ProductOptionSelectedValue
			{
				Type = this.DisplayKbn
			};

			if (includesEmptySelection)
			{
				poSelectedValue.Name = StringUtility.ToEmpty(this.ValueName);
			}

			if ((string.IsNullOrEmpty(this.ValueName) == false)
				&& (string.IsNullOrEmpty(this.SelectedSettingValue) == false))
			{
				var strSelectedSettingValues = this.SelectedSettingValue.Replace(
					Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE,
					"\n").Split('\n');
				var selectedValues = strSelectedSettingValues
					.Where(value => (string.IsNullOrEmpty(value) == false))
					.Select(value => value).ToArray();

				poSelectedValue.Name = this.ValueName;
				poSelectedValue.Value = (selectedValues.Length > 0)
					? string.Join(",", selectedValues)
					: string.Empty;
			}
			else if (string.IsNullOrEmpty(this.FixedPurchaseSelectedSettingValue) == false)
			{
				var selectedValue = this.FixedPurchaseSelectedSettingValue.Replace(
					Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE,
					"\n").Split('\n');

				poSelectedValue.Name = selectedValue[0];
				poSelectedValue.Value = (selectedValue.Length > 1)
					? selectedValue[1]
					: string.Empty;
			}

			return poSelectedValue;
		}

		/// <summary>
		/// <para>表示用の商品付帯情報選択値取得</para>
		/// </summary>
		/// <returns>商品付帯情報選択値</returns>
		public string GetDisplayProductOptionSettingSelectedValue()
		{
			if ((this.ValueName != string.Empty) && (this.IsSelectMenu || this.IsDropDownPrice))
			{
				var selectedValue = StringUtility.ToEmpty(this.SelectedSettingValue) != string.Empty
					? this.SelectedSettingValue
					: null;
				return selectedValue;
			}

			return null;
		}

		/// <summary>
		/// 入力チェック用ValidatorXml作成
		/// </summary>
		/// <param name="validatorName">バリデータ名</param>
		/// <returns>バリデータXml</returns>
		public ValidatorXml CreateValidatorXml(string validatorName)
		{
			ValidatorXml textboxValidator = new ValidatorXml();
			textboxValidator.ValidatorName = validatorName;
			XmlNode rootNode = textboxValidator.CreateElement(validatorName);
			textboxValidator.AppendChild(rootNode);

			ValidatorXmlColumn columnValidation = new ValidatorXmlColumn(this.ValueName);
			columnValidation.DisplayName = this.ValueName;
			columnValidation.Necessary = this.Necessary;
			columnValidation.Type = this.Type;
			columnValidation.Length = this.Length;
			columnValidation.LengthMin = this.LengthMin;
			columnValidation.LengthMax = this.LengthMax;
			columnValidation.NumberMin = this.MinValue;
			columnValidation.NumberMax = this.MaxValue;

			textboxValidator.AddColumn(columnValidation);

			textboxValidator.CreateColumnXml();

			return textboxValidator;
		}

		/// <summary>
		/// 有効な選択値か
		/// </summary>
		/// <param name="strValue">商品付帯情報の項目を選択された値</param>
		/// <returns>チェックの結果</returns>
		public bool IsValidSelectedSettingValue(string strValue)
		{
			return ((strValue == "")
				|| SettingValues.Contains(strValue)
				|| IsValidSelectedSettingValueDisplayKbnCheckBox(strValue));
		}

		/// <summary>
		/// 有効な選択値か（表示形式がチェックボックスの場合）
		/// </summary>
		/// <param name="strValue">商品付帯情報の項目を選択された値</param>
		/// <returns>チェックの結果</returns>
		private bool IsValidSelectedSettingValueDisplayKbnCheckBox(string strValue)
		{
			var values = strValue.Split(new[] { Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE }, StringSplitOptions.RemoveEmptyEntries);
			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
			{
				values = strValue.Split(',');
			}

			var result = values.All(value => this.SettingValues.Contains(value));
			return result;
		}

		/// <summary>
		/// 適当なテキスト値か（表示式がテキストボックスの場合）
		/// </summary>
		/// <param name="value">商品付帯情報の項目に入力された値</param>
		/// <returns>チェックの結果: エラーメッセージ</returns>
		/// <remarks>検証用XMLを生成しチェックする</remarks>
		public string CheckValidText(string value)
		{
			string checkKbn = "OptionValueValidate";

			Hashtable param = new Hashtable();
			param[this.ValueName] = value;
			return (w2.App.Common.Util.Validator.ChangeToValidate(Validator.Validate(checkKbn, this.CreateValidatorXml(checkKbn).InnerXml, param)));
		}

		/// <summary>
		/// オプション価格を取得
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>付帯情報一つの価格</returns>
		public decimal GetOptionSinglePrice(string key)
		{
			if (string.IsNullOrEmpty(key)) return 0m;
			var result = this.OptionPriceDictionary.ContainsKey(key) ? this.OptionPriceDictionary[key] : 0m;
			return result;
		}

		/// <summary>
		/// オプション価格合計取得
		/// </summary>
		/// <returns>オプション価格</returns>
		public decimal GetPriceOfSelectedValue()
		{
			if (HasOptionPrice() == false) return 0m;
			var keys = this.SelectedSettingValue
				.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE, "\n")
				.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var result = keys.Sum(key => this.OptionPriceDictionary.ContainsKey(key)
				? this.OptionPriceDictionary[key]
				: 0);
			return result;
		}

		/// <summary>
		/// オプション価格持っているか
		/// </summary>
		/// <returns>TRUE：オプション価格あり；　FALSE：なし</returns>
		public bool HasOptionPrice()
		{
			if ((Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
				|| string.IsNullOrEmpty(this.SelectedSettingValue))
			{
				return false;
			}

			var keys = this.SelectedSettingValue
				.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE, "\n")
				.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var result = keys.Any(key => Regex.IsMatch(key, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION));
			if (result == false)
			{
				result = keys.Any(key => Regex.IsMatch(key, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW));
			}
			return result;
		}

		/// <summary>
		/// 商品付帯情報オブジェクト複製
		/// </summary>
		/// <returns>複製した商品付帯情報オブジェクト</returns>
		public ProductOptionSetting Clone()
		{
			var clone = (ProductOptionSetting)MemberwiseClone();
			return clone;
		}

		/// <summary>表示区分</summary>
		public string DisplayKbn { get; set; }
		/// <summary>項目名</summary>
		public string ValueName { get; set; }
		/// <summary>設定値一覧</summary>
		public List<string> SettingValues { get; set; }
		/// <summary>設定値一覧（リストアイテムコレクション）</summary>
		public ListItemCollection SettingValuesListItemCollection
		{
			get
			{
				ListItemCollection lic = new ListItemCollection();
				if (this.SettingValues != null)
				{
					foreach (string SettingValue in this.SettingValues)
					{
						ListItem li = new ListItem(SettingValue, SettingValue);

						if ((this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX)
							|| (this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX))
						{
							li.Selected = StringUtility.ToEmpty(SelectedSettingValue)
								.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE, "\n")
								.Split('\n')
								.Contains(li.Value);
						}

						lic.Add(li);
					}
				}

				return lic;
			}
		}
		/// <summary>初期値</summary>
		public string DefaultValue { get; set; }
		/// <summary>入力必須チェック</summary>
		public string Necessary { get; set; }
		/// <summary>入力タイプ</summary>
		public string Type { get; set; }
		/// <summary>固定文字列長</summary>
		public string Length { get; set; }
		/// <summary>最低文字列長</summary>
		public string LengthMin { get; set; }
		/// <summary>最大文字列長</summary>
		public string LengthMax { get; set; }
		/// <summary>最低値</summary>
		public string MinValue { get; set; }
		/// <summary>最大値</summary>
		public string MaxValue { get; set; }
		/// <summary>選択された設定値</summary>
		public string SelectedSettingValue { get; set; }
		/// <summary>定期購入時に選択された設定値</summary>
		public string FixedPurchaseSelectedSettingValue { get; set; }
		/// <summary> 選択された設定値（TextBox） </summary>
		public string SelectedSettingValueForTextBox
		{
			get
			{
				return (string.IsNullOrEmpty(this.SelectedSettingValue) == false)
					? this.SelectedSettingValue
					: this.DefaultValue;
			}
		}
		/// <summary>テキストボックスか</summary>
		public bool IsTextBox
		{
			get { return this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX; }
		}
		/// <summary>セレクトメニューか</summary>
		public bool IsSelectMenu
		{
			get { return this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU; }
		}
		/// <summary>チェックボックスか</summary>
		public bool IsCheckBox
		{
			get { return this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX; }
		}
		/// <summary>セレクトメニューか</summary>
		public bool IsDropDownPrice
		{
			get { return this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU; }
		}
		/// <summary>チェックボックスか</summary>
		public bool IsCheckBoxPrice
		{
			get { return this.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX; }
		}
		/// <summary>必要である</summary>
		public bool IsNecessary
		{
			get { return Necessary == "1"; }
		}
		/// <summary>商品付帯情報が選択されたか</summary>
		public bool IsSelectedProductOptionValue
		{
			get { return (StringUtility.ToEmpty(SelectedSettingValue) != ""); }
		}
		/// <summary>オプション価格の表示テキストと価格</summary>
		public Dictionary<string, decimal> OptionPriceDictionary { get; set; }
		/// <summary>オプション価格の設定値一覧</summary>
		public List<string> OptionPriceSettingValues { get; set; }
		/// <summary>最新の商品マスタと一致しているか</summary>
		public bool MatchesLatestProductMaster { get; set; }
	}
}
