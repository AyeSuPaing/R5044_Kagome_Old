/*
=========================================================================================================
  Module      : Elogit Response (ElogitResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit response
	/// </summary>
	public class ElogitResponse
	{
		/// <summary>Is error</summary>
		[JsonProperty("isError")]
		public bool IsError { set; get; }
		/// <summary>Is abort</summary>
		[JsonProperty("isAbort")]
		public bool IsAbort { set; get; }
		/// <summary>If history key</summary>
		[JsonProperty("IFHISTORYKEY")]
		public string IfHistoryKey { set; get; }
		/// <summary>Error info</summary>
		[JsonProperty("errorinfo")]
		public ErrorInfo ErrorInfo { set; get; }
	}

	/// <summary>
	/// Error Info
	/// </summary>
	public class ErrorInfo
	{
		/// <summary>Is error</summary>
		[JsonProperty("isError")]
		public string IsError { set; get; }
		/// <summary>Success</summary>
		[JsonProperty("success")]
		public string Success { set; get; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { set; get; }
		/// <summary>Error</summary>
		[JsonProperty("error")]
		public string Error { set; get; }
		/// <summary>Log text</summary>
		[JsonProperty("logtext")]
		public string LogText { set; get; }
		/// <summary>Status code</summary>
		[JsonProperty("statusCode")]
		public int StatusCode { set; get; }
	}
}
