/*
=========================================================================================================
  Module      : アトディーネAPI用設定(AtodeneApiSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE
{
	/// <summary>
	/// アトディーネAPI用設定
	/// </summary>
	public class AtodeneApiSetting
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

		/// <summary>出荷報告URL</summary>
		public string UrlShipment { get; set; }

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
