/*
=========================================================================================================
  Module      : FLAPS商品情報サービスクラス(ProductApiService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Linq;

namespace w2.App.Common.Flaps.Product
{
	/// <summary>
	/// FLAPS商品情報サービス
	/// </summary>
	public class ProductApiService : FlapsApiServiceBase
	{
		/// <summary>
		/// 商品情報の取得
		/// </summary>
		/// <param name="requestXml">リクエスト用XML</param>
		internal static ProductResult Get(XDocument requestXml)
		{
			var result = GetResult<ProductResult>(requestXml, Constants.FLAPS_API_NAME_PRODUCT);
			return result;
		}
	}
}
