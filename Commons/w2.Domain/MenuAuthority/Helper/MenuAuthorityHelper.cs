/*
=========================================================================================================
  Module      : メニュー権限ヘルパ (MenuAuthorityHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Helper.Attribute;

namespace w2.Domain.MenuAuthority.Helper
{
	/// <summary>
	/// メニュー権限ヘルパ
	/// </summary>
	public class MenuAuthorityHelper
	{
		/// <summary>管理画面タイプ</summary>
		public enum ManagerSiteType
		{
			/// <summary>w2Commerce</summary>
			[EnumTextName("EC")]
			Ec,
			/// <summary>w2MarketingPlanner</summary>
			[EnumTextName("MP")]
			Mp,
			/// <summary>w2CustomerSupport</summary>
			[EnumTextName("CS")]
			Cs,
			/// <summary>w2Cms</summary>
			[EnumTextName("CMS")]
			Cms
		}

		/// <summary>
		/// メニューアクセスレベルフィールド名取得
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>メニューアクセスレベルフィールド名</returns>
		public static string GetPkgKbn(ManagerSiteType managerSiteType)
		{
			switch (managerSiteType)
			{
				case ManagerSiteType.Ec:
					return Constants.FLG_MENUAUTHORITY_PKG_KBN_EC;

				case ManagerSiteType.Mp:
					return Constants.FLG_MENUAUTHORITY_PKG_KBN_MP;

				case ManagerSiteType.Cs:
					return Constants.FLG_MENUAUTHORITY_PKG_KBN_CS;

				case ManagerSiteType.Cms:
					return Constants.FLG_MENUAUTHORITY_PKG_KBN_CMS;
			}
			throw new Exception("不明な管理画面タイプ" + managerSiteType);
		}

		/// <summary>
		/// メニューアクセスレベルフィールド名取得
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>メニューアクセスレベルフィールド名</returns>
		public static string GetOperatorMenuAccessLevelFieldName(ManagerSiteType managerSiteType)
		{
			switch (managerSiteType)
			{
				case ManagerSiteType.Ec:
					return Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_EC;

				case ManagerSiteType.Mp:
					return Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_MP;

				case ManagerSiteType.Cs:
					return Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_CS;

				case ManagerSiteType.Cms:
					return Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_CMS;
			}
			throw new Exception("不明な管理画面タイプ" + managerSiteType);
		}
	}
}
