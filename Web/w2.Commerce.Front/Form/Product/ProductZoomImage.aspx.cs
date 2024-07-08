/*
=========================================================================================================
  Module      : 商品詳細画面処理(ProductZoomImage.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using w2.App.Common.Order;
using w2.App.Common.Preview;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Product_ProductZoomImage : ProductPage
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrVariation { get { return GetWrappedControl<WrappedRepeater>(this.Form, "rVariation"); } }
	WrappedRepeater WrSubImage { get { return GetWrappedControl<WrappedRepeater>(this.Form, "rSubImage"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 商品画像情報セット
			//------------------------------------------------------
			this.ProductImageInfo = new Hashtable();
			this.ProductImageInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD, Request["ihead"]);
			this.ProductImageInfo.Add(Constants.FIELD_PRODUCTVARIATION_SHOP_ID, Request[Constants.REQUEST_KEY_SHOP_ID]);

			// サブ画像No
			int subImageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PRODUCT_SUBIMAGE_NO], out subImageNo) == false)
			{
				subImageNo = 0;
			}
			this.SubImageNo = subImageNo;

			if (this.SubImageNo == 0) this.ProductImageInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, (string)Request[Constants.REQUEST_KEY_PRODUCT_ID]);

			//------------------------------------------------------
			// 商品画像出力リスト作成
			//------------------------------------------------------
			var lProductSubImage = new List<DataRowView>();
			var productId = (string)Request[Constants.REQUEST_KEY_PRODUCT_ID];
			var dvProduct = this.IsPreviewProductMode
				? ProductPreview.GetProductDetailPreview(Constants.CONST_DEFAULT_SHOP_ID, productId)
				: ProductCommon.GetProductInfo(this.ShopId, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
			if (dvProduct.Count > 0)
			{
				// メイン画像ヘッダ取得
				this.ImageHead = (string)dvProduct[0][Constants.FIELD_PRODUCT_IMAGE_HEAD];

				// サブ画像設定取得＆データバインド（存在するもののみ）
				DataView dvProductSubImageSettings = ProductCommon.GetProductSubImageSettingList(this.ShopId);
				if (dvProductSubImageSettings.Count > 0)
				{
					// サブ画像の情報設定
					List<DataRowView> lProductSubImageListTmp = new List<DataRowView>();
					foreach (DataRowView drvProductSubImage in dvProductSubImageSettings)
					{
						if (CheckProductSubImageExist(dvProduct[0], (int)drvProductSubImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO]))
						{
							lProductSubImageListTmp.Add(drvProductSubImage);
						}
					}

					// メイン画像の情報設定（メイン画像をデフォルト表示する）
					DataRowView drvMainImage = dvProductSubImageSettings.AddNew();
					drvMainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO] = Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1; // 商品サブ画像Noの上限値よりも+1大きい商品サブ画像Noの時はメイン画像として扱う
					drvMainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME] = ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_PRODUCT_DETAIL,
						Constants.VALUETEXT_PARAM_PRODUCT_DATA_SETTING_MESSAGE,
						Constants.VALUETEXT_PARAM_PRODUCT_DATA_MAIN_IMAGE);

					// メイン・サブ画像をリストへ追加
					lProductSubImage.Add(drvMainImage);
					foreach (DataRowView drvProductSubImage in lProductSubImageListTmp)
					{
						lProductSubImage.Add(drvProductSubImage);
					}
				}

				// 対象商品のバリエーション画像一覧セット
				var groupedVariationList = GetGroupedVariationList(
					this.IsPreviewProductMode
						? dvProduct
						: GetVariationList(dvProduct, this.MemberRankId));
				this.WrVariation.DataSource =
					((groupedVariationList != null) && groupedVariationList.ContainsKey(productId))
						? groupedVariationList[productId]
						: null;

				// サブ画像一覧セット
				this.WrSubImage.DataSource = lProductSubImage;
			}

			//------------------------------------------------------
			// 全体データバインド
			//------------------------------------------------------
			this.DataBind();
		}
	}

	/// <summary>
	/// 商品バリエーション画像パス生成
	/// </summary>
	/// <param name="value">商品バリエーション情報</param>
	/// <param name="imageFooter">画像フッタ</param>
	protected string CreateVariationImagePath(object value, string imageFooter)
	{
		var productImagePath = new StringBuilder(Constants.PATH_ROOT);
		var isPreviewMode = (string.IsNullOrEmpty(PreviewGuidString) == false);
		var imageFileNameHead = (string)ProductPage.GetKeyValue(value, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD);
		if (isPreviewMode)
		{
			// For the case preview product from manager site
			productImagePath.AppendFormat(
				"{0}ProductImages/{1}/{2}{3}",
				Constants.PATH_TEMP,
				PreviewGuidString,
				imageFileNameHead,
				Constants.PRODUCTIMAGE_FOOTER_LL);
		}
		else
		{
			productImagePath.AppendFormat(
				"{0}{1}/{2}{3}",
				Constants.PATH_PRODUCTIMAGES,
				this.ShopId,
				imageFileNameHead,
				imageFooter);
		}

		if (File.Exists(Server.MapPath(productImagePath.ToString())) == false)
		{
			return SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.PATH_PRODUCTIMAGES + Constants.PRODUCTIMAGE_NOIMAGE_HEADER + imageFooter);
		}

		return EncodeImageUrl(productImagePath.ToString(), imageFileNameHead);
	}

	/// <summary>
	/// 商品バリエーション情報の値取得
	/// </summary>
	/// <param name="objValue">商品バリエーション情報</param>
	/// <param name="strKey">参照キー</param>
	protected string GetVariationValue(object objValue, string strKey)
	{
		return (string)ProductPage.GetKeyValue(objValue, strKey);
	}

	/// <summary>商品画像情報</summary>
	protected Hashtable ProductImageInfo { get; set; }
	/// <summary>画像ヘッダ</summary>
	protected string ImageHead { get; set; }
	/// <summary>サブ画像No</summary>
	protected int SubImageNo { get; set; }
}