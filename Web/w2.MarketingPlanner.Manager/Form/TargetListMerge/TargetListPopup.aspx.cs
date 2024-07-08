/*
=========================================================================================================
  Module      : ターゲットリストポップアップ一覧ページ処理(TargetListPopup.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;

public partial class Form_TargetListMerge_TargetListPopup : BasePage
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
			// ターゲットリスト一覧取得
			var targetListResult = GetTargetList();

			// 画面表示
			DisplayResult(targetListResult);
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="targetListResult">ターゲットリスト一覧取得</param>
	private void DisplayResult(DataView targetListResult)
	{
		int totalDataCounts = 0;
		if (targetListResult.Count != 0)
		{
			trListError.Visible = false;
			totalDataCounts = (int)targetListResult[0].Row["row_count"];
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// 一覧セット
		rList.DataSource = targetListResult;
		rList.DataBind();

		// 検索ボックスセット
		tbTargeId.Text = this.TargetListId;
		tbTargetName.Text = this.TargetListName;

		// ページャセット
		string nextUrl = CreateListUrl();
		lbPager.Text = WebPager.CreateDefaultListPager(totalDataCounts, this.PageNo, nextUrl);
	}

	/// <summary>
	/// ターゲットリスト一覧取得
	/// </summary>
	/// <returns>ターゲットリスト一覧データ</returns>
	private DataView GetTargetList()
	{
		DataView list = null;
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("TargetList", "GetTargetLists"))
		{
			var input = new Hashtable
				{
					{Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId},
					{Constants.FIELD_TARGETLIST_TARGET_ID + "_like_escaped",StringUtility.SqlLikeStringSharpEscape(this.TargetListId)},
					{Constants.FIELD_TARGETLIST_TARGET_NAME + "_like_escaped",StringUtility.SqlLikeStringSharpEscape(this.TargetListName)},
					{"bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST*(this.PageNo - 1) + 1},
					{"end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST*this.PageNo}
				};
			list = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}
		return list;
	}

	/// <summary>
	/// 一覧URL作成(ページャ用）
	/// </summary>
	/// <returns>一覧URL</returns>
	protected string CreateListUrl()
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST_POPUP);
		url.Append("?").Append(Constants.REQUEST_KEY_TARGET_ID).Append("=").Append(HttpUtility.UrlEncode(tbTargeId.Text));
		url.Append("&").Append(Constants.REQUEST_KEY_TARGET_NAME).Append("=").Append(HttpUtility.UrlEncode(tbTargetName.Text));
		return url.ToString();
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

	#region プロパティ
	/// <summary>ターゲットID（検索用）</summary>
	private string TargetListId { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_ID]); } }
	/// <summary>ターゲット名（検索用）</summary>
	private string TargetListName { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_NAME]); } }
	/// <summary>ページNO</summary>
	private int PageNo
	{
		get
		{
			int pageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) == false) pageNo = 1;
			return pageNo;
		}
	}
	#endregion
}
