/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文情報更新レスポンスクラス(TriLinkAfterPayUpdateResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文情報更新レスポンスクラス
	/// </summary>
	public class TriLinkAfterPayUpdateResponse : ResponseBase
	{
		#region プロパティ
		/// <summary>問合せ番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ORDER_CODE)]
		public string OrderCode { get; set; }
		/// <summary>審査結果</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_AUTHORIZATION)]
		public PaymentTriLinkAfterPayResponseAuthorizationData Authorization { get; set; }
		/// <summary>ログ区分</summary>
		public override string NameForLog
		{
			get { return "UpdateOrder"; }
		}
		/// <summary>レスポンス結果</summary>
		public override bool ResponseResult
		{
			get { return this.IsHttpStatusCodeOK; }
		}
		#endregion
	}
}
