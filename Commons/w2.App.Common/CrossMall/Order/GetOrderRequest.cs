/*
=========================================================================================================
  Module      : 注文取得するリクエストクラス (GetOrderRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.CrossMall.Order
{
	/// <summary>
	/// 注文取得するリクエストクラス
	/// </summary>
	public class GetOrderRequest : CrossMallRequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public GetOrderRequest(string orderId)
			: base()
		{
			this.OrderCode = orderId;
			this.Params.Add(CrossMallConstants.CONST_PARAM_KEY_NAME_ORDER_CODE, OrderCode);
			this.RequestRawUrl = Constants.CROSS_MALL_GET_ORDER_API_URL;
		}
		#endregion

		#region プロパティ
		/// <summary> 注文番号 </summary>
		public string OrderCode { get; set; }
		#endregion
	}
}
