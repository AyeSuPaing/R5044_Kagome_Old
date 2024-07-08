/*
=========================================================================================================
  Module      : メール振分設定基底ページ(MailAssignSettingPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.MailAssignSetting;

/// <summary>
/// MailAssignSetting の概要の説明です
/// </summary>
public abstract class MailAssignSettingPage : BasePageCs
{
	/// <summary>
	/// 一覧画面URL作成
	/// </summary>
	/// <returns>一覧画面URL</returns>
	protected string CreateListUrl()
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILASSIGNSETTING_LIST;
	}

	/// <summary>
	/// 詳細画面URL作成
	/// </summary>
	/// <param name="mailAssignId">メール振分設定ID</param>
	/// <returns>詳細画面URL</returns>
	protected string CreateDetailUrl(string mailAssignId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILASSIGNSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL
			+ "&" + Constants.REQUEST_KEY_MAIL_ASSIGN_ID + "=" + StringUtility.ToEmpty(mailAssignId);
	}

	/// <summary>
	/// 登録画面（新規登録）URL作成
	/// </summary>
	/// <returns>登録画面URL</returns>
	protected string CreateRegisterInsertUrl()
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILASSIGNSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT;
	}

	/// <summary>
	/// 登録画面（更新）URL作成
	/// </summary>
	/// <param name="mailAssignId">メール振分設定ID</param>
	/// <returns>登録画面URL</returns>
	protected string CreateRegisterUpdateUrl(string mailAssignId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILASSIGNSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE
			+ "&" + Constants.REQUEST_KEY_MAIL_ASSIGN_ID + "=" + StringUtility.ToEmpty(mailAssignId);
	}

	/// <summary>
	/// 確認画面（新規登録）URL作成
	/// </summary>
	/// <returns>確認画面URL</returns>
	protected string CreateInsertConfirmUrl()
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILASSIGNSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT;
	}

	/// <summary>
	/// 確認画面（更新）URL作成
	/// </summary>
	/// <param name="mailAssignId">メール振分設定ID</param>
	/// <returns>確認画面URL</returns>
	protected string CreateUpdateConfirmUrl(string mailAssignId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILASSIGNSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE
			+ "&" + Constants.REQUEST_KEY_MAIL_ASSIGN_ID + "=" + StringUtility.ToEmpty(mailAssignId);
	}

	#region プロパティ
	/// <summary>メール振分設定ID</summary>
	protected string MailAssignId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAIL_ASSIGN_ID]); }
	}
	/// <summary>登録・更新対象のモデル</summary>
	public CsMailAssignSettingModel MailAssignSettingModel
	{
		get { return (CsMailAssignSettingModel)Session[Constants.SESSION_KEY_MAILASSIGNSETTING_INFO]; }
		set { Session[Constants.SESSION_KEY_MAILASSIGNSETTING_INFO] = value; }
	}
	#endregion
}
