/*
=========================================================================================================
  Module      : 決済種別情報基底ページ(PaymentPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Domain.Payment;

/// <summary>
/// 決済種別情報基底ページ
/// </summary>
public class PaymentPage : BasePage
{
	/// <summary>
	/// 決済警告メッセージ作成
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="paymentId">決済種別ID</param>
	protected string CreatePaymentAlertMessage(string shopId, string paymentId)
	{
		if (string.IsNullOrEmpty(Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)) return "";

		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			var paymentProvisionalCreditCard = new PaymentService().Get(
				shopId,
				Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
			return string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_ALERT_FOR_CREDIT),
				paymentProvisionalCreditCard.PaymentId,
				paymentProvisionalCreditCard.PaymentName);
		}
		else if (paymentId == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
		{
			var paymentCreditCard = new PaymentService().Get(shopId, Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			return string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_ALERT_FOR_PROVISIONAL_CREDIT),
				paymentCreditCard.PaymentId,
				paymentCreditCard.PaymentName);
		}
		return "";
	}
}