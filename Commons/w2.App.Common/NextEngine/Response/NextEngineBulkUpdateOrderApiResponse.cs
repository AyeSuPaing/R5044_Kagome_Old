/*
=========================================================================================================
  Module      : Next Engine bulk update order api response (NextEngineBulkUpdateOrderApiResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.NextEngine.Response
{
	/// <summary>
	/// Next Engine bulk update order api response
	/// </summary>
	public class NextEngineBulkUpdateOrderApiResponse: NextEngineApiResponseBase
	{
		/// <summary>Data</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_DATA)]
		public string Data { get; set; }
		/// <summary>Message</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_MESSAGE)]
		public new object Message { get; set; }
	}

	/// <summary>
	/// Next Engine bulk update order message api response
	/// </summary>
	public class NextEngineBulkUpdateOrderMessageApiResponse
	{
		/// <summary>Receive order id</summary>
		[JsonProperty(NextEngineConstants.PARAM_SEARCH_ORDER_ORDER_ID)]
		public string ReceiveOrderId { get; set; }
		/// <summary>Code</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_CODE)]
		public string Code { get; set; }
		/// <summary>Message</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_MESSAGE)]
		public string Message { get; set; }
	}
}
