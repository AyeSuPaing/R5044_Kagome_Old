/*
=========================================================================================================
  Module      : オプションカテゴリクラス (OptionCategory.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace OptionAppeal
{
	/// <summary>
	/// オプションカテゴリクラス
	/// </summary>
	[Serializable]
	public class OptionCategory
	{
		/// <summary>オプション一覧</summary>
		private const string OPTIONAPPEAL_CATEGORY_OPTION_LIST = "option_list";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OptionCategory()
		{
			this.CategoryId = string.Empty;
			this.CategoryName = string.Empty;
			this.CategoryParent = string.Empty;
			this.CategoryIcon = string.Empty;
			this.Options = new List<OptionItem>();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="categoryParent">親カテゴリ</param>
		/// <param name="categoryIcon">カテゴリアイコン</param>
		/// <param name="optionList">オプション一覧</param>
		public OptionCategory(
			string categoryId,
			string categoryName,
			string categoryParent,
			string categoryIcon,
			List<OptionItem> optionList)
		{
			this.CategoryId = categoryId;
			this.CategoryName = categoryName;
			this.CategoryParent = categoryParent;
			this.CategoryIcon = categoryIcon;
			this.Options = optionList;
		}

		/// <summary>カテゴリーID</summary>
		public string CategoryId { get; set; }
		/// <summary>カテゴリー名</summary>
		public string CategoryName { get; set; }
		/// <summary>親カテゴリー</summary>
		public string CategoryParent { get; set; }
		/// <summary>カテゴリーアイコン</summary>
		public string CategoryIcon { get; set; }
		/// <summary>oオプション一覧</summary>
		public List<OptionItem> Options { get; set; }
	}
}
