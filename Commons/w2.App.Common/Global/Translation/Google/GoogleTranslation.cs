/*
=========================================================================================================
  Module      : Google翻訳クラス(GoogleTranslation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using w2.App.Common.Global.Config;
using w2.Common.Logger;
using w2.Domain.AutoTranslationWord;

namespace w2.App.Common.Global.Translation.Google
{
	/// <summary>
	/// Google翻訳クラス
	/// </summary>
	public class GoogleTranslation
	{
		/// <summary>
		/// 翻訳する
		/// </summary>
		/// <param name="text">テキスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>翻訳した文章</returns>
		public string Translation(string text, string languageCode, string lastChanged)
		{
			// ポスト・データの作成
			var requestParameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("key", GlobalConfigUtil.GetGoogleConnection().GoogleApiKey),
				new KeyValuePair<string, string>("target", languageCode),
				new KeyValuePair<string, string>("format", "html"),
				new KeyValuePair<string, string>("q", text),
				// new KeyValuePair<string, string>("source", "ja"),  //元の言語を明示したいときに使用
			};

			try
			{
				var response = Post(
					requestParameters,
					GlobalConfigUtil.GetGoogleConnection().TranslationUrl);

				var translationData = JsonConvert.DeserializeObject<TranslationData>(response);

				var translatedText = HttpUtility.HtmlDecode(translationData.Data.TranslationsList[0].TranslatedText);

				var autoTranslationWordModel = new AutoTranslationWordModel
					{
						WordHashKey = TranslationManager.WordHash(text),
						LanguageCode = languageCode,
						WordBefore = text,
						WordAfter = translatedText,
						LastChanged = lastChanged
					};
				TranslationManager.ControlStackInsertWords(autoTranslationWordModel);

				return translatedText;
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("Google翻訳API:存在しない言語コードが入力されました。", ex);
			}

			return text;
		}

		/// <summary>
		/// Postする
		/// </summary>
		/// <param name="requestParameters">パラメータDictionary</param>
		/// <param name="url">Post先のUrl</param>
		/// <returns>Postしたレスポンス</returns>
		private static string Post(List<KeyValuePair<string, string>> requestParameters, string url)
		{
			var encodeType = Encoding.UTF8;

			var param = string.Join("&", requestParameters.Select(p => string.Format("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value, encodeType))));

			var data = encodeType.GetBytes(param);

			//リクエスト設定
			var request = WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;

			// ポスト・データの書き込み
			using (var reqStream = request.GetRequestStream())
			{
				reqStream.Write(data, 0, data.Length);
			}

			// リクエスト送信
			using (var responseStream = request.GetResponse().GetResponseStream())
			using (var streamReader = new StreamReader(responseStream))
			{
				var body = streamReader.ReadToEnd();
				return body;
			}
		}

		/// <summary>
		/// 翻訳データ
		/// </summary>
		public class TranslationData
		{
			/// <summary>翻訳データ</summary>
			[JsonProperty("data")]
			public Translations Data { get; set; }
		}

		/// <summary>
		/// 翻訳データ
		/// </summary>
		public class Translations
		{
			/// <summary>翻訳リスト</summary>
			[JsonProperty("translations")]
			public List<TranslationsValue> TranslationsList { get; set; }
		}

		/// <summary>
		/// 翻訳の値
		/// </summary>
		public class TranslationsValue
		{
			/// <summary>翻訳文</summary>
			[JsonProperty("translatedText")]
			public string TranslatedText { get; set; }
			/// <summary>特定した言語</summary>
			[JsonProperty("detectedSourceLanguage")]
			public string DetectedSourceLanguage { get; set; }
		}
	}
}
