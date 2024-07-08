/*
=========================================================================================================
  Module      : 注文ワークフロー設定情報一覧ページ処理(OrderWorkflowSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Web;

public partial class Form_OrderWorkflowSetting_OrderWorkflowSettingList : OrderWorkflowPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			var requestParameters = GetParameters(Request);
			// 不正パラメータが存在した場合
			if ((bool)requestParameters[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// 検索情報保持(編集で利用)
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_ORDER_WORKFLOW_SEARCH_INFO] = requestParameters;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			var sqlParameters = GetSearchSqlInfo(requestParameters);
			var currentPageNumber = (int)requestParameters[Constants.REQUEST_KEY_PAGE_NO];


			//------------------------------------------------------
			// 注文ワークフロー設定該当件数取得
			//------------------------------------------------------
			var totalWorkFlowSettingCount = 0;	// ページング可能総数
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("OrderWorkflowSetting", "GetOrderWorkflowSettingCount"))
			{
				sqlParameters.Add(Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, this.LoginOperatorShopId);
				var workFlowSettingCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParameters);
				if (workFlowSettingCount.Count != 0)
				{
					totalWorkFlowSettingCount = (int)workFlowSettingCount[0]["row_count"];
				}
			}

			if (totalWorkFlowSettingCount > 0)
			{
				int lastPage = (totalWorkFlowSettingCount - 1) / Constants.CONST_DISP_CONTENTS_ORDERWORKFLOWSETTING_LIST + 1;
				currentPageNumber = currentPageNumber > lastPage ? lastPage : currentPageNumber;

				//------------------------------------------------------
				// 注文ワークフロー設定情報一覧
				//------------------------------------------------------
				DataView workflowList = GetOrderWorkflowSettingList(sqlParameters, currentPageNumber);

				// データソースセット
				rList.DataSource = workflowList;
				rList.DataBind();

			}

			if (totalWorkFlowSettingCount != 0)
			{
				trListError.Visible = false;
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			lbPager1.Text = WebPager.CreateOrderWorkflowSettingListPager(
				totalWorkFlowSettingCount,
				currentPageNumber,
				CreatOrderWorkflowSettingListUrl(requestParameters, false, null));

			// 最大件数に達していたら新規登録ボタンを無効にする
			if (IsLessOrderWorkflowSettingThenMaxCount() == false)
			{
				btnInsertTop.Enabled = btnInsertBotttom.Enabled = false;
				btnInsertTop.ToolTip = btnInsertBotttom.ToolTip = string.Format(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_REGISTER_OVER_MAX_COUNT),
					Constants.ORDERWORKFLOWSETTING_MAXCOUNT);
			}
			else
			{
				btnInsertTop.Enabled = btnInsertBotttom.Enabled = true;
			}
		}
	}

	/// <summary>
	/// SQL検索情報取得
	/// </summary>
	/// <param name="searchParameters">検索情報</param>
	/// <returns>SQL検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable searchParameters)
	{
		return new Hashtable()
		{
			{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, StringUtility.ToEmpty(searchParameters[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN])},
			{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, StringUtility.ToEmpty(searchParameters[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN])},
			{Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG, StringUtility.ToEmpty(searchParameters[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_VALID_FLG])},
			{Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG, StringUtility.ToEmpty(searchParameters[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ADDITIONAL_SEARCH_FLG])},
			{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParameters[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NAME])},
			{"sort_kbn", StringUtility.ToEmpty(searchParameters[Constants.REQUEST_KEY_SORT_KBN])},
		};
	}

	/// <summary>
	/// 注文ワークフロー設定情報データビューを表示分だけ取得
	/// </summary>
	/// <param name="sqlParameters">SQLパラメタ情報</param>
	/// <param name="currentPageNumber">表示開始記事番号</param>
	/// <returns>注文ワークフロー設定情報データビュー</returns>
	private DataView GetOrderWorkflowSettingList(Hashtable sqlParameters, int currentPageNumber)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderWorkflowSetting", "GetOrderWorkflowSettingList"))
		{
			sqlParameters.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_ORDERWORKFLOWSETTING_LIST * (currentPageNumber - 1) + 1);
			sqlParameters.Add("end_row_num", Constants.CONST_DISP_CONTENTS_ORDERWORKFLOWSETTING_LIST * currentPageNumber);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParameters);
		}
	}

	/// <summary>
	/// データバインド用注文ワークフロー設定情報編集URL作成
	/// </summary>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <param name="workflowKbn">ワークフロー区分</param>
	/// <param name="workflowNo">枝番</param>
	/// <returns>注文ワークフロー設定情報編集URL</returns>
	protected string CreateOrderWorkflowSettingEditUrl(string workflowType, string workflowKbn, int workflowNo)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOWSETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_WORKFLOW_TYPE, workflowType)
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn)
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo.ToString())
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE);

		return url.CreateUrl();
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 新規登録画面へ
		Response.Redirect(
			Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOWSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// ワークフロー区分
		ddlWorkflowKbn.Items.Add(new ListItem("", ""));
		ddlWorkflowKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN));

		// ワークフロー詳細区分
		ddlWorkFlowDetailKbn.Items.Add(new ListItem("", ""));
		ddlWorkFlowDetailKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN));

		// 有効フラグ
		ddlValidFlg.Items.Add(new ListItem("", ""));
		ddlValidFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG));
	}

	/// <summary>
	/// ワークフロー一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">ワークフロー一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable resultParameters = new Hashtable();
		int currentPageNumber = 1;
		bool paramError = false;

		try
		{
			// ワークフロー区分
			resultParameters.Add(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN]));
			ddlWorkflowKbn.SelectedValue = hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN];
			// ワークフロー名
			resultParameters.Add(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NAME, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NAME]));
			tbWorkflowName.Text = hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NAME];
			// 有効フラグ
			resultParameters.Add(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_VALID_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_VALID_FLG]));
			ddlValidFlg.SelectedValue = hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_VALID_FLG];
			// ワークフロー詳細区分
			resultParameters.Add(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN]));
			ddlWorkFlowDetailKbn.SelectedValue = hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN];
			// 追加検索可否FLG
			resultParameters.Add(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ADDITIONAL_SEARCH_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ADDITIONAL_SEARCH_FLG]));
			ddlAdditionalSearchFlg.SelectedValue = hrRequest[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ADDITIONAL_SEARCH_FLG];
			// ソート区分
			string sortKbn = null;
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_WORKFLOW_KBN_ASC: // ワークフロー区分
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_DISPLAY_ORDER_ASC: // 実行順/昇順
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_DISPLAY_ORDER_DESC: // 実行順/降順
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CREATED_ASC: // 作成日/昇順
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CREATED_DESC: // 作成日/降順
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CHANGED_ASC: // 更新日/昇順
				case Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CHANGED_DESC: // 更新日/降順
					sortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN];
					break;
				case "":
					sortKbn = Constants.KBN_SORT_ORDERWORKFLOWSETTING_LIST_WORKFLOW_KBN_ASC; // ワークフロー区分
					break;
				default:
					paramError = true;
					break;
			}
			resultParameters.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			ddlSortKbn.SelectedValue = sortKbn;
			// ページ番号（ページャ動作時のみもちまわる）
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				currentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			paramError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		resultParameters.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);
		resultParameters.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return resultParameters;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// ワークフロー設定一覧へ
		Response.Redirect(CreatOrderWorkflowSettingListUrl(GetSearchInfo(), true, 1));
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo()
	{
		// 変数宣言
		return new Hashtable() {
			{Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN, ddlWorkflowKbn.SelectedValue},	// ワークフロー区分 
			{Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NAME, tbWorkflowName.Text.Trim()},		// ワークフロー名
			{Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_VALID_FLG, ddlValidFlg.SelectedValue},		// 有効フラグ
			{Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, ddlWorkFlowDetailKbn.SelectedValue},	// ワークフロー詳細区分
			{Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ADDITIONAL_SEARCH_FLG, ddlAdditionalSearchFlg.SelectedValue},	// 追加検索可否FLG
			{Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue}	 // ソート区分
		};
	}
}

