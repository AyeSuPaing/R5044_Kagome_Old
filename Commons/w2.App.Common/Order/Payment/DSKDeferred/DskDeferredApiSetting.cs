/*
=========================================================================================================
  Module      : DSK後払いAPI用設定(DSKDeferredApiSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.DSKDeferred
{
	/// <summary>
	///  DSK後払いAPI用設定
	/// </summary>
	public class DskDeferredApiSetting
	{
		/// <summary>APIエンコード</summary>
		public Encoding ApiEncoding { get; set; }
		/// <summary>注文情報登録URL</summary>
		public string UrlOrderRegister { get; set; }
		/// <summary>注文情報修正URL</summary>
		public string UrlOrderModify { get; set; }
		/// <summary>与信審査結果取得URL</summary>
		public string UrlGetAuthResult { get; set; }
		/// <summary>注文キャンセルURL</summary>
		public string UrlOrderCancel { get; set; }
		/// <summary>請求書印字データ取得URL</summary>
		public string UrlGetinvoicePrintData { get; set; }
		/// <summary>発送情報登録URL</summary>
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
	}
}
