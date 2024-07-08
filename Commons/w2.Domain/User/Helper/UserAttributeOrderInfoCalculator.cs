/*
=========================================================================================================
  Module      : ユーザー属性受注情報カリキュレーター (UserAttributeOrderInfoCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.Domain.Order;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ユーザー属性受注情報カリキュレーター
	/// </summary>
	public class UserAttributeOrderInfoCalculator
	{
		/// <summary>インスタンス</summary>
		private static readonly UserAttributeOrderInfoCalculator m_instance = new UserAttributeOrderInfoCalculator();

		/// <summary>
		/// インスタンス取得
		/// </summary>
		public static UserAttributeOrderInfoCalculator GetInstance()
		{
			return m_instance;
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private UserAttributeOrderInfoCalculator()
		{
		}

		/// <summary>
		/// 計算
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>UserAttributeModel</returns>
		public UserAttributeModel Calculate(string userId)
		{
			// 集計してUserAttributeModel作成
			//	※購入日系は、キャンセルは除外、返品分も除外して算出
			//	※購入金額は、キャンセルは除外、返品分はマイナスとして算出（返品完了）
			//    ※注文基準→交換では「返品完了」で加算
			//    ※出荷基準→交換では交換注文「出荷完了」で加算
			//	※購入回数は、キャンセルは除外、返品分も除外して算出
	
			// 注文情報取得
			var uncancelledOrders = new OrderService().GetUncancelledOrdersByUserId(userId);
			var uncancelledParentOrders = uncancelledOrders.Where(o => o.IsNotReturnExchangeOrder).ToArray();

			// 注文金額(注文基準)用未キャンセル注文を抽出（通常注文 or 返品交換完了）
			var uncancelledOrdersForOrderAmountOrder = uncancelledOrders
				.Where(o => o.IsNotReturnExchangeOrder || o.IsAlreadyReturnExchangeCompleted).ToArray();
			// 注文金額(出荷基準)用未キャンセル注文を抽出（出荷済み or 返品かつ返品完了）
			var uncancelledOrdersForOrderAmountShip = uncancelledOrders
				.Where(o => (o.IsAlreadyShipped) || 
					o.IsReturnOrder && o.IsAlreadyReturnExchangeCompleted).ToArray();

			// 以下、ユーザー属性にセット
			var userAttributeModel = new UserAttributeModel();
			userAttributeModel.UserId = userId;
			// 初回購入日
			userAttributeModel.FirstOrderDate = (uncancelledParentOrders.Length > 0) ? uncancelledParentOrders[0].OrderDate : null;
			// ２回め購入日
			userAttributeModel.SecondOrderDate = (uncancelledParentOrders.Length > 1) ? uncancelledParentOrders[1].OrderDate : null;
			// 最終購入日
			userAttributeModel.LastOrderDate = (uncancelledParentOrders.Length > 0) ? uncancelledParentOrders[uncancelledParentOrders.Length - 1].OrderDate : null;
			// 累計購入金額（注文基準・全体）
			userAttributeModel.OrderAmountOrderAll = uncancelledOrdersForOrderAmountOrder
				.Sum(o => o.OrderPriceTotal);
			// 累計購入金額（注文基準・定期のみ）
			userAttributeModel.OrderAmountOrderFp = uncancelledOrdersForOrderAmountOrder
				.Where(o => o.IsFixedPurchaseOrder)
				.Sum(o => o.OrderPriceTotal);
			// 累計購入回数（注文基準・全体）
			userAttributeModel.OrderCountOrderAll = uncancelledParentOrders.Count();
			// 累計購入回数（注文基準・定期のみ）
			userAttributeModel.OrderCountOrderFp = uncancelledParentOrders.Count(o => o.IsFixedPurchaseOrder);
			// 累計購入金額（出荷基準・全体）
			userAttributeModel.OrderAmountShipAll = uncancelledOrdersForOrderAmountShip
				.Sum(o => o.OrderPriceTotal);
			// 累計購入金額（出荷基準・定期のみ）
			userAttributeModel.OrderAmountShipFp = uncancelledOrdersForOrderAmountShip
				.Where(o => o.IsFixedPurchaseOrder)
				.Sum(o => o.OrderPriceTotal);
			// 累計購入回数（出荷基準・全体）
			userAttributeModel.OrderCountShipAll = uncancelledParentOrders.Count(o => o.IsAlreadyShipped);
			// 累計購入回数（出荷基準・定期のみ）
			userAttributeModel.OrderCountShipFp = uncancelledParentOrders.Count(o => o.IsAlreadyShipped && o.IsFixedPurchaseOrder);

			return userAttributeModel;
		}
	}
}
