/*
=========================================================================================================
  Module      : Rakuten Capture Request(RakutenCaptureRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten Capture Request
	/// </summary>
	[Serializable]
	public class RakutenCaptureRequest : RakutenRequestBase
	{
		/// <summary>
		/// Construction
		/// </summary>
		public RakutenCaptureRequest() : base()
		{
		}
	}
}
