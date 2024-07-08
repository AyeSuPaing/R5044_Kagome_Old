/*
=========================================================================================================
  Module      : GMOセッションラッパー (GmoSessionWrapper.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
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
	/// GMOセッションラッパー
	/// </summary>
	public static class GmoSessionWrapper
	{
		/// <summary>セッションキー：処理中の注文情報</summary>
		private const string SESSION_KEY_ORDER = "w2cFront_gmoOrders";

		/// <summary>
		/// GMOマルチ決済 / 決済変更保留注文を追加
		/// </summary>
		/// <param name="order">注文情報</param>
		public static void AddGmoMultiPendingOrder(OrderModel order)
		{
			var dic = Session[SESSION_KEY_ORDER] as Dictionary<string, OrderModel>
				?? new Dictionary<string, OrderModel>();

			dic[order.OrderId] = order;
			Session[SESSION_KEY_ORDER] = dic;
		}

		/// <summary>
		/// GMOマルチ決済 / 決済変更保留注文を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		public static OrderModel FindGmoMultiPendingOrder(string orderId)
		{
			var dic = Session[SESSION_KEY_ORDER] as Dictionary<string, OrderModel>;
			if (dic == null) return null;

			var order = dic.ContainsKey(orderId)
				? dic[orderId]
				: null;
			return order;
		}

		/// <summary>
		/// GMOマルチ決済 / 決済変更保留注文を削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public static void RemoveGmoMultiPendingOrder(string orderId)
		{
			var dic = Session[SESSION_KEY_ORDER] as Dictionary<string, OrderModel>;
			if (dic == null) return;
			if (dic.ContainsKey(orderId)) dic.Remove(orderId);

			if (dic.Count == 0)
			{
				Session.Remove(SESSION_KEY_ORDER);
			}
		}

		/// <summary>セッション</summary>
		private static HttpSessionState Session
		{
			get { return HttpContext.Current.Session; }
		}
	}
}
