/*
=========================================================================================================
  Module      : EC Pay Response Result(ECPayResponseResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Specialized;
using System.Web;

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// EC Pay Response Result Class
	/// </summary>
	public class ECPayResponseResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ECPayResponseResult()
		{
			this.IsSuccess = false;
			this.IsExecReturnCsvApiSuccess = false;
			this.IsExecReturnHomeApiSuccess = false;
			this.Response = string.Empty;
			this.Parameters = new NameValueCollection();
			this.ErrorMessage = string.Empty;
			this.Code = string.Empty;
		}

		/// <summary>
		/// Set Response Data
		/// </summary>
		/// <param name="response">Response</param>
		public void SetResponseData(string response)
		{
			this.Response = response;

			ParseData();
		}

		/// <summary>
		/// Get Response Value
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Response Value</returns>
		public string GetResponseValue(string key)
		{
			var result = this.Parameters[key];
			return result;
		}

		/// <summary>
		/// Parse Data
		/// </summary>
		private void ParseData()
		{
			if (string.IsNullOrEmpty(this.Response)
				|| this.Response.Contains("<html")
				|| this.Response.Contains("<form")
				|| this.Response.Contains("<div"))
			{
				return;
			}

			var rawData = this.Response.Split('|');

			if (rawData.Length == 0) return;

			if (rawData.Length == 1)
			{
				this.Parameters = HttpUtility.ParseQueryString(rawData[0]);
				return;
			}

			this.Code = rawData[0];
			this.Parameters = HttpUtility.ParseQueryString(rawData[1]);
			this.ErrorMessage = (this.Code != ECPayConstants.CONST_CODE_SUCCESS)
				? rawData[1]
				: string.Empty;
			this.IsExecReturnHomeApiSuccess = ((this.Code == ECPayConstants.CONST_CODE_SUCCESS)
				&& (rawData[1] == "OK"));

			// Parse data for case this response is return Csv
			if ((string.IsNullOrEmpty(this.Code) == false)
				&& (rawData.Length == 2))
			{
				this.Parameters.Add(ECPayConstants.PARAM_RTN_MERCHANT_TRADE_NO, rawData[0]);
				this.Parameters.Add(ECPayConstants.PARAM_RTN_ORDER_NO, rawData[1]);
				this.IsExecReturnCsvApiSuccess = true;
			}
		}

		/// <summary>Response</summary>
		private string Response { get; set; }
		/// <summary>Code</summary>
		private string Code { get; set; }
		/// <summary>Parameters</summary>
		private NameValueCollection Parameters { get; set; }
		/// <summary>Is Success</summary>
		public bool IsSuccess { get; set; }
		/// <summary>Error Message</summary>
		public string ErrorMessage { get; set; }
		/// <summary>Is Exec Register Api Success</summary>
		public bool IsExecRegisterApiSuccess
		{
			get
			{
				return (this.IsSuccess
					&& (this.Code == ECPayConstants.CONST_CODE_SUCCESS));
			}
		}
		/// <summary>Is Exec Update Api Success</summary>
		public bool IsExecUpdateApiSuccess
		{
			get
			{
				return ((this.ErrorMessage == "OK")
					&& (this.Code == ECPayConstants.CONST_CODE_SUCCESS));
			}
		}
		/// <summary>Is Exec Return Home Api Success</summary>
		public bool IsExecReturnHomeApiSuccess { get; private set; }
		/// <summary>Is Exec Return Csv Api Success</summary>
		public bool IsExecReturnCsvApiSuccess { get; private set; }
	}
}
