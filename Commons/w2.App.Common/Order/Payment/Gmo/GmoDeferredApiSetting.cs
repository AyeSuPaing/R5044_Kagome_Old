/*
=========================================================================================================
  Module      : Gmo後払いApi設定(GmoDeferredApiSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// Gmo後払いApi設定
	/// </summary>
	public class GmoDeferredApiSetting
	{
		/// <summary>APIエンコード</summary>
		public Encoding ApiEncoding { get; set; }

		/// <summary>基本認証ユーザーID</summary>
		public string BasicUserId { get; set; }

		/// <summary>基本認証パスワード</summary>
		public string BasicPassword { get; set; }

		/// <summary>HTTPタイムアウト（ミリ秒）</summary>
		public int HttpTimeOutMiriSecond { get; set; }

		/// <summary>取引登録URL</summary>
		public string UrlOrderRegister { get; set; }

		/// <summary>与信審査結果取得URL</summary>
		public string UrlGetAuthResult { get; set; }

		/// <summary>請求書印字データ取得URL</summary>
		public string UrlGetinvoicePrintData { get; set; }

		/// <summary>取引修正・キャンセルURL</summary>
		public string UrlOrderModifyCancel { get; set; }

		/// <summary>請求減額URL</summary>
		public string UrlReduceSales { get; set; }

		/// <summary>出荷報告URL</summary>
		public string UrlShipment { get; set; }

		/// <summary>出荷報告修正・キャンセルURL</summary>
		public string UrlShipmentModifyCancel { get; set; }

		/// <summary>入金状況確認URL</summary>
		public string UrlGetDefPaymentStatus { get; set; }

		/// <summary>Url reissue</summary>
		public string UrlReissue { get; set; }

		/// <summary>
		/// リクエスト実行前処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData> OnBeforeRequest;

		/// <summary>
		/// リクエスト実行後処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエスト結果が入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData, string> OnAfterRequest;

		/// <summary>
		/// リクエスト実行時エラー処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエストしたURL
		/// 第三引数：発生したExceptionが入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData, string, Exception> OnRequestError;

		/// <summary>
		/// Webリクエストの拡張処理を設定
		/// </summary>
		/// <param name="method">
		/// WebRequest拡張処理
		/// 第一引数：WebRequestがわたってくるので好きにプロパティやHttpヘッダ、タイムアウト値等を設定してください
		/// </param>
		public Action<HttpWebRequest> OnExtendWebRequest;
	}
}
