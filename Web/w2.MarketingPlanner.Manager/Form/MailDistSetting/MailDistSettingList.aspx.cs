/*
=========================================================================================================
  Module      : メール配信設定一覧ページ処理(MailDistSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class Form_MailDistSetting_MailDistSettingList : BasePage
{
	private string m_strMailSetId = null;
	private string m_strMailSetName = null;
	private int m_iPageNo = 0;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得
			//------------------------------------------------------
			m_strMailSetId = StringUtility.ToEmpty(Request["maildist_id"]);
			m_strMailSetName = StringUtility.ToEmpty(Request["maildist_name"]);
			if (int.TryParse(StringUtility.ToEmpty(Request["pno"]), out m_iPageNo) == false)
			{
				this.m_iPageNo = 1;
			}

			//------------------------------------------------------
			// 一覧取得
			//------------------------------------------------------
			DataView dvMailDistSettingList = null;
			using (SqlAccessor sqlAcc = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement("MailDistSetting", "GetMailDistSettingList"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
				htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(m_strMailSetId));
				htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(m_strMailSetName));
				htInput.Add("bgn_row_num", (Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (m_iPageNo - 1)) + 1);
				htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * m_iPageNo);

				dvMailDistSettingList = statement.SelectSingleStatementWithOC(sqlAcc, htInput);
			}
			int iTotal = 0;
			if (dvMailDistSettingList.Count != 0)
			{
				trListError.Visible = false;
				iTotal = (int)dvMailDistSettingList[0]["row_count"];
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
			rList.DataSource = dvMailDistSettingList;
			rList.DataBind();
			tbMailDistSetId.Text = m_strMailSetId;
			tbMailDistSetName.Text = m_strMailSetName;

			//------------------------------------------------------
			// ページャ作成
			//------------------------------------------------------
			string strPageUrl = this.CreateListUrl();
			lbPager.Text = WebPager.CreateDefaultListPager(iTotal, m_iPageNo, strPageUrl);
		}
	}

	/// <summary>
	/// 最終配信件数取得
	/// </summary>
	/// <param name="strActionMasterId">アクションマスターID</param>
	/// <returns>最終配信件数/抽出ターゲット数</returns>
	protected string GetLastProgress(string strActionMasterId)
	{
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleLastProgress"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, m_strMailSetId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, strActionMasterId);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return (dvTaskSchedule.Count > 0) ? (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS] : "-";
	}

	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <returns></returns>
	protected string CreateListUrl()
	{
		StringBuilder sbListUrl = new StringBuilder();
		sbListUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_LIST);
		sbListUrl.Append("?").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailSetId));
		sbListUrl.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_NAME).Append("=").Append(HttpUtility.UrlEncode(m_strMailSetName));
		return sbListUrl.ToString();
	}
	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <param name="iPageNo"></param>
	/// <returns></returns>
	protected string CreateListUrl(int iPageNo)
	{
		return (CreateListUrl() + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNo.ToString());
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="strSettingId"></param>
	/// <returns></returns>
	protected string CreateDetailUrl(string strSettingId)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode(strSettingId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		return sbUrl.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		m_strMailSetId = tbMailDistSetId.Text;
		m_strMailSetName = tbMailDistSetName.Text;

		Response.Redirect(CreateListUrl(1));
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		Response.Redirect(sbUrl.ToString());
	}

}
