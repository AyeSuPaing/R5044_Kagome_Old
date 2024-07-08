/*
=========================================================================================================
  Module      :受信時振分けルール設定一覧ページ処理(MailAssignSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.Cs.Message;
using w2.App.Common.MailAssignSetting;
using w2.Common.Util;

public partial class Form_MailAssignSetting_MailAssignSettingList : MailAssignSettingPage
{
	#region Event
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 表示用モデル取得
			var service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
			var models = service.GetAllWithItems(this.LoginOperatorDeptId);

			// 一覧表示
			if (models.Length == 0)
			{
				// 存在なしエラー
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
			else
			{
				// 表示文言の組み立て：カテゴリ欄
				CsIncidentCategoryService catService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
				CsIncidentCategoryModel[] categories = catService.GetAll(this.LoginOperatorDeptId);
				foreach (CsMailAssignSettingModel setting in models)
				{
					// カテゴリ振分けなし
					if (setting.AssignIncidentCategoryId == "") continue;

					// カテゴリ振分けあり
					CsIncidentCategoryModel match = categories.FirstOrDefault(p => setting.AssignIncidentCategoryId == p.CategoryId);
					if (match != null)
					{
						// 通常カテゴリ（有効or無効）
						setting.EX_AssignIncidentCategoryName = match.CategoryName;
						setting.EX_AssignIncidentCategoryName
							+= GetDispTextInvalid(match.EX_RankedValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID);
					}
					else
					{
						// 削除済みカテゴリ
						setting.EX_AssignIncidentCategoryName = GetDispTextDelete(setting.AssignIncidentCategoryId);
					}
				}

				// 表示文言の組み立て：担当オペレータ欄
				foreach (CsMailAssignSettingModel setting in models)
				{
					if (setting.AssignOperatorId == "") continue;

					if (setting.EX_AssignOperatorExists == false)
					{
						setting.EX_AssignOperatorName = GetDispTextDelete(setting.AssignOperatorId);
					}
					else
					{
						setting.EX_AssignOperatorName
							+= GetDispTextInvalid(setting.EX_AssignOperatorValid == false);
					}
				}

				// 表示文言の組み立て：担当グループ欄
				foreach (CsMailAssignSettingModel setting in models)
				{
					if (setting.AssignCsGroupId == "") continue;

					if (setting.EX_AssignCsGroupExists == false)
					{
						setting.EX_AssignCsGroupName = GetDispTextDelete(setting.AssignCsGroupId);
					}
					else
					{
						setting.EX_AssignCsGroupName
							+= GetDispTextInvalid(setting.EX_AssignCsGroupValid == false);
					}
				}

				// 表示用モデル一覧をデータバインド
				rList.DataSource = models;
				rList.DataBind();
			}
		}
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		this.MailAssignSettingModel = null;	// 入力途中のセッション情報クリア
		Response.Redirect(CreateRegisterInsertUrl());
	}

	/// <summary>
	/// Event click btnRunMailAssign
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRunMailAssign_Click(object sender, EventArgs e)
	{
		// Get mail assign setting is checked
		var mailAssignIds = GetCheckedMailAssignSetting();

		// Get list mail receive
		var listMessages = new CsMessageService(new CsMessageRepository()).GetAllReceiveMail(this.LoginOperatorDeptId);
		if (listMessages.Count() == 0) RedirectPageError();

		var mailAssignSettingModel = new CsMailAssignSettingService(new CsMailAssignSettingRepository()).GetAllWithItems(this.LoginOperatorDeptId, mailAssignIds);
		foreach (CsMessageModel messageModel in listMessages)
		{
			var messageMailModel = new CsMessageMailService(new CsMessageMailRepository()).Get(this.LoginOperatorDeptId, messageModel.ReceiveMailId);

			CsIncidentModel boundIncident = null;
			if (string.IsNullOrEmpty(messageMailModel.InReplyTo) == false)
			{
				var incidents = new CsIncidentService(new CsIncidentRepository()).GetByMessageId(Constants.CONST_DEFAULT_DEPT_ID, messageMailModel.InReplyTo);
				if (incidents.Length > 0) boundIncident = incidents[0];
			}

			// Get mail assign setting to apply
			var applySetting = CreateMailAssignSetting(messageMailModel, boundIncident, mailAssignSettingModel);

			// Update incident
			if (applySetting != null)
			{
				UpdateIncidentByMailAssignSetting(applySetting, messageModel);
			}
		}

		// Execute sussess show message
		dvMessageExecSuccess.Visible = true;
	}
	#endregion

	#region Function Private
	/// <summary>
	/// Get mail assign id is checked
	/// </summary>
	/// <returns>Mail assign ids</returns>
	private string[] GetCheckedMailAssignSetting()
	{
		var targets = (from RepeaterItem item in rList.Items
			where ((CheckBox)item.FindControl("cbCheck")).Checked
			select ((HiddenField)item.FindControl("hfMailAssignId")).Value);
		return targets.ToArray();
	}

	/// <summary>
	/// Create mail assign setting
	/// </summary>
	/// <param name="messageMailModel">Message mail model</param>
	/// <param name="boundIncident">Bound incident</param>
	/// <param name="assignSettings">Assign settings</param>
	/// <returns>Model mail assign setting</returns>
	private CsMailAssignSettingModel CreateMailAssignSetting(CsMessageMailModel messageMailModel, CsIncidentModel boundIncident, CsMailAssignSettingModel[] assignSettings)
	{
		CsMailAssignSettingModel applySetting = null;
		foreach (CsMailAssignSettingModel setting in assignSettings)
		{
			bool isMatch = false;

			// 振分け条件の判定
			if (setting.EX_IsMatchOnBind)
			{
				if (boundIncident != null) isMatch = true;
			}
			else if (IsMatchAssignSetting(setting, messageMailModel) || setting.EX_IsMatchAnything)
			{
				isMatch = true;
			}

			// 振分けアクション作成
			if (isMatch)
			{
				if (applySetting == null)
				{
					applySetting = (CsMailAssignSettingModel)setting.Clone();
				}
				else
				{
					applySetting.Merge(setting);
				}
			}

			// 振分け停止、オートレスポンスの適用
			if (isMatch)
			{
				// 振分け停止
				if (setting.StopFiltering == Constants.FLG_CSMAILASSIGNSETTING_STOP_FILTERING_VALID) break;
			}
		}
		return applySetting;
	}

	/// <summary>
	/// Is match assign setting
	/// </summary>
	/// <param name="setting">Setting</param>
	/// <param name="messageMailModel">Message mail model</param>
	/// <returns>
	/// True : Is match
	/// False : Is not match
	/// </returns>
	private static bool IsMatchAssignSetting(CsMailAssignSettingModel setting, CsMessageMailModel messageMailModel)
	{
		List<bool> isMatchList = new List<bool>();

		// 個々の振り分け条件を判定
		foreach (var settingDetail in setting.Items)
		{
			bool result = IsMatchAssignSettingItem(settingDetail, messageMailModel);
			isMatchList.Add(result);
		}

		// この振り分け条件を適用するかどうか確定
		bool isAnd = (setting.LogicalOperator == Constants.FLG_CSMAILASSIGNSETTING_LOGICAL_OPERATOR_AND);	// かつ
		bool isOr = (setting.LogicalOperator == Constants.FLG_CSMAILASSIGNSETTING_LOGICAL_OPERATOR_OR);		// または

		if (isAnd) return (isMatchList.Contains(false) == false);
		if (isOr) return isMatchList.Contains(true);

		throw new Exception();
	}

	/// <summary>
	/// Is match assign setting item
	/// </summary>
	/// <param name="detail">Detail</param>
	/// <param name="messageMailModel">Message mail model</param>
	/// <returns>
	/// True : Is match
	/// False : Is not match
	/// </returns>
	private static bool IsMatchAssignSettingItem(CsMailAssignSettingItemModel detail, CsMessageMailModel messageMailModel)
	{
		string value = detail.MatchingValue;
		bool ignoreCase = (detail.IgnoreCase == Constants.FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_VALID);

		bool isContains;
		switch (detail.MatchingTarget)
		{
			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_SUBJECT:
				isContains = StringExtention.Contains(messageMailModel.MailSubject, value, ignoreCase);
				break;

			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_BODY:
				isContains = StringExtention.Contains(messageMailModel.MailBody, value, ignoreCase);
				break;

			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_HEADER:
				isContains = false;
				// Mail from
				isContains |= StringExtention.Contains(messageMailModel.MailFrom, value, ignoreCase);
				// Mail To
				isContains |= StringExtention.Contains(messageMailModel.MailTo, value, ignoreCase);
				// Mail CC
				isContains |= StringExtention.Contains(messageMailModel.MailCc, value, ignoreCase);
				// In reply to
				isContains |= StringExtention.Contains(messageMailModel.InReplyTo, value, ignoreCase);
				// Mail subject
				isContains |= StringExtention.Contains(messageMailModel.MailSubject, value, ignoreCase);
				// Time receive
				isContains |= StringExtention.Contains(messageMailModel.ReceiveDatetime.Value.GetDateTimeFormats()[112], value, ignoreCase);
				// Message id of mail
				isContains |= StringExtention.Contains(messageMailModel.MessageId, value, ignoreCase);
				break;

			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_FROM:
				isContains = StringExtention.Contains(messageMailModel.MailFrom, value, ignoreCase);
				break;

			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_TOCC:
				isContains = false;
				// Mail To
				isContains |= StringExtention.Contains(messageMailModel.MailTo, value, ignoreCase);
				// Mail CC
				isContains |= StringExtention.Contains(messageMailModel.MailCc, value, ignoreCase);
				break;

			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_TO:
				isContains = false;
				// Mail To
				isContains |= StringExtention.Contains(messageMailModel.MailTo, value, ignoreCase);
				break;

			case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_CC:
				isContains = false;
				// Mail CC
				isContains |= StringExtention.Contains(messageMailModel.MailCc, value, ignoreCase);
				break;

			default:
				throw new Exception("不正な振り分け項目です。");
		}

		return isContains ^ (detail.MatchingType == Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE_NO_INCLUDE);
	}

	/// <summary>
	/// Update incident by mail assign setting
	/// </summary>
	/// <param name="applySetting">Setting apply</param>
	/// <param name="messageModel">Message model</param>
	private void UpdateIncidentByMailAssignSetting(CsMailAssignSettingModel applySetting, CsMessageModel messageModel)
	{
		var incidentModel = new CsIncidentModel()
		{
			IncidentId = messageModel.IncidentId,
			DeptId = this.LoginOperatorDeptId,
			Status = applySetting.AssignStatus,
			IncidentCategoryId = applySetting.AssignIncidentCategoryId,
			Importance = applySetting.AssignImportance,
			CsGroupId = applySetting.AssignCsGroupId,
			OperatorId = applySetting.AssignOperatorId,
			LastChanged = this.LoginOperatorName
		};

		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateIncidentByMailAssignSetting(incidentModel);
	}

	/// <summary>
	/// Redirect page error
	/// </summary>
	private void RedirectPageError()
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR + "?"
			+ Constants.REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN + "=" + HttpUtility.UrlEncode(WebMessages.MSG_MANAGER_MAIL_ASSIGN_NO_MAIL));
	}
	#endregion
}