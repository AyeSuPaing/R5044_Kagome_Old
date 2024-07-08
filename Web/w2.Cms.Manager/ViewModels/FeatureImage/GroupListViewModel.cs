/*
=========================================================================================================
  Module      : 特集画像グループ一覧ビューモデル(GroupListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using w2.App.Common;
using w2.Cms.Manager.Input;
using w2.Domain.FeatureImage;

namespace w2.Cms.Manager.ViewModels.FeatureImage
{
	/// <summary>
	/// 特集画像グループ一覧ビューモデル
	/// </summary>
	public class GroupListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupListViewModel()
		{
			this.GroupImageViewModel = new List<GroupImageModel>();
		}

		/// <summary>グループ別</summary>
		public List<GroupImageModel> GroupImageViewModel { get; set; }
		/// <summary>商品画像タイプ</summary>
		public ImageType ImageType { get; set; }
	}

	/// <summary>
	/// 画像グループモデル
	/// </summary>
	public class GroupImageModel : FeatureImageGroupModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GroupImageModel()
			: base()
		{
			this.ListImageViewModels = new List<ImageViewModel>();
			this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_FEATURE_IMAGE_GROUP_ID] = (long)0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">グループモデル</param>
		public GroupImageModel(FeatureImageGroupModel model) : base(model.DataSource)
		{
			this.ListImageViewModels = new List<ImageViewModel>();
		}

		/// <summary>グループ内の画像一覧</summary>
		public List<ImageViewModel> ListImageViewModels { get; set; }
	}

	/// <summary>
	/// 表示用Imageオブジェクト
	/// </summary>
	[Serializable]
	public class ImageViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImageViewModel()
		{
			this.GroupId = 0;
			this.Sort = 0;
		}

		/// <summary>画像ID</summary>
		public long ImageId { get; set; }
		/// <summary>ファイル名</summary>
		public string FileName
		{
			get
			{
				return Path.GetFileName(this.ImagePath);
			}
		}
		/// <summary>更新日</summary>
		public string DataChanged { get; set; }
		/// <summary>作成日</summary>
		public DateTime DateCreated { get; set; }
		/// <summary>ファイルサイズ(KB,MB)</summary>
		public string FileSize { get; set; }
		/// <summary>イメージサイズ(縦×横)</summary>
		public string ImageSize { get; set; }
		/// <summary>画像パス</summary>
		public string ImagePath { get;set; }
		/// <summary>グループID</summary>
		public long GroupId { get; set; }
		/// <summary>画像順序</summary>
		public int Sort { get; set; }
		/// <summary> 表示用ファイル名 </summary>
		public string RealFileName { get; set; }
	}
}