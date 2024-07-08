/*
=========================================================================================================
  Module      : 商品サブ画像画面処理(ProductDetailSubImage.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.App.Common.Order;

public partial class Form_Product_ProductDetailSubImage : ProductPage
{
	#region ラップ済コントロール宣言
	# endregion
	
	protected const int SUB_IMAGE_DEFAULT_NO = 0;		// サブ画像番号デフォルト値

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
			// パラメタ取得
			//------------------------------------------------------
			// 共通パラメタ取得
			GetParameters();

			// サブ画像NO取得
			int subImageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PRODUCT_SUBIMAGE_NO], out subImageNo) == false)
			{
				subImageNo = SUB_IMAGE_DEFAULT_NO;
			}
			this.SubImageNo = subImageNo;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				//------------------------------------------------------
				// 商品情報取得
				//------------------------------------------------------
				DataView productInfo = ProductCommon.GetProductInfoUnuseMemberRankPrice(this.ShopId, this.ProductId);
				if (productInfo.Count == 0)
				{
					// 見つからなかったらエラー画面
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
				this.ProductMaster = productInfo[0];

				//------------------------------------------------------
				// サブ画像設定取得＆データバインド（存在するもののみ）
				//------------------------------------------------------
				this.ProductSubImageList = new List<DataRowView>();
				{
					// サブ画像設定取得
					DataView productSubImageSettings = ProductCommon.GetProductSubImageSettingList(this.ShopId);
					if (productSubImageSettings.Count == 0) return;

					int defaultSubImageNo = 0;
					// サブ画像の情報設定
					List<DataRowView> productSubImageListTmp = new List<DataRowView>();
					foreach (DataRowView drvProductSubImage in productSubImageSettings)
					{
						if (CheckProductSubImageExist(this.ProductMaster, (int)drvProductSubImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO]))
						{
							productSubImageListTmp.Add(drvProductSubImage);

							// 最初に存在したサブ画像の画像Noをデフォルト設定とする
							if (defaultSubImageNo == 0)
							{
								defaultSubImageNo = (int)drvProductSubImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO];
							}
						}
					}

					// メイン画像の情報設定
					if (productSubImageListTmp.Count != 0)
					{
						defaultSubImageNo = Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1; // 商品サブ画像Noの上限値よりも+1大きい商品サブ画像Noの時はメイン画像として扱う
						DataRowView mainImage = productSubImageSettings.AddNew();
						mainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO] = defaultSubImageNo;
						mainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME] = ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_PRODUCT_DETAIL,
							Constants.VALUETEXT_PARAM_PRODUCT_DATA_SETTING_MESSAGE,
							Constants.VALUETEXT_PARAM_PRODUCT_DATA_MAIN_IMAGE);

						this.ProductSubImageList.Add(mainImage);
					}

					// メイン・サブ画像をリストへ追加
					foreach (DataRowView drv in productSubImageListTmp)
					{
						this.ProductSubImageList.Add(drv);
					}

					// デフォルト表示を行うID番号の取得
					if (this.SubImageNo == SUB_IMAGE_DEFAULT_NO)
					{
						this.SubImageNo = defaultSubImageNo;
					}
				}

				// データバインド
				this.DataBind();
			}
		}
	}

	/// <summary>商品情報</summary>
	protected DataRowView ProductMaster { get; set; }
	/// <summary>サブ画像番号</summary>
	protected int SubImageNo { get; set; }
	/// <summary>商品サブ画像リスト</summary>
	protected List<DataRowView> ProductSubImageList { get; private set; }
}
