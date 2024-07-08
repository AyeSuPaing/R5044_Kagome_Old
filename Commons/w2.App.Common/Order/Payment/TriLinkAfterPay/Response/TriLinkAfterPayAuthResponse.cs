/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文審査レスポンスクラス(TriLinkAfterPayAuthResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文審査レスポンスクラス
	/// </summary>
	[Serializable]
	[JsonObject]
	public class TriLinkAfterPayAuthResponse : ResponseBase
	{
		#region プロパティ
		/// <summary>結果</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_RESULT)]
		public string Result { get; set; }
		/// <summary>承認番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ACCEPT_NUMBER)]
		public string AcceptNumber { get; set; }
		/// <summary>ログ区分</summary>
		public override string NameForLog
		{
			get { return "RegisterOrder"; }
		}
		/// <summary>レスポンス結果</summary>
		public override bool ResponseResult
		{
			get { return (this.IsHttpStatusCodeOK 
				&& Result == TriLinkAfterPayConstants.FLG_TW_AFTERPAY_AUTH_OK); }
		}
		#endregion
	}
}