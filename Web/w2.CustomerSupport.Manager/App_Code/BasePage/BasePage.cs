/*
=========================================================================================================
  Module      : 基底ページ(BasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using w2.Common.Web;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Manager.Menu;
using w2.App.Common.Web.Page;
using w2.Domain.ShopOperator;

/// <summary>
/// BasePage の概要の説明です
/// </summary>
public partial class BasePage : CommonPage
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>Page.MasterPageFile の書き換えは Page_PreInitでないといけない</remarks>
	public void Page_PreInit(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
			//------------------------------------------------------
			CheckHttps(Constants.PROTOCOL_HTTPS + Request.Url.Authority + this.RawUrl);

			//------------------------------------------------------
			// ポップアップ画面リダイレクト処理
			// （リファラにポップアップフラグが含まれていたら、パラメタ付加してリダイレクト）
			//------------------------------------------------------
			if ((Request[Constants.REQUEST_KEY_WINDOW_KBN] != Constants.KBN_WINDOW_POPUP)
				&& (Request.UrlReferrer != null))
			{
				if ((Request.UrlReferrer.AbsolutePath.EndsWith(Constants.PAGE_MANAGER_WEBFORMS_SINGLE_SIGN_ON) == false)
					&& HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP)
				{
					StringBuilder sbRedirectUrl = new StringBuilder(Request.Url.PathAndQuery);
					sbRedirectUrl.Append((Request.Url.Query == "") ? "?" : "&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(Constants.KBN_WINDOW_POPUP);

					Response.Redirect(sbRedirectUrl.ToString());
				}
			}
		}

		//------------------------------------------------------
		// ポップアップフラグが立っていたらポップアップマスタを読み込み（ポストバック時も呼び出す必要あり）
		//------------------------------------------------------
		if (Page.Master != null) // ログインページ対策
		{
			if (Request[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP)
			{
				Page.MasterPageFile = "~/Form/Common/PopupPage.master";
			}
		}
	}

	/// <summary>
	/// HTTPS通信チェック
	/// </summary>
	public virtual void CheckHttps()
	{
		// HTTPS通信チェック（ＮＧの場合トップページへ遷移）
		CheckHttps(Constants.PATH_ROOT);
	}
	/// <summary>
	/// HTTPS通信チェック
	/// </summary>
	/// <param name="strRedirectUrl">ＮＧ時リダイレクトURL</param>
	public void CheckHttps(string strRedirectUrl)
	{
		WebUtility.CheckHttps(Request, Response, strRedirectUrl);
	}

	/// <summary>
	/// 処理区分チェック
	/// </summary>
	/// <param name="strActionStatus">処理区分</param>
	/// <remarks>
	/// 不正な処理区分の場合、エラー画面へ遷移
	/// </remarks>
	protected void CheckActionStatus(string strActionStatus)
	{
		// アクションステータスがマッチしていない場合
		if (this.ActionStatus != strActionStatus)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 文字列を省略する
	/// </summary>
	/// <param name="value">指定文字列</param>
	/// <param name="length">文字列数</param>
	/// <returns>省略文字列</returns>
	protected string AbbreviateString(string value, int length)
	{
		return StringUtility.StrTrim(value, length, "...");
	}

	/// <summary>
	/// 詳細表示用サイト名取得
	/// </summary>
	/// <param name="strMallId">モールID</param>
	/// <param name="strMallName">モール名</param>
	/// <returns>サイト名（モール名＋モールID）</returns>
	public static string CreateSiteNameForDetail(string strMallId, string strMallName)
	{
		StringBuilder sbSiteName = new StringBuilder();
		if (strMallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
		{
			sbSiteName.Append(strMallName);
			sbSiteName.Append(" (モールID：").Append(strMallId).Append(")");
		}
		else
		{
			sbSiteName.Append(ValueText.GetValueText("SiteName", "OwnSiteName", Constants.FLG_USER_MALL_ID_OWN_SITE));
		}
		return sbSiteName.ToString();
	}

	/// <summary>
	/// ブラウザーのキャッシュ削除
	/// </summary>
	protected void ClearBrowserCache()
	{
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();
	}

	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl
	{
		get { return w2.Common.Web.WebUtility.GetRawUrl(Request); }
	}
	/// <summary>ログイン店舗オペレータ</summary>
	protected ShopOperatorModel LoginShopOperator
	{
		get { return (ShopOperatorModel)Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] ?? new ShopOperatorModel(); }
	}
	/// <summary>ログインオペレータ店舗ID</summary>
	protected string LoginOperatorShopId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータ識別ID</summary>
	protected string LoginOperatorDeptId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータID</summary>
	protected string LoginOperatorId
	{
		get { return this.LoginShopOperator.OperatorId; }
	}
	/// <summary>ログインオペレータ名</summary>
	protected string LoginOperatorName
	{
		get { return this.LoginShopOperator.Name; }
	}
	/// <summary>ログインオペレータメニュー</summary>
	protected List<MenuLarge> LoginOperatorMenu
	{
		get { return (List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
	/// <summary>ログインオペレータメニュー権限</summary>
	protected string LoginOperatorMenuAccessLevel
	{
		get { return Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL].ToString(); }
	}
	/// <summary>ログインオペレータCSグループID</summary>
	protected string[] LoginOperatorCsGroupIds
	{
		get { return (string[])Session[Constants.SESSION_KEY_LOGIN_OPERTOR_CS_GROUP_IDS]; }
		set { Session[Constants.SESSION_KEY_LOGIN_OPERTOR_CS_GROUP_IDS] = value; }
	}
	/// <summary>ログインCSオペレータ情報</summary>
	protected CsOperatorModel LoginOperatorCsInfo
	{
		get { return (CsOperatorModel)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_CS_GINFO]; }
		set { Session[Constants.SESSION_KEY_LOGIN_OPERTOR_CS_GINFO] = value; }
	}
	/// <summary>アクションステータス</summary>
	protected string ActionStatus
	{
		get { return (string)Request[Constants.REQUEST_KEY_ACTION_STATUS]; }
	}
	/// <summary>サイトのCSSクラス名</summary>
	protected string SiteCssClassName
	{
		get
		{
			if (this.IsW2Cs) return "";
			if (this.IsRepeatPlus) return "repeatplus";
			if (this.IsRepeatFood) return "repeatfood";
			return "hanyou";
		}
	}
	/// <summary>w2CSか</summary>
	protected bool IsW2Cs
	{
		get { return (this.ManagerDesingSettingDirName == Constants.KBN_MANAGER_DESIGN_SETTING_W2); }
	}
	/// <summary>RepeatPlusか</summary>
	protected bool IsRepeatPlus
	{
		get { return (this.ManagerDesingSettingDirName == Constants.KBN_MANAGER_DESIGN_SETTING_REPEATPLUS); }
	}
	/// <summary>RepeatFoodか</summary>
	protected bool IsRepeatFood
	{
		get { return (this.ManagerDesingSettingDirName == Constants.KBN_MANAGER_DESIGN_SETTING_REPEATFOOD); }
	}
	/// <summary>w2製品か</summary>
	protected bool IsW2Product
	{
		get { return (this.IsW2Cs || this.IsRepeatPlus || this.IsRepeatFood); }
	}
	/// <summary>管理画面デザイン管理ディレクトリ名</summary>
	protected string ManagerDesingSettingDirName
	{
		get { return Constants.MANAGER_DESIGN_SETTING; }
	}
	/// <summary>プレビュー対象HTMLリスト</summary>
	protected List<string> HtmlForPreviewList
	{
		get
		{
			return (Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST] != null)
				? (List<string>)Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST]
				: null;
		}
		set { Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST] = value; }
	}
}
