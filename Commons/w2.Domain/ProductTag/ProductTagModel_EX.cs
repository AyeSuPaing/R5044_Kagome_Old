/*
=========================================================================================================
  Module      : 商品タグマスタモデル (ProductTagModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductTag
{
	/// <summary>
	/// 商品タグマスタモデル
	/// </summary>
	public partial class ProductTagModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>Product tag IDs</summary>
		public string[] ProductTagIds
		{
			get { return (string[])this.DataSource["product_tag_ids"]; }
			set { this.DataSource["product_tag_ids"] = value; }
		}
		/// <summary>Product tag values</summary>
		public string[] ProductTagValues
		{
			get { return (string[])this.DataSource["product_tag_values"]; }
			set { this.DataSource["product_tag_values"] = value; }
		}
		#endregion
	}
}
