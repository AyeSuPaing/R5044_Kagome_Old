/*
=========================================================================================================
  Module      : Payment SBPS Paypay Cancel Response Data (PaymentSBPSPaypayCancelResponseData.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// Payment SBPS Paypay cancel response data
	/// </summary>
	public class PaymentSBPSPaypayCancelResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">SBPS settings</param>
		internal PaymentSBPSPaypayCancelResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
