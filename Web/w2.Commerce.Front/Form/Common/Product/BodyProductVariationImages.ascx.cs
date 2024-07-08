/*
=========================================================================================================
  Module      : 商品バリエーション画像レイヤー出力コントローラ(BodyProductVariationImages.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using w2.App.Common.Order;
using w2.Common.Web;

public partial class Form_Common_Product_BodyProductVariationImages : ProductUserControl
{
	/// <summary>
	/// 商品一覧対象の全商品バリエーション情報リスト取得
	/// </summary>
	/// <returns>商品バリエーション情報</returns>
	protected List<DataRowView> CreateProductVariationMaster()
	{
		if (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED == false) return new List<DataRowView>();
		
		// 商品一覧対象の全商品バリエーション情報リストから商品情報を取得する。
		string productId = (string)ProductPage.GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_PRODUCT_ID);
		if (((Dictionary<string, List<DataRowView>>) this.VariationList).ContainsKey(productId))
		{
			return ((Dictionary<string, List<DataRowView>>)this.VariationList)[productId];
		}
		// 商品一覧対象の全商品バリエーション情報リストに存在しない場合は、商品マスタから商品情報を取得する。
		else
		{
			List<DataRowView> produtMasterList = new List<DataRowView>();
			produtMasterList.Add((DataRowView)this.ProductMaster);
			return produtMasterList;
		}
	}

	/// <summary>
	/// 商品詳細URL作成（バリエーション選択状態）
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailSelectedVariationUrl(object objValue)
	{
		string brandId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_BRAND_ID]);
		if (brandId == "")
		{
			brandId = (string)ProductPage.GetKeyValue((DataRowView)this.ProductMaster, Constants.FIELD_PRODUCT_BRAND_ID1);
		}

		return ProductCommon.CreateProductDetailUrl(
				(string)ProductPage.GetKeyValue((DataRowView)this.ProductMaster, Constants.FIELD_PRODUCT_SHOP_ID),
				(string)Request[Constants.REQUEST_KEY_CATEGORY_ID],
				brandId,
				(string)Request[Constants.REQUEST_KEY_SEARCH_WORD],
				(string)ProductPage.GetKeyValue((DataRowView)this.ProductMaster, Constants.FIELD_PRODUCT_PRODUCT_ID),
				(string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID),
				(string)ProductPage.GetKeyValue((DataRowView)this.ProductMaster, Constants.FIELD_PRODUCT_NAME),
				w2.App.Common.Product.ProductBrandUtility.GetProductBrandName(brandId));
	}

	/// <summary>
	/// Alt文字作成
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <returns>Altタグ用文字列</returns>
	protected string CreateAltString(object objValue)
	{
		if (this.CheckVariation(objValue))
		{
			return ProductPage.CreateProductJointName(
							(string)ProductPage.GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_NAME),
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1),
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2),
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3));
		}
		else
		{
			return (string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCT_NAME);
		}
	}

	/// <summary>
	/// IMGタグのID作成
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <returns>IMGタグのID</returns>
	protected string CreateImageTagId(object objValue)
	{
		return "imgProductImage_" + (string)ProductPage.GetKeyValue(objValue, this.CheckVariation(objValue) ? Constants.FIELD_PRODUCTVARIATION_VARIATION_ID : Constants.FIELD_PRODUCT_PRODUCT_ID);
	}

	/// <summary>
	/// バリエーションID作成
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <returns>バリエーションID</returns>
	protected string CreateVariationId(object objValue)
	{
		return (string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID);
	}

	/// <summary>
	/// バリエーション使用フラグ
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <returns>バリエーションID</returns>
	protected bool IsUseVariation()
	{
		return HasVariation(this.ProductMaster);
	}

	/// <summary>
	/// ホバー表示を行うか
	/// </summary>
	/// <returns>True:行う</returns>
	protected bool IsUseHover()
	{
		return (HasVariation(this.ProductMaster)) && (HasVariationId() == false);
	}

	/// <summary>
	/// バリエーションIDが存在するか
	/// </summary>
	/// <returns>True:存在する</returns>
	protected bool HasVariationId()
	{
		return ((String.IsNullOrEmpty((string)GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) == false));
	}
	
	/// <summary>
	/// バリエーション情報存在判定
	/// </summary>
	/// <param name="objValue">お気に入り商品情報</param>
	/// <returns>バリエーション情報存在有無</returns>
	private bool CheckVariation(object objValue)
	{
		return ((this.IsUseHover()) && (String.IsNullOrEmpty((string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) != true));
	}
	/// <summary>
	/// バリエーション情報存在判定
	/// </summary>
	/// <param name="objValue">お気に入り商品情報</param>
	/// <param name="variationFavoriteCorrespondence">SKU単位でのお気に入りに対応しているか</param>
	/// <returns>バリエーション情報存在有無</returns>
	private bool CheckVariation(object objValue, bool variationFavoriteCorrespondence)
	{
		return (this.IsUseVariation() && ((string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) != null));
	}

	/// <summary>
	/// 画像URL作成
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <param name="variationFavoriteCorrespondence">SKU単位でのお気に入りに対応しているか</param>
	/// <returns>画像URL</returns>
	protected string CreateImageUrl(object objValue, bool variationFavoriteCorrespondence = false)
	{
		// ファイル名ヘッダ決定
		string strImageFileNameHead = null;
		var useVariationFavoriteCorrespondence = (variationFavoriteCorrespondence)
					? this.CheckVariation(objValue)
					: this.CheckVariation(objValue, variationFavoriteCorrespondence);

		// URL取得
		if (useVariationFavoriteCorrespondence)
		{
			strImageFileNameHead = (String.IsNullOrEmpty((string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD)) == false)
				? ((string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD) == null)
					? (string)ProductPage.GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_IMAGE_HEAD)
					: (string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD)
				: (string)ProductPage.GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_IMAGE_HEAD);
		}
		else
		{
			strImageFileNameHead = (((string)ProductPage.GetKeyValueToNull(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD)) == null)
				? (string)ProductPage.GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_IMAGE_HEAD)
				: (string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD);
		}

		// ファイル名フッタ決定
		string strFileNameFoot = null;
		switch (StringUtility.ToEmpty(this.ImageSize).ToUpper())
		{
			case "S":
				strFileNameFoot = Constants.PRODUCTIMAGE_FOOTER_S;
				break;

			case "M":
				strFileNameFoot = Constants.PRODUCTIMAGE_FOOTER_M;
				break;

			case "L":
				strFileNameFoot = Constants.PRODUCTIMAGE_FOOTER_L;
				break;

			case "LL":
				strFileNameFoot = Constants.PRODUCTIMAGE_FOOTER_LL;
				break;

			default:
				strFileNameFoot = Constants.PRODUCTIMAGE_FOOTER_M;	// デフォルトはM
				break;
		}

		// 画像URL作成
		StringBuilder sbImageFilePath = new StringBuilder();
		if (strImageFileNameHead.Contains(Uri.SchemeDelimiter))
		{
			// 外部画像URLの場合はスキーマをリプレース
			sbImageFilePath.Append(strImageFileNameHead.Replace(strImageFileNameHead.Substring(0, strImageFileNameHead.IndexOf(Uri.SchemeDelimiter)), HttpContext.Current.Request.Url.Scheme)).Append(strFileNameFoot);
		}
		else
		{
			sbImageFilePath.Append(Constants.PATH_ROOT).Append(Constants.PATH_PRODUCTIMAGES).Append((string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCT_SHOP_ID)).Append("/").Append(strImageFileNameHead).Append(strFileNameFoot);
			if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(sbImageFilePath.ToString())) == false)
			{
				// 画像無しの場合はNOIMAGE画像
				sbImageFilePath = new StringBuilder();
				sbImageFilePath.Append(SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.PATH_PRODUCTIMAGES + Constants.PRODUCTIMAGE_NOIMAGE_HEADER + strFileNameFoot));
			}
		}

		return GetEncodeImageUrl(sbImageFilePath.ToString(), strImageFileNameHead);
	}

	/// <summary>
	/// 画像URLをエンコードする
	/// </summary>
	/// <param name="url">URL</param>
	/// <param name="imageHead">画像ヘッダ名</param>
	/// <returns>エンコードされた画像URL</returns>
	private static string GetEncodeImageUrl(string url, string imageHead)
	{
		var result = (string.IsNullOrEmpty(imageHead) == false)
			? url.Replace(imageHead, HttpUtility.UrlEncode(imageHead))
			: url;
		return result;
	}

	/// <summary>商品マスタ（外部必須設定）</summary>
	public object ProductMaster { get; set; }
	/// <summary>商品一覧対象の全商品バリエーション情報リスト（外部必須設定）</summary>
	public object VariationList { get; set; }
	/// <summary>商品画像サイズ（外部設定）</summary>
	public string ImageSize { get; set; }
	/// <summary>バリエーションNo（外部必須設定）</summary>
	public string VariationNo
	{
		get { return this.m_strVariationNo; }
		set { m_strVariationNo = value + this.ClientID; }
	}
	private string m_strVariationNo = "";
	/// <summary> カラーで検索しているかどうか判断 </summary>
	protected bool IsColorVriationDisplay
	{
		get
		{
			return (CheckVariation(this.ProductMaster)
				&& (string.IsNullOrEmpty(this.RequestParameter[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]) == false));
		}
	}

}
