/*
=========================================================================================================
  Module      : Preview View Model (PreviewViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Preview view model
	/// </summary>
	public class PreviewViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PreviewViewModel()
		{
			this.PreviewUrl = string.Empty;
		}

		/// <summary>Preview url</summary>
		public string PreviewUrl { get; set; }
	}
}
