/*
=========================================================================================================
  Module      : メニュー権限設定Listビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.ViewModels.MenuAuthority
{
	/// <summary>
	/// メニュー権限設定Listビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="all">メニュー権限設定リスト</param>
		/// <param name="shopId">店舗ID</param>
		public ListViewModel(MenuAuthorityModel[] all, string shopId)
		{
			this.List = all.Select(
				a => new MenuAuthorityViewModel
				{
					Name = a.MenuAuthorityName,
					Level = a.MenuAuthorityLevel,
					Count = a.MenuCounts,
				}).ToArray();
		}

		/// <summary>一覧表示用メニュー権限設定</summary>
		public class MenuAuthorityViewModel
		{
			/// <summary>メニュー権限名</summary>
			public string Name { get; set; }
			/// <summary>メニュー権限レベル</summary>
			public int Level { get; set; }
			/// <summary>許可メニュー数</summary>
			public int Count { get; set; }
		}

		/// <summary>メニュー権限設定一覧</summary>
		public MenuAuthorityViewModel[] List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}