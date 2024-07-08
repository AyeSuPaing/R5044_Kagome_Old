/*
=========================================================================================================
  Module      : メニューユーティリティ(MenuUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Manager.Menu;
using w2.Domain.MenuAuthority;

/// <summary>
/// メニューユーティリティ
/// </summary>
public class MenuUtility : CommonMenuUtility
{
	/// <summary>
	/// メニュー権限一覧リストアイテム作成
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="menuAccessLevel">権限レベル</param>
	/// <returns>リストアイテム配列</returns>
	public static ListItem[] CreateMenuAuthorityList(string shopId, string menuAccessLevel)
	{
		var menuAuthorities = new MenuAuthorityService().GetAllByPkgKbn(shopId, Constants.ManagerSiteType);

		// リストアイテム作成
		var list = new List<ListItem>();
		list.Add(new ListItem(Constants.STRING_UNACCESSABLEUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER));	// アクセス権限なし
		if (menuAccessLevel == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER) // スーパーユーザはスーパーユーザの時のみ表示	
		{
			list.Add(new ListItem(Constants.STRING_SUPERUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER));
		}
		list.AddRange(menuAuthorities.Select(ma => new ListItem(ma.MenuAuthorityName, ma.MenuAuthorityLevel.ToString())));
		return list.ToArray();
	}

	/// <summary>
	///  タイトル取得
	/// </summary>
	/// <param name="filePath">仮想パス</param>
	/// <returns>タイトル</returns>
	public static string GetTitle(string filePath)
	{
		var title = ManagerMenuCache.Instance.GetTitle(filePath);
		return title;
	}

	/// <summary>
	/// CS向けMenuLarge
	/// </summary>
	[Serializable]
	public class MenuLargeCs 
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">メニュー名</param>
		/// <param name="iconCss">アイコンCSS</param>
		public MenuLargeCs(string name, string iconCss) 
		{
			this.Name = name;
			this.IconCss = iconCss;
		}

		/// <summary>
		/// 小メニューの中で、未読/未対応なものが存在するかどうかを判定します。
		/// </summary>
		/// <param name="operatorId">オペレータId</param>
		/// <returns></returns>
		public bool HasCount(string operatorId)
		{
			var result = this.SmallMenus.Any(sm => sm.HasCount(operatorId));
			return result;
		}

		/// <summary>メニュー名</summary>
		public string Name { get; set; }
		/// <summary>アイコン名</summary>
		public string IconCss { get; set; }
		/// <summary>小メニューCS</summary>
		public MenuSmallCs[] SmallMenus { get; set; }
	}

	/// <summary>
	/// CS向けMenuSmall
	/// </summary>
	[Serializable]
	public class MenuSmallCs
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">メニュー名</param>
		/// <param name="page">トップページ</param>
		/// <param name="path">メニューパス</param>
		/// <param name="cskbn">CS区分</param>
		public MenuSmallCs(string name, string page, string path, string cskbn)
		{
			this.Name = name;
			this.TopPage = page;
			this.MenuPath = path;
			this.Href = Constants.PATH_ROOT + path + page;
			this.CsKbn = cskbn;
		}

		/// <summary>
		/// このSmallMenuに表示する、タスクの件数を返します。
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="mode">タスクターゲットモード</param>
		/// <returns>表示件数（件数非表示のときはnull）</returns>
		public int? GetTaskCount(string operatorId, TaskTargetMode mode)
		{
			return GetCount(operatorId, mode);
		}

		/// <summary>
		/// このSmallMenuに表示する、未対応な個人タスク/グループタスクの件数を返します。
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="target">タスクターゲットモード</param>
		/// <returns>表示件数（件数非表示のときはnull）</returns>
		private int? GetCount(string operatorId, TaskTargetMode target)
		{
			if (operatorId == null) return null;	// セッションタイムアウト時にシステムエラーになるのを回避

			switch (this.CsKbn)
			{
				case "NONE":
				case "ACTIVE":
				case "SUSPEND":
				case "URGENT":
					return MenuTaskCountManager.GetIncidentCount(
						operatorId,
						(TaskStatusRefineMode)Enum.Parse(typeof(TaskStatusRefineMode), this.CsKbn, true),
						target);

				case "Draft":
				case "Approval":
				case "ApprovalRequest":
				case "ApprovalResult":
				case "Send":
				case "SendRequest":
				case "SendResult":
					return MenuTaskCountManager.GetMessageCount(
						operatorId,
						(TopPageKbn)Enum.Parse(typeof(TopPageKbn), this.CsKbn, true),
						target);

				default:
					return null;
			}
		}

		/// <summary>
		/// このSmallMenuが未読/未対応かどうかを判定します。
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <returns></returns>
		public bool HasCount(string operatorId)
		{
			return false;
		}

		/// <summary>メニュー名</summary>
		public string Name { get; set; }
		/// <summary>トップページ名</summary>
		public string TopPage { get; set; }
		/// <summary>メニューパス</summary>
		public string MenuPath { get; set; }
		/// <summary>サイト</summary>
		public string Href { get; set; }
		/// <summary>CS区分</summary>
		public string CsKbn { get; set; }
	}
}
