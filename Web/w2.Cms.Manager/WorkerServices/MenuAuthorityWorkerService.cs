/*
=========================================================================================================
  Module      : メニュー権限ワーカーサービス(MenuAuthorityWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ViewModels.MenuAuthority;
using w2.Common.Util;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// メニュー権限ワーカーサービス
	/// </summary>
	public class MenuAuthorityWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm()
		{
			var all = new MenuAuthorityService().GetAllByPkgKbn(this.SessionWrapper.LoginShopId, MenuAuthorityHelper.ManagerSiteType.Cms);
			return new ListViewModel(all, this.SessionWrapper.LoginShopId);
		}

		/// <summary>
		/// 確認詳細ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="menuLevel">メニュー権限レベル</param>
		/// <param name="tempData">入力情報</param>
		/// <returns>ビューモデル</returns>
		public object CreateConfirmVm(ActionStatus actionStatus, int? menuLevel, TempDataManager tempData)
		{
			switch (actionStatus)
			{
				case ActionStatus.Detail:
					var memuAuthorityList = new MenuAuthorityService().Get(
						this.SessionWrapper.LoginShopId,
						MenuAuthorityHelper.ManagerSiteType.Cms,
						menuLevel.Value);
					if (memuAuthorityList.Length > 0)
						return new ConfirmViewModel(
							actionStatus,
							memuAuthorityList,
							this.SessionWrapper.LoginOperatorMenus);
					return null;

				case ActionStatus.Insert:
				case ActionStatus.Update:
					return new ConfirmViewModel(actionStatus, tempData.MenuAuthority);

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 登録編集ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="menuLevel">メニュー権限レベル</param>
		/// <returns>ビューモデル</returns>
		internal RegisterViewModel CreateRegisterVm(ActionStatus actionStatus, int? menuLevel = 0)
		{
			var sw = this.SessionWrapper;
			
			switch (actionStatus)
			{
				case ActionStatus.Update:
					var menuAuthorityList = new MenuAuthorityService().Get(sw.LoginShopId, MenuAuthorityHelper.ManagerSiteType.Cms, menuLevel.Value);
					if (menuAuthorityList.Length > 0)
						return new RegisterViewModel(sw.LoginOperatorMenus, menuAuthorityList)
						{
							ActionStatus = actionStatus
						};
					return null;

				case ActionStatus.Insert:
					return new RegisterViewModel(sw.LoginOperatorMenus)
					{
						ActionStatus = actionStatus
					};

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// メニュー権限情報に設定されているかチェック
		/// </summary>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		/// <returns>true:設定されている false:設定されていない</returns>
		internal bool CheckMenuAuthorityUsed(int menuAuthorityLevel)
		{
			var service = new MenuAuthorityService();
			var isUsed = service.CheckMenuAuthorityUsed(
				this.SessionWrapper.LoginShopId,
				MenuAuthorityHelper.ManagerSiteType.Cms,
				menuAuthorityLevel);
			return isUsed;
		}

		/// <summary>
		/// 登録更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="authorityMenuList">メニュー権限リスト</param>
		internal void InsertUpdate(ActionStatus actionStatus, MenuAuthorityModel[] authorityMenuList)
		{
			var service = new MenuAuthorityService();

			switch (actionStatus)
			{
				case ActionStatus.Insert:
					var authorityLevel = NumberingUtility.CreateNewNumber(
						this.SessionWrapper.LoginShopId,
						Constants.NUMBER_KEY_MENU_AUTHORITY_LEVEL);
					service.Insert(authorityMenuList, authorityLevel);
					break;

				case ActionStatus.Update:
					service.Update(authorityMenuList);
					break;

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		internal void Delete(int menuAuthorityLevel)
		{
			var service = new MenuAuthorityService();
			service.Delete(
				this.SessionWrapper.LoginShopId,
				MenuAuthorityHelper.ManagerSiteType.Cms,
				menuAuthorityLevel);
		}
	}
}