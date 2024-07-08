/*
=========================================================================================================
  Module      : メニュー権限設定Registerビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.Input;
using w2.Domain.MenuAuthority;

namespace w2.Cms.Manager.ViewModels.MenuAuthority
{
	/// <summary>
	/// メニュー権限設定Registerビューモデル
	/// </summary>
	[Serializable]
	public class RegisterViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisterViewModel()
		{
		}

		/// <summary>
		/// コンストラクタ(新規登録)
		/// </summary>
		/// <param name="menuLarges">ログインオペレーターメニュー</param>
		public RegisterViewModel(MenuLarge[] menuLarges)
		{
			this.Input = new MenuAuthorityInput();
			this.Input.Initialize(menuLarges);
		}

		/// <summary>
		/// コンストラクタ(編集)
		/// </summary>
		/// <param name="menuLarges">ログインオペレーターメニュー</param>
		/// <param name="menuAuthorityList">メニュー権限設定リスト</param>
		public RegisterViewModel(MenuLarge[] menuLarges, MenuAuthorityModel[] menuAuthorityList)
		{
			this.Input = new MenuAuthorityInput();
			this.Input.SetMenuAuthorityInput(menuLarges, menuAuthorityList);
		}

		/// <summary>メニュー権限入力</summary>
		public MenuAuthorityInput Input { get; set; }
	}

}