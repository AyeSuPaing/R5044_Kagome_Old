/*
=========================================================================================================
  Module      : ページ選択によるグループ移動 ビューモデル(GroupMoveViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Domain.PageDesign;

namespace w2.Cms.Manager.ViewModels.PageDesign
{
	/// <summary>
	/// ページ選択によるグループ移動 ビューモデル
	/// </summary>
	public class GroupMoveViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupMoveViewModel()
		{
			this.GroupItems = new PageDesignService().GetAllGroup().Select(
					g => new SelectListItem
					{
						Value = g.GroupId.ToString(),
						Text = g.GroupName
					}).ToArray();
			var otherGroup = PageDesignUtility.OtherPageGroupModel;
			this.GroupItems = this.GroupItems.Concat(
				new[]
				{
					new SelectListItem
					{
						Value = otherGroup.GroupId.ToString(),
						Text = otherGroup.GroupName
					}
				}).ToArray();
		}

		/// <summary>選択グループモデル</summary>
		public PageDesignGroupModel SelectGroupModel { get; set; }
		/// <summary>ページID</summary>
		public long PageId { get; set; }
		/// <summary>グループリスト</summary>
		public SelectListItem[] GroupItems { get; private set; }
	}
}