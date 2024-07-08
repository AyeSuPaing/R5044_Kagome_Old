/*
=========================================================================================================
  Module      : メニュー権限設定Confirmビューモデル(ConfirmViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Menu;
using w2.Domain.MenuAuthority;

namespace w2.Cms.Manager.ViewModels.MenuAuthority
{
	/// <summary>
	/// メニュー権限設定Confirmビューモデル
	/// </summary>
	[Serializable]
	public class ConfirmViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="menuAuthorityList">メニュー権限リスト</param>
		/// <param name="operatorAuthorityMenusForDetail">詳細向けオペレータ認証メニュー</param>
		public ConfirmViewModel(
			ActionStatus actionStatus,
			MenuAuthorityModel[] menuAuthorityList,
			MenuLarge[] operatorAuthorityMenusForDetail = null)
		{
			this.ActionStatus = actionStatus;
			this.MenuAuthorityLevel = (menuAuthorityList.Length > 0) ? menuAuthorityList[0].MenuAuthorityLevel : 0;
			this.Name = menuAuthorityList[0].MenuAuthorityName;
			this.MenuLarges = ManagerMenuCache.Instance.GetAuthorityMenuList(menuAuthorityList);
			this.IsActionStatusDetail = (base.IsActionStatusDetail
				&& ((operatorAuthorityMenusForDetail == null)
					|| ManagerMenuCache.Instance.HasOperatorMenuAuthority(this.MenuLarges, operatorAuthorityMenusForDetail)));
		}

		/// <summary>メニュー権限名</summary>
		public string Name { get; set; }
		/// <summary>メニュー権限レベル</summary>
		public int MenuAuthorityLevel { get; set; }
		/// <summary>大メニュー配列</summary>
		public MenuLarge[] MenuLarges { get; set; }
		/// <summary>詳細ステータスか（オペレーターが利用できない権限があるときは編集削除できない）</summary>
		public new bool IsActionStatusDetail { get; set; }
	}
}