/*
=========================================================================================================
  Module      : 商品初期設定フィールド格納クラス(ProductDefaultSettingField.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.ProductDefaultSetting
{
	/// <summary>
	/// 商品初期設定フィールド格納クラス
	/// </summary>
	public class ProductDefaultSettingField
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <param name="comment">項目メモ</param>
		/// <param name="display">項目表示有無</param>
		public ProductDefaultSettingField(
			string fieldName,
			string defaultValue,
			string comment,
			bool display)
		{
			this.Name = fieldName;
			this.Default = defaultValue;
			this.Comment = comment;
			this.Display = display;
		}

		/// <summary>フィールド名</summary>
		public string Name { get; set; }
		/// <summary>デフォルト値</summary>
		public string Default { get; set; }
		/// <summary>項目メモを取得する</summary>
		public string Comment { get; set; }
		/// <summary>項目表示有無</summary>
		public bool Display { get; set; }
	}
}
