/*
=========================================================================================================
  Module      : Order Register Utility(OrderRegisterUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Sql;
using w2.Domain.Point;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Order register utility
	/// </summary>
	[Serializable]
	public class OrderRegisterUtility : BotChanUtility
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

				default:
					return Constants.AddCartKbn.Normal;
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

		/// <summary>
		/// Get Cart
		/// </summary>
		/// <param name="cartId">Cart Id</param>
		/// <returns>Cart</returns>
		public static DataView GetCart(string cartId)
		{
			DataView dvResult = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Cart", "GetCart"))
			{
				var input = new Hashtable();
				input.Add(Constants.FIELD_CART_CART_ID, cartId);
				dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
			return dvResult;
		}

		/// <summary>
		/// Create card
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <returns>User Credit Card</returns>
		public static UserCreditCardModel CreateCard(string userId, string lastChanged)
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				CooperationId = userId,
				CooperationId2 = string.Empty,
				CardDispName = string.Empty,
				LastFourDigit = string.Empty,
				ExpirationMonth = string.Empty,
				ExpirationYear = string.Empty,
				AuthorName = string.Empty,
				DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				LastChanged = lastChanged,
				CompanyCode = string.Empty,
				CooperationType = string.Empty,
			};
			return userCreditCard;
		}
	}
}
