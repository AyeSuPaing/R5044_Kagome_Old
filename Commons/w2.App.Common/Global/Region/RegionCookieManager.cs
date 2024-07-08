/*
=========================================================================================================
  Module      : リージョンクッキー管理クラス(RegionCookieManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using w2.Common.Web;

namespace w2.App.Common.Global.Region
{
	/// <summary>
	/// リージョンクッキー管理クラス
	/// </summary>
	public class RegionCookieManager
	{
		/// <summary>クッキー有効期限(年)</summary>
		private const int COOKIE_EXPIRES_ADD_YEARS = 20;

		/// <summary>
		/// リージョンクラスクッキーの設定 フロント限定
		/// </summary>
		/// <param name="model">リージョンモデル</param>
		public static void SetCookie(RegionModel model)
		{
			if (CheckFrontApp() == false) return;

			var jsonModel = JsonConvert.SerializeObject(model);
			CookieManager.SetCookie(
				Constants.COOKIE_KEY_GLOBAL_REGION,
				jsonModel,
				Constants.PATH_ROOT,
				DateTime.Now.AddYears(COOKIE_EXPIRES_ADD_YEARS),
				secure: true);
		}

		/// <summary>
		/// リージョンクラスクッキーの取得 フロント限定
		/// </summary>
		/// <returns>リージョンモデル クッキーが存在しない場合はnull</returns>
		public static RegionModel GetValue()
		{
			if (CheckFrontApp() == false) return null;

			RegionModel model = null;
			try
			{
				var value = CookieManager.GetValue(Constants.COOKIE_KEY_GLOBAL_REGION);
				if (string.IsNullOrEmpty(value)) return null;

				model = JsonConvert.DeserializeObject<RegionModel>(value);
				if (model.RegionCheck() == false) model = null;
			}
			catch (Exception)
			{
				model = null;
			}

			return model;
		}

		/// <summary>
		/// リージョンクラスクッキーの削除 フロント限定
		/// </summary>
		public static void RemoveCookie()
		{
			if (CheckFrontApp() == false) return;

			CookieManager.RemoveCookie(Constants.COOKIE_KEY_GLOBAL_REGION, Constants.PATH_ROOT);
		}

		/// <summary>
		/// フロントチェック
		/// </summary>
		/// <returns>TRUE:フロント FALSE:フロント以外</returns>
		private static bool CheckFrontApp()
		{
			var isFront = ((HttpContext.Current != null) && Constants.CONFIGURATION_SETTING.ReadKbnList.Any(s => s == ConfigurationSetting.ReadKbn.C300_Pc));
			return isFront;
		}
	}
}
