/*
=========================================================================================================
  Module      :  特集ページ情報 詳細 ビューモデル (FeaturePageDetailViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Domain.FeaturePage;

namespace w2.Cms.Manager.ViewModels.FeaturePage
{
	/// <summary>
	/// 特集ページ情報 詳細 ビューモデル
	/// </summary>
	public class FeaturePageDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageDetailViewModel(FeaturePageInput input)
		{
			this.Input = input;

			this.PcDirPath = string.Empty;

			this.PublishItems = ValueTextForCms.GetValueSelectListItems(
					Constants.TABLE_FEATUREPAGE,
					Constants.FIELD_FEATUREPAGE_PUBLISH)
				.Select(
					item => new SelectListItem
					{
						Value = item.Value,
						Text = item.Text
					})
				.ToArray();

			var defaultList = new[]
			{
				new SelectListItem
				{
					Value = string.Empty,
					Text = string.Empty
				}
			};

			this.ParentCategoryItems = defaultList.Concat(
				new FeaturePageService()
					.GetRootCategoryItem()
					.Select(
						s => new SelectListItem
						{
							Selected = (this.Input.ParentCategoryId == s.CategoryId),
							Value = s.CategoryId,
							Text = s.CategoryName,
						})).ToArray();

			if (string.IsNullOrEmpty(this.Input.ParentCategoryId) == false)
			{
				this.CategoryItems = new FeaturePageService()
					.GetChildCategoryItem(this.Input.ParentCategoryId)
					.Select(
						s => new SelectListItem
						{
							Selected = false,
							Value = s.CategoryId,
							Text = s.CategoryName
						}).ToArray();
			}
			else
			{
				this.CategoryItems = defaultList;
			}

			this.BrandItems = new FeaturePageService().GetBrand()
				.Select(
					s => new SelectListItem
					{
						Value = s.DataSource["brand_id"].ToString(),
						Text = s.DataSource["brand_name"].ToString(),
					}).ToArray();

			this.LayoutEditViewModelPc = new LayoutEditViewModel("Input.PcContentInput.LayoutEditInput", true);
			this.LayoutEditViewModelSp = new LayoutEditViewModel("Input.SpContentInput.LayoutEditInput", true);

			this.ImageModalViewModel = new ImageModalViewModel(ImageType.Page);
		}

		/// <summary>特集ページ詳細 入力内容</summary>
		public FeaturePageInput Input { get; set; }
		/// <summary>PC実ページ</summary>
		public RealPage PcRealPage { get; set; }
		/// <summary>SP実ページ</summary>
		public RealPage SpRealPage { get; set; }
		/// <summary>PCディレクトリパス</summary>
		public string PcDirPath { get; set; }
		/// <summary>公開状態ドロップダウンリスト</summary>
		public SelectListItem[] PublishItems { get; set; }
		/// <summary>レイアウト編集PC 入力内容</summary>
		public LayoutEditViewModel LayoutEditViewModelPc { get; set; }
		/// <summary>レイアウト編集SP 入力内容</summary>
		public LayoutEditViewModel LayoutEditViewModelSp { get; set; }
		/// <summary>画像リストから選択</summary>
		public ImageModalViewModel ImageModalViewModel { get; set; }
		/// <summary>親カテゴリドロップダウンリスト</summary>
		public SelectListItem[] ParentCategoryItems { get; set; }
		/// <summary>子カテゴリドロップダウンリスト</summary>
		public SelectListItem[] CategoryItems { get; set; }
		/// <summary>ブランドチェックボックスリスト</summary>
		public SelectListItem[] BrandItems { get; set; }
	}
}