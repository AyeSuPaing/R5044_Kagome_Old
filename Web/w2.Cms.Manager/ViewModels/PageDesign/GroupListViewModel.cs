/*
=========================================================================================================
  Module      : ページ管理 グループ一覧ビューモデル(GroupListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Design;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.PageDesign;

namespace w2.Cms.Manager.ViewModels.PageDesign
{
	/// <summary>
	///  ページ管理 グループ一覧ビューモデル
	/// </summary>
	public class GroupListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupListViewModel()
		{
			this.GroupPageViewModels = new List<GroupViewModel>();
			this.ErrorMessage = string.Empty;
		}

		/// <summary>グループ別 ページリスト</summary>
		public List<GroupViewModel> GroupPageViewModels { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// グループ ビュー用モデル
	/// </summary>
	public class GroupViewModel : PageDesignGroupModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupViewModel()
		{
			this.ListPageViewModels = new List<PageViewModel>();
			this.GroupId = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">グループモデル</param>
		public GroupViewModel(PageDesignGroupModel model)
			: base(model.DataSource)
		{
			this.ListPageViewModels = new List<PageViewModel>();
		}

		/// <summary>
		/// グループ内のページ一覧
		/// </summary>
		public List<PageViewModel> ListPageViewModels { get; set; }
	}

	/// <summary>
	/// ページ ビュー用モデル
	/// </summary>
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
			this.Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PUBLIC;
			this.UseType = Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP;
			this.MetadataDesc = string.Empty;
		}
		/// <summary>ページID</summary>
		public long PageId { get; set; }
		/// <summary>公開状態</summary>
		public string Publish { get; set; }
		/// <summary>グループID</summary>
		public long GroupId { get; set; }
		/// <summary>デバイス利用状況</summary>
		public string UseType { get; set; }
		/// <summary>ページ順序</summary>
		public int PageSortNumber { get; set; }
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc { get; set; }
		/// <summary>PC実ページ</summary>
		public RealPage PcRealPage { get; set; }
		/// <summary>SP実ページ</summary>
		public RealPage SpRealPage { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>公開範囲 設定有無</summary>
		public bool IsSettingReleaseRange { get; set; }
		/// <summary>ページタイプ</summary>
		public string PageType
		{
			get
			{
				return this.PcRealPage.PageType;
			}
		}
		/// <summary>カスタムページか</summary>
		public bool IsCustomPage
		{
			get { return this.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM; }
		}
		/// <summary>ページタイプ 表示テキスト</summary>
		public string PageTypeText
		{
			get
			{
				string  text =string.Empty;

				switch(this.PageType)
				{
					case Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM:
						text = ValueText.GetValueText(
							Constants.TABLE_PAGEDESIGN,
							Constants.FIELD_PAGEDESIGN_PAGE_TYPE,
							Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM);
						break;

					case Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL:
						text = ValueText.GetValueText(
							Constants.TABLE_PAGEDESIGN,
							Constants.FIELD_PAGEDESIGN_PAGE_TYPE,
							Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL);
						break;

					case Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML:
						text = ValueText.GetValueText(
							Constants.TABLE_PAGEDESIGN,
							Constants.FIELD_PAGEDESIGN_PAGE_TYPE,
							Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML);
						break;
				}

				return text;
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
		/// <summary>更新日時</summary>
		public DateTime DateChanged { get; set; }
		/// <summary>更新日時を文字列化したもの</summary>
		public string DateChangedConvertedToString
		{
			get
			{
				return DateTimeUtility.ToStringForManager(
					this.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			}
		}
		/// <summary>PC利用</summary>
		public bool UsePc
		{
			get
			{
				return (((this.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC)
						&& (this.PcRealPage.Existence == RealPage.ExistStatus.Exist))
					|| ((this.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP)
						&& (this.PcRealPage.Existence == RealPage.ExistStatus.Exist)));
			}
		}
		/// <summary>SP利用</summary>
		public bool UseSp
		{
			get
			{
				return (((this.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP)
						&& (this.SpRealPage.Existence == RealPage.ExistStatus.Exist))
					|| ((this.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP)
						&& (this.SpRealPage.Existence == RealPage.ExistStatus.Exist)));
			}
		}
	}
}
