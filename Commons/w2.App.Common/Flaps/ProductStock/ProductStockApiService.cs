/*
=========================================================================================================
  Module      : FLAPS商品在庫情報サービスクラス(ProductStockApiService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Linq;

namespace w2.App.Common.Flaps.ProductStock
{
	/// <summary>
	/// FLAPS商品在庫情報サービス
	/// </summary>
	public class ProductStockApiService : FlapsApiServiceBase
	{
		/// <summary>
		/// 商品在庫情報の取得
		/// </summary>
		/// <param name="requestXml">リクエストXML</param>
		public static ProductStockResult Get(XDocument requestXml)
		{
			var result = GetResult<ProductStockResult>(requestXml, Constants.FLAPS_API_NAME_PRODUCT_STOCK);
			return result;
		}
	}
}