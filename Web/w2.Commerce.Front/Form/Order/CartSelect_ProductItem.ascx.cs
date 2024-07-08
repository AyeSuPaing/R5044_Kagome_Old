/*
=========================================================================================================
  Module      : ログイン後カート選択画面：商品表示ユーザーコントロール処理(CartSelect_ProductItem.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Order_CartSelect_ProductItem : ProductUserControl
{
	// ラップ済みコントロール宣言
	protected WrappedCheckBox wcbAddToCart { get { return GetWrappedControl<WrappedCheckBox>(this, "cbAddToCart"); } }
	protected WrappedCheckBox wcbAddSetToCart { get { return GetWrappedControl<WrappedCheckBox>(this, "cbAddSetToCart"); } }

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
	/// オプション価格表示するか
	/// </summary>
	/// <param name="cartProducts">カートプロダクト</param>
	/// <returns>オプション表示非表示</returns>
	protected bool IsDisplayOption(List<CartProduct> cartProducts)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return false;
		return cartProducts.Any(cartProduct => (cartProduct.TotalOptionPrice != 0) || cartProduct.ProductOptionSettingList.HasOptionPrice);
	}

	/// <summary>カート商品</summary>
	public CartProduct CartProduct
	{
		get { return (CartProduct)ViewState["CartProduct"]; }
		set { ViewState["CartProduct"] = value; }
	}
	/// <summary>カートデフォルトチェック設定</summary>
	public bool DefaultChecked { get; set; }
	/// <summary>チェック状態取得</summary>
	public bool Checked
	{
		get { return (this.CartProduct.IsSetItem) ? wcbAddSetToCart.Checked : wcbAddToCart.Checked; }
	}
	/// <summary>カートプロダクトリスト</summary>
	public List<CartProduct> CartProducts
	{
		get { return (List<CartProduct>)ViewState["CartProducts"]; }
		set { ViewState["CartProducts"] = value; }
	}
}
