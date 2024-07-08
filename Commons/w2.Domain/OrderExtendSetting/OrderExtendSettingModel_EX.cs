/*
=========================================================================================================
  Module      : 注文拡張項目設定マスタモデル (OrderExtendSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;

namespace w2.Domain.OrderExtendSetting
{
	/// <summary>
	/// 注文拡張項目設定マスタモデル
	/// </summary>
	public partial class OrderExtendSettingModel
	{
		#region メソッド
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
		/// KeyValuePairで項目リストを返却
		/// </summary>
		/// <returns>項目リスト</returns>
		public List<KeyValuePair<string, string>> GetKeyValuePair()
		{
			var keyVals = new List<KeyValuePair<string, string>>();
			foreach (var keyValue in this.InputDefault.Split(';'))
			{
				var item = keyValue.Split(',');
				keyVals.Add(new KeyValuePair<string, string>(item[0], item[1])); // key,value になるように格納
			}

			return keyVals;
		}
		#endregion

		#region プロパティ
		/// <summary>概要 HTML内容</summary>
		public string OutlineHtmlEncode { get; set; }
		/// <summary>テキストボックス入力か</summary>
		public bool IsInputTypeText
		{
			get { return (this.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT); }
		}
		/// <summary>ドロップダウン入力か</summary>
		public bool IsInputTypeDropDown
		{
			get { return (this.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN); }
		}
		/// <summary>チェックボックス入力か</summary>
		public bool IsInputTypeCheckBox
		{
			get { return (this.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX); }
		}
		/// <summary>ラジオボタン入力か</summary>
		public bool IsInputTypeRadio
		{
			get { return (this.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO); }
		}
		/// <summary>Front 登録可能か</summary>
		public bool CanUseRegister
		{
			get
			{
				return ((this.InitOnlyFlg == Constants.FLG_ORDEREXTENDSETTING_INITONLY)
					|| (this.InitOnlyFlg == Constants.FLG_ORDEREXTENDSETTING_UPDATABLE));
			}
		}
		/// <summary>Front 変更可能か</summary>
		public bool CanUseModify
		{
			get { return (this.InitOnlyFlg == Constants.FLG_ORDEREXTENDSETTING_UPDATABLE); }
		}
		/// <summary>PC表示可能か</summary>
		public bool CanUseFront
		{
			get { return (this.DisplayKbn.Contains(Constants.FLG_ORDEREXTENDSETTING_DISPLAY_PC)); }
		}
		/// <summary>管理画面表示可能か</summary>
		public bool CanUseEc
		{
			get { return (this.DisplayKbn.Contains(Constants.FLG_ORDEREXTENDSETTING_DISPLAY_EC)); }
		}
		/// <summary>バリデーション内容</summary>
		public ValidatorXmlColumn ValidatorXmlColumn { get; set; }
		/// <summary>必須項目か</summary>
		public bool IsNeecessary
		{
			get { return (this.ValidatorXmlColumn.Necessary == "1"); }
		}
		#endregion
	}
}