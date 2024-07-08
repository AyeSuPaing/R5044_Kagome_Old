/*
=========================================================================================================
  Module      : 受信時振分けルール設定確認ページ処理(MailAssignSettingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.MailAssignSetting;
using w2.Common.Util;
using w2.App.Common.Cs.IncidentCategory;

public partial class Form_MailAssignSetting_MailAssignSettingConfirm : MailAssignSettingPage
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
			// 初期化処理
			InitializeComponent();
		}
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	private void InitializeComponent()
	{
		switch (this.ActionStatus)
		{
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				DispControlInsert();
				trConfirm.Visible = true;
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				DispControlUpdate();
				trConfirm.Visible = true;
				break;

			// 詳細
			case Constants.ACTION_STATUS_DETAIL:
				DispControlDetail();
				trDetail.Visible = true;
				break;

			// 不正パラメタエラー
			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 新規登録時表示制御
	/// </summary>
	private void DispControlInsert()
	{
		this.AssignSetting = this.MailAssignSettingModel;

		btnInsertTop.Visible = true;
		btnInsertBottom.Visible = true;
	}

	/// <summary>
	/// 更新時表示制御
	/// </summary>
	private void DispControlUpdate()
	{
		this.AssignSetting = this.MailAssignSettingModel;

		btnUpdateTop.Visible = true;
		btnUpdateBottom.Visible = true;
	}

	/// <summary>
	/// 詳細時表示制御
	/// </summary>
	private void DispControlDetail()
	{
		CsMailAssignSettingService service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
		this.AssignSetting = service.GetWithItems(this.LoginOperatorDeptId, this.MailAssignId);

		// 表示文言の組み立て：カテゴリ欄
		if (this.AssignSetting.AssignIncidentCategoryId != "")
		{
			CsIncidentCategoryService catService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
			CsIncidentCategoryModel[] categories = catService.GetAll(this.LoginOperatorDeptId);

			// カテゴリ振分けあり
			CsIncidentCategoryModel match = categories.FirstOrDefault(p => this.AssignSetting.AssignIncidentCategoryId == p.CategoryId);
			if (match != null)
			{
				// 通常カテゴリ（有効or無効）
				this.AssignSetting.EX_AssignIncidentCategoryName = match.CategoryName;
				this.AssignSetting.EX_AssignIncidentCategoryName
					+= GetDispTextInvalid(match.EX_RankedValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID);
			}
			else
			{
				// 削除済みカテゴリ
				this.AssignSetting.EX_AssignIncidentCategoryName = GetDispTextDelete(this.AssignSetting.AssignIncidentCategoryId);
			}
		}

		// 表示文言の組み立て：担当オペレータ欄
		if (this.AssignSetting.AssignOperatorId != "")
		{
			if (this.AssignSetting.EX_AssignOperatorExists == false)
			{
				this.AssignSetting.EX_AssignOperatorName = GetDispTextDelete(this.AssignSetting.AssignOperatorId);
			}
			else
			{
				this.AssignSetting.EX_AssignOperatorName
					+= GetDispTextInvalid(this.AssignSetting.EX_AssignOperatorValid == false);
			}
		}

		// 表示文言の組み立て：担当グループ欄
		if (this.AssignSetting.AssignCsGroupId != "")
		{
			if (this.AssignSetting.EX_AssignCsGroupExists == false)
			{
				this.AssignSetting.EX_AssignCsGroupName = GetDispTextDelete(this.AssignSetting.AssignCsGroupId);
			}
			else
			{
				this.AssignSetting.EX_AssignCsGroupName
					+= GetDispTextInvalid(this.AssignSetting.EX_AssignCsGroupValid == false);
			}
		}

		btnEditTop.Visible = true;
		btnEditBottom.Visible = true;
		if ((this.MailAssignId != Constants.CONST_MAIL_ASSIGN_ID_MATCH_ON_BIND) && (this.MailAssignId != Constants.CONST_MAIL_ASSIGN_ID_MATCH_ANYTHING))
		{
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
		}
	}

	/// <summary>
	/// 戻るボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		switch (this.ActionStatus)
		{
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				Response.Redirect(CreateRegisterInsertUrl());
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				Response.Redirect(CreateRegisterUpdateUrl(this.MailAssignId));
				break;

			// 詳細
			case Constants.ACTION_STATUS_DETAIL:
				Response.Redirect(CreateListUrl());
				break;
		}
	}

	/// <summary>
	/// 登録ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		CsMailAssignSettingService service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
		service.InsertWithItems(this.MailAssignSettingModel);
		this.MailAssignSettingModel = null;	// Sessioinデータクリア

		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 更新ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		CsMailAssignSettingService service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
		service.UpdateWithItems(this.MailAssignSettingModel);
		this.MailAssignSettingModel = null;	// Sessioinデータクリア

		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 編集ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		this.MailAssignSettingModel = null;	// 入力途中のセッション情報クリア

		Response.Redirect(CreateRegisterUpdateUrl(this.MailAssignId));
	}

	/// <summary>
	/// 削除ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		CsMailAssignSettingService service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
		service.DeleteWithItems(this.LoginOperatorDeptId, this.MailAssignId);

		Response.Redirect(CreateListUrl());
	}

	#region プロパティ
	/// <summary>
	/// 振分け設定情報
	/// </summary>
	protected CsMailAssignSettingModel AssignSetting { get; set; }
	#endregion
}
