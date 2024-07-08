/*
=========================================================================================================
  Module      : Neweb Pay Response Result(NewebPayResponseResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// Neweb Pay Response Result Class
	/// </summary>
	public class NewebPayResponseResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public NewebPayResponseResult()
		{
			this.Response = new NewebPayResponse();
		}

		/// <summary>
		/// Set response
		/// </summary>
		/// <param name="response">Response</param>
		public void SetResponse(NewebPayResponse response)
		{
			this.Response = response;
		}

		/// <summary>
		/// Set response
		/// </summary>
		/// <param name="response">Response</param>
		public void SetResponse(string response)
		{
			var requests = HttpUtility.ParseQueryString(response);
			var parameters = requests.AllKeys.ToDictionary(item => item, item => requests[item]);
			this.Response = ConvertToResponseObject(parameters);
		}

		/// <summary>
		/// Set response
		/// </summary>
		/// <param name="response">Response</param>
		public void SetResponse(Dictionary<string, string> response)
		{
			this.Response = ConvertToResponseObject(response);
		}

		/// <summary>
		/// Convert to response object
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <returns>NewebPay response</returns>
		private NewebPayResponse ConvertToResponseObject(Dictionary<string, string> parameters)
		{
			var response = new NewebPayResponse
			{
				Status = GetKeyValue(parameters, NewebPayConstants.PARAM_STATUS),
				Amount = int.Parse(GetKeyValue(parameters, NewebPayConstants.PARAM_AMOUNT)),
				TradeNo = GetKeyValue(parameters, NewebPayConstants.PARAM_TRADE_NO),
				MerchantOrderNo = GetKeyValue(parameters, NewebPayConstants.PARAM_MERCHANT_ORDER_NO),
				MerchantID = GetKeyValue(parameters, NewebPayConstants.PARAM_MERCHANT_ID),
			};
			return response;
		}

		/// <summary>
		/// Get key value
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <param name="key">Key</param>
		/// <returns>The value</returns>
		private string GetKeyValue(Dictionary<string, string> parameters, string key)
		{
			var value = parameters.ContainsKey(key)
				? parameters[key]
				: string.Empty;
			return value;
		}

		/// <summary>Response</summary>
		public NewebPayResponse Response { get; set; }
		/// <summary>Is success</summary>
		public bool IsSuccess
		{
			get
			{
				return (this.Response.Status == NewebPayConstants.CONST_STATUS_SUCCESS);
			}
		}
	}
}
