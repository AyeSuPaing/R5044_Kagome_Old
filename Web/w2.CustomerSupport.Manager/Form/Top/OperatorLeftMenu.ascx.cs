/*
=========================================================================================================
  Module      : オペレータ左メニューユーザーコントロール処理(OperatorLeftMenu.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.IncidentCategory;
using w2.Common.Web;

public partial class Form_Top_OperatorLeftMenu : BaseUserControl
{
	// CSカテゴリツリーの展開ノードを格納
	private const string COOKIE_NAME_CS_CATEGORY_TREE = "cookie_cs_category_tree";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// カテゴリツリー作成
			CreateCategoryTree();
		}
	}

	/// <summary>
	/// カテゴリツリー作成
	/// </summary>
	private void CreateCategoryTree()
	{
		// 初期設定
		tvCategoryTree.Nodes.Clear();
		tvCategoryTree.CollapseImageUrl = "../../Images/Cs/tree_close.png";
		tvCategoryTree.ExpandImageUrl = "../../Images/Cs/tree_open.png";
		tvCategoryTree.NodeStyle.CssClass = "menu";
		tvCategoryTree.NodeIndent = 15;

		// カテゴリ情報取得してツリーに登録
		var service = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		var categories = service.GetValidAll(this.LoginOperatorDeptId);

		foreach (CsIncidentCategoryModel category in categories)
		{
			var categoryName = (category.CategoryName.Length < 10) ? category.CategoryName : category.CategoryName.Substring(0, 10) + "...";
			TreeNode node = new TreeNode(WebSanitizer.HtmlEncode(categoryName), category.CategoryId);
			node.ToolTip = category.CategoryName;
			node.NavigateUrl = Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_TOP_PAGE + "?" + Constants.REQUEST_KEY_INCIDENT_CATEGORY_ID + "=" + category.CategoryId;
			node.Selected = (Request[Constants.REQUEST_KEY_INCIDENT_CATEGORY_ID] == category.CategoryId);

			TreeNode parent = tvCategoryTree.FindNode(category.EX_ParentCategoryPath);
			if (parent != null)
			{
				parent.ChildNodes.Add(node);
			}
			else
			{
				tvCategoryTree.Nodes.Add(node);
			}

			// 選択中のカテゴリをSelectedにすることでスタイル適用
			if (Request.RawUrl.StartsWith(node.NavigateUrl))
			{
				node.Selected = true;
			}
		}

		// クッキーからツリーの初期状態を復元
		var treeData = CookieManager.GetValue(COOKIE_NAME_CS_CATEGORY_TREE);
		if (treeData != null)
		{
			foreach (string path in (treeData.Split(',')))
			{
				if (tvCategoryTree.FindNode(path) != null) tvCategoryTree.FindNode(path).Expand();
			}
		}
	}

	/// <summary>
	/// ページプリレンダー（コントロールのレンダリングが再実行される直前に実行される）
	/// </summary>
	public void Page_PreRender()
	{
		// インシデントメニュー
		var menuListIncident = new List<MenuUtility.MenuLargeCs>();
		var large = new MenuUtility.MenuLargeCs(
			// 「インシデント」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
				Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE,
				Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE_INCIDENT),
			"icon-incident")
		{
			SmallMenus = new MenuUtility.MenuSmallCs[]
			{
				new MenuUtility.MenuSmallCs(
					// 「未対応」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_NONE),
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TASKSTATUS_MODE + "=" + TaskStatusRefineMode.None,
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH,
					"NONE"),
				new MenuUtility.MenuSmallCs(
					// 「対応中」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_ACTIVE),
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TASKSTATUS_MODE + "=" + TaskStatusRefineMode.Active,
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH,
					"ACTIVE"),
				new MenuUtility.MenuSmallCs(
					// 「保留」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_SUSPEND),
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TASKSTATUS_MODE + "=" + TaskStatusRefineMode.Suspend,
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH,
					"SUSPEND"),
				new MenuUtility.MenuSmallCs(
					// 「至急」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
						Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_URGENT),
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TASKSTATUS_MODE + "=" + TaskStatusRefineMode.Urgent,
					Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH,
					"URGENT"),
			}
		};
		menuListIncident.Add(large);
		rLargeMenuIncident.DataSource = menuListIncident;
		rLargeMenuIncident.DataBind();

		// オペレータメニュー
		var menuListOperator = new List<MenuUtility.MenuLargeCs>
		{
			new MenuUtility.MenuLargeCs(
				// 「メッセージ」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE_MESSAGE),
				"icon-message")
			{
				SmallMenus = new MenuUtility.MenuSmallCs[]
				{
					new MenuUtility.MenuSmallCs(
						// 「下書き」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_DRAFT),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.Draft,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "Draft"),
					new MenuUtility.MenuSmallCs(
						// 「回答済み」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_MESSAGE_REPLY),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.Reply,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "Reply"),
				}
			},
			new MenuUtility.MenuLargeCs(
				// 「承認フロー」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE_APPROVAL),
				"icon-approval")
			{
				SmallMenus = new MenuUtility.MenuSmallCs[]
				{
					new MenuUtility.MenuSmallCs(
						// 「承認」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_APPROVAL),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.Approval,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "Approval"),
					new MenuUtility.MenuSmallCs(
						// 「依頼中」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_REQUEST),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.ApprovalRequest,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "ApprovalRequest"),
					new MenuUtility.MenuSmallCs(
						// 「結果返却済み」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_RESULT),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.ApprovalResult,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "ApprovalResult"),
				}
			},
			new MenuUtility.MenuLargeCs(
				// 「送信代行フロー」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE_SEND),
				"icon-sending-agency-flow")
			{
				SmallMenus = new MenuUtility.MenuSmallCs[]
				{
					new MenuUtility.MenuSmallCs(
						// 「送信代行」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_SEND),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.Send,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "Send"),
					new MenuUtility.MenuSmallCs(
						// 「依頼中」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_REQUEST),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.SendRequest,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "SendRequest"),
					new MenuUtility.MenuSmallCs(
						// 「結果返却済み」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_RESULT),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.SendResult,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "SendResult"),
				}
			},
			new MenuUtility.MenuLargeCs(
				// 「検索」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE_SEARCH),
				"icon-search")
			{
				SmallMenus = new MenuUtility.MenuSmallCs[]
				{
					new MenuUtility.MenuSmallCs(
						// 「検索」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_SEARCH_MESSAGE),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.SearchMessage + "&SearchClear=",
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "SearchMessage"),
				}
			},
			new MenuUtility.MenuLargeCs(
				// 「ごみ箱」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE,
					Constants.VALUETEXT_PARAM_OPERATOR_MENU_LARGE_TRASH),
				"icon-trash")
			{
				SmallMenus = new MenuUtility.MenuSmallCs[]
				{
					new MenuUtility.MenuSmallCs(
						// 「インシデント」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_TRASH_INCIDENT),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.TrashIncident,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "TrashIncident"),
					new MenuUtility.MenuSmallCs(
						// 「メッセージ」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_TRASH_MESSAGE),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.TrashMessage,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "TrashMessage"),
					new MenuUtility.MenuSmallCs(
						// 「承認/送信代行」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_TRASH_REQUEST),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.TrashRequest,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "TrashRequest"),
					new MenuUtility.MenuSmallCs(
						// 「下書き」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_OPERATOR_LEFT_MENU,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL,
							Constants.VALUETEXT_PARAM_OPERATOR_MENU_SMALL_DRAFT),
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + TopPageKbn.TrashDraft,
						Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PATH, "TrashDraft"),
				}
			},
		};
		rLargeMenuOperator.DataSource = menuListOperator;
		rLargeMenuOperator.DataBind();
	}

	/// <summary>
	/// ツリー内のノードが展開されたときの処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Node_Expand(Object sender, TreeNodeEventArgs e)
	{
		NodeChanged(e, true);
	}

	/// <summary>
	/// ツリー内のノードが折りたたまれたときの処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Node_Collapse(Object sender, TreeNodeEventArgs e)
	{
		NodeChanged(e, false);
	}

	/// <summary>
	/// ツリー内のノードが展開/折りたたまれたときの共通処理
	/// </summary>
	/// <param name="e"></param>
	/// <param name="isExpand">trueで展開処理、falseで折りたたみ</param>
	private void NodeChanged(TreeNodeEventArgs e, bool isExpand)
	{
		var treeData = CookieManager.GetValue(COOKIE_NAME_CS_CATEGORY_TREE) ?? "";
		var set = treeData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

		// 展開 or 折りたたみ
		if (isExpand)
		{
			set.Add(e.Node.ValuePath);
		}
		else
		{
			set.Remove(e.Node.ValuePath);
		}

		CookieManager.SetCookie(COOKIE_NAME_CS_CATEGORY_TREE, string.Join(",", set), Constants.PATH_ROOT);
	}

	/// <summary>
	/// SmallMenuに表示するタスクの件数を、文字列で返します。
	/// </summary>
	/// <param name="count">表示件数</param>
	/// <returns>表示文字列（非表示のときは空文字）</returns>
	protected string GetTaskCountString(int? count)
	{
		return count.HasValue ? StringUtility.ToNumeric(count.Value) : "";
	}

	/// <summary>
	/// CSステータスモードが同じか
	/// </summary>
	/// <param name="menu">小メニュー</param>
	/// <returns>同じステータスモードか</returns>
	protected bool IsSameTaskStatusMode(MenuUtility.MenuSmallCs menu)
	{
		var result = (Request.FilePath.Contains(menu.MenuPath)
			&& (string.Compare(
				Request[Constants.REQUEST_KEY_CS_TASKSTATUS_MODE],
				menu.CsKbn,
				StringComparison.CurrentCultureIgnoreCase) == 0));
		return result;
	}

	/// <summary>
	/// CS区分が同じか
	/// </summary>
	/// <param name="menu">小メニュー</param>
	/// <returns>同じCS区分か</returns>
	protected bool IsSameCsKbn(MenuUtility.MenuSmallCs menu)
	{
		return (Request.FilePath.Contains(menu.MenuPath)
			&& (string.Compare(Request[Constants.REQUEST_KEY_CS_TOPPAGE_KBN], menu.CsKbn, StringComparison.CurrentCultureIgnoreCase) == 0));
	}
}
