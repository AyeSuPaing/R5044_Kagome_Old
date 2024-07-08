/*
=========================================================================================================
  Module      : 商品一覧表示設定Modifyビューモデル(ModifyViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Input;

namespace w2.Cms.Manager.ViewModels.ProductListDispSetting
{
	/// <summary>
	/// 商品一覧表示設定Modifyビューモデル
	/// </summary>
	[Serializable]
	public class ModifyViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ModifyViewModel()
		{
		}

		/// <summary>商品一覧表示設定入力情報</summary>
		public ProductListDispSettingInput Input { get; set; }
		/// <summary>更新 成功フラグ</summary>
		public bool UpdateSuccessFlg { get; set; }
	}
}