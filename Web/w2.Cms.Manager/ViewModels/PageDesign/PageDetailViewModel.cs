/*
=========================================================================================================
  Module      :  ページ管理 詳細 ビューモデル(PageDetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.PageDesign;

namespace w2.Cms.Manager.ViewModels.PageDesign
{
	/// <summary>
	/// ページ管理 詳細 ビューモデル
	/// </summary>
	public class PageDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// ページ管理 詳細 ビューモデル
		/// </summary>
		public PageDetailViewModel()
		{
			this.Input = new PageDesignPageInput();
			this.PcDirPath = string.Empty;

			this.PublishItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PAGEDESIGN, Constants.FIELD_PAGEDESIGN_PUBLISH)
				.Select(s => new SelectListItem
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

			var groupList = new PageDesignService().GetAllGroup().Select(
				g => new SelectListItem
				{
					Value = g.GroupId.ToString(),
					Text = g.GroupName
				}).ToArray();

			this.GroupItems = groupList.Concat(otherGroup).ToArray();
			this.LayoutEditViewModelPc = new LayoutEditViewModel("Input.PcContentInput.LayoutEditInput");
			this.LayoutEditViewModelSp = new LayoutEditViewModel("Input.SpContentInput.LayoutEditInput");
		}

		/// <summary>ページ詳細 入力内容</summary>
		public PageDesignPageInput Input { get; set; }
		/// <summary>PC実ページ</summary>
		public RealPage PcRealPage { get; set; }
		/// <summary>SP実ページ</summary>
		public RealPage SpRealPage { get; set; }
		/// <summary>PCディレクトリパス</summary>
		public string PcDirPath { get; set; }
		/// <summary>公開状態ドロップダウンリスト</summary>
		public SelectListItem[] PublishItems { get; set; }
		/// <summary>グループ ドロップダウンリスト</summary>
		public SelectListItem[] GroupItems { get; set; }
		/// <summary>レイアウト編集PC 入力内容</summary>
		public LayoutEditViewModel LayoutEditViewModelPc { get; set; }
		/// <summary>レイアウト編集SP 入力内容</summary>
		public LayoutEditViewModel LayoutEditViewModelSp { get; set; }
	}
}