/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) アクセストークンレスポンスクラス(TriLinkAfterPayAccessTokenResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) アクセストークンレスポンスクラス
	/// </summary>
	public class TriLinkAfterPayAccessTokenResponse : ResponseBase
	{
		#region プロパティ
		/// <summary>アクセストークン</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ACCESS_TOKEN)]
		public string AccessToken { get; set; }
		/// <summary>ログ区分</summary>
		public override string NameForLog
		{
			get { return "AccessToken"; }
		}
		/// <summary>レスポンス結果</summary>
		public override bool ResponseResult
		{
			get { return this.IsHttpStatusCodeOK; }
		}
		#endregion
	}
}
