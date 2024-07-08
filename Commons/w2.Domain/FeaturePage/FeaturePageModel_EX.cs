/*
=========================================================================================================
  Module      : 特集ページ情報モデル (FeaturePageModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;

namespace w2.Domain.FeaturePage
{
	/// <summary>
	/// 特集ページ情報モデル
	/// </summary>
	partial class FeaturePageModel
	{
		#region メソッド
		/// <summary>
		/// ページタイトルを取得
		/// </summary>
		/// <param name="isPc">コンテンツ区分</param>
		/// <returns>ページタイトル</returns>
		public string GetTitle(bool isPc)
		{
			var isPcFlg = isPc
				? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC
				: Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP;
			var result = (this.Contents.Length > 0)
				? this.Contents.First(item => ((item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE)
						&& (item.ContentsKbn == isPcFlg)))
					.PageTitle
				: string.Empty;

			return result;
		}

		/// <summary>
		/// 代替テキスト取得
		/// </summary>
		/// <param name="isPc">PCフラグ</param>
		/// <returns>代替テキスト</returns>
		public string GetAltText(bool isPc)
		{
			var isPcFlg = isPc
				? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC
				: Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP;

			var result = (this.Contents.Length > 0)
				? this.Contents.First(item => ((item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_HEADER_BANNER)
						&& (item.ContentsKbn == isPcFlg)))
					.AltText
				: string.Empty;
			return result;
		}

		/// <summary>
		/// 並び順取得
		/// </summary>
		/// <param name="isPc">PCフラグ</param>
		/// <returns>並び順</returns>
		public string[] GetSort(bool isPc)
		{
			var isPcFlg = isPc
				? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC
				: Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP;

			var contents = this.Contents
				.Where(item => (item.ContentsKbn == isPcFlg))
				.OrderBy(item => item.ContentsSortNumber)
				.ToArray();

			var productCnt = 0;
			var result = contents.Select(item =>
				{
					switch (item.ContentsType)
					{
						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE:
							return Constants.FEATUREPAGE_PAGE_TITLE;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_HEADER_BANNER:
							return Constants.FEATUREPAGE_HEADER_BANNER;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_UPPER_CONTENTS_AREA:
							return Constants.FEATUREPAGE_UPPER_CONTENTS_AREA;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST:
							return Constants.FEATUREPAGE_GROUP_ITEMS;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_LOWER_CONTENTS_AREA:
							return Constants.FEATUREPAGE_LOWER_CONTENTS_AREA;

						default:
							return string.Empty;
					}
				}).ToArray();

			return result;
		}

		/// <summary>
		/// 管理画面用並び順取得
		/// </summary>
		/// <param name="isPc">PCフラグ</param>
		/// <returns>並び順</returns>
		public string[] GetSortWithAddProductList(bool isPc)
		{
			var isPcFlg = isPc
				? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC
				: Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP;

			var contents = this.Contents
				.Where(item => (item.ContentsKbn == isPcFlg))
				.OrderBy(item => item.ContentsSortNumber)
				.ToArray();

			var productCnt = 0;
			var list = contents.Select(item =>
				{
					switch (item.ContentsType)
					{
						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE:
							return Constants.FEATUREPAGE_PAGE_TITLE;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_HEADER_BANNER:
							return Constants.FEATUREPAGE_HEADER_BANNER;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_UPPER_CONTENTS_AREA:
							return Constants.FEATUREPAGE_UPPER_CONTENTS_AREA;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST:
							return Constants.FEATUREPAGE_PRODUCR_LIST + productCnt++;

						case Constants.FLG_FEATUREPAGECONTENTS_TYPE_LOWER_CONTENTS_AREA:
							return Constants.FEATUREPAGE_LOWER_CONTENTS_AREA;

						default:
							return string.Empty;
					}
				}).ToList();

			var insertPos = list.IndexOf(Constants.FEATUREPAGE_PRODUCR_LIST + (productCnt - 1));
			list.Insert(insertPos + 1, Constants.FEATUREPAGE_ADD_PRODUCT_LIST);
			return list.ToArray();
		}
		#endregion

		#region プロパティ
		/// <summary>コンテンツ</summary>
		public FeaturePageContentsModel[] Contents
		{
			get { return (FeaturePageContentsModel[])this.DataSource["EX_Contents"]; }
			set { this.DataSource["EX_Contents"] = value; }
		}
		/// <summary>特集ページパス</summary>
		public string FeaturePagePath { get; set; }
		/// <summary>PCページ画像ソース</summary>
		public string PcBannerImageSrc { get; set; }
		/// <summary>SPページ画像ソース</summary>
		public string SpBannerImageSrc { get; set; }
		/// <summary>PCページタイトル</summary>
		public string PcPageTitle
		{
			get
			{
				var content = this.Contents.FirstOrDefault(
					item =>
					{
						return ((item.ContentsKbn == Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC)
							&& (item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE));
					});
				var pageTitle = (content == null) ? string.Empty : content.PageTitle;
				return pageTitle;
			}
		}
		/// <summary>SPページタイトル</summary>
		public string SpPageTitle
		{
			get
			{
				var content = this.Contents.FirstOrDefault(
					item =>
					{
						return ((item.ContentsKbn == Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP)
							&& (item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE));
					});
				var pageTitle = (content == null) ? string.Empty : content.PageTitle;
				return pageTitle;
			}
		}
		/// <summary>カテゴリ名</summary>
		public string CategoryName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME] = value; }
		}
		/// <summary>ブランドリスト</summary>
		public string[] BrandIdList
		{
			get { return this.PermittedBrandIds.Split(','); }
		}
		#endregion
	}
}