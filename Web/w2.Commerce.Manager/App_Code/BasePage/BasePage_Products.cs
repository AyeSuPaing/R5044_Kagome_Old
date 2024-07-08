/*
=========================================================================================================
  Module      : 基底ページ 商品系共通処理部分(BasePage_Products.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Order;

/// <summary>
/// BasePage_Products の概要の説明です
/// </summary>
public partial class BasePage : w2.App.Common.Web.Page.CommonPage
{
	/// <summary>
	/// 全配送種別情報取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>配送種別情報</returns>
	public static DataView GetShopShippingsAll(string strShopId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopShipping", "GetShippingAllList"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_SHOPSHIPPING_SHOP_ID, strShopId);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 配送種別情報取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strShippingId">配送種別ID</param>
	/// <returns>配送種別情報</returns>
	public static DataView GetShopShipping(string strShopId, string strShippingId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopShipping", "GetShopShipping"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_SHOPSHIPPING_SHOP_ID, strShopId);
			htInput.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, strShippingId);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 商品在庫文言情報取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>商品在庫文言情報</returns>
	public static DataView GetProductStockMessageAll(string strShopId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductStockMessage", "GetProductStockMessageAllList"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID, strShopId);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 商品拡張項目設定取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>商品拡張項目設定</returns>
	public static DataView GetProductExtendSetting(string strShopId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductExtendSetting", "GetProductExtendSetting"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_SHOP_ID, strShopId);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 商品タグ設定取得
	/// </summary>
	/// <returns>商品タグ設定</returns>
	public static DataView GetProductTagSetting()
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductTagSetting", "GetTagSettingList"))
		{
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
		}
	}

	/// <summary>
	/// 商品モール出品設定取得
	/// </summary>
	/// <param name="strShopId"></param>
	/// <param name="strProductId"></param>
	/// <returns></returns>
	public static List<KeyValuePair<string, string>> GetProductMallExhibitsConfig(string strShopId, string strProductId)
	{
		List<KeyValuePair<string, string>> lProductMallExhibitsConfigs = new List<KeyValuePair<string, string>>();

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();

			//------------------------------------------------------
			// モール出品設定マスタ取得
			//------------------------------------------------------
			DataView dvMallExhibitsConfig = null;
			using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "GetMallExhibitsConfigAll"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_PRODUCT_ID, strProductId);

				dvMallExhibitsConfig = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}
			if (dvMallExhibitsConfig.Count != 0)
			{
				//------------------------------------------------------
				// モール連携設定取得（該当商品のフラグオンのみ）
				//------------------------------------------------------
				DataView dvMallCooperationSettings = MallPage.GetMallCooperationSettingAll(strShopId);

				int iExhibitsCount = 1;
				foreach (ListItem liMallExhibitsConfig in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
				{
					if ((string)dvMallExhibitsConfig[0]["exhibits_flg" + iExhibitsCount.ToString()] == Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)
					{
						// モール連携設定取得
						foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSettings)
						{
							if (drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG].ToString() == liMallExhibitsConfig.Value)
							{
								lProductMallExhibitsConfigs.Add(
									new KeyValuePair<string, string>(
									(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG],
									(string)dvMallExhibitsConfig[0]["exhibits_flg" + iExhibitsCount.ToString()]));

								break;
							}
						}
					}
					iExhibitsCount++;
				}
			}
		}

		return lProductMallExhibitsConfigs;
	}

	/// <summary>
	/// 商品有効性チェック
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>エラーメッセージ</returns>
	public static string CheckValidProduct(string shopId, string productId)
	{
		var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(shopId, productId);
		if (product.Count == 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", productId);
		}
		else if ((string)product[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", productId);
		}

		return string.Empty;
	}
}
