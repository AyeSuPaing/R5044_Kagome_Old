/*
=========================================================================================================
  Module      : Score Axis List View Model (ScoreAxisListViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Cms.Manager.ViewModels.ScoreAxis
{
	/// <summary>
	/// Score axis list view model
	/// </summary>
	[Serializable]
	public class ScoreAxisListViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoreAxisListViewModel()
		{
			this.Items = new ScoreAxisItemDetailViewModel[0];
		}

		/// <summary>Items</summary>
		public ScoreAxisItemDetailViewModel[] Items { get; set; }
	}

	/// <summary>
	/// Score axis item detail view model
	/// </summary>
	[Serializable]
	public class ScoreAxisItemDetailViewModel : ViewModelBase
	{
		/// <summary>Score axis id</summary>
		public string ScoreAxisId { get; set; }
		/// <summary>Score axis title</summary>
		public string ScoreAxisTitle { get; set; }
		/// <summary>Date changed(date)</summary>
		public string DateChanged1 { get; set; }
		/// <summary>Date changed(time)</summary>
		public string DateChanged2 { get; set; }
	}
}
