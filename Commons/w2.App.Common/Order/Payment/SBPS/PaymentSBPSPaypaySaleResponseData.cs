/*
=========================================================================================================
  Module      : Payment SBPS Paypay Sale Response Data (PaymentSBPSPaypaySaleResponseData.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// Payment SBPS Paypay sale response data
	/// </summary>
	public class PaymentSBPSPaypaySaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">SBPS settings</param>
		internal PaymentSBPSPaypaySaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
