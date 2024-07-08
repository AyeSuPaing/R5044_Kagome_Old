/*
=========================================================================================================
  Module      : 商品同梱 同梱商品テーブルモデル (ProductBundleItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.ProductBundle
{
	/// <summary>
	/// 商品同梱 同梱商品テーブルモデル
	/// </summary>
	public partial class ProductBundleItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>初回のみ同梱か</summary>
		public bool IsOrderedProductExcept
		{
			get { return (this.OrderedProductExceptFlg == Constants.FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_EXCEPT); }
		}
		#endregion
	}
}
