/*
=========================================================================================================
  Module      : 商品ランキング出力コントローラ処理(BodyProductRanking.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

public partial class Form_Common_Product_BodyProductRanking : ProductRecommendBaseUserControl
{
	/// <summary>
	/// キャッシュキー取得
	/// </summary>
	/// <returns>キャッシュキー</returns>
	protected override string GetCacheKey()
	{
		return this.DataKbn + " " + this.BrandId + " " + this.CategoryId + " " + this.MemberRankId + " " + this.UserFixedPurchaseMemberFlg;
	}

	/// <summary>
	/// キャッシング時間を取得（分。「0」はキャッシングしない）
	/// </summary>
	/// <returns>キャッシング時間</returns>
	protected override int GetCacheExpireMinutes()
	{
		return Constants.PRODUCT_RANKING_CACHE_EXPIRE_MINUTES;
	}

	/// <summary>
	/// DBからデータ取得（ランキング商品取得）
	/// </summary>
	/// <returns>ランキング商品</returns>
	protected override DataView GetDatasFromDbInner()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ProductDisplay", "GetProductRanking"))
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPPRODUCTINFO_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID},
				{Constants.FIELD_DISPPRODUCTINFO_DATA_KBN, this.DataKbn},
				{Constants.FIELD_PRODUCTBRAND_BRAND_ID, this.BrandId},
				{Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, this.CategoryId},
				{Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId},
				{Constants.KEY_SHOW_OUT_OF_STOCK_ITEMS, this.ShowOutOfStock ? Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID : Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID},
				{Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg},
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return dv;
		}
	}

	/// <summary>
	/// ソート（特にソートしない）
	/// </summary>
	/// <param name="products">商品リスト</param>
	/// <returns>商品リスト</returns>
	protected override DataView SortProducts(DataView products)
	{
		return products;
	}

	#region プロパティ
	/// <summary>データ区分（外部から設定可能）</summary>
	public string DataKbn
	{
		get { return m_strDataKbn; }
		set { m_strDataKbn = value; }
	}
	string m_strDataKbn = "SRK";
	#endregion
}