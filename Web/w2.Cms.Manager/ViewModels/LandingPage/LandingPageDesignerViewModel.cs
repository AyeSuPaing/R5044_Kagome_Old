/*
=========================================================================================================
  Module      : LP デザイナビューモデル(LandingPageDesignerViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.LandingPaeg;

namespace w2.Cms.Manager.ViewModels.LandingPage
{
	/// <summary>
	/// LP デザイナビューモデル
	/// </summary>
	[Serializable]
	public class LandingPageDesignerViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageDesignerViewModel()
		{
			this.ParamModel = new LandingPageListParamModel();
		}

		/// <summary>パラメタモデル</summary>
		public LandingPageListParamModel ParamModel { get; set; }
		/// <summary>入力値</summary>
		public LandingPageInput Input { get; set; }
		/// <summary>公開状態</summary>
		public string PublicStatus { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>デザインタイプ</summary>
		public string DesignType { get; set; }
	}
}