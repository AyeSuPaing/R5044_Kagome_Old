/*
=========================================================================================================
  Module      : インシデント基底ユーザーコントロール(IncidentUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.Cs.IncidentVoc;
using w2.App.Common.Cs.SummarySetting;
using w2.Domain.User;

/// <summary>
/// IncidentUserControl の概要の説明です
/// </summary>
public class IncidentUserControl : BaseUserControl
{
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
		if (groups.Any(g => g.CsGroupId == ddlCsGroups.SelectedValue) == false)
		{
			var find = groups.FirstOrDefault(g => ddlCsGroups.Items.Cast<ListItem>().Select(li => li.Value).Contains(g.CsGroupId));

			ddlCsGroups.SelectedValue = (find != null) ? find.CsGroupId : "";
			RecreateCsOperatorDropDown(ddlCsOperators, ddlCsGroups.SelectedValue);
		}
		foreach (ListItem li in ddlCsOperators.Items) li.Selected = (li.Value == this.LoginOperatorId);
	}
	#endregion

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
		ddlOperators.Items.Add("");
		ddlOperators.Items.AddRange(CreateCsOperatorItems(groupId));

		if (ddlOperators.Items.FindByValue(before) != null) ddlOperators.SelectedValue = before;
	}
	#endregion

	#region #CreateIncidentCategoryItems インシデントカテゴリ ドロップダウンリストアイテム作成
	/// <summary>
	/// インシデントカテゴリ ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateIncidentCategoryItems()
	{
		var service = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		var list = service.GetValidAll(this.LoginOperatorDeptId);

		var items = (from c in list select new ListItem(c.EX_CategoryNameForDropdown, c.CategoryId)).ToArray();
		return items;
	}
	#endregion

	#region #CreateIncidentStatusItems インシデントステータス ドロップダウンリストアイテム作成
	/// <summary>
	/// インシデントステータス ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateIncidentStatusItems()
	{
		var items = ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS);
		return items;
	}
	#endregion

	#region #CreateIncidentImportanceItems インシデント重要度 ドロップダウンリストアイテム作成
	/// <summary>
	/// インシデント重要度 ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateIncidentImportanceItems()
	{
		var items = ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_IMPORTANCE);
		return items;
	}
	#endregion

	#region #CreateIncidentVocItems インシデントVOC ドロップダウンリストアイテム作成
	/// <summary>
	/// インシデントVOC ドロップダウンリストアイテム作成
	/// </summary>
	/// <returns>リストアイテム</returns>
	protected ListItem[] CreateIncidentVocItems()
	{
		var service = new CsIncidentVocService(new CsIncidentVocRepository());
		var list = service.GetValidAll(this.LoginOperatorDeptId);

		var items = (from v in list select new ListItem(v.VocText, v.VocId)).ToArray();
		return items;
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

		var items = (from g in list select new ListItem(g.CsGroupName, g.CsGroupId)).ToArray();
		return items;
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
		var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
		var list = service.GetValidOperators(this.LoginOperatorDeptId, groupId);

		var items = (from o in list select new ListItem(o.EX_ShopOperatorName, o.OperatorId)).ToArray();
		return items;
	}
	#endregion

	#region #CsSummarySettingArray 集計区分 リピータ用データソース作成
	/// <summary>
	/// 集計区分 リピータ用データソース作成
	/// </summary>
	/// <returns>配列</returns>
	protected CsSummarySettingModel[] CsSummarySettingArray()
	{
		var service = new CsSummarySettingService(new CsSummarySettingRepository());
		var list = service.GetValidAllWithValidItems(this.LoginOperatorDeptId);

		return list;
	}
	#endregion

	#region #CheckAndCreateIncident インシデントチェック＆作成
	/// <summary>
	/// インシデントチェック＆作成
	/// </summary>
	/// <param name="input">入力情報</param>
	/// <param name="rIncidentSummary">インシデント集計区分リピータ</param>
	/// <param name="isDraft">下書き保存用か（であれば必須チェックしない）</param>
	/// <param name="errors">エラーメッセージ</param>
	/// <returns>エラーメッセージ(HTMLエンコード済み)</returns>
	protected CsIncidentModel CheckAndCreateIncident(Hashtable input, Repeater rIncidentSummary, bool isDraft, out string errors)
	{
		// チェック用Hashatable作成
		var inputForCheck = (Hashtable)input.Clone();
		if (isDraft) inputForCheck.Keys.Cast<string>().Where(key => StringUtility.ToEmpty(inputForCheck[key]) == "").ToList().ForEach(key => inputForCheck[key] = null);

		// 入力チェック
		errors = Validator.Validate("CsIncident", inputForCheck);

		// ユーザー存在チェック
		string userId = (string)input[Constants.FIELD_CSINCIDENT_USER_ID];
		if (string.IsNullOrEmpty(userId) == false)
		{
			var user = new UserService().Get(userId);
			if (user == null) errors += WebSanitizer.HtmlEncode("ユーザーIDで指定したユーザーが見つかりません。") + "<br />";
		}

		// 集計区分チェック＆作成
		string tmpError;
		var list = CheckAndCreateSummaryValues((string)input[Constants.FIELD_CSINCIDENT_INCIDENT_ID], rIncidentSummary, out tmpError);
		errors += tmpError;

		// エラーがあればreturn
		if (errors != "") return null;

		// インシデント作成
		var incident = new CsIncidentModel(input);
		incident.EX_SummaryValues = list.ToArray();

		return incident;
	}
	#endregion

	#region -CheckAndCreateSummaryValues 集計区分チェック＆作成
	/// <summary>
	/// 集計区分チェック＆作成
	/// </summary>
	/// <param name="incidentId">インシデントID（新規の場合は空でOK）</param>
	/// <param name="rIncidentSummary">インシデント集計区分リピータ</param>
	/// <param name="errors">エラーメッセージ</param>
	/// <returns>集計区分値リスト</returns>
	private List<CsIncidentSummaryValueModel> CheckAndCreateSummaryValues(string incidentId, Repeater rIncidentSummary, out string errors)
	{
		errors = "";

		var list = new List<CsIncidentSummaryValueModel>();
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			string value = null, text = null;

			var rbl = ((RadioButtonList)ri.FindControl("rblSummaryValue"));
			var ddl = ((DropDownList)ri.FindControl("ddlSummaryValue"));
			var tb = ((TextBox)ri.FindControl("tbSummaryValue"));
	
			if (rbl.Visible)
			{
				value = rbl.SelectedValue;
				text = (rbl.SelectedIndex >= 0) ? rbl.SelectedItem.Text : "";
			}
			else if (ddl.Visible)
			{
				value = ddl.SelectedValue;
				text = (ddl.SelectedIndex >= 0) ? ddl.SelectedItem.Text : "";
			}
			else if (tb.Visible)
			{
				value = text = ((TextBox)ri.FindControl("tbSummaryValue")).Text;
			}
			if (value == null) continue;

			var model = new CsIncidentSummaryValueModel();
			model.DeptId = this.LoginOperatorDeptId;
			model.IncidentId = incidentId;
			model.SummaryNo = int.Parse(((HiddenField)ri.FindControl("hfSummaryNo")).Value);
			model.Value = value;
			model.EX_Text = text;
			model.LastChanged = this.LoginOperatorName;

			errors += Validator.Validate("CsIncidentSummaryValue", model.DataSource).Replace("@@ name @@", WebSanitizer.HtmlEncode(((HiddenField)ri.FindControl("hfSummarySettingTitle")).Value));

			list.Add(model);
		}
		return list;
	}
	#endregion
}