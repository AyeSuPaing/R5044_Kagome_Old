/*
=========================================================================================================
  Module      : ドメインファサード(DomainFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.Domain.AbTest;
using w2.Domain.AdvCode;
using w2.Domain.Affiliate;
using w2.Domain.AutoTranslationWord;
using w2.Domain.CountryLocation;
using w2.Domain.Coupon;
using w2.Domain.DefaultSetting;
using w2.Domain.DeliveryCompany;
using w2.Domain.DispSummaryAnalysis;
using w2.Domain.ExchangeRate;
using w2.Domain.FeatureArea;
using w2.Domain.FeaturePage;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.GlobalShipping;
using w2.Domain.GlobalZipcode;
using w2.Domain.InvoiceAtobaraicom;
using w2.Domain.InvoiceVeritrans;
using w2.Domain.LandingPage;
using w2.Domain.MailTemplate;
using w2.Domain.MallCooperationSetting;
using w2.Domain.MallExhibitsConfig;
using w2.Domain.ManagerListDispSetting;
using w2.Domain.MasterExportSetting;
using w2.Domain.MemberRank;
using w2.Domain.MemberRankRule;
using w2.Domain.NameTranslationSetting;
using w2.Domain.News;
using w2.Domain.Novelty;
using w2.Domain.OgpTagSetting;
using w2.Domain.OperatorAuthority;
using w2.Domain.Order;
using w2.Domain.OrderExtendSetting;
using w2.Domain.OrderExtendStatusSetting;
using w2.Domain.OrderMemoSetting;
using w2.Domain.OrderWorkflowExecHistory;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.PageDesign;
using w2.Domain.PartsDesign;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ProductBrand;
using w2.Domain.ProductCategory;
using w2.Domain.ProductDefaultSetting;
using w2.Domain.ProductExtend;
using w2.Domain.ProductExtendSetting;
using w2.Domain.ProductFixedPurchaseDiscountSetting;
using w2.Domain.ProductGroup;
using w2.Domain.ProductListDispSetting;
using w2.Domain.ProductPrice;
using w2.Domain.ProductSale;
using w2.Domain.ProductSet;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;
using w2.Domain.ProductStockMessage;
using w2.Domain.ProductSubImageSetting;
using w2.Domain.ProductTag;
using w2.Domain.ProductTaxCategory;
using w2.Domain.RealShop;
using w2.Domain.Recommend;
using w2.Domain.ScoreAxis;
using w2.Domain.ScoringSale;
using w2.Domain.SeoMetadatas;
using w2.Domain.SetPromotion;
using w2.Domain.ShopOperator;
using w2.Domain.ShopShipping;
using w2.Domain.ShortUrl;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.TwInvoice;
using w2.Domain.TwOrderInvoice;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserManagementLevel;
using w2.Domain.UserShipping;

namespace w2.Domain
{
	/// <summary>
	/// ドメインファサード
	/// </summary>
	public class DomainFacade
	{
		/// <summary>インスタンス</summary>
		private static readonly DomainFacade m_instance = new DomainFacade();

		/// <summary>本物のサービスクラスをセットするか</summary>
		private readonly bool m_setRealServices = false;

		/// <summary>サービスの集合（コード簡略化のためHashtableで実装）</summary>
		private readonly Hashtable m_services = new Hashtable();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setRealServices">本物のサービスクラスをセットするか（テストの時にfalse利用想定）</param>
		private DomainFacade(bool setRealServices = true)
		{
			m_setRealServices = setRealServices;
		}

		#region -GetService サービス取得

		/// <summary>
		/// サービス取得
		/// </summary>
		/// <typeparam name="T1">サービスのインターフェース</typeparam>
		/// <typeparam name="T2">本物のサービスクラス</typeparam>
		/// <returns>サービス</returns>
		private T1 GetService<T1, T2>()
			where T1 : IService
			where T2 : IService, new()
		{
			var key = CreateServicesKey<T1>();
			if (m_services[key] == null)
			{
				if (m_setRealServices) m_services[key] = (T1) (object) new T2(); // いったんobjectキャストしないと変換できない
				else throw new NullReferenceException("オブジェクトがありません：" + typeof(T1));
			}

			return (T1) m_services[key];
		}

		#endregion

		#region -CreateServicesKey サービス取得のためのキー取得

		/// <summary>
		/// サービス取得のためのキー取得
		/// </summary>
		/// <typeparam name="T1">サービスのインターフェース</typeparam>
		/// <returns>キー</returns>
		private string CreateServicesKey<T1>()
		{
			var key = typeof(T1).ToString();
			return key;
		}

		#endregion

		/// <summary>インスタンス</summary>
		public static DomainFacade Instance
		{
			get { return m_instance; }
		}
		/// <summary>クーポンサービス</summary>
		public ICouponService CouponService
		{
			get { return GetService<ICouponService, CouponService>(); }
			set { m_services[CreateServicesKey<ICouponService>()] = value; }
		}
		/// <summary>配送会社サービス</summary>
		public IDeliveryCompanyService DeliveryCompanyService
		{
			get { return GetService<IDeliveryCompanyService, DeliveryCompanyService>(); }
			set { m_services[CreateServicesKey<IDeliveryCompanyService>()] = value; }
		}
		/// <summary>定期購入情報サービス</summary>
		public IFixedPurchaseService FixedPurchaseService
		{
			get { return GetService<IFixedPurchaseService, FixedPurchaseService>(); }
			set { m_services[CreateServicesKey<IFixedPurchaseService>()] = value; }
		}
		/// <summary>定期購入継続分析テーブルサービス</summary>
		public IFixedPurchaseRepeatAnalysisService FixedPurchaseRepeatAnalysisService
		{
			get { return GetService<IFixedPurchaseRepeatAnalysisService, FixedPurchaseRepeatAnalysisService>(); }
			set { m_services[CreateServicesKey<IFixedPurchaseRepeatAnalysisService>()] = value; }
		}
		/// <summary>名称翻訳設定サービス</summary>
		public INameTranslationSettingService NameTranslationSettingService
		{
			get { return GetService<INameTranslationSettingService, NameTranslationSettingService>(); }
			set { m_services[CreateServicesKey<INameTranslationSettingService>()] = value; }
		}
		/// <summary>注文情報サービス</summary>
		public IOrderService OrderService
		{
			get { return GetService<IOrderService, OrderService>(); }
			set { m_services[CreateServicesKey<IOrderService>()] = value; }
		}
		/// <summary>Order Memo Setting Service</summary>
		public IOrderMemoSettingService OrderMemoSettingService
		{
			get { return GetService<IOrderMemoSettingService, OrderMemoSettingService>(); }
			set { m_services[CreateServicesKey<IOrderMemoSettingService>()] = value; }
		}
		/// <summary>税率毎の注文金額情報サービス</summary>
		public IOrderPriceByTaxRateService OrderPriceByTaxRateService
		{
			get { return GetService<IOrderPriceByTaxRateService, OrderPriceByTaxRateService>(); }
			set { m_services[CreateServicesKey<IOrderPriceByTaxRateService>()] = value; }
		}
		/// <summary>決済種別サービス</summary>
		public IPaymentService PaymentService
		{
			get { return GetService<IPaymentService, PaymentService>(); }
			set { m_services[CreateServicesKey<IPaymentService>()] = value; }
		}
		/// <summary>ポイントサービスクラス</summary>
		public IPointService PointService
		{
			get { return GetService<IPointService, PointService>(); }
			set { m_services[CreateServicesKey<IPointService>()] = value; }
		}
		/// <summary>商品サービス</summary>
		public IProductService ProductService
		{
			get { return GetService<IProductService, ProductService>(); }
			set { m_services[CreateServicesKey<IProductService>()] = value; }
		}
		/// <summary>商品税率カテゴリサービス</summary>
		public IProductTaxCategoryService ProductTaxCategoryService
		{
			get { return GetService<IProductTaxCategoryService, ProductTaxCategoryService>(); }
			set { m_services[CreateServicesKey<IProductTaxCategoryService>()] = value; }
		}
		/// <summary>店舗配送種別サービス</summary>
		public IShopShippingService ShopShippingService
		{
			get { return GetService<IShopShippingService, ShopShippingService>(); }
			set { m_services[CreateServicesKey<IShopShippingService>()] = value; }
		}
		/// <summary>定期購入電子発票情報サービス</summary>
		public ITwFixedPurchaseInvoiceService TwFixedPurchaseInvoiceService
		{
			get { return GetService<ITwFixedPurchaseInvoiceService, TwFixedPurchaseInvoiceService>(); }
			set { m_services[CreateServicesKey<ITwFixedPurchaseInvoiceService>()] = value; }
		}
		/// <summary>電子発票情報サービス</summary>
		public ITwInvoiceService TwInvoiceService
		{
			get { return GetService<ITwInvoiceService, TwInvoiceService>(); }
			set { m_services[CreateServicesKey<ITwInvoiceService>()] = value; }
		}
		/// <summary>注文電子発票情報サービス</summary>
		public ITwOrderInvoiceService TwOrderInvoiceService
		{
			get { return GetService<ITwOrderInvoiceService, TwOrderInvoiceService>(); }
			set { m_services[CreateServicesKey<ITwOrderInvoiceService>()] = value; }
		}
		/// <summary>更新履歴情報サービス</summary>
		public IUpdateHistoryService UpdateHistoryService
		{
			get { return GetService<IUpdateHistoryService, UpdateHistoryService>(); }
			set { m_services[CreateServicesKey<IUpdateHistoryService>()] = value; }
		}
		/// <summary>ユーザーサービス</summary>
		public IUserService UserService
		{
			get { return GetService<IUserService, UserService>(); }
			set { m_services[CreateServicesKey<IUserService>()] = value; }
		}
		/// <summary>決済カード連携サービス</summary>
		public IUserCreditCardService UserCreditCardService
		{
			get { return GetService<IUserCreditCardService, UserCreditCardService>(); }
			set { m_services[CreateServicesKey<IUserCreditCardService>()] = value; }
		}
		/// <summary>デフォルト注文方法サービス</summary>
		public IUserDefaultOrderSettingService UserDefaultOrderSettingService
		{
			get { return GetService<IUserDefaultOrderSettingService, UserDefaultOrderSettingService>(); }
			set { m_services[CreateServicesKey<IUserDefaultOrderSettingService>()] = value; }
		}
		/// <summary>ユーザー配送先サービス</summary>
		public IUserShippingService UserShippingService
		{
			get { return GetService<IUserShippingService, UserShippingService>(); }
			set { m_services[CreateServicesKey<IUserShippingService>()] = value; }
		}
		/// <summary>User Management Level Service</summary>
		public IUserManagementLevelService UserManagementLevelService
		{
			get { return GetService<IUserManagementLevelService, UserManagementLevelService>(); }
			set { m_services[CreateServicesKey<IUserManagementLevelService>()] = value; }
		}
		/// <summary>Taiwan User Invoice Service</summary>
		public ITwUserInvoiceService TwUserInvoiceService
		{
			get { return GetService<ITwUserInvoiceService, TwUserInvoiceService>(); }
			set { m_services[CreateServicesKey<ITwUserInvoiceService>()] = value; }
		}
		/// <summary>商品タグサービス</summary>
		public IProductTagService ProductTagService
		{
			get { return GetService<IProductTagService, ProductTagService>(); }
			set { m_services[CreateServicesKey<IProductTagService>()] = value; }
		}
		/// <summary>会員ランクサービス</summary>
		public IMemberRankService MemberRankService
		{
			get { return GetService<IMemberRankService, MemberRankService>(); }
			set { m_services[CreateServicesKey<IMemberRankService>()] = value; }
		}
		/// <summary>商品定期購入割引設定サービス</summary>
		public IProductFixedPurchaseDiscountSettingService ProductFixedPurchaseDiscountSettingService
		{
			get { return GetService<IProductFixedPurchaseDiscountSettingService, ProductFixedPurchaseDiscountSettingService>(); }
			set { m_services[CreateServicesKey<IProductFixedPurchaseDiscountSettingService>()] = value; }
		}
		/// <summary>モール連携設定サービス</summary>
		public IMallCooperationSettingService MallCooperationSettingService
		{
			get { return GetService<IMallCooperationSettingService, MallCooperationSettingService>(); }
			set { m_services[CreateServicesKey<IMallCooperationSettingService>()] = value; }
		}
		/// <summary>商品セット設定サービス</summary>
		public IProductSetService ProductSetService
		{
			get { return GetService<IProductSetService, ProductSetService>(); }
			set { m_services[CreateServicesKey<IProductSetService>()] = value; }
		}
		/// <summary>広告コードサービス</summary>
		public IAdvCodeService AdvCodeService
		{
			get { return GetService<IAdvCodeService, AdvCodeService>(); }
			set { m_services[CreateServicesKey<IAdvCodeService>()] = value; }
		}
		/// <summary>アフィリエイトタグ設定サービス</summary>
		public IAffiliateTagSettingService AffiliateTagSettingService
		{
			get { return GetService<IAffiliateTagSettingService, AffiliateTagSettingService>(); }
			set { m_services[CreateServicesKey<IAffiliateTagSettingService>()] = value; }
		}
		/// <summary>自動翻訳分設定サービス</summary>
		public IAutoTranslationWordService AutoTranslationWordService
		{
			get { return GetService<IAutoTranslationWordService, AutoTranslationWordService>(); }
			set { m_services[CreateServicesKey<IAutoTranslationWordService>()] = value; }
		}
		/// <summary>国ISOコードサービス</summary>
		public ICountryLocationService CountryLocationService
		{
			get { return GetService<ICountryLocationService, CountryLocationService>(); }
			set { m_services[CreateServicesKey<ICountryLocationService>()] = value; }
		}
		/// <summary>国ISOコードサービス</summary>
		public IFeatureAreaService FeatureAreaService
		{
			get { return GetService<IFeatureAreaService, FeatureAreaService>(); }
			set { m_services[CreateServicesKey<IFeatureAreaService>()] = value; }
		}
		/// <summary>ランディングページサービス</summary>
		public ILandingPageService LandingPageService
		{
			get { return GetService<ILandingPageService, LandingPageService>(); }
			set { m_services[CreateServicesKey<ILandingPageService>()] = value; }
		}
		/// <summary>新着情報サービス</summary>
		public INewsService NewsService
		{
			get { return GetService<INewsService, NewsService>(); }
			set { m_services[CreateServicesKey<INewsService>()] = value; }
		}
		/// <summary>ノベルティサービス</summary>
		public INoveltyService NoveltyService
		{
			get { return GetService<INoveltyService, NoveltyService>(); }
			set { m_services[CreateServicesKey<INoveltyService>()] = value; }
		}
		/// <summary>OGPタグ設定サービス</summary>
		public IOgpTagSettingService OgpTagSettingService
		{
			get { return GetService<IOgpTagSettingService, OgpTagSettingService>(); }
			set { m_services[CreateServicesKey<IOgpTagSettingService>()] = value; }
		}
		/// <summary>ページデザインサービス</summary>
		public IPageDesignService PageDesignService
		{
			get { return GetService<IPageDesignService, PageDesignService>(); }
			set { m_services[CreateServicesKey<IPageDesignService>()] = value; }
		}
		/// <summary>パーツデザインサービス</summary>
		public IPartsDesignService PartsDesignService
		{
			get { return GetService<IPartsDesignService, PartsDesignService>(); }
			set { m_services[CreateServicesKey<IPartsDesignService>()] = value; }
		}
		/// <summary>商品グループサービス</summary>
		public IProductGroupService ProductGroupService
		{
			get { return GetService<IProductGroupService, ProductGroupService>(); }
			set { m_services[CreateServicesKey<IProductGroupService>()] = value; }
		}
		/// <summary>商品一覧表示設定サービス</summary>
		public IProductListDispSettingService ProductListDispSettingService
		{
			get { return GetService<IProductListDispSettingService, ProductListDispSettingService>(); }
			set { m_services[CreateServicesKey<IProductListDispSettingService>()] = value; }
		}
		/// <summary>レコメンド設定サービス</summary>
		public IRecommendService RecommendService
		{
			get { return GetService<IRecommendService, RecommendService>(); }
			set { m_services[CreateServicesKey<IRecommendService>()] = value; }
		}
		/// <summary>SEOメタデータサービス</summary>
		public ISeoMetadatasService SeoMetadatasService
		{
			get { return GetService<ISeoMetadatasService, SeoMetadatasService>(); }
			set { m_services[CreateServicesKey<ISeoMetadatasService>()] = value; }
		}
		/// <summary>セットプロモーションサービス</summary>
		public ISetPromotionService SetPromotionService
		{
			get { return GetService<ISetPromotionService, SetPromotionService>(); }
			set { m_services[CreateServicesKey<ISetPromotionService>()] = value; }
		}
		/// <summary>ショートURLサービス</summary>
		public IShortUrlService ShortUrlService
		{
			get { return GetService<IShortUrlService, ShortUrlService>(); }
			set { m_services[CreateServicesKey<IShortUrlService>()] = value; }
		}
		/// <summary>Default setting service</summary>
		public IDefaultSettingService DefaultSettingService
		{
			get { return GetService<IDefaultSettingService, DefaultSettingService>(); }
			set { m_services[CreateServicesKey<IDefaultSettingService>()] = value; }
		}
		/// <summary>Global zipcode service</summary>
		public IGlobalZipcodeService GlobalZipcodeService
		{
			get { return GetService<IGlobalZipcodeService, GlobalZipcodeService>(); }
			set { m_services[CreateServicesKey<IGlobalZipcodeService>()] = value; }
		}
		/// <summary>マスタ出力定義サービス</summary>
		public IMasterExportSettingService MasterExportSettingService
		{
			get { return GetService<IMasterExportSettingService, MasterExportSettingService>(); }
			set { m_services[CreateServicesKey<IMasterExportSettingService>()] = value; }
		}
		/// <summary>サマリ分析結果テーブルサービス</summary>
		public IDispSummaryAnalysisService DispSummaryAnalysisService
		{
			get { return GetService<IDispSummaryAnalysisService, DispSummaryAnalysisService>(); }
			set { m_services[CreateServicesKey<IDispSummaryAnalysisService>()] = value; }
		}
		/// <summary>注文拡張ステータス設定サービス</summary>
		public IOrderExtendStatusSettingService OrderExtendStatusSettingService
		{
			get { return GetService<IOrderExtendStatusSettingService, OrderExtendStatusSettingService>(); }
			set { m_services[CreateServicesKey<IOrderExtendStatusSettingService>()] = value; }
		}
		/// <summary>注文拡張設定サービス</summary>
		public IOrderExtendSettingService OrderExtendSettingService
		{
			get { return GetService<IOrderExtendSettingService, OrderExtendSettingService>(); }
			set { m_services[CreateServicesKey<IOrderExtendSettingService>()] = value; }
		}
		/// <summary>注文ワークフロー設定サービス</summary>
		public IOrderWorkflowSettingService OrderWorkflowSettingService
		{
			get { return GetService<IOrderWorkflowSettingService, OrderWorkflowSettingService>(); }
			set { m_services[CreateServicesKey<IOrderWorkflowSettingService>()] = value; }
		}
		/// <summary>受注ワークフロー実行履歴サービス</summary>
		public IOrderWorkflowExecHistoryService OrderWorkflowExecHistoryService
		{
			get { return GetService<IOrderWorkflowExecHistoryService, OrderWorkflowExecHistoryService>(); }
			set { m_services[CreateServicesKey<IOrderWorkflowExecHistoryService>()] = value; }
		}
		/// <summary>定期ワークフロー設定サービス</summary>
		public IFixedPurchaseWorkflowSettingService FixedPurchaseWorkflowSettingService
		{
			get { return GetService<IFixedPurchaseWorkflowSettingService, FixedPurchaseWorkflowSettingService>(); }
			set { m_services[CreateServicesKey<IFixedPurchaseWorkflowSettingService>()] = value; }
		}
		/// <summary>メールテンプレートサービス</summary>
		public IMailTemplateService MailTemplateService
		{
			get { return GetService<IMailTemplateService, MailTemplateService>(); }
			set { m_services[CreateServicesKey<IMailTemplateService>()] = value; }
		}
		/// <summary>表示設定管理サービス</summary>
		public IManagerListDispSettingService ManagerListDispSettingService
		{
			get { return GetService<IManagerListDispSettingService, ManagerListDispSettingService>(); }
			set { m_services[CreateServicesKey<IManagerListDispSettingService>()] = value; }
		}
		/// <summary>AbTestサービス</summary>
		public IAbTestService AbTestService
		{
			get { return GetService<IAbTestService, AbTestService>(); }
			set { m_services[CreateServicesKey<IAbTestService>()] = value; }
		}
		/// <summary>後払い.com請求書サービス</summary>
		public IInvoiceAtobaraicomService InvoiceAtobaraicomService
		{
			get { return GetService<IInvoiceAtobaraicomService, InvoiceAtobaraicomService>(); }
			set { m_services[CreateServicesKey<IInvoiceAtobaraicomService>()] = value; }
		}
		/// <summary>ベリトランス請求書サービス</summary>
		public IInvoiceVeritransService InvoiceVeritransService
		{
			get { return GetService<IInvoiceVeritransService, InvoiceVeritransService>(); }
			set { m_services[CreateServicesKey<IInvoiceVeritransService>()] = value; }
		}
		/// <summary>Product brand service</summary>
		public IProductBrandService ProductBrandService
		{
			get { return GetService<IProductBrandService, ProductBrandService>(); }
			set { m_services[CreateServicesKey<IProductBrandService>()] = value; }
		}
		/// <summary>商品カテゴリサービス</summary>
		public IProductCategoryService ProductCategoryService
		{
			get { return GetService<IProductCategoryService, ProductCategoryService>(); }
			set { m_services[CreateServicesKey<IProductCategoryService>()] = value; }
		}
		/// <summary>Product default setting service</summary>
		public IProductDefaultSettingService ProductDefaultSettingService
		{
			get { return GetService<IProductDefaultSettingService, ProductDefaultSettingService>(); }
			set { m_services[CreateServicesKey<IProductDefaultSettingService>()] = value; }
		}
		/// <summary>Product Extend Setting Service</summary>
		public IProductExtendSettingService ProductExtendSettingService
		{
			get { return GetService<IProductExtendSettingService, ProductExtendSettingService>(); }
			set { m_services[CreateServicesKey<IProductExtendSettingService>()] = value; }
		}
		/// <summary>商品拡張拡張項目サービス</summary>
		public IProductExtendService ProductExtendService
		{
			get { return GetService<IProductExtendService, ProductExtendService>(); }
			set { m_services[CreateServicesKey<IProductExtendService>()] = value; }
		}
		/// <summary>商品価格サービス</summary>
		public IProductPriceService ProductPriceService
		{
			get { return GetService<IProductPriceService, ProductPriceService>(); }
			set { m_services[CreateServicesKey<IProductPriceService>()] = value; }
		}
		/// <summary>商品在庫サービス</summary>
		public IProductStockService ProductStockService
		{
			get { return GetService<IProductStockService, ProductStockService>(); }
			set { m_services[CreateServicesKey<IProductStockService>()] = value; }
		}
		/// <summary>商品在庫履歴サービス/summary>
		public IProductStockHistoryService ProductStockHistoryService
		{
			get { return GetService<IProductStockHistoryService, ProductStockHistoryService>(); }
			set { m_services[CreateServicesKey<IProductStockHistoryService>()] = value; }
		}
		/// <summary>商品在庫文言サービス</summary>
		public IProductStockMessageService ProductStockMessageService
		{
			get { return GetService<IProductStockMessageService, ProductStockMessageService>(); }
			set { m_services[CreateServicesKey<IProductStockMessageService>()] = value; }
		}
		/// <summary>商品サブ画像設定サービス</summary>
		public IProductSubImageSettingService ProductSubImageSettingService
		{
			get { return GetService<IProductSubImageSettingService, ProductSubImageSettingService>(); }
			set { m_services[CreateServicesKey<IProductSubImageSettingService>()] = value; }
		}
		/// <summary>モール出品設定サービス</summary>
		public IMallExhibitsConfigService MallExhibitsConfigService
		{
			get { return GetService<IMallExhibitsConfigService, MallExhibitsConfigService>(); }
			set { m_services[CreateServicesKey<IMallExhibitsConfigService>()] = value; }
		}
		/// <summary>Scoring sale service</summary>
		public IScoringSaleService ScoringSaleService
		{
			get { return GetService<IScoringSaleService, ScoringSaleService>(); }
			set { m_services[CreateServicesKey<IScoringSaleService>()] = value; }
		}
		/// <summary>Score axis service</summary>
		public IScoreAxisService ScoreAxisService
		{
			get { return GetService<IScoreAxisService, ScoreAxisService>(); }
			set { m_services[CreateServicesKey<IScoreAxisService>()] = value; }
		}
		/// <summary>頒布会サービス</summary>
		public ISubscriptionBoxService SubscriptionBoxService
		{
			get { return GetService<ISubscriptionBoxService, SubscriptionBoxService>(); }
			set { m_services[CreateServicesKey<ISubscriptionBoxService>()] = value; }
		}
		/// <summary>Member rank rule service</summary>
		public IMemberRankRuleService MemberRankRuleService
		{
			get { return GetService<IMemberRankRuleService, MemberRankRuleService>(); }
			set { m_services[CreateServicesKey<IMemberRankRuleService>()] = value; }
		}
		/// <summary>店舗管理者サービス</summary>
		public IShopOperatorService ShopOperatorService
		{
			get { return GetService<IShopOperatorService, ShopOperatorService>(); }
			set { m_services[CreateServicesKey<IShopOperatorService>()] = value; }
		}
		/// <summary>為替レートサービス</summary>
		public IExchangeRateService ExchangeRateService
		{
			get { return GetService<IExchangeRateService, ExchangeRateService>(); }
			set { m_services[CreateServicesKey<IExchangeRateService>()] = value; }
		}
		/// <summary>Product sale service</summary>
		public IProductSaleService ProductSaleService
		{
			get { return GetService<IProductSaleService, ProductSaleService>(); }
			set { m_services[CreateServicesKey<IProductSaleService>()] = value; }
		}
		/// <summary>Real shop service</summary>
		public IRealShopService RealShopService
		{
			get { return GetService<IRealShopService, RealShopService>(); }
			set { m_services[CreateServicesKey<IRealShopService>()] = value; }
		}
		/// <summary>Operator authority service</summary>
		public IOperatorAuthorityService OperatorAuthorityService
		{
			get { return GetService<IOperatorAuthorityService, OperatorAuthorityService>(); }
			set { m_services[CreateServicesKey<IOperatorAuthorityService>()] = value; }
		}
		/// <summary>feature page service</summary>
		public IFeaturePageService FeaturePageService
		{
			get { return GetService<IFeaturePageService, FeaturePageService>(); }
			set { m_services[CreateServicesKey<IFeaturePageService>()] = value; }
		}
		/// <summary> 海外配送エリア構成サービス </summary>
		public IGlobalShippingService GlobalShippingService
		{
			get { return GetService<IGlobalShippingService, GlobalShippingService>(); }
			set { m_services[CreateServicesKey<IGlobalShippingService>()] = value; }
		}
	}
}
