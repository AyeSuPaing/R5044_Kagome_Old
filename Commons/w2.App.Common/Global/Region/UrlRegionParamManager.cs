/*
=========================================================================================================
  Module      : リージョンURLパラメータ生成クラス(UrlRegionParamManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web;
using w2.App.Common.Global.Config;

namespace w2.App.Common.Global.Region
{
	/// <summary>
	/// リージョンURLパラメータ生成クラス
	/// </summary>
	public class UrlRegionParamManager
	{
		/// <summary>
		/// URLパラメータよりリージョンを更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		public static void ChangeRegionByUrlParams(string userId)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return;

			var languageCode = HttpContext.Current.Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE];
			var currencyCode = HttpContext.Current.Request[Constants.REQUEST_KEY_GLOBAL_CURRENCY_CODE];
			ChangeRegion(userId, languageCode, currencyCode);
		}

		/// <summary>
		/// リージョン変更用のURL作成
		/// </summary>
		/// <param name="languageCode">変更する言語コード</param>
		/// <param name="currencyCode">変更する通貨コード</param>
		/// <returns>作成後のURL</returns>
		public static string CreateUrl(string languageCode = "", string currencyCode = "")
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return string.Empty;
			
			// 多言語のサイトの切替設定がある場合指定したURLを返す
			var languageModel = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.FirstOrDefault(l => l.Code == languageCode);
			if ((languageModel != null)
				&& (string.IsNullOrEmpty(languageModel.DefaultUrl) == false)) return languageModel.DefaultUrl;

			var resultUrl = "";
			var queryString = "";

			var url = HttpContext.Current.Request.RawUrl;
			var urlArray = url.Split('?');
			if (urlArray.Length == 1)
			{
				resultUrl = url;
			}
			else
			{
				resultUrl = urlArray[0];
				queryString = urlArray[1];
			}

			var urlQuery = HttpUtility.ParseQueryString(queryString);

			if (string.IsNullOrEmpty(languageCode) == false)
			{
				urlQuery[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE] = languageCode;
			}
			else
			{
				urlQuery.Remove(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE);
			}

			if (string.IsNullOrEmpty(currencyCode) == false)
			{
				urlQuery[Constants.REQUEST_KEY_GLOBAL_CURRENCY_CODE] = currencyCode;
			}
			else
			{
				urlQuery.Remove(Constants.REQUEST_KEY_GLOBAL_CURRENCY_CODE);
			}

			if ((string.IsNullOrEmpty(languageCode) == false) || (string.IsNullOrEmpty(currencyCode) == false))
			{
				resultUrl += "?" + urlQuery.ToString();
			}
			return resultUrl;
		}

		/// <summary>
		/// リージョンを更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="languageCode">変更後の言語コード</param>
		/// <param name="currencyCode">変更後の通貨コード</param>
		private static void ChangeRegion(string userId, string languageCode = "", string currencyCode = "")
		{
			var model = new RegionModel();
			var coockieRegion = RegionCookieManager.GetValue();
			var languageLocaleId = (coockieRegion != null) ? coockieRegion.LanguageLocaleId : string.Empty;
			if ((string.IsNullOrEmpty(languageCode) == false) && (string.IsNullOrEmpty(currencyCode) == false))
			{
				var frontLanguageCurrency = GlobalConfigUtil.GetFrontLanguageCurrency(languageCode, currencyCode);
				if (frontLanguageCurrency != null)
				{
					model.LanguageCode = frontLanguageCurrency.LanguageCode;
					model.LanguageLocaleId = GlobalConfigUtil.GetLanguageLocaleId(frontLanguageCurrency.LanguageCode);
					model.CurrencyCode = frontLanguageCurrency.CurrencyCode;
					model.CurrencyLocaleId = frontLanguageCurrency.CurrencyLocaleId;
					languageLocaleId = model.LanguageLocaleId;
				}
			}
			else if (string.IsNullOrEmpty(languageCode) == false)
			{
				var frontLanguages = GlobalConfigUtil.GetFrontLanguage(languageCode);
				if (frontLanguages != null)
				{
					model.LanguageCode = frontLanguages.Code;
					model.LanguageLocaleId = GlobalConfigUtil.GetLanguageLocaleId(frontLanguages.Code);
					model.CurrencyCode = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;
					model.CurrencyLocaleId = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId;
					languageLocaleId = model.LanguageLocaleId;
				}
			}
			else if (string.IsNullOrEmpty(currencyCode) == false)
			{
				var frontCurrencies = GlobalConfigUtil.GetFrontCurrency(currencyCode);
				if (frontCurrencies != null)
				{
					model.CurrencyCode = frontCurrencies.Code;
					model.CurrencyLocaleId = frontCurrencies.LocaleId;
				}
			}

			if ((string.IsNullOrEmpty(languageCode) == false) || (string.IsNullOrEmpty(currencyCode) == false)) RegionManager.GetInstance().UpdateUserRegion(model, userId);
		}
	}
}
