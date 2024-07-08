/*
=========================================================================================================
  Module      : CurrencyConverter WebAPIクラス(CurrencyConverterWebApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using w2.Common.Web;

namespace w2.Commerce.GetExchangeRate.WebApi.CurrencyConverterWebApi
{
	/// <summary>
	/// CurrencyConverter WebAPIクラス
	/// </summary>
	internal class CurrencyConverterWebApi : WebApiBase
	{
		#region クエリパラメータ
		/// <summary>クエリパラメータキー: アクセスキー</summary>
		private const string QUERY_PARAMETER_KEY_ACCESS_KEY = "apiKey";
		/// <summary>クエリパラメータキー: 通貨ペア</summary>
		private const string QUERY_PARAMETER_KEY_PAIR = "q";
		/// <summary>クエリパラメータキー: レスポンスフォーマット</summary>
		private const string QUERY_PARAMETER_KEY_RESPONSE_FORMAT = "compact";
		#endregion クエリパラメータ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseUrl">ベースURL</param>
		/// <param name="accessKey">アクセスキー</param>
		public CurrencyConverterWebApi(string baseUrl, string accessKey)
			: base()
		{
			this.BaseUrl = baseUrl;
			this.AccessKey = accessKey;
			this.ResponseResult = new Dictionary<string, List<Dictionary<string, string>>>();
		}

		/// <summary>
		/// レートを取得します。
		/// </summary>
		/// <param name="srcCurrencyCodes">通貨コード（元）</param>
		/// <param name="dstCurrencyCodes">通貨コード（先）</param>
		/// <returns>レスポンス</returns>
		public IEnumerable<GetExchangeRateResposeModel> GetExchangeRates(string[] srcCurrencyCodes, string[] dstCurrencyCodes = null)
		{
			if (this.ResponseResult == null)
			{
				this.ResponseResult = new Dictionary<string, List<Dictionary<string, string>>>();
			}
			return srcCurrencyCodes.Select(srcCurrencyCode => GetExchangeRates(srcCurrencyCode, dstCurrencyCodes));
		}
		/// <summary>
		/// レートを取得します。
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCodes">通貨コード（先）</param>
		/// <returns>レスポンス</returns>
		public GetExchangeRateResposeModel GetExchangeRates(
			string srcCurrencyCode,
			string[] dstCurrencyCodes = null)
		{
			var responseResult = new List<Dictionary<string, string>>();
			var parameter = new Dictionary<string, string>();
			if (dstCurrencyCodes.Any())
			{
				foreach (var curenncy in dstCurrencyCodes)
				{
					parameter.Clear();
					parameter.Add(QUERY_PARAMETER_KEY_ACCESS_KEY, this.AccessKey);
					parameter.Add(QUERY_PARAMETER_KEY_RESPONSE_FORMAT, "ultra");
					parameter.Add(QUERY_PARAMETER_KEY_PAIR, (srcCurrencyCode + "_" + curenncy));
					var urlCreator = new UrlCreator(this.BaseUrl);
					foreach (var p in parameter) urlCreator.AddParam(p.Key, p.Value);
					var a = urlCreator.CreateUrl();
					var response = Get(new Uri(urlCreator.CreateUrl()));
					responseResult.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(response));
				}
			}
			if (this.ResponseResult.ContainsKey(srcCurrencyCode) == false)
			{
				this.ResponseResult.Add(srcCurrencyCode, responseResult);
			}
			return ConvertExchangeRateModel(responseResult, srcCurrencyCode);
		}

		/// <summary>
		/// 為替レートをJSON文字列に変換します。
		/// </summary>
		/// <param name="rates">為替レートレスポンスモデルの配列</param>
		/// <returns>為替レート配列のJSON文字列</returns>
		public string ConvertExchangeRatesToJson(IEnumerable<GetExchangeRateResposeModel> rates)
		{
			var rateResult = rates.Select(res => this.ResponseResult[res.Source]);
			var contents = JsonConvert.SerializeObject(
				rateResult,
				Formatting.Indented,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
			return contents;
		}

		/// <summary>
		/// ApiレスポンスをGetExchangeRateモデルに変換
		/// </summary>
		/// <param name="rate">為替レート結果</param>
		/// <param name="source">通貨コード（元）</param>
		/// <returns>ExchangeRateモデル</returns>
		public GetExchangeRateResposeModel ConvertExchangeRateModel(List<Dictionary<string,string>> rate, string source)
		{
			var exchangeRate = new GetExchangeRateResposeModel
			{
				Success = true,
				Source = source,
				Quotes = new Dictionary<string, string>(),
			};

			foreach (var res in rate)
			{
				if(res.ContainsKey("error"))
				{
					exchangeRate.Success = false;
					break;
				}

				foreach (KeyValuePair<string, string> item in res)
				{
					var k = item.Key.Replace("_", "");
					exchangeRate.Quotes.Add(item.Key.Replace("_", ""), item.Value);
				}
			}

			return exchangeRate;
		}

		/// <summary>ベースURL</summary>
		public string BaseUrl { get; set; }
		/// <summary>アクセスキー</summary>
		public string AccessKey { get; set; }
		/// <summary>レスポンス結果</summary>
		public Dictionary<string, List<Dictionary<string, string>>> ResponseResult { get; set; }
	}
}