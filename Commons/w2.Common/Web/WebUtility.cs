/*
=========================================================================================================
  Module      : WEBユーティリティモジュール(WebUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web;

namespace w2.Common.Web
{
	///**************************************************************************************
	/// <summary>
	/// HTTPに関するユーティリティメソッドを提供する
	/// </summary>
	///**************************************************************************************
	public class WebUtility
	{
		/// <summary>
		/// HTTPS通信チェック
		/// </summary>
		/// <param name="hrRequest">HttpRequest</param>
		/// <param name="hrResponse">HttpResponse</param>
		/// <param name="strRedirectUrl">リダイレクト先URL</param>
		/// <remarks>
		/// Constants.PROTOCOL_HTTPSでの通信ではない場合、指定リダイレクト先URLへ遷移する。
		/// </remarks>
		public static void CheckHttps(HttpRequest hrRequest, HttpResponse hrResponse, string strRedirectUrl)
		{
			// HTTPS通信では無い場合指定ページへ
			if (hrRequest.Url.AbsoluteUri.IndexOf(Constants.PROTOCOL_HTTPS.Replace("://", "")) != 0)
			{
				hrResponse.Redirect(strRedirectUrl);
			}
		}

		/// <summary>
		/// RawUrl取得（IISのバージョンによる機能の違いを吸収）
		/// </summary>
		/// <param name="Request">HTTPリクエスト</param>
		/// <returns>RawUrl</returns>
		public static string GetRawUrl(HttpRequest Request)
		{
			// RawUrlは、フレンドリURLなどが「?」以前がUrlEncodeされていないものが取得できてしまう。
			// そのため、再度エンコードを行う
			var rawUrlSplits = Request.RawUrl.Split('?');
			rawUrlSplits[0] = string.Join("/", rawUrlSplits[0].Split('/').Select(HttpUtility.UrlEncode));
			var strRawUrl = string.Join("?", rawUrlSplits);

			if (strRawUrl.Contains("?404;"))	// IIS6系？
			{
				Uri uTargat = new Uri(strRawUrl.Split(';')[1]);
				strRawUrl = uTargat.LocalPath + uTargat.Query;
			}

			return strRawUrl;
		}
	}
}
