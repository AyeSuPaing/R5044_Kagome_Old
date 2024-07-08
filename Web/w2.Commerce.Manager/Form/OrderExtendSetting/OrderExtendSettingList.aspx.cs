/*
=========================================================================================================
  Module      : 注文拡張項目設定ページ処理(OrderExtendSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common.RefreshFileManager;
using w2.Common.Util;
using w2.Domain.OrderExtendSetting;

/// <summary>
/// 注文拡張項目設定ページ処理
/// </summary>
public partial class Form_OrderExtendSetting_OrderExtendSettingList : BasePage
{
	#region 定数
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
			this.OrderExtendSettingModel = new OrderExtendSettingService().GetAll()
				.Select(m => new OrderExtendSettingInput(m)).ToArray();

			SetDataBind();
		}
	}

	#region イベント
	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		GetInputData();
		var errorList = this.OrderExtendSettingModel.Select(i => i.Validate()).ToArray();
		if (errorList.Any(es => (string.IsNullOrEmpty(es) == false)))
		{
			RedirectErrorPage(string.Join("<br />", errorList));
		}

		var updateModels = this.OrderExtendSettingModel.Select(i => i.CreateModel());
		var result = new OrderExtendSettingService().Update(updateModels);
		if (result > 0)
		{
			RefreshFileManagerProvider.GetInstance(RefreshFileType.OrderExtendSetting).CreateUpdateRefreshFile();

			this.OrderExtendSettingModel = new OrderExtendSettingService().GetAll()
				.Select(m => new OrderExtendSettingInput(m)).ToArray();
			this.MessageHtml = "一括更新が正常に終了しました。";
		}
		else
		{
			this.MessageHtml = "<span style='color:Red;'>一括更新が正常に終了しませんでした。</span>";
		}

		SetDataBind();
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

		// ListBox内１要素の入力チェック
		var errorMessage = CheckListBoxItemInput(sender, keyValuePair);
		// 値重複チェック(表示は許可,値は非許可)
		if (errorMessage == "")
		{
			errorMessage = CheckListBoxItemAdd(sender, keyValuePair.Value);
		}

		if (errorMessage != "")
		{
			GetLabelOfTarget(sender).Text = errorMessage;
			return;
		}

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
	/// <remarks>値の空入力はアイテム追加しない</remarks>
	protected void btnInputDefaultKeyValueUpdate_ForOther_Click(object sender, EventArgs e)
	{
		// KeyValueを取得(text:表示,value:値)
		var keyValuePair = GetTargetKeyValuePair(sender);

		// ListBox内１要素の入力チェック
		var errorMessage = CheckListBoxItemInput(sender, keyValuePair);
		// 値重複チェック(表示は許可,値は非許可)
		if (errorMessage == "")
		{
			errorMessage = CheckListBoxItemUpdate(sender, keyValuePair.Value);
		}

		if (errorMessage != "")
		{
			GetLabelOfTarget(sender).Text = errorMessage;
			return;
		}

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
		var text = ((ListBox)sender).SelectedItem.Text;
		((CheckBox)((ListBox)sender).Parent.Parent.FindControl("cbDefault")).Checked =
			text.Contains(SYMBOL_DEFAULT_CHECKED);
		((CheckBox)((ListBox)sender).Parent.Parent.FindControl("cbHide")).Checked = text.Contains(SYMBOL_DEFAULT_HIDE);
		((TextBox)((ListBox)sender).Parent.Parent.FindControl("tbInputDefaultValue_ForOther")).Text =
			text.Replace(SYMBOL_DEFAULT_CHECKED, "").Replace(SYMBOL_DEFAULT_HIDE, "");
		((TextBox)((ListBox)sender).Parent.Parent.FindControl("tbInputDefaultKey_ForOther")).Text =
			((ListBox)sender).SelectedItem.Value;
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
		var commandArgument = (string)e.CommandArgument;
		var listItems = GetListBoxOfTarget(sender);

		// アイテムが選択されていない場合は処理を行わない
		if (listItems.SelectedIndex == -1) return;

		// アイテムの一番上/下のインデックスを取得し
		// アイテムがない場合 または 選択されたアイテムが一番上/下のアイテムの場合は処理を行わない
		var errorIndex = (commandArgument == "up") ? 0 : listItems.Items.Count - 1;
		if ((listItems.Items.Count == 0) || (listItems.SelectedIndex == errorIndex)) return;

		// 並べ替え
		var targetIndex = (commandArgument == "up") ? listItems.SelectedIndex - 1 : listItems.SelectedIndex + 1;
		var valueTemp = listItems.Items[targetIndex].Value;
		var textTemp = listItems.Items[targetIndex].Text;

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
	protected void ddlOrderExtendSettingInputType_SelectedIndexChanged(object sender, EventArgs e)
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
		var inputCheckTypeDdl = (DropDownList)sender;

		// ドロップダウンの選択値により入力エリアの表示制御を行う
		switch (inputCheckTypeDdl.SelectedValue)
		{
			case Validator.STRTYPE_FULLWIDTH:
			case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
			case Validator.STRTYPE_FULLWIDTH_KATAKANA:
			case Validator.STRTYPE_HALFWIDTH:
			case Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
				// 文字列長入力エリア表示
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvLengthInputArea")
					.Visible = true;
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvNumberRangeInputArea")
					.Visible = false;
				break;

			case Validator.STRTYPE_HALFWIDTH_NUMBER:
				// 数字範囲入力エリア表示
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvLengthInputArea")
					.Visible = false;
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvNumberRangeInputArea")
					.Visible = true;
				break;

			case Validator.STRTYPE_MAILADDRESS:
			case Validator.STRTYPE_DATE:
			case Validator.STRTYPE_DATE_FUTURE:
			case Validator.STRTYPE_DATE_PAST:
			default:
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvLengthInputArea")
					.Visible = false;
				((RepeaterItem)((DropDownList)inputCheckTypeDdl).Parent.Parent).FindControl("dvNumberRangeInputArea")
					.Visible = false;
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
		var result = (((RadioButtonList)sender).SelectedValue == "1");
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
		DataBind();
		// ListControl用にユーザ拡張項目設定をロード
		LoadOrderExtendSettingForListControl();
	}

	/// <summary>
	/// 項目毎の入力方法別初期値のロード
	/// </summary>
	private void LoadInputDefault()
	{
		this.InputDefaultForText = new List<string>();
		this.InputDefaultForListItem = new List<ListItemCollection>();
		foreach (var settingData in this.OrderExtendSettingModel)
		{
			switch (settingData.InputType)
			{
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
					// テキストデータを格納
					this.InputDefaultForText.Add(settingData.InputDefault);

					// ListItem用のデータには空を格納
					this.InputDefaultForListItem.Add(new ListItemCollection());
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
					// ListItem用のデータを格納
					var inputDefaultForListItem = new ListItemCollection();
					if (settingData.InputDefault != "")
					{
						var itemList = settingData.InputDefault.Split(';');
						foreach (var keyValue in itemList)
						{
							var splited = keyValue.Split(',');
							var text = splited[1];

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
	private void LoadOrderExtendSettingForListControl()
	{
		foreach (RepeaterItem ri in rOrderExtendSettingList.Items)
		{
			// 概要区分
			((RadioButtonList)(ri.FindControl("rblOutlineKbn"))).SelectedValue =
				this.OrderExtendSettingModel[ri.ItemIndex].OutlineKbn;
			// 項目表示
			((RadioButtonList)(ri.FindControl("rblInitOnlyType"))).SelectedValue =
				this.OrderExtendSettingModel[ri.ItemIndex].InitOnlyFlg;
			// 入力方法
			((DropDownList)(ri.FindControl("ddlOrderExtendSettingInputType"))).SelectedValue =
				this.OrderExtendSettingModel[ri.ItemIndex].InputType;
			// 文字数有無
			((RadioButtonList)ri.FindControl("rblFixedLength")).SelectedValue = "0";
			// 入力方法の入力エリア表示制御
			DisplayControlPropertyInputArea(((DropDownList)(ri.FindControl("ddlOrderExtendSettingInputType"))));


			ValidatorXmlColumn validatorXmlColumn = null;
			if (string.IsNullOrEmpty(this.OrderExtendSettingModel[ri.ItemIndex].ValidatorXml) == false)
			{
				var doc = new XmlDocument();
				doc.LoadXml(this.OrderExtendSettingModel[ri.ItemIndex].ValidatorXml);
				var newNode = (XmlNode)doc.DocumentElement;
				validatorXmlColumn = new ValidatorXmlColumn(newNode);
			}

			if ((string.IsNullOrEmpty(this.OrderExtendSettingModel[ri.ItemIndex].ValidatorXml) == false)
				&& (validatorXmlColumn != null))
			{
				// 必須有無
				((CheckBox)ri.FindControl("cbNecessary")).Checked = (validatorXmlColumn.Necessary == "1");
				// 入力チェック種別
				((DropDownList)(ri.FindControl("ddlCheckType"))).SelectedValue = validatorXmlColumn.Type;
				// 固定長
				var fixedLength = (validatorXmlColumn.Length != "");
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
	/// 入力方法の入力エリア表示制御
	/// </summary>
	/// <param name="inputTypeDdl">入力方法選択ドロップダウン</param>
	private void DisplayControlPropertyInputArea(DropDownList inputTypeDdl)
	{
		// ドロップダウンの選択値により入力エリアの表示制御を行う
		switch (((DropDownList)inputTypeDdl).SelectedValue)
		{
			case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForTB").Visible =
					true;
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForOther").Visible =
					false;
				break;

			case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
			case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
			case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForTB").Visible =
					false;
				((RepeaterItem)((DropDownList)inputTypeDdl).Parent).FindControl("dvInputPropertyAreaForOther").Visible =
					true;
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
		this.OrderExtendSettingModel = new OrderExtendSettingService().GetAll()
			.Select(m => new OrderExtendSettingInput(m)).ToArray();
		foreach (RepeaterItem ri in rOrderExtendSettingList.Items)
		{
			// 項目名
			this.OrderExtendSettingModel[ri.ItemIndex].SettingName =
				StringUtility.ToEmpty(((TextBox)ri.FindControl("tbOrderExtendSettingName")).Text.Trim());
			// 表示順
			this.OrderExtendSettingModel[ri.ItemIndex].SortOrder =
				StringUtility.ToEmpty(((TextBox)ri.FindControl("tbSortOrder")).Text.Trim());
			;
			// 項目説明区分
			this.OrderExtendSettingModel[ri.ItemIndex].OutlineKbn =
				StringUtility.ToEmpty(((RadioButtonList)ri.FindControl("rblOutlineKbn")).SelectedValue);
			// 項目説明
			this.OrderExtendSettingModel[ri.ItemIndex].Outline =
				StringUtility.ToEmpty(((TextBox)ri.FindControl("tbOrderExtendOutline")).Text);
			// 入力方法
			this.OrderExtendSettingModel[ri.ItemIndex].InputType = StringUtility.ToEmpty(
				((DropDownList)ri.FindControl("ddlOrderExtendSettingInputType")).SelectedValue);
			// 会員登録時のみ区分
			this.OrderExtendSettingModel[ri.ItemIndex].InitOnlyFlg = StringUtility.ToEmpty(
				((RadioButtonList)ri.FindControl("rblInitOnlyType")).SelectedValue);
			// 表示サイト区分
			this.OrderExtendSettingModel[ri.ItemIndex].DisplayKbn = CreateDisplayKbnString(
				((CheckBox)ri.FindControl("cbPcDisplayFlg")).Checked,
				((CheckBox)ri.FindControl("cbEcDisplayFlg")).Checked);
			// 初期値
			this.OrderExtendSettingModel[ri.ItemIndex].InputDefault = "";
			switch (((DropDownList)ri.FindControl("ddlOrderExtendSettingInputType")).SelectedValue)
			{
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
					this.OrderExtendSettingModel[ri.ItemIndex].InputDefault =
						StringUtility.ToEmpty(((TextBox)ri.FindControl("tbInputDefault_forTb")).Text.Trim());
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
					foreach (ListItem li in ((ListBox)ri.FindControl("lbInputDefaultList_ForOther")).Items)
					{
						this.OrderExtendSettingModel[ri.ItemIndex].AddInputDefaultForListItem(
							li.Value,
							li.Text.Replace(SYMBOL_DEFAULT_CHECKED, "").Replace(SYMBOL_DEFAULT_HIDE, ""),
							li.Text.Contains(SYMBOL_DEFAULT_CHECKED),
							li.Text.Contains(SYMBOL_DEFAULT_HIDE));
					}

					break;

				default:
					break;
			}

			var columnValidation = new ValidatorXmlColumn(this.OrderExtendSettingModel[ri.ItemIndex].SettingId);
			SetValidatorXmlColumn(ri, columnValidation);
			this.OrderExtendSettingModel[ri.ItemIndex].ValidatorXml = columnValidation.CreateColumnXml().InnerXml;
		}
	}

	/// <summary>
	/// テキストボックスの入力チェック種別に応じてバリデータを設定
	/// </summary>
	/// <param name="Item">リピータアイテム</param>
	/// <param name="columnValidation">バリデータXmlの1ノード分</param>
	/// <remarks>バリデーション内容</remarks>
	private void SetValidatorXmlColumn(RepeaterItem Item, ValidatorXmlColumn columnValidation)
	{
		columnValidation.DisplayName = this.OrderExtendSettingModel[Item.ItemIndex].SettingName;
		columnValidation.Necessary = (((CheckBox)Item.FindControl("cbNecessary")).Checked) ? "1" : "";

		var type = "";
		var length = "";
		var lengthMin = "";
		var lengthMax = "";
		var numberMin = "";
		var numberMax = "";

		// 入力方法がテキストボックスの場合
		if (((DropDownList)Item.FindControl("ddlOrderExtendSettingInputType")).SelectedValue
			== Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT)
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
	/// <param name="ecDisp">表示区分EC管理</param>
	/// <returns>表示区分の文字列</returns>
	/// <remarks>カンマ区切りで作成した文字列はDBにそのまま格納</remarks>
	private string CreateDisplayKbnString(bool pcDisp, bool ecDisp)
	{
		StringBuilder result = new StringBuilder();
		if (pcDisp) result.Append(Constants.FLG_ORDEREXTENDSETTING_DISPLAY_PC);

		if (ecDisp)
		{
			if (result.Length != 0) result.Append(",");
			result.Append(Constants.FLG_ORDEREXTENDSETTING_DISPLAY_EC);
		}

		return result.ToString();
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void RedirectErrorPage(string errorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// KeyValueを取得
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>KeyValue(表示,値)</returns>
	private ListItem GetTargetKeyValuePair(object sender)
	{
		var text = GetTargetItemTextInput(sender).Text.Trim().Replace(SYMBOL_DEFAULT_CHECKED, "")
			.Replace(SYMBOL_DEFAULT_HIDE, "");
		var value = GetTargetItemValueInput(sender).Text.Trim();
		return new ListItem(text, value);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 入力方法を取得
	/// </summary>
	/// <param name="sender"></param>
	private string GetTargetPropertyInput(object sender)
	{
		return ((DropDownList)((Button)sender).Parent.Parent.FindControl("ddlOrderExtendSettingInputType"))
			.SelectedValue;
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// ListBoxに追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="keyValuePair">入力内容</param>
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
		GetListBoxOfTarget(sender).Items.Remove(GetListBoxOfTarget(sender).SelectedItem);
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// 対象KeyValueを更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="keyValuePair">入力内容</param>
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
		GetTargetItemTextInput(sender).Text = "";
		GetTargetItemValueInput(sender).Text = "";
		GetTargetDefaultChecked(sender).Checked = false;
		GetTargetDefaultHide(sender).Checked = false;
		GetLabelOfTarget(sender).Text = "";
	}

	/// <summary>
	/// テキストボックス以外の入力方法編集エリア用
	/// デフォルトチェックを変更
	/// </summary>
	/// <param name="sender"></param>
	///  <param name="keyValuePair">入力内容</param>
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
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
					foreach (ListItem listItem in GetListBoxOfTarget(sender).Items)
					{
						listItem.Text = listItem.Text.Replace(SYMBOL_DEFAULT_CHECKED, "");
					}

					keyValuePair.Text += SYMBOL_DEFAULT_CHECKED;
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					keyValuePair.Text += SYMBOL_DEFAULT_CHECKED;
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
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
	/// <param name="keyValuePair">入力内容</param>
	private void ChangeDefaultHide(object sender, ListItem keyValuePair)
	{
		if (GetTargetDefaultHide(sender).Checked)
		{
			switch (GetTargetPropertyInput(sender))
			{
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					keyValuePair.Text += SYMBOL_DEFAULT_HIDE;
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
				default:
					break;
			}
		}
	}

	/// <summary>
	/// リストボックスの追加/更新する要素の入力チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="keyValuePair">入力内容</param>
	/// <returns>true:正常 false:検証エラー</returns>
	private string CheckListBoxItemInput(object sender, ListItem keyValuePair)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT + "_value", keyValuePair.Text);
		input.Add(Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT + "_key", keyValuePair.Value);

		return Validator.Validate("OrderExtendSetting", input);
	}

	/// <summary>
	/// リストボックス要素の入力チェック（追加時）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="value">値</param>
	/// <returns>エラー文言</returns>
	private string CheckListBoxItemAdd(object sender, string value)
	{
		if (GetListBoxOfTarget(sender).Items.FindByValue(value) != null)
		{
			return "<br />値が重複しています。";
		}

		return "";
	}

	/// <summary>
	/// リストボックス要素の入力チェック（更新時）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="value">値</param>
	/// <returns>エラー文言</returns>
	private string CheckListBoxItemUpdate(object sender, string value)
	{
		foreach (ListItem item in GetListBoxOfTarget(sender).Items)
		{
			if ((item.Selected == false) && (item.Value == value))
			{
				return "<br />値が重複しています。";
			}
		}

		return "";
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
	#endregion

	#region プロパティ
	/// <summary>ユーザ拡張項目設定リスト</summary>
	protected OrderExtendSettingInput[] OrderExtendSettingModel { get; set; }
	/// <summary>入力方法種別リスト</summary>
	protected ListItemCollection InputCheckTypeList
	{
		get
		{
			ListItemCollection listItemCollection = new ListItemCollection();
			listItemCollection.Add(new ListItem("", ""));
			foreach (ListItem listItem in ValueText.GetValueItemList(
				Constants.TABLE_ORDEREXTENDSETTING,
				"validation_type"))
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
	/// <summary>表示メッセージ</summary>
	protected string MessageHtml { get; private set; }
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; private set; }
	#endregion
}