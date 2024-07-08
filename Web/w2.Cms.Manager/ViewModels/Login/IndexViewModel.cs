/*
=========================================================================================================
  Module      : ログインインデックスビューモデル(IndexViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.Input;

namespace w2.Cms.Manager.ViewModels.Login
{
	/// <summary>
	/// ログインインデックスビューモデル
	/// </summary>
	public class IndexViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IndexViewModel()
		{
			this.Input = new LoginInput();
		}

		/// <summary>ログイン入力</summary>
		public LoginInput Input { get; set; }
		/// <summary>エラーページ</summary>
		public string ErrorMessage { get; set; }
	}
}