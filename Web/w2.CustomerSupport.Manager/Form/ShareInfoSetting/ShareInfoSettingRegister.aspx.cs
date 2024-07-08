/*
=========================================================================================================
  Module      : 共有情報管理登録ページ処理(ShareInfoSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Extensions;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.ShareInfo;

public partial class Form_ShareInfoSetting_ShareInfoSettingRegister : ShareInfoSettingPage
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
			// 画面初期化
			Initialize();

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// 共有情報管理表示
			Display();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trRegister.Visible = true;
				btnConfirmTop.Visible = btnConfirmBottom.Visible = true;
				break;
	
			case Constants.ACTION_STATUS_UPDATE:
				trEdit.Visible = true;
				trInfoId.Visible = true;
				btnConfirmTop.Visible = btnConfirmBottom.Visible = true;
				trDateCreated.Visible = true;
				break;
		}

		// 共有テキスト区分ドロップダウン作成
		rblInfoTextKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_TEXT_KBN));
		rblInfoTextKbn.Items[0].Selected = true;

		// 区分ドロップダウン作成
		ddlInfoKbn.Items.Add(new ListItem("", ""));
		ddlInfoKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_KBN));

		// 重要度ドロップダウン作成
		ddlImportance.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE));

		// CSグループ＆オペレータセット
		var groups = GetAllGroupsWithOperators();
		rCsGroups.DataSource = groups;
		rCsGroups.DataBind();

		// CSオペレータ一覧取得、ビューステート格納
		var operators = GetAllOperators();
		rCsOperators.DataSource = operators;
		rCsOperators.DataBind();
	}
	#endregion

	#region -Display 画面表示
	/// <summary>
	/// 画面表示
	/// </summary>
	private void Display()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				lSenderName.Text = WebSanitizer.HtmlEncode(this.LoginOperatorName);
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				this.ShareInfo = (CsShareInfoModel)Session[Constants.SESSION_KEY_SHAREINFO_INFO];
				this.ShareInfoReads = (CsShareInfoReadModel[])Session[Constants.SESSION_KEY_SHAREINFOREAD_INFO];
				lSenderName.Text = WebSanitizer.HtmlEncode(this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT ? this.LoginOperatorName : this.ShareInfo.EX_SenderName);
				DisplayShareInfoSetting();
				break;
		}
	}
	#endregion

	#region -DisplayShareInfoSetting 共有情報管理画面表示
	/// <summary>
	/// 共有情報管理画面表示
	/// </summary>
	private void DisplayShareInfoSetting()
	{
		lInfoNo.Text = WebSanitizer.UrlAttrHtmlEncode(this.ShareInfo.InfoNo);
		foreach (ListItem li in ddlInfoKbn.Items) li.Selected = (li.Value == this.ShareInfo.InfoKbn);
		foreach (ListItem li in ddlImportance.Items) li.Selected = (li.Value == this.ShareInfo.InfoImportance.ToString());
		if (ddlImportance.SelectedIndex == -1) ddlImportance.SelectedValue = "3";
		foreach (ListItem li in rblInfoTextKbn.Items) li.Selected = (li.Value == this.ShareInfo.InfoTextKbn);
		tbInfoText.Text = this.ShareInfo.InfoText;
		lDateCreated.Text = DateTimeUtility.ToStringForManager(
			this.ShareInfo.DateCreated,
			DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);

		var operatorRepeaterItems = GetOperatorRepeaterItems();
		foreach (var read in this.ShareInfoReads)
		{
			var targets = (from RepeaterItem ri in operatorRepeaterItems
						  where ((HiddenField)ri.FindControl("hfOperatorId")).Value == read.OperatorId
						  select ((CheckBox)ri.FindControl("cbOperator"))).ToList();
			targets.ForEach(cb => cb.Checked = true);
		}
	}
	#endregion

	#region -GetAllGroupsWithOperators 全てのグループと紐づくオペレータ取得
	/// <summary>
	/// 全てのグループと紐づくオペレータ取得
	/// </summary>
	/// <returns>グループリスト</returns>
	private CsGroupModel[] GetAllGroupsWithOperators()
	{
		var groupService = new CsGroupService(new CsGroupRepository());
		var groups = groupService.GetValidAllWithValidOperators(this.LoginOperatorDeptId);
		return groups;
	}
	#endregion

	#region -GetAllOperators 全てのオペレータ取得
	/// <summary>
	/// 全てのオペレータ取得
	/// </summary>
	/// <returns>オペレータリスト</returns>
	private CsOperatorModel[] GetAllOperators()
	{
		var operatorService = new CsOperatorService(new CsOperatorRepository());
		var operators = operatorService.GetValidAll(this.LoginOperatorDeptId);
		return operators;
	}
	#endregion

	#region -GetInputAndCheck 入力情報取得＆チェック（エラーの場合は画面遷移）
	/// <summary>
	/// 入力情報取得＆チェック（エラーの場合は画面遷移）
	/// </summary>
	/// <returns>入力値</returns>
	private CsShareInfoModel GetInputAndCheck(string validatorName)
	{
		// 入力値取得
		Hashtable ht = GetInput();

		// 入力チェック＆重複チェック
		string errorMessages = Validator.Validate(validatorName, ht);

		// オペレータ選択チェック
		bool selected = GetOperatorSelected();
		if (selected == false) errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHARE_INFO_OPERATOR_NO_SELECTED_ERROR);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		var model = new CsShareInfoModel(ht);
		model.InfoImportance = int.Parse((string)ht[Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE]);
		return model;
	}
	#endregion

	#region -GetInput 入力情報取得
	/// <summary>
	/// 入力情報取得
	/// </summary>
	/// <returns>入力値</returns>
	private Hashtable GetInput()
	{
		Hashtable ht = new Hashtable();
		ht.Add(Constants.FIELD_CSSHAREINFO_DEPT_ID, this.LoginOperatorDeptId);
		ht.Add(Constants.FIELD_CSSHAREINFO_SENDER, this.LoginOperatorId);
		ht.Add(Constants.FIELD_CSSHAREINFO_INFO_TEXT_KBN, rblInfoTextKbn.SelectedValue);
		ht.Add(Constants.FIELD_CSSHAREINFO_INFO_TEXT, tbInfoText.Text);
		ht.Add(Constants.FIELD_CSSHAREINFO_INFO_KBN, ddlInfoKbn.SelectedValue);
		ht.Add(Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE, ddlImportance.SelectedValue);
		ht.Add(Constants.FIELD_CSSHAREINFO_LAST_CHANGED, this.LoginOperatorName);
		return ht;
	}
	#endregion

	#region -GetOperatorSelected オペレータが１つでも選択されているかチェック
	/// <summary>
	/// オペレータが１つでも選択されているかチェック
	/// </summary>
	/// <returns>どれか１つでも選択されているか</returns>
	private bool GetOperatorSelected()
	{
		var items = GetOperatorRepeaterItems().ToArray();
		var checkedItems = (from ri in items where ((CheckBox)(ri.FindControl("cbOperator"))).Checked select ri).ToArray();
		return (checkedItems.Length > 0);
	}
	#endregion

	#region -GetOperatorRepeaterItems オペレータリピータアイテム取得
	/// <summary>
	/// オペレータリピータアイテム取得
	/// </summary>
	/// <returns>オペレータリピータアイテム</returns>
	private IEnumerable<RepeaterItem> GetOperatorRepeaterItems()
	{
		foreach (RepeaterItem riGrp in rCsGroups.Items)
		{
			foreach (RepeaterItem riOpe in ((Repeater)riGrp.FindControl("rCsOperators")).Items) yield return riOpe;
		}
		foreach (RepeaterItem riOpe in rCsOperators.Items) yield return riOpe;
	}
	#endregion

	#region #btnConfirm_Click 確認ボタンクリック
	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// 入力情報チェック＆取得
		CsShareInfoModel shareInfoTemp = null;
		CsShareInfoReadModel[] shareInfoReadsTemp = null;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				shareInfoTemp = GetInputAndCheck("CsShareInfoRegister");
				shareInfoTemp.InfoNo = 0;
				shareInfoTemp.DateCreated = DateTime.Now;
				shareInfoTemp.EX_SenderName = this.LoginOperatorName;			// 確認画面用に格納
				shareInfoReadsTemp = GetShareInfoReads();
				break;

			case Constants.ACTION_STATUS_UPDATE:
				shareInfoTemp = GetInputAndCheck("CsShareInfoModify");
				shareInfoTemp.InfoNo = this.ShareInfo.InfoNo;
				shareInfoTemp.DateCreated = this.ShareInfo.DateCreated;
				shareInfoTemp.EX_SenderName = this.ShareInfo.EX_SenderName;		// 確認画面用に格納
				shareInfoReadsTemp = GetShareInfoReads();
				foreach (var read in shareInfoReadsTemp) read.InfoNo = shareInfoTemp.InfoNo;
				break;
		}
			
		// 確認画面へ
		Session[Constants.SESSION_KEY_SHAREINFO_INFO] = shareInfoTemp;
		Session[Constants.SESSION_KEY_SHAREINFOREAD_INFO] = shareInfoReadsTemp;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + HttpUtility.UrlEncode(this.ActionStatus));
	}
	#endregion

	#region -CsShareInfoReadModel 共有情報既読管理モデル取得
	/// <summary>
	/// 共有情報既読管理モデル取得
	/// </summary>
	/// <returns>共有情報既読管理モデルリスト</returns>
	private CsShareInfoReadModel[] GetShareInfoReads()
	{
		CsShareInfoReadService service = new CsShareInfoReadService(new CsShareInfoReadRepository());
		var models = new List<CsShareInfoReadModel>();

		var checkedOperators = GetCheckedOperators();
		foreach (var ope in checkedOperators)
		{
			var read = new CsShareInfoReadModel();
			read.DeptId = this.LoginOperatorDeptId;
			read.OperatorId = ope.Key;
			read.EX_OperatorName = ope.Value;
			read.ReadFlg = Constants.FLG_CSSHAREINFOREAD_READ_FLG_UNREAD;
			read.LastChanged = this.LoginOperatorName;
			models.Add(read);
		}
		return models.ToArray();
	}
	#endregion

	#region -GetCheckedOperators チェックされたオペレータID・名称取得
	/// <summary>
	/// チェックされたオペレータID・名称取得
	/// </summary>
	/// <returns>チェックされたオペレータID・名称</returns>
	private HashSet<KeyValuePair<string, string>> GetCheckedOperators()
	{
		var checkedOperatorIds = new HashSet<KeyValuePair<string, string>>();
		foreach (RepeaterItem riGrp in rCsGroups.Items)
		{
			Repeater rOperator = (Repeater)riGrp.FindControl("rCsOperators");
			foreach (RepeaterItem riOpe in rOperator.Items)
			{
				if (((CheckBox)(riOpe.FindControl("cbOperator"))).Checked)
				{
					checkedOperatorIds.Add(
						new KeyValuePair<string, string>(
							((HiddenField)riOpe.FindControl("hfOperatorId")).Value,
							((HiddenField)riOpe.FindControl("hfOperatorName")).Value));
				}
			}
		}
		foreach (RepeaterItem riOpe in rCsOperators.Items)
		{
			if (((CheckBox)(riOpe.FindControl("cbOperator"))).Checked)
			{
				checkedOperatorIds.Add(
					new KeyValuePair<string, string>(
						((HiddenField)riOpe.FindControl("hfOperatorId")).Value,
						((HiddenField)riOpe.FindControl("hfOperatorName")).Value));
			}
		}
		return checkedOperatorIds;
	}
	#endregion
}