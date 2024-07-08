/*
=========================================================================================================
  Module      : 商品プレビュークラス(Preview.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;

namespace w2.App.Common.Preview
{
	public class ProductPreview : BasePreview
	{
		/// <summary>
		/// 商品詳細プレビュー情報登録
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="dtProduct">商品情報</param>
		public static void InsertProductDetailPreview(string strShopId, string strProductId, DataTable dtProduct)
		{
			InsertPreview(Constants.FLG_PREVIEW_PREVIEW_KBN_PRODUCT_DETAIL, strShopId, strProductId, "", "", "", dtProduct);
		}

		/// <summary>
		/// 商品詳細プレビュー情報取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <returns>商品詳細プレビュー情報</returns>
		public static DataView GetProductDetailPreview(string strShopId, string strProductId)
		{
			return GetPreview(Constants.FLG_PREVIEW_PREVIEW_KBN_PRODUCT_DETAIL, strShopId, strProductId, "", "", "");
		}

		/// <summary>
		///  商品マスタから商品詳細プレビューデータ情報取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>商品詳細プレビュー情報</returns>
		public static DataView GetProductDetailPreviewData(string strShopId, string strProductId, SqlAccessor sqlAccessor)
		{
			DataView dvResult = null;

			// 商品情報取得
			using (SqlStatement sqlStatement = new SqlStatement("Preview", "GetProductDetailPreview"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, strProductId);
				dvResult = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			return dvResult;
		}

		/// <summary>
		/// 商品詳細プレビュー用ハッシュ値を生成する
		/// </summary>
		/// <returns>商品詳細プレビュー用ハッシュ値</returns>
		public static string CreateProductDetailHash()
		{
			return CreateHash(Constants.FLG_PREVIEW_PREVIEW_KBN_PRODUCT_DETAIL, 32);
		}
	}
}
