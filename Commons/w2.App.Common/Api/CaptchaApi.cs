/*
=========================================================================================================
  Module      : キャプチャ認証APIクラス(CaptchaApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Common.Logger;

namespace w2.App.Common.Api
{
	/// <summary>
	/// キャプチャ認証APIクラス
	/// </summary>
	public class CaptchaApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="secretKey">シークレットキー</param>
		/// <param name="token">トークン（パラメータ名：g-recaptcha-response）</param>
		public CaptchaApi(string secretKey, string token)
		{
			this.SecretKey = secretKey;
			this.Token = token;
			this.IsSuccess = false;
		}

		/// <summary>
		/// 認証
		/// </summary>
		public void Auth()
		{
			// リクエストURL作成
			var url = new UrlCreator("https://www.google.com/recaptcha/api/siteverify");
			url.AddParam("secret", this.SecretKey);
			url.AddParam("response", this.Token);

			// キャプチャ認証結果取得
			var webRequest = (HttpWebRequest)WebRequest.Create(url.CreateUrl());
			string responseText = "";
			try
			{
				using (var responseStream = webRequest.GetResponse().GetResponseStream())
				using (var reader = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseText = reader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				responseText = SerializeHelper.SerializeJson(
					new Dictionary<string, object>(){
					{ "success", "false" },
					{ "error-codes", new ArrayList { ex.ToString() }}});
			}

			// JSON⇒Dictionaryに変換
			Dictionary<string, object> response;
			try
			{
				response = SerializeHelper.DeserializeJson<Dictionary<string, object>>(responseText);
			}
			catch (Exception ex)
			{
				var message = string.Format("キャプチャ認証結果デシリアライズエラー:{0}",
					ex.ToString());
				AppLogger.WriteError(message);
				return;
			}

			// 成功？
			if (response.ContainsKey("success"))
			{
				this.IsSuccess = bool.Parse(response["success"].ToString());
				if (this.IsSuccess == false)
				{
					var message = string.Format("キャプチャ認証エラー:{0}", 
						string.Join(",", ((ArrayList)response["error-codes"]).ToArray()));
					AppLogger.WriteError(message);
				}
			}
		}

		#region プロパティ
		/// <summary>シークレットキー</summary>
		public string SecretKey { get; private set; }
		/// <summary>トークン（パラメータ名：g-recaptcha-response）</summary>
		public string Token { get; private set; }
		/// <summary>キャプチャ認証成功？</summary>
		public bool IsSuccess { get; private set; }
		#endregion
	}
}