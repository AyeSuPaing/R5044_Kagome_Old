/*
=========================================================================================================
  Module      : Incident Modify Input (IncidentModifyInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.IncidentCategory;
using w2.Domain.CsIncident;

public partial class Form_Incident_IncidentModifyInput : BasePage
{
	#region 定数
	// 「ー 更新しない ー」
	private static string NOT_UPDATE_TEXT = ValueText.GetValueText(
		Constants.VALUETEXT_PARAM_INCIDENT,
		Constants.VALUETEXT_PARAM_UPDATE_TEXT,
		Constants.VALUETEXT_PARAM_NOT_UPDATE);
	private static string NOT_UPDATE_VALUE = "notupdate";
	// 「ー 空で更新する ー」
	private static string UPDATE_EMPTY_TEXT = ValueText.GetValueText(
		Constants.VALUETEXT_PARAM_INCIDENT,
		Constants.VALUETEXT_PARAM_UPDATE_TEXT,
		Constants.VALUETEXT_PARAM_UPDATE_EMPTY);
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			((Form_Common_PopupPage)Master).HideCloseButton = true;

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_INCIDENT_ID]))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
			}

			// Incident ids
			lIncidentId.Text = Request[Constants.REQUEST_KEY_INCIDENT_ID].Replace(",", ", ");
			// インシデントカテゴリ作成
			ddlIncidentCategory.Items.Add(new ListItem(NOT_UPDATE_TEXT, NOT_UPDATE_VALUE));
			ddlIncidentCategory.Items.Add(new ListItem(UPDATE_EMPTY_TEXT, string.Empty));
			ddlIncidentCategory.Items.AddRange(CreateIncidentCategoryItems());

			// ステータスセット
			ddlIncidentStatus.Items.Add(new ListItem(NOT_UPDATE_TEXT, NOT_UPDATE_VALUE));
			ddlIncidentStatus.Items.AddRange(CreateIncidentStatusItems());

			// グループ
			ddlCsGroups.Items.Add(new ListItem(NOT_UPDATE_TEXT, NOT_UPDATE_VALUE));
			ddlCsGroups.Items.Add(new ListItem(UPDATE_EMPTY_TEXT, string.Empty));
			ddlCsGroups.Items.AddRange(CreateCsGroupItems());

			// 有効な拠点グループ設定が存在しない場合
			if (ddlCsGroups.Items.Cast<ListItem>().Any(i => ((i.Text != NOT_UPDATE_TEXT) && (i.Text != UPDATE_EMPTY_TEXT))) == false)
			{
				tdCsGroups.Visible = false;
				ddlCsOperators.Enabled = true;

				// 更新対象インシデントに担当グループが設定されている場合はアラートメッセージを表示する
				lbCsGroupsAlertMessage.Visible = CheckDisplayCsGroupAlertMessage();
			}
			else
			{
				ddlCsOperators.Enabled = false;
			}

			// オペレータ
			ddlCsOperators.Items.Add(new ListItem(NOT_UPDATE_TEXT, NOT_UPDATE_VALUE));
			ddlCsOperators.Items.Add(new ListItem(UPDATE_EMPTY_TEXT, string.Empty));
			ddlCsOperators.Items.AddRange(CreateCsOperatorItems(ddlCsGroups.SelectedValue));
		}
	}

	/// <summary>
	/// Update button click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// Get incident info
		var input = GetIncidentInfo();

		// Update incident by incident ids
		new CsIncidentService().UpdateIncidentByIncidentIds(
			lIncidentId.Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
			input);
		divComp.Visible = true;
	}

	/// <summary>
	/// Get incident info
	/// </summary>
	/// <returns>Incident info</returns>
	private Hashtable GetIncidentInfo()
	{
		var csGroupId = tdCsGroups.Visible
			? ddlCsGroups.SelectedValue
			: (ddlCsOperators.SelectedValue == NOT_UPDATE_VALUE)
				? NOT_UPDATE_VALUE
				: string.Empty;

		return new Hashtable()
		{
			{Constants.FIELD_CSINCIDENT_DEPT_ID, this.LoginOperatorDeptId},
			{Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID, ddlIncidentCategory.SelectedValue},
			{Constants.FIELD_CSINCIDENT_STATUS, ddlIncidentStatus.SelectedValue},
			{Constants.FIELD_CSINCIDENT_CS_GROUP_ID, csGroupId},
			{Constants.FIELD_CSINCIDENT_OPERATOR_ID, ddlCsOperators.SelectedValue},
			{Constants.FIELD_CSINCIDENT_LAST_CHANGED, this.LoginOperatorName}
		};
	}

	#region #CreateIncidentCategoryItems インシデントカテゴリ ドロップダウンリストアイテム作成
	/// <summary>
	/// インシデントカテゴリ ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateIncidentCategoryItems()
	{
		var service = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		var list = service.GetValidAll(this.LoginOperatorDeptId);
		var categoryItems = list.Select(category => new ListItem(category.EX_CategoryNameForDropdown, category.CategoryId)).ToArray();

		return categoryItems;
	}
	#endregion

	#region #CreateIncidentStatusItems インシデントステータス ドロップダウンリストアイテム作成
	/// <summary>
	/// インシデントステータス ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateIncidentStatusItems()
	{
		return ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS);
	}
	#endregion

	#region #CreateCsGroupItems CSオペレータグループ ドロップダウンリストアイテム作成
	/// <summary>
	/// CSオペレータグループ ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateCsGroupItems()
	{
		var service = new CsGroupService(new CsGroupRepository());
		var list = service.GetValidAll(this.LoginOperatorDeptId);
		var csGroupItems = list.Select(group => new ListItem(group.CsGroupName, group.CsGroupId)).ToArray();

		return csGroupItems;
	}
	#endregion

	#region #CreateCsOperatorItems CSオペレータ ドロップダウンリストアイテム作成
	/// <summary>
	/// CSオペレータ ドロップダウンリストアイテム作成
	/// </summary>
	/// <param name="groupId">グループID</param>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateCsOperatorItems(string groupId)
	{
		if (groupId == NOT_UPDATE_VALUE) groupId = string.Empty;
		var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
		var list = service.GetValidOperators(this.LoginOperatorDeptId, groupId);
		var csOperatorItems = list.Select(operatorItem => new ListItem(operatorItem.EX_ShopOperatorName, operatorItem.OperatorId)).ToArray();

		return csOperatorItems;
	}
	#endregion

	/// <summary>
	/// Csgroups select
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCsGroups_SelectedIndexChanged(object sender, EventArgs e)
	{
		RecreateCsOperatorDropDown(ddlCsOperators, ddlCsGroups.SelectedValue);
		if (ddlCsGroups.SelectedValue == NOT_UPDATE_VALUE)
		{
			if (ddlCsOperators.Items.FindByText(NOT_UPDATE_TEXT) == null) ddlCsOperators.Items.Add(new ListItem(NOT_UPDATE_TEXT, NOT_UPDATE_VALUE));
			ddlCsOperators.SelectedValue = NOT_UPDATE_VALUE;
			ddlCsOperators.Enabled = false;
		}
		else
		{
			ddlCsOperators.Enabled = true;
		}
	}

	#region #RecreateCsOperatorDropDown オペレータドロップダウン再生成
	/// <summary>
	/// オペレータドロップダウン再生成
	/// </summary>
	/// <param name="ddlOperators">オペレータドロップダウン</param>
	/// <param name="groupId">グループID</param>
	protected void RecreateCsOperatorDropDown(DropDownList ddlOperators, string groupId)
	{
		var before = ddlOperators.SelectedValue;

		ddlOperators.Items.Clear();
		if (tdCsGroups.Visible == false) ddlOperators.Items.Add(new ListItem(NOT_UPDATE_TEXT, NOT_UPDATE_VALUE));
		ddlOperators.Items.Add(new ListItem(UPDATE_EMPTY_TEXT, string.Empty));
		ddlOperators.Items.AddRange(CreateCsOperatorItems(groupId));

		if (ddlOperators.Items.FindByValue(before) != null) ddlOperators.SelectedValue = before;
	}
	#endregion

	/// <summary>
	/// Set operator and group
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetOperatorAndGroup_Click(object sender, EventArgs e)
	{
		SetOperatorAndGroup(this.LoginOperatorId, ddlCsGroups, ddlCsOperators);
		ddlCsOperators.Enabled = true;
	}

	#region #SetOperatorAndGroup CSオペレータ・グループ自動セット
	/// <summary>
	/// CSオペレータ・グループ自動セット
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <param name="ddlCsGroups">CSグループドロップダウンリスト</param>
	/// <param name="ddlCsOperators">CSオペレータドロップダウンリスト</param>
	protected void SetOperatorAndGroup(string operatorId, DropDownList ddlCsGroups, DropDownList ddlCsOperators)
	{
		var operatorGroupService = new CsOperatorGroupService(new CsOperatorGroupRepository());
		var groups = operatorGroupService.GetGroups(this.LoginOperatorDeptId, operatorId);

		// 自分のグループが選択されていない場合は自分のグループをセット
		if (groups.Any(group => group.CsGroupId == ddlCsGroups.SelectedValue) == false)
		{
			var find = groups.FirstOrDefault(group => ddlCsGroups.Items.Cast<ListItem>().Select(item => item.Value).Contains(group.CsGroupId));

			ddlCsGroups.SelectedValue = (find != null) ? find.CsGroupId : string.Empty;
			RecreateCsOperatorDropDown(ddlCsOperators, ddlCsGroups.SelectedValue);
		}

		foreach (ListItem item in ddlCsOperators.Items) item.Selected = (item.Value == this.LoginOperatorId);
	}
	#endregion

	#region -CheckDisplayCsGroupAlertMessage 担当グループアラートメッセージ表示判定
	/// <summary>
	/// 担当グループアラートメッセージ表示判定
	/// </summary>
	/// <returns>アラートメッセージを表示するか</returns>
	private bool CheckDisplayCsGroupAlertMessage()
	{
		var csGroupIdList = new List<string>();
		foreach (var incidentId in Request[Constants.REQUEST_KEY_INCIDENT_ID].Split(','))
		{
			csGroupIdList.Add(new CsIncidentService().GetCsGroupIdByIncidentId(Constants.CONST_DEFAULT_DEPT_ID, incidentId));
		}

		// 有効な拠点グループ設定が存在せず、かつ更新対象インシデントに担当グループが設定されている場合は表示する
		return csGroupIdList.Any(x => (string.IsNullOrEmpty(x) == false));
	}
	#endregion
}