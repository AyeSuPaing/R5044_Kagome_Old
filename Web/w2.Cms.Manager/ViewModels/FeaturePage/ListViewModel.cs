/*
=========================================================================================================
  Module      : 特集ページリストビューモデル (ListViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Design;
using w2.App.Common.Util;

namespace w2.Cms.Manager.ViewModels.FeaturePage
{
	/// <summary>
	/// 特集ページリストビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ListViewModel()
		{
			this.ListPageViewModels = new List<PageViewModel>();
			this.HitCount = 0;
		}

		/// <summary>PC 実ページリスト</summary>
		public List<RealPage> PcRealPageList { get; set; }
		/// <summary>SP 実ページリスト</summary>
		public List<RealPage> SpRealPageList { get; set; }
		/// <summary>特集ページリスト</summary>
		public List<PageViewModel> ListPageViewModels { get; set; }
		/// <summary>検索ヒット数</summary>
		public int HitCount { get; set; }
	}

	/// <summary>
	/// 特集ページ ビュー用モデル
	/// </summary>
	[Serializable]
	public class PageViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageViewModel()
			: base()
		{
			this.PcRealPage = new RealPage();
			this.SpRealPage = new RealPage();
			this.Publish = Constants.FLG_FEATUREPAGE_PUBLISH_PUBLIC;
			this.UseType = Constants.FLG_FEATUREPAGE_USE_TYPE_ALL;
		}

		/// <summary>ページタイプ</summary>
		public string PageType
		{
			get
			{
				return this.PcRealPage.PageType;
			}
		}
		/// <summary>更新日付文字列</summary>
		public string UpdateDateStr
		{
			get
			{
				return DateTimeUtility.ToStringForManager(
					this.UpdateDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			}
		}
		/// <summary>更新日付</summary>
		public DateTime UpdateDate
		{
			get
			{
				return (this.UsePc) ? this.PcRealPage.UpdateDate : this.SpRealPage.UpdateDate;
			}
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get
			{
				return this.PcRealPage.FileName;
			}
		}
		/// <summary>PC ページディレクトリパス</summary>
		public string PcDirPath
		{
			get { return this.PcRealPage.PageDirPath; }
		}
		/// <summary>ページID</summary>
		public long PageId { get; set; }
		/// <summary>公開状態</summary>
		public string Publish { get; set; }
		/// <summary>デバイス利用状況</summary>
		public string UseType { get; set; }
		/// <summary>PC利用</summary>
		public bool UsePc
		{
			get { return (string.IsNullOrEmpty(this.UseType) || this.UseType.Contains(Constants.FLG_FEATUREPAGE_USE_TYPE_PC)); }
		}
		/// <summary>SP利用</summary>
		public bool UseSp
		{
			get { return (string.IsNullOrEmpty(this.UseType) || this.UseType.Contains(Constants.FLG_FEATUREPAGE_USE_TYPE_SP)); }
		}
		/// <summary>ページ順序</summary>
		public int PageSortNumber { get; set; }
		/// <summary>PC実ページ</summary>
		public RealPage PcRealPage { get; set; }
		/// <summary>SP実ページ</summary>
		public RealPage SpRealPage { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>公開開始日</summary>
		public DateTime? PublishDateFrom { get; set; }
		/// <summary>公開終了日</summary>
		public DateTime? PublishDateTo { get; set; }
		/// <summary>公開範囲 設定有無</summary>
		public bool IsSettingReleaseRange { get; set; }
		/// <summary>ビューカウント</summary>
		public long ViewCount { get; set; }
		/// <summary>CVカウント</summary>
		public long CvCount { get; set; }
		/// <summary>CV額</summary>
		public decimal CvPrice { get; set; }
		/// <summary>ブランドID</summary>
		public string BrandIds { get; set; }
		/// <summary>ブランドIDリスト</summary>
		public string[] BrandIdList { get; set; }
		/// <summary>最上位カテゴリ</summary>
		public string RootCateogryId { get; set; }
	}
}
