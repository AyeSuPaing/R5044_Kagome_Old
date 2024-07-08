/*
=========================================================================================================
  Module      : 集計区分設定確認ページ処理(SummarySettingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.App.Common.Cs.SummarySetting;

public partial class Form_SummarySetting_SummarySettingConfirm : BasePage
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
			// 初期化
			Initialize();

			// 表示
			DisplayData();
		}
	}
	#endregion

	#region -Initialize コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			// 新規・コピー新規
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				btnInsertTop.Visible = true;
				btnInsertBottom.Visible = true;
				trConfirm.Visible = true;
				trSummarySettingNo.Visible = false;
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				btnUpdateTop.Visible = true;
				btnUpdateBottom.Visible = true;
				trConfirm.Visible = true;
				break;

			// 詳細
			case Constants.ACTION_STATUS_DETAIL:
				btnEditTop.Visible = true;
				btnEditBottom.Visible = true;
				trDateCreated.Visible = true;
				trDateChanged.Visible = true;
				trLastChanged.Visible = true;
				trDetail.Visible = true;
				break;
		}
	}
	#endregion

	#region -DisplayData 画面表示データセット
	/// <summary>
	/// 画面表示データセット
	/// </summary>
	private void DisplayData()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				CheckActionStatus(this.ActionStatus);	// 処理区分チェック
				this.SummarySettingModel = (CsSummarySettingModel)Session[Constants.SESSION_KEY_SUMMARYSETTING_INFO];
				break;

			case Constants.ACTION_STATUS_DETAIL:
				var summarySettingService = new CsSummarySettingService(new CsSummarySettingRepository());
				this.SummarySettingModel = summarySettingService.GetWithItems(this.LoginOperatorDeptId, this.SummarySettingNo);
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		lSummarySettingNo.Text = WebSanitizer.HtmlEncode(this.SummarySettingModel.SummarySettingNo);
		lSummarySettingTitle.Text = WebSanitizer.HtmlEncode(this.SummarySettingModel.SummarySettingTitle);
		lSummarySettingValidFlg.Text = WebSanitizer.HtmlEncode(this.SummarySettingModel.EX_ValidFlgText);
		lSummarySettingDisplayOrder.Text = WebSanitizer.HtmlEncode(this.SummarySettingModel.DisplayOrder);
		lSummarySettingSummarySettingType.Text = WebSanitizer.HtmlEncode(this.SummarySettingModel.EX_SummarySettingTypeText);
		lDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				this.SummarySettingModel.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lDateChanged.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				this.SummarySettingModel.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lLastChanged.Text = WebSanitizer.HtmlEncode(this.SummarySettingModel.LastChanged);

		divSummarySettingItems.Visible = (this.SummarySettingModel.SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO)
			|| (this.SummarySettingModel.SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN);
		rSummarySettingItems.DataSource = this.SummarySettingModel.EX_Items;
		rSummarySettingItems.DataBind();
	}
	#endregion

	#region #btnEdit_Click 編集ボタンクリック
	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		Session[Constants.SESSION_KEY_SUMMARYSETTING_INFO] = this.SummarySettingModel;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SUMMARYSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);		
	}
	#endregion

	#region #btnInsert_Click 登録ボタンクリック
	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		CsSummarySettingService service = new CsSummarySettingService(new CsSummarySettingRepository());
		service.RegisterWithItems(this.SummarySettingModel);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SUMMARYSETTING_LIST);
	}
	#endregion

	#region #btnUpdate_Click 更新ボタンクリック
	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		CsSummarySettingService service = new CsSummarySettingService(new CsSummarySettingRepository());
		service.UpdateWithItems(this.SummarySettingModel);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SUMMARYSETTING_LIST);
	}
	#endregion

	#region プロパティ
	/// <summary>集計区分NO</summary>
	protected int SummarySettingNo
	{
		get { return int.Parse(Request[Constants.REQUEST_KEY_SUMMARY_SETTING_NO]); }
	}
	/// <summary>集計区分情報</summary>
	protected CsSummarySettingModel SummarySettingModel
	{
		get { return (CsSummarySettingModel)ViewState[Constants.SESSION_KEY_SUMMARYSETTING_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_SUMMARYSETTING_INFO] = value; }
	}
	#endregion
}
