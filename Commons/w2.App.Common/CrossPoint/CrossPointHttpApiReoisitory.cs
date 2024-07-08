/*
=========================================================================================================
  Module      : CrossPoint API レポジトリ基底クラス (CrossPointHttpApiReoisitory.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.CrossPoint.Helper;
using w2.Common.Web;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// レポジトリ基底クラス
	/// </summary>
	internal abstract class CrossPointHttpApiReoisitory
	{
		/// <summary>API接続用Http</summary>
		private static readonly HttpClient s_httpClient = new HttpClient();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CrossPointHttpApiReoisitory()
		{
		}

		/// <summary>
		/// レスポンス文字列を取得
		/// </summary>
		/// <param name="apiUrl">APIのURL</param>
		/// <param name="httpClient">接続用HTTPクライアント</param>
		/// <param name="paramList">リクエスト用パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		public string GetResponse(
			string apiUrl,
			HttpClient httpClient,
			IDictionary<string, string> paramList)
		{
			var request = GetRequest(apiUrl, paramList);
			var result = AsyncHelper.RunSync(() => GetResponseAsync(httpClient, request));
			return result;
		}
		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="apiName">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>レスポンスデータ</returns>
		public string GetResponse(string apiName, IDictionary<string, string> param)
		{
			var response = GetResponse(apiName, s_httpClient, param);
			return response;
		}

		/// <summary>
		/// パラメータを含むリクエストを取得
		/// </summary>
		/// <param name="apiUrl">APIのURL</param>
		/// <param name="paramList">リクエスト用パラメータ</param>
		/// <returns>リクエスト</returns>
		private HttpRequestMessage GetRequest(string apiUrl, IDictionary<string, string> paramList)
		{
			var characterCode = Encoding.GetEncoding("shift_jis");
			paramList[Constants.CROSS_POINT_PARAM_AUTH_TENANT_CODE] = Constants.CROSS_POINT_AUTH_TENANT_CODE;
			paramList[Constants.CROSS_POINT_PARAM_AUTH_SHOP_CODE] = Constants.CROSS_POINT_AUTH_SHOP_CODE;

			var signSource = string.Join(
				@"&",
				paramList.Select(param => string.Format(
					"{0}={1}",
					param.Key,
					param.Value)));

			var url = new UrlCreator(Constants.CROSS_POINT_API_URL_ROOT_PATH + apiUrl)
				.AddRangeParam(paramList)
				.AddParam("signing", GetSign(signSource, characterCode))
				.CreateUrl(characterCode)
				.Replace("+", "%20");

			var request = new HttpRequestMessage(HttpMethod.Get, url);
			return request;
		}

		/// <summary>
		/// リクエストからレスポンス文字列を取得(非同期)
		/// </summary>
		/// <param name="httpClient">接続用HTTPクライアント</param>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス文字列</returns>
		private async Task<string> GetResponseAsync(HttpClient httpClient, HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var result = await response.Content.ReadAsStringAsync();
			return result;
		}

		/// <summary>
		/// リクエストの署名を取得
		/// </summary>
		/// <param name="param">リクエスト引数</param>
		/// <param name="encode">エンコード</param>
		/// <returns>署名文字列</returns>
		private string GetSign(string param, Encoding encode)
		{
			using (var md5 = MD5.Create())
			{
				var bytes = md5.ComputeHash(
					encode.GetBytes(
						string.Concat(
							param,
							Constants.CROSS_POINT_AUTH_AUTHENTICATION_KEY)));

				var sign = string.Concat(bytes.Select(b => string.Format("{0:x2}", b)));
				return sign;
			}
		}
	}
}
