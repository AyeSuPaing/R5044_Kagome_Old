/*
=========================================================================================================
  Module      : 商品価格マスタモデル (ProductPriceModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductPrice
{
	/// <summary>
	/// 商品価格マスタモデル
	/// </summary>
	public partial class ProductPriceModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>Is set product price</summary>
		public bool IsSetProductPrice
		{
			get { return (bool)this.DataSource["is_set_product_price"]; }
			set { this.DataSource["is_set_product_price"] = value; }
		}
		/// <summary>会員ランクID</summary>
		public string MemberRankName
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME] = value; }
		}
		#endregion
	}
}
