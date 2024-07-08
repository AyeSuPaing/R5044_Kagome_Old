/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 出荷報告リクエストクラス(TriLinkAfterPayShipmentCompleteRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) 出荷報告リクエストクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayShipmentCompleteRequest : RequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="deliveryCompanyCode">運送会社コード</param>
		/// <param name="slipNumber">配送伝票番号</param>
		/// <param name="cardTranId">決済取引ID</param>
		public TriLinkAfterPayShipmentCompleteRequest(string deliveryCompanyCode, string slipNumber, string cardTranId)
		{
			this.DeliveryCompanyCode = deliveryCompanyCode;
			this.SlipNumber = slipNumber;
			this.RequestUrl = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE + "/" + cardTranId + "/shipment";
		}
		#endregion

		#region プロパティ
		/// <summary>配送運送会社コード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_DELIVERY_COMPANY_CODE)]
		public string DeliveryCompanyCode { get; set; }
		/// <summary>配送伝票番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_SLIP_NUMBER)]
		public string SlipNumber { get; set; }
		#endregion
	}
}
