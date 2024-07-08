/*
=========================================================================================================
  Module      : ポイントルールスケジュール一覧ページ処理(PointRuleScheduleList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
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
using w2.Common.Web;
using w2.Domain.Point;
using w2.Domain.Point.Helper;

public partial class Form_PointRuleSchedule_PointRuleScheduleList : BasePage
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
			ViewPointRuleScheduleList();
		}
	}

	/// <summary>
	/// ポイントルールスケジュール一覧表示
	/// </summary>
	private void ViewPointRuleScheduleList()
	{
		// 変数宣言
		Hashtable param = new Hashtable();

		// リクエスト情報取得
		param = GetParameters(Request);
		// 不正パラメータが存在した場合
		if ((bool)param[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// ポイントルールスケジュール情報一覧
		int totalPointRuleCounts = 0;	// ページング可能総数

		//検索実行
		var result = new PointService().PointRuleScheduleListSearch(GetSearchCondition(param));
		if (result.Any())
		{
			totalPointRuleCounts = result.FirstOrDefault().RowCount;
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			totalPointRuleCounts = 0;
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

		}
		// データソースセット
		rList.DataSource = result.Select(x => new WrappedSearchResult(x)).ToArray();


		// ページャ作成（一覧処理で総件数を取得）
		string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_LIST;
		lbPager.Text = WebPager.CreateDefaultListPager(totalPointRuleCounts, (int)param[Constants.REQUEST_KEY_PAGE_NO], strNextUrl);

		// データバインド
		DataBind();
	}

	/// <summary>
	/// パラメータ取得
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <returns>パラメータ</returns>
	private Hashtable GetParameters(System.Web.HttpRequest request)
	{
		var result = new Hashtable();
		var paramError = false;

		// ポイントルールスケジュールID
		try
		{
			result.Add(Constants.REQUEST_KEY_POINTRULESCHEDULE_ID, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_POINTRULESCHEDULE_ID]));
			tbPointRuleScheduleId.Text = request[Constants.REQUEST_KEY_POINTRULESCHEDULE_ID];
		}
		catch
		{
			paramError = true;
		}

		// ポイントルールスケジュール名
		try
		{
			result.Add(Constants.REQUEST_KEY_POINTRULESCHEDULE_NAME, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_POINTRULESCHEDULE_NAME]));
			tbPointRuleScheduleName.Text = request[Constants.REQUEST_KEY_POINTRULESCHEDULE_NAME];
		}
		catch
		{
			paramError = true;
		}

		// ページ番号
		var currentPageNumber = 1;
		try
		{
			if (StringUtility.ToEmpty(request[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				currentPageNumber = int.Parse(request[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			paramError = true;
		}
		result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);

		result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return result;
	}

	/// <summary>
	/// 検索条件取得
	/// </summary>
	/// <param name="param">パラメータ</param>
	/// <returns>検索条件</returns>
	private PointRuleScheduleListSearchCondition GetSearchCondition(Hashtable param)
	{
		return new PointRuleScheduleListSearchCondition
		{
			PointRuleScheduleId = StringUtility.ToEmpty(param[Constants.REQUEST_KEY_POINTRULESCHEDULE_ID]),
			PointRuleScheduleName = StringUtility.ToEmpty(param[Constants.REQUEST_KEY_POINTRULESCHEDULE_NAME]),
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * ((int)param[Constants.REQUEST_KEY_PAGE_NO] - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (int)param[Constants.REQUEST_KEY_PAGE_NO],
		};
	}

	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <param name="iPageNo"></param>
	/// <returns></returns>
	protected string CreateListUrl(int iPageNo)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_LIST)
			.AddParam(Constants.REQUEST_KEY_POINTRULESCHEDULE_ID, tbPointRuleScheduleId.Text)
			.AddParam(Constants.REQUEST_KEY_POINTRULESCHEDULE_NAME, tbPointRuleScheduleName.Text)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, iPageNo.ToString());

		return url.CreateUrl();
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
	/// <returns>URL</returns>
	protected string CreateDetailUrl(string pointRuleScheduleId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_POINTRULESCHEDULE_ID, pointRuleScheduleId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);

		return url.CreateUrl();
	}

	/// <summary>
	/// 最終付与人数取得
	/// </summary>
	/// <param name="strActionMasterId">アクションマスターID</param>
	/// <returns>最終付与人数/抽出ターゲット数</returns>
	protected string GetLastProgress(string strActionMasterId)
	{
		DataView taskSchedule = null;
		using (SqlAccessor accessor = new SqlAccessor())
		using (SqlStatement statement = new SqlStatement("TaskSchedule", "GetTaskScheduleLastProgress"))
		{
			var param = new Hashtable
			{
				{Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT},
				{Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, strActionMasterId},
			};
			taskSchedule = statement.SelectSingleStatementWithOC(accessor, param);
		}

		return (taskSchedule.Count > 0) ? (string)taskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS] : "-";
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

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT);
		Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO] = null;

		Response.Redirect(url.CreateUrl());
	}

	#region 検索結果のラッパークラス
	/// <summary>
	/// 検索結果のラッパークラス
	/// </summary>
	[Serializable]
	protected class WrappedSearchResult : PointRuleScheduleListSearchResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result"></param>
		public WrappedSearchResult(PointRuleScheduleListSearchResult result)
			: base(result.DataSource)
		{

		}
	}
	#endregion
}
