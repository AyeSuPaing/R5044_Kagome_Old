/*
=========================================================================================================
  Module      : ABテストアイテムビューモデル(AbTestItemViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.Cms.Manager.ViewModels.AbTest
{
	/// <summary>
	/// ABテスト アイテムビューモデル
	/// </summary>
	[Serializable]
	public class AbTestItemViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AbTestItemViewModel()
		{
			this.AbTestId = "";
			this.Items = new Item[] { };
			this.ItemTemplate = new Item();
		}

		/// <summary>ABテストID</summary>
		public string AbTestId { get; set; }
		/// <summary>アイテム</summary>
		public IEnumerable<Item> Items { get; set; }
		/// <summary>アイテムテンプレート (新規に追加する際に利用)</summary>
		public Item ItemTemplate { get; set; }

		/// <summary>
		/// ABテスト アイテム
		/// </summary>
		public class Item
		{
			public Item()
			{
				this.PageId = "";
				this.PageTitle = "";
				this.PageUrl = "";
				this.DistributionRate = 0;
			}

			/// <summary>ページID</summary>
			public string PageId { get; set; }
			/// <summary>ページタイトル</summary>
			public string PageTitle { get; set; }
			/// <summary>ページURL</summary>
			public string PageUrl { get; set; }
			/// <summary>作成日</summary>
			public string DateCreated { get; set; }
			/// <summary>振り分け比率</summary>
			public int DistributionRate { get; set; }
		}
	}
}