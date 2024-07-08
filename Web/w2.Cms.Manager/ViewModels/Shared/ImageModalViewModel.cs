/*
=========================================================================================================
  Module      : 画像リストから選択ビューモデル(ImageModalViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureImage;
using w2.Domain.FeatureImage;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>
	/// 画像リストから選択
	/// </summary>
	public class ImageModalViewModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ImageModalViewModel(ImageType imageType = ImageType.Normal)
		{
			this.ImageType = imageType;

			this.ParamModel = new FeatureImageListSearchParamModel();

			var otherGroup = ValueTextForCms.GetValueSelectListItems(
					Constants.TABLE_FEATUREIMAGEGROUP,
					Constants.FIELD_FEATUREIMAGEGROUP_GROUP_NAME);

			this.GroupItems = otherGroup.Concat(new FeatureImageService().GetAllGroup().Select(
				g => new SelectListItem
				{
					Value = g.GroupId.ToString(),
					Text = g.GroupName
				})).ToArray();
		}
		
		/// <summary>検索パラメタモデル</summary>
		public FeatureImageListSearchParamModel ParamModel { get; set; }
		/// <summary>グループドロップダウンリストアイテム</summary>
		public SelectListItem[] GroupItems { get; set; }
		/// <summary>画像タイプ</summary>
		public ImageType ImageType { get; set; }
	}
}
