/*
=========================================================================================================
  Module      : おすすめ商品ユーザコントロール(ProductRecommendUserControl.cs)
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

/// <summary>
/// ProductRecommend の概要の説明です
/// </summary>
public class ProductRecommendUserControl : ProductRecommendBaseUserControl
{
	/// <summary>
	/// キャッシュキー取得
	/// </summary>
	/// <returns>キャッシュキー</returns>
	protected override string GetCacheKey()
	{
		return string.Format("DataKbn='{0}';BrandId='{1}';CategoryId='{2}';MemberRankId='{3}';UserFixedPurchaseMemberFlg='{4}';", this.DataKbn, this.BrandId, this.CategoryId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
	}

	/// <summary>
	/// キャッシング時間を取得（分。「0」はキャッシングしない）
	/// </summary>
	/// <returns>キャッシング時間</returns>
	protected override int GetCacheExpireMinutes()
	{
		return Constants.PRODUCT_RECOMMEND_CACHE_EXPIRE_MINUTES;
	}

	/// <summary>
	/// DBからデータ取得（おすすめ商品取得）
	/// </summary>
	/// <returns>おすすめ商品</returns>
	protected override DataView GetDatasFromDbInner()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ProductDisplay", "GetProductParticular"))
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
	/// ソート（商品対象リストをランダムに並び替え）
	/// </summary>
	/// <param name="products">商品リスト</param>
	/// <returns>ランダムな商品リスト</returns>
	/// <remarks>
	/// 共有リソースのDataViewを書き換えるのではなく、複製したものを書き換えたい
	/// </remarks>
	protected override DataView SortProducts(DataView products)
	{
		var list = products.Table.Rows.Cast<DataRow>().ToList();
		var list2 = list.OrderBy(i => Guid.NewGuid()).ToList();

		var productsTable = (products.Table.Clone());
		list2.ForEach(productsTable.ImportRow);

		return productsTable.DefaultView;
	}

	#region プロパティ
	/// <summary>データ区分（外部から設定可能）</summary>
	string m_strDataKbn = "IC1";
	public string DataKbn
	{
		get { return m_strDataKbn; }
		set { m_strDataKbn = value; }
	}
	#endregion
}
