/*
=========================================================================================================
  Module      : LP検索条件 (LandingPageSearchParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.LandingPage.Helper
{
	/// <summary>
	/// LP検索条件
	/// </summary>
	public class LandingPageSearchParamModel : BaseDbMapModel
	{
		/// <summary>ページID（SQL LIKEエスケープ済）</summary>
		[DbMapName("page_id_like_escaped")]
		public string PageIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.PageId); }
		}
		/// <summary>ページID</summary>
		public string PageId { get; set; }
		/// <summary>ページ名（SQL LIKEエスケープ済）</summary>
		[DbMapName("name_like_escaped")]
		public string NameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Name); }
		}
		/// <summary>ページ名</summary>
		public string Name { get; set; }
		/// <summary>ページ番号</summary>
		public int PagerNo { get; set; }
		/// <summary>オフセット行数</summary>
		[DbMapName("offset_num")]
		public int OffsetNumber { get; set; }
		/// <summary>フェッチ行数</summary>
		[DbMapName("fetch_num")]
		public int FetchNumber { get; set; }
	}
}