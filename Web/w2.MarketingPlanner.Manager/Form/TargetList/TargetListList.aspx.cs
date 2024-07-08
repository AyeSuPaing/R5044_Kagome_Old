/*
=========================================================================================================
  Module      : ターゲットリスト設定一覧ページ処理(TargetListList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================

  */
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class Form_TargetList_TargetListList : BasePage
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
			// 検索パラメータ取得、セッションに格納
			var parameters = GetParameters();
			var targetId = parameters[Constants.REQUEST_KEY_TARGET_ID].ToString();
			var targetName = parameters[Constants.REQUEST_KEY_TARGET_NAME].ToString();
			var pageNo = (int)(parameters[Constants.REQUEST_KEY_PAGE_NO]);
			Session[Constants.SESSIONPARAM_KEY_TARGETLIST_SEARCH_INFO] = parameters;

			//------------------------------------------------------
			// 一覧取得
			//------------------------------------------------------
			DataView dvList = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TargetList", "GetTargetLists"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId);
				htInput.Add(Constants.FIELD_TARGETLIST_TARGET_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(targetId));
				htInput.Add(Constants.FIELD_TARGETLIST_TARGET_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(targetName));
				htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNo - 1) + 1);
				htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNo);

				dvList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			int iTotalUserCounts = 0;
			if (dvList.Count != 0)
			{
				trListError.Visible = false;
				iTotalUserCounts = (int)dvList[0].Row["row_count"];
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
			tbTargeId.Text = targetId;
			tbTargetName.Text = targetName;

			// ページャセット
			string strNextUrl = CreateListUrl();
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalUserCounts, pageNo, strNextUrl);
		}
	}

	/// <summary>
	/// 検索ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 一覧URL作成(ページャ用）
	/// </summary>
	/// <returns></returns>
	protected string CreateListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_TARGET_ID).Append("=").Append(HttpUtility.UrlEncode(tbTargeId.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_TARGET_NAME).Append("=").Append(HttpUtility.UrlEncode(tbTargetName.Text));

		return sbResult.ToString();
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="strTargetId"></param>
	/// <returns></returns>
	protected string CreateDetailUrl(string strTargetId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_CONFIRM);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_TARGET_ID).Append("=").Append(HttpUtility.UrlEncode(strTargetId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbResult.ToString();
	}

	/// <summary>
	/// 検索パラメータ取得
	/// </summary>
	/// <returns>検索パラメータ</returns>
	private Hashtable GetParameters()
	{
		Hashtable parameters = new Hashtable();
		// ターゲットID
		parameters.Add(Constants.REQUEST_KEY_TARGET_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_ID]));
		// ターゲット名
		parameters.Add(Constants.REQUEST_KEY_TARGET_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_NAME]));
		// ページ番号
		int pageNo = 0;
		if (!int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo))
		{
			pageNo = 1;
		}
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, pageNo);

		return parameters;
	}

	/// <summary>
	/// 抽出タイミング情報生成
	/// </summary>
	/// <param name="drvTargetList">ターゲットリスト</param>
	/// <returns>抽出タイミング文字列</returns>
	protected string CreateExecTimingString(DataRowView drvTargetList)
	{
		string strResult = null;

		switch ((string)drvTargetList[Constants.FIELD_TARGETLIST_EXEC_TIMING])
		{
			case Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL:
				strResult = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_EXEC_TIMING, (string)drvTargetList[Constants.FIELD_TARGETLIST_EXEC_TIMING]);
				break;
			
			case Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE:
				strResult = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_KBN, (string)drvTargetList[Constants.FIELD_TARGETLIST_SCHEDULE_KBN]);
				break;
		}

		return strResult;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, EventArgs e)
	{
		// Clear Session
		Session[Constants.SESSION_KEY_PARAM] = null;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// アップロードボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpload_Click(object sender, EventArgs e)
	{
		// ファイルが無ければアップロード画面、無ければ確認画面へ遷移
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_UPLOAD);
		url.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPLOAD);
		Response.Redirect(url.ToString());
	}
}
