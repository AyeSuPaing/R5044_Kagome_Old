/*
=========================================================================================================
  Module      : 店舗管理者ワーカーサービス(ShopOperatoWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.ShopOperator;
using w2.Cms.Manager.ViewModels.ShopOperator;
using w2.Common.Util;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;
using w2.Domain.ShopOperator.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 店舗管理者ワーカーサービス
	/// </summary>
	public class ShopOperatoWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(ListParamModel pm)
		{
			var searchCondition = new ShopOperatorListSearchCondition
			{
				ShopId = this.SessionWrapper.LoginShopId,
				PkgKbn = Constants.FLG_MENUAUTHORITY_PKG_KBN_CMS,
				OperatorId = pm.OperatorId,
				OperatorName = pm.OperatorName,
				SortKbn = pm.SortKbn,
				ValidFlg = pm.ValidFlg,
				LoginOperatorMenuAccessLevelCms = this.SessionWrapper.LoginMenuAccessInfo.MenuAccessLevel,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				ConditionMenuAccessLevel = pm.ConditionMenuAccessLevel,
			};
			var menuAuthority = new List<ListItem>
			{
				new ListItem
				{
					Text = "",
					Value = ""
				},
				new ListItem
				{
					Text = Constants.STRING_UNACCESSABLEUSER_NAME,
					Value = Constants.FLG_SHOPOPERATOR_NO_AUTHORITY_VALUE
				}
			};
			if (this.SessionWrapper.LoginMenuAccessInfo.MenuAccessLevel.ToString()
				== Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER)
			{
				menuAuthority.Add(new ListItem(Constants.STRING_SUPERUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER));
			}
			var menuAuthorities = new MenuAuthorityService().GetAllByPkgKbn(this.SessionWrapper.LoginShopId, MenuAuthorityHelper.ManagerSiteType.Cms);
			var menuAuthorityListItem = menuAuthorities
				.Select(me => new ListItem(me.MenuAuthorityName, me.MenuAuthorityLevel.ToString()));
			menuAuthority.AddRange(menuAuthorityListItem);
			var total = new ShopOperatorService().GetSearchHitCount(searchCondition);
			var list = new ShopOperatorService().Search(searchCondition);
			if (list.Length == 0)
			{
				return new ListViewModel(menuAuthority)
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError,
				};
			}

			var url = this.UrlHelper.Action(
				"List",
				Constants.CONTROLLER_W2CMS_MANAGER_OPERATOR,
				new
				{
					OperatorId = pm.OperatorId,
					OperatorName = pm.OperatorName,
					SortKbn = pm.SortKbn,
					ConditionMenuAccessLevel = pm.ConditionMenuAccessLevel,
					ValidFlg = pm.ValidFlg,
				});
			return new ListViewModel(menuAuthority)
			{
				ParamModel = pm,
				List = list,
				PagerHtml = WebPager.CreateDefaultListPager(total, pm.PagerNo, url),
			};
		}

		/// <summary>
		/// 確認詳細ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="pageLayout">ページレイアウト</param>
		/// <param name="tempDataManager">一時データ</param>
		/// <returns>確認ビューモデル</returns>
		public ConfirmViewModel CreateConfirmVm(
			ActionStatus actionStatus,
			string operatorId,
			string pageLayout,
			TempDataManager tempDataManager)
		{
			switch (actionStatus)
			{
				case ActionStatus.Detail:
					var so = new ShopOperatorService().Get(this.SessionWrapper.LoginShopId, operatorId);
					return (so != null) ? new ConfirmViewModel(actionStatus, pageLayout, so) : null;

				case ActionStatus.Insert:
				case ActionStatus.Update:
					return new ConfirmViewModel(actionStatus, pageLayout, tempDataManager.ShopOperator);

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 登録編集ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>登録ビューモデル</returns>
		public RegisterViewModel CreateRegisterVm(ActionStatus actionStatus, string operatorId)
		{
			var menuAuthorities = new MenuAuthorityService().GetAllByPkgKbn(
				this.SessionWrapper.LoginShopId,
				MenuAuthorityHelper.ManagerSiteType.Cms);

			if ((actionStatus == ActionStatus.Update)
				|| (string.IsNullOrEmpty(operatorId) == false))
			{
				var op = new ShopOperatorService().Get(Constants.CONST_DEFAULT_SHOP_ID, operatorId);
				if (op == null) return null;
				return new RegisterViewModel
				{
					ActionStatus = actionStatus,
					MenuAccessLevel = op.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Cms),
					MenuAuthorities = menuAuthorities,
					OperatorId = operatorId,
					Name = op.Name,
					LoginId = op.LoginId,
					Password = op.Password,
					MailAddr = op.MailAddr,
					ValidFlg = op.ValidFlg,
				};
			}
			else if (actionStatus == ActionStatus.Insert)
			{
				return new RegisterViewModel
				{
					ActionStatus = actionStatus,
					MenuAuthorities = menuAuthorities,
					ValidFlg = Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID,
				};
			}
			throw new Exception("未対応のactionStatus：" + actionStatus);
		}

		/// <summary>
		/// 登録更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="shopOperator">店舗オペレータ</param>
		public void InsertUpdate(ActionStatus actionStatus, ShopOperatorModel shopOperator)
		{
			shopOperator.LastChanged = this.SessionWrapper.LoginOperatorName;

			var service = new ShopOperatorService();
			switch (actionStatus)
			{
				case ActionStatus.Insert:
					shopOperator.OperatorId = NumberingUtility.CreateKeyId(
						this.SessionWrapper.LoginShopId,
						Constants.NUMBER_KEY_SHOP_OPERATOR_ID,
						Constants.CONST_SHOPOPERATOR_ID_LENGTH);
					service.Insert(shopOperator);
					break;

				case ActionStatus.Update:
					var old = service.Get(shopOperator.ShopId, shopOperator.OperatorId);
					if (string.IsNullOrEmpty(shopOperator.Password)) shopOperator.Password = old.Password;
					foreach (MenuAuthorityHelper.ManagerSiteType ty
						in Enum.GetValues(typeof(MenuAuthorityHelper.ManagerSiteType)))
					{
						if (ty == MenuAuthorityHelper.ManagerSiteType.Cms) continue;
						shopOperator.SetMenuAccessLevel(ty, old.GetMenuAccessLevel(ty));
					}
					service.Update(shopOperator);
					break;

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		public void Delete(string operatorId)
		{
			new ShopOperatorService().Delete(this.SessionWrapper.LoginShopId, operatorId);
		}
	}
}