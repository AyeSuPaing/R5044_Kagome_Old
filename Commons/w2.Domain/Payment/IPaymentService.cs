/*
=========================================================================================================
  Module      : 決済種別サービスのインタフェース(IPaymentService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Payment
{
	/// <summary>
	/// 決済種別サービスのインタフェース
	/// </summary>
	public interface IPaymentService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>モデル</returns>
		PaymentModel Get(string shopId, string paymentId);

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		PaymentModel[] GetAll(string shopId);

		/// <summary>
		/// GetAllWithPrice（価格情報も含めた全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		PaymentModel[] GetAllWithPrice(string shopId);

		/// <summary>
		/// 取得（有効なもの全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		PaymentModel[] GetValidAll(string shopId);

		/// <summary>
		/// 取得（定期購入で有効なもの全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		PaymentModel[] GetValidAllForFixedPurchase(string shopId);

		/// <summary>
		/// 取得（既定のお支払方法で有効なもの全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		PaymentModel[] GetValidUserDefaultPayment(string shopId);

		/// <summary>
		/// 決済種別IDより決済種別名を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>決済種別名</returns>
		string GetPaymentName(string shopId, string paymentId);

		/// <summary>
		/// Get payment names by payment ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="paymentIds">Payment ids</param>
		/// <returns>Payment names</returns>
		string[] GetPaymentNamesByPaymentIds(string shopId, string[] paymentIds);

		/// <summary>
		/// Get valid payments
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>An array of payment models</returns>
		PaymentModel[] GetValidPayments(string shopId);
	}
}