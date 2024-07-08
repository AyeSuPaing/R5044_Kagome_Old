/*
=========================================================================================================
  Module      : Scoring Sale Chart View Model (ScoringSaleChartViewModel.cshtml)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale chart view model class
	/// </summary>
	public class ScoringSaleChartViewModel
	{
		/// <summary>Page exit rate</summary>
		public double PageExitRate { get; set; }
		/// <summary>Page name</summary>
		public string PageName { get; set; }
	}
}
