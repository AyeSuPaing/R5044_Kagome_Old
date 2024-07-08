/*
=========================================================================================================
  Module      : 送料無料案内おすすめ商品出力コントローラ処理(BodyProductRanking.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public partial class Form_Common_BodyRecommendFreeShipping : FreeShippingUserControl
{
	/// <summary>最大表示件数</summary>
	const int CONST_DISPCOUNT_MAX = 10;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		// なにもしない
	}

	/// <summary>
	/// 送料無料案内リコメンド商品リスト取得
	/// </summary>
	/// <returns>表示対象List</returns>
	protected List<DataRowView> GetRecommendFreeShippingProductList()
	{
		// 出力フラグがfalseなら取得しない。
		if (this.IsDisplayAnnounceFreeShipping == false) return null;

		// 変数宣言
		List<DataRowView> productList = new List<DataRowView>();

		// リコメンド商品一覧取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// 変数宣言
			DataView crossSellProducts;

			// クロスセル商品の取得
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetCrossSellProductsFromCart"))
			{
				crossSellProducts = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, GetInputParamForRecommendFreeShipping());
			}

			// 表示件数に満たない場合は補填する
			productList = GetFillingUpRecommendProductList(crossSellProducts, sqlAccessor);
		}

		// 表示できる件数がゼロの場合非表示
		if (productList.Count == 0) this.Visible = false;

		return productList;
	}


	/// <summary>
	/// 重複判定
	/// </summary>
	/// <param name="productList">商品リスト</param>
	/// <param name="targetProduct">チェック対象データ</param>
	/// <returns></returns>
	private bool IsDuplicate(List<DataRowView> productList, DataRowView targetProduct)
	{
		foreach (DataRowView drvSource in productList)
		{
			if (drvSource[Constants.FIELD_PRODUCT_PRODUCT_ID].ToString()
				       == targetProduct[Constants.FIELD_PRODUCT_PRODUCT_ID].ToString())
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// レコメンド用商品を最大数まで設定する
	/// </summary>
	/// <param name="crossSellProducts">クロスセル商品</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>不足分を補填したList</returns>
	private List<DataRowView> GetFillingUpRecommendProductList(DataView crossSellProducts, SqlAccessor sqlAccessor)
	{
		List<DataRowView> productList = new List<DataRowView>();
		DataView dvForRecommendProductFillingUp;
		int productCount = 0;

		// Listに表示件数分の商品を設定する
		foreach (DataRowView drvCrossSellProduct in crossSellProducts)
		{
			productList.Add(drvCrossSellProduct);
			productCount++;

			// 必要数量を満たした場合はbreak
			if (productCount == MaxDispCount) break;
		}

		// 必要な数量を満たしていない場合、クロスセル以外の商品で補填する
		if (productCount < MaxDispCount)
		{
			// 表示件数に満たない場合に補助する商品の取得
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductsForRecommendProductFillingUp"))
			{
				dvForRecommendProductFillingUp = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, GetInputParamForRecommendFreeShipping());
			}

			// 残りの表示件数分を設定する
			foreach (DataRowView drvForFillingUp in dvForRecommendProductFillingUp)
			{
				// 重複していない場合、結果に追加する
				if (IsDuplicate(productList, drvForFillingUp) == false)
				{
					productList.Add(drvForFillingUp);
					productCount++;

					// 必要数量を満たした場合はbreak
					if (productCount == MaxDispCount) break;
				}
			}
		}

		return productList;
	}

	/// <summary>
	/// 利用するパラメータを設定
	/// </summary>
	/// <returns></returns>
	private Hashtable GetInputParamForRecommendFreeShipping()
	{
		Hashtable htResult = new Hashtable();
		htResult.Add(Constants.FIELD_CART_SHOP_ID, this.ShopId);
		htResult.Add(Constants.FIELD_CART_CART_ID, this.CartId);
		htResult.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, this.ShippingId);
		htResult.Add(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG, this.DigitalContentsFlg);
		htResult.Add("current_date", DateTime.Now);
		htResult.Add(Constants.KEY_SHOW_OUT_OF_STOCK_ITEMS, this.ShowOutOfStock ? Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID : Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID);

		// 会員ランク
		htResult.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId);

		return htResult;
	}

	#region プロパティ
	/// <summary>表示する商品数</summary>
	public int MaxDispCount
	{
		get { return maxDispCount; }
		set { maxDispCount = (value <= CONST_DISPCOUNT_MAX) ? value : CONST_DISPCOUNT_MAX; }
	}
	int maxDispCount;
	/// <summary>在庫切れ商品を表示するかどうか</summary>
	protected bool ShowOutOfStock
	{
		get { return m_showOutOfStock; }
		set { m_showOutOfStock = value; }
	}
	private bool m_showOutOfStock = Constants.SHOW_OUT_OF_STOCK_ITEMS;
	/// <summary>商品画像サイズ（外部から設定可能）</summary>
	public string ImageSize { get; set; }
	#endregion
}