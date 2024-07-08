/*
=========================================================================================================
  Module      : FLAPS商品在庫同期クラス (FlapsProductStockReplication.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Flaps.ProductStock
{
	/// <summary>
	/// FLAPS商品在庫同期
	/// </summary>
	[XmlRoot(ElementName = "FWS")]
	public class FlapsProductStockReplication : FlapsEntity
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FlapsProductStockReplication()
		{
			this.Method = "iPIS/CheckStockForEcSpSum";
			this.GoodsCodes = new string[] { };
			this.RequestDateTime = "";
			this.Request = ProductStockApiService.Get;
		}

		/// <summary>
		/// 取得API実行
		/// </summary>
		/// <returns>結果オブジェクト</returns>
		internal ProductStockResult Get()
		{
			var result = base.Get<ProductStockResult>();
			return result;
		}

		/// <summary>商品コード</summary>
		[XmlArray("Datas")]
		[XmlArrayItem("GoodsCode")]
		public string[] GoodsCodes { get; set; }
		/// <summary>差分取得日時(この日時よりも後にFLAPS側で更新された商品を取得する)</summary>
		[XmlElement(ElementName = "DateTime")]
		public string RequestDateTime { get; set; }
	}
}
