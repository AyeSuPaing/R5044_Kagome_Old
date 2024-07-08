/*
=========================================================================================================
  Module      : マスタページクラス(MasterBasePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Manager.Menu;
using w2.Domain.ShopOperator;

/// <summary>
/// MasterBasePage の概要の説明です
/// </summary>
public class MasterBasePage : System.Web.UI.MasterPage
{
	/// <summary>
	/// UpdatePanel内でのエラーハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void smScriptManager_AsyncPostBackError(object sender, System.Web.UI.AsyncPostBackErrorEventArgs e)
	{
		AppLogger.WriteError(e.Exception);
#if DEBUG
		// デバッグモードである時に詳細なエラー内容をエラー画面に表示のため、セッションにエラー内容を保持する
		Session[Constants.SESSION_KEY_ERROR_MSG] =
			AppLogger.CreateExceptionMessage(e.Exception).Replace("\r\n", "<br />");
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
#endif
	}

	/// <summary>
	/// TextBoxでEnterキーを押したときのイベントを設定する。
	///   検索ボタンがあれば検索。なければ何もしないようにする。
	/// </summary>
	/// <param name="hdTargetForm">設定対象のForm</param>
	/// <param name="btnDummy">Enterキーのイベントをキャンセルするボタン</param>
	public void SetTextBoxEnterKeyDownEventSearchOrNone(HtmlForm hfTargetForm, Button btnDummy)
	{
		InnerTextBoxList itbcTextBoxList = new InnerTextBoxList(hfTargetForm);
		if (ExistsSearchButton(hfTargetForm))
		{
			// 検索ボタンがあるページは検索ボタンを実行するスクリプトを追加する
			itbcTextBoxList.SetAttributeToSingleLineTextBox("onkeypress", "if (event.keyCode == 13) { __doPostBack('" + GetSearchButton(hfTargetForm).UniqueID + "',''); return false; }");
		}
		else
		{
			// 検索ボタンの無いページではDefaultButtonにキャンセル用のボタンを設定する
			hfTargetForm.DefaultButton = btnDummy.UniqueID;
		}
	}

	/// <summary>
	/// 検索ボタンの存在チェック
	/// </summary>
	/// <param name="hfTargetForm">検索対象のForm</param>
	/// <returns>存在したらTrue</returns>
	public bool ExistsSearchButton(HtmlForm hfTargetForm)
	{
		return GetButtonList(hfTargetForm).Exists((cTarget) => { return cTarget.ID.StartsWith("btnSearch"); });
	}

	/// <summary>
	/// 検索ボタンを取得
	/// </summary>
	/// <param name="hfTargetForm">検索対象のForm</param>
	/// <returns>検索ボタン</returns>
	public Button GetSearchButton(HtmlForm hfTargetForm)
	{
		return GetButtonList(hfTargetForm).Find((cTarget) => { return cTarget.ID.StartsWith("btnSearch"); });
	}

	/// <summary>
	/// ボタンの一覧を取得
	/// </summary>
	/// <param name="hfTargetForm">検索対象のForm</param>
	/// <returns>Formが持っているボタンのリスト</returns>
	private List<Button> GetButtonList(HtmlForm hfTargetForm)
	{
		return new InnerSpecificControlList<Button>(hfTargetForm);
	}

	/// <summary>OnLoadイベントセット</summary>
	public string OnLoadEvents
	{
		get { return m_strOnLoadEvents; }
		set { m_strOnLoadEvents = value; }
	}
	protected string m_strOnLoadEvents = null;
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
}
