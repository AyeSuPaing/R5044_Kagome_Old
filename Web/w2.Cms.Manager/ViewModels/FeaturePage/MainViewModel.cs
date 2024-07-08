/*
=========================================================================================================
  Module      : 特集ページメインビューモデル (MainViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.ParamModels.FeaturePage;

namespace w2.Cms.Manager.ViewModels.FeaturePage
{
	/// <summary>
	/// 特集ページ情報 メインビューモデル
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			this.ParamModel = new FeaturePageListSearchParamModel();
		}

		/// <summary>検索パラメータ</summary>
		public FeaturePageListSearchParamModel ParamModel { get; set; }
	}
}