/*
=========================================================================================================
  Module      : Recalculation Utility(RecalculationUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Product;
using w2.Domain.Point;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Recalculation Utility
	/// </summary>
	[Serializable]
	public class RecalculationUtility : BotChanUtility
	{
		/// <summary>
		/// Get Add Cart Kbn
		/// </summary>
		/// <param name="orderDivision">Order Division</param>
		/// <returns>Add Cart Kbn</returns>
		public static Constants.AddCartKbn GetAddCartKbn(string orderDivision)
		{
			switch (orderDivision)
			{
				case Constants.FLG_ADD_CART_KBN_FIXEDPURCHASE:
					return Constants.AddCartKbn.FixedPurchase;

				case Constants.FLG_ADD_CART_KBN_NORMAL:
					return Constants.AddCartKbn.Normal;

				default:
					return Constants.AddCartKbn.GiftOrder;
			}
		}

		/// <summary>
		/// Get Point Use Price
		/// </summary>
		/// <param name="usePoint">Use Point</param>
		/// <returns>Point Use Price</returns>
		public static decimal GetPointUsePrice(decimal usePoint)
		{
			var pointUsePrice = new PointService().GetOrderPointUsePrice(
				usePoint,
				Constants.FLG_POINT_POINT_KBN_BASE);
			return pointUsePrice;
		}
	}
}
