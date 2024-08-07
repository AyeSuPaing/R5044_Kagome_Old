﻿/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文登録レスポンスクラス(TriLinkAfterPayRegisterResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文登録レスポンスクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayRegisterResponse : ResponseBase
	{
		#region プロパティ
		/// <summary>問い合わせ番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ORDER_CODE)]
		public string OrderCode { get; set; }
		/// <summary>審査結果</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_AUTHORIZATION)]
		public PaymentTriLinkAfterPayResponseAuthorizationData Authorization { get; set; }
		/// <summary>ログ区分</summary>
		public override string NameForLog
		{
			get { return "RegisterOrder"; }
		}
		/// <summary>レスポンス結果</summary>
		public override bool ResponseResult
		{
			get { return this.IsHttpStatusCodeCreated; }
		}
		#endregion
	}
}
