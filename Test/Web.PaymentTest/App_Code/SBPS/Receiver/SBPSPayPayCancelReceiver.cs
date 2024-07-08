/*
=========================================================================================================
  Module      : SBPS PayPay Cancel Receiver (SBPSPayPayCancelReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
/// SBPS PayPay cancel receiver
/// </summary>
public class SBPSPayPayCancelReceiver : SBPSBaseCancelReceiver
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="requestData">Request data</param>
	public SBPSPayPayCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}
}
