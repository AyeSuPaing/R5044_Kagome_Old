/*
=========================================================================================================
  Module      : 検索パーツ処理(SearchParts.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Util;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.Cs.IncidentVoc;
using w2.App.Common.Cs.Search;
using w2.App.Common.Cs.SummarySetting;
using w2.App.Common.Web.Page;

public partial class Form_Top_SearchForm : BaseUserControl
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
			// 初期化
			Initialize();

			// 対象期間初期化（パフォーマンス向上のため）
			InitializeTargetPeriod();

			// 検索条件の復元
			SetSearchParameter();
			
			// 表示制御
			DisplayComponents();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// リスト表示モード
		rblListMode.Items.AddRange(new ListItem[]
		{
			new ListItem(
				// 「メッセージ単位」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_MESSAGE_UNIT),
				TopPageKbn.SearchMessage.ToString()),
			new ListItem(
				// 「インシデント単位」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_INCIDENT_UNIT),
				TopPageKbn.SearchIncident.ToString()),
		});
		rblListMode.SelectedIndex = 0;

		// 検索モード
		rblSearchMode.Items.AddRange(GetSearchModeListItems());
		rblSearchMode.SelectedIndex = 0;

		// 検索項目
		rbContentsAndHeader.Checked = true;
		// その他メッセージ項目：
		cblTargetMessageItem.Items.AddRange(GetTargetMessageItems());
		// その他インシデント項目：
		cblTargetIncidentItem.Items.AddRange(GetTargetIncidentItems());

		// 追加条件：タイプ
		cblMessageType.Items.AddRange(GetMessageTypeListItems());
		// 追加条件：メッセージステータス
		ddlMessageStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_MESSAGE_STATUS));
		// 追加条件：ステータス
		ddlStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS));
		// 追加条件：カテゴリ
		CsIncidentCategoryService serviceCategory = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		ddlCategory.Items.AddRange(serviceCategory.GetValidAll(this.LoginOperatorDeptId).Select(p => new ListItem(p.EX_CategoryNameForDropdown, p.CategoryId)).ToArray());
		// 追加条件：重要度
		ddlImportance.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_IMPORTANCE));
		// 追加条件：VOC
		CsIncidentVocService serviceVoc = new CsIncidentVocService(new CsIncidentVocRepository());
		ddlVoc.Items.AddRange(serviceVoc.GetValidAll(this.LoginOperatorDeptId).Select(p => new ListItem(p.VocText, p.VocId)).ToArray());
		// 追加条件：グループ
		CsOperatorService serviceOperator = new CsOperatorService(new CsOperatorRepository());
		ddlOperator.Items.Add(new ListItem("", ""));
		ddlOperator.Items.AddRange(serviceOperator.GetAll(this.LoginOperatorDeptId).Select(p => new ListItem(p.EX_ShopOperatorName, p.OperatorId)).ToArray());
		// 追加条件：担当者
		CsGroupService serviceGroup = new CsGroupService(new CsGroupRepository());
		ddlGroup.Items.Add(new ListItem("", ""));
		ddlGroup.Items.AddRange(serviceGroup.GetAll(this.LoginOperatorDeptId).Select(p => new ListItem(p.CsGroupName, p.CsGroupId)).ToArray());
		// 追加条件：集計区分値
		CsSummarySettingService service = new CsSummarySettingService(new CsSummarySettingRepository());
		var summarySettings = service.GetAllWithItems(this.LoginOperatorDeptId);
		foreach (var setting in summarySettings)
		{
			if (setting.SummarySettingType != Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO) continue;
			foreach (var settingItem in setting.EX_Items) settingItem.SummarySettingItemText = WebSanitizer.HtmlEncode(settingItem.SummarySettingItemText);
		}
		rIncidentSummary.DataSource = summarySettings;
		rIncidentSummary.DataBind();
		if (summarySettings.Length == 0) trSummarySetting.Visible = false;	// 集計区分設定が1つもないときは非表示
		foreach (RepeaterItem ri in rIncidentSummary.Items) { ((RadioButtonList)ri.FindControl("rblSummaryValue")).SelectedIndex = 0; }
		// 追加条件：対象期間
		rblPeriodType.SelectedIndex = 0;
	}

	/// <summary>
	/// 対象期間初期化（パフォーマンス向上のため）
	/// </summary>
	private void InitializeTargetPeriod()
	{
		if (Constants.SEARCH_PAGE_FIRSTVIEW_TIMESPAN_SETTING == "") return;

		cbPeriod.Checked = true;

		var ts = RelativeCalendar.FromText(Constants.SEARCH_PAGE_FIRSTVIEW_TIMESPAN_SETTING);
		tbPeriodFrom.Text = DateTimeUtility.ToStringForManager(
			ts.BeginTime,
			DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
		tbPeriodTo.Text = DateTimeUtility.ToStringForManager(
			ts.EndTime,
			DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
	}

	/// <summary>
	/// 検索モード選択肢のリストを返却します。
	/// </summary>
	/// <returns>選択肢のリスト</returns>
	private static ListItem[] GetSearchModeListItems()
	{
		return new ListItem[]
		{
			new ListItem(
				// 「すべて含む」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_INCLUDING_ALL),
				SearchMode.All.ToString()),
			new ListItem(
				// 「いずれか含む」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_INCLUDING_ANY),
				SearchMode.Any.ToString()),
			new ListItem(
				// 「完全一致」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_PERFECT_MATCHING),
				SearchMode.Exact.ToString()),
		};
	}

	/// <summary>
	/// その他メッセージ項目のリストを返却します。
	/// </summary>
	/// <returns>選択肢のリスト</returns>
	private static ListItem[] GetTargetMessageItems()
	{
		return new ListItem[]
		{
			new ListItem("From", SearchTargetMessageColumns.MessageFrom.ToString()),
			new ListItem("To", SearchTargetMessageColumns.MessageTo.ToString()),
			new ListItem("Cc", SearchTargetMessageColumns.MessageCc.ToString()),
			new ListItem("Bcc", SearchTargetMessageColumns.MessageBcc.ToString()),
			new ListItem("Subjcet", SearchTargetMessageColumns.MessageSubject.ToString()),
			new ListItem(
				CommonPage.ReplaceTag("@@User.name.name@@"),
				SearchTargetMessageColumns.MessageUserName.ToString()),
			new ListItem(
				CommonPage.ReplaceTag("@@User.tel1.name@@"),
				SearchTargetMessageColumns.MessageUserTel.ToString()),
			new ListItem(
				CommonPage.ReplaceTag("@@User.mail_addr.name@@"),
				SearchTargetMessageColumns.MessageUserMailAddr.ToString()),
			new ListItem(
				// 「件名」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_SUBJECT),
				SearchTargetMessageColumns.MessageInquiryTitle.ToString()),
		};
	}

	/// <summary>
	/// その他インシデント項目のリストを返却します。
	/// </summary>
	/// <returns>選択肢のリスト</returns>
	private static ListItem[] GetTargetIncidentItems()
	{
		return new ListItem[]
		{
			new ListItem(
				// 「タイトル」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_TITLE),
				SearchTargetIncidentColumns.IncidentTitle.ToString()),
			new ListItem(
				// 「VOCメモ」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_MEMO),
				SearchTargetIncidentColumns.IncidentVocMemo.ToString()),
			new ListItem(
				// 「内部メモ」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_INTERNAL_MEMO),
				SearchTargetIncidentColumns.IncidentComment.ToString()),
			new ListItem(
				// 「問合せ元名称/連絡先」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_INQUIRER_NAME_CONTACT_INFORMATION),
				SearchTargetIncidentColumns.IncidentFrom.ToString()),
		};
	}

	/// <summary>
	/// メッセージタイプ選択肢のリストを返却します。
	/// </summary>
	/// <returns>選択肢のリスト</returns>
	private static ListItem[] GetMessageTypeListItems()
	{
		return new ListItem[]
		{
			new ListItem(
				// 「メール受信」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_INCOMING_MAIL),
				SearchMessageTypes.Receive.ToString()),
			new ListItem(
				// 「メール送信」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_SEND_MAIL),
				SearchMessageTypes.Send.ToString()),
			new ListItem(
				// 「電話受信」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_RECEIVE_CALL),
				SearchMessageTypes.TellIn.ToString()),
			new ListItem(
				// 「電話発信」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_MAKE_CALL),
				SearchMessageTypes.TellOut.ToString()),
			new ListItem(
				// 「その他受信」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_OTHER_RECEPTION),
				SearchMessageTypes.OthersIn.ToString()),
			new ListItem(
				// 「その他発信」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SEARCH_PART,
					Constants.VALUETEXT_PARAM_SEARCH_MODEL_LIST,
					Constants.VALUETEXT_PARAM_OTHER_OUTGOING),
				SearchMessageTypes.OthersOut.ToString()),
		};
	}

	/// <summary>
	/// 検索パラメタの復元
	/// </summary>
	private void SetSearchParameter()
	{
		if (this.SearchParameter == null) return;

		// リスト表示モード
		rblListMode.SelectedValue = Request[Constants.REQUEST_KEY_CS_TOPPAGE_KBN];

		// キーワード欄
		tbKeyword.Text = this.SearchParameter.Keyword;
		rblSearchMode.SelectedValue = this.SearchParameter.Mode.ToString();

		// 検索項目欄
		rbContentsAndHeader.Checked = (this.SearchParameter.Target == SearchTarget.ContentsAndHeader);
		rbIncidentId.Checked = (this.SearchParameter.Target == SearchTarget.IncidentId);
		rbContents.Checked = (this.SearchParameter.Target == SearchTarget.Contents);
		rbHeader.Checked = (this.SearchParameter.Target == SearchTarget.Header);
		rbMessageItem.Checked = (this.SearchParameter.TargetMessageColumns != SearchTargetMessageColumns.NoSelection);
		foreach (ListItem li in cblTargetMessageItem.Items)
		{
			li.Selected = (this.SearchParameter.TargetMessageColumns.HasFlag((SearchTargetMessageColumns)Enum.Parse(typeof(SearchTargetMessageColumns), li.Value)));
		}
		rbIncidentItem.Checked = (this.SearchParameter.TargetIncidentColumns != SearchTargetIncidentColumns.NoSelection);
		foreach (ListItem li in cblTargetIncidentItem.Items)
		{
			li.Selected = (this.SearchParameter.TargetIncidentColumns.HasFlag((SearchTargetIncidentColumns)Enum.Parse(typeof(SearchTargetIncidentColumns), li.Value)));
		}

		// 追加条件欄
		cbMessageType.Checked = (this.SearchParameter.MessageType != SearchMessageTypes.NoSelection);
		foreach (ListItem cb in cblMessageType.Items)
		{
			cb.Selected = (this.SearchParameter.MessageType.HasFlag((SearchMessageTypes)Enum.Parse(typeof(SearchMessageTypes), cb.Value)));
		}
		cbMessageStatus.Checked = (this.SearchParameter.MessageStatusFilter != null);
		cbStatus.Checked = (this.SearchParameter.StatusFilter != null);
		ddlMessageStatus.SelectedValue = this.SearchParameter.MessageStatusFilter;
		ddlStatus.SelectedValue = this.SearchParameter.StatusFilter;
		cbCategory.Checked = (this.SearchParameter.CategoryFilter != null);
		ddlCategory.SelectedValue = this.SearchParameter.CategoryFilter;
		cbImportance.Checked = (this.SearchParameter.ImportanceFilter != null);
		ddlImportance.SelectedValue = this.SearchParameter.ImportanceFilter;
		cbVoc.Checked = (this.SearchParameter.VocFilter != null);
		ddlVoc.SelectedValue = this.SearchParameter.VocFilter;
		cbAssign.Checked = (this.SearchParameter.OperatorFilter != null || this.SearchParameter.GroupFilter != null);
		if (cbAssign.Checked) ddlGroup.SelectedValue = this.SearchParameter.GroupFilter;
		if (cbAssign.Checked) ddlOperator.SelectedValue = this.SearchParameter.OperatorFilter;
		cbPeriod.Checked = (this.SearchParameter.DateFromFilter != null || this.SearchParameter.DateToFilter != null);
		if (cbPeriod.Checked)
			tbPeriodFrom.Text = DateTimeUtility.ToStringForManager(
				this.SearchParameter.DateFromFilter,
				DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
		if (cbPeriod.Checked)
			tbPeriodTo.Text = DateTimeUtility.ToStringForManager(
				this.SearchParameter.DateToFilter,
				DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
		if (cbPeriod.Checked) rblPeriodType.SelectedValue = this.SearchParameter.DateKbn.ToString();

		// 追加条件欄（集計区分値）
		cbDisplayInvalid.Checked = this.SearchParameter.DisplayInvalidSummarySetting;
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			int summaryNo = int.Parse(((HiddenField)ri.FindControl("hfSummaryNo")).Value);

			if (this.SearchParameter.SummaryValues.ContainsKey(summaryNo))
			{
				((CheckBox)ri.FindControl("cbSummarySetting")).Checked = true;

				switch (((HiddenField)ri.FindControl("hfSummarySettingType")).Value)
				{
					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO:
						((RadioButtonList)ri.FindControl("rblSummaryValue")).SelectedValue = this.SearchParameter.SummaryValues[summaryNo];
						break;

					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN:
						((DropDownList)ri.FindControl("ddlSummaryValue")).SelectedValue = this.SearchParameter.SummaryValues[summaryNo];
						break;

					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT:
						((TextBox)ri.FindControl("tbSummaryValue")).Text = this.SearchParameter.SummaryValues[summaryNo];
						break;
				}
			}
		}

		// その他欄
		cbIncludeTrash.Checked = this.SearchParameter.IncludeTrash;
	}

	/// <summary>
	/// 表示制御
	/// </summary>
	private void DisplayComponents()
	{
		// 検索モードの表示/非表示
		rblSearchMode.Visible = (rbIncidentId.Checked == false);

		// 検索項目の有効/無効
		cblTargetMessageItem.Enabled = rbMessageItem.Checked;
		cblTargetIncidentItem.Enabled = rbIncidentItem.Checked;

		// 追加条件の有効/無効
		cblMessageType.Enabled = cbMessageType.Checked;
		ddlMessageStatus.Enabled = cbMessageStatus.Checked;
		ddlStatus.Enabled = cbStatus.Checked;
		ddlCategory.Enabled = cbCategory.Checked;
		ddlImportance.Enabled = cbImportance.Checked;
		ddlVoc.Enabled = cbVoc.Checked;
		ddlOperator.Enabled = cbAssign.Checked;
		ddlGroup.Enabled = cbAssign.Checked;
		tbPeriodFrom.Enabled = cbPeriod.Checked;
		tbPeriodTo.Enabled = cbPeriod.Checked;
		rblPeriodType.Enabled = cbPeriod.Checked;

		// 追加条件（集計区分）の有効/無効
		int itemCount = 0;
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			bool display = cbDisplayInvalid.Checked || (((HiddenField)ri.FindControl("hfValidFlg")).Value == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID);
			((HtmlTableRow)ri.FindControl("trSummarySettingRow")).Visible = display;
			if (display) itemCount++;
		}
		tdSummarySettingHead.RowSpan = itemCount + 1;
		tdSummarySettingData.Visible = (tdSummarySettingHead.RowSpan == 1) ? true : false;
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			((RadioButtonList)ri.FindControl("rblSummaryValue")).Enabled = ((CheckBox)ri.FindControl("cbSummarySetting")).Checked;
			((DropDownList)ri.FindControl("ddlSummaryValue")).Enabled = ((CheckBox)ri.FindControl("cbSummarySetting")).Checked;
			((TextBox)ri.FindControl("tbSummaryValue")).Enabled = ((CheckBox)ri.FindControl("cbSummarySetting")).Checked;
		}

		// 期間指定の日付入力チェック
		label.Text = "";
		if (cbPeriod.Checked && (tbPeriodFrom.Text.Trim() != "") && (CheckDateTimeFormat(tbPeriodFrom.Text) == null))
		{
			label.Text += WebMessages.GetMessages(WebMessages.MSG_MANAGER_SEARCH_PARTS_INCORRECT_START_DATE_FOR_PERIOD);
		}
		if (cbPeriod.Checked && (tbPeriodTo.Text.Trim() != "") && (CheckDateTimeFormat(tbPeriodTo.Text) == null))
		{
			label.Text += WebMessages.GetMessages(WebMessages.MSG_MANAGER_SEARCH_PARTS_INCORRECT_END_DATE_FOR_PERIOD);
		}
		label.Visible = (label.Text.Length != 0);
	}

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

	/// <summary>
	/// チェックボックス/ラジオボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void SelectedItem_Changed(object sender, EventArgs e)
	{
		DisplayComponents();
	}

	/// <summary>
	/// 担当グループ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCsGroup_SelectedIndexChanged(object sender, EventArgs e)
	{
		var before = ddlOperator.SelectedValue;

		ddlOperator.Items.Clear();
		ddlOperator.Items.Add("");
		ddlOperator.Items.AddRange(CreateCsOperatorItems(ddlGroup.SelectedValue));

		if (ddlOperator.Items.FindByValue(before) != null) ddlOperator.SelectedValue = before;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 検索パラメタをセッションへ格納
		SetSearchParamToSession();

		// 検索画面へ遷移して検索実行
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_TOP_PAGE + "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + Enum.Parse(typeof(TopPageKbn), rblListMode.SelectedValue));
	}

	/// <summary>
	/// 検索パラメタをセッションへ格納
	/// </summary>
	private void SetSearchParamToSession()
	{
		// 検索パラメータを組み立て
		SearchParameter sp = new SearchParameter();
		sp.Keyword = tbKeyword.Text;
		sp.Mode = (SearchMode)Enum.Parse(typeof(SearchMode), rblSearchMode.SelectedValue);
		sp.Target =
			rbContentsAndHeader.Checked ? SearchTarget.ContentsAndHeader :
			rbIncidentId.Checked ? SearchTarget.IncidentId :
			rbContents.Checked ? SearchTarget.Contents :
			rbHeader.Checked ? SearchTarget.Header :
			rbMessageItem.Checked ? SearchTarget.MessageColumns :
			rbIncidentItem.Checked ? SearchTarget.IncidentColumns : 
			SearchParameter.GetSearchTarget(null);
		if (rbMessageItem.Checked)
		{
			sp.TargetMessageColumns = SearchTargetMessageColumns.NoSelection;
			foreach (ListItem li in cblTargetMessageItem.Items)
			{
				if (li.Selected) sp.TargetMessageColumns |= (SearchTargetMessageColumns)Enum.Parse(typeof(SearchTargetMessageColumns), li.Value);
			}
			if (sp.TargetMessageColumns == SearchTargetMessageColumns.NoSelection)
			{
				foreach (ListItem li in cblTargetMessageItem.Items) sp.TargetMessageColumns |= (SearchTargetMessageColumns)Enum.Parse(typeof(SearchTargetMessageColumns), li.Value);
			}
		}
		if (rbIncidentItem.Checked)
		{
			sp.TargetIncidentColumns = SearchTargetIncidentColumns.NoSelection;
			foreach (ListItem li in cblTargetIncidentItem.Items)
			{
				if (li.Selected) sp.TargetIncidentColumns |= (SearchTargetIncidentColumns)Enum.Parse(typeof(SearchTargetIncidentColumns), li.Value);
			}
			if (sp.TargetIncidentColumns == SearchTargetIncidentColumns.NoSelection)
			{
				foreach (ListItem li in cblTargetIncidentItem.Items) sp.TargetIncidentColumns |= (SearchTargetIncidentColumns)Enum.Parse(typeof(SearchTargetIncidentColumns), li.Value);
			}
		}
		if (cbMessageType.Checked)
		{
			sp.MessageType = SearchMessageTypes.NoSelection;
			foreach (ListItem li in cblMessageType.Items)
			{
				if (li.Selected) sp.MessageType |= (SearchMessageTypes)Enum.Parse(typeof(SearchMessageTypes), li.Value);
			}
		}
		if (cbMessageStatus.Checked) sp.MessageStatusFilter = ddlMessageStatus.SelectedValue;
		if (cbStatus.Checked) sp.StatusFilter = ddlStatus.SelectedValue;
		if (cbCategory.Checked) sp.CategoryFilter = ddlCategory.SelectedValue;
		if (cbImportance.Checked) sp.ImportanceFilter = ddlImportance.SelectedValue;
		if (cbVoc.Checked) sp.VocFilter = ddlVoc.SelectedValue;
		if (cbAssign.Checked)
		{
			sp.GroupFilter = ddlGroup.SelectedValue;
			sp.OperatorFilter = ddlOperator.SelectedValue;
		}
		if (cbPeriod.Checked)
		{
			DateTime? periodFrom = CheckDateTimeFormat(tbPeriodFrom.Text);
			if (periodFrom != null) sp.DateFromFilter = periodFrom;
			DateTime? periodTo = CheckDateTimeFormat(tbPeriodTo.Text);
			if (periodTo != null) sp.DateToFilter = periodTo;
			sp.DateKbn = (SearchDateKbn)Enum.Parse(typeof(SearchDateKbn), rblPeriodType.SelectedValue);
		}
		sp.IncludeTrash = cbIncludeTrash.Checked;

		// 検索パラメータ（集計区分）を組み立て
		sp.DisplayInvalidSummarySetting = cbDisplayInvalid.Checked;
		Dictionary<int, string> dicValues = new Dictionary<int, string>();
		Dictionary<int, string> dicTypes = new Dictionary<int, string>();
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			if (((CheckBox)ri.FindControl("cbSummarySetting")).Checked)
			{
				int summaryNo = int.Parse(((HiddenField)ri.FindControl("hfSummaryNo")).Value);
				string summaryType = ((HiddenField)ri.FindControl("hfSummarySettingType")).Value;
				string summaryValue = "";

				switch (summaryType)
				{
					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO:
						if (((RadioButtonList)ri.FindControl("rblSummaryValue")).Visible == false) continue;	// 折りたたまれて非表示のときは検索しない
						summaryValue = ((RadioButtonList)ri.FindControl("rblSummaryValue")).SelectedValue;
						break;

					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN:
						if (((DropDownList)ri.FindControl("ddlSummaryValue")).Visible == false) continue;		// 折りたたまれて非表示のときは検索しない
						summaryValue = ((DropDownList)ri.FindControl("ddlSummaryValue")).SelectedValue;
						break;

					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT:
						if (((TextBox)ri.FindControl("tbSummaryValue")).Visible == false) continue;				// 折りたたまれて非表示のときは検索しない
						summaryValue = ((TextBox)ri.FindControl("tbSummaryValue")).Text;
						break;
				}

				if (summaryValue == "") continue; // テキストボックスで空入力時は検索しない（ドロップダウン/ラジオボタンリストに合わせる）

				dicValues.Add(summaryNo, summaryValue);
				dicTypes.Add(summaryNo, summaryType);
			}
		}
		sp.SummaryValues = dicValues;
		sp.SummarySettingTypes = dicTypes;

		// 検索条件をセッションに保存
		this.SearchParameter = sp;
	}

	/// <summary>
	/// 日付フォーマットチェック
	/// </summary>
	/// <param name="inputText">入力テキスト</param>
	/// <returns>変換後のDateTime値、チェックNGであればnull</returns>
	private static DateTime? CheckDateTimeFormat(string inputText)
	{
		// YYYYMMDD形式も入力可能とする
		DateTime tmp;
		return (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
			? DateTime.TryParse(inputText, out tmp)
			: DateTime.TryParse(
				inputText,
				new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE),
				DateTimeStyles.None,
				out tmp) || DateTime.TryParseExact(inputText, "yyyyMMdd", null, DateTimeStyles.None, out tmp))
			? (DateTime?)tmp
			: null;
	}

	/// <summary>検索パラメタ（セッションに保存）</summary>
	private SearchParameter SearchParameter
	{
		get { return (SearchParameter)Session[Constants.SESSION_KEY_CS_SEARCH_PARAMETER]; }
		set { Session[Constants.SESSION_KEY_CS_SEARCH_PARAMETER] = value; }
	}
}
