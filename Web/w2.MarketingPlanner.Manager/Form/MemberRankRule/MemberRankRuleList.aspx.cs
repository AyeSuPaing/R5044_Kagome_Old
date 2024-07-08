/*
=========================================================================================================
  Module      : 会員ランク変動ルール一覧ページ処理(MemberRankRuleList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_MemberRankRule_MemberRankRuleList : BasePage
{
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
			string strMemberRankRuleId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MEMBERRANKRULE_ID]);
			string strMemberRankRuleName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MEMBERRANKRULE_NAME]);
			int iPageNo = 1;
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out iPageNo) == false)
			{
				iPageNo = 1;
			}

			//------------------------------------------------------
			// 一覧取得
			//------------------------------------------------------
			DataView dvList = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetMemberRankRuleList"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strMemberRankRuleId));
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strMemberRankRuleName));
				htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNo - 1) + 1);
				htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNo);

				dvList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			int iTotalMemberRankRuleCount = 0;
			if (dvList.Count != 0)
			{
				trListError.Visible = false;
				iTotalMemberRankRuleCount = (int)dvList[0].Row["row_count"];
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// 一覧セット
			rList.DataSource = dvList;
			rList.DataBind();

			// 検索ボックスセット
			tbMemberRankRuleId.Text = strMemberRankRuleId;
			tbMemberRankRuleName.Text = strMemberRankRuleName;

			// ページャセット
			string strNextUrl = CreateListUrl();
			lbPager.Text = WebPager.CreateDefaultListPager(iTotalMemberRankRuleCount, iPageNo, strNextUrl);
		}
    }

	/// <summary>
	/// 一覧URL作成(ページャ用）
	/// </summary>
	/// <returns></returns>
	protected string CreateListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANKRULE_ID).Append("=").Append(HttpUtility.UrlEncode(tbMemberRankRuleId.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MEMBERRANKRULE_NAME).Append("=").Append(HttpUtility.UrlEncode(tbMemberRankRuleName.Text));

		return sbResult.ToString();
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
	/// <param name="strMemberRankRuleId"></param>
	/// <returns></returns>
	protected string CreateDetailUrl(string strMemberRankRuleId)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANKRULE_ID).Append("=").Append(HttpUtility.UrlEncode(strMemberRankRuleId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbUrl.ToString();
	}

	/// <summary>
	/// 最終付与人数取得
	/// </summary>
	/// <param name="strActionMasterId">アクションマスターID</param>
	/// <returns>最終付与人数/抽出ターゲット数</returns>
	protected string GetLastProgress(string strActionMasterId)
	{
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleLastProgress"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, strActionMasterId);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return (dvTaskSchedule.Count > 0) ? (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS] : "-";
	}

	/// <summary>
	/// 検索ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
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
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		Response.Redirect(sbUrl.ToString());
	}
}
