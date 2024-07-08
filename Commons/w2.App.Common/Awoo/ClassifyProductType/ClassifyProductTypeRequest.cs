/*
=========================================================================================================
  Module      : ClassifyProductTypeリクエストデータ (ClassifyProductTypeRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;

namespace w2.App.Common.Awoo.ClassifyProductType
{
	/// <summary>
	/// ClassifyProductTypeリクエストデータ
	/// </summary>
	[Serializable]
	public class ClassifyProductTypeRequest : AwooApiPostRequestBase
	{
		/// <summary>productType</summary>
		[JsonProperty("productType")]
		public string ProductType { get; set; }
		/// <summary>sort</summary>
		[JsonProperty("sort")]
		public string Sort { get; set; }
	}
}
