/*
=========================================================================================================
  Module      : カート 注文拡張項目クラス(CartOrderExtendItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Cart
{
	/// <summary>
	/// カート 注文拡張項目クラス
	/// </summary>
	[Serializable]
	public class CartOrderExtendItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartOrderExtendItem()
		{
			this.Value = string.Empty;
			this.IsFixedPurchaseTakeOverNext = true;
		}

		/// <summary>入力内容</summary>
		public string Value { get; set; }
		/// <summary>次回の定期注文に引き継ぐかどうか</summary>
		public bool IsFixedPurchaseTakeOverNext { get; set; }
	}
}