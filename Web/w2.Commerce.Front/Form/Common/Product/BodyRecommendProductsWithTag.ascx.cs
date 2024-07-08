/*
=========================================================================================================
  Module      : 関連タグ・おすすめ商品出力コントローラ処理(BodyRecommendProductsWithTag.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Awoo;
using w2.App.Common.Awoo.GetTags;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain.Product;

public partial class Form_Common_Product_BodyRecommendProductsWithTag : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// Awoo連携オプションOFFの場合何も表示しない
			if (Constants.AWOO_OPTION_ENABLED == false) return;

			var response = GetAwooRecommendData();
			if (response == null) return;

			// おすすめタグを設定
			this.TagList = response.Result.Tags;

			// レスポンスのおすすめ商品IDを使って表示情報をDBから取得
			var productIds = Array.Empty<string>();
			if (response.Result.Products != null)
			{
				var idsV = response.Result.Products.DirectionsV != null
					? response.Result.Products.DirectionsV.Products.Select(product => product.ProductId)
					: Array.Empty<string>();
				var idsH = response.Result.Products.DirectionsH != null
					? response.Result.Products.DirectionsH.Products.Select(product => product.ProductId)
					: Array.Empty<string>();
				productIds = idsV.Union(idsH).ToArray();
			}
			if (productIds.Any())
			{
				var info = GetDatas(productIds);
				this.ProductMasterList = info.Item1;
				this.ProductVariationList = info.Item2;
			}

			this.DataBind();
		}
	}

	/// <summary>
	/// Awoo TagAPIのデータを取得
	/// </summary>
	/// <returns>Awoo API レスポンス</returns>
	private GetTagsResponse GetAwooRecommendData()
	{
		var directions = Constants.AWOO_RECOMMEND_DIRECTION.Select(
			d =>
			{
				RecommendDirectionType direction;
				EnumHelper.TryParseToEnum(d, out direction);
				return direction;
			}).ToArray();
		var selectContents = Array.Empty<SelectRecommendInfoType>();
		switch (this.DisplayKbn)
		{
			case Constants.FLG_AWOO_PRODUCT_RECOMMEND_KBN_ALL:
				selectContents = new[] { SelectRecommendInfoType.Tags, SelectRecommendInfoType.Products };
				break;

			case Constants.FLG_AWOO_PRODUCT_RECOMMEND_KBN_PRODUCT:
				selectContents = new[] { SelectRecommendInfoType.Products };
				break;

			case Constants.FLG_AWOO_PRODUCT_RECOMMEND_KBN_TAG:
				selectContents = new[] { SelectRecommendInfoType.Tags };
				break;

			default:
				break;
		}
		var response = AwooApiFacade.GetTags(
			this.ProductId,
			new GetTagsRequest()
			{
				Directions = directions.Any() ? directions : new[] { RecommendDirectionType.Vertical },
				Select = selectContents,
				Limit = Constants.AWOO_PRODUCT_DETAIL_PRODUCT_LIMIT,
			});

		return response;
	}

	/// <summary>
	/// DBからデータ取得
	/// </summary>
	/// <param name="productIds">商品ID</param>
	/// <returns>商品マスタとバリエーションデータ</returns>
	private Tuple<DataView, Dictionary<string, List<DataRowView>>> GetDatas(string[] productIds)
	{
		var datas = new ProductService().GetProductForAwooProductSync(
			Constants.CONST_DEFAULT_SHOP_ID,
			this.BrandId,
			this.MemberRankId,
			this.UserFixedPurchaseMemberFlg,
			productIds);

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

	/// <summary>
	/// おすすめタグリンクURLを作成
	/// </summary>
	/// <param name="tag">タグ</param>
	/// <returns>おすすめタグリンクURL</returns>
	protected string CreateRecommendProductsUrl(Tags tag)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_RECOMMEND_PRODUCTS_LIST)
			.AddParam(Constants.REQUEST_KEY_TAGS, tag.Link)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, "1")
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="product">商品マスタ</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object product)
	{
		return ProductCommon.CreateProductDetailUrlUseProductCategoryx(product, "", this.BrandId);
	}

	/// <summary>商品ID</summary>
	public string ProductId { get; set; }
	/// <summary>表示区分(TAG:関連タグ表示 PRODUCT:おすすめ商品表示 ALL:どちらも表示)</summary>
	public string DisplayKbn { get; set; }
	/// <summary>商品画像サイズ</summary>
	public string ImageSize { get; set; }
	/// <summary>商品バリエーションを表示するか</summary>
	public bool VariationDisplayEnabled
	{
		get { return (bool?)ViewState["VariationDisplayEnabled"] ?? Constants.PRODUCT_RECOMMEND_VARIATION_DISPLAY_ENABLED; }
		set { ViewState["VariationDisplayEnabled"] = value; }
	}
	/// <summary>おすすめタグリスト</summary>
	protected List<Tags> TagList { get; set; }
	/// <summary>おすすめ商品マスタリスト</summary>
	protected DataView ProductMasterList { get; set; }
	/// <summary>商品画像用バリエーションリスト</summary>
	protected Dictionary<string, List<DataRowView>> ProductVariationList { get; set; }
}
