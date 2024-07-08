/*
=========================================================================================================
  Module      : CurrencyLayer WebAPIクラス(CurrencyLayerWebApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using w2.Commerce.GetExchangeRate.WebApi.CurrencyLayer.WebApi.Model;
using w2.Common.Web;

namespace w2.Commerce.GetExchangeRate.WebApi.CurrencyLayer
{
	/// <summary>
	/// CurrencyLayer WebAPIクラス
	/// </summary>
	internal class CurrencyLayerWebApi : WebApiBase
	{
		#region クエリパラメータ
		/// <summary>クエリパラメータキー: アクセスキー</summary>
		private const string QUERY_PARAMETER_KEY_ACCESS_KEY = "access_key";
		/// <summary>クエリパラメータキー: 通貨コード(元)</summary>
		private const string QUERY_PARAMETER_KEY_SOURCE = "source";
		/// <summary>クエリパラメータキー: フォーマット</summary>
		private const string QUERY_PARAMETER_KEY_FORMAT = "format";
		/// <summary>クエリパラメータキー: 通貨コード(先)</summary>
		private const string QUERY_PARAMETER_KEY_CURRENCIES = "currencies";

		/// <summary>クエリパラメータ値: フォーマット有効</summary>
		private const string QUERY_PARAMETER_VALUE_FORMAT_ON = "1";
		#endregion クエリパラメータ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseUrl">ベースURL</param>
		/// <param name="accessKey">アクセスキー</param>
		public CurrencyLayerWebApi(string baseUrl, string accessKey)
			: base()
		{
			this.BaseUrl = baseUrl;
			this.AccessKey = accessKey;
			this.ResponseResult = new Dictionary<string, LiveResponseModel>();
		}

		/// <summary>
		/// レートを取得します。
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCodes">通貨コード（先）</param>
		/// <param name="doFormat">レスポンス(JSON)をフォーマットする：true、フォーマットしない：false</param>
		/// <returns>レスポンス</returns>
		public GetExchangeRateResposeModel GetExchangeRates(
			string srcCurrencyCode,
			string[] dstCurrencyCodes = null,
			bool doFormat = false)
		{
			var parameter = new Dictionary<string, string>
			{
				{QUERY_PARAMETER_KEY_ACCESS_KEY, this.AccessKey},
				{QUERY_PARAMETER_KEY_SOURCE, srcCurrencyCode},
			};

			if (doFormat) parameter.Add(QUERY_PARAMETER_KEY_FORMAT, QUERY_PARAMETER_VALUE_FORMAT_ON);
			if (dstCurrencyCodes.Any()) parameter.Add(QUERY_PARAMETER_KEY_CURRENCIES, string.Join(",", dstCurrencyCodes));

			var urlCreator = new UrlCreator(this.BaseUrl + "live");
			foreach (var p in parameter) urlCreator.AddParam(p.Key, p.Value);
			var response = Get(new Uri(urlCreator.CreateUrl()));
			var responseDeserialize = JsonConvert.DeserializeObject<LiveResponseModel>(response);
			if (this.ResponseResult.ContainsKey(srcCurrencyCode) == false)
			{
				this.ResponseResult.Add(srcCurrencyCode, responseDeserialize);
			}
			return ConvertExchangeRateModel(responseDeserialize, srcCurrencyCode);
		}
		/// <summary>
		/// レートを取得します。
		/// </summary>
		/// <param name="srcCurrencyCodes">通貨コード（元）</param>
		/// <param name="dstCurrencyCodes">通貨コード（先）</param>
		/// <returns>レスポンス</returns>
		public IEnumerable<GetExchangeRateResposeModel> GetExchangeRates(string[] srcCurrencyCodes, string[] dstCurrencyCodes = null)
		{
			return srcCurrencyCodes.Select(srcCurrencyCode => GetExchangeRates(srcCurrencyCode, dstCurrencyCodes));
		}

		/// <summary>
		/// 為替レートをJSON文字列に変換します。
		/// </summary>
		/// <param name="rates">為替レートレスポンスモデルの配列</param>
		/// <returns>為替レート配列のJSON文字列</returns>
		public string ConvertExchangeRatesToJson(IEnumerable<GetExchangeRateResposeModel> rates)
		{
			var rateResult = rates.Select(rate => this.ResponseResult[rate.Source]);
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
		/// LiveレスポンスモデルをGetExchangeRateモデルに変換
		/// </summary>
		/// <param name="liveModel">為替レート結果</param>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <returns>ExchangeRateモデル</returns>
		public GetExchangeRateResposeModel ConvertExchangeRateModel(LiveResponseModel liveModel, string srcCurrencyCode)
		{
			var exchangeRate = new GetExchangeRateResposeModel
			{
				Success = liveModel.Success,
				Source = srcCurrencyCode,
				Quotes = liveModel.Quotes,
			};

			return exchangeRate;
		}
		/// <summary>ベースURL</summary>
		public string BaseUrl { get; set; }
		/// <summary>アクセスキー</summary>
		public string AccessKey { get; set; }
		/// <summary>レスポンス結果</summary>
		public Dictionary<string, LiveResponseModel> ResponseResult { get; set; }
	}
}