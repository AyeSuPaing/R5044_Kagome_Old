/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI 基底クラス(SocialLoginBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;
using w2.Common.Logger;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ソーシャルログイン基底クラス
	/// </summary>
	public class SocialLoginBase
	{
		/// <summary>URL設定</summary>
		private static readonly SocialLoginUrlSetting m_urlSetting = null;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static SocialLoginBase()
		{
			m_urlSetting = new SocialLoginUrlSetting();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="functiontype">機能区分</param>
		public SocialLoginBase(SocialLoginApiFunctionType functiontype)
		{
			this.FunctionType = functiontype;
			this.Url = m_urlSetting.GetApiUrl(functiontype, this.ProviderType);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="functiontype">機能区分</param>
		/// <param name="providertype">プロバイダ区分</param>
		public SocialLoginBase(SocialLoginApiFunctionType functiontype, SocialLoginApiProviderType providertype)
		{
			this.FunctionType = functiontype;
			this.Url = string.Format(
				m_urlSetting.GetApiUrl(functiontype, providertype),
				Constants.SOCIAL_LOGIN_ACCOUNT_ID,
				Constants.SOCIAL_LOGIN_SITE_ID,
				providertype.ToValue());
			this.ProviderType = providertype;
		}

		/// <summary>
		/// GETリクエスト行います。
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンス</returns>
		protected string GetHttpRequest(string[][] param)
		{
			var requestUrl = this.Url + "?" + SocialLoginUtil.GetQueryParam(param);
			return ExecHttpRequest(WebRequestMethods.Http.Get, requestUrl);
		}

		/// <summary>
		/// PUTリクエスト行います。
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンス</returns>
		protected string PutHttpRequest(string[][] param)
		{
			return ExecHttpRequest(WebRequestMethods.Http.Put, this.Url, SocialLoginUtil.GetQueryParam(param));
		}

		/// <summary>
		/// HTTPリクエストを行います。
		/// </summary>
		/// <param name="httpMethod">HTTPメソッド</param>
		/// <param name="requestUrl">リクエストURL</param>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンス</returns>
		protected string ExecHttpRequest(string httpMethod, string requestUrl, string param = null)
		{
			AppLogger.WriteDebug(string.Format("[social plus][request][{0}] {1}", httpMethod, requestUrl));

			var webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
			webRequest.Method = httpMethod;
			webRequest.Timeout = Constants.SOCIAL_LOGIN_WEBAPI_TIMEOUT;

			string responseText = null;
			try
			{
				if (httpMethod == WebRequestMethods.Http.Put)
				{
					var byteArray = Encoding.UTF8.GetBytes(param);

					webRequest.ContentType = "application/x-www-form-urlencoded";
					webRequest.ContentLength = byteArray.Length;

					using (var rstream = webRequest.GetRequestStream())
					{
						rstream.Write(byteArray, 0, byteArray.Length);
					}
				}

				using (var response = webRequest.GetResponse())
				using (var rstream = response.GetResponseStream())
				using (var sreader = new StreamReader(rstream, Encoding.UTF8))
				{
					responseText = sreader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					using (var rstream = ((HttpWebResponse)ex.Response).GetResponseStream())
					using (var sreader = new StreamReader(rstream, Encoding.UTF8))
					{
						responseText = sreader.ReadToEnd();
					}
				}
				else
				{
					responseText = ex.ToString();
				}
			}

			AppLogger.WriteDebug("[social plus][response] " + responseText);

			return responseText;
		}

		/// <summary>機能区分</summary>
		public SocialLoginApiFunctionType FunctionType { get; set; }
		/// <summary>URL</summary>
		public string Url { get; set; }
		/// <summary>プロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get; set; }
	}
}
