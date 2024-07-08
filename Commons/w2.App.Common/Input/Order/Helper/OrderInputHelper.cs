/*
=========================================================================================================
  Module      : OrderInputヘルパー(OrderInputHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Input.Order.Helper
{
	/// <summary>
	/// OrderInputヘルパー
	/// </summary>
	public class OrderInputHelper
	{
		/// <summary>
		/// Set CanDelete Property To OrderItemInput（明細が2つ以上のときに削除OKとする）
		/// </summary>
		/// <param name="orderItems">Order item</param>
		/// <returns>Order items</returns>
		public static OrderItemInput[] SetCanDeletePropertyToOrderItemInput(IEnumerable<OrderItemInput> orderItems)
		{
			var listItems = orderItems.ToList();
			var itemSetCount = listItems.Where(item => item.IsProductSet).GroupBy(
				item => new
				{
					SetId = item.ProductSetId,
					SetNo = item.ProductSetNo
				}).Count();
			var itemCount = listItems.Count(item => (item.IsProductSet == false));
			listItems.ForEach(item => item.CanDelete = ((itemCount + itemSetCount) > 1));

			return listItems.ToArray();
		}
	}
}
