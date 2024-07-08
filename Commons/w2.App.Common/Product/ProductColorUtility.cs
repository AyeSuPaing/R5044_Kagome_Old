/*
=========================================================================================================
  Module      : 商品カラーユーティリティクラス(ProductColorUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.DataCacheController;

namespace w2.App.Common.Product
{
	/// <summary>
	/// 商品カラーユーティリティークラス
	/// </summary>
	public class ProductColorUtility
	{
		/// <summary>
		/// 商品カラー画像のファイルのURLを取得
		/// </summary>
		/// <param name="productColorId">商品カラーID</param>
		/// <returns>URL</returns>
		public static string GetColorImageUrl(string productColorId)
		{
			var productColor = GetProductColor(productColorId);
			return (productColor != null) ? productColor.Url : "";
		}

		/// <summary>
		/// 商品カラーの表示名を取得
		/// </summary>
		/// <param name="productColorId">商品カラーID</param>
		/// <returns>商品カラー表示名</returns>
		public static string GetColorImageDispName(string productColorId)
		{
			var productColor = GetProductColor(productColorId);
			return (productColor != null) ? productColor.DispName : "";
		}

		/// <summary>
		/// 商品カラーを取得する
		/// </summary>
		/// <param name="productColorId">商品カラーId</param>
		/// <returns>商品カラー</returns>
		public static ProductColor GetProductColor(string productColorId)
		{
			var productColor = GetProductColorList().FirstOrDefault(color => (color.Id == productColorId));
			return productColor;
		}

		/// <summary>
		/// 商品カラー一覧を取得
		/// </summary>
		/// <returns>商品カラー一覧</returns>
		public static ProductColor[] GetProductColorList()
		{
			return DataCacheControllerFacade.GetProductColorCacheController().GetProductColorList();
		}
	}
}
