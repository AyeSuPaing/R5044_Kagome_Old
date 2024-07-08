/*
=========================================================================================================
  Module      : ABテスト Listビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.AbTest;

namespace w2.Cms.Manager.ViewModels.AbTest
{
	/// <summary>
	/// ABテスト Listビューモデル
	/// </summary>
	[Serializable]
	public class AbTestListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AbTestListViewModel()
		{
			this.ParamModel = new AbTestListParamModel();
			this.Items = new AbTestListItemDetailViewModel[0];
			this.OpenDetailAbTestId = "";
			this.ItemCount = 0;
			this.CanAbTestRegister = true;
			this.CanEditShortUrl = true;
		}

		/// <summary>パラメタモデル</summary>
		public AbTestListParamModel ParamModel { get; set; }
		/// <summary>ABテストリスト詳細ビュー</summary>
		public AbTestListItemDetailViewModel[] Items { get; set; }
		/// <summary>詳細表示ABテストID</summary>
		public string OpenDetailAbTestId { get; set; }
		/// <summary>ABテスト件数</summary>
		public int ItemCount { get; set; }
		/// <summary>ABテスト新規作成可能か</summary>
		public bool CanAbTestRegister { get; set; }
		/// <summary>ショートURL機能利用可能か</summary>
		public bool CanEditShortUrl { get; set; }

		/// <summary>
		/// ABテストリスト詳細ビュー
		/// </summary>
		[Serializable]
		public class AbTestListItemDetailViewModel : ViewModelBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AbTestListItemDetailViewModel()
			{
				this.PublicStatus = Constants.FLG_ABTEST_PUBLISH_PRIVATE;
				this.Items = new[] { new AbTestListItem() };
			}

			/// <summary>ABテストID</summary>
			public string AbTestId { get; set; }
			/// <summary>ABテストタイトル</summary>
			public string AbTestTitle { get; set; }
			/// <summary>公開状態</summary>
			public string PublicStatus { get; set; }
			/// <summary>ビューカウント</summary>
			public long ViewCount { get; set; }
			/// <summary>更新日(日付)</summary>
			public string DateChanged1 { get; set; }
			/// <summary>更新日(時間)</summary>
			public string DateChanged2 { get; set; }
			/// <summary>ABテストリストアイテム</summary>
			public AbTestListItem[] Items { get; set; }
		}

		/// <summary>
		/// ABテストリストアイテムビュー
		/// </summary>
		[Serializable]
		public class AbTestListItem : ViewModelBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AbTestListItem()
			{
			}

			/// <summary>ページID</summary>
			public string PageId { get; set; }
			/// <summary>ページタイトル</summary>
			public string PageTitle { get; set; }
			/// <summary>振り分け比率</summary>
			public int DistributionRate { get; set; }
		}
	}
}