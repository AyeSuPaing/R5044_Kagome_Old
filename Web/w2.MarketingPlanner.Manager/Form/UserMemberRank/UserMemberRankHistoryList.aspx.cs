/*
=========================================================================================================
  Module      : 会員ランク更新履歴一覧ページ処理(UserMemberRankHistoryList.aspx.cs)
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
using w2.App.Common.Util;
using System.Globalization;
using w2.Common.Web;

public partial class Form_UserMemberRank_UserMemberRankHistoryList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			Hashtable htParam = new Hashtable();

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			htParam = GetParameters(Request);

			//------------------------------------------------------
			// パラメタセット
			//------------------------------------------------------
			tbUserId.Text = (string)htParam[Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID];		// ユーザーID
			// 更新前ランクID
			foreach (ListItem li in ddlBeforeRankId.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID]);
			}
			// 更新後ランクID
			foreach (ListItem li in ddlAfterRankId.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID]);
			}
			ucUpdateDate.HfStartDate.Value =
				StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM]);

			ucUpdateDate.HfEndDate.Value =
				StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO]);
			//------------------------------------------------------
			// 検索情報取得
			//------------------------------------------------------
			Hashtable htSearch = GetSearchSqlInfo(htParam);
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// 更新履歴の該当件数取得
			//------------------------------------------------------
			int iTotalMemberRankHisCount = GetMemberRankHisCount(htSearch);	// ページング可能総更新履歴数

			//------------------------------------------------------
			// 更新履歴一覧表示
			//------------------------------------------------------
			// 表示可能上限数を超えている場合
			if (iTotalMemberRankHisCount > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				// エラー表示制御
				trListError.Visible = true;
				StringBuilder sbErrorMsg = new StringBuilder();
				sbErrorMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				sbErrorMsg.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalMemberRankHisCount));
				sbErrorMsg.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));
				tdErrorMessage.InnerHtml = sbErrorMsg.ToString();
			}
			else
			{
				// 更新履歴取得
				DataView dvMemberRankHis = GetMemberRankHisDataView(htSearch, iCurrentPageNumber);

				// 会員ランク更新履歴が存在する場合
				if (dvMemberRankHis.Count != 0)
				{
					// エラー非表示制御
					trListError.Visible = false;
				}
				else
				{
					// エラー表示制御
					trListError.Visible = true;
					tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				}
				// データソースセット
				rList.DataSource = dvMemberRankHis;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = CreateMemberRankHisListUrl(htParam);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalMemberRankHisCount, iCurrentPageNumber, strNextUrl);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		//------------------------------------------------------
		// 抽出ランク設定
		//------------------------------------------------------
		DataView dvMemberRanks = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatetment = new SqlStatement("MemberRank", "GetMemberRankListAll"))
		{
			dvMemberRanks = sqlStatetment.SelectSingleStatementWithOC(sqlAccessor);
		}
		// 更新前ランク
		ddlBeforeRankId.Items.Add(new ListItem("", ""));
		foreach (DataRowView drv in dvMemberRanks)
		{
			ddlBeforeRankId.Items.Add(new ListItem((string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME], (string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]));
		}
		// 更新後ランク
		ddlAfterRankId.Items.Add(new ListItem("", ""));
		foreach (DataRowView drv in dvMemberRanks)
		{
			ddlAfterRankId.Items.Add(new ListItem((string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME], (string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]));
		}
	}

	/// <summary>
	/// 会員ランク更新履歴一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">会員ランク更新履歴一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();
		int iCurrentPageNumber = 1;
		string strSortKbn = String.Empty;

		// ユーザーIDのリクエストキーを取得
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID,
			StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID]));

		// 更新前・後ランクIDのリクエストキーの取得
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID]));
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID]));

		// Set request key member rank history date from to
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM]));
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO]));

		// Set request key member rank history time from to
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM]));
		htResult.Add(
			Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO]));

		var userMemberRankHistoryFrom = string.Format("{0}{1}",
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM])
			.Replace("/", string.Empty),
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM])
			.Replace(":", string.Empty));

		var userMemberRankHistoryTo = string.Format("{0}{1}",
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO])
			.Replace("/", string.Empty),
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO])
			.Replace(":", string.Empty));

		ucUpdateDate.SetPeriodDate(
			userMemberRankHistoryFrom,
			userMemberRankHistoryTo);

		// ページ番号（ページャ動作時のみもちまわる）
		if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
		{
			iCurrentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
		}

		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);

		return htResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htSearch)
	{
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		// ユーザーID
		htResult.Add(Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID]));
		// 更新前ランクID
		htResult.Add(Constants.FIELD_USERMEMBERRANKHISTORY_BEFORE_RANK_ID,
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID]));
		// 更新後ランクID
		htResult.Add(Constants.FIELD_USERMEMBERRANKHISTORY_AFTER_RANK_ID,
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID]));
		// 更新履歴抽出日初期化
		htResult.Add(Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED + "_from", System.DBNull.Value);
		htResult.Add(Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED + "_to", System.DBNull.Value);
		// 更新履歴抽出日(From)
		string strDateFrom = (string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM];
		if (Validator.IsDate(strDateFrom))
		{
			htResult[Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED + "_from"] = string.Format("{0} {1}",
				strDateFrom,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM]);
		}
		// 更新履歴抽出日(To)
		string strDateTo = (string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO];
		if (Validator.IsDate(strDateTo))
		{
			var dateTimeTo = DateTime.Parse(string.Format("{0} {1}",
				strDateTo,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO])).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			htResult[Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED + "_to"] = dateTimeTo;
		}

		return htResult;
	}

	/// <summary>
	/// 更新履歴から検索条件に該当する件数を取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>更新履歴の検索該当件数</returns>
	private int GetMemberRankHisCount(Hashtable htSearch)
	{
		int iTotalMemberRankHisCount = 0;

		// ステートメントから更新履歴の該当件数取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("UserMemberRank", "GetUserMemberRankHisCount"))
		{
			DataView dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearch);
			if (dv.Count != 0)
			{
				iTotalMemberRankHisCount = (int)dv[0]["row_count"];
			}
		}

		return iTotalMemberRankHisCount;
	}

	/// <summary>
	/// 更新履歴一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>更新履歴一覧データビュー</returns>
	private DataView GetMemberRankHisDataView(Hashtable htSearch, int iPageNumber)
	{
		DataView dvResult = null;

		// ステートメントから更新履歴一覧データ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("UserMemberRank", "GetUserMemberRankHisList"))
		{
			htSearch.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1);	// 表示開始記事番号
			htSearch.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber);				// 表示開始記事番号

			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htSearch);
			dvResult = ds.Tables["Table"].DefaultView;
		}

		return dvResult;
	}

	/// <summary>
	/// 更新履歴一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>更新履歴一覧遷移URL</returns>
	private string CreateMemberRankHisListUrl(Hashtable htSearch)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_MEMBER_RANK_HISTORY_LIST)
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID])
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID])
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID])
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM])
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM])
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO])
			.AddParam(
				Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO,
				(string)htSearch[Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO])
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_MEMBER_RANK_HISTORY_LIST)
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_USER_ID, tbUserId.Text.Trim())
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID, ddlBeforeRankId.SelectedValue)
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID, ddlAfterRankId.SelectedValue)
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM, ucUpdateDate.HfStartDate.Value)
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM, ucUpdateDate.HfStartTime.Value)
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO, ucUpdateDate.HfEndDate.Value)
			.AddParam(Constants.REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO, ucUpdateDate.HfEndTime.Value)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, "1")
			.CreateUrl();
		Response.Redirect(url);
	}
}