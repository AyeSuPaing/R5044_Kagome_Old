/*
=========================================================================================================
  Module      : ExchangeRate WebAPIクラス(ExchangeRateWebApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using w2.Commerce.GetExchangeRate.WebApi.ExchangeRate.WebApi.Model;

namespace w2.Commerce.GetExchangeRate.WebApi.ExchangeRateApi
{
	/// <summary>
	/// ExchangeRate WebAPIクラス
	/// </summary>
	internal class ExchangeRateWebApi : WebApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseUrl">ベースURL</param>
		/// <param name="accessKey">アクセスキー</param>
		public ExchangeRateWebApi(string baseUrl, string accessKey)
			: base()
		{
			this.BaseUrl = baseUrl;
			this.AccessKey = accessKey;
			this.ResponseResult = new Dictionary<string, ExchangeRateResponseModel>();
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
		/// レートを取得します。
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCodes">通貨コード（先）</param>
		/// <returns>レスポンス</returns>
		public GetExchangeRateResposeModel GetExchangeRates(
			string srcCurrencyCode,
			string[] dstCurrencyCodes = null)
		{

			var url = this.BaseUrl + this.AccessKey + "/latest/" + srcCurrencyCode;
			var response = Get(new Uri(url));
			var responseDeserialize = JsonConvert.DeserializeObject<ExchangeRateResponseModel>(response);
			if (this.ResponseResult.ContainsKey(srcCurrencyCode) == false)
			{
				this.ResponseResult.Add(srcCurrencyCode, responseDeserialize);
			}
			return ConvertExchangeRateModel(responseDeserialize, srcCurrencyCode, dstCurrencyCodes);
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
		/// ExchangeRateレスポンスモデルをGetExchangeRateモデルに変換
		/// </summary>
		/// <param name="exchangeRateModel">為替レート結果</param>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCodes">通貨コード（先）</param>
		/// <returns>ExchangeRateモデル</returns>
		public GetExchangeRateResposeModel ConvertExchangeRateModel(ExchangeRateResponseModel exchangeRateModel, string srcCurrencyCode, string[] dstCurrencyCodes)
		{
			// 通貨コード（先）のレートのみ取得
			var quotes = new Dictionary<string, string>();
			if (exchangeRateModel.Success == "success")
			{
				Array.ForEach(dstCurrencyCodes, dstCurrencyCode => quotes.Add(srcCurrencyCode + dstCurrencyCode, exchangeRateModel.ConversionRates[dstCurrencyCode]));
			}

			var exchangeRate = new GetExchangeRateResposeModel
			{
				Success = (exchangeRateModel.Success == "success"),
				Source = srcCurrencyCode,
				Quotes = quotes,
			};

			return exchangeRate;
		}

		/// <summary>ベースURL</summary>
		public string BaseUrl { get; set; }
		/// <summary>アクセスキー</summary>
		public string AccessKey { get; set; }
		/// <summary>レスポンス結果</summary>
		public Dictionary<string, ExchangeRateResponseModel> ResponseResult { get; set; }
	}
}