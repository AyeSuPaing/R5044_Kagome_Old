/*
=========================================================================================================
  Module      : ユーザ拡張項目設定ページ処理(UserExtendSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.TargetList;
using w2.Common.Util;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserExtendSetting;

public partial class Form_UserExtendSetting_UserExtendSettingList : BasePage
{
	#region 定数
	/// <summary>ユーザ拡張項目設定リスト</summary>
	private const string CACHE_KEY_USEREXTENDSETTING = "userextendsetting";
	/// <summary>入力方法別初期値リスト(テキスト)</summary>
	private const string CACHE_KEY_INPUTDEFAULT_FOR_TEXT = "input_default_for_text";
	/// <summary>入力方法別初期値リスト(リスト)</summary>
	private const string CACHE_KEY_INPUTDEFAULT_FOR_LIST = "input_default_for_list";
	/// <summary>ValidatorXmlオブジェクト</summary>
	private const string CACHE_KEY_VALIDATOR_XML = "validator_xml_file";
	/// <summary>セッションキー：ユーザ拡張項目設定情報</summary>
	private const string SESSIONPARAM_KEY_USEREXTENDSETTINGLIST_INFO = "userextendsettinglist_info";
	/// <summary>シンボルマーク：デフォルト選択済み</summary>
	private const string SYMBOL_DEFAULT_CHECKED = " （●）";
	/// <summary>シンボルマーク：非表示にする</summary>
	private const string SYMBOL_DEFAULT_HIDE = " （×）";
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
			//------------------------------------------------------
			// 初期表示の場合はDBから取得する
			//------------------------------------------------------
			if (Session[SESSIONPARAM_KEY_USEREXTENDSETTINGLIST_INFO] == null)
			{
				this.UserExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
			}
			else
			{
				this.UserExtendSettingList = (UserExtendSettingList)Session[SESSIONPARAM_KEY_USEREXTENDSETTINGLIST_INFO];
				Session[SESSIONPARAM_KEY_USEREXTENDSETTINGLIST_INFO] = null;
			}

			//------------------------------------------------------
			// ValidatorXmlをロード
			//------------------------------------------------------
			this.ValidatorXml = LoadValidatorXmlFile();

			// 翻訳設定情報取得
			this.UserExtendSettingTranslationData = new UserExtendSettingTranslation[0];
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var searchCondition = new NameTranslationSettingSearchCondition
				{
					DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING,
				};
				var translationData = new NameTranslationSettingService().GetTranslationSettingsByDataKbn(searchCondition);
				this.UserExtendSettingTranslationData = (translationData != null)
					? translationData.GroupBy(d => d.MasterId1).Select(g => new UserExtendSettingTranslation(g)).ToArray()
					: new UserExtendSettingTranslation[0];
			}

			// 追加ボタンの状態を制御
			ControlAddButton();

			//------------------------------------------------------
			// 情報のロード＆バインド
			//------------------------------------------------------
			SetDataBind();
		}
		else
		{
			//------------------------------------------------------
			// 初期表示時以外はキャッシュを確認し、あればキャッシュから、なければDBからて取得する
			//------------------------------------------------------
			this.UserExtendSettingList =
				(Cache.Get(CACHE_KEY_USEREXTENDSETTING) == null) ? new UserExtendSettingList(this.LoginOperatorName) : (UserExtendSettingList)Cache.Get(CACHE_KEY_USEREXTENDSETTING);

			//------------------------------------------------------
			// 項目毎の入力方法別初期値のキャッシュを確認し、両方あればキャッシュから、そうで無ければ設定値リストから作成する
			//------------------------------------------------------
			if ((Cache.Get(CACHE_KEY_INPUTDEFAULT_FOR_TEXT) == null) || (Cache.Get(CACHE_KEY_INPUTDEFAULT_FOR_LIST) == null))
			{
				LoadInputDefault();
			}
			else
			{
				this.InputDefaultForText = (List<string>)Cache.Get(CACHE_KEY_INPUTDEFAULT_FOR_TEXT);
				this.InputDefaultForListItem = (List<ListItemCollection>)Cache.Get(CACHE_KEY_INPUTDEFAULT_FOR_LIST);
			}

			//------------------------------------------------------
			// ValidatorXmlのキャシュを確認し、あればキャッシュから、なければValidatorXmlをロード
			//------------------------------------------------------
			this.ValidatorXml =
				(Cache.Get(CACHE_KEY_VALIDATOR_XML) == null) ? LoadValidatorXmlFile() : (ValidatorXml)Cache.Get(CACHE_KEY_VALIDATOR_XML);
		}
	}

	/// <summary>
	/// ページロードコンプリート
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>拡張項目設定のリストはキャッシュしておく</remarks>
	protected void Page_LoadComplete(object sender, EventArgs e)
	{
		Cache.Insert(CACHE_KEY_USEREXTENDSETTING, this.UserExtendSettingList);
		Cache.Insert(CACHE_KEY_INPUTDEFAULT_FOR_TEXT, this.InputDefaultForText);
		Cache.Insert(CACHE_KEY_INPUTDEFAULT_FOR_LIST, this.InputDefaultForListItem);
		Cache.Insert(CACHE_KEY_VALIDATOR_XML, this.ValidatorXml);
	}

	#region イベント
	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 各コントロールから値を取得してリストに反映
		//------------------------------------------------------
		GetInputData();

		//------------------------------------------------------
		// 入力チェック & エラーページ遷移
		//------------------------------------------------------
		string errorMessages = ValidateInputData();
		if (errorMessages != string.Empty) RedirectErrorPage(errorMessages);

		//------------------------------------------------------
		// リストの内容をDBに反映する
		//------------------------------------------------------
		// DB反映完了後にValidatorXml削除するため、対象の設定IDを退避
		List<string> deletedSettingIdList = new List<string>();
		foreach (var deleteData in this.UserExtendSettingList.Items.Where(settingData => settingData.ActionType == UserExtendActionType.Delete))
		{
			deletedSettingIdList.Add(deleteData.SettingId);
		}
		bool result = new UserService().UserExtendSettingExecuteAll(this.UserExtendSettingList);
		if (result)
		{
			// ValidatorXml書き出し
			deletedSettingIdList.ForEach(settingId => this.ValidatorXml.DeleteColumnXmlNode(settingId));
			this.ValidatorXml.DeleteColumnXmlNode(Constants.FLG_USEREXTENDSETTING_PREFIX_KEY);
			this.ValidatorXml.WriteValidatorXml(Constants.PHYSICALDIRPATH_FRONT_PC + Constants.PHYSICALPATH_FILE_XML_USEREXTEND);

			// 各サイトの情報を最新状態にする
			RefreshFileManagerProvider.GetInstance(RefreshFileType.UserExtendSetting).CreateUpdateRefreshFile();

			// DBのデータを取得し、表示に反映
			this.UserExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
			this.MessageHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PARAM_COMPLETED);
		}
		else
		{
			this.MessageHtml = string.Format("<span style='color:Red;'>{0}</span>",
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PARAM_FAIL));
		}

		// 追加ボタンの状態を制御
		ControlAddButton();

		//------------------------------------------------------
		// 情報のロード＆バインド
		//------------------------------------------------------
		SetDataBind();
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		// 追加の前に現在の状態を保存
		GetInputData();

		// ユーザ拡張項目設定リストに空の項目を追加する
		int newSortOrder = (this.UserExtendSettingList.Items.Count > 0) ? (from item in this.UserExtendSettingList.Items select item.SortOrder).Max() + 10 : 10;
		var userExtendSetting = new UserExtendSettingModel
		{
			SettingId = Constants.FLG_USEREXTENDSETTING_PREFIX_KEY,
			SortOrder = newSortOrder,
			LastChanged = LoginOperatorName,
		};
		this.UserExtendSettingList.Items.Add(userExtendSetting);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.UserExtendSettingTranslationData = new UserExtendSettingTranslation[0];
		}

		// 追加ボタンの状態を制御
		ControlAddButton();

		// 情報のロード＆バインド
		SetDataBind();

		// Validatorに空のカラムを追加
		this.ValidatorXml.AddColumn(new ValidatorXmlColumn());
	}

	/// <summary>
	/// 追加ボタンの状態を制御
	/// </summary>
	private void ControlAddButton()
	{
		// 最大件数に達していたら追加ボタンを無効にする
		if (IsLessUserExtendSettingThenMaxCount() == false)
		{
			btnAddTop.Enabled = btnAddBottom.Enabled = false;
			btnAddTop.ToolTip = btnAddBottom.ToolTip = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_USER_EXTEND_SETTING_OVER_CAPACITY_ERROR)
				.Replace("@@ 1 @@", Constants.USEREXTENDSETTING_MAXCOUNT.ToString())
				.Replace("<br />", string.Empty);
		}
		else
		{
			btnAddTop.Enabled = btnAddBottom.Enabled = true;
			btnAddTop.ToolTip = btnAddBottom.ToolTip = string.Empty;
		}
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// Key:Value追加ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInputDefaultKeyValueAdd_ForOther_Click(object sender, EventArgs e)
	{
		// KeyValueを取得(text:表示,value:値)
		var keyValuePair = GetTargetKeyValuePair(sender);

		var isSuccess = PrepareForInsertAndUpdate(sender, keyValuePair);
		if (isSuccess == false) return;

		// 「選択済にする」ONの場合、マーク付与して入力方法に合わせ既存アイテムを再加工
		ChangeDefaultCheckedAll(sender, keyValuePair);
		// 「非表示にする」ONの場合、マーク付与して
		ChangeDefaultHide(sender, keyValuePair);

		// ListBoxに追加
		AddTargetKeyValuePair(sender, keyValuePair);

		// 入力用TextBoxから値を削除する
		ClearTargetKeyValuePair(sender);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// Key:Value更新ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>すでに値が空の項目が存在したら登録しない</remarks>
	protected void btnInputDefaultKeyValueUpdate_ForOther_Click(object sender, EventArgs e)
	{
		// KeyValueを取得(text:表示,value:値)
		var keyValuePair = GetTargetKeyValuePair(sender);

		var isSuccess = PrepareForInsertAndUpdate(sender, keyValuePair);
		if (isSuccess == false) return;

		// 「選択済にする」ONの場合、マーク付与して入力方法に合わせ既存アイテムを再加工
		ChangeDefaultCheckedAll(sender, keyValuePair);
		// 「非表示にする」ONの場合、マーク付与して
		ChangeDefaultHide(sender, keyValuePair);

		// ListBoxを更新
		UpdateTargetKeyValuePair(sender, keyValuePair);

		// 入力用TextBoxから値を削除する
		ClearTargetKeyValuePair(sender);
	}

	/// <summary>
	/// テキストボックス以外の入力準備
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="keyValuePair">入力された表示名と値</param>
	/// <returns>エラーメッセージが存在するかどうか</returns>
	private bool PrepareForInsertAndUpdate(object sender, ListItem keyValuePair)
	{
		// ListBox内が空なら1項目だけ空入力許可
		var errorMessage = CheckForEmptyAndDuplicateListBox(sender, keyValuePair.Value);

		if ((errorMessage == string.Empty)) return true;

		GetLabelOfTarget(sender).Text = errorMessage;
		return false;
	}

	/// <summary>
	/// 値の空と重複チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="value">値</param>
	/// <returns>重複する値が存在するか</returns>
	private string CheckForEmptyAndDuplicateListBox(object sender, string value)
	{
		var errorMessage = string.Empty;

		var listBoxValue = GetListBoxOfTarget(sender).Items.FindByValue(value);
		if (listBoxValue == null) return errorMessage;

		errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_VALUE_DUPLICATION);

		if (value == string.Empty)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_VALUE_EMPTY_DUPLICATION);
		}

		return errorMessage;
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// Key:Value削除ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInputDefaultKeyValueDelete_ForOther_Click(object sender, EventArgs e)
	{
		// KeyValueを削除
		RemoveTargetKeyValuePair(sender);

		// 入力用TextBoxから値を削除する
		ClearTargetKeyValuePair(sender);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// リストボックス内データ選択時テキストボックスへの戻し処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInputDefaultList_ForOther_SelectedIndexChanged(object sender, EventArgs e)
	{
		string text = ((ListBox)sender).SelectedItem.Text;
		((CheckBox)((ListBox)sender).Parent.Parent.FindControl("cbDefault")).Checked = text.Contains(SYMBOL_DEFAULT_CHECKED);
		((CheckBox)((ListBox)sender).Parent.Parent.FindControl("cbHide")).Checked = text.Contains(SYMBOL_DEFAULT_HIDE);
		((TextBox)((ListBox)sender).Parent.Parent.FindControl("tbInputDefaultValue_ForOther")).Text = text.Replace(SYMBOL_DEFAULT_CHECKED, "").Replace(SYMBOL_DEFAULT_HIDE, "");
		((TextBox)((ListBox)sender).Parent.Parent.FindControl("tbInputDefaultKey_ForOther")).Text = ((ListBox)sender).SelectedItem.Value;
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// ListBoxの内容並べ替え処理（実際に並べ替えるのでなく、値を入れ替える）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpDown_OnCommand(object sender, CommandEventArgs e)
	{
		// 対象のリストボックス取得
		string commandArgument = (string)e.CommandArgument;
		ListBox listItems = GetListBoxOfTarget(sender);

		// アイテムが選択されていない場合は処理を行わない
		if (listItems.SelectedIndex == -1) return;

		// アイテムの一番上/下のインデックスを取得し
		// アイテムがない場合 または 選択されたアイテムが一番上/下のアイテムの場合は処理を行わない
		int errorIndex = (commandArgument == "up") ? 0 : listItems.Items.Count - 1;
		if ((listItems.Items.Count == 0) || (listItems.SelectedIndex == errorIndex)) return;

		// 並べ替え
		int targetIndex = (commandArgument == "up") ? listItems.SelectedIndex - 1 : listItems.SelectedIndex + 1;
		string valueTemp = listItems.Items[targetIndex].Value;
		string textTemp = listItems.Items[targetIndex].Text;

		listItems.Items[targetIndex].Value = listItems.SelectedItem.Value;
		listItems.Items[targetIndex].Text = listItems.SelectedItem.Text;

		listItems.Items[listItems.SelectedIndex].Value = valueTemp;
		listItems.Items[listItems.SelectedIndex].Text = textTemp;

		listItems.Items[listItems.SelectedIndex].Selected = false;
		listItems.Items[targetIndex].Selected = true;
	}

	/// <summary>
	/// 入力方法ドロップダウン変更時の処理（入力方法の入力エリア表示制御）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserExtendSettingInputType_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayControlPropertyInputArea((DropDownList)sender);
	}

	/// <summary>
	/// 入力チェック種別ドロップダウン変更時の処理
	/// チェックタイプ選択状態変化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCheckType_SelectedIndexChanged(object sender, EventArgs e)
	{
		DropDownList inputCheckTypeDdl = (DropDownList)sender;

		// ドロップダウンの選択値により入力エリアの表示制御を行う
		switch (inputCheckTypeDdl.SelectedValue)
		{
			case Validator.STRTYPE_FULLWIDTH:
			case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
			case Validator.STRTYPE_FULLWIDTH_KATAKANA:
			case Validator.STRTYPE_HALFWIDTH:
			case Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
				// 文字列長入力エリア表示
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvLengthInputArea").Visible = true;
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvNumberRangeInputArea").Visible = false;
				break;

			case Validator.STRTYPE_HALFWIDTH_NUMBER:
				// 数字範囲入力エリア表示
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvLengthInputArea").Visible = false;
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvNumberRangeInputArea").Visible = true;
				break;

			case Validator.STRTYPE_MAILADDRESS:
			case Validator.STRTYPE_DATE:
			case Validator.STRTYPE_DATE_FUTURE:
			case Validator.STRTYPE_DATE_PAST:
			default:
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvLengthInputArea").Visible = false;
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvNumberRangeInputArea").Visible = false;
				break;
		}
	}

	/// <summary>
	/// 入力チェック種別の固定長チェックボックス変更時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblFixedLength_SelectedIndexChanged(object sender, EventArgs e)
	{
		bool result = (((RadioButtonList)sender).SelectedValue == "1");
		((RadioButtonList)sender).Parent.FindControl("dvFixedLengthInputArea").Visible = result;
		((RadioButtonList)sender).Parent.FindControl("dvMaxMinLengthInputArea").Visible = (result == false);
	}
	#endregion

	#region +メソッド
	/// <summary>
	/// 設定情報のロード＆バインド
	/// </summary>
	private void SetDataBind()
	{
		// 項目毎の入力方法別初期値をロードする
		LoadInputDefault();

		// RadioButtonList、DropDownListの選択状態初期化の為、一旦ここでデータバインド
		this.DataBind();

		// ListControl用にユーザ拡張項目設定をロード
		LoadUserExtendSettingForListControl();
	}

	/// <summary>
	/// 項目毎の入力方法別初期値のロード
	/// </summary>
	private void LoadInputDefault()
	{
		this.InputDefaultForText = new List<string>();
		this.InputDefaultForListItem = new List<ListItemCollection>();

		foreach (var settingData in this.UserExtendSettingList.Items)
		{
			switch (settingData.InputType)
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					// テキストデータを格納
					this.InputDefaultForText.Add(settingData.InputDefault);

					// ListItem用のデータには空を格納
					this.InputDefaultForListItem.Add(new ListItemCollection());
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					// ListItem用のデータを格納
					ListItemCollection inputDefaultForListItem = new ListItemCollection();
					if (settingData.InputDefault != "")
					{
						string[] itemList = settingData.InputDefault.Split(';');
						foreach (string keyValue in itemList)
						{
							string[] splited = keyValue.Split(',');
							string text = splited[1];

							if (splited.Length >= 3)
							{
								text += (splited[2] == "selected") ? SYMBOL_DEFAULT_CHECKED : "";
								text += (splited[2] == "hide") ? SYMBOL_DEFAULT_HIDE : "";

								if (splited.Length == 4)
								{
									text += (splited[3] == "selected") ? SYMBOL_DEFAULT_CHECKED : "";
									text += (splited[3] == "hide") ? SYMBOL_DEFAULT_HIDE : "";
								}
							}

							inputDefaultForListItem.Add(new ListItem(text, splited[0]));
						}
					}
					this.InputDefaultForListItem.Add(inputDefaultForListItem);

					// テキストデータには空を格納
					this.InputDefaultForText.Add("");
					break;

				default:
					break;
			}
		}
	}

	/// <summary>
	/// ユーザ拡張項目設定のロード（リストコントロール用）
	/// </summary>
	/// <remarks>メインのRepeaterコントロールへのデータバインド後にコールしてください</remarks>
	private void LoadUserExtendSettingForListControl()
	{
		foreach (RepeaterItem ri in rUserExtendSettingList.Items)
		{
			// 概要区分
			((RadioButtonList)(ri.FindControl("rblOutlineKbn"))).SelectedValue = this.UserExtendSettingList.Items[ri.ItemIndex].OutlineKbn;
			// 項目表示
			((RadioButtonList)(ri.FindControl("rblInitOnlyType"))).SelectedValue = this.UserExtendSettingList.Items[ri.ItemIndex].InitOnlyFlg;
			// 入力方法
			((DropDownList)(ri.FindControl("ddlUserExtendSettingInputType"))).SelectedValue = this.UserExtendSettingList.Items[ri.ItemIndex].InputType;
			// 文字数有無
			((RadioButtonList)ri.FindControl("rblFixedLength")).SelectedValue = "0";
			// 入力方法の入力エリア表示制御
			DisplayControlPropertyInputArea(((DropDownList)(ri.FindControl("ddlUserExtendSettingInputType"))));

			//------------------------------------------------------
			// 入力チェック系 ※ValidatorXmlから各項目を取得する
			//------------------------------------------------------
			ValidatorXmlColumn validatorXmlColumn = this.ValidatorXml.Columns.Where(column => column.Name == this.UserExtendSettingList.Items[ri.ItemIndex].SettingId).FirstOrDefault();
			if (validatorXmlColumn != null)
			{
				// 必須有無
				((CheckBox)ri.FindControl("cbNecessary")).Checked = (validatorXmlColumn.Necessary == "1");
				// 入力チェック種別
				((DropDownList)(ri.FindControl("ddlCheckType"))).SelectedValue = validatorXmlColumn.Type;
				// 固定長
				bool fixedLength = (validatorXmlColumn.Length != "");
				((RadioButtonList)ri.FindControl("rblFixedLength")).SelectedValue = fixedLength ? "1" : "0";
				((TextBox)ri.FindControl("tbFixedLength")).Text = validatorXmlColumn.Length;
				((TextBox)ri.FindControl("tbLengthMin")).Text = validatorXmlColumn.LengthMin;
				((TextBox)ri.FindControl("tbLengthMax")).Text = validatorXmlColumn.LengthMax;
				// 変動（mix-max）
				((TextBox)ri.FindControl("tbNumMin")).Text = validatorXmlColumn.NumberMin;
				((TextBox)ri.FindControl("tbNumMax")).Text = validatorXmlColumn.NumberMax;

				// 入力方法の入力エリア表示制御
				switch (validatorXmlColumn.Type)
				{
					// 半角数字/数値以外
					case Validator.STRTYPE_FULLWIDTH:
					case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
					case Validator.STRTYPE_FULLWIDTH_KATAKANA:
					case Validator.STRTYPE_HALFWIDTH:
					case Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
						(ri.FindControl("dvLengthInputArea")).Visible = true;
						(ri.FindControl("dvFixedLengthInputArea")).Visible = fixedLength;
						(ri.FindControl("dvMaxMinLengthInputArea")).Visible = (fixedLength == false);
						break;

					// 半角数字/数値
					case Validator.STRTYPE_HALFWIDTH_NUMBER:
						(ri.FindControl("dvNumberRangeInputArea")).Visible = true;
						break;

					case Validator.STRTYPE_MAILADDRESS:
					case Validator.STRTYPE_DATE:
					case Validator.STRTYPE_DATE_FUTURE:
					case Validator.STRTYPE_DATE_PAST:
					default:
						break;
				}
			}
		}
	}

	/// <summary>
	/// ValidatorXmlオブジェクト
	/// </summary>
	/// <remarks>PCサイトのValidatorXmlを優先してロード</remarks>
	/// <returns>ValidatorXmlオブジェクト</returns>
	private ValidatorXml LoadValidatorXmlFile()
	{
		ValidatorXml validatorXml = new ValidatorXml();
		validatorXml = new ValidatorXml(Constants.PHYSICALDIRPATH_FRONT_PC + Constants.PHYSICALPATH_FILE_XML_USEREXTEND, Constants.FILE_NAME_XML_USEREXTEND);

		return validatorXml;
	}

	/// <summary>
	/// 入力方法の入力エリア表示制御
	/// </summary>
	/// <param name="inputTypeDdl">入力方法選択ドロップダウン</param>
	private void DisplayControlPropertyInputArea(DropDownList inputTypeDdl)
	{
		// ドロップダウンの選択値により入力エリアの表示制御を行う
		switch (((DropDownList)inputTypeDdl).SelectedValue)
		{
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForTB").Visible = true;
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForOther").Visible = false;
				break;

			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForTB").Visible = false;
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForOther").Visible = true;
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// 各コントロールからの値取得処理
	/// </summary>
	private void GetInputData()
	{
		// 別タブまたは別ユーザーでユーザー拡張項目数が変わっていた場合エラーを出す
		if (rUserExtendSettingList.Items.Count != this.UserExtendSettingList.Items.Count)
		{
			RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_EXTEND_SETTING_MISMATCH_ERROR));
		}

		foreach (RepeaterItem ri in rUserExtendSettingList.Items)
		{
			// 項目ID
			this.UserExtendSettingList.Items[ri.ItemIndex].SettingId = Constants.FLG_USEREXTENDSETTING_PREFIX_KEY + StringUtility.ToHankaku(StringUtility.ToEmpty(((TextBox)ri.FindControl("tbUserExtendSettingId")).Text.Trim().ToLower()));
			// 項目名
			this.UserExtendSettingList.Items[ri.ItemIndex].SettingName = StringUtility.ToEmpty(((TextBox)ri.FindControl("tbUserExtendSettingName")).Text.Trim());
			// 表示順
			int sortOrder;
			if (int.TryParse(StringUtility.ToHankaku((((TextBox)ri.FindControl("tbSortOrder")).Text.Trim())), out sortOrder) == false)
			{
				sortOrder = 10;
			}
			this.UserExtendSettingList.Items[ri.ItemIndex].SortOrder = sortOrder;
			// 項目説明区分
			this.UserExtendSettingList.Items[ri.ItemIndex].OutlineKbn = StringUtility.ToEmpty(((RadioButtonList)ri.FindControl("rblOutlineKbn")).SelectedValue);
			// 項目説明
			this.UserExtendSettingList.Items[ri.ItemIndex].Outline = StringUtility.ToEmpty(((TextBox)ri.FindControl("tbUserExtendOutline")).Text);
			// 入力方法
			this.UserExtendSettingList.Items[ri.ItemIndex].InputType = StringUtility.ToEmpty(((DropDownList)ri.FindControl("ddlUserExtendSettingInputType")).SelectedValue);
			// 会員登録時のみ区分
			this.UserExtendSettingList.Items[ri.ItemIndex].InitOnlyFlg = StringUtility.ToEmpty(((RadioButtonList)ri.FindControl("rblInitOnlyType")).SelectedValue);
			// 表示サイト区分
			this.UserExtendSettingList.Items[ri.ItemIndex].DisplayKbn = CreateDisplayKbnString(
				((CheckBox)ri.FindControl("cbPcDisplayFlg")).Checked,
				false,
				((CheckBox)ri.FindControl("cbEcDisplayFlg")).Checked,
				((CheckBox)ri.FindControl("cbCsDisplayFlg")).Checked);
			// 削除チェックの状態を取得
			this.UserExtendSettingList.Items[ri.ItemIndex].DeleteFlg = ((CheckBox)ri.FindControl("cbDelete")).Checked;
			// 初期値
			this.UserExtendSettingList.Items[ri.ItemIndex].InputDefault = "";
			switch (((DropDownList)ri.FindControl("ddlUserExtendSettingInputType")).SelectedValue)
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					this.UserExtendSettingList.Items[ri.ItemIndex].InputDefault = StringUtility.ToEmpty(((TextBox)ri.FindControl("tbInputDefault_forTb")).Text.Trim());
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					foreach (ListItem li in ((ListBox)ri.FindControl("lbInputDefaultList_ForOther")).Items)
					{
						this.UserExtendSettingList.Items[ri.ItemIndex].AddInputDefaultForListItem(
							li.Value,
							li.Text.Replace(SYMBOL_DEFAULT_CHECKED, "").Replace(SYMBOL_DEFAULT_HIDE, ""),
							li.Text.Contains(SYMBOL_DEFAULT_CHECKED),
							li.Text.Contains(SYMBOL_DEFAULT_HIDE));
					}
					break;

				default:
					break;
			}

			// バリデータXMLに必須チェックの追加
			// 存在する場合は追加し、存在しない場合はXMLノード追加してから必須チェックを追加する
			if (this.ValidatorXml.FindColumn(this.UserExtendSettingList.Items[ri.ItemIndex].SettingId) != null)
			{
				ValidatorXmlColumn columnValidation = this.ValidatorXml.FindColumn(this.UserExtendSettingList.Items[ri.ItemIndex].SettingId);
				SetValidatorXmlColumn(ri, columnValidation);
			}
			else
			{
				ValidatorXmlColumn columnValidation = new ValidatorXmlColumn(this.UserExtendSettingList.Items[ri.ItemIndex].SettingId);
				SetValidatorXmlColumn(ri, columnValidation);
				this.ValidatorXml.AddColumn(columnValidation);
			}
		}
	}

	/// <summary>
	/// テキストボックスの入力チェック種別に応じてバリデータを設定
	/// </summary>
	/// <param name="Item">リピータアイテム</param>
	/// <param name="columnValidation">バリデータXmlの1ノード分</param>
	/// <remarks></remarks>
	private void SetValidatorXmlColumn(RepeaterItem Item, ValidatorXmlColumn columnValidation)
	{
		columnValidation.DisplayName = this.UserExtendSettingList.Items[Item.ItemIndex].SettingName;
		columnValidation.Necessary = (((CheckBox)Item.FindControl("cbNecessary")).Checked) ? "1" : "";

		string type = "";
		string length = "";
		string lengthMin = "";
		string lengthMax = "";
		string numberMin = "";
		string numberMax = "";

		// 入力方法がテキストボックスの場合
		if (((DropDownList)Item.FindControl("ddlUserExtendSettingInputType")).SelectedValue == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT)
		{
			type = ((DropDownList)Item.FindControl("ddlCheckType")).SelectedValue;
			var tmp = "";
			// 入力チェック種別により格納する値制御
			switch (type)
			{
				case Validator.STRTYPE_FULLWIDTH:
				case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
				case Validator.STRTYPE_FULLWIDTH_KATAKANA:
				case Validator.STRTYPE_HALFWIDTH:
				case Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
					// 固定超フラグがONの場合はLength。OFFならLengthMin-Max。に格納
					bool fixedLength = ((RadioButtonList)Item.FindControl("rblFixedLength")).SelectedValue == "1";
					tmp = StringUtility.ToHankaku(((TextBox)Item.FindControl("tbFixedLength")).Text.Trim());
					length = (fixedLength && Validator.IsHalfwidthNumber(tmp)) ? tmp : "";

					tmp = StringUtility.ToHankaku(((TextBox)Item.FindControl("tbLengthMin")).Text.Trim());
					lengthMin = ((fixedLength == false) && Validator.IsHalfwidthNumber(tmp)) ? tmp : "";

					tmp = StringUtility.ToHankaku(((TextBox)Item.FindControl("tbLengthMax")).Text.Trim());
					lengthMax = ((fixedLength == false) && Validator.IsHalfwidthNumber(tmp)) ? tmp : "";
					break;

				case Validator.STRTYPE_MAILADDRESS:
					lengthMax = "256";
					break;

				case Validator.STRTYPE_HALFWIDTH_NUMBER:
					tmp = StringUtility.ToHankaku(((TextBox)Item.FindControl("tbNumMin")).Text.Trim());
					numberMin = Validator.IsHalfwidthNumber(tmp) ? tmp : "";

					tmp = StringUtility.ToHankaku(((TextBox)Item.FindControl("tbNumMax")).Text.Trim());
					numberMax = Validator.IsHalfwidthNumber(tmp) ? tmp : "";
					break;

				case Validator.STRTYPE_DATE:
				case Validator.STRTYPE_DATE_FUTURE:
				case Validator.STRTYPE_DATE_PAST:
				default:
					break;
			}
		}

		columnValidation.Type = type;
		columnValidation.Length = length;
		columnValidation.LengthMin = lengthMin;
		columnValidation.LengthMax = lengthMax;
		columnValidation.NumberMin = numberMin;
		columnValidation.NumberMax = numberMax;
	}

	/// <summary>
	/// 表示区分文字列生成
	/// </summary>
	/// <param name="pcDisp">表示区分PC</param>
	/// <param name="mbDisp">表示区分モバイル</param>
	/// <param name="ecDisp">表示区分EC管理</param>
	/// <param name="csDisp">表示区分CS管理</param>
	/// <returns>表示区分の文字列</returns>
	/// <remarks>カンマ区切りで作成した文字列はDBにそのまま格納</remarks>
	private string CreateDisplayKbnString(bool pcDisp, bool mbDisp, bool ecDisp, bool csDisp)
	{
		StringBuilder result = new StringBuilder();
		if (pcDisp) result.Append(Constants.FLG_USEREXTENDSETTING_DISPLAY_PC);
		if (mbDisp)
		{
			if (result.Length != 0) result.Append(",");
			result.Append(Constants.FLG_USEREXTENDSETTING_DISPLAY_MB);
		}
		if (ecDisp)
		{
			if (result.Length != 0) result.Append(",");
			result.Append(Constants.FLG_USEREXTENDSETTING_DISPLAY_EC);
		}
		if (csDisp)
		{
			if (result.Length != 0) result.Append(",");
			result.Append(Constants.FLG_USEREXTENDSETTING_DISPLAY_CS);
		}
		return result.ToString();
	}

	/// <summary>
	/// 一括更新前に入力値チェック
	/// </summary>
	private string ValidateInputData()
	{
		StringBuilder errorMessages = new StringBuilder();
		//------------------------------------------------------
		// 追加・更新対象のみ入力チェック
		//------------------------------------------------------
		foreach (var settingData in this.UserExtendSettingList.Items.Where(settingData =>
			(settingData.ActionType == UserExtendActionType.Insert) || (settingData.ActionType == UserExtendActionType.Update)))
		{
			Hashtable input = new Hashtable(settingData.DataSource);
			input[Constants.FIELD_USEREXTENDSETTING_SORT_ORDER] = input[Constants.FIELD_USEREXTENDSETTING_SORT_ORDER].ToString();
			input[Constants.FIELD_USEREXTENDSETTING_INPUT_DEFAULT + ((settingData.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT) ? "_text" : "_list")]
				= input[Constants.FIELD_USEREXTENDSETTING_INPUT_DEFAULT];

			errorMessages.Append(Validator.Validate("UserExtendSetting", input).Replace("@@ 1 @@", settingData.SettingId));
		}

		//------------------------------------------------------
		// 設定IDキーと項目名の重複チェック
		//------------------------------------------------------
		// 追加・更新対象から重複している項目IDと項目名を抽出（追加項目が既存と重複、追加項目同士が重複をまとめてエラーとする）
		var insertUpdateSettings = this.UserExtendSettingList.Items.Where(item =>
			((item.ActionType == UserExtendActionType.Insert) || (item.ActionType == UserExtendActionType.Update)));

		// Check duplication setting id
		foreach (var settingId in insertUpdateSettings.GroupBy(item => item.SettingId).Where(item => item.Count() > 1))
		{
			errorMessages
				.Append(CreateMessageForProject(WebMessages.INPUTCHECK_DUPLICATION, settingId.Key))
				.Append("<br />");
		}

		// Check duplication setting name
		foreach (var settingName in insertUpdateSettings.GroupBy(item => item.SettingName).Where(item => item.Count() > 1))
		{
			errorMessages
				.Append(CreateMessageForProject(WebMessages.INPUTCHECK_DUPLICATION, settingName.Key, true))
				.Append("<br />");
		}

		//------------------------------------------------------
		// 削除対象が、メール配信のターゲット設定で利用されているかチェック
		//------------------------------------------------------
		Dictionary<string, List<string>> usingSettingId = GetUsingTargetListSetting();
		if (usingSettingId.Count > 0)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_EXTEND_SETTING_USED_ID_NOT_DELETE));
			foreach (var key in usingSettingId.Keys)
			{
				errorMessages.Append(CreateMessageForProject(WebMessages.ERRMSG_MANAGER_USER_EXTEND_SETTING_ID_TARGETLIST_USED_ERROR, key));
				usingSettingId[key].ForEach(
					targetId => errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_EXTEND_SETTING_ID_TARGETLIST_USED))
						.Replace("@@ 1 @@", targetId));
			}
		}

		// ユーザー拡張項目設定可能件数を超えて拡張項目を作成していないかチェック(追加・更新対象のみ)
		if (Constants.USEREXTENDSETTING_MAXCOUNT < this.UserExtendSettingList.Items.Count(settingData =>
			((settingData.ActionType == UserExtendActionType.Insert) || (settingData.ActionType == UserExtendActionType.Update))
				&& (IsSystemUsedUserExtendSetting(settingData.SettingId) == false)))
		{
			errorMessages
				.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_EXTEND_SETTING_OVER_CAPACITY_ERROR))
				.Replace("@@ 1 @@", Constants.USEREXTENDSETTING_MAXCOUNT.ToString());
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// Create message for check duplication project name & id
	/// </summary>
	/// <param name="key">A key to get value</param>
	/// <param name="isName">Is project name</param>
	/// <returns>A message</returns>
	private string CreateMessageForProject(string error, string key, bool isName = false)
	{
		var message = WebMessages
			.GetMessages(error)
			.Replace(
				"@@ 1 @@",
			//「項目ID or 項目名」
				string.Format(
					"{0}「{1}」",
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_USER_EXTEND_SETTING_LIST,
						Constants.VALUETEXT_PARAM_INUTCHECK_DUPLICATION,
						isName
							? Constants.VALUETEXT_PARAM_PROJECT_NAME
							: Constants.VALUETEXT_PARAM_PROJECT_ID),
					key));
		return message;
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void RedirectErrorPage(string errorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		Session[SESSIONPARAM_KEY_USEREXTENDSETTINGLIST_INFO] = this.UserExtendSettingList;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// MPターゲットリスト設定関連
	/// ターゲットリスト設定から利用中のユーザー拡張項目を取得する
	/// </summary>
	/// <remarks>
	/// 削除対象の拡張項目IDが、ターゲットリストに設定されている状態かどうかを確認する。
	/// ターゲットリストに設定されている場合には、対象のターゲットIDも分かるようなエラーを表示する。
	/// </remarks>
	/// <returns>ターゲットリスト設定から利用中のユーザー拡張項目一覧</returns>
	private Dictionary<string, List<string>> GetUsingTargetListSetting()
	{
		// ターゲットに設定中のものがあればDictionaryで返却する
		Dictionary<string, List<string>> usingTargetListUserExtendList = new Dictionary<string, List<string>>();
		// メール配信ターゲットリスト設定を全て取得する
		DataView targetList = GetMailSendTargetListAll();
		// 削除対象のユーザー拡張項目ID
		List<string> deleteSettingIdList = GetDeleteActSettingIds();

		/// ターゲットリスト設定がない または 削除対象の拡張項目がない
		if ((targetList.Count == 0) || (deleteSettingIdList.Count == 0)) return usingTargetListUserExtendList;

		foreach (DataRowView dataRowView in targetList.Cast<DataRowView>().Where(item => (string)(item[Constants.FIELD_TARGETLIST_TARGET_CONDITION]) != string.Empty))
		{
			// 削除対象のキーを探す(複数あればループしながら探す)
			foreach (string userExtendSettingId in deleteSettingIdList)
			{
				var result = false;
				foreach (var conditions in TargetListConditionRelationXml.CreateTargetListConditionList((string)dataRowView[Constants.FIELD_TARGETLIST_TARGET_CONDITION]).TargetConditionList)
				{
					if (conditions.IsExistDataField(conditions, userExtendSettingId)) result = true;
				}
				// フィールドに設定されていたら ターゲットリストのIDを記録する
				if (result) // 設定されていれば[True]
				{
					// 利用中のターゲットリストIDをまとめる
					List<string> targetIdList = new List<string>();
					if (usingTargetListUserExtendList.ContainsKey(userExtendSettingId))
					{
						// 既にDictionaryに存在する場合は要素を追加して上書き
						targetIdList = usingTargetListUserExtendList[userExtendSettingId];
						targetIdList.Add((string)dataRowView[Constants.FIELD_TARGETLIST_TARGET_ID]);
						usingTargetListUserExtendList[userExtendSettingId] = targetIdList; // 新しいListで要素を上書き
					}
					else
					{
						// 新規にDictionaryへ登録する
						targetIdList.Add((string)dataRowView[Constants.FIELD_TARGETLIST_TARGET_ID]);
						usingTargetListUserExtendList.Add(userExtendSettingId, targetIdList); // 新しいListを追加
					}
				}
			}
		}

		return usingTargetListUserExtendList;
	}

	/// <summary>
	/// MPターゲットリスト設定関連
	/// ターゲットリスト設定情報を取得(削除フラグONデータは除く)
	/// </summary>
	/// <returns>ターゲットリストデータ</returns>
	private DataView GetMailSendTargetListAll()
	{
		DataView result = new DataView();
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TargetList", "GetTargetListAll"))
		{
			Hashtable input = new Hashtable();
			input[Constants.FIELD_TARGETLIST_DEPT_ID] = this.LoginOperatorShopId;
			result = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}
		return result;
	}

	/// <summary>
	/// MPターゲットリスト設定関連
	/// 削除処理を実施する項目を取得し、SettingIdの一覧を返却
	/// </summary>
	/// <returns>削除対象項目のID一覧</returns>
	private List<string> GetDeleteActSettingIds()
	{
		List<string> result = new List<string>();
		foreach (var userExtendSetting in this.UserExtendSettingList.Items.Where(item => item.ActionType == UserExtendActionType.Delete))
		{
			result.Add(userExtendSetting.SettingId);
		}
		return result;
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// KeyValueを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>KeyValue(表示,値)</returns>
	private ListItem GetTargetKeyValuePair(object sender)
	{
		var text = GetTargetItemTextInput(sender).Text.Trim().Replace(SYMBOL_DEFAULT_CHECKED, string.Empty).Replace(SYMBOL_DEFAULT_HIDE, string.Empty);
		var value = GetTargetItemValueInput(sender).Text.Trim();
		return new ListItem(text, value);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 入力方法を取得
	/// </summary>
	/// <param name="sender"></param>
	private static string GetTargetPropertyInput(object sender)
	{
		return ((DropDownList)((Button)sender).Parent.Parent.FindControl("ddlUserExtendSettingInputType")).SelectedValue;
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// ListBoxに追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="keyValuePair">入力された表示名と値</param>
	private void AddTargetKeyValuePair(object sender, ListItem keyValuePair)
	{
		GetListBoxOfTarget(sender).Items.Add(keyValuePair);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象KeyValueを削除
	/// </summary>
	/// <param name="sender"></param>
	private void RemoveTargetKeyValuePair(object sender)
	{
		GetListBoxOfTarget(sender).Items.Remove(
			GetListBoxOfTarget(sender).SelectedItem);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象KeyValueを更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="keyValuePair">入力された表示名と値</param>
	private void UpdateTargetKeyValuePair(object sender, ListItem keyValuePair)
	{
		ListControl listControl = GetListBoxOfTarget(sender);
		if ((listControl != null) && (listControl.SelectedIndex != -1))
		{
			GetListBoxOfTarget(sender).Items[listControl.SelectedIndex].Text = keyValuePair.Text;
			GetListBoxOfTarget(sender).Items[listControl.SelectedIndex].Value = keyValuePair.Value;
		}
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 入力用TextBoxから値を削除しリストボックス選択を解除
	/// </summary>
	/// <param name="sender"></param>
	private void ClearTargetKeyValuePair(object sender)
	{
		GetListBoxOfTarget(sender).ClearSelection();
		GetTargetItemTextInput(sender).Text = string.Empty;
		GetTargetItemValueInput(sender).Text = string.Empty;
		GetTargetDefaultChecked(sender).Checked = false;
		GetTargetDefaultHide(sender).Checked = false;
		GetLabelOfTarget(sender).Text = string.Empty;
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// デフォルトチェックを変更
	/// </summary>
	/// <param name="sender"></param>
	/// <remarks>
	/// DDL,RADIO：既存のデフォルトをOFFにして、今回の追加分をデフォルトONにする
	/// CHECKBOX：既存のデフォルトはそのままで、今回の追加分をデフォルトON/OFFにする
	/// </remarks>
	private void ChangeDefaultCheckedAll(object sender, ListItem keyValuePair)
	{
		if (GetTargetDefaultChecked(sender).Checked)
		{
			switch (GetTargetPropertyInput(sender))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					foreach (ListItem listItem in GetListBoxOfTarget(sender).Items)
					{
						listItem.Text = listItem.Text.Replace(SYMBOL_DEFAULT_CHECKED, "");
					}
					keyValuePair.Text += SYMBOL_DEFAULT_CHECKED;
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					keyValuePair.Text += SYMBOL_DEFAULT_CHECKED;
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
				default:
					break;
			}
		}
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// デフォルトアイテム非表示を変更
	/// </summary>
	/// <param name="sender"></param>
	private void ChangeDefaultHide(object sender, ListItem keyValuePair)
	{
		if (GetTargetDefaultHide(sender).Checked)
		{
			switch (GetTargetPropertyInput(sender))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					keyValuePair.Text += SYMBOL_DEFAULT_HIDE;
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
				default:
					break;
			}
		}
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象のコントロールを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>デフォルト選択済みのチェックボックス</returns>
	private CheckBox GetTargetDefaultChecked(object sender)
	{
		return ((CheckBox)((Button)sender).Parent.Parent.FindControl("cbDefault"));
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象のコントロールを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>デフォルト非表示のチェックボックス</returns>
	private CheckBox GetTargetDefaultHide(object sender)
	{
		return ((CheckBox)((Button)sender).Parent.Parent.FindControl("cbHide"));
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象のリストボックスコントロールを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>リストボックス</returns>
	private ListBox GetListBoxOfTarget(object sender)
	{
		return ((ListBox)((Button)sender).Parent.Parent.FindControl("lbInputDefaultList_ForOther"));
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象の検証文言表示するラベルコントロールを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>ラベル</returns>
	private Label GetLabelOfTarget(object sender)
	{
		return ((Label)((Button)sender).Parent.Parent.FindControl("lblKeyValueMessage"));
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象の値用テキストボックスコントロールを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>テキストボックス</returns>
	private TextBox GetTargetItemValueInput(object sender)
	{
		return ((TextBox)((Button)sender).Parent.Parent.FindControl("tbInputDefaultKey_ForOther"));
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象の表示用テキストボックスコントロールを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>テキストボックス</returns>
	private TextBox GetTargetItemTextInput(object sender)
	{
		return ((TextBox)((Button)sender).Parent.Parent.FindControl("tbInputDefaultValue_ForOther"));
	}

	/// <summary>
	/// システム利用のユーザー拡張項目か
	/// </summary>
	/// <param name="settingId">ユーザー拡張項目ID</param>
	/// <returns>システム利用のユーザー項目IDか</returns>
	protected bool IsSystemUsedUserExtendSetting(string settingId)
	{
		if (string.IsNullOrEmpty(settingId)) return false;

		var isSystemUsed = Constants.USEREXTENDSETTING_SYSTEM_USED_ITEMS.Contains(settingId);
		return isSystemUsed;
	}

	/// <summary>
	/// システム利用のため削除のみできないユーザー項目IDか
	/// </summary>
	/// <param name="settingId">ユーザー拡張項目ID</param>
	/// <returns>システム利用のため削除のみできないユーザー項目IDか</returns>
	protected bool IsNotDeleteUserExtendSetting(string settingId)
	{
		if (string.IsNullOrEmpty(settingId)) return false;

		var isSystemUsed = Constants.USEREXTENDSETTING_SYSTEM_USED_NOT_DELETE_ITEMS.Contains(settingId);
		return isSystemUsed;
	}

	/// <summary>
	/// システム利用分を除くユーザー拡張項目設定利用数がユーザー利用可能最大件数より少ないか
	/// </summary>
	/// <returns>システム利用分を除くユーザー拡張項目設定利用数がユーザー利用可能最大件数より少ないか</returns>
	protected bool IsLessUserExtendSettingThenMaxCount()
	{
		var isLess = (this.UserExtendSettingItemsCountWithoutSystemUsed < Constants.USEREXTENDSETTING_MAXCOUNT);
		return isLess;
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = new string[0];
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザ拡張項目設定リスト</summary>
	protected UserExtendSettingList UserExtendSettingList { get; set; }
	/// <summary>システム利用分を除くユーザー拡張項目設定利用数</summary>
	protected int UserExtendSettingItemsCountWithoutSystemUsed
	{
		get
		{
			return this.UserExtendSettingList.Items.Count(item =>
				Constants.USEREXTENDSETTING_SYSTEM_USED_ITEMS.Contains(item.SettingId) == false);
		}
	}
	/// <summary>入力方法種別リスト</summary>
	protected ListItemCollection InputCheckTypeList
	{
		get
		{
			ListItemCollection listItemCollection = new ListItemCollection();
			listItemCollection.Add(new ListItem("", ""));
			foreach (ListItem listItem in ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, "validation_type"))
			{
				if ((this.IsShippingCountryAvailableJp == false)
					&& ((listItem.Value == Validator.STRTYPE_FULLWIDTH_HIRAGANA)
						|| (listItem.Value == Validator.STRTYPE_FULLWIDTH_KATAKANA)))
				{
					continue;
				}

				listItemCollection.Add(listItem);
			}
			return listItemCollection;
		}
	}
	/// <summary>入力方法別初期値リスト</summary>
	protected List<ListItemCollection> InputDefaultForListItem { get; set; }
	protected List<string> InputDefaultForText { get; set; }
	/// <summary>ValidatorXmlオブジェクト</summary>
	protected ValidatorXml ValidatorXml { get; set; }
	/// <summary>表示メッセージ</summary>
	protected string MessageHtml { get; private set; }
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; private set; }
	/// <summary>ユーザー拡張項目設定翻訳設定情報</summary>
	protected UserExtendSettingTranslation[] UserExtendSettingTranslationData
	{
		get { return (UserExtendSettingTranslation[])ViewState["userextendsetting_translation_data"]; }
		set { ViewState["userextendsetting_translation_data"] = value; }
	}
	#endregion
}
