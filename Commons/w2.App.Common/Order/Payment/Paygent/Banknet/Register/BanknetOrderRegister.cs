/*
=========================================================================================================
  Module      : Paygent Banknet Order Register (BanknetOrderRegister.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Order.Payment.Paygent.Banknet.Register.Dto;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Banknet.Register
{
	/// <summary>
	/// Paygent Banknet order register
	/// </summary>
	public class BanknetOrderRegister
	{
		/// <summary>銀行ネット決済ASP申込電文</summary>
		private const string PAYGENT_CVS_API_NAME = "銀行ネット決済ASP申込電文";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cart">The cart information</param>
		/// <param name="order">The order information</param>
		public BanknetOrderRegister(CartObject cart, Hashtable order)
		{
			var postBody = new BanknetOrderRegisterPostBody(cart, order);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <returns>Result</returns>
		public BanknetOrderRegisterResult Register()
		{
			var response = new PaygentApiService(
					PAYGENT_CVS_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<BanknetOrderRegisterResponseDataset>();
			var result = new BanknetOrderRegisterResult(response);
			return result;
		}

		/// <summary>Post body</summary>
		public BanknetOrderRegisterPostBody PostBody { get; private set; }
	}
}
