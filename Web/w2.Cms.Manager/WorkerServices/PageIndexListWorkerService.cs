/*
=========================================================================================================
  Module      : 機能一覧ワーカーサービス(PageIndexListWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Menu;
using w2.Cms.Manager.ViewModels.PageIndexList;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 機能一覧ワーカーサービス
	/// </summary>
	public class PageIndexListWorkerService : BaseWorkerService
	{
		/// <summary>
		/// PageIndexList取得
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="loginOperatorMenus"></param>
		/// <returns>PageIndexList</returns>
		public PageIndexViewModel GetPageIndexList(string key, MenuLarge[] loginOperatorMenus)
		{
			var pageIndexList = PageIndexListUtility.GetPageIndexList(
				Constants.PHYSICALDIRPATH_MANAGER_PAGE_INDEX_LIST_XML,
				key,
				ManagerMenuCache.Instance.MenuList);
			var vm = new PageIndexViewModel(pageIndexList, key, loginOperatorMenus);
			return vm;
		}
	}
}