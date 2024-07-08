/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) レスポンス基底クラス(ResponseBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) レスポンス基底クラス
	/// </summary>
	public abstract class ResponseBase
	{
		#region プロパティ
		/// <summary>レスポンスコード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_CODE)]
		public string Code { get; set; }
		/// <summary>メッセージ</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_MESSAGE)]
		public string Message { get; set; }
		/// <summary>エラーコード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ERROR_CODE)]
		public string ErrorCode { get; set; }
		/// <summary>エラー詳細</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ERRORS)]
		public List<PaymentTriLinkAfterPayResponseErrorData> Errors { get; set; }
		/// <summary>HTTPステータスコード 200</summary>
		public bool IsHttpStatusCodeOK
		{
			get { return this.Code == "200"; }
		}
		/// <summary>HTTPステータスコード 201</summary>
		public bool IsHttpStatusCodeCreated
		{
			get { return this.Code == "201"; }
		}
		/// <summary>HTTPステータスコード 400</summary>
		public bool IsHttpStatusCodeBadRequest
		{
			get { return this.Code == "400"; }
		}
		/// <summary>HTTPステータスコード 401</summary>
		public bool IsHttpStatusCodeUnauthorized
		{
			get { return this.Code == "401"; }
		}
		/// <summary>HTTPステータスコード 404</summary>
		public bool IsHttpStatusCodeNotFound
		{
			get { return this.Code == "404"; }
		}
		/// <summary>ログ区分</summary>
		public abstract string NameForLog { get; }
		/// <summary>レスポンス結果</summary>
		public abstract bool ResponseResult { get; }
		#endregion
	}
}
