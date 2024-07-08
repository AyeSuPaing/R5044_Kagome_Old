/*
=========================================================================================================
  Module      : 特集ページ情報 特集ページ詳細入力 (FeaturePageInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.ViewModels.FeaturePage;
using w2.Domain;
using w2.Domain.FeaturePage;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 特集ページ情報 ページ詳細入力
	/// </summary>
	public class FeaturePageInput : InputBase<FeaturePageModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageInput()
		{
			this.ManagementTitle = string.Empty;
			this.HtmlPageTitle = string.Empty;
			this.MetadataDesc = string.Empty;
			this.FileNameNoExtension = string.Empty;
			this.PageType = Constants.FLG_FEATUREPAGE_GROUP;
			this.Publish = Constants.FLG_FEATUREPAGE_PUBLISH_PRIVATE;
			this.ParentCategoryId = string.Empty;
			this.CategoryId = string.Empty;
			this.BrandIdList = new string[0];
			this.PcContentInput = new FeaturePageContentInput();
			this.SpContentInput = new FeaturePageContentInput();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">特集ページデザイン管理モデル</param>
		public FeaturePageInput(FeaturePageModel model)
			: this()
		{
			this.PageId = model.FeaturePageId;
			this.ManagementTitle = model.ManagementTitle;
			this.FileNameNoExtension = Path.GetFileNameWithoutExtension(model.FileName);
			this.PcContentInput.NoUse = ((string.IsNullOrEmpty(model.UseType) == false)
				&& (model.UseType.Contains(Constants.FLG_FEATUREPAGE_USE_TYPE_PC) == false));
			this.SpContentInput.NoUse = ((string.IsNullOrEmpty(model.UseType) == false)
				&& (model.UseType.Contains(Constants.FLG_FEATUREPAGE_USE_TYPE_SP) == false));
			this.PageType = model.PageType;
			this.Publish = model.Publish;
			this.MetadataDesc = model.MetadataDesc;
			this.HtmlPageTitle = model.HtmlPageTitle;
			this.ParentCategoryId = (string.IsNullOrEmpty(model.CategoryId) == false) ? model.CategoryId.Substring(0, 3) : string.Empty;
			this.CategoryId = model.CategoryId;
			this.BrandIdList = model.BrandIdList;

			var modalPublishModel = new ReleaseRangeSettingModel
			{
				ConditionPublishDateFrom = model.ConditionPublishDateFrom,
				ConditionPublishDateTo = model.ConditionPublishDateTo,
				ConditionMemberOnlyType = model.ConditionMemberOnlyType,
				ConditionMemberRankId = model.ConditionMemberRankId,
				ConditionTargetListIds = model.ConditionTargetListIds,
				ConditionTargetListType = model.ConditionTargetListType,
			};

			this.ReleaseRangeSettingInput = new ReleaseRangeSettingInput(
				"Page",
				"Input.ReleaseRangeSettingInput",
				modalPublishModel,
				useTargetList: false);
			this.ReleaseRangeSettingInput.UseMemberRank = false;
			this.ReleaseRangeSettingInput.UseTargetList = false;

			this.PcContentInput.Sort = model.GetSortWithAddProductList(true);
			this.SpContentInput.Sort = model.GetSortWithAddProductList(false);

			this.PcContentInput.ProductList = this.PcContentInput.Sort.Where(list => list.StartsWith("product-list-")).ToArray();

			this.PcContentInput.PageTitle = model.GetTitle(true);
			this.SpContentInput.PageTitle = model.GetTitle(false);

			this.PcContentInput.AltText = model.GetAltText(true);
			this.SpContentInput.AltText = model.GetAltText(false);
		}

		/// <summary>
		/// モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public override FeaturePageModel CreateModel()
		{
			var useTypeList = new List<string>();
			if (this.PcContentInput.NoUse == false) useTypeList.Add(Constants.FLG_FEATUREPAGE_USE_TYPE_PC);
			if (this.SpContentInput.NoUse == false) useTypeList.Add(Constants.FLG_FEATUREPAGE_USE_TYPE_SP);

			var useType = ((useTypeList.Count > 0)
				&& (useTypeList.Contains(Constants.FLG_FEATUREPAGE_USE_TYPE_PC) == false)
				|| (useTypeList.Contains(Constants.FLG_FEATUREPAGE_USE_TYPE_SP) == false))
				? string.Join(",", useTypeList)
				: string.Empty;

			var model = new FeaturePageModel
			{
				FeaturePageId = this.PageId,
				PageType = this.PageType,
				FileName = this.FileName,
				UseType = useType,
				ManagementTitle = this.ManagementTitle,
				CategoryId = this.CategoryId,
				PermittedBrandIds = string.Join(",", this.BrandIdList),
				Publish = this.Publish,
			};

			var releaseRangeSettingModel = this.ReleaseRangeSettingInput.CreateModel();
			model.ConditionPublishDateFrom = releaseRangeSettingModel.ConditionPublishDateFrom;
			model.ConditionPublishDateTo = releaseRangeSettingModel.ConditionPublishDateTo;
			model.ConditionMemberOnlyType = releaseRangeSettingModel.ConditionMemberOnlyType;
			model.ConditionMemberRankId = releaseRangeSettingModel.ConditionMemberRankId;
			model.ConditionTargetListIds = releaseRangeSettingModel.ConditionTargetListIds;
			model.ConditionTargetListType = releaseRangeSettingModel.ConditionTargetListType;
			model.MetadataDesc = this.MetadataDesc;
			model.HtmlPageTitle = this.HtmlPageTitle;

			return model;
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns>結果</returns>
		public string Validate()
		{
			var errorMessage = string.Empty;

			if ((this.PcContentInput.NoUse) && (this.SpContentInput.NoUse))
			{
				errorMessage += WebMessages.PageDesignDeviceUseCheck;
			}

			Hashtable ht;
			if (string.IsNullOrEmpty(this.ParentCategoryId))
			{
				ht = new Hashtable
				{
					{ Constants.FIELD_FEATUREPAGE_MANAGEMENT_TITLE, this.ManagementTitle },
					{ "file_name_no_extension", this.FileNameNoExtension },
					{ Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID, this.ParentCategoryId },
					{ "seo_title", this.PcContentInput.SeoTitle },
					{ "pc_alt_text", this.PcContentInput.AltText },
					{ "sp_alt_text", this.SpContentInput.AltText },
					{ "pc_" + Constants.FIELD_FEATUREPAGECONTENTS_PAGE_TITLE, this.PcContentInput.PageTitle },
					{ "sp_" + Constants.FIELD_FEATUREPAGECONTENTS_PAGE_TITLE, this.SpContentInput.PageTitle }
				};
			}
			else
			{
				ht = new Hashtable
				{
					{ Constants.FIELD_FEATUREPAGE_MANAGEMENT_TITLE, this.ManagementTitle },
					{ "file_name_no_extension", this.FileNameNoExtension },
					{ Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID, this.ParentCategoryId },
					{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, this.CategoryId },
					{ "seo_title", this.PcContentInput.SeoTitle },
					{ "pc_alt_text", this.PcContentInput.AltText },
					{ "sp_alt_text", this.SpContentInput.AltText },
					{ "pc_" + Constants.FIELD_FEATUREPAGECONTENTS_PAGE_TITLE, this.PcContentInput.PageTitle },
					{ "sp_" + Constants.FIELD_FEATUREPAGECONTENTS_PAGE_TITLE, this.SpContentInput.PageTitle }
				};
			}

			errorMessage += Validator.Validate("FeaturePageContentDetailModify", ht);

			if (this.FileNameNoExtension.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				errorMessage += WebMessages.PageDesignFileNameError;
			}

			errorMessage += this.ReleaseRangeSettingInput.Validate();

			if ((this.ProductInput != null))
			{
				if ((this.PageType != Constants.FLG_FEATUREPAGE_MULTI_GROUP) && (this.ProductInput.Length > 1))
				{
					errorMessage += WebMessages.FeaturePageMultiProductListError;
				}
				if ((this.PageType == Constants.FLG_FEATUREPAGE_SINGLE) && (this.ProductInput.Any(x=> x.DispNum > 1)))
				{
					errorMessage += WebMessages.FeaturePageSingleError;
				}

				this.ProductInput.ToList().ForEach(
					pinput => { errorMessage += pinput.Validate(); });
			}

			return errorMessage;
		}

		#region プロパティ
		/// <summary>ページID</summary>
		public long PageId { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>HTMLページタイトル</summary>
		public string HtmlPageTitle { get; set; }
		/// <summary>SEOディスクリプション</summary>
		public string MetadataDesc { get; set; }
		/// <summary>PCコンテンツ編集内容</summary>
		public FeaturePageContentInput PcContentInput { get; set; }
		/// <summary>SPコンテンツ編集内容</summary>
		public FeaturePageContentInput SpContentInput { get; set; }
		/// <summary>ファイル名(拡張子含む)</summary>
		private string FileName
		{
			get
			{
				return this.FileNameNoExtension.Replace(PageDesignUtility.PAGE_FILE_EXTENSION, string.Empty)
					+ PageDesignUtility.PAGE_FILE_EXTENSION;
			}
		}
		/// <summary>ファイル名(拡張子なし)</summary>
		public string FileNameNoExtension { get; set; }
		/// <summary>ページの公開状態</summary>
		public string Publish { get; set; }
		/// <summary>ページタイプ</summary>
		public string PageType { get; set; }
		/// <summary>商品一覧入力</summary>
		public ProductInputViewModel[] ProductInput { get; set; }
		/// <summary>公開範囲設定入力内容</summary>
		public ReleaseRangeSettingInput ReleaseRangeSettingInput { get; set; }
		/// <summary>親カテゴリ</summary>
		public string ParentCategoryId { get; set; }
		/// <summary>子カテゴリ</summary>
		public string CategoryId { get; set; }
		/// <summary>カテゴリ名</summary>
		public string CategoryName { get; set; }
		/// <summary>ブランドID</summary>
		public string[] BrandIdList { get; set; }
		/// <summary>商品リスト</summary>
		public string[] ProductList { get; set; }
		#endregion
	}

	/// <summary>
	/// 特集ページ情報 レイアウト・コンテンツ入力内容
	/// </summary>
	public class FeaturePageContentInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageContentInput()
		{
			this.LayoutEditInput = new LayoutEditInput(true);
		}

		/// <summary>利用しない？</summary>
		public bool NoUse { get; set; }
		/// <summary>レイアウト内容</summary>
		public LayoutEditInput LayoutEditInput { get; set; }
		/// <summary>SEOタイトル</summary>
		public string SeoTitle { get; set; }
		/// <summary>SEOキーワード</summary>
		public string SeoKeyword { get; set; }
		/// <summary>SEOディスクリプション</summary>
		public string SeoDescription { get; set; }
		/// <summary>コンテンツ内容</summary>
		public Dictionary<string, string> Content { get; set; }
		/// <summary>ページタイトル</summary>
		public string PageTitle { get; set; }
		/// <summary>ヘッダーバナー</summary>
		public ImageInput HeaderBanner { get; set; }
		/// <summary>代替テキスト</summary>
		public string AltText { get; set; }
		/// <summary>特集ページコンテンツ並び順</summary>
		public string[] Sort { get; set; }
		/// <summary>ページタイトル表示</summary>
		public bool PageTitleDisp { get; set; }
		/// <summary>ヘッダーバナー表示</summary>
		public bool HeaderBannerDisp { get; set; }
		/// <summary>商品一覧表示</summary>
		public bool[] ProductListDisp { get; set; }
		/// <summary>商品一覧</summary>
		public string[] ProductList { get; set; }
		/// <summary>コンテンツエリア上部表示</summary>
		public bool UpperContentsAreaDisp { get; set; }
		/// <summary>コンテンツエリア下部表示</summary>
		public bool LowerContentsAreaDisp { get; set; }
	}
}
