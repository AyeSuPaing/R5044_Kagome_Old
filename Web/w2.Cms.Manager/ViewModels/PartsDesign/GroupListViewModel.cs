/*
=========================================================================================================
  Module      : パーツ管理 グループ一覧ビューモデル(GroupListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Design;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.ViewModels.PartsDesign
{
	/// <summary>
	///  パーツ管理 グループ一覧ビューモデル
	/// </summary>
	public class GroupListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupListViewModel()
		{
			this.GroupPartsViewModels = new List<GroupViewModel>();
			this.ErrorMessage = string.Empty;
		}

		/// <summary>グループ別 パーツリスト</summary>
		public List<GroupViewModel> GroupPartsViewModels { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// グループ ビュー用モデル
	/// </summary>
	public class GroupViewModel : PartsDesignGroupModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupViewModel()
		{
			this.ListPartsViewModels = new List<PartsViewModel>();
			this.GroupId = 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">グループモデル</param>
		public GroupViewModel(PartsDesignGroupModel model) : base(model.DataSource)
		{
			this.ListPartsViewModels = new List<PartsViewModel>();
		}

		/// <summary>
		/// グループ内 パーツ一覧
		/// </summary>
		public List<PartsViewModel> ListPartsViewModels { get; set; }
	}

	/// <summary>
	/// ページ ビュー用モデル
	/// </summary>
	public class PartsViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsViewModel()
		{
			this.PcRealParts = new RealParts();
			this.SpRealParts = new RealParts();
			this.Publish = Constants.FLG_PARTSDESIGN_PUBLISH_PUBLIC;
			this.UseType = Constants.FLG_PARTSDESIGN_USE_TYPE_PC_SP;
		}

		/// <summary>パーツID</summary>
		public long PartsId { get; set; }
		/// <summary>公開状態</summary>
		public string Publish { get; set; }
		/// <summary>グループID</summary>
		public long GroupId { get; set; }
		/// <summary>デバイス利用状況</summary>
		public string UseType { get; set; }
		/// <summary>ページ順序</summary>
		public int PartsSortNumber { get; set; }
		/// <summary>PC実パーツ</summary>
		public RealParts PcRealParts { get; set; }
		/// <summary>SP実パーツ</summary>
		public RealParts SpRealParts { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>公開範囲 設定有無</summary>
		public bool IsSettingReleaseRange { get; set; }
		/// <summary>パーツタイプ</summary>
		public string PartsType
		{
			get { return this.PcRealParts.PageType; }
		}
		/// <summary>カスタムページか</summary>
		public bool IsCustomParts
		{
			get { return this.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM; }
		}
		/// <summary>ページタイプ 表示テキスト</summary>
		public string PartsTypeText
		{
			get
			{
				return (this.IsCustomParts)
					? ValueText.GetValueText(
						Constants.TABLE_PARTSDESIGN,
						Constants.FIELD_PARTSDESIGN_PARTS_TYPE,
						Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
					: ValueText.GetValueText(
						Constants.TABLE_PARTSDESIGN,
						Constants.FIELD_PARTSDESIGN_PARTS_TYPE,
						Constants.FLG_PARTSDESIGN_PARTS_TYPE_NORMAL);
			}
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get { return this.PcRealParts.FileName; }
		}
		/// <summary>PC ページディレクトリパス</summary>
		public string PcDirPath
		{
			get { return this.PcRealParts.PageDirPath; }
		}
		/// <summary>更新日付</summary>
		public string UpdateDate
		{
			get
			{
				return DateTimeUtility.ToStringForManager(
					(this.UsePc) ? this.PcRealParts.UpdateDate : this.SpRealParts.UpdateDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			}
		}
		/// <summary>PC利用</summary>
		public bool UsePc
		{
			get
			{
				return (((this.UseType == Constants.FLG_PARTSDESIGN_USE_TYPE_PC)
						&& (this.PcRealParts.Existence == RealPage.ExistStatus.Exist))
					|| ((this.UseType == Constants.FLG_PARTSDESIGN_USE_TYPE_PC_SP)
						&& (this.PcRealParts.Existence == RealPage.ExistStatus.Exist)));
			}
		}
		/// <summary>SP利用</summary>
		public bool UseSp
		{
			get
			{
				return (((this.UseType == Constants.FLG_PARTSDESIGN_USE_TYPE_SP)
						&& (this.SpRealParts.Existence == RealPage.ExistStatus.Exist))
					|| ((this.UseType == Constants.FLG_PARTSDESIGN_USE_TYPE_PC_SP)
						&& (this.SpRealParts.Existence == RealPage.ExistStatus.Exist)));
			}
		}
		/// <summary>削除許可</summary>
		public bool PermissionDelete
		{
			get
			{
				return (this.PcRealParts.PermissionDelete
					&& this.SpRealParts.PermissionDelete);
			}
		}
	}
}