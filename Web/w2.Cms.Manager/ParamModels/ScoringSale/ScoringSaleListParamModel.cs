/*
=========================================================================================================
  Module      : Scoring Sale List Param Model (ScoringSaleListParamModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Cms.Manager.ParamModels.ScoringSale
{
	/// <summary>
	/// Scoring sale list view model param model
	/// </summary>
	[Serializable]
	public class ScoringSaleListParamModel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleListParamModel()
		{
			this.SearchWord = string.Empty;
			this.SearchPublishStatus = string.Empty;
			this.SearchPublicDateKbn = "UNSELECTED";
		}

		/// <summary>Search word</summary>
		public string SearchWord { get; set; }
		/// <summary>Publish status</summary>
		public string SearchPublishStatus { get; set; }
		/// <summary>Public date kbn</summary>
		public string SearchPublicDateKbn { get; set; }
	}
}
