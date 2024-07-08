/*
=========================================================================================================
  Module      : リージョン管理クラス(RegionManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.CountryIpv4;
using w2.Domain.CountryLocation;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Global.Region
{
	/// <summary>
	/// リージョン管理クラス
	/// １回のアクセスでIPアドレスが変動することはなく、
	///  管理インスタンスが複数あると混乱するためインスタンスはセッションで管理しています。
	///  また、クッキーはレスポンスを返した後でないと更新が反映されないため、最新のリージョンはこの管理インスタンスより取得するようにしてください。
	/// </summary>
	[Serializable]
	public class RegionManager
	{
		/// <summary>ローカル開発時のIPアドレス</summary>
		private const string LOCAL_DEVELOP_IP = "111.89.210.65";

		/// <summary>クローラユーザエージェントパターン</summary>
		private string[] m_crawlerUserAgentPattern = new string[]
		{
			"Y!J-SRD",
			"Y!J-MBS/1.0",
			"Yahoo! Slurp",
			"Yahoo! DE Slurp",
			// "Googlebot-Mobile", Googleは一般ユーザと同様に扱う必要があるため、クローラ巡回時はIPアドレスによる振り分けを行います
			// "Googlebot/",
			"adsence-Google",
			"adsence-Google",
			"msnbot",
			"bingbot/",
			"Hatena",
			"MicroAd/",
			"Baidu",
			"MJ12bo",
			"Steeler",
			"YodaoBot",
			"OutfoxBot",
			"Pockey",
			"psbot",
			"Yeti/",
			"Websi",
			"Wget/",
			"NaverBot",
			"BecomeBot",
			"heritr",
			"DotBot",
			"Twiceler",
			"ichiro",
			"archive.org_bot",
			"YandexBot/",
			"ICC-Crawler",
			"SiteBot/",
			"TurnitinBot/",
			"Purebot/",
			"facebookexternalhit/1.1",
			"Facebot",
			"APIs-Google",
			"Twitterbot",
			"https://developers.google.com/+/web/snippet/",
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegionManager()
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false)
			{
				RegionCookieManager.RemoveCookie();
				this.Region = new RegionModel();
				return;
			}

			//リージョンモデルの初期設定
			var cookieModel = RegionCookieManager.GetValue();
			this.Region = cookieModel ?? new RegionModel();
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static RegionManager GetInstance()
		{
			if ((HttpContext.Current == null) || (HttpContext.Current.Session == null)) return new RegionManager();

			if (HttpContext.Current.Session[Constants.SESSION_KEY_GLOBAL_REGION] == null) HttpContext.Current.Session[Constants.SESSION_KEY_GLOBAL_REGION] = new RegionManager();
			return (RegionManager)HttpContext.Current.Session[Constants.SESSION_KEY_GLOBAL_REGION];
		}

		/// <summary>
		/// IPアドレス範囲による国の割り振り
		/// </summary>
		/// <param name="userId">会員:ユーザID , ゲスト:空</param>
		/// <param name="forcibly">強制更新:true, リージョンクッキーが存在しない場合のみ:false</param>
		public void CountryAllcationByIpAddressIpv4(string userId = "", bool forcibly = false)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return;

			// クローラBotの場合はデフォルト国ISOコードにてリージョン設定
			if (IsCrawler(HttpContext.Current.Request.UserAgent))
			{
				this.Region = GetRegionModelByCountryIsoCode(Constants.GLOBAL_CONFIGS.GlobalSettings.NotFoundCountryIsoCode);
				RegionCookieManager.SetCookie(this.Region);
				return;
			}

			var cookieModel = RegionCookieManager.GetValue();
			if ((cookieModel == null) || forcibly)
			{
				var countryIsoCode = GetCountryIsoCode();
				RegistrationRegion(countryIsoCode, userId);
			}
			else
			{
				this.Region = cookieModel;
			}
		}

		/// <summary>
		/// リージョン更新
		/// 現在、リージョンモデルに設定されている内容がリージョンクッキー及びDBに反映されます。
		/// </summary>
		/// <param name="model">RegionModelインスタンス(空プロパティは更新されません)</param>
		/// <param name="userId">ユーザーID</param>
		public void UpdateUserRegion(RegionModel model, string userId)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return;

			if ((model == null) || (this.Region.UpdateRegionModel(model) == false))
			{
				FileLogger.WriteError(string.Format("リージョン更新に失敗しました。 ユーザ:{0}", userId));
				return;
			}

			RegionCookieManager.SetCookie(this.Region);

			if (string.IsNullOrEmpty(userId) == false)
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var userService = new UserService();
					var userModel = userService.Get(userId, accessor);
					if (userModel != null)
					{
						userModel.AccessCountryIsoCode = this.Region.CountryIsoCode;
						userModel.DispLanguageCode = this.Region.LanguageCode;
						userModel.DispLanguageLocaleId = this.Region.LanguageLocaleId;
						userModel.DispCurrencyCode = this.Region.CurrencyCode;
						userModel.DispCurrencyLocaleId = this.Region.CurrencyLocaleId;
						userService.Update(userModel, UpdateHistoryAction.DoNotInsert, accessor);
					}
					accessor.CommitTransaction();
				}
			}
		}

		/// <summary>
		/// アクセスユーザのIpv4アドレスを取得
		/// </summary>
		/// <returns></returns>
		private string GetIpAddressIpv4()
		{
			//IP取得
			var userIpAddressIpv4 = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

			//ローカル開発環境 日本のIPアドレスを設定
			userIpAddressIpv4 = (userIpAddressIpv4 == "::1") ? LOCAL_DEVELOP_IP : userIpAddressIpv4;

			//動作確認用 パラメータが設定されている場合
			if (HttpContext.Current.Request[Constants.REQUEST_KEY_GLOBAL_IP] != null)
			{
				var requestIp = HttpContext.Current.Request[Constants.REQUEST_KEY_GLOBAL_IP];
				userIpAddressIpv4 = (IpAddressUtil.CheckIp(requestIp)) ? requestIp : userIpAddressIpv4;
			}

			return userIpAddressIpv4;
		}

		/// <summary>
		/// 国ISOコードの取得
		/// </summary>
		/// <returns>国ISOコード</returns>
		private string GetCountryIsoCode()
		{
			//IPアドレスより国ISOコードの算出、算出できない場合はデフォルト
			var countryIsoCode = Constants.GLOBAL_CONFIGS.GlobalSettings.NotFoundCountryIsoCode;
			var countryIpv4 = new CountryIpv4Service().GetByIpNumeric(IpAddressUtil.ConvertToInt(GetIpAddressIpv4()));
			if (countryIpv4 != null)
			{
				var countryLocation = new CountryLocationService().Get(countryIpv4.GeonameId);
				if (countryLocation != null)
				{
					countryIsoCode = countryLocation.CountryIsoCode;
				}
			}

			//IP振り分け対象に存在しない場合はデフォルト
			if (Constants.GLOBAL_CONFIGS.GlobalSettings.Regions.Any(r => r.CountryIsoCode == countryIsoCode) == false)
			{
				countryIsoCode = Constants.GLOBAL_CONFIGS.GlobalSettings.NotFoundCountryIsoCode;
			}

			return countryIsoCode;
		}

		/// <summary>
		/// 国リージョンの登録
		/// ゲスト(user_idが存在しない):リージョンクッキーのみ更新
		/// 会員(user_idが存在する):リージョンクッキーとDB更新
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <param name="userId">ユーザID</param>
		private void RegistrationRegion(string countryIsoCode, string userId)
		{
			var model = GetRegionModelByCountryIsoCode(countryIsoCode);
			UpdateUserRegion(model, userId);
		}

		/// <summary>
		/// 国ISOコードよりリージョンモデルを取得
		/// </summary>
		/// <param name="countryIsoCode"></param>
		/// <returns>リージョンモデル</returns>
		private RegionModel GetRegionModelByCountryIsoCode(string countryIsoCode)
		{
			var region = Constants.GLOBAL_CONFIGS.GlobalSettings.Regions.FirstOrDefault(r => r.CountryIsoCode == countryIsoCode);
			if (region == null) return null;

			var model = new RegionModel
			{
				CountryIsoCode = region.CountryIsoCode,
				LanguageCode = region.LanguageCode,
				LanguageLocaleId = region.LanguageLocaleId,
				CurrencyCode = region.CurrencyCode,
				CurrencyLocaleId = region.CurrencyLocaleId
			};
			return model;
		}

		/// <summary>
		/// UAがクローラーのものであるかどうか
		/// </summary>
		/// <param name="userAgent">調べるUA</param>
		/// <returns>
		/// True:クローラーのUAである
		/// False：クローラーのUAではない
		/// </returns>
		private bool IsCrawler(string userAgent)
		{
			if (string.IsNullOrEmpty(userAgent)) return false;

			var result = m_crawlerUserAgentPattern.Any(agent => userAgent.Contains(agent));
			return result;
		}

		/// <summary>(読み込み専用)最新のリージョン情報</summary>
		public RegionModel Region { get; private set; }
	}
}
