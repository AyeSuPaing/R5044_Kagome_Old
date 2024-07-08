/*
=========================================================================================================
  Module      : Score Axis Search Param Model (ScoreAxisSearchParamModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.ScoreAxis.Helper
{
	/// <summary>
	/// Score axis search param model
	/// </summary>
	public class ScoreAxisSearchParamModel : BaseDbMapModel
	{
		/// <summary>Pager no</summary>
		public int PagerNo { get; set; }
		/// <summary>Begin row num</summary>
		[DbMapName(Constants.FIELD_COMMON_BEGIN_NUM)]
		public int BeginRowNumber { get; set; }
		/// <summary>End row num</summary>
		[DbMapName(Constants.FIELD_COMMON_END_NUM)]
		public int EndRowNumber { get; set; }
	}
}
