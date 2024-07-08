/*
=========================================================================================================
  Module      : GetTagsRequest (GetTagsRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.Common.Helper;

namespace w2.App.Common.Awoo.GetTags
{
	/// <summary>
	/// GetTagsRequest
	/// </summary>
	public class GetTagsRequest : AwooApiGetRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GetTagsRequest()
		{
		}

		/// <summary>directions</summary>
		public RecommendDirectionType[] Directions
		{
			set { base.m_data["directions"] = string.Join(",", value.Select(v => v.ToText())); }
		}
		/// <summary>select</summary>
		public SelectRecommendInfoType[] Select
		{
			set { base.m_data["select"] = string.Join(",", value.Select(v => v.ToText())); }
		}
		/// <summary>limit</summary>
		public int Limit
		{
			set { base.m_data["limit"] = value.ToString(); }
		}
		/// <summary>productType</summary>
		public string ProductType
		{
			set { base.m_data["productType"] = value; }
		}
	}
}
