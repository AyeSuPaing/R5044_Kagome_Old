/*
=========================================================================================================
  Module      : LP Listビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.ParamModels.LandingPaeg;
using w2.Domain.LandingPage;

namespace w2.Cms.Manager.ViewModels.LandingPage
{
	/// <summary>
	/// LP Listビューモデル
	/// </summary>
	[Serializable]
	public class LandingPageListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageListViewModel()
		{
			this.ParamModel = new LandingPageListParamModel();
			this.Items = new LandingPageListItemDetailViewModel[0];
			this.OpenDetailPageId = "";
			this.ItemCount = 0;
			this.CanLpRegister = true;
			this.CanEditShortUrl = true;
			this.HitCount = 0;
		}

		/// <summary>パラメタモデル</summary>
		public LandingPageListParamModel ParamModel { get; set; }
		/// <summary>Lpリスト詳細ビュー</summary>
		public LandingPageListItemDetailViewModel[] Items { get; set; }
		/// <summary>詳細表示ページID</summary>
		public string OpenDetailPageId { get; set; }
		/// <summary>Lp件数</summary>
		public int ItemCount { get; set; }
		/// <summary>Lp新規作成可能か</summary>
		public bool CanLpRegister { get; set; }
		/// <summary>ショートURL機能利用可能か</summary>
		public bool CanEditShortUrl { get; set; }
		/// <summary>検索ヒット数</summary>
		public int HitCount { get; set; }

		/// <summary>
		/// Lpリスト詳細ビュー
		/// </summary>
		[Serializable]
		public class LandingPageListItemDetailViewModel : ViewModelBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public LandingPageListItemDetailViewModel()
			{
				this.PublicStatus = LandingPageConst.PUBLIC_STATUS_UNPUBLISHED;
				this.UsePublicRange = false;
				this.ProductSet = new[] { new LandingPageListProductSet() };
			}

			/// <summary>ページID</summary>
			public string PageId { get; set; }
			/// <summary>タイトル</summary>
			public string PageTitle { get; set; }
			/// <summary>管理用タイトル</summary>
			public string ManagementTitle { get; set; }
			/// <summary>URL</summary>
			public string PageUrl { get; set; }
			/// <summary>更新日(日付)</summary>
			public string DateChanged1 { get; set; }
			/// <summary>更新日(時間)</summary>
			public string DateChanged2 { get; set; }
			/// <summary>ビューカウント</summary>
			public long ViewCount { get; set; }
			/// <summary>CVカウント</summary>
			public long CvCount { get; set; }
			/// <summary>CV額</summary>
			public decimal CvPrice { get; set; }
			/// <summary>公開状態</summary>
			public string PublicStatus { get; set; }
			/// <summary>デザインモード</summary>
			public string DesignMode { get; set; }
			/// <summary>公開範囲ありなし</summary>
			public bool UsePublicRange { get; set; }
			/// <summary>PCサムネイルURL</summary>
			public string ThumbnailUrlPc { get; set; }
			/// <summary>SPサムネイルURL</summary>
			public string ThumbnailUrlSp { get; set; }
			/// <summary>Lpリスト商品</summary>
			public LandingPageListProductSet[] ProductSet { get; set; }
		}

		/// <summary>
		/// Lpリスト商品セットビュー
		/// </summary>
		[Serializable]
		public class LandingPageListProductSet : ViewModelBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public LandingPageListProductSet()
			{
				this.Products = new LandingPageListProduct[0];
			}
			/// <summary>Lpリスト商品</summary>
			public LandingPageListProduct[] Products { get; set; }
			/// <summary>商品件数</summary>
			public int ItemCount
			{
				get { return this.Products.Length; }
			}
		}

		/// <summary>
		/// Lpリスト商品ビュー
		/// </summary>
		[Serializable]
		public class LandingPageListProduct : ViewModelBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public LandingPageListProduct()
			{
				this.ItemImageUrl = "";
			}

			/// <summary>画像URL</summary>
			public string ItemImageUrl { get; set; }
			/// <summary>数量</summary>
			public int Quantity { get; set; }
			/// <summary>商品名</summary>
			public string ProductName { get; set; }
		}
	}
}
