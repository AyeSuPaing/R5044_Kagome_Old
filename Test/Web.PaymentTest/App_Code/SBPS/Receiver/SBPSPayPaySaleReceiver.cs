/*
=========================================================================================================
  Module      : SBPS PayPay Sale Receiver (SBPSPayPaySaleReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
/// SBPS PayPay sale receiver
/// </summary>
public class SBPSPayPaySaleReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="requestData">Request data</param>
	public SBPSPayPaySaleReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// Receive
	/// </summary>
	/// <returns>Response</returns>
	public override string Receive()
	{
		decimal? amountAuth = null;

		// 今のところ排他は考慮無し
		if (SBPSCreditAuthReceiver.TrackingIds.ContainsKey(m_requestData.TrackingId))
		{
			amountAuth = SBPSCreditAuthReceiver.TrackingIds[m_requestData.TrackingId];
		}

		// 見つからなかった場合も成功で返すことにする
		if (amountAuth == null) return GetResponseXmlBase(true);

		if ((m_requestData.AmountSales != null)
			&& (m_requestData.AmountSales > amountAuth.Value))
		{
			return GetResponseXmlBase(false);
		}

		return GetResponseXmlBase(true);
	}
}
