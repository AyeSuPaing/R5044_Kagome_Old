/*
=========================================================================================================
  Module      : カートリポジトリ (CartRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Collections;

namespace w2.Domain.Cart
{
	/// <summary>
	/// カートリポジトリ
	/// </summary>
	internal class CartRepository : RepositoryBase
	{
		/// <summary>XMLファイル名</summary>
		private const string XML_KEY_NAME = "Cart";

		/// <summary>
		/// カート商品情報取得（未削除商品）
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>カート商品情報</returns>
		internal DataView GetProductForDeleteCheck(string cartId)
		{
			Hashtable htInput = new Hashtable
			{
				{Constants.FIELD_CART_CART_ID, cartId}
			};
			var dv = Get(XML_KEY_NAME, "GetProductForDeleteCheck", htInput);
			return dv;
		}
	}
}