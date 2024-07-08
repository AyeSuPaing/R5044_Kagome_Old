/*
=========================================================================================================
  Module      : FLAPS受注キャンセルサービスクラス(OrderCancellationApiService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Linq;

namespace w2.App.Common.Flaps.Order
{
	/// <summary>
	/// FLAPS受注情報サービス
	/// </summary>
	public class OrderCancellationApiService : FlapsApiServiceBase
	{
		/// <summary>
		/// 受注情報の取得
		/// </summary>
		/// <param name="requestXml">リクエスト用XML</param>
		public static OrderCancellationResult Get(XDocument requestXml)
		{
			var result = GetResult<OrderCancellationResult>(requestXml, Constants.FLAPS_API_NAME_ORDER);
			return result;
		}
	}
}