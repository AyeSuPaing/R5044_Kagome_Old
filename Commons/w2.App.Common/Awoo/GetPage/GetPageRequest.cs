/*
=========================================================================================================
  Module      : GetPageRequest (GetPageRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper;

namespace w2.App.Common.Awoo.GetPage
{
	/// <summary>
	/// GetPageRequest
	/// </summary>
	public class GetPageRequest : AwooApiGetRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GetPageRequest()
		{
		}

		/// <summary>tags</summary>
		public string[] Tags
		{
			set { m_data["tags"] = string.Join(",", value); }
		}
		/// <summary>page</summary>
		public int Page
		{
			set { base.m_data["page"] = value.ToString(); }
		}
		/// <summary>limit</summary>
		public int Limit
		{
			set { base.m_data["limit"] = value.ToString(); }
		}
		/// <summary>sort</summary>
		public ProductSortType Sort
		{
			set { base.m_data["sort"] = value.ToText(); }
		}
		/// <summary>select</summary>
		public SelectAdditionalProductFieldType Select
		{
			set { base.m_data["select"] = value.ToText(); }
		}
		/// <summary>productType</summary>
		public string ProductType
		{
			set { base.m_data["productType"] = value; }
		}
		/// <summary>tag_filter</summary>
		public string TagFilter
		{
			set { base.m_data["tag_filter"] = value; }
		}
	}
}
