/*
=========================================================================================================
  Module      : ZcomAPI連携ファサードインターフェース (IZcomApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;
using w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth;
using w2.App.Common.Order.Payment.GMO.Zcom.Direct;
using w2.App.Common.Order.Payment.GMO.Zcom.Sales;

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// ZcomAPI連携ファサードインターフェース
	/// </summary>
	public interface IZcomApiFacade
	{
		/// <summary>
		/// 決済実施
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		ZcomDirectResponse DirectPayment(ZcomDirectRequest request);

		/// <summary>
		/// 取消し実施
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		ZcomCancelResponse CancelPayment(ZcomCancelRequest request);

		/// <summary>
		/// 実売上実施
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		ZcomSalesResponse SalesPayment(ZcomSalesRequest request);

		/// <summary>
		/// Zcom credit check auth
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Response</returns>
		ZcomCheckAuthResponse ZcomCreditCheckAuth(ZcomCheckAuthRequest request);
	}
}
