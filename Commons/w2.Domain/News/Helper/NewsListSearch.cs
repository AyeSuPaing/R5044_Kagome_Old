/*
=========================================================================================================
  Module      : News List Search (NewsListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.News.Helper
{
	/// <summary>
	/// Class news list search condition
	/// </summary>
	[Serializable]
	public class NewsListSearchCondition : BaseDbMapModel
	{
		#region Property
		/// <summary>
		/// News text
		/// </summary>
		public string NewsText { get; set; }

		/// <summary>
		/// News text（SQL LIKE）
		/// </summary>
		[DbMapName("news_text_like_escaped")]
		public string NewsTextLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.NewsText); }
		}

		/// <summary>
		/// Shop id
		/// </summary>
		[DbMapName("shop_id")]
		public string ShopId { get; set; }

		/// <summary>
		/// Disp flg
		/// </summary>
		[DbMapName("disp_flg")]
		public string DispFlg { get; set; }

		/// <summary>
		/// Mobile disp flg
		/// </summary>
		[DbMapName("mobile_disp_flg")]
		public string MobileDispFlg { get; set; }

		/// <summary>
		/// Valid flg
		/// </summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }

		/// <summary>
		/// Display date from (from)
		/// </summary>
		[DbMapName("display_date_from_from")]
		public string DisplayDateFromFrom { get; set; }

		/// <summary>
		/// Display date from (to)
		/// </summary>
		[DbMapName("display_date_from_to")]
		public string DisplayDateFromTo { get; set; }

		/// <summary>
		/// Display date to (from)
		/// </summary>
		[DbMapName("display_date_to_from")]
		public string DisplayDateToFrom { get; set; }

		/// <summary>
		/// Display date to (to)
		/// </summary>
		[DbMapName("display_date_to_to")]
		public string DisplayDateToTo { get; set; }

		/// <summary>
		/// Begin row number
		/// </summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }

		/// <summary>
		/// End row number
		/// </summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }

		/// <summary>
		/// Sort kbn
		/// </summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		#endregion
	}

	/// <summary>
	/// Class news list search result
	/// </summary>
	[Serializable]
	public class NewsListSearchResult : NewsModel
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public NewsListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion
	}
}
