/*
=========================================================================================================
 Module      : 編集画面ビューモデル(ModifyViewModel.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.Input;

namespace w2.Cms.Manager.ViewModels.OgpTagSetting
{
	/// <summary>
	/// 編集画面ビューモデル
	/// </summary>
	public class ModifyViewModel
	{
		/// <summary>全体設定インプット</summary>
		public OgpTagSettingInput InputForDefaultSetting { get; set; }
	}
}