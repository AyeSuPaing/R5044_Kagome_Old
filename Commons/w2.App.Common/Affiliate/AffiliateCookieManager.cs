/*
=========================================================================================================
  Module      : アフィリエイトの成果報告用パラメーター管理クラス(AffiliateCookieManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Affiliate
{

	/// <summary>
	/// アフィリエイトの成果報告用パラメーター管理クラス
	/// </summary>
	public class AffiliateCookieManager
	{
		/// <summary> 設定ファイルディレクトリ </summary>
		private const string DIRNAME_AFFILIATE_PARAMETER_SETTING = "Settings";
		/// <summary> 設定ファイル名 </summary>
		private const string FILENAME_AFFILIATE_PARAMETER_SETTING = "AffiliateParameterSetting.xml";
		/// <summary> 対応パラメータリスト </summary>
		private static Dictionary<string, int> m_validParameters;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static AffiliateCookieManager()
		{
			UpdateValidParameters();
			FileUpdateObserver.GetInstance().AddObservation(
				Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_AFFILIATE_PARAMETER_SETTING),
				FILENAME_AFFILIATE_PARAMETER_SETTING,
				UpdateValidParameters);
		}

		/// <summary>
		/// 有効なCookieの値を取得
		/// </summary>
		/// <param name="name">Cookie名</param>
		/// <returns>Cookie値</returns>
		public static string GetCookieValue(string name)
		{
			if (string.IsNullOrEmpty(name)) return string.Empty;

			var cookie = CookieManager.Get(name);
			return (cookie != null) ? cookie.Value : string.Empty;
		}

		/// <summary>
		/// 広告パラメータをCookieに保存する
		/// </summary>
		/// <param name="name">Cookie名</param>
		/// <param name="value">Cookie値</param>
		public static void CreateCookie(string name, string value)
		{
			// Cookieに広告パラメータを保存
			CookieManager.SetCookie(name, value, w2.Common.Constants.PATH_ROOT, DateTime.Now.AddDays(m_validParameters[name]));
		}

		/// <summary>
		/// クエリストリング内の一番最初の有効な広告パラメータの名前を取得
		/// </summary>
		/// <param name="request">HTTPリクエスト</param>
		/// <returns>広告パラメータ名</returns>
		public static string GetFirstValidAdvParamName(HttpRequest request)
		{
			foreach (var queryKey in request.QueryString)
			{
				// 対応パラメータ名と一致している最初のパラメータ名の探索
				var advParamName = m_validParameters.Keys.FirstOrDefault(
					key => ((key == (string)queryKey)
						&& (request.QueryString.GetValues((string)queryKey).Any(value => (string.IsNullOrEmpty(value) == false)))));

				// 有効なパラメータ名が見つかればその名前を返す
				if (string.IsNullOrEmpty(advParamName) == false)
				{
					return advParamName;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// 設定リストの更新
		/// </summary>
		private static void UpdateValidParameters()
		{
			if (File.Exists(
				Path.Combine(
					w2.Common.Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					DIRNAME_AFFILIATE_PARAMETER_SETTING,
					FILENAME_AFFILIATE_PARAMETER_SETTING)) == false) return;

			var validParameters = new Dictionary<string, int>();

			//XMLからリストを読み込む
			var xDocument = XDocument.Load(
				Path.Combine(
					w2.Common.Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					DIRNAME_AFFILIATE_PARAMETER_SETTING,
					FILENAME_AFFILIATE_PARAMETER_SETTING));
			var rootElement = xDocument.Element("AffiliateParameterSettings");
			if (rootElement == null) return;

			foreach (var parameter in rootElement.Elements("Parameter"))
			{
				var parameterKeyElement = parameter.Element("ParameterKey");
				var validDaysElement = parameter.Element("ValidDays");
				if ((parameterKeyElement == null) || (validDaysElement == null)) continue;

				var parameterKey = parameterKeyElement.Value;
				int validDays;

				if (validParameters.ContainsKey(parameterKey) || (int.TryParse(validDaysElement.Value, out validDays) == false)) continue;
				
				validParameters.Add(parameterKey, validDays);
			}

			m_validParameters = validParameters;
		}
	}
}
