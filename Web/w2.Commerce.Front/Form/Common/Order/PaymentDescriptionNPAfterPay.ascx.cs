/*
=========================================================================================================
  Module      : Payment Description NP After Pay(PaymentDescriptionNPAfterPay.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
 */
using System;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;

/// <summary>
/// Payment Description Np After Pay
/// </summary>
public partial class Form_Common_Order_PaymentDescriptionNPAfterPay : BaseUserControl
{
	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED)
		{
			var paymentInfo = DataCacheControllerFacade.GetPaymentCacheController()
				.Get(Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			if (paymentInfo != null)
			{
				var paymentTypeFee = (paymentInfo.PriceList.Length > 0)
					? paymentInfo.PriceList[0].PaymentPrice
					: 0m;
				this.PaymentTypeFee = CurrencyManager.ToPrice(paymentTypeFee);
				this.PaymentTypeMaxAmount = paymentInfo.UsablePriceMax.HasValue
					? CurrencyManager.ToPrice(paymentInfo.UsablePriceMax)
					: CurrencyManager.ToPrice(999999);
			}
		}
	}

	/// <summary>Payment Type Fee</summary>
	protected string PaymentTypeFee { get; set; }
	/// <summary>Payment Type Max Amount</summary>
	protected string PaymentTypeMaxAmount { get; set; }
}