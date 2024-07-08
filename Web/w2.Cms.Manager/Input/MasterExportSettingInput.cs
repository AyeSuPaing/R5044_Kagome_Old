/*
=========================================================================================================
  Module      : マスターダウンロード設定入力クラス(MasterExportSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// マスタ種別入力クラス
	/// </summary>
	[Serializable]
	public class MasterExportSettingInput
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MasterExportSettingInput()
		{
			this.Fields = "";
			this.MasterKbn = new MasterKbnInput();
			this.ExportFileTypeSelectedValue = "";
			this.SettingName = "";
			this.SelectSettingIndex = 0;
			this.SelectSettingValue = "";
		}
		#endregion

		/// <summary>フィールド</summary>
		public string Fields { get; set; }
		/// <summary>マスタ種別</summary>
		public MasterKbnInput MasterKbn { get; set; }
		/// <summary>出力ファイル種類設定</summary>
		public string ExportFileTypeSelectedValue { get; set; }
		/// <summary>出力設定の名前</summary>
		public string SettingName { get; set; }
		/// <summary>出力設定</summary>
		public string SelectSettingValue { get; set; }
		/// <summary>出力設定のインデックス</summary>
		public int SelectSettingIndex { get; set; }
		/// <summary>登録か</summary>
		public bool IsInsert { get; set; }


		/// <summary>
		/// マスタ種別入力クラス
		/// </summary>
		[Serializable]
		public class MasterKbnInput
		{
			/// <summary>マスタ種別テキスト</summary>
			public string Text { get; set; }
			/// <summary>マスタ種別値</summary>
			public string Value { get; set; }
		}
	}
}