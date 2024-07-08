/*
=========================================================================================================
  Module      : Elogit Request (ElogitRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit upload request
	/// </summary>
	public class ElogitUploadRequest : BaseElogitRequest
	{
		/// <summary>Api mode</summary>
		[JsonProperty("APIMODE")]
		public string ApiMode { set; get; }
		/// <summary>If category cd</summary>
		[JsonProperty("IFCATEGORYCD")]
		public string IfCategoryCd { set; get; }
		/// <summary>IF history key</summary>
		[JsonProperty("IFHISTORYKEY")]
		public string IfHistoryKey { set; get; }
	}

	/// <summary>
	/// Elogit download request
	/// </summary>
	public class ElogitDownloadRequest : BaseElogitRequest
	{
		/// <summary>Api mode</summary>
		[JsonProperty("APIMODE")]
		public string ApiMode { set; get; }
		/// <summary>If category cd</summary>
		[JsonProperty("IFCATEGORYCD")]
		public string IfCategoryCd { set; get; }
		/// <summary>Where condition</summary>
		[JsonProperty("WHERECONDITION")]
		public string WhereCondition { set; get; }
		/// <summary>IF history key</summary>
		[JsonProperty("IFHISTORYKEY")]
		public string IfHistoryKey { set; get; }
		/// <summary>Dl file nm</summary>
		[JsonProperty("DLFILENM")]
		public string DlFileNm { set; get; }
	}
}
