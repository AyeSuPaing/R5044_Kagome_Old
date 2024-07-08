/*
=========================================================================================================
  Module      : プレビューモデル (PreviewViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.ViewModels.LandingPage
{
	/// <summary>
	/// プレビューモデル
	/// </summary>
	public class PreviewViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PreviewViewModel()
		{
			this.PreviewUrl = "";
		}

		/// <summary>プレビューURL</summary>
		public string PreviewUrl { get; set; }
	}
}