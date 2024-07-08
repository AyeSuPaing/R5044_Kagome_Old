/*
=========================================================================================================
  Module      : 商品付帯情報選択値クラス(ProductOptionSelectedValue.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.Domain.Product.Helper
{
	/// <summary>
	/// 商品付帯情報選択値
	/// </summary>
	[JsonObject]
	public class ProductOptionSelectedValue
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductOptionSelectedValue()
		{
			this.Name = string.Empty;
			this.Type = string.Empty;
			this.Value = string.Empty;
		}

		/// <summary> 項目名 </summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary> 表示区分 </summary>
		[JsonProperty("type")]
		public string Type { get; set; }
		/// <summary> 選択値 </summary>
		[JsonProperty("value")]
		public string Value { get; set; }
	}
}
