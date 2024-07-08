/*
=========================================================================================================
  Module      : Payment Paygent CVS (PaymentPaygentCvs.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// Payment Paygent CVS
	/// </summary>
	[Serializable]
	public class PaymentPaygentCvs : IPayment
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="conveniType">支払先</param>
		public PaymentPaygentCvs(string conveniType)
		{
			this.ConveniType = conveniType;
		}

		/// <summary>支払先</summary>
		public string ConveniType { get; private set; }
	}
}
