/*
=========================================================================================================
  Module      : ZcomAPI設定 (ZcomApiSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Net;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// ZcomAPI設定
	/// </summary>
	public class ZcomApiSetting
	{
		/// <summary>APIエンコード</summary>
		public Encoding ApiEncoding { get; set; }
		/// <summary>基本認証ユーザーID</summary>
		public string BasicUserId { get; set; }
		/// <summary>基本認証パスワード</summary>
		public string BasicPassword { get; set; }
		/// <summary>HTTPタイムアウト（ミリ秒）</summary>
		public int HttpTimeOutMiriSecond { get; set; }
		/// <summary>クレカ決済URL</summary>
		public string UrlDirectPayment { get; set; }
		/// <summary>取消しURL</summary>
		public string UrlCancelPayment { get; set; }
		/// <summary>実売上URL</summary>
		public string UrlSalsePayment { get; set; }
		/// <summary>Url check auth</summary>
		public string UrlCheckAuth { get; set; }
		/// <summary>
		/// リクエスト実行前処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData> OnBeforeRequest { get; set; }
		/// <summary>
		/// リクエスト実行後処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエスト結果が入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData, string> OnAfterRequest { get; set; }
		/// <summary>
		/// リクエスト実行時エラー処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエストしたURL
		/// 第三引数：発生したExceptionが入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData, string, Exception> OnRequestError { get; set; }
		/// <summary>
		/// Webリクエストの拡張処理を設定
		/// WebRequest拡張処理
		/// 第一引数：WebRequestがわたってくるので好きにプロパティやHttpヘッダ、タイムアウト値等を設定してください
		/// </summary>
		public Action<HttpWebRequest> OnExtendWebRequest { get; set; }
	}
}
