/*
=========================================================================================================
  Module      : ユーザ拡張項目設定マスタモデル (UserExtendSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using w2.Domain.User;

namespace w2.Domain.UserExtendSetting
{
	/// <summary>ユーザ拡張項目設定のSQL実行タイプ</summary>
	public enum UserExtendActionType
	{
		/// <summary>なにもしない</summary>
		None,
		/// <summary>追加</summary>
		Insert,
		/// <summary>更新</summary>
		Update,
		/// <summary>削除</summary>
		Delete
	}

	/// <summary>
	/// ユーザ拡張項目設定マスタモデル
	/// </summary>
	public partial class UserExtendSettingModel
	{
		#region メソッド
		/// <summary>
		/// 登録状態更新
		/// </summary>
		public void UpdateRegistrationStatus()
		{
			var userService = new UserService();
			// DBの状態を参照し、IsRegistedの値を更新
			var model = userService.GetUserExtendSetting(this.SettingId);
			this.IsRegisted = (model != null);
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
			StringBuilder keyValueString = new StringBuilder();
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
		/// ・管理サイトで表示する場合は、「非表示にする」設定は考慮しない
		/// </remarks>
		/// <returns>テキスト：string テキスト以外：ListItemリスト</returns>
		private object GetListItemDefault(bool hasSelected, bool isFront = true)
		{
			//------------------------------------------------------
			// 入力方法がテキストの場合
			//------------------------------------------------------
			if (this.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT)
			{
				return (hasSelected) ? this.InputDefault : "";
			}

			//------------------------------------------------------
			// 入力方法がテキスト以外の場合
			//------------------------------------------------------
			List<ListItem> listItem = new List<ListItem>();
			if (this.InputDefault == "") return listItem;

			bool singleSelected = false;
			foreach (string keyValue in (this.InputDefault).Split(';'))
			{
				string[] elements = keyValue.Split(',');
				ListItem li = new ListItem(elements[1], elements[0]);

				//------------------------------------------------------
				// 「デフォルト選択済み」の設定
				//------------------------------------------------------
				if (hasSelected)
				{
					switch (this.InputType)
					{
						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
							if (singleSelected == false) li.Selected = singleSelected = CheckKeyExists(elements, "selected");
							break;

						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
							li.Selected = CheckKeyExists(elements, "selected");
							break;

						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
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
			bool result = false;
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
			List<KeyValuePair<string, string>> keyVals = new List<KeyValuePair<string, string>>();
			foreach (string keyValue in this.InputDefault.Split(';'))
			{
				string[] item = keyValue.Split(',');
				keyVals.Add(new KeyValuePair<string, string>(item[0], item[1])); // key,value になるように格納
			}
			return keyVals;
		}
		#endregion

		#region プロパティ
		/// <summary>登録済みフラグ</summary>
		public bool IsRegisted { get; internal set; }
		/// <summary>削除フラグ</summary>
		public bool DeleteFlg { get; set; }
		/// <summary>実行タイプ</summary>
		public UserExtendActionType ActionType
		{
			get
			{
				// 未登録項目に対して「削除フラグ ON」や「IDが未入力」の場合はなにもしない
				UserExtendActionType actionType;

				if (this.IsRegisted)
				{
					// 登録済み項目 かつ 削除対象フラグON ならばDelete そうでなければUpdate
					actionType = (this.DeleteFlg) ? UserExtendActionType.Delete : UserExtendActionType.Update;
				}
				else
				{
					// 未登録項目 かつ 「拡張項目IDの入力有り ならば Insert 」そうでなければNone
					actionType = ((this.SettingId != "") && (this.SettingId != Constants.FLG_USEREXTENDSETTING_PREFIX_KEY))
						? UserExtendActionType.Insert
						: UserExtendActionType.None;
				}

				return actionType;
			}
		}
		/// <summary>リストボックス内容をListItemのリストで取得 ※入力方法がテキストの場合stringで返す</summary>
		public object ListItem { get { return GetListItemDefault(false); } }
		/// <summary>(管理用)リストボックス内容をListItemのリストで取得(ユーザー登録時) ※入力方法がテキストの場合stringで返す</summary>
		public object ListItemForManagerInsert { get { return GetListItemDefault(true, false); } }
		/// <summary>(管理用)リストボックス内容をListItemのリストで取得(ユーザー編集時) ※入力方法がテキストの場合stringで返す</summary>
		public object ListItemForManagerUpdate { get { return GetListItemDefault(false, false); } }
		/// <summary>リストボックス内容をListItemのリストでかつデフォルト選択状態で取得 ※入力方法がテキストの場合stringでかつデフォルト文言返す</summary>
		public object ListItemDefaultSelected { get { return GetListItemDefault(true); } }
		/// <summary>ユーザ拡張項目設定翻訳前設定情報</summary>
		public UserExtendSettingBeforeTranslationModel BeforeTranslationData
		{
			get { return (UserExtendSettingBeforeTranslationModel)this.DataSource["before_translation_data"]; }
			set { this.DataSource["before_translation_data"] = value; }
		}
		/// <summary>クロスポイントアプリ会員か</summary>
		public bool IsDiaplayableForShopAppMember
		{
			get { return this.SettingId == Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG; }
		}
		#endregion
	}

	/// <summary>
	/// ユーザ拡張項目設定翻訳前設定情報モデルクラス
	/// </summary>
	/// <remarks>グローバルOP：ON時、表示名称の切り替えに使用</remarks>
	[Serializable]
	public class UserExtendSettingBeforeTranslationModel : ModelBase<UserExtendSettingBeforeTranslationModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserExtendSettingBeforeTranslationModel(){}
		#endregion

		#region プロパティ
		/// <summary>名称</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_NAME] = value; }
		}
		/// <summary>ユーザ拡張項目概要表示区分</summary>
		public string OutlineKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN] = value; }
		}
		/// <summary>ユーザ拡張項目概要</summary>
		public string Outline
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE] = value; }
		}
		#endregion
	}
}
