/*
=========================================================================================================
  Module      : メインビューモデル(MainViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.FeatureImage;
using w2.Common.Util;
using w2.Domain.FeatureImage;

namespace w2.Cms.Manager.ViewModels.FeatureImage
{
	/// <summary>
	/// メインビューモデル
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			this.GroupListViewModel = new GroupListViewModel();

			this.ParamModel = new FeatureImageListSearchParamModel();

			var otherGroup = ValueTextForCms.GetValueSelectListItems(
					Constants.TABLE_FEATUREIMAGEGROUP,
					Constants.FIELD_FEATUREIMAGEGROUP_GROUP_NAME);

			this.GroupItems = otherGroup.Concat(
				new FeatureImageService().GetAllGroup().Select(
					g => new SelectListItem
					{
						Value = g.GroupId.ToString(),
						Text = g.GroupName
					})).ToArray();

			this.DateCreatedKbnItems = ValueTextForCms.GetValueSelectListItems(
					Constants.TABLE_FEATUREIMAGEGROUP,
					Constants.FIELD_FEATUREIMAGEGROUP_DATE_CREATED +"_kbn").ToArray();
		}

		/// <summary>特集画像グループビューモデル</summary>
		public GroupListViewModel GroupListViewModel { get; set; }
		/// <summary>グループドロップダウンリストアイテム</summary>
		public SelectListItem[] GroupItems { get; set; }
		/// <summary>作成日区分ドロップダウンリストアイテム</summary>
		public SelectListItem[] DateCreatedKbnItems { get; set; }
		/// <summary>検索パラメタモデル</summary>
		public FeatureImageListSearchParamModel ParamModel { get; set; }
	}
}