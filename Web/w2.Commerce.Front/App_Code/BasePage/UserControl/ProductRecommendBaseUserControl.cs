/*
=========================================================================================================
  Module      : おすすめ商品基底ユーザコントロール(ProductRecommendBaseUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Product;

/// <summary>
/// おすすめ商品基底ユーザコントロール
/// </summary>
public abstract class ProductRecommendBaseUserControl : ProductUserControl
{
	#region ラップ済コントロール宣言
	protected WrappedRepeater WrProducts { get { return GetWrappedControl<WrappedRepeater>("rProducts"); } }
	protected WrappedRepeater WrProductsTableParent { get { return GetWrappedControl<WrappedRepeater>("rProductsTableParent"); } }
	# endregion

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	protected ProductRecommendBaseUserControl()
	{
		this.UseCategory = false;
		this.MaxDispCount = 5;
		this.CategoryIdLength = Constants.CONST_CATEGORY_ID_LENGTH;
		this.TableDispEnable = false;
		this.TableDispItemRowCount = 1;
		this.ShowOutOfStock = Constants.SHOW_OUT_OF_STOCK_ITEMS;
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// パラメタ取得
			GetParams();

			// 表示用商品取得
			var dispData = GetDispData();

			// データセット
			this.ProductCount = dispData.Item1.Count;
			SetRepeaterDataSources(dispData.Item1);
			this.ProductVariationList = dispData.Item2;
			this.DataBind();
		}
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	protected virtual void GetParams()
	{
		// 対象カテゴリ決定（カテゴリ指定がない場合は空）
		if (this.UseCategory)
		{
			var val = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CATEGORY_ID]);
			this.CategoryId = (val.Length > this.CategoryIdLength) ? val.Substring(0, this.CategoryIdLength) : val;
		}
		else
		{
			this.CategoryId = "";
		}
	}

	/// <summary>
	/// 表示データ取得
	/// </summary>
	/// <returns></returns>
	private Tuple<List<DataRowView>, Dictionary<string, List<DataRowView>>> GetDispData()
	{
		// キャッシュキー取得
		var cacheKey = GetCacheKey();

		// 商品・バリエーション情報取得
		var productDatas = GetDataFromCacheOrDb(cacheKey);
		var products = productDatas.Item1;
		var variations = productDatas.Item2;

		// 商品表示情報作成
		var productsRandom = SortProducts(products);
		var dipsData = CreateProductAndVariationsForDiplay(productsRandom, variations);
		return dipsData;
	}

	/// <summary>
	/// キャッシュキー取得
	/// </summary>
	/// <returns>キャッシュキー</returns>
	protected abstract string GetCacheKey();

	/// <summary>
	/// キャッシュ有効期限(分)を取得
	/// </summary>
	/// <returns>キャッシング時間</returns>
	protected abstract int GetCacheExpireMinutes();

	/// <summary>
	/// 商品リストソート
	/// </summary>
	/// <param name="products">商品リスト</param>
	/// <returns>ソートした商品リスト</returns>
	protected abstract DataView SortProducts(DataView products);

	/// <summary>
	/// キャッシュからデータを取得する
	/// </summary>
	/// <param name="cacheKey">キャッシュキー</param>
	/// <returns>商品データ群（Item1：商品リスト、Item2：バリエーションリスト）</returns>
	private Tuple<DataView, Dictionary<string, List<DataRowView>>> GetDataFromCacheOrDb(string cacheKey)
	{
		var refreshFileManager = RefreshFileManagerProvider.GetInstance(RefreshFileType.DisplayProduct);
		var cacheDependency = new CacheDependency(refreshFileManager.RefreshFilePath);

		var data = DataCacheManager.GetInstance().GetData(
			cacheKey,
			GetCacheExpireMinutes(),
			GetDatasFromDb,
			cacheDependency);
		return data;
	}

	/// <summary>
	/// DBからデータ取得
	/// </summary>
	private Tuple<DataView, Dictionary<string, List<DataRowView>>> GetDatasFromDb()
	{
		var datas = GetDatasFromDbInner();
		var groupedVariationList = this.VariationDisplayEnabled
			? ProductPage.GetGroupedVariationList(ProductPage.GetVariationList(datas, this.MemberRankId)) 
			: null;

		// 翻訳情報設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			datas = SetProductTranslationData(datas);
		}

		return new Tuple<DataView, Dictionary<string, List<DataRowView>>>(datas, groupedVariationList ?? new Dictionary<string, List<DataRowView>>());
	}

	/// <summary>
	/// データ取得
	/// </summary>
	protected abstract DataView GetDatasFromDbInner();

	/// <summary>
	/// 表示用商品・バリエーション対象リスト作成
	/// </summary>
	/// <param name="products">ランダム商品リスト</param>
	/// <param name="variations">バリエーションリスト</param>
	/// <returns>表示用商品リスト、表示用商品バリエーションリストのペア</returns>
	private Tuple<List<DataRowView>, Dictionary<string, List<DataRowView>>> CreateProductAndVariationsForDiplay(DataView products, Dictionary<string, List<DataRowView>> variations)
	{
		var productDisplays = new List<DataRowView>();
		var productVariationDisplays = new Dictionary<string, List<DataRowView>>();

		// 表示データ件数分取得
		int dispCounter = 0;
		for (int i = 0; ((i < products.Count) && (dispCounter < this.MaxDispCount)); i++)
		{
			// 閲覧可能な会員ランクチェク
			var dispRank = (string)products[i][Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK];
			if (MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, dispRank))
			{
				productDisplays.Add(products[i]);
				if (HasVariation(products[i])
					&& (variations.ContainsKey((string)(products[i][Constants.FIELD_PRODUCT_PRODUCT_ID])))
					&& (productVariationDisplays.ContainsKey((string)(products[i][Constants.FIELD_PRODUCT_PRODUCT_ID])) == false)
					)
				{
					productVariationDisplays.Add(
						(string)(products[i][Constants.FIELD_PRODUCT_PRODUCT_ID]),
						variations[(string)(products[i][Constants.FIELD_PRODUCT_PRODUCT_ID])]);
				}
				dispCounter++;
			}
		}
		return new Tuple<List<DataRowView>, Dictionary<string, List<DataRowView>>>(
			productDisplays,
			productVariationDisplays);
	}
	
	/// <summary>
	/// Reperterデータソース設定
	/// </summary>
	/// <param name="displayProducts">商品リストデータ</param>
	protected void SetRepeaterDataSources(List<DataRowView> displayProducts)
	{
		// テーブ表示しない場合のデータソースセットして処理を抜ける
		if (this.TableDispEnable == false)
		{
			this.WrProducts.DataSource = displayProducts;
			this.WrProductsTableParent.Visible = false;
			return;
		}

		// テーブル表示する場合のデータソースセット
		if (displayProducts.Count > 0)
		{
			var productTables = CreateProductDatasourceForTable(displayProducts).ToArray();
			this.WrProductsTableParent.DataSource = productTables;
		}
		this.WrProducts.Visible = false;
	}

	/// <summary>
	/// テーブル表示用商品データソース作成
	/// </summary>
	/// <param name="displayProducts">表示用商品</param>
	/// <returns></returns>
	private IEnumerable<List<DataRowView>> CreateProductDatasourceForTable(List<DataRowView> displayProducts)
	{
		// 親の設定
		// 親には行数を持たせる
		var parentCount = displayProducts.Count / this.TableDispItemRowCount +
		                  (((displayProducts.Count % this.TableDispItemRowCount) > 0) ? 1 : 0);
		var parents = Enumerable.Range(0, parentCount - 1).ToList();

		// 子の設定
		foreach (int rownum in parents)
		{
			var tableChilds = new List<DataRowView>();
			int count = 0;
			foreach (var drv in displayProducts)
			{
				if ((count < displayProducts.Count)
				    && (count >= rownum * this.TableDispItemRowCount)
				    && (count < ((rownum + 1) * this.TableDispItemRowCount)))
				{
					tableChilds.Add(drv);
				}
				count++;
			}
			yield return tableChilds;
		}
	}

	/// <summary>
	/// 商品翻訳情報設定
	/// </summary>
	/// <param name="productData">商品情報</param>
	/// <returns>翻訳後商品リスト</returns>
	private DataView SetProductTranslationData(DataView productData)
	{
		var products = productData.Cast<DataRowView>().Select(
			drv => new ProductModel
			{
				ProductId = (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]
			}).ToArray();

		productData = (DataView)NameTranslationCommon.Translate(productData, products);
		return productData;
	}

	#region プロパティ
	/// <summary>リクエストのカテゴリIDを利用するか（外部から設定可能）</summary>
	public bool UseCategory { get; set; }
	/// <summary>商品最大表示数（外部から設定可能）</summary>
	public int MaxDispCount { get; set; }
	/// <summary>カテゴリID有効桁（外部から設定可能）</summary>
	public int CategoryIdLength { get; set; }
	/// <summary>商品画像サイズ（外部から設定可能）</summary>
	public string ImageSize { get; set; }
	/// <summary>商品数</summary>
	public int ProductCount
	{
		get { return (int)ViewState["ProductCount"]; }
		set { ViewState["ProductCount"] = value; }
	}
	/// <summary>表示用商品バリエーションリスト</summary>
	protected Dictionary<string, List<DataRowView>> ProductVariationList { get; set; }
	/// <summary>テーブルタグ表示有無（外部から設定可能）</summary>
	public bool TableDispEnable { get; set; }
	/// <summary>テーブルタグ1行あたりの表示件数（外部から設定可能）</summary>
	public int TableDispItemRowCount { get; set; }
	/// <summary>在庫切れ商品を表示するかどうか</summary>
	protected bool ShowOutOfStock { get; set; }
	/// <summary>商品バリエーションを表示するか</summary>
	public bool VariationDisplayEnabled
	{ 
		get{ return (bool?)ViewState["VariationDisplayEnabled"] ?? Constants.PRODUCT_RECOMMEND_VARIATION_DISPLAY_ENABLED; }
		set{ ViewState["VariationDisplayEnabled"] = value; } 
	}
	#endregion
}