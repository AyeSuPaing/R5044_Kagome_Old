/*
=========================================================================================================
  Module      : Base Elogit Request (BaseElogitRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Base elogit request
	/// </summary>
	public class BaseElogitRequest
	{
		/// <summary>Syori kbn</summary>
		[JsonProperty("SYORIKBN")]
		public string SyoriKbn { set; get; }
		/// <summary>Target type</summary>
		[JsonProperty("TARGETTYPE")]
		public string TargetType { set; get; }
		/// <summary>Code</summary>
		[JsonProperty("CODE")]
		public string Code { set; get; }
		/// <summary>User id</summary>
		[JsonProperty("USERID")]
		public string UserId { set; get; }
		/// <summary>Password</summary>
		[JsonProperty("PASSWORD")]
		public string Password { set; get; }
	}
}
