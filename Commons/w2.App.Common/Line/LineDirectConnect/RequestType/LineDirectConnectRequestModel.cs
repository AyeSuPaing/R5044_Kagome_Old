/*
=========================================================================================================
  Module      : LINE直接連携モデル (LineDirectConnectRequestModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.LineDirectConnect.RequestType
{
	/// <summary>
	/// LINE直接連携モデル
	/// </summary>
	public class LineDirectConnectRequestModel : PostBodyBase
	{
		[JsonProperty(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_GRANT_TYPE)]
		public string GrantType
		{
			get { return Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_GRANT_TYPE; }
		}

		[JsonProperty(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CODE)]
		public string Code { get; set; }

		[JsonProperty(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_REDIRECT_URI)]
		public string RedirectUri
		{
			get { return Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_REDIRECT_URI; }
		}

		[JsonProperty(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_ID)]
		public string ClientId
		{
			get { return Constants.LINE_DIRECT_CONNECT_CLIENT_ID; }
		}

		[JsonProperty(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_SECRET)]
		public string ClientSecret
		{
			get { return Constants.LINE_DIRECT_CONNECT_CLIENT_SECRET; }
		}
	}
}