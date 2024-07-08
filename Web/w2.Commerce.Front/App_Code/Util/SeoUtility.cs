/*
=========================================================================================================
  Module      : SeoUtilityクラス(SeoUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using w2.App.Common.Coordinate;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order;
using w2.App.Common.Preview;
using w2.Domain.Coordinate;
using w2.Domain.FeaturePage;
using w2.Domain.OgpTagSetting;
using w2.Domain.PageDesign;
using w2.Domain.SeoMetadatas;

/// <summary>
/// SeoUtilityクラス
/// </summary>
public class SeoUtility
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <param name="page">ページ</param>
	/// <param name="session">セッション</param>
	public SeoUtility(
		HttpRequest request,
		Page page,
		HttpSessionState session)
	{
		this.Request = request;
		this.Page = page;
		this.Session = session;

		if (Constants.SEOTAGDISPSETTING_OPTION_ENABLED)
		{
			this.SeoData = new Seo(GetSeoMetadatas(), this.SeoDefaultSetting);
			this.OgpData = new Ogp(GetOgpTagSetting(), this.OgpDefaultSetting);
		}
	}

	/// <summary>
	/// Seoデータを取得
	/// </summary>
	/// <returns>Seoデータ</returns>
	public SeoMetadatasModel GetSeoMetadatas()
	{
		var className = this.Page.GetType().BaseType.FullName;
		var fileName = Path.GetFileName(this.Request.Path);

		switch (className)
		{
			case Constants.CLASS_NAME_FEATURE_TEMPLATE:
				var featurePage = new FeaturePageService().GetByFileName(fileName);
				return new SeoMetadatasModel
				{
					HtmlTitle = this.Page.Title,
					MetadataDesc = (featurePage == null)
						? string.Empty
						: featurePage.MetadataDesc
				};

			case Constants.CLASS_NAME_CONTENTS_PAGE:
				var page = new PageDesignService().GetPageByFileName(fileName);
				return new SeoMetadatasModel
				{
					HtmlTitle = this.Page.Title,
					MetadataDesc = (page == null) ? string.Empty : page.MetadataDesc
				};

			case Constants.CLASS_NAME_PRODUCT_DETAIL:
				return GetSeoMetadatasForProductDetail();

			case Constants.CLASS_NAME_PRODUCT_LIST:
				return ProductCommon.GetSeoMetadatasForProductList(
					this.ShopId,
					this.CategoryId,
					this.BrandId,
					this.ProductGroupId,
					null,
					this.MinPrice,
					this.MaxPrice,
					this.Color,
					this.SearchWord,
					this.CampaignIcon,
					this.ProductTag);

			case Constants.CLASS_NAME_COORDINATE_TOP:
				return CoordinateCommon.GetSeoMetadatasForCoordinateTop();

			case Constants.CLASS_NAME_COORDINATE_DETAIL:
				return GetSeoMetadatasForCoordinateDetail();

			case Constants.CLASS_NAME_COORDINATE_LIST:
				return CoordinateCommon.GetSeoMetadatasForCoordinateList(this.CoordinateCategoryId);

			case Constants.CLASS_NAME_SHOPLIST:
			case Constants.CLASS_NAME_SHOPDETAIL:
				var result = new SeoMetadatasModel
				{
					HtmlTitle = this.Page.Title,
				};

				return result;

			default:
				return this.SeoDefaultSetting;
		}
	}

	/// <summary>
	/// Ogpデータを取得
	/// </summary>
	/// <returns>Ogpデータ</returns>
	public OgpTagSettingModel GetOgpTagSetting()
	{
		var className = this.Page.GetType().BaseType.FullName;

		switch (className)
		{
			case Constants.CLASS_NAME_PRODUCT_DETAIL:
				return GetOgpTagSettingForProductDetail();

			case Constants.CLASS_NAME_COORDINATE_DETAIL:
				return GetOgpTagSettingForCoordinateDetail();

			default:
				return this.OgpDefaultSetting;
		}
	}

	/// <summary>
	/// 商品詳細用のSEOデータを取得
	/// </summary>
	/// <returns></returns>
	public SeoMetadatasModel GetSeoMetadatasForProductDetail()
	{
		bool bPreviewMode = false;
		if (this.PreviewHash != null)
		{
			// ハッシュチェック
			if (this.PreviewHash == ProductPreview.CreateProductDetailHash()) bPreviewMode = true;
		}

		DataView dvProduct = (bPreviewMode) ?
			ProductPreview.GetProductDetailPreview(this.ShopId, this.ProductId)
			: ProductCommon.GetProductInfo(this.ShopId, this.ProductId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);

		return ProductCommon.GetSeoMetadatasForProductDetail(this.ShopId, (string)dvProduct[0][Constants.FIELD_PRODUCT_CATEGORY_ID1], this.BrandId, dvProduct[0]);
	}

	/// <summary>
	/// コーディネート詳細用のSEOデータを取得
	/// </summary>
	/// <returns></returns>
	public SeoMetadatasModel GetSeoMetadatasForCoordinateDetail()
	{
		var coordinate = (this.IsContentsPreview)
			? CoordinateCommon.GetPreview(this.ShopId, this.CoordinateId)
			: new CoordinateService().GetWithChilds(this.CoordinateId, this.ShopId);

		var categoryId = (coordinate.CategoryList.Count != 0)
			? coordinate.CategoryList[0].CoordinateCategoryId
			: string.Empty;

		return CoordinateCommon.GetSeoMetadatasForCoordinateDetail(categoryId, coordinate);
	}

	/// <summary>
	/// 商品詳細用のOGPデータを取得
	/// </summary>
	/// <returns></returns>
	public OgpTagSettingModel GetOgpTagSettingForProductDetail()
	{
		bool bPreviewMode = false;
		if (this.PreviewHash != null)
		{
			// ハッシュチェック
			if (this.PreviewHash == ProductPreview.CreateProductDetailHash()) bPreviewMode = true;
		}

		var dvProduct = (bPreviewMode) ?
			ProductPreview.GetProductDetailPreview(this.ShopId, this.ProductId)
			: ProductCommon.GetProductInfo(this.ShopId, this.ProductId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);

		var imagePath = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PATH_PRODUCTIMAGES;

		var ogpTagSetting = new OgpTagSettingModel
		{
			PageTitle = this.OgpDefaultSetting.PageTitle,
			Description = this.OgpDefaultSetting.Description,
			SiteUrl = this.OgpDefaultSetting.SiteUrl,
			Type = Constants.FLG_OGPTAGSETTING_TYPE_ARTICLE,
			SiteTitle = this.OgpDefaultSetting.SiteTitle,
			ImageUrl = string.IsNullOrEmpty((string)dvProduct[0][Constants.FIELD_PRODUCT_IMAGE_HEAD])
				? imagePath + "NowPrinting_M.jpg"
				: imagePath
					+ this.ShopId + "/"
					+ (string)dvProduct[0][Constants.FIELD_PRODUCT_IMAGE_HEAD]
					+ "_M.jpg",
		};

		return ogpTagSetting;
	}

	/// <summary>
	/// コーディネート詳細用のOGPデータを取得
	/// </summary>
	/// <returns>OGPデータ</returns>
	public OgpTagSettingModel GetOgpTagSettingForCoordinateDetail()
	{
		var imagePath = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + CoordinatePage.CreateCoordinateImageUrl(this.CoordinateId, 1, this.OperatorId);

		var ogpTagSetting = new OgpTagSettingModel
		{
			PageTitle = this.OgpDefaultSetting.PageTitle,
			Description = this.OgpDefaultSetting.Description,
			SiteUrl = this.OgpDefaultSetting.SiteUrl,
			Type = Constants.FLG_OGPTAGSETTING_TYPE_ARTICLE,
			SiteTitle = this.OgpDefaultSetting.SiteTitle,
			ImageUrl = imagePath,
		};

		return ogpTagSetting;
	}

	/// <summary>リクエスト</summary>
	public HttpRequest Request { get; set; }
	/// <summary>ページ</summary>
	public Page Page { get; set; }
	/// <summary>ページ</summary>
	public HttpSessionState Session { get; set; }
	/// <summary>ページ</summary>
	public Seo SeoData { get; set; }
	/// <summary>ページ</summary>
	public Ogp OgpData { get; set; }
	/// <summary>店舗ID</summary>
	protected string ShopId
	{
		get
		{
			var shopId = this.Request[Constants.REQUEST_KEY_SHOP_ID];
			if (string.IsNullOrEmpty(shopId)) return Constants.CONST_DEFAULT_SHOP_ID;
			return shopId;
		}
	}
	/// <summary>コーディネートID</summary>
	protected string CoordinateId
	{
		get{ return this.Request[Constants.REQUEST_KEY_COORDINATE_ID] ?? string.Empty;}
	}
	/// <summary>スタッフID</summary>
	protected string StaffId
	{
		get { return this.Request[Constants.REQUEST_KEY_COORDINATE_STAFF_ID] ?? string.Empty; }
	}
	/// <summary>コーディネートカテゴリID</summary>
	protected string CoordinateCategoryId
	{
		get { return this.Request[Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID] ?? string.Empty; }
	}
	/// <summary>リアル店舗ID</summary>
	protected string RealShopId
	{
		get { return this.Request[Constants.REQUEST_KEY_REAL_SHOP_ID] ?? string.Empty; }
	}
	/// <summary>タグID</summary>
	protected string ContentsTagId
	{
		get { return this.Request[Constants.REQUEST_KEY_CONTENTS_TAG_ID] ?? string.Empty; }
	}
	/// <summary>オペレータID</summary>
	protected string OperatorId
	{
		get { return this.Request[Constants.REQUEST_KEY_OPERATOR_ID] ?? string.Empty; }
	}
	/// <summary>商品ID</summary>
	protected string ProductId
	{
		get
		{
			var product = this.Request[Constants.REQUEST_KEY_PRODUCT_ID] ?? string.Empty;
			if (this.LoginUserId == "preview") return Uri.UnescapeDataString(product);

			return product;
		}
	}
	/// <summary>コンテンツプレビューか</summary>
	protected bool IsContentsPreview
	{
		get { return (string.IsNullOrEmpty(this.OperatorId) == false); }
	}
	/// <summary>商品グループID</summary>
	protected string ProductGroupId
	{
		get { return this.Request[Constants.REQUEST_KEY_PRODUCT_GROUP_ID] ?? string.Empty; }
	}
	/// <summary>カテゴリID</summary>
	protected string CategoryId
	{
		get { return this.Request[Constants.REQUEST_KEY_CATEGORY_ID] ?? string.Empty; }
	}
	/// <summary>検索ワード</summary>
	protected string SearchWord
	{
		get { return this.Request[Constants.REQUEST_KEY_SEARCH_WORD]; }
	}
	/// <summary>キャンペーンアイコン</summary>
	public string CampaignIcon
	{
		get { return this.Request[Constants.REQUEST_KEY_CAMPAINGN_ICOM]; }
	}
	/// <summary>最小価格</summary>
	public string MinPrice
	{
		get { return this.Request[Constants.REQUEST_KEY_MIN_PRICE]; }
	}
	/// <summary>最大価格</summary>
	public string MaxPrice
	{
		get { return this.Request[Constants.REQUEST_KEY_MAX_PRICE]; }
	}
	/// <summary>カラー</summary>
	protected string Color
	{
		get { return this.Request[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]; }
	}
	/// <summary>商品タグ</summary>
	public string ProductTag
	{
		get
		{
			Dictionary<string, object> dicParams = ProductPage.GetParameters(this.Request);
			var RequestParameter = new Dictionary<string, string>();
			foreach (string requestKey in dicParams.Keys)
			{
				RequestParameter.Add(requestKey, dicParams[requestKey].ToString());
			}

			var productTags = ProductListPage.GetTagSettingListForSeo(RequestParameter);
			var productTag = (productTags != null) ? string.Join(" ", productTags.Cast<DataRowView>().Select(item => item["tag_name"]).ToList()) : string.Empty;
			return productTag;
		}
	}
	/// <summary>プレビューハッシュ</summary>
	public string PreviewHash
	{
		get { return this.Request[Constants.REQUEST_KEY_PREVIEW_HASH] ?? string.Empty; }
	}
	/// <summary>ログイン状態</summary>
	public bool IsLoggedIn
	{
		get { return (this.LoginUserId != null); }
	}
	/// <summary>ログインユーザーID</summary>
	public string LoginUserId
	{
		get { return (string)this.Session[Constants.SESSION_KEY_LOGIN_USER_ID]; }
		set { this.Session[Constants.SESSION_KEY_LOGIN_USER_ID] = value; }
	}
	/// <summary>会員ランクID</summary>
	public string MemberRankId
	{
		// あまりいいやり方でないけれど、ログインしてるかどうかに関わらず使ってる箇所が多すぎるので。。
		get
		{
			if (this.IsLoggedIn)
			{
				return this.LoginUserMemberRankId;
			}
			else
			{
				// 会員ランクOPOFFの場合はnothingを返す必要がある
				return Constants.MEMBER_RANK_OPTION_ENABLED ? "" : "nothing";
			}
		}
	}
	/// <summary>ログインユーザー会員ランクID</summary>
	public string LoginUserMemberRankId
	{
		get { return (string)this.Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID]; }
		set { this.Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID] = value; }
	}
	/// <summary>User Fixed Purchase Member Flg</summary>
	public string UserFixedPurchaseMemberFlg
	{
		get
		{
			return (this.IsLoggedIn ? (this.LoginUserFixedPurchaseMemberFlg ?? string.Empty) : string.Empty);
		}
	}
	/// <summary>ログインユーザー定期会員フラグ</summary>
	public string LoginUserFixedPurchaseMemberFlg
	{
		get { return (string)this.Session[Constants.SESSION_KEY_LOGIN_USER_FIXED_PURCHASE_MEMBER_FLG]; }
		set { this.Session[Constants.SESSION_KEY_LOGIN_USER_FIXED_PURCHASE_MEMBER_FLG] = value; }
	}
	/// <summary>ブランドID</summary>
	public string BrandId
	{
		get { return StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_BRAND_ID]); }
		set { this.Session[Constants.SESSION_KEY_BRAND_ID] = value; }
	}
	/// <summary>OGP全体設定</summary>
	private OgpTagSettingModel OgpDefaultSetting
	{
		get
		{
			var cash = DataCacheControllerFacade
				.GetOgpTagSettingCacheController()
				.CacheData
				.FirstOrDefault(s => (s.DataKbn == Constants.FLG_OGPTAGSETTING_DATA_KBN_DEFAULT_SETTING)) ?? new OgpTagSettingModel();
			var ogpTagDefaultSetting = cash.Clone();

			ogpTagDefaultSetting.PageTitle = ogpTagDefaultSetting.PageTitle.Replace(Constants.SEOSETTING_KEY_SEO_TITLE, this.SeoData.HtmlTitle);
			ogpTagDefaultSetting.Description = ogpTagDefaultSetting.Description.Replace(Constants.SEOSETTING_KEY_SEO_DESCRIPTION, this.SeoData.MetadataDesc);
			ogpTagDefaultSetting.SiteUrl = this.Request.Url.AbsoluteUri;
			ogpTagDefaultSetting.Type = (this.Request.RawUrl == Constants.PATH_ROOT_FRONT_PC)
				? Constants.FLG_OGPTAGSETTING_TYPE_WEBSITE
				: Constants.FLG_OGPTAGSETTING_TYPE_ARTICLE;
			ogpTagDefaultSetting.SiteTitle = ogpTagDefaultSetting.SiteTitle;

			return ogpTagDefaultSetting;
		}
	}
	/// <summary>SEO全体設定</summary>
	private SeoMetadatasModel SeoDefaultSetting
	{
		get
		{
			var cash = DataCacheControllerFacade
				.GetSeoMetadatasCacheController()
				.CacheData
				.FirstOrDefault(s => (s.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_DEFAULT_SETTING)) ?? new SeoMetadatasModel();
			var seoDefaultSetting = cash.Clone();

			var htmlTitle = this.Page.Title;
			seoDefaultSetting.HtmlTitle = seoDefaultSetting.HtmlTitle.Replace(Constants.SEOSETTING_KEY_HTML_TITLE, htmlTitle);
			seoDefaultSetting.MetadataDesc = seoDefaultSetting.MetadataDesc.Replace(Constants.SEOSETTING_KEY_HTML_TITLE, htmlTitle);

			return seoDefaultSetting;
		}
	}

	/// <summary>
	/// OGPメタデータ
	/// </summary>
	public class Ogp : OgpTagSettingModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Ogp()
		{
			this.DataKbn = string.Empty;
			this.SiteTitle = string.Empty;
			this.PageTitle = string.Empty;
			this.Description = string.Empty;
			this.ImageUrl = string.Empty;
			this.LastChanged = string.Empty;
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Ogp(OgpTagSettingModel ogp, OgpTagSettingModel defaultOgp)
		{
			this.SiteTitle = string.IsNullOrEmpty(ogp.SiteTitle)
				? defaultOgp.SiteTitle
				: ogp.SiteTitle;
			this.ImageUrl = string.IsNullOrEmpty(ogp.ImageUrl)
				? defaultOgp.ImageUrl
				: ogp.ImageUrl;
			this.PageTitle = string.IsNullOrEmpty(ogp.PageTitle)
				? defaultOgp.PageTitle
				: ogp.PageTitle;
			this.Description = string.IsNullOrEmpty(ogp.Description)
				? defaultOgp.Description
				: ogp.Description;
			this.SiteUrl = string.IsNullOrEmpty(ogp.SiteUrl)
				? defaultOgp.SiteUrl
				: ogp.SiteUrl;
			this.Type = string.IsNullOrEmpty(ogp.Type)
				? defaultOgp.Type
				: ogp.Type;
		}
	}

	/// <summary>
	/// SEOメタデータ
	/// </summary>
	public class Seo : SeoMetadatasModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Seo()
		{
			this.DelFlg = "0";
			this.HtmlTitle = string.Empty;
			this.MetadataDesc = string.Empty;
			this.MetadataKeywords = string.Empty;
			this.Comment = string.Empty;
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Seo(SeoMetadatasModel seo, SeoMetadatasModel defaultSeo)
		{
			this.HtmlTitle = string.IsNullOrEmpty(seo.HtmlTitle)
				? defaultSeo.HtmlTitle
				: seo.HtmlTitle;
			this.MetadataDesc = string.IsNullOrEmpty(seo.MetadataDesc)
				? defaultSeo.MetadataDesc
				: seo.MetadataDesc;
			this.MetadataKeywords = string.IsNullOrEmpty(seo.MetadataKeywords)
				? defaultSeo.MetadataKeywords
				: seo.MetadataKeywords;
			this.Comment = string.IsNullOrEmpty(seo.Comment)
				? defaultSeo.Comment
				: seo.Comment;
		}
	}
}