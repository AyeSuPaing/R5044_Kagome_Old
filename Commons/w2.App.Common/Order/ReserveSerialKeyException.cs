/*
=========================================================================================================
  Module      : シリアルキー引当例外(ReserveSerialKeyException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order
{
	/// <summary>
	/// シリアルキー引当例外
	/// </summary>
	public class ReserveSerialKeyException : Exception
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="errorCartProduct">カート商品情報</param>
		public ReserveSerialKeyException(CartProduct errorCartProduct)
		{
			this.ErrorCartProduct = errorCartProduct;
		}
		#endregion

		#region プロパティ
		/// <summary>エラーになったカート商品情報</summary>
		public CartProduct ErrorCartProduct { get; private set; }
		#endregion
	}
}
