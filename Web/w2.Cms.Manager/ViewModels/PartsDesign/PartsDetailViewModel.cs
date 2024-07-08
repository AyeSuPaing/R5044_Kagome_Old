/*
=========================================================================================================
  Module      : パーツ管理 詳細ビューモデル(PartsDetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Common.Util;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.ViewModels.PartsDesign
{
	/// <summary>
	/// パーツ管理 詳細ビューモデル
	/// </summary>
	public class PartsDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// ページ表示 ステータス 列挙
		/// </summary>
		public enum EditPage
		{
			/// <summary>ページ管理内</summary>
			PageDesign,
			/// <summary>パーツ管理内</summary>
			PartsDesign
		}

		/// <summary>
		/// 表示デバイス
		/// </summary>
		public enum DisplayDevice
		{
			/// <summary>PC</summary>
			PC,
			/// <summary>SP</summary>
			SP
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsDetailViewModel()
		{
			this.Input = new PartsDesignPartsInput();
			this.PartsModel = new PartsDesignModel();
			this.PublishItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PAGEDESIGN, Constants.FIELD_PAGEDESIGN_PUBLISH)
				.Select(
					s => new SelectListItem
					{
						Value = s.Value,
						Text = s.Text
					}).ToArray();

			var otherGroupModel = PageDesignUtility.OtherPageGroupModel;
			var otherGroup = new[]
			{
				new SelectListItem
				{
					Value = otherGroupModel.GroupId.ToString(),
					Text = otherGroupModel.GroupName
				}
			};

			var groupList = new PartsDesignService().GetAllGroup().Select(
				g => new SelectListItem
				{
					Value = g.GroupId.ToString(),
					Text = g.GroupName
				}).ToArray();

			this.GroupItems = groupList.Concat(otherGroup).ToArray();

			this.EditPageStatus = EditPage.PartsDesign;
		}

		/// <summary>パーツ詳細 入力内容</summary>
		public PartsDesignPartsInput Input { get; set; }
		public PartsDesignModel PartsModel { get; set; }
		/// <summary>PC実ページ</summary>
		public RealParts PcRealParts { get; set; }
		/// <summary>SP実ページ</summary>
		public RealParts SpRealParts { get; set; }
		/// <summary>公開状態ドロップダウンリスト</summary>
		public SelectListItem[] PublishItems { get; set; }
		/// <summary>グループ ドロップダウンリスト</summary>
		public SelectListItem[] GroupItems { get; set; }
		/// <summary>デフォルト表示デバイス</summary>
		public DisplayDevice DefaultDisplayDevice { get; set; }
		/// <summary>ページ表示 ステータス</summary>
		public EditPage EditPageStatus { get; set; }
		/// <summary>削除許可</summary>
		public bool PermissionDelete
		{
			get
			{
				return ((this.EditPageStatus == EditPage.PartsDesign) 
					&& this.PcRealParts.PermissionDelete 
					&& this.SpRealParts.PermissionDelete);
			}
		}
		/// <summary>複製許可</summary>
		public bool PermissionCopy
		{
			get
			{
				return ((this.EditPageStatus == EditPage.PartsDesign) 
					&& this.PcRealParts.PermissionCopy 
					&& this.SpRealParts.PermissionCopy);
			}
		}
		/// <summary>コード編集許可</summary>
		public bool PermissionCodeEdit
		{
			get
			{
				return (this.PcRealParts.PermissionCodeEdit 
					&& this.SpRealParts.PermissionCodeEdit);
			}
		}
		/// <summary>グループ追加許可</summary>
		public bool PermissionAddGroup
		{
			get
			{
				return (this.EditPageStatus == EditPage.PartsDesign);
			}
		}
	}
}