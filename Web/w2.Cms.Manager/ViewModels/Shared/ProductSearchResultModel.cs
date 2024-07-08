/*
=========================================================================================================
  Module      : 商品検索結果ビューモデル(ProductSearchResultModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>
	/// 商品検索結果ビューモデル
	/// </summary>
	public class ProductSearchResultModel
	{
		/// <summary>
		/// JSONに変換
		/// </summary>
		/// <returns>JSON</returns>
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>バリエーション名</summary>
		public string VariationName { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>カテゴリ名</summary>
		public string CategoryName { get; set; }
		/// <summary>価格</summary>
		public string Price { get; set; }
		/// <summary>販売開始日</summary>
		public string SellStartDate { get; set; }
		/// <summary>総件数HTML</summary>
		public string CountHtml { get; set; }
		/// <summary>コントローラー</summary>
		public string Controller { get; set; }
	}
}