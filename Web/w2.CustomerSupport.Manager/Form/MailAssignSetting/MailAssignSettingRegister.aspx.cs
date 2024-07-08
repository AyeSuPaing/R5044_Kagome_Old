/*
=========================================================================================================
  Module      : 受信時振分けルール設定登録ページ処理(MailAssignSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.MailAssignSetting;

public partial class Form_MailAssignSetting_MailAssignSettingRegister : MailAssignSettingPage
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

			// データ表示処理
			SetViewData();
		}
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	private void InitializeComponent()
	{
		// プルダウンリスト用データを取得
		var categoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		var categories = categoryService.GetValidAll(this.LoginOperatorDeptId);
		var groupService = new CsGroupService(new CsGroupRepository());
		var groups = groupService.GetValidAll(this.LoginOperatorDeptId);

		// 論理演算子（かつ/または）
		rblLogicalOperation.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSMAILASSIGNSETTING, Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR));
		
		// 振分けルール：ステータス
		ddlAssignStatus.Items.Add(new ListItem("", ""));
		ddlAssignStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS));
		// 振分けルール：カテゴリ
		ddlIncidentCateory.Items.Add(new ListItem("", ""));
		ddlIncidentCateory.Items.AddRange(categories.Select(cat => new ListItem(cat.EX_CategoryNameForDropdown, cat.CategoryId)).ToArray());
		// 振分けルール：重要度
		ddlAssignImportance.Items.Add(new ListItem("", ""));
		ddlAssignImportance.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_IMPORTANCE));
		// 振分けルール：担当グループ
		ddlAssignCsGroup.Items.Add(new ListItem("", ""));
		ddlAssignCsGroup.Items.AddRange(groups.Select(model => new ListItem(model.CsGroupName, model.CsGroupId)).ToArray());
		ddlAssignCsGroup.Items.Add(new ListItem(Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_CLEAR_TEXT, Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_GROUP_ID_CLEAR));
		// 振分けルール：担当オペレータ
		DisplayOperatorsFromGroupSelected();
	}

	/// <summary>
	/// データ表示処理
	/// </summary>
	private void SetViewData()
	{
		// 処理区分に応じた制御
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:	// 新規登録
				DispControlInsert();
				trRegister.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:	// 更新
				DispControlUpdate();
				trEdit.Visible = true;
				break;

			default:
				// 不正パラメタエラー
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		DataBind();
	}

	/// <summary>
	/// 新規登録時表示制御
	/// </summary>
	private void DispControlInsert()
	{
		if (this.MailAssignSettingModel != null)
		{
			// 確認画面に渡した入力情報を復元
			SetMailAssignSettingModel(this.MailAssignSettingModel);
		}
		else
		{
			// 新規登録用に初期情報セット
			rblLogicalOperation.SelectedIndex = 0;	// 論理演算子
			cbStopFiltering.Checked = true;		// 振分け停止：SelectedValue指定と重なるとエラーになるので注意

			// 振分条件詳細の初期化
			List<MailAssignSettingItem> conditionList = new List<MailAssignSettingItem>();
			conditionList.Add(new MailAssignSettingItem());
			rConditionList.DataSource = conditionList;
		}
	}

	/// <summary>
	/// 更新時表示制御
	/// </summary>
	private void DispControlUpdate()
	{
		if (this.MailAssignSettingModel != null)
		{
			// 確認画面に渡した入力情報を復元
			SetMailAssignSettingModel(this.MailAssignSettingModel);
		}
		else
		{
			// 更新用にメール振分け設定情報セット
			CsMailAssignSettingService service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
			SetMailAssignSettingModel(service.GetWithItems(this.LoginOperatorDeptId, this.MailAssignId));
		}
	}

	/// <summary>
	/// メール振分け設定モデルをセット
	/// </summary>
	/// <param name="model">メール振分け設定モデル</param>
	private void SetMailAssignSettingModel(CsMailAssignSettingModel model)
	{
		// 条件指定：優先順
		tbAssignPriority.Text = StringUtility.ToEmpty(model.AssignPriority);
		// 条件指定：論理演算子（かつ/または）
		rblLogicalOperation.SelectedValue = model.LogicalOperator;
		// 条件指定：振分け停止
		cbStopFiltering.Checked = (model.StopFiltering == Constants.FLG_CSMAILASSIGNSETTING_STOP_FILTERING_VALID);

		// 条件指定：振分け条件詳細
		rConditionList.DataSource = model.Items.Select(m => new MailAssignSettingItem(m));

		// 振分けルール
		ddlAssignStatus.SelectedValue = StringUtility.ToEmpty(model.AssignStatus);
		ddlIncidentCateory.SelectedValue = (ddlIncidentCateory.Items.FindByValue(model.AssignIncidentCategoryId) != null) ? StringUtility.ToEmpty(model.AssignIncidentCategoryId) : "";
		ddlAssignImportance.SelectedValue = StringUtility.ToEmpty(model.AssignImportance);
		ddlAssignCsGroup.SelectedValue = (ddlAssignCsGroup.Items.FindByValue(model.AssignCsGroupId) != null) ? StringUtility.ToEmpty(model.AssignCsGroupId) : "";
		DisplayOperatorsFromGroupSelected();
		ddlAssignOperator.SelectedValue = (ddlAssignOperator.Items.FindByValue(model.AssignOperatorId) != null) ? StringUtility.ToEmpty(model.AssignOperatorId) : "";
		cbTrash.Checked = (model.Trash == Constants.FLG_CSMAILASSIGNSETTING_TRASH_VALID);

		// オートレスポンス
		cbAutoResponse.Checked = (model.AutoResponse == Constants.FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_VALID);
		tbAutoResponseFrom.Text = model.AutoResponseFrom;
		tbAutoResponseCc.Text = model.AutoResponseCc;
		tbAutoResponseBcc.Text = model.AutoResponseBcc;
		tbAutoResponseSubject.Text = model.AutoResponseSubject;
		tbAutoResponseBody.Text = model.AutoResponseBody;
		if (model.AutoResponse == Constants.FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_VALID)
		{
			trAutoResponseFrom.Visible = true;
			trAutoResponseCc.Visible = true;
			trAutoResponseBcc.Visible = true;
			trAutoResponseSubject.Visible = true;
			trAutoResponseBody.Visible = true;
		}

		// システム固定ルールの表示切り替え
		switch (model.MailAssignId)
		{
			case Constants.CONST_MAIL_ASSIGN_ID_MATCH_ON_BIND:
				conditionNormal.Visible = false;
				conditionBind.Visible = true;
				tbAssignPriority.Enabled = false;
				break;

			case Constants.CONST_MAIL_ASSIGN_ID_MATCH_ANYTHING:
				conditionNormal.Visible = false;
				conditionNoMatch.Visible = true;
				tbAssignPriority.Enabled = false;
				break;
		}

		// 振分設定名
		tbMailAssignName.Text = model.MailAssignName;
	}

	/// <summary>
	/// ラジオボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblLogicalOperation_Click(object sender, EventArgs e)
	{
		List<MailAssignSettingItem> conditionList = GetCurrentAssignItemList();
		rConditionList.DataSource = conditionList;
		rConditionList.DataBind();
	}

	/// <summary>
	/// 追加/削除ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddDel_Click(object sender, EventArgs e)
	{
		// 押したボタンのモード,index取得
		string mode = ((Button)sender).CommandName;
		int index = int.Parse(((Button)sender).CommandArgument);

		List<MailAssignSettingItem> conditionList = GetCurrentAssignItemList();
		switch (mode)
		{
			case "Add":
				conditionList.Insert(index + 1, (new MailAssignSettingItem()));
				break;

			case "Del":
				conditionList.RemoveAt(index);
				break;
		}

		rConditionList.DataSource = conditionList;
		rConditionList.DataBind();
	}

	/// <summary>
	/// 現在の入力/選択状態を取得
	/// </summary>
	/// <returns>振分設定アイテムのリスト</returns>
	private List<MailAssignSettingItem> GetCurrentAssignItemList()
	{
		List<MailAssignSettingItem> conditionList = new List<MailAssignSettingItem>();
		foreach (RepeaterItem ri in rConditionList.Items)
		{
			MailAssignSettingItem item = new MailAssignSettingItem();
			item.SelectedMatchingTarget = ((DropDownList)ri.FindControl("ddlAssignItem")).SelectedItem;
			item.ConditionValue = ((TextBox)ri.FindControl("tbKeyword")).Text;
			item.SelectedIncludeCondition = ((DropDownList)ri.FindControl("ddlAssignItemCondition")).SelectedItem;
			item.CaseSensitive = ((CheckBox)ri.FindControl("cbCaseSensitive")).Checked;
			conditionList.Add(item);
		}
		return conditionList;
	}

	/// <summary>
	/// オートレスポンスON/OFF時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbAutoResponse_CheckedChanged(object sender, EventArgs e)
	{
		trAutoResponseFrom.Visible
			= trAutoResponseCc.Visible
			= trAutoResponseBcc.Visible
			= trAutoResponseSubject.Visible
			= trAutoResponseBody.Visible
			= cbAutoResponse.Checked;
	}

	/// <summary>
	/// 戻るボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnbackTop_Click(object sender, EventArgs e)
	{
		// 保持している入力を破棄
		this.MailAssignSettingModel = null;

		switch (this.ActionStatus)
		{
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				Response.Redirect(CreateListUrl());
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				Response.Redirect(CreateDetailUrl(this.MailAssignId));
				break;
		}
	}

	/// <summary>
	/// 確認ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// 入力値検証
		var result = ValidateInput();

		// エラーの場合、処理を抜ける
		if (result == false) return;

		// 入力値からモデル作成
		CsMailAssignSettingModel inputModel = GetCurrentSettingModel();

		switch (this.ActionStatus)
		{
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				this.MailAssignSettingModel = inputModel;
				Response.Redirect(CreateInsertConfirmUrl());
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				this.MailAssignSettingModel = inputModel;
				Response.Redirect(CreateUpdateConfirmUrl(this.MailAssignId));
				break;
		}
	}

	/// <summary>
	/// Assign cs group selected index changed
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event argument</param>
	protected void ddlAssignCsGroup_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayOperatorsFromGroupSelected();
	}
	
	/// <summary>
	/// 入力値検証
	/// </summary>
	/// <returns>正常：true、エラー：false</returns>
	private bool ValidateInput()
	{
		var ht = new Hashtable();
		var autoResponseInput = new Hashtable();
		var errorMessage = new StringBuilder();
		var autoResponseErrorMessage = new StringBuilder();

		// 優先順のチェック（システム固定ルールのときはスキップ）
		if ((this.MailAssignId != Constants.CONST_MAIL_ASSIGN_ID_MATCH_ON_BIND) && (this.MailAssignId != Constants.CONST_MAIL_ASSIGN_ID_MATCH_ANYTHING))
		{
			ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, this.LoginOperatorDeptId);
			ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID, this.MailAssignId);
			ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_PRIORITY, tbAssignPriority.Text);
		}

		ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_NAME, tbMailAssignName.Text);
		errorMessage.Append(Validator.Validate("CsMailAssignSetting", ht));
		ht.Clear();

		// 振分け条件のチェック
		foreach (MailAssignSettingItem con in GetCurrentAssignItemList())
		{
			if (this.MailAssignId == Constants.CONST_MAIL_ASSIGN_ID_MATCH_ON_BIND) continue;	// システム固定ルールのときはスキップ
			if (this.MailAssignId == Constants.CONST_MAIL_ASSIGN_ID_MATCH_ANYTHING) continue;	// システム固定ルールのときはスキップ
			ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_VALUE, con.ConditionValue);
			errorMessage.Append(Validator.Validate("CsMailAssignSetting", ht));
			ht.Clear();
		}

		// オートレスポンス
		if (cbAutoResponse.Checked)
		{
			autoResponseInput.Add(Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_FROM, tbAutoResponseFrom.Text);
			autoResponseInput.Add(Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_CC, tbAutoResponseCc.Text);
			autoResponseInput.Add(Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BCC, tbAutoResponseBcc.Text);
			autoResponseInput.Add(Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_SUBJECT, tbAutoResponseSubject.Text);
			autoResponseInput.Add(Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BODY, tbAutoResponseBody.Text);
		}

		// すべての条件のチェック
		errorMessage.Append(Validator.Validate("CsMailAssignSetting", ht));
		autoResponseErrorMessage.Append(Validator.Validate("CsMailAssignSetting", autoResponseInput));
		if (cbAutoResponse.Checked)
		{
			autoResponseErrorMessage.Append(CheckMailAddrInputs(tbAutoResponseCc.Text));
			autoResponseErrorMessage.Append(CheckMailAddrInputs(tbAutoResponseBcc.Text));
		}

		// エラー表示
		trMailAssignErrorMessages.Visible =
			trMailAssignErrorMessagesTitle.Visible = (errorMessage.Length != 0);
		lbMailAssignErrorMessages.Text = errorMessage.ToString();
		trAutoResponseErrorMessages.Visible =
			trAutoResponseErrorMessagesTitle.Visible = (autoResponseErrorMessage.Length != 0);
		lbAutoResponseErrorMessages.Text = autoResponseErrorMessage.ToString();

		return (errorMessage.Length == 0) && (autoResponseErrorMessage.Length == 0);
	}

	/// <summary>
	/// 入力値のモデルを生成
	/// </summary>
	/// <returns></returns>
	private CsMailAssignSettingModel GetCurrentSettingModel()
	{
		CsMailAssignSettingModel model = new CsMailAssignSettingModel();
		model.DeptId = this.LoginOperatorDeptId;
		model.MailAssignId = this.MailAssignId;

		// 条件指定：優先順
		model.AssignPriority = int.Parse(tbAssignPriority.Text);
		// 条件指定：論理演算子（かつ/または）
		model.LogicalOperator = rblLogicalOperation.SelectedValue;
		// 条件指定：振分け停止
		model.StopFiltering = cbStopFiltering.Checked ? Constants.FLG_CSMAILASSIGNSETTING_STOP_FILTERING_VALID : Constants.FLG_CSMAILASSIGNSETTING_STOP_FILTERING_INVALID;

		// 条件指定：振分け条件詳細
		List<CsMailAssignSettingItemModel> itemModelList = new List<CsMailAssignSettingItemModel>();
		int counter = 1;
		foreach (MailAssignSettingItem con in GetCurrentAssignItemList())
		{
			// 主キー項目
			CsMailAssignSettingItemModel itemModel = new CsMailAssignSettingItemModel();
			itemModel.DeptId = this.LoginOperatorDeptId;
			itemModel.MailAssignId = this.MailAssignId;
			itemModel.ItemNo = counter++;

			// 1行分のデータセット
			itemModel.MatchingTarget = con.SelectedMatchingTarget.Value;
			itemModel.MatchingValue = con.ConditionValue;
			itemModel.MatchingType = con.SelectedIncludeCondition.Value;
			itemModel.IgnoreCase = con.CaseSensitive ? Constants.FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_INVALID : Constants.FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_VALID;
			itemModel.LastChanged = this.LoginOperatorName;

			// データ格納
			itemModelList.Add(itemModel);
		}
		model.Items = itemModelList.ToArray();

		// 振分けルール
		model.AssignStatus = ddlAssignStatus.SelectedValue;
		model.AssignIncidentCategoryId = ddlIncidentCateory.SelectedValue;
		model.AssignImportance = ddlAssignImportance.SelectedValue;
		model.AssignCsGroupId = ddlAssignCsGroup.SelectedValue;
		model.AssignOperatorId = ddlAssignOperator.SelectedValue;
		model.Trash = cbTrash.Checked ? Constants.FLG_CSMAILASSIGNSETTING_TRASH_VALID : Constants.FLG_CSMAILASSIGNSETTING_TRASH_INVALID;
		
		// オートレスポンス
		model.AutoResponse = cbAutoResponse.Checked ? Constants.FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_VALID : Constants.FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_INVALID;
		model.AutoResponseFrom = cbAutoResponse.Checked ? tbAutoResponseFrom.Text : string.Empty;
		model.AutoResponseCc = cbAutoResponse.Checked ? tbAutoResponseCc.Text : string.Empty;
		model.AutoResponseBcc = cbAutoResponse.Checked ? tbAutoResponseBcc.Text : string.Empty;
		model.AutoResponseSubject = cbAutoResponse.Checked ? tbAutoResponseSubject.Text : string.Empty;
		model.AutoResponseBody = cbAutoResponse.Checked ? tbAutoResponseBody.Text : string.Empty;

		// その他
		model.ValidFlg = Constants.FLG_CSMAILASSIGNSETTING_VALID_FLG_VALID;
		model.LastChanged = this.LoginOperatorName;

		// 確認画面表示用
		model.EX_AssignIncidentCategoryName = (ddlIncidentCateory.SelectedValue == "") ? "" : ddlIncidentCateory.SelectedItem.Text;
		model.EX_AssignOperatorName = (ddlAssignOperator.SelectedValue == "") ? "" : ddlAssignOperator.SelectedItem.Text;
		model.EX_AssignCsGroupName = (ddlAssignCsGroup.SelectedValue == "") ? "" : ddlAssignCsGroup.SelectedItem.Text;

		// 振分設定名
		model.MailAssignName = tbMailAssignName.Text;

		return model;
	}

	/// <summary>
	/// Display Operators From Group Selected
	/// </summary>
	private void DisplayOperatorsFromGroupSelected()
	{
		ddlAssignOperator.Items.Clear();

		// Get Operators
		var operators = new CsOperatorGroupService(new CsOperatorGroupRepository()).GetValidOperators(this.LoginOperatorDeptId, ddlAssignCsGroup.SelectedValue);

		// Filter Assign Operators
		ddlAssignOperator.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlAssignOperator.Items.AddRange(operators.Select(model => new ListItem(model.EX_ShopOperatorName, model.OperatorId)).ToArray());
		ddlAssignOperator.Items.Add(new ListItem(Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_CLEAR_TEXT, Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID_CLEAR));
	}

	#region メール振分け設定アイテムデータクラス
	/// <summary>
	/// メール振分け設定アイテムデータクラス
	/// </summary>
	protected class MailAssignSettingItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MailAssignSettingItem()
		{
			this.SelectedMatchingTarget = this.AssignItemList[0];
			this.ConditionValue = "";
			this.CaseSensitive = false;
			this.SelectedIncludeCondition = this.IncludeConditionList[0];
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">メール振分け設定アイテムモデル</param>
		public MailAssignSettingItem(CsMailAssignSettingItemModel model)
		{
			this.SelectedMatchingTarget = this.AssignItemList.First(p => p.Value == model.MatchingTarget);
			this.ConditionValue = model.MatchingValue;
			this.CaseSensitive = (model.IgnoreCase != Constants.FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_VALID);
			this.SelectedIncludeCondition = this.IncludeConditionList.First(p => p.Value == model.MatchingType);
		}

		#region プロパティ
		/// <summary>対象振分け項目</summary>
		public ListItem SelectedMatchingTarget { get; set; }
		/// <summary>検索値</summary>
		public string ConditionValue { get; set; }
		/// <summary>大文字小文字を区別するか</summary>
		public bool CaseSensitive { get; set; }
		/// <summary>検索値を含む/含まないの条件</summary>
		public ListItem SelectedIncludeCondition { get; set; }
		/// <summary>振分項目の選択肢リスト</summary>
		public ListItem[] AssignItemList { get { return ValueText.GetValueItemArray(Constants.TABLE_CSMAILASSIGNSETTINGITEM, Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET); } }
		/// <summary>検索値含有種別の選択肢リスト</summary>
		public ListItem[] IncludeConditionList { get { return ValueText.GetValueItemArray(Constants.TABLE_CSMAILASSIGNSETTINGITEM, Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE); } }
		#endregion
	}
	#endregion
}
