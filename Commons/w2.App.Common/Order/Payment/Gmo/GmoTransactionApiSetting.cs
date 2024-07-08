/*
=========================================================================================================
  Module      : Gmo transaction api setting (GmoTransactionApiSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
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
	public class GmoTransactionApiSetting
	{
		/// <summary>APIエンコード</summary>
		public Encoding ApiEncoding { get; set; }

		/// <summary>基本認証ユーザーID</summary>
		public string BasicUserId { get; set; }

		/// <summary>基本認証パスワード</summary>
		public string BasicPassword { get; set; }

		/// <summary>HTTPタイムアウト（ミリ秒）</summary>
		public int HttpTimeOutMiriSecond { get; set; }

		/// <summary>GMOに接続するための認証ID</summary>
		public string AuthenticationId { get; set; }

		/// <summary>店舗コード</summary>
		public string ShopCode { get; set; }

		/// <summary>接続パスワード</summary>
		public string ConnectPassword { get; set; }

		/// <summary>取引登録APIのURL</summary>
		public string UrlTransactionRegister { get; set; }

		/// <summary>枠保証ステータス取得APIのURL</summary>
		public string UrlFrameGuaranteeGetStatus { get; set; }

		/// <summary>枠保証登録APIのURL</summary>
		public string UrlFrameGuaranteeRegister { get; set; }

		/// <summary>枠保証更新APIのURL</summary>
		public string UrlFrameGuaranteeUpdate { get; set; }

		/// <summary>与信結果取得APIのURL</summary>
		public string UrlGetCreditResult { get; set; }

		/// <summary>取引変更・キャンセルAPIのURL</summary>
		public string UrlTransactionModifyCancel { get; set; }

		/// <summary>請求確認APIのURL</summary>
		public string UrlBillingConfirm { get; set; }

		/// <summary>請求変更・キャンセルAPIのURL</summary>
		public string UrlBillingModifyCancel { get; set; }

		/// <summary>請求削減APIのURL</summary>
		public string UrlReducedClaims { get; set; }

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
