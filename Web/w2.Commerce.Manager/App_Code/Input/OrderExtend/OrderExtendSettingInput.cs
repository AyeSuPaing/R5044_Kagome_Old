/*
=========================================================================================================
  Module      : 注文拡張項目設定入力クラス (OrderExtendSettingInput.cs)
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
using w2.App.Common.Input;
using w2.Domain.OrderExtendSetting;

/// <summary>
/// 注文拡張項目設定マスタ入力クラス
/// </summary>
[Serializable]
public class OrderExtendSettingInput : InputBase<OrderExtendSettingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public OrderExtendSettingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public OrderExtendSettingInput(OrderExtendSettingModel model) : this()
	{
		this.SettingId = model.SettingId;
		this.SettingName = model.SettingName;
		this.OutlineKbn = model.OutlineKbn;
		this.Outline = model.Outline;
		this.SortOrder = model.SortOrder.ToString();
		this.InputType = model.InputType;
		this.InputDefault = model.InputDefault;
		this.InitOnlyFlg = model.InitOnlyFlg;
		this.DisplayKbn = model.DisplayKbn;
		this.ValidatorXml = model.Validator;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	/// <summary>
	/// バリデーション
	/// </summary>
	/// <returns>エラー内容</returns>
	public string Validate()
	{
		var input = new Hashtable(this.DataSource);
		input[Constants.FIELD_ORDEREXTENDSETTING_SORT_ORDER] = input[Constants.FIELD_ORDEREXTENDSETTING_SORT_ORDER].ToString();
		input[Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT + ((this.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT) ? "_text" : "_list")]
			= input[Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT];

		var result = Validator.Validate("OrderExtendSetting", input).Replace("@@ 1 @@", this.SettingId);
		return result;
	}

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override OrderExtendSettingModel CreateModel()
	{
		var model = new OrderExtendSettingModel
		{
			SettingId = this.SettingId,
			SettingName = this.SettingName,
			OutlineKbn = this.OutlineKbn,
			Outline = this.Outline,
			SortOrder = int.Parse(this.SortOrder),
			InputType = this.InputType,
			InputDefault = this.InputDefault,
			InitOnlyFlg = this.InitOnlyFlg,
			Validator = this.ValidatorXml,
			DisplayKbn = this.DisplayKbn,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// リストコントロール用の入力値の追加処理
	/// </summary>
	/// <param name="key">キーとなる文字列</param>
	/// <param name="value">値となる文字列</param>
	/// <param name="defaultSelected">デフォルトで選択済みにするか？</param>
	/// <param name="defaultHide">リストの表示する際、非表示にするか？</param>
	public void AddInputDefaultForListItem(string key, string value, bool defaultSelected, bool defaultHide)
	{
		var keyValueString = new StringBuilder();
		// 既に何か設定されていれば区切り文字を先頭に追加
		if (this.InputDefault != "") keyValueString.Append(this.InputDefault).Append(";");

		keyValueString.Append(key).Append(",").Append(value);
		if (defaultSelected) keyValueString.Append(",").Append("selected");
		if (defaultHide) keyValueString.Append(",").Append("hide");

		this.InputDefault = keyValueString.ToString();
	}

	/// <summary>
	/// 初期値情報を取得 ※プロパティからしか呼ばない
	/// </summary>
	/// <param name="hasSelected">初期値情報を選択or入力済みで取得するか</param>
	/// <param name="isFront">Frontサイトからの呼び出しか</param>
	/// <remarks>
	/// ・デフォルト選択済にする場合、アイテムのselected有効にする
	/// 　ただし、DDL/RADIOの場合にデフォルト選択が複数ある場合はリスト中の最初に付いているアイテムのみにする
	/// ・管理サイトで表示する場合は、「デフォルト選択にする」や「非表示にする」設定は考慮しない
	/// </remarks>
	/// <returns>テキスト：string テキスト以外：ListItemリスト</returns>
	private object GetListItemDefault(bool hasSelected, bool isFront = true)
	{
		//------------------------------------------------------
		// 入力方法がテキストの場合
		//------------------------------------------------------
		if (this.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT)
		{
			return (isFront && hasSelected) ? this.InputDefault : "";
		}

		//------------------------------------------------------
		// 入力方法がテキスト以外の場合
		//------------------------------------------------------
		var listItem = new List<ListItem>();
		if (this.InputDefault == "") return listItem;

		var singleSelected = false;
		foreach (var keyValue in (this.InputDefault).Split(';'))
		{
			var elements = keyValue.Split(',');
			var li = new ListItem(elements[1], elements[0]);

			//------------------------------------------------------
			// 「デフォルト選択済み」の設定
			//------------------------------------------------------
			// フロントサイトからの呼び出し かつ デフォルト選択済みにする
			if (isFront && hasSelected)
			{
				switch (this.InputType)
				{
					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
						if (singleSelected == false)
							li.Selected = singleSelected = CheckKeyExists(elements, "selected");
						break;

					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
						li.Selected = CheckKeyExists(elements, "selected");
						break;

					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
					default:
						break;
				}
			}

			//------------------------------------------------------
			// 「リストアイテムに表示」の設定
			//------------------------------------------------------
			// フロントサイトからの呼び出しではない または 「非表示にする」設定がなければ表示用リストへ追加
			if ((isFront == false) || (CheckKeyExists(elements, "hide") == false))
			{
				listItem.Add(li);
			}
		}

		return listItem;
	}

	/// <summary>
	/// 「選択済みにする」「非表示にする」キー設定が存在するかチェック
	/// </summary>
	/// <param name="elements">リストアイテム内の1要素</param>
	/// <param name="key">キー文字列</param>
	/// <returns>true：存在する</returns>
	private bool CheckKeyExists(string[] elements, string key)
	{
		var result = false;
		if (elements.Length >= 3)
		{
			// 3 or 4要素目をチェックしキー（selected,hide）が存在すればTrue
			result = ((elements[2] == key) || ((elements.Length == 4) && (elements[3] == key)));
		}

		return result;
	}

	/// <summary>
	/// KeyValuePairで項目リストを返却
	/// </summary>
	/// <returns>項目リスト</returns>
	public List<KeyValuePair<string, string>> GetKeyValuePair()
	{
		var result = this.InputDefault.Split(';').Select(keyValue => keyValue.Split(','))
			.Select(item => new KeyValuePair<string, string>(item[0], item[1])).ToList();
		return result;
	}
	#endregion

	#region プロパティ
	/// <summary>リストボックス内容をListItemのリストで取得 ※入力方法がテキストの場合stringで返す</summary>
	public object ListItem
	{
		get { return GetListItemDefault(false); }
	}
	/// <summary>ユーザ拡張項目ID</summary>
	public string SettingId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_ID]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_ID] = value; }
	}
	/// <summary>名称</summary>
	public string SettingName
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_NAME]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_NAME] = value; }
	}
	/// <summary>概要表示区分</summary>
	public string OutlineKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE_KBN]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE_KBN] = value; }
	}
	/// <summary>概要</summary>
	public string Outline
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE] = value; }
	}
	/// <summary>並び順</summary>
	public string SortOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SORT_ORDER]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SORT_ORDER] = value; }
	}
	/// <summary>入力種別</summary>
	public string InputType
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_TYPE]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_TYPE] = value; }
	}
	/// <summary>初期値</summary>
	public string InputDefault
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT] = value; }
	}
	/// <summary>登録時のみフラグ</summary>
	public string InitOnlyFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INIT_ONLY_FLG]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INIT_ONLY_FLG] = value; }
	}
	/// <summary>表示区分</summary>
	public string DisplayKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DISPLAY_KBN]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DISPLAY_KBN] = value; }
	}
	/// <summary>バリデータテキスト</summary>
	public string ValidatorXml
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_VALIDATOR]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_VALIDATOR] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_LAST_CHANGED] = value; }
	}
	#endregion
}