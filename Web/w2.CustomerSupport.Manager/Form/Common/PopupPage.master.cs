/*
=========================================================================================================
  Module      : ポップアップマスタページ処理(PopupPage.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.App.Common.OperationLog;
using w2.Common.Web;

public partial class Form_Common_PopupPage : MasterBasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		OperationLogWriter.WriteOperationLog(this.LoginOperatorId, Request.UserHostAddress, Session.SessionID, this.RawUrl);

		//------------------------------------------------------
		// OnLoadイベント設定
		//------------------------------------------------------
		if (!IsPostBack)
		{
			//LogデータをJsonファイルに記録
			OperationLogWriter.SendJsonAndWriteOnFailure(
				Constants.PLAN_NAME,
				Constants.PROJECT_NO,
				Constants.ENVIRONMENT_NAME,
				this.LoginOperatorId,
				this.LoginOperatorName,
				Request.UserHostAddress,
				Session.SessionID,
				this.RawUrl,
				this.RawUrl.Split('?')[0],
				this.Request.Url.Query);

			if (Request[Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW] == Constants.KBN_RELOAD_PARENT_WINDOW_ON)
			{
				// 親画面リロード関数セット
				m_strOnLoadEvents += "reload_parent_window();";
			}

			ViewState["OnLoadEvent"] = m_strOnLoadEvents;
		}
		else
		{
			m_strOnLoadEvents = (string)ViewState["OnLoadEvent"];
		}

		//------------------------------------------------------
		// テキストボックスでエンターキーを押した時の動作を制御
		// （検索、または何もしない様にする）
		//------------------------------------------------------
		SetTextBoxEnterKeyDownEventSearchOrNone(form1, btnDummy);
	}

	/// <summary>閉じるボタンを隠すかどうか</summary>
	public bool HideCloseButton
	{
		get { return (bool)(ViewState["HideCloseButton"] ?? false); }
		set { ViewState["HideCloseButton"] = value; }
	}
}
