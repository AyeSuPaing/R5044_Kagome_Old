/*
=========================================================================================================
  Module      : Exchange Token (ExchangeToken.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Exchange token
	/// </summary>
	public class ExchangeToken
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public ExchangeToken()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authResult">Authentication result</param>
		public ExchangeToken(AuthenticationResult authResult)
		{
			this.AccessToken = authResult.AccessToken;
			this.TokenType = authResult.TokenType;
			this.ExpiresOn = authResult.ExpiresOn;
			this.ExtendedExpiresOn = authResult.ExtendedExpiresOn;
			this.IsExtendedLifeTimeToken = authResult.IsExtendedLifeTimeToken;
			this.Scopes = authResult.Scopes;
		}
		#endregion

		#region Properties
		/// <summary>Access token</summary>
		public string AccessToken { get; set; }
		/// <summary>Token type</summary>
		public string TokenType { get; set; }
		/// <summary>Expires on</summary>
		public DateTimeOffset ExpiresOn { get; set; }
		/// <summary>Extended expires on</summary>
		public DateTimeOffset ExtendedExpiresOn { get; set; }
		/// <summary>Is extended life time token</summary>
		public bool IsExtendedLifeTimeToken { get; set; }
		/// <summary>Scopes</summary>
		public IEnumerable<string> Scopes { get; set; }
		#endregion
	}
}
