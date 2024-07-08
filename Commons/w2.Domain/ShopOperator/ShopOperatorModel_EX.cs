/*
=========================================================================================================
  Module      : 店舗管理者マスタモデル (ShopOperatorModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Common.Util;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Domain.ShopOperator
{
	/// <summary>
	/// 店舗管理者マスタモデル
	/// </summary>
	public partial class ShopOperatorModel
	{
		#region メソッド
		/// <summary>
		/// スーパーユーザーか
		/// </summary>
		/// <param name="managerSiteType">管理画面サイトタイプ</param>
		/// <returns>スーパーユーザーか</returns>
		public bool IsSuperUser(MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			var menuAccessLevel = GetMenuAccessLevel(managerSiteType);
			return (menuAccessLevel.HasValue
				&& (menuAccessLevel.Value.ToString() == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER));
			// HACK:FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSERはnullとかintのはず
		}

		/// <summary>
		/// アクセス不能ユーザーか
		/// </summary>
		/// <param name="managerSiteType">管理画面サイトタイプ</param>
		/// <returns>アクセス不能ユーザーか</returns>
		public bool IsInaccessibleUser(MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			var menuAccessLevel = GetMenuAccessLevel(managerSiteType);
			return (menuAccessLevel.HasValue == false);
		}

		/// <summary>
		/// メニューアクセスレベル設定
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <param name="menuAccessLevel">アクセスレベル</param>
		public void SetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType managerSiteType, int? menuAccessLevel)
		{
			switch (managerSiteType)
			{
				case MenuAuthorityHelper.ManagerSiteType.Ec:
					this.MenuAccessLevel1 = menuAccessLevel;
					break;
				case MenuAuthorityHelper.ManagerSiteType.Mp:
					this.MenuAccessLevel2 = menuAccessLevel;
					break;
				case MenuAuthorityHelper.ManagerSiteType.Cs:
					this.MenuAccessLevel3 = menuAccessLevel;
					break;
				case MenuAuthorityHelper.ManagerSiteType.Cms:
					this.MenuAccessLevel4 = menuAccessLevel;
					break;
				default:
					throw new Exception("不明な管理画面タイプ" + managerSiteType);
			}
		}

		/// <summary>
		/// メニューアクセスレベル取得
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>メニューアクセスレベル</returns>
		public int? GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			switch (managerSiteType)
			{
				case MenuAuthorityHelper.ManagerSiteType.Ec:
					return this.MenuAccessLevel1;

				case MenuAuthorityHelper.ManagerSiteType.Mp:
					return this.MenuAccessLevel2;

				case MenuAuthorityHelper.ManagerSiteType.Cs:
					return this.MenuAccessLevel3;

				case MenuAuthorityHelper.ManagerSiteType.Cms:
					return this.MenuAccessLevel4;
			}
			throw new Exception("不明な管理画面タイプ" + managerSiteType);
		}

		/// <summary>
		/// 管理メニュー取得
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>管理メニュー</returns>
		public MenuAuthorityModel[] GetMenuAuthorities(MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			switch (managerSiteType)
			{
				case MenuAuthorityHelper.ManagerSiteType.Ec:
					return this.EcMenuAuthorities;

				case MenuAuthorityHelper.ManagerSiteType.Mp:
					return this.MpMenuAuthorities;

				case MenuAuthorityHelper.ManagerSiteType.Cs:
					return this.CsMenuAuthorities;

				case MenuAuthorityHelper.ManagerSiteType.Cms:
					return this.CmsMenuAuthorities;
			}
			throw new Exception("不明な管理画面タイプ" + managerSiteType);
		}

		/// <summary>
		/// 閲覧可能なタグIDを配列で取得
		/// </summary>
		/// <returns>閲覧可能なタグID配列</returns>
		public string[] GetUsableAffiliateTagIdsArray()
		{
			var usableTags = this.UsableAffiliateTagIdsInReport.Split(',')
				.Where(tagId => (string.IsNullOrWhiteSpace(tagId) == false))
				.ToArray();

			return usableTags;
		}

		/// <summary>
		/// 閲覧可能な広告媒体区分を配列で取得
		/// </summary>
		/// <returns>閲覧可能な広告媒体区分配列</returns>
		public string[] GetUsableAdvcodeMediaTypeIdsArray()
		{
			var usableMediaTypes = this.UsableAdvcodeMediaTypeIds.Split(',')
				.Where(mediaType => (string.IsNullOrWhiteSpace(mediaType) == false))
				.ToArray();

			return usableMediaTypes;
		}

		/// <summary>
		/// 閲覧可能な設置箇所を配列で取得
		/// </summary>
		/// <returns>閲覧可能な設置箇所配列</returns>
		public string[] GetUsableOutputLocationsArray()
		{
			var usableLocations = this.UsableOutputLocations.Split(',')
				.Where(location => (string.IsNullOrWhiteSpace(location) == false))
				.ToArray();

			return usableLocations;
		}
		#endregion

		#region プロパティ
		/// <summary>有効フラグがオンか</summary>
		public bool ValidFlgOn
		{
			get { return (this.ValidFlg == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID); }
		}
		/// <summary>EC管理メニュー</summary>
		public MenuAuthorityModel[] EcMenuAuthorities { get; set; }
		/// <summary>MP管理メニュー</summary>
		public MenuAuthorityModel[] MpMenuAuthorities { get; set; }
		/// <summary>CS管理メニュー</summary>
		public MenuAuthorityModel[] CsMenuAuthorities { get; set; }
		/// <summary>CMS管理メニュー</summary>
		public MenuAuthorityModel[] CmsMenuAuthorities { get; set; }
		/// <summary>オペレータ総件数</summary>
		public int RowCount
		{
			get { return (int)this.DataSource[Constants.FIELD_COMMON_ROW_COUNT]; }
			set { this.DataSource[Constants.FIELD_COMMON_ROW_COUNT] = value; }
		}
		/// <summary>ECメニュー権限1</summary>
		public string EcMenuAccessLevel
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1]); }
		}
		/// <summary>Mpメニュー権限</summary>
		public string MpMenuAccessLevel
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2]); }
		}
		/// <summary>メニュー権限名</summary>
		public string MenuAuthorityName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME]); }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlag
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG]); }
		}
		/// <summary>有効フラグ表示名</summary>
		public string ValidFlagName
		{
			get
			{
				return ValueText.GetValueText(
					Constants.TABLE_SHOPOPERATOR,Constants.FIELD_SHOPOPERATOR_VALID_FLG, this.ValidFlag);
			}
		}
		#endregion
	}
}
