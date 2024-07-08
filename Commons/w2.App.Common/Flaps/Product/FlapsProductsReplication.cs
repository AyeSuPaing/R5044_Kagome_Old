/*
=========================================================================================================
 Module      : FLAPS注文同期クラス (FlapsProductsReplication.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Flaps.Product
{
	/// <summary>
	/// FLAPS注文同期クラス
	/// </summary>
	[XmlRoot(ElementName = "FWS")]
	public class FlapsProductsReplication : FlapsEntity
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FlapsProductsReplication()
		{
			this.Method = "iPIS/GetGoodsInfo";
			this.Records = "";
			this.DateThreshold = "";
			this.Request = ProductApiService.Get;
		}

		/// <summary>
		/// 取得API実行
		/// </summary>
		/// <returns>取得API実行結果オブジェクト</returns>
		public ProductResult Get()
		{
			var result = base.Get<ProductResult>();
			return result;
		}

		/// <summary>一度に取得するレコード数</summary>
		[XmlElement(ElementName = "Records")]
		public string Records { get; set; }
		/// <summary>差分取得日時(この日時よりも後にFLAPS側で更新された商品を取得する)</summary>
		[XmlElement(ElementName = "DateThreshold")]
		public string DateThreshold { get; set; }
	}
}
