/*
=========================================================================================================
  Module      : カートサービス (CartService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Data;

namespace w2.Domain.Cart
{
	/// <summary>
	/// カートサービス
	/// </summary>
	public class CartService : ServiceBase
	{
		/// <summary>
		/// カート商品情報取得（未削除商品）
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>カート商品情報</returns>
		public DataView GetProductForDeleteCheck(string cartId)
		{
			using (var repository = new CartRepository())
			{
				var dv = repository.GetProductForDeleteCheck(cartId);
				return dv;
			}
		}
	}
}