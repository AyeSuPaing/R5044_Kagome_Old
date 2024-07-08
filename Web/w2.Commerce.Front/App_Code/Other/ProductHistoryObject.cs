/*
=========================================================================================================
  Module      : 商品履歴オブジェクトクラス(ProductHistoryObject.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Product;

///*********************************************************************************************
/// <summary>
/// 商品履歴オブジェクトクラス
/// </summary>
///*********************************************************************************************
[Serializable]
public class ProductHistoryObject : List<Hashtable>
{
	/// <summary>
	/// 商品履歴追加
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	public void Add(DataRowView drvProduct)
	{
		//------------------------------------------------------
		// 商品情報作成
		//------------------------------------------------------
		Hashtable htProduct = new Hashtable();
		htProduct.Add(Constants.FIELD_PRODUCT_SHOP_ID, drvProduct[Constants.FIELD_PRODUCT_SHOP_ID]);
		htProduct.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]);
		htProduct.Add(Constants.FIELD_PRODUCT_IMAGE_HEAD, drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD]);
		htProduct.Add(Constants.FIELD_PRODUCT_NAME, drvProduct[Constants.FIELD_PRODUCT_NAME]);
		htProduct.Add(Constants.FIELD_PRODUCT_DISPLAY_PRICE, drvProduct[Constants.FIELD_PRODUCT_DISPLAY_PRICE]);
		htProduct.Add(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE, drvProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]);
		htProduct.Add(Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE, drvProduct[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE]);
		htProduct.Add(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE, drvProduct[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE]);
		htProduct.Add(Constants.FIELD_PRODUCT_CATEGORY_ID1, drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID1]);
		htProduct.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG, drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]);
		htProduct.Add(Constants.FIELD_PRODUCT_BRAND_ID1, drvProduct[Constants.FIELD_PRODUCT_BRAND_ID1]);
		htProduct.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE, drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE]);
		htProduct.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE, drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE]);
		if (drvProduct.DataView.Table.Columns.Contains(ProductListUtility.FIELD_PRODUCT_SOLDOUT))
		{
			htProduct.Add(ProductListUtility.FIELD_PRODUCT_SOLDOUT, drvProduct[ProductListUtility.FIELD_PRODUCT_SOLDOUT]);
		}

		//------------------------------------------------------
		// 商品情報追加
		//------------------------------------------------------
		// 同一商品があるか比較(店舗ID、商品ID)、あれば削除
		foreach (Hashtable htProductTmp in this)
		{
			if (((string)htProductTmp[Constants.FIELD_PRODUCT_SHOP_ID] == (string)htProduct[Constants.FIELD_PRODUCT_SHOP_ID])
				&& ((string)htProductTmp[Constants.FIELD_PRODUCT_PRODUCT_ID] == (string)htProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]))
			{
				this.Remove(htProductTmp);
				break;
			}
		}

		// Hashtableを先頭に追加
		this.Insert(0, htProduct);

		// 商品保持数が商品履歴保持数を超えた場合一番古い情報を削除
		if (this.Count > Constants.CONST_PRODUCTHISTORY_HOLD_COUNT)
		{
			this.RemoveAt(Constants.CONST_PRODUCTHISTORY_HOLD_COUNT);
		}
	}

}
