/*
=========================================================================================================
  Module      : 集計区分設定一覧ページ処理(SummarySettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.App.Common.Cs.SummarySetting;

public partial class Form_SummarySetting_SummarySettingList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			Initialize();

			DisplayList();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		this.TotalCount = GetToalCount();
		btnInsertTop.Visible
			= btnInsertBotttom.Visible = (this.TotalCount < Constants.MAX_SUMMARY_SETTING_COUNT);
	}
	#endregion

	#region -DisplayList 一覧表示
	/// <summary>
	/// 一覧表示
	/// </summary>
	private void DisplayList()
	{
		var service = new CsSummarySettingService(new CsSummarySettingRepository());
		var settings = service.Search(this.LoginOperatorDeptId);

		if (settings.Length == 0)
		{
			trListError.Visible = true;
			lErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		else
		{
			trListError.Visible = false;
		}

		rList.DataSource = settings;
		rList.DataBind();
	}
	#endregion

	#region #CreateDetailUrl 詳細URL作成
	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="summarySettingNo">問合せ集計区分番号</param>
	/// <returns>URL</returns>
	protected string CreateDetailUrl(int summarySettingNo)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SUMMARYSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_SUMMARY_SETTING_NO + "=" + summarySettingNo
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
	}
	#endregion

	#region #btnInsert_Click 新規登録ボタンクリック
	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// これ以上登録できないときはエラー
		int totalCount = GetToalCount();
		if (totalCount >= Constants.MAX_SUMMARY_SETTING_COUNT)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUMMARYSETTING_REGISTER_ERROR).Replace("@@ 1 @@", Constants.MAX_SUMMARY_SETTING_COUNT.ToString());
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 新規登録画面へ
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SUMMARYSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
	#endregion

	#region -GetToalCount トータル件数取得
	/// <summary>
	/// トータル件数取得
	/// </summary>
	/// <returns>件数</returns>
	private int GetToalCount()
	{
		var service = new CsSummarySettingService(new CsSummarySettingRepository());
		var settings = service.GetAll(this.LoginOperatorDeptId);
		return (settings.Length);
	}
	#endregion

	#region プロパティ
	/// <summary>トータル件数</summary>
	protected int TotalCount
	{
		get { return (int)ViewState["TotalCount"]; }
		set { ViewState["TotalCount"] = value; }
	}
	#endregion 
}