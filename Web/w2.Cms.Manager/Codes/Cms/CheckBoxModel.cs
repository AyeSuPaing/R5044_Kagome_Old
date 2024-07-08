/*
=========================================================================================================
  Module      : チェックボックスモデル(CheckBoxModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.Codes.Cms
{
	/// <summary>
	/// チェックボックスモデル
	/// </summary>
	public class CheckBoxModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CheckBoxModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">項目名</param>
		/// <param name="value">内容</param>
		/// <param name="isSelected">チェック状況</param>
		public CheckBoxModel(string name, string value, bool isSelected)
		{
			this.Name = name;
			this.Value = value;
			this.IsSelected = isSelected;
		}

		/// <summary>項目名</summary>
		public string Name { get; set; }
		/// <summary>内容</summary>
		public string Value { get; set; }
		/// <summary>チェック状況</summary>
		public bool IsSelected { get; set; }
	}
}