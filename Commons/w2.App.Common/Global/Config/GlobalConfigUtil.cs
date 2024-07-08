/*
=========================================================================================================
  Module      : グローバル設定 ユーティリティクラス(GlobalConfigUtill.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using System.Linq;
using w2.App.Common.Util;

namespace w2.App.Common.Global.Config
{
	/// <summary>
	/// グローバル設定 ユーティリティクラス
	/// </summary>
	public class GlobalConfigUtil
	{
		/// <summary>
		/// 国ISOコードの対応状況確認
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>対応:true 非対応:false</returns>
		public static bool CheckCountryPossible(string countryIsoCode)
		{
			if (string.IsNullOrEmpty(countryIsoCode)) return false;

			var result = Constants.GLOBAL_CONFIGS.GlobalSettings.CountryIsoCodes.Any(cic => cic == countryIsoCode);
			return result;
		}

		/// <summary>
		/// 言語コードの対応状況確認
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <returns>対応:true 非対応:false</returns>
		public static bool CheckLanguagePossible(string languageCode)
		{
			if (string.IsNullOrEmpty(languageCode)) return false;

			var result = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Any(lc => lc.Code == languageCode);
			return result;
		}

		/// <summary>
		/// 通貨コードの対応状況確認
		/// </summary>
		/// <param name="currencyCode">通貨コード</param>
		/// <returns>対応:true 非対応:false</returns>
		public static bool CheckCurrencyPossible(string currencyCode)
		{
			if (string.IsNullOrEmpty(currencyCode)) return false;

			var result = Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.Any(cc => cc.Code == currencyCode);
			return result;
		}

		/// <summary>
		/// 言語ロケールIDの対応状況確認
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>対応:true 非対応:false</returns>
		public static bool CheckLanguageLocaleIdPossible(string languageCode, string languageLocaleId)
		{
			if (string.IsNullOrEmpty(languageCode) || string.IsNullOrEmpty(languageLocaleId)) return false;

			var result = (GetLanguageLocaleId(languageCode) == languageLocaleId);
			return result;
		}

		/// <summary>
		/// 通貨ロケールIDの対応状況確認
		/// </summary>
		/// <param name="currencyCode">通貨コード</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <returns>対応:true 非対応:false</returns>
		public static bool CheckCurrencyLocaleIdPossible(string currencyCode, string currencyLocaleId)
		{
			if (string.IsNullOrEmpty(currencyCode) || string.IsNullOrEmpty(currencyLocaleId)) return false;

			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.FirstOrDefault(cc => cc.Code == currencyCode);
			if (model == null) return false;

			var result = model.CurrencyLocales.Any(cl => cl.LocaleId == currencyLocaleId);
			return result;
		}

		/// <summary>
		/// 通貨ロケールIDの書式フォーマットを取得
		/// </summary>
		/// <param name="currencyLocalId">通貨ロケールID</param>
		/// <returns>通貨ロケールIDの書式フォーマット</returns>
		public static string GetCurrencyLocaleFormat(string currencyLocalId)
		{
			var format = Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies
				.SelectMany(cl => cl.CurrencyLocales)
				.FirstOrDefault(cli => cli.LocaleId == currencyLocalId);
			if (format == null || String.IsNullOrEmpty(format.LocaleFormat)) return "{0:C}";

			return format.LocaleFormat;
		}

		/// <summary>
		/// 通貨ロケールIDを画面表示時のフォーマットに変更
		/// </summary>
		/// <param name="localeId">通貨ロケールID</param>
		/// <returns>通貨ロケールID ＋ サンプル通貨記号</returns>
		public static string CurrencyLocaleIdDisplayFormat(string localeId)
		{
			if (string.IsNullOrEmpty(localeId)) return string.Empty;

			var result = string.Format("{0} ({1})", localeId, CurrencySymbol(localeId));
			return result;
		}

		/// <summary>
		/// 通貨ロケールIDから通貨コードを取得
		/// </summary>
		/// <param name="localeId">通貨ロケールID</param>
		/// <returns>通貨コード</returns>
		public static Currencies GetCurrencyByLocaleId(string localeId)
		{
			var result = Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.FirstOrDefault(c => c.CurrencyLocales.Any(cl => cl.LocaleId == localeId));
			return result;
		}

		/// <summary>
		/// 通貨コードから通貨ロケールIDを取得
		/// </summary>
		/// <param name="currency">通貨コード</param>
		/// <returns>通貨ロケールID</returns>
		public static Currencies GetLocalIdByCurrency(string currency)
		{
			var result = Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.FirstOrDefault(c => c.Code == currency);
			return result;
		}

		/// <summary>
		/// 通貨ロケールIDより通貨記号を取得
		/// </summary>
		/// <param name="localeId">通貨ロケールID</param>
		/// <returns>通貨記号</returns>
		public static string CurrencySymbol(string localeId)
		{
			var result = new CultureInfo(localeId).NumberFormat.CurrencySymbol;
			return result;
		}


		/// <summary>
		/// 言語コードから言語ロケールIDを取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <returns>言語ロケールID</returns>
		public static string GetLanguageLocaleId(string languageCode)
		{
			var languageLocaleId = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.FirstOrDefault(l => l.Code == languageCode);
			if (languageLocaleId == null) return string.Empty;
			return languageLocaleId.LocaleId;
		}

		/// <summary>
		/// 言語ロケールIDから言語コードを取得
		/// </summary>
		/// <param name="localeId"></param>
		/// <returns></returns>
		public static Language GetLanguageByLocaleId(string localeId)
		{
			var result = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.FirstOrDefault(l => l.LocaleId == localeId);
			return result;
		}


		/// <summary>
		/// 言語ロケールIDを画面表示のフォーマットに変更
		/// </summary>
		/// <param name="localeId">言語ロケールID</param>
		/// <returns>言語ロケールID ＋ サンプル日付</returns>
		public static string LanguageLocaleIdDisplayFormat(string localeId)
		{
			if (string.IsNullOrEmpty(localeId)) return string.Empty;

			var result = string.Format("{0} ({1})", localeId, DateTimeFormat(localeId));
			return result;
		}

		/// <summary>
		/// 言語ロケールIDより日付をテキストフォーマットに変換
		/// </summary>
		/// <param name="localeId">言語ロケールID</param>
		/// <param name="format">日付フォーマット</param>
		/// <param name="dateTime">日付インスタンス(nullの場合は2000:01:01)</param>
		/// <returns>変換後の日付</returns>
		public static string DateTimeFormat(string localeId, string format = "F", DateTime? dateTime = null)
		{
			var dt = (dateTime == null) ? new DateTime(2000, 1, 1, 1, 1, 1) : dateTime.Value;
			var result = dt.ToString(format, new CultureInfo(localeId));
			return result;
		}

		/// <summary>
		/// フロント言語切り替えの選択肢を言語コードから取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <returns>選択肢</returns>
		public static FrontLanguages GetFrontLanguage(string languageCode)
		{
			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontLanguages
				.FirstOrDefault(fl => fl.Code == languageCode);
			return model;
		}

		/// <summary>
		/// フロント通貨切り替えの選択肢を通貨コードから取得
		/// </summary>
		/// <param name="currencyCode">通貨コード</param>
		/// <returns>選択肢</returns>
		public static FrontCurrencies GetFrontCurrency(string currencyCode)
		{
			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontCurrencies
				.FirstOrDefault(fc => fc.Code == currencyCode);
			return model;
		}

		/// <summary>
		/// フロント言語＋通貨切り替えの選択肢を言語コードと通貨コードの組み合わせより取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="currencyCode">通貨コード</param>
		/// <returns>選択肢</returns>
		public static FrontLanguageCurrencies GetFrontLanguageCurrency(string languageCode, string currencyCode)
		{
			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontLanguageCurrencies
				.FirstOrDefault(flc => (flc.LanguageCode == languageCode && flc.CurrencyCode == currencyCode));
			return model;
		}

		/// <summary>
		/// フロント言語＋通貨切り替えの選択肢を通貨コードの組み合わせより取得
		/// </summary>
		/// <param name="currencyCode">通貨コード</param>
		/// <returns>選択肢</returns>
		public static FrontLanguageCurrencies GetFrontLanguageCurrency(string currencyCode)
		{
			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontLanguageCurrencies
				.FirstOrDefault(flc => (flc.CurrencyCode == currencyCode));
			return model;
		}

		/// <summary>
		/// 日付フォーマット配列取得
		/// </summary>
		/// <param name="localeId">ロケールID</param>
		/// <param name="formatType">日付形式区分</param>
		/// <returns>日付フォーマット配列</returns>
		public static string GetDateTimeFormatText(string localeId, DateTimeUtility.FormatType formatType)
		{
			// 日付形式IDへの変換
			var formatId = DateTimeUtility.ConvertFormatType(formatType);
			// フォーマットIDを基にして、設定情報を取得
			var formatModel = Constants.GLOBAL_CONFIGS.GlobalSettings.DateTimeFormats
				.Where(dt => dt.FormatId == formatId)
				.SelectMany(dt => dt.FormatDetails).ToArray();
			// 設定情報がなければ、空を戻す
			if (formatModel.Length == 0) return string.Empty;

			// 言語ロケールIDを基にして、フォーマット配列を取得
			// ※該当ロケールIDがなければ、1番目の設定値を取得
			var result = formatModel.Any(f => f.LocaleId == localeId)
				&& Constants.GLOBAL_OPTION_ENABLE
				? formatModel.First(f => f.LocaleId == localeId).FormatText
				: formatModel.First().FormatText;
			return result;
		}

		/// <summary>
		/// 翻訳Api情報を取得
		/// </summary>
		/// <returns>翻訳Api情報</returns>
		public static Translation GetTranslation()
		{
			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.Translation;
			return model;
		}

		/// <summary>
		/// Google接続情報を取得
		/// </summary>
		/// <returns>Google接続情報</returns>
		public static GoogleConnection GetGoogleConnection()
		{
			var model = Constants.GLOBAL_CONFIGS.GlobalSettings.GoogleConnection;
			return model;
		}

		/// <summary>
		/// 翻訳APIの使用可否
		/// </summary>
		/// <returns>翻訳APIの使用可否</returns>
		public static bool GlobalTranslationEnabled()
		{
			var enabled = (Constants.GLOBAL_OPTION_ENABLE) && (string.IsNullOrEmpty(GetTranslation().TranslationApiKbn) == false);
			return enabled;
		}

		/// <summary>
		/// リードタイム設定利用可否
		/// </summary>
		/// <returns>利用可否</returns>
		public static bool UseLeadTime()
		{
			// 出荷予定日オプションがONで運用地が日本の場合
			var result = (Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE
				&& ((Constants.GLOBAL_OPTION_ENABLE == false)
					|| (Constants.OPERATIONAL_BASE_ISO_CODE == Constants.COUNTRY_ISO_CODE_JP)
					|| Constants.TW_COUNTRY_SHIPPING_ENABLE));
			return result;
		}
	}
}
