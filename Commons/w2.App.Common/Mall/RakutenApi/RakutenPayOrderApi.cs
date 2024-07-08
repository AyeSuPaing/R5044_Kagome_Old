/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API連携のラッパークラス (RakutenPayOrderApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Mall.Rakuten;
using w2.Common.Helper;
using w2.Common.Logger;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天受注API(WebService)を使用するためのラッパークラス
	/// </summary>
	public class RakutenPayOrderApi
	{
		/// <summary> 楽天ペイ受注API接続 </summary>
		private static HttpClient m_rakutenApiClient = new HttpClient();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mallSetting">モール設定情報</param>
		public RakutenPayOrderApi(DataRowView mallSetting)
		{
			this.ApiUrl = "https://api.rms.rakuten.co.jp/es/2.0/order/getOrder/";
			this.ServiceSecret = (string)mallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET];
			this.LicenseKey = (string)mallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY];
		}

		/// <summary>
		/// 注文IDから注文情報取得
		/// </summary>
		/// <param name="orderIdList">注文ID一覧</param>
		/// <returns>受注情報取得レスポンスモデル</returns>
		public RakutenApiOrderResponse GetRakutenOrderInfo(string[] orderIdList)
		{
			var result = Helper.AsyncHelper.RunSync<RakutenApiOrderResponse>(() => this.GetRakutenOrderInfoAsync(orderIdList));
			return result;
		}
		/// <summary>
		/// 注文IDから注文情報取得
		/// </summary>
		/// <param name="orderIdList">注文ID一覧</param>
		/// <returns>受注情報取得レスポンスモデル</returns>
		private async Task<RakutenApiOrderResponse> GetRakutenOrderInfoAsync(string[] orderIdList)
		{
			var requestParameter = new RakutenApiOrderRequest
			{
				ApiUrl = this.ApiUrl,
				ServiceSecret = this.ServiceSecret,
				LicenseKey = this.LicenseKey,
				ContentType = "application/json",
			};
			var request = new HttpRequestMessage(HttpMethod.Post, requestParameter.ApiUrl);
			request.Headers.Add("Authorization", requestParameter.AuthKey);
			var requestApiVersion = Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "7" : "3";
			request.Content = new StringContent
			(
				SerializeHelper.SerializeJson
				(
					new Hashtable
					{
						{ "orderNumberList", orderIdList.ToList() },
						{ "version", requestApiVersion },
					}
				),
				Encoding.UTF8,
				requestParameter.ContentType
			);

			try
			{
				var response = await m_rakutenApiClient.SendAsync(request);
				var contents = await response.Content.ReadAsStringAsync();
				var result = new RakutenApiOrderResponse(contents);
				return result;
			}
			catch (Exception ex)
			{
				// エラーログに出力
				FileLogger.WriteError($"注文番号[{orderIdList[0]}]の注文情報取得に失敗しました。{Environment.NewLine}{ex.Message}");

				return new RakutenApiOrderResponse();
			}
		}

		/// <summary>受注API用インスタンス</summary>
		public OrderApiService OrderApi { get; set; }
		/// <summary>APIのURL（値保持）</summary>
		private string ApiUrl { get; set; }
		/// <summary>サービスシークレット</summary>
		private string ServiceSecret { get; set; }
		/// <summary>ライセンスキー</summary>
		private string LicenseKey { get; set; }
	}
}
