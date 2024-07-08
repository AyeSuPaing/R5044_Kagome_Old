/*
=========================================================================================================
  Module      : Score Axis Detail View Model (ScoreAxisDetailViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Cms.Manager.ViewModels.ScoreAxis
{
	/// <summary>
	/// Score axis detail view model
	/// </summary>
	[Serializable]
	public class ScoreAxisDetailViewModel : ViewModelBase
	{
		#region +Properties
		/// <summary>Score axis id</summary>
		public string ScoreAxisId { get; set; }
		/// <summary>Score axis title</summary>
		public string ScoreAxisTitle { get; set; }
		/// <summary>Axis names</summary>
		public string[] AxisNames { get; set; }
		#endregion
	}
}
