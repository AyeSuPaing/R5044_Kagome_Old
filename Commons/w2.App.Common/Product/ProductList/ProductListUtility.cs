/*
=========================================================================================================
  Module      : 商品一覧系ユーティリティクラス(ProductListUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Domain.Product;

namespace w2.App.Common.Product
{
	public class ProductListUtility
	{
		/// <summary>商品の在庫切れ状態のフィールド名 ※SQL文で定義</summary>
		public static string FIELD_PRODUCT_SOLDOUT = "productsoldout";
		
		/// <summary>
		/// 商品の在庫切れ状態取得
		/// </summary>
		/// <param name="objectProduct">商品情報</param>
		/// <returns>有効状態</returns>
		/// <remarks>
		/// SKU全てが売切れの場合は有効：0
		/// SKU何れかの在庫が1つ以上存在する場合は無効：1以上
		/// </remarks>
		public static bool IsProductSoldOut(object objectProduct)
		{
			bool result = false;
			try
			{
				if ((objectProduct is DataRowView)
					&& (((DataRowView)objectProduct).DataView.Table.Columns.Contains(FIELD_PRODUCT_SOLDOUT)))
				{
					result = ((int)(((DataRowView)objectProduct)[FIELD_PRODUCT_SOLDOUT]) == 0);
				}
				else if ((objectProduct is Hashtable)
					&& (((Hashtable)objectProduct).Contains(FIELD_PRODUCT_SOLDOUT)))
				{
					result = ((int)(((Hashtable)objectProduct)[FIELD_PRODUCT_SOLDOUT]) == 0);
				}
				else if (objectProduct is ProductModel)
				{
					result = ((int)((ProductModel)objectProduct).DataSource[FIELD_PRODUCT_SOLDOUT] == 0);
				}
			}
			catch (Exception)
			{
				// なにもしない
				// HACK: MobileのCreateProductLoopDataメソッドで処理を行うが、レコメンド以外にも呼び出されるためデータがなくて落ちる
				// モバイルの一覧系でCreateProductLoopDataを使うすべてのSQL文に追加するのはありえないためコメント化。どちらのサイトで呼ばれているのか判定できればコメント外してOK
				//throw new ArgumentException("パラメタエラー: objProduct is [" + objectProduct.GetType().ToString() + "]");
			}

			return result;
		}
	}
}
