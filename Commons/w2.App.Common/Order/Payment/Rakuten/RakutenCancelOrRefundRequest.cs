/*
=========================================================================================================
  Module      : Rakuten CancelOrRefund Request(RakutenCancelOrRefundRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten CancelOrRefund Request
	/// </summary>
	[Serializable]
	public class RakutenCancelOrRefundRequest : RakutenRequestBase
	{
		/// <summary>
		/// Construction
		/// </summary>
		public RakutenCancelOrRefundRequest()
			: base()
		{ 
		}
	}
}
