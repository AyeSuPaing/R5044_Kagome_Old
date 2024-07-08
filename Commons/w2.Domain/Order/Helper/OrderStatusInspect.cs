/*
=========================================================================================================
  Module      : 注文状態調査のためのヘルパクラス (OrderStatusInspect.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// 注文状態調査のためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class OrderStatusInspect
	{
		#region +InspectReturnAllItems 注文商品が全て返品されているか？
		/// <summary>
		/// 注文商品が全て返品されているか？
		/// </summary>
		/// <param name="service">サービス</param>
		/// <param name="relatedOrders">注文（返品交換含む）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>全て返品している：true、全て返品していない：false</returns>
		internal bool InspectReturnAllItems(OrderService service, OrderModel[] relatedOrders, SqlAccessor accessor)
		{
			// 全返品注文が返品交換ステータスが「返品交換完了」？
			var returnOrders = relatedOrders.Where(o => o.IsReturnOrder);
			if (returnOrders.All(o => o.IsAlreadyReturnExchangeCompleted))
			{
				// 元注文+交換注文の商品を取得
				var items = new List<OrderItemModel>();
				var relatedOrderItems =
					relatedOrders.Where(o => (o.IsNotReturnExchangeOrder || o.IsExchangeOrder))
						.SelectMany(o => o.Shippings.SelectMany(s => s.Items)).ToArray();
				foreach (var item in relatedOrderItems)
				{
					// 商品数量を合算
					if (items.Any(i => (i.ShopId == item.ShopId) && (i.ProductId == item.ProductId) && (i.VariationId == item.VariationId)))
					{
						var itemTemp = items.First(i => (i.ShopId == item.ShopId) && (i.ProductId == item.ProductId) && (i.VariationId == item.VariationId));
						itemTemp.ItemQuantity = itemTemp.ItemQuantity + item.ItemQuantity;
					}
					else
					{
						// ※メソッド内でインスタンスのデータ変更したくないので、クローン作成
						items.Add((OrderItemModel)item.Clone());
					}
				}

				// 全商品が返品されている場合は、trueを返す
				foreach (var item in items)
				{
					var returnItems =
						returnOrders.SelectMany(o => o.Shippings.SelectMany(s => s.Items)).ToArray();
					var itemQuantity = returnItems.Where(i => (i.ShopId == item.ShopId) && (i.ProductId == item.ProductId) && (i.VariationId == item.VariationId)).Sum(i => i.ItemQuantity);
					item.ItemQuantity = item.ItemQuantity + itemQuantity;
				}
				if (items.Any(i => i.ItemQuantity > 0) == false) return true;
			}

			return false;
		}
		#endregion
	}
}
