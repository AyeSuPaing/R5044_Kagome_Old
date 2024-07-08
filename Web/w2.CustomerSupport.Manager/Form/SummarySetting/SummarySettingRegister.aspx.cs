/*
=========================================================================================================
  Module      : 集計区分設定登録ページ処理(SummarySettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Text;
using w2.App.Common.Cs.SummarySetting;

public partial class Form_SummarySetting_SummarySettingRegister : BasePage
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

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// 集計区分設定表示
			DisplayData();
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
				break;

			case Constants.ACTION_STATUS_UPDATE:
				trEdit.Visible = true;
				trSummarySettingNo.Visible = true;
				break;
		}

		// 表示順ドロップダウン作成
		ddlDisplayOrder.Items.AddRange(this.DispOrderListItems);

		// 入力タイプラジオボタン作成
		rblSummarySettingType.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_CSSUMMARYSETTING, Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE));
		rblSummarySettingType.SelectedIndex = 0;
	}
	#endregion

	#region -DisplayData  集計区分設定表示
	/// <summary>
	/// 集計区分設定表示
	/// </summary>
	private void DisplayData()
	{
		CsSummarySettingModel setting = null;

		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				var item = CreateDummyItem();
				setting = new CsSummarySettingModel();
				setting.SummarySettingNo = 0;
				setting.DisplayOrder = 1;
				setting.SummarySettingType = Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO;
				setting.ValidFlg = Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID;
				setting.EX_Items = new CsSummarySettingItemModel[] { item };
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				setting = (CsSummarySettingModel)Session[Constants.SESSION_KEY_SUMMARYSETTING_INFO];
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		lSummarySettingNo.Text = setting.SummarySettingNo.ToString();
		hfSummarySettingNo.Value = setting.SummarySettingNo.ToString();
		tbSummarySettingTitle.Text = setting.SummarySettingTitle;
		foreach (ListItem li in ddlDisplayOrder.Items) li.Selected = (li.Value == setting.DisplayOrder.ToString());
		cbValidFlg.Checked = (setting.ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID);
		foreach (ListItem li in rblSummarySettingType.Items) li.Selected = (li.Value == setting.SummarySettingType);

		// 問合せ集計区分アイテム入力域表示切替
		DisplaySummarySettingItem(rblSummarySettingType.SelectedValue);

		rSummarySettingItems.DataSource = setting.EX_Items;
		rSummarySettingItems.DataBind();
	}
	#endregion

	#region -DisplaySummarySettingItem 問合せ集計区分アイテム入力域表示切替処理
	/// <summary>
	/// 問合せ集計区分アイテム入力域表示切替処理
	/// </summary>
	/// <param name="summarySettingType">問合せ集計区分入力種別</param>
	private void DisplaySummarySettingItem(string summarySettingType)
	{
		switch (summarySettingType)
		{
			case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO:
			case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN:
				divSummarySettingItems.Visible = true;
				break;

			case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT:
				divSummarySettingItems.Visible = false;
				break;
		}
	}
	#endregion

	#region #btnAddItem_Click アイテム追加ボタンクリック
	/// <summary>
	/// アイテム追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddItem_Click(object sender, System.EventArgs e)
	{
		var items = GetSummarySettingItemInputs();
		items.Add(CreateDummyItem());

		rSummarySettingItems.DataSource = items.ToArray();
		rSummarySettingItems.DataBind();
	}
	#endregion

	#region #btnDeleteItem_Click アイテム削除ボタンクリック
	/// <summary>
	/// アイテム削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteItem_Click(object sender, System.EventArgs e)
	{
		var items = GetSummarySettingItemInputs();
		int index = int.Parse(((Button)sender).CommandArgument);
		items.RemoveAt(index);

		rSummarySettingItems.DataSource = items.ToArray();
		rSummarySettingItems.DataBind();
	}
	#endregion

	#region -CreateDummyItem ダミーアイテム作成
	/// <summary>
	/// ダミーアイテム作成
	/// </summary>
	/// <returns>ダミーアイテム</returns>
	private CsSummarySettingItemModel CreateDummyItem()
	{
		var item = new CsSummarySettingItemModel();
		item.SummarySettingNo = 0;	// ダミー
		item.DisplayOrder = 1;
		item.ValidFlg = Constants.FLG_CSSUMMARYSETTINGITEM_VALID_FLG_VALID;
		item.DateCreated = DateTime.MinValue;
		item.DateChanged = DateTime.MinValue;
		return item;
	}
	#endregion

	#region #btnConfirm_Click 確認するボタンクリック
	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 入力値取得
		var setting = GetInput();

		// 入力チェック
		CheckInput(setting);

		// パラメタをセッションへ格納＆画面遷移
		Session[Constants.SESSION_KEY_SUMMARYSETTING_INFO] = setting;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SUMMARYSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);		
	}
	#endregion

	#region -GetInput 入力値取得
	/// <summary>
	/// 入力値取得
	/// </summary>
	/// <returns>入力値</returns>
	private CsSummarySettingModel GetInput()
	{
		var setting = new CsSummarySettingModel();
		setting.DeptId = this.LoginOperatorDeptId;
		setting.SummarySettingNo = int.Parse(hfSummarySettingNo.Value);
		setting.SummarySettingTitle = tbSummarySettingTitle.Text;
		setting.ValidFlg = cbValidFlg.Checked ? Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID : Constants.FLG_CSSUMMARYSETTING_VALID_FLG_INVALID;
		setting.DisplayOrder = int.Parse(ddlDisplayOrder.SelectedValue);
		setting.SummarySettingType = rblSummarySettingType.SelectedValue;
		setting.DateCreated = DateTime.MinValue;
		setting.DateChanged = DateTime.MinValue;
		setting.LastChanged = this.LoginOperatorName;

		var items = new List<CsSummarySettingItemModel>();
		if ((rblSummarySettingType.SelectedValue == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO)
			|| (rblSummarySettingType.SelectedValue == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN))
		{
			items = GetSummarySettingItemInputs();
		}
		setting.EX_Items = items.ToArray();

		return setting;
	}
	#endregion

	#region -CheckInput 入力チェック
	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <param name="setting">集計区分設定</param>
	private void CheckInput(CsSummarySettingModel setting)
	{
		StringBuilder errorMessages = new StringBuilder();

		// 集計区分入力チェック
		string validator = null;
		if (this.ActionStatus == Constants.ACTION_STATUS_INSERT || this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			validator = "CsSummarySettingRegister";
		}
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			validator = "CsSummarySettingModify";
		}
		Hashtable settingForCheck = (Hashtable)setting.DataSource.Clone();
		settingForCheck[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_NO] = setting.SummarySettingNo.ToString();	// validator用に文字列で置き換える
		settingForCheck[Constants.FIELD_CSSUMMARYSETTING_DISPLAY_ORDER] = setting.DisplayOrder.ToString();	// validator用に文字列で置き換える
		errorMessages.Append(Validator.Validate(validator, settingForCheck));

		// アイテム入力チェック＆ID重複チェック
		int index = 0;
		bool duplicationError = false;
		List<string> idsForDuplicationCheck = new List<string>();
		foreach (var item in setting.EX_Items)
		{
			Hashtable itemForCheck = (Hashtable)item.DataSource.Clone();
			itemForCheck[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_NO] = item.SummarySettingNo.ToString();	// validator用に文字列で置き換える
			itemForCheck[Constants.FIELD_CSSUMMARYSETTINGITEM_DISPLAY_ORDER] = item.DisplayOrder.ToString();	// validator用に文字列で置き換える

			errorMessages.Append(Validator.Validate("CsSummarySettingItemRegister", itemForCheck)
				.Replace(
					"@@ 1 @@",
					string.Format(
						ReplaceTag("@@DispText.common_message.location_no@@"),
						(index + 1).ToString(),
						string.Empty)));

			if (idsForDuplicationCheck.Contains(item.SummarySettingItemId))
			{
				duplicationError = true;
			}
			else
			{
				idsForDuplicationCheck.Add(item.SummarySettingItemId);
			}
			index++;
		}
		if (duplicationError)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
				.Replace(
					"@@ 1 @@",
					// 「保存する値」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_SUMMARY_SETTING_REGISTER,
						Constants.VALUETEXT_PARAM_ITEM_INPUT_CHECK,
						Constants.VALUETEXT_PARAM_VALUE_SAVE)));
		}

		// ラジオボタン・ドロップダウンでは有効な集計アイテム情報を１つ以上登録させる
		switch (rblSummarySettingType.SelectedValue)
		{
			case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO:
			case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN:
				if ((setting.ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID)
					&& (setting.EX_Items.Any(item => item.ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID) == false))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERROR_MANAGER_REGISTER_ONE_OR_MORE_VALID_AGGREGATE_ITEM_INFORMATION));
				}
				break;

			case Constants. FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT:
			default:
				// なにもしない
				break;
		}

		// エラーがあれば画面遷移
		if (errorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
	#endregion

	#region -GetSummarySettingItemInputs 集計区分アイテム入力値取得
	/// <summary>
	/// 集計区分アイテム入力値取得
	/// </summary>
	/// <returns>アイテム入力値</returns>
	private List<CsSummarySettingItemModel> GetSummarySettingItemInputs()
	{
		var items = new List<CsSummarySettingItemModel>();
		foreach (RepeaterItem ri in rSummarySettingItems.Items)
		{
			var item = new CsSummarySettingItemModel();
			item.DeptId = this.LoginOperatorDeptId;
			item.SummarySettingNo = int.Parse(hfSummarySettingNo.Value);
			item.DisplayOrder = int.Parse(((DropDownList)ri.FindControl("ddlDisplayOrderItem")).SelectedValue);
			item.SummarySettingItemId = ((TextBox)ri.FindControl("tbSummarySettingItemId")).Text;
			item.SummarySettingItemText = ((TextBox)ri.FindControl("tbSummarySettingItemText")).Text;
			item.ValidFlg = ((CheckBox)ri.FindControl("chklValidFlg")).Checked ? Constants.FLG_CSSUMMARYSETTINGITEM_VALID_FLG_VALID : Constants.FLG_CSSUMMARYSETTING_VALID_FLG_INVALID;
			item.LastChanged = this.LoginOperatorName;
			items.Add(item);
		}
		return items;
	}
	#endregion

	#region #rblSummarySettingType_SelectedIndexChanged 入力タイプ変更イベント
	/// <summary>
	/// 入力タイプ変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblSummarySettingType_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.DisplaySummarySettingItem(rblSummarySettingType.SelectedValue);
	}
	#endregion

	#region プロパティ
	/// <summary>並び順リストアイテム</summary>
	protected ListItem[] DispOrderListItems
	{
		get { return (from i in Enumerable.Range(1, 100) select new ListItem(i.ToString(), i.ToString())).ToArray(); }
	}
	#endregion
}