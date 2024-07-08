/*
=========================================================================================================
  Module      : LP 画像グループビューモデル(LandingPageImageGroupViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;

namespace w2.Cms.Manager.ViewModels.LandingPage
{
	/// <summary>
	/// LP 画像グループビューモデル
	/// </summary>
	public class LandingPageImageGroupViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageImageGroupViewModel()
		{
			this.GroupListItems = new SelectListItem[] { };
		}

		/// <summary>グループリスト</summary>
		public SelectListItem[] GroupListItems { get; set; }
	}
}