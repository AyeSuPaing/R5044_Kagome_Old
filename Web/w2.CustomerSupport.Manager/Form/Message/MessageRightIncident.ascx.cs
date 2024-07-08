/*
=========================================================================================================
  Module      : メッセージページインシデントフォーム出力コントローラ処理(MessageRightIncident.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;

public partial class Form_Message_MessageRightIncident : IncidentUserControl
{
	#region #Page_Init ページ初期化
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			Initialize();
		}
	}
	#endregion

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// インシデントカテゴリ作成
		ddlIncidentCategory.Items.Add("");
		ddlIncidentCategory.Items.AddRange(CreateIncidentCategoryItems());
		// ステータスセット
		ddlIncidentStatus.Items.AddRange(CreateIncidentStatusItems());
		// 重要度セット
		ddlImportance.Items.AddRange(CreateIncidentImportanceItems());
		ddlImportance.SelectedValue = Constants.FLG_CSINCIDENT_IMPORTANCE_MIDDLE;
		// VOCセット
		ddlVoc.Items.Add("");
		ddlVoc.Items.AddRange(CreateIncidentVocItems());
		// グループ
		ddlCsGroups.Items.Add("");
		ddlCsGroups.Items.AddRange(CreateCsGroupItems());
		// オペレータ
		ddlCsOperators.Items.Add("");
		ddlCsOperators.Items.AddRange(CreateCsOperatorItems(ddlCsGroups.SelectedValue));
		// 集計区分セット
		rIncidentSummary.DataSource = CsSummarySettingArray();
		rIncidentSummary.DataBind();

		// インシデントクリアボタンの制御
		btnClearIncident.Enabled = (Request[Constants.REQUEST_KEY_MESSAGE_EDIT_MODE] == Constants.KBN_MESSAGE_EDIT_MODE_NEW);
	}
	#endregion

	#region #btnClearIncident_Click クリアボタンクリック
	/// <summary>
	/// クリアボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClearIncident_Click(object sender, EventArgs e)
	{
		var incident = new CsIncidentModel();
		incident.EX_SummaryValues = new CsIncidentSummaryValueModel[0];
		incident.Importance = Constants.FLG_CSINCIDENT_IMPORTANCE_MIDDLE;

		SetIncident(incident);

		ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh_action_button", "refresh_action_button();", true);
	}
	#endregion

	#region #ddlCsGroup_SelectedIndexChanged 担当グループ変更
	/// <summary>
	/// 担当グループ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCsGroup_SelectedIndexChanged(object sender, EventArgs e)
	{
		RecreateCsOperatorDropDown(ddlCsOperators, ddlCsGroups.SelectedValue);
	}
	#endregion

	#region #btnSetOperatorAndGroup_Click CSオペレータ・グループセット
	/// <summary>
	/// CSオペレータ・グループセット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetOperatorAndGroup_Click(object sender, EventArgs e)
	{
		SetOperatorAndGroup(this.LoginOperatorId, ddlCsGroups, ddlCsOperators);
	}
	#endregion

	#region +SetIncident インシデント情報差し込み
	/// <summary>
	/// インシデント情報差し込み
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	public void SetIncident(string incidentId)
	{
		var service = new CsIncidentService(new CsIncidentRepository());
		var incident = service.GetWithSummaryValues(this.LoginOperatorDeptId, incidentId);

		if (incident == null) return;

		SetIncident(incident);
	}
	#endregion
	#region +SetIncident インシデント情報差し込み
	/// <summary>
	/// インシデント情報差し込み
	/// </summary>
	/// <param name="incident">インシデント</param>
	public void SetIncident(CsIncidentModel incident)
	{
		imgLock.Visible = (incident.LockStatus != "");
		lIncidentId.Text = WebSanitizer.HtmlEncode(incident.IncidentId);
		this.IncidentId = incident.IncidentId;
		tbIncidentUserId.Text = incident.UserId;
		tbIncidentTitle.Text = incident.IncidentTitle;
		foreach (ListItem li in ddlIncidentCategory.Items) li.Selected = (li.Value == incident.IncidentCategoryId);
		foreach (ListItem li in ddlIncidentStatus.Items) li.Selected = (li.Value == incident.Status);
		foreach (ListItem li in ddlImportance.Items) li.Selected = (li.Value == incident.Importance);
		foreach (ListItem li in ddlVoc.Items) li.Selected = (li.Value == incident.VocId);
		tbVocMemo.Text = incident.VocMemo;
		foreach (ListItem li in ddlCsGroups.Items) li.Selected = (li.Value == incident.CsGroupId);
		foreach (ListItem li in ddlCsOperators.Items) li.Selected = (li.Value == incident.OperatorId);
		hfCsOperatorBefore.Value = ddlCsOperators.SelectedValue;
		tbIncidentComment.Text = incident.Comment;
		// 集計区分値
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			var hfSummaryNo = (HiddenField)ri.FindControl("hfSummaryNo");
			var rblSummaryValue = (RadioButtonList)ri.FindControl("rblSummaryValue");
			var ddlSummaryValue = (DropDownList)ri.FindControl("ddlSummaryValue");
			var tbSummaryValue = (TextBox)ri.FindControl("tbSummaryValue");

			var values = incident.EX_SummaryValues.Where(v => v.SummaryNo == int.Parse(hfSummaryNo.Value)).ToArray();
			var value = (values.Length != 0) ? values[0].Value : null;

			if (rblSummaryValue.Visible)
			{
				rblSummaryValue.Items[0].Selected = true;	// デフォルト選択
				foreach (ListItem li in rblSummaryValue.Items) li.Selected = (li.Value == value);
			}
			else if (ddlSummaryValue.Visible)
			{
				foreach (ListItem li in ddlSummaryValue.Items) li.Selected = (li.Value == value);
			}
			else if (tbSummaryValue.Visible) tbSummaryValue.Text = value;

		}
		lIncidentLastChanged.Text = WebSanitizer.HtmlEncode(incident.LastChanged);

		lIncidentIdCreateMessage.Visible = (this.IncidentId == "");
		lIncidentId.Visible = (this.IncidentId != "");
	}
	#endregion

	#region +SetIncidentStatus インシデントステータスセット
	/// <summary>
	/// インシデントステータスセット
	/// </summary>
	/// <param name="incidentStatus">インシデントステータス</param>
	public void SetIncidentStatus(string incidentStatus)
	{
		foreach (ListItem li in ddlIncidentStatus.Items)
		{
			li.Selected = (li.Value == incidentStatus);
		}
	}
	#endregion

	#region +SetUser ユーザー情報差込
	/// <summary>
	/// ユーザー情報差込（ユーザーより）
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	public void SetUser(string userId)
	{
		tbIncidentUserId.Text = userId;
	}
	#endregion

	#region +CheckInputインシデントの入力チェック＆インシデント取得
	/// <summary>
	/// インシデントの入力チェック
	/// </summary>
	/// <param name="saveAsDraft">下書きか（下書きの場合は必須チェックを実施しない）</param>
	/// <returns>インシデントモデル</returns>
	/// <remarks>IncidentAndMessageParts.ascxに同様のメソッドあり</remarks>
	public CsIncidentModel CheckInput(bool saveAsDraft)
	{
		// 入力情報取得
		var input = GetInput();

		// インシデントチェック
		string errorMessage = "";
		var incident = CheckAndCreateIncident(input, rIncidentSummary, saveAsDraft, out errorMessage);
		lErrorMessages.Text = errorMessage;
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			aIncidentTitle.Focus();
			return null;
		}

		// その他インシデント情報セット
		incident.EX_IncidentCategoryName = ddlIncidentCategory.SelectedItem.Text.Trim();
		incident.EX_VocText = ddlVoc.SelectedItem.Text;
		incident.EX_CsGroupName = ddlCsGroups.SelectedItem.Text;
		incident.EX_CsOperatorName = ddlCsOperators.SelectedItem.Text;

		return incident;
	}
	#endregion

	#region -GetInput インシデント入力情報取得（入力チェック用）
	/// <summary>
	/// インシデント入力情報取得（入力チェック用）
	/// </summary>
	/// <returns>インシデント入力情報</returns>
	/// <remarks>IncidentAndMessageParts.ascxに同様のメソッドあり</remarks>
	private Hashtable GetInput()
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, this.LoginOperatorDeptId);
		input.Add(Constants.FIELD_CSINCIDENT_INCIDENT_ID, this.IncidentId);
		input.Add(Constants.FIELD_CSINCIDENT_USER_ID, tbIncidentUserId.Text);
		input.Add(Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID, ddlIncidentCategory.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_INCIDENT_TITLE, tbIncidentTitle.Text);
		input.Add(Constants.FIELD_CSINCIDENT_STATUS, ddlIncidentStatus.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_VOC_ID, ddlVoc.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_VOC_MEMO, tbVocMemo.Text);
		input.Add(Constants.FIELD_CSINCIDENT_COMMENT, tbIncidentComment.Text);
		input.Add(Constants.FIELD_CSINCIDENT_IMPORTANCE, ddlImportance.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_USER_NAME, null);		// あとでセット
		input.Add(Constants.FIELD_CSINCIDENT_USER_CONTACT, null);	// あとでセット
		input.Add(Constants.FIELD_CSINCIDENT_CS_GROUP_ID, ddlCsGroups.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_OPERATOR_ID, ddlCsOperators.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_OPERATOR_ID + "_before", hfCsOperatorBefore.Value);
		input.Add(Constants.FIELD_CSINCIDENT_VALID_FLG, Constants.FLG_CSINCIDENT_VALID_FLG_VALID);
		input.Add(Constants.FIELD_CSINCIDENT_LAST_CHANGED, this.LoginOperatorName);

		return input;
	}
	#endregion

	#region プロパティ
	/// <summary>インシデントID</summary>
	public string IncidentId
	{
		get { return (string)ViewState["IncidentId"]; }
		set { ViewState["IncidentId"] = value; }
	}
	#endregion
}