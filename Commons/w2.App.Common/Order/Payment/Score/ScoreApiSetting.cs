/*
=========================================================================================================
  Module      : スコア後払いApi設定(ScoreApiSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Net;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Score
{
	/// <summary>
	/// スコア後払いApi設定
	/// </summary>
	public class ScoreApiSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="onAfterRequest">リクエスト実行後処理</param>
		/// <param name="onBeforeRequest">リクエスト実行前処理</param>
		/// <param name="onRequestError">リクエスト実行時エラー処理</param>
		/// <param name="onExtendWebRequest">Webリクエストの拡張処理を設定</param>
		/// <param name="apiEncoding">APIエンコード</param>
		/// <param name="basicUserId">基本認証ユーザーID</param>
		/// <param name="basicPassword">基本認証パスワード</param>
		/// <param name="httpTimeOutMiriSecond">HTTPタイムアウト（ミリ秒）</param>
		/// <param name="orderRegisterUrl">取引登録URL</param>
		/// <param name="orderModifyUrl">取引修正URL</param>
		/// <param name="orderCancelUrl">取引キャンセルURL</param>
		/// <param name="getAuthResultUrl">与信審査結果取得URL</param>
		/// <param name="deliveryRegisterUrl">発送情報登録URL</param>
		/// <param name="getInvoiceUrl">払込票印字データ取得URL</param>
		public ScoreApiSetting(
			Action<IHttpApiRequestData, string> onAfterRequest,
			Action<IHttpApiRequestData> onBeforeRequest,
			Action<IHttpApiRequestData, string, Exception> onRequestError,
			Action<HttpWebRequest> onExtendWebRequest,
			Encoding apiEncoding,
			string basicUserId,
			string basicPassword,
			int httpTimeOutMiriSecond,
			string orderRegisterUrl,
			string orderModifyUrl,
			string orderCancelUrl,
			string getAuthResultUrl,
			string deliveryRegisterUrl,
			string getInvoiceUrl)
		{
			OnAfterRequest = onAfterRequest;
			OnBeforeRequest = onBeforeRequest;
			OnRequestError = onRequestError;
			OnExtendWebRequest = onExtendWebRequest;
			this.ApiEncoding = apiEncoding;
			this.BasicUserId = basicUserId;
			this.BasicPassword = basicPassword;
			this.HttpTimeOutMiriSecond = httpTimeOutMiriSecond;
			this.OrderRegisterUrl = orderRegisterUrl;
			this.OrderModifyUrl = orderModifyUrl;
			this.OrderCancelUrl = orderCancelUrl;
			this.GetAuthResultUrl = getAuthResultUrl;
			this.DeliveryRegisterUrl = deliveryRegisterUrl;
			this.GetInvoiceUrl = getInvoiceUrl;
		}

		/// <summary>APIエンコード</summary>
		public Encoding ApiEncoding { get; }
		/// <summary>基本認証ユーザーID</summary>
		public string BasicUserId { get; }
		/// <summary>基本認証パスワード</summary>
		public string BasicPassword { get; }
		/// <summary>HTTPタイムアウト（ミリ秒）</summary>
		public int HttpTimeOutMiriSecond { get; }
		/// <summary>取引登録URL</summary>
		public string OrderRegisterUrl { get; }
		/// <summary>与信審査結果取得URL</summary>
		public string GetAuthResultUrl { get; }
		/// <summary>取引修正URL</summary>
		public string OrderModifyUrl { get; }
		/// <summary>取引キャンセルURL</summary>
		public string OrderCancelUrl { get; }
		/// <summary>発送情報登録URL</summary>
		public string DeliveryRegisterUrl { get; }
		/// <summary>請求書印字データ取得URL</summary>
		public string GetInvoiceUrl { get; set; }
		/// <summary>
		/// リクエスト実行前処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData> OnBeforeRequest { get; }
		/// <summary>
		/// リクエスト実行後処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエスト結果が入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData, string> OnAfterRequest { get; }
		/// <summary>
		/// リクエスト実行時エラー処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエストしたURL
		/// 第三引数：発生したExceptionが入っているのでログとかに利用
		/// </summary>
		public Action<IHttpApiRequestData, string, Exception> OnRequestError { get; }
		/// <summary>
		/// Webリクエストの拡張処理を設定
		/// </summary>
		/// <param name="method">
		/// WebRequest拡張処理
		/// 第一引数：WebRequestがわたってくるので好きにプロパティやHttpヘッダ、タイムアウト値等を設定してください
		/// </param>
		public Action<HttpWebRequest> OnExtendWebRequest { get; }
	}
}
