/*
=========================================================================================================
  Module      : SEO設定Modifyビューモデル(ModifyViewModel.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Affiliate;
using w2.Cms.Manager.Input;
using w2.Common.Util;

namespace w2.Cms.Manager.ViewModels.SeoMetadatas
{
	/// <summary>
	/// SEO設定Modifyビューモデル
	/// </summary>
	public class ModifyViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ModifyViewModel()
		{
			this.SeoAllReplaceTags = new ReplaceTagManager()
				.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_SEO_ALL)
				.Select(m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();

			this.SeoProductListReplaceTags = new ReplaceTagManager()
				.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_SEO_PRODUCT_LIST)
				.Select(m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();

			this.SeoProductDetailReplaceTags = new ReplaceTagManager()
				.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_SEO_PRODUCT_DETAIL)
				.Select(m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();

			this.CoordinateListReplaceTags = new ReplaceTagManager()
				.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_COORDINATE_LIST)
				.Select(m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();

			this.CoordinateDetailReplaceTags = new ReplaceTagManager()
				.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_COORDINATE_DETAIL)
				.Select(m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();

			this.SeoProductListReplaceTags[3].Text = string.Format(
				this.SeoProductListReplaceTags[3].Text,
				Constants.SEOSETTING_CHILD_CATEGORY_TOP_COUNT);

			this.CoordinateListReplaceTags[2].Text = string.Format(
				this.CoordinateListReplaceTags[2].Text,
				Constants.SEOSETTING_CHILD_CATEGORY_TOP_COUNT);
		}

		/// <summary>全体設定用インプット</summary>
		public SeoMetadatasInput InputForDefault { get; set; }
		/// <summary>商品一覧用インプット</summary>
		public SeoMetadatasInput InputForProductList { get; set; }
		/// <summary>商品詳細用インプット</summary>
		public SeoMetadatasInput InputForProductDetail { get; set; }
		/// <summary>コーディネートトップ用インプット</summary>
		public SeoMetadatasInput InputForCoordinateTop { get; set; }
		/// <summary>コーディネート一覧用インプット</summary>
		public SeoMetadatasInput InputForCoordinateList { get; set; }
		/// <summary>コーディネート詳細用インプット</summary>
		public SeoMetadatasInput InputForCoordinateDetail { get; set; }
		/// <summary>SEO全体設定用置換タグリスト</summary>
		public ListItem[] SeoAllReplaceTags { get; set; }
		/// <summary>SEO商品一覧用置換タグリスト</summary>
		public ListItem[] SeoProductListReplaceTags { get; set; }
		/// <summary>SEO商品詳細用置換タグリスト</summary>
		public ListItem[] SeoProductDetailReplaceTags { get; set; }
		/// <summary>コーディネート一覧用置換タグリスト</summary>
		public ListItem[] CoordinateListReplaceTags { get; set; }
		/// <summary>コーディネート詳細用置換タグリスト</summary>
		public ListItem[] CoordinateDetailReplaceTags { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
	}
}