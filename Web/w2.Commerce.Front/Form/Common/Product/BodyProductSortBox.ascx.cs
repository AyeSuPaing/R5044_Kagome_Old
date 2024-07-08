/*
=========================================================================================================
  Module      : 商品一覧ソートリンク出力コントローラ処理(BodyProductSortBox.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using ProductListDispSetting;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.ProductListDispSetting;

public partial class Form_Common_Product_BodyProductSortBox : ProductUserControl
{
	#region ラップ済みコントロール宣言
	protected WrappedRepeater WrSortList { get { return GetWrappedControl<WrappedRepeater>("rSortList"); } }
	protected WrappedRepeater WrImgList { get { return GetWrappedControl<WrappedRepeater>("rImgList"); } }
	protected WrappedRepeater WrStockList { get { return GetWrappedControl<WrappedRepeater>("rStockList"); } }
	protected WrappedRepeater WrNumberDisplayLinks { get { return GetWrappedControl<WrappedRepeater>("rNumberDisplayLinks"); } }
	#endregion

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Init(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 基底メソッドコール
		//------------------------------------------------------
		base.Page_Init(sender, e);

		//------------------------------------------------------
		// 商品一覧表示設定取得
		//------------------------------------------------------
		this.ProductListDispSetting = DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData;
	}
	
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.ProductListDispSetting = NameTranslationCommon.SetProductListDispSettingTranslationData(
					this.ProductListDispSetting,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);
			}

			this.WrSortList.DataSource = ProductListDispSettingUtility.GetSortSetting(this.ProductListDispSetting);
			this.WrSortList.DataBind();

			this.WrImgList.DataSource = ProductListDispSettingUtility.GetImgSetting(this.ProductListDispSetting);
			this.WrImgList.DataBind();

			this.WrStockList.DataSource = ProductListDispSettingUtility.GetStockSetting(this.ProductListDispSetting);
			this.WrStockList.DataBind();

			this.WrNumberDisplayLinks.DataSource = ProductListDispSettingUtility.GetCountSetting(this.ProductListDispSetting);
			this.WrNumberDisplayLinks.DataBind();
		}
	}

	/// <summary>
	/// ソートURL作成
	/// </summary>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>ソートURL</returns>
	protected string CreateSortUrl(string strSortKbn)
	{
		return CreateUrl(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);
	}

	/// <summary>
	/// 画像表示種別URL作成
	/// </summary>
	/// <param name="strImageDispType"></param>
	/// <returns>画像表示種別URL</returns>
	protected string CreateImageDispTypeUrl(string strImageDispType)
	{
		return CreateUrl(Constants.REQUEST_KEY_DISP_IMG_KBN, strImageDispType);
	}

	/// <summary>
	/// 商品一覧件数表示URL作成
	/// </summary>
	/// <param name="iDisplayCount">表示件数</param>
	/// <returns>商品一覧件数表示URL</returns>
	protected string CreateDisplayCountUrl(int iDisplayCount)
	{
		return CreateUrl(Constants.REQUEST_KEY_DISP_PRODUCT_COUNT, iDisplayCount.ToString());
	}

	/// <summary>
	/// 在庫なし表示/非表示URL作成
	/// </summary>
	/// <param name="unDispNostock">表示非表示区分</param>
	/// <returns>商品一覧URL</returns>
	protected string CreateDisplayStockUrl(string unDispNostock)
	{
		return CreateUrl(Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT, unDispNostock);
	}

	/// <summary>
	/// 定期購入フィルタURL作成
	/// </summary>
	/// <param name="fixedPurchaseFilter">定期購入フィルタ区分</param>
	/// <returns>商品一覧URL</returns>
	protected string CreateFixedPurchaseFilterUrl(string fixedPurchaseFilter)
	{
		return CreateUrl(Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER, fixedPurchaseFilter);
	}

	/// <summary>
	/// 商品一覧URL作成
	/// ソート、画像表示、表示件数、在庫の共通化
	/// </summary>
	/// <param name="requestKey">リクエストキー</param>
	/// <param name="requestKeyValue">格納する値</param>
	/// <returns>商品一覧URL</returns>
	private string CreateUrl(string requestKey, string requestKeyValue)
	{
		Dictionary<string, string> urlParameter = new Dictionary<string, string>(this.RequestParameter);
		// パラメーター削除
		urlParameter.Remove(requestKey);
		urlParameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
		urlParameter.Remove(ProductCommon.URL_KEY_CATEGORY_NAME);

		// パラメーターセット
		// フレンドリURL用のカテゴリ名は、このタイミングでしか渡せない。
		urlParameter.Add(requestKey, requestKeyValue);
		urlParameter.Add(Constants.REQUEST_KEY_PAGE_NO, "1"); // ソート変更時は1ページ目に戻す
		urlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, this.CategoryName);
		// URL作成
		return string.IsNullOrEmpty(this.TargetUrl)
			? ProductCommon.CreateProductListUrl(urlParameter)
			: ProductCommon.CreateProductListUrl(urlParameter, this.TargetUrl);
	}

	/// <summary>商品一覧表示設定</summary>
	protected ProductListDispSettingModel[] ProductListDispSetting { get; private set; }
	/// <summary>カテゴリ名（外部設定）</summary>
	public string CategoryName { get; set;}
	/// <summary>遷移先Url</summary>
	public string TargetUrl { get; set; }
	/// <summary>並び替えのみ</summary>
	public bool IsSortOnly { get; set; }
	/// <summary>表示件数選択肢を表示するか</summary>
	protected bool DisplayCountFilter
	{
		get
		{
			return (ProductListDispSettingUtility.CountSetting.Length > 1) && (this.IsSortOnly == false);
		}
	}
	/// <summary>表示切り替え選択肢を表示するか</summary>
	protected bool DisplayChangeFilter
	{
		get
		{
			return (ProductListDispSettingUtility.ImgSetting.Length > 1) && (this.IsSortOnly == false);
		}
	}
	/// <summary>在庫選択肢を表示するか</summary>
	protected bool DisplayStockFilter
	{
		get
		{
			return (ProductListDispSettingUtility.StockSetting.Length > 1) && (this.IsSortOnly == false);
		}
	}
	/// <summary>通常・定期選択肢を表示するか</summary>
	protected bool DisplayFixedPurchaseFilter
	{
		get
		{
			return Constants.FIXEDPURCHASE_OPTION_ENABLED && (this.IsSortOnly == false);
		}
	}
}
