/*
=========================================================================================================
  Module      : グローバル対応 設定クラス(GlobalConfigs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Global.Config
{
	/// <summary>
	/// グローバル対応 設定クラス
	/// </summary>
	public class GlobalConfigs
	{
		/// <summary>定数: グローバル設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = "Global";
		/// <summary>定数: プロジェクト用グローバル設定ファイル名</summary>
		private const string PROJECT_SETTING_FILE_NAME = "ProjectGlobalSetting.xml";


		/// <summary>定数: グローバル設定Baseディレクトリ名</summary>
		private const string SETTING_FILE_BASE_DIR = @"Global\base";
		/// <summary>定数: グローバル設定ファイル名</summary>
		private const string SETTING_FILE_NAME = "GlobalSetting.xml";

		/// <summary>インスタンス</summary>
		private static GlobalConfigs m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GlobalConfigs()
		{
			Update();
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR), SETTING_FILE_NAME, Update);
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR), PROJECT_SETTING_FILE_NAME, Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static GlobalConfigs GetInstance()
		{
			if (m_singletonInstance == null) m_singletonInstance = new GlobalConfigs();
			return m_singletonInstance;
		}

		/// <summary>
		/// 最新の設定データに更新
		/// </summary>
		private void Update()
		{
			if (File.Exists(this.SettingFileFullPath) == false) return;

			try
			{
				using (var fs = File.OpenRead(this.SettingFileFullPath))
				{
					this.GlobalSettings = (GlobalSettings)new XmlSerializer(typeof(GlobalSettings)).Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("グローバル設定「" + (this.SettingFileFullPath) + "」の読み込みに失敗しました。", ex);
			}

			UpdateProjectConf();
		}

		/// <summary>
		/// プロジェクト別の設定更新
		/// </summary>
		private void UpdateProjectConf()
		{
			if (File.Exists(this.SettingProjectFileFullPath) == false) return;

			// 上書きする元がなければ何もしない
			if (this.GlobalSettings == null) return;

			try
			{
				using (var fs = File.OpenRead(this.SettingProjectFileFullPath))
				{
					var projectSetting = (GlobalSettings)new XmlSerializer(typeof(GlobalSettings)).Deserialize(fs);

					// 上書き
					foreach (var prop in projectSetting.GetType().GetProperties()
						.Where(p => Attribute.GetCustomAttribute(p, typeof(XmlArrayItemAttribute)) != null))
					{
						// コレクション要素で要素数が1つ以上あるもののみ、すなわち要素が存在する場合だけを上書き
						if (prop.GetValue(projectSetting) is ICollection && ((ICollection)prop.GetValue(projectSetting)).Count > 0)
						{
							this.GlobalSettings.GetType().GetProperty(prop.Name).SetValue(this.GlobalSettings, prop.GetValue(projectSetting));
						}
					}

					foreach (var prop in projectSetting.GetType().GetProperties()
						.Where(p => Attribute.GetCustomAttribute(p, typeof(XmlElementAttribute)) != null))
					{
						// Null以外、すなわち要素が存在する場合だけ上書き
						if (prop.GetValue(projectSetting) != null)
						{
							this.GlobalSettings.GetType().GetProperty(prop.Name).SetValue(this.GlobalSettings, prop.GetValue(projectSetting));
						}
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("グローバル設定「" + (this.SettingProjectFileFullPath) + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>グローバル対応 設定</summary>
		public GlobalSettings GlobalSettings { get; set; }
		/// <summary>グローバル設定ファイル フルパス</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR, SETTING_FILE_NAME); }
		}
		/// <summary>プロジェクト毎のグローバル設定ファイルのパス </summary>
		private string SettingProjectFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR, PROJECT_SETTING_FILE_NAME); }
		}
	}

	/// <summary>
	/// 対応範囲
	/// </summary>
	[XmlRoot("GlobalSettings")]
	public class GlobalSettings
	{
		/// <summary>国ISOコード対応範囲</summary>
		[XmlArray("Countries")]
		[XmlArrayItem("Code")]
		public List<string> CountryIsoCodes { get; set; }
		/// <summary>言語対応範囲</summary>
		[XmlArray("Languages")]
		[XmlArrayItem("Language")]
		public List<Language> Languages { get; set; }
		/// <summary>通貨対応範囲</summary>
		[XmlArray("Currencies")]
		[XmlArrayItem("Currency")]
		public List<Currencies> Currencies { get; set; }
		/// <summary>基軸通貨</summary>
		[XmlElement("KeyCurrency")]
		public KeyCurrency KeyCurrency { get; set; }
		/// <summary>IP振り分け対応範囲</summary>
		[XmlArray("Regions")]
		[XmlArrayItem("Region")]
		public List<Regions> Regions { get; set; }
		/// <summary>IP範囲対応外の場合のデフォルト国ISOコード</summary>
		[XmlElement("NotFoundCountryIsoCode")]
		public string NotFoundCountryIsoCode { get; set; }
		/// <summary>言語切り替え設定</summary>
		[XmlArray("FrontLanguages")]
		[XmlArrayItem("FrontLanguage")]
		public List<FrontLanguages> FrontLanguages { get; set; }
		/// <summary>通貨切り替え設定</summary>
		[XmlArray("FrontCurrencies")]
		[XmlArrayItem("FrontCurrency")]
		public List<FrontCurrencies> FrontCurrencies { get; set; }
		/// <summary>言語と通貨切り替え設定</summary>
		[XmlArray("FrontLanguageCurrencies")]
		[XmlArrayItem("FrontLanguageCurrency")]
		public List<FrontLanguageCurrencies> FrontLanguageCurrencies { get; set; }
		/// <summary>切り替え方式</summary>
		[XmlElement("FrontChangeType")]
		public string FrontChangeType { get; set; }
		/// <summary>日付切替対応範囲</summary>
		[XmlArray("DateTimeFormats")]
		[XmlArrayItem("DateTimeFormat")]
		public List<DateTimeFormats> DateTimeFormats { get; set; }
		/// <summary>購買区分レポート項目</summary>
		[XmlArray("OrderPriceAnalysis")]
		[XmlArrayItem("Section")]
		public List<OrderPriceAnalysis> OrderPriceAnalysis { get; set; }
		/// <summary>Google接続設定</summary>
		[XmlElement("GoogleConnection")]
		public GoogleConnection GoogleConnection { get; set; }
		/// <summary>翻訳設定</summary>
		[XmlElement("Translation")]
		public Translation Translation { get; set; }
		/// <summary>電話番号国コード</summary>
		[XmlArray("InternationalTelephoneCode")]
		[XmlArrayItem("Code")]
		public List<InternationalTelephoneCode> InternationalTelephoneCode { get; set; }
		/// <summary>定期区分レポート項目</summary>
		[XmlArray("FixedPurchasePriceAnalysis")]
		[XmlArrayItem("Section")]
		public List<FixedPurchasePriceAnalysis> FixedPurchasePriceAnalysis { get; set; }
	}

	/// <summary>
	/// 言語対応範囲
	/// </summary>
	[Serializable]
	public class Language
	{
		/// <summary>言語コード</summary>
		[XmlAttribute("Code")]
		public string Code { get; set; }
		/// <summary>言語ロケールID</summary>
		[XmlAttribute("LocaleId")]
		public string LocaleId { get; set; }
		/// <summary>デフォルトURL</summary>
		[XmlAttribute("DefaultUrl")]
		public string DefaultUrl { get; set; }
	}

	/// <summary>
	/// 通貨対応範囲
	/// </summary>
	[Serializable]
	public class Currencies
	{
		/// <summary>通貨コード</summary>
		[XmlAttribute("Code")]
		public string Code { get; set; }
		/// <summary>通貨ロケール</summary>
		[XmlArray("CurrencyLocales")]
		[XmlArrayItem("CurrencyLocale")]
		public List<CurrencyLocale> CurrencyLocales { get; set; }
	}

	/// <summary>
	/// 通貨ロケール
	/// </summary>
	[Serializable]
	public class CurrencyLocale
	{
		/// <summary>通貨ロケールID</summary>
		[XmlAttribute("LocaleId")]
		public string LocaleId { get; set; }
		/// <summary>通貨ロケール書式</summary>
		[XmlAttribute("LocaleFormat")]
		public string LocaleFormat { get; set; }
		/// <summary>通貨ロケール書式（日本特別対応）</summary>
		[XmlAttribute("LocaleFormatJapanese")]
		public string LocaleFormatJapanese { get; set; }
		/// <summary>通貨ロケール書式(通貨記号なし)</summary>
		[XmlAttribute("LocaleFormatWithoutSymbol")]
		public string LocaleFormatWithoutSymbol { get; set; }
	}

	/// <summary>
	/// 基軸通貨
	/// </summary>
	[Serializable]
	public class KeyCurrency
	{
		/// <summary>通貨コード</summary>
		[XmlAttribute("Code")]
		public string Code { get; set; }
		/// <summary>通貨ロケールID</summary>
		[XmlAttribute("LocaleId")]
		public string LocaleId { get; set; }
		/// <summary>通貨ロケール書式</summary>
		[XmlAttribute("LocaleFormat")]
		public string LocaleFormat { get; set; }
		/// <summary>PDF用通貨ロケール書式</summary>
		[XmlAttribute("PdfFormat")]
		public string PdfFormat { get; set; }
		/// <summary>補助単位 小数点以下の有効桁数</summary>
		[XmlIgnore]
		public int? CurrencyDecimalDigits
		{
			get
			{
				var digits = 0;
				return ((string.IsNullOrEmpty(this.Digits) == false) && int.TryParse(this.Digits, out digits))
					? (int?)digits
					: null;
			}
		}
		/// <summary>補助単位 小数点以下の有効桁数 取得用</summary>
		[XmlAttribute("CurrencyDecimalDigits")]
		[DefaultValue("")]
		public string Digits { get; set; }
		/// <summary>基軸通貨単位（基本的に管理画面で利用するため管理画面の言語で記載）</summary>
		[XmlAttribute("UnitString")]
		[DefaultValue("")]
		public string UnitString { get; set; }
	}

	/// <summary>
	/// IP割り振り設定
	/// </summary>
	[Serializable]
	public class Regions
	{
		/// <summary>国ISOコード</summary>
		[XmlAttribute("CountryIsoCode")]
		public string CountryIsoCode { get; set; }
		/// <summary>言語コード</summary>
		[XmlAttribute("LanguageCode")]
		public string LanguageCode { get; set; }
		/// <summary>言語ロケールID</summary>
		[XmlAttribute("LanguageLocaleId")]
		public string LanguageLocaleId { get; set; }
		/// <summary>通貨コード</summary>
		[XmlAttribute("CurrencyCode")]
		public string CurrencyCode { get; set; }
		/// <summary>通貨ロケールID</summary>
		[XmlAttribute("CurrencyLocaleId")]
		public string CurrencyLocaleId { get; set; }
	}

	/// <summary>
	/// 言語切り替え設定
	/// </summary>
	[Serializable]
	public class FrontLanguages
	{
		/// <summary>言語コード</summary>
		[XmlAttribute("Code")]
		public string Code { get; set; }
		/// <summary>選択肢名</summary>
		[XmlAttribute("SelectName")]
		public string SelectName { get; set; }
	}

	/// <summary>
	/// 通貨切り替え設定
	/// </summary>
	[Serializable]
	public class FrontCurrencies
	{
		/// <summary>通貨コード</summary>
		[XmlAttribute("Code")]
		public string Code { get; set; }
		/// <summary>通貨ロケールID</summary>
		[XmlAttribute("LocaleId")]
		public string LocaleId { get; set; }
		/// <summary>選択肢名</summary>
		[XmlAttribute("SelectName")]
		public string SelectName { get; set; }
	}

	/// <summary>
	/// 言語と通貨切り替え設定
	/// </summary>
	[Serializable]
	public class FrontLanguageCurrencies
	{
		/// <summary>言語コード</summary>
		[XmlAttribute("LanguageCode")]
		public string LanguageCode { get; set; }
		/// <summary>通貨コード</summary>
		[XmlAttribute("CurrencyCode")]
		public string CurrencyCode { get; set; }
		/// <summary>通貨ロケールID</summary>
		[XmlAttribute("CurrencyLocaleId")]
		public string CurrencyLocaleId { get; set; }
		/// <summary>選択肢名</summary>
		[XmlAttribute("SelectName")]
		public string SelectName { get; set; }
	}

	/// <summary>
	/// 日付フォーマット設定
	/// </summary>
	[Serializable]
	public class DateTimeFormats
	{
		/// <summary> フォーマットID </summary>
		[XmlAttribute("FormatId")]
		public string FormatId { get; set; }
		/// <summary> フォーマット詳細 </summary>
		[XmlElement("Format")]
		public List<DateTimeFormat> FormatDetails { get; set; }
	}

	/// <summary>
	/// 日付フォーマット
	/// </summary>
	[Serializable]
	public class DateTimeFormat
	{
		/// <summary> ロケールID </summary>
		[XmlAttribute("LanguageLocaleId")]
		public string LocaleId { get; set; }
		/// <summary> フォーマット配列 </summary>
		[XmlText()]
		public string FormatText { get; set; }
	}

	/// <summary>
	/// 購買区分レポートの項目条件
	/// </summary>
	[Serializable]
	public class OrderPriceAnalysis
	{
		/// <summary>項目名</summary>
		[XmlAttribute("Name")]
		public string Name { get; set; }
		/// <summary>From地点</summary>
		[XmlAttribute("From")]
		public string From { get; set; }
		/// <summary>To地点</summary>
		[XmlAttribute("To")]
		public string To { get; set; }
	}

	/// <summary>
	/// 定期区分レポートの項目条件
	/// </summary>
	[Serializable]
	public class FixedPurchasePriceAnalysis
	{
		/// <summary>項目名</summary>
		[XmlAttribute("Name")]
		public string Name { get; set; }
		/// <summary>From</summary>
		[XmlAttribute("From")]
		public string From { get; set; }
		/// <summary>To</summary>
		[XmlAttribute("To")]
		public string To { get; set; }
	}

	/// <summary>
	/// Google設定
	/// </summary>
	[Serializable]
	public class GoogleConnection
	{
		/// <summary>GoogleApiキー</summary>
		[XmlAttribute("GoogleApiKey")]
		public string GoogleApiKey { get; set; }
		/// <summary>翻訳URL</summary>
		[XmlAttribute("TranslationUrl")]
		public string TranslationUrl { get; set; }
	}

	/// <summary>
	/// 翻訳設定
	/// </summary>
	[Serializable]
	public class Translation
	{
		/// <summary>翻訳削除間隔(日単位)</summary>
		[XmlAttribute("TranslationDeletingIntervalDay")]
		public int TranslationDeletingIntervalDay { get; set; }
		/// <summary>翻訳Api区分</summary>
		[XmlAttribute("TranslationApiKbn")]
		public string TranslationApiKbn { get; set; }
	}

	/// <summary>
	/// 電話番号国コード
	/// </summary>
	[Serializable]
	public class InternationalTelephoneCode
	{
		/// <summary>isoコード</summary>
		[XmlAttribute("iso")]
		public string Iso { get; set; }
		/// <summary>番号</summary>
		[XmlAttribute("number")]
		public string Number { get; set; }
	}
}
