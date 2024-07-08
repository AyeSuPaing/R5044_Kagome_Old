/*
=========================================================================================================
  Module      : Paygent CVS Order Register(CvsOrderRegister.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Order.Payment.Paygent.Cvs.Register.Dto;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Cvs.Register
{
	/// <summary>
	/// Paygent CVS Order Register
	/// </summary>
	public class OrderRegister
	{
		/// <summary>コンビニ決済（番号方式）</summary>
		private const string PAYGENT_CVS_API_NAME = "コンビニ決済（番号方式）申込電文";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cart">The cart information</param>
		/// <param name="order">The order information</param>
		public OrderRegister(CartObject cart, Hashtable order)
		{
			var postBody = new OrderRegisterPostBody(cart, order);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <returns>Result</returns>
		public OrderRegisterResult Register()
		{
			var response = new PaygentApiService(
					PAYGENT_CVS_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<OrderRegisterResponseDataset>();
			var result = new OrderRegisterResult(response);
			return result;
		}

		/// <summary>Post body</summary>
		public OrderRegisterPostBody PostBody { get; private set; }
	}
}
