/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 送り先区分(PaymentYamatoKaSendDiv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 送り先区分
	/// </summary>
	public enum PaymentYamatoKaSendDiv
	{
		/// <summary>別送：本人送り</summary>
		Myself,
		/// <summary>別送：本人以外送り</summary>
		AnotherShipping,
		/// <summary>同梱</summary>
		Bundle,
		/// <summary>SMS認証</summary>
		SmsAuth,
		/// <summary>SMS有効性</summary>
		SmsAvailable,
	}

	/// <summary>
	/// ヤマト決済(後払い) 送り先区分２
	/// </summary>
	public enum PaymentYamatoKaSendDiv2
	{
		/// <summary>別送</summary>
		Send,
		/// <summary>同梱</summary>
		Bundle,
		/// <summary>SMS連携決済</summary>
		SmsAuth,
	}
}
