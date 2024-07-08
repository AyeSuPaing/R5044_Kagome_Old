/*
=========================================================================================================
  Module      : Atm Order Register(AtmOrderRegister.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Order.Payment.Paygent.ATM.Register.Dto;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.ATM.Register
{
	/// <summary>
	/// Atm order register
	/// </summary>
	internal class AtmOrderRegister
	{
		/// <summary>ATM決済申込電文</summary>
		private const string PAYGENT_ATM_API_NAME = "ATM決済申込電文";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cart">The cart information</param>
		/// <param name="order">The order information</param>
		public AtmOrderRegister(CartObject cart, Hashtable order)
		{
			var postBody = new AtmOrderRegisterPostBody(cart, order);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <returns>Result</returns>
		public AtmOrderRegisterResult Register()
		{
			var response = new PaygentApiService(
				PAYGENT_ATM_API_NAME,
				this.PostBody.GenerateKeyValues())
					.GetResult<AtmOrderRegisterResponse>();
			var result = new AtmOrderRegisterResult(response);
			return result;
		}

		/// <summary>Post body</summary>
		public AtmOrderRegisterPostBody PostBody { get; private set; }
	}
}
