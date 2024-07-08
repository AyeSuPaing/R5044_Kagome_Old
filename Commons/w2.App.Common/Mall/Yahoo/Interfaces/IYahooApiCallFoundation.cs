/*
=========================================================================================================
  Module      : YAHOO API API基盤インターフェース (IYahooApiCallFoundation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Mall.Yahoo.Foundation;

namespace w2.App.Common.Mall.Yahoo.Interfaces
{
	/// <summary>
	/// YAHOO API API基盤インターフェース
	/// </summary>
	public interface IYahooApiCallFoundation
	{
		/// <summary>
		/// HTTP GET
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="mediaType">メディアタイプ</param>
		/// <param name="authorization">認証ヘッダ</param>
		/// <returns>結果オブジェクト</returns>
		Task<SharedApiResponse> HttpGet(
			string url,
			MediaTypeWithQualityHeaderValue mediaType,
			string authorization);

		/// <summary>
		/// HTTP POST
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="content">コンテント</param>
		/// <param name="encoding">エンコード</param>
		/// <param name="contentType">コンテントタイプ</param>
		/// <param name="authorization">認証ヘッダ</param>
		/// <param name="signature">シグネチャー</param>
		/// <param name="publicKeyVersion">公開鍵バージョン</param>
		/// <returns>結果オブジェクト</returns>
		Task<SharedApiResponse> HttpPost(
			string url,
			string content,
			Encoding encoding,
			string contentType,
			string authorization,
			string signature = "",
			string publicKeyVersion = "");
	}
}
