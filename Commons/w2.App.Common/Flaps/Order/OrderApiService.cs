/*
=========================================================================================================
  Module      : FLAPS受注情報サービスクラス(OrderApiService.cs)
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
	public class OrderApiService : FlapsApiServiceBase
	{
		/// <summary>
		/// 受注情報の取得
		/// </summary>
		/// <param name="requestXml">リクエスト用XML</param>
		public static OrderResult Get(XDocument requestXml)
		{
			var result = GetResult<OrderResult>(requestXml, Constants.FLAPS_API_NAME_ORDER);
			return result;
		}
	}
}