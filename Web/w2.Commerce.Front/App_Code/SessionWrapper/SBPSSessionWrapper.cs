/*
=========================================================================================================
  Module      : SBPSセッションラッパー (SBPSSessionWrapper.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using w2.Domain.Order;

namespace SessionWrapper
{
	/// <summary>
	/// SBPSセッションラッパー
	/// </summary>
	public static class SBPSSessionWrapper
	{
		/// <summary>セッションキー：処理中の注文情報</summary>
		private const string SESSION_KEY_ORDER_SBPS = "w2cFront_sbpsOrders";

		/// <summary>
		/// SBPSマルチ決済 / 決済変更保留注文を追加
		/// </summary>
		/// <param name="order">注文情報</param>
		public static void AddSbpsMultiPendingOrder(OrderModel order)
		{
			var dic = Session[SESSION_KEY_ORDER_SBPS] as Dictionary<string, OrderModel>
				?? new Dictionary<string, OrderModel>();

			dic[order.OrderId] = order;
			Session[SESSION_KEY_ORDER_SBPS] = dic;
		}

		/// <summary>
		/// SBPSマルチ決済 / 決済変更保留注文を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		public static OrderModel FindSBPSMultiPendingOrder(string orderId)
		{
			var dic = Session[SESSION_KEY_ORDER_SBPS] as Dictionary<string, OrderModel>;
			if (dic == null) return null;

			var order = dic.ContainsKey(orderId)
				? dic[orderId]
				: null;
			return order;
		}

		/// <summary>
		/// SBPSマルチ決済 / 決済変更保留注文を削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public static void RemoveSbpsMultiPendingOrder(string orderId)
		{
			var dic = Session[SESSION_KEY_ORDER_SBPS] as Dictionary<string, OrderModel>;
			if (dic == null) return;

			if (dic.ContainsKey(orderId)) dic.Remove(orderId);

			if (dic.Count == 0)
			{
				Session.Remove(SESSION_KEY_ORDER_SBPS);
			}
		}

		/// <summary>セッション</summary>
		private static HttpSessionState Session
		{
			get { return HttpContext.Current.Session; }
		}
	}
}
