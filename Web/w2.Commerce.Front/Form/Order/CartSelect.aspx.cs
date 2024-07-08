/*
=========================================================================================================
  Module      : ログイン後カート選択画面処理(CartSelect.aspx.cs)
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
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Order_CartSelect : OrderPage
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrProductList { get { return GetWrappedControl<WrappedRepeater>("rProductList"); } }
	WrappedRepeater WrProductListBefore { get { return GetWrappedControl<WrappedRepeater>("rProductListBefore"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);

			this.CartProductsNow = new List<CartProduct>();
			this.CartProductsBefore = new List<CartProduct>();

			var cartProduct = Preview.GetDummyCartProduct(this.ShopId);
			if (cartProduct != null)
			{
				this.CartProductsNow.Add(cartProduct);
				this.CartProductsBefore.Add(cartProduct);
			}

			this.WrProductList.DataSource = this.CartProductsNow;
			this.WrProductList.DataBind();

			this.WrProductListBefore.DataSource = this.CartProductsBefore;
			this.WrProductListBefore.DataBind();
			return;
		}

		//------------------------------------------------------
		// 未ログイン、パラメタが存在しない場合TOPページへ遷移
		//------------------------------------------------------
		if ((this.IsLoggedIn == false) || (this.AliveSessionParameter == false))
		{
			Response.Redirect(Constants.PATH_ROOT);
		}

		if (!IsPostBack)
		{
			this.NextUrl = (string)StringUtility.ToValue(Session[Constants.SESSION_KEY_NEXT_URL], Constants.PATH_ROOT);

			//------------------------------------------------------
			// 現行カートに入っている商品のリスト作成
			//------------------------------------------------------
			this.CartProductsNow = new List<CartProduct>();
			foreach (CartObject coNow in this.CartList.Items)
			{
				foreach (CartProduct cpNow in coNow.Items)
				{
					this.CartProductsNow.Add(cpNow);
				}
			}

			//------------------------------------------------------
			// 以前カートに入っていた商品のリスト作成
			//------------------------------------------------------
			this.CartProductsBefore = new List<CartProduct>();
			CartObjectList colBeforeCartList = (CartObjectList)((Hashtable)Session[Constants.SESSION_KEY_PARAM])["cart"];
			if (colBeforeCartList.Items.Count == 0)
			{
				Response.Redirect(this.NextUrl);
			}

			foreach (CartObject coBefore in colBeforeCartList.Items)
			{
				foreach (CartProduct cpBefore in coBefore.Items)
				{
					if (cpBefore.IsSetItem)
					{
						bool blAdd = true;
						foreach (CartProduct cpNow in this.CartProductsNow)
						{
							if (cpNow.IsSetItem)
							{
								if (cpBefore.ProductSet.IsSameAs(cpNow.ProductSet))
								{
									blAdd = false;
									break;
								}
							}
						}
						// 同じパターンのセット商品がカートになれればリストへ追加
						if (blAdd)
						{
							this.CartProductsBefore.Add(cpBefore);
						}
					}
					else
					{
						bool blAdd = true;
						foreach (CartProduct cpNow in this.CartProductsNow)
						{
							if (cpNow.IsSetItem == false)
							{
								if ((cpNow.ShopId == cpBefore.ShopId)
								&& (cpNow.ProductId == cpBefore.ProductId)
								&& (cpNow.VariationId == cpBefore.VariationId)
								&& (cpNow.IsFixedPurchase == cpBefore.IsFixedPurchase)
								&& (cpNow.ProductSaleId == cpBefore.ProductSaleId)
								&& (cpNow.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == cpBefore.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()))
								{
									blAdd = false;
									break;
								}
							}
						}

						// 同じパターンの商品がカートになれればリストへ追加
						if (blAdd)
						{
							this.CartProductsBefore.Add(cpBefore);
						}
					}
				}
			}

			if (this.CartProductsBefore.Count == 0)
			{
				// 前回カート情報なしの場合（新カートと内容重複）、選択画面を表示せずに遷移
				Response.Redirect(this.NextUrl);
			}

			//------------------------------------------------------
			// 今現在のカート情報データバインド
			//------------------------------------------------------
			this.WrProductList.DataSource = this.CartProductsNow;
			this.WrProductList.DataBind();

			//------------------------------------------------------
			// 以前カートに入っていた商品のリスト作成
			//------------------------------------------------------
			this.WrProductListBefore.DataSource = this.CartProductsBefore;
			this.WrProductListBefore.DataBind();
		}
	}

	/// <summary>
	/// 次へボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 現行カート処理
		//------------------------------------------------------
		foreach (RepeaterItem ri in this.WrProductList.Items)
		{
			// WrappedControl宣言
			var whfIsSetItem = GetWrappedControl<WrappedHiddenField>(ri, "hfIsSetItem", "");
			var whfShopId = GetWrappedControl<WrappedHiddenField>(ri, "hfShopId", "");
			var whfProductId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductId", "");
			var whfVariationId = GetWrappedControl<WrappedHiddenField>(ri, "hfVariationId", "");
			var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(ri, "hfIsFixedPurchase", "false"); // 旧バージョン用
			var whfhfAddCartKbn = GetWrappedControl<WrappedHiddenField>(ri, "hfAddCartKbn", "Normal");
			var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSaleId", "");
			var whfProductSetId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSetId", "");
			var whfProductSetNo = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSetNo", "");
			var whfProductOptionSettingList = GetWrappedControl<WrappedHiddenField>(ri, "hfProductOptionSettingList", "");

			// チェックがはずされていた場合
			if (((Form_Order_CartSelect_ProductItem)ri.FindControl("ucCartSelect_ProductItem") != null)
				&& (((Form_Order_CartSelect_ProductItem)ri.FindControl("ucCartSelect_ProductItem")).Checked == false))
			{
				// 通常商品？
				if (bool.Parse(whfIsFixedPurchase.Value) == false)
				{
					string strShopId = whfShopId.Value;
					string strProductId = whfProductId.Value;
					string strVatiationId = whfVariationId.Value;
					Constants.AddCartKbn addCartKbn = Constants.AddCartKbn.Normal;
					string productOptionSettingList = whfProductOptionSettingList.Value;
					if (whfhfAddCartKbn.InnerControl != null)
					{
						Enum.TryParse<Constants.AddCartKbn>(whfhfAddCartKbn.Value, out addCartKbn);
					}
					else
					{
						addCartKbn = (Constants.FIXEDPURCHASE_OPTION_ENABLED && bool.Parse(whfIsFixedPurchase.Value)) ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal;
					}
					string strProductSaleId = whfProductSaleId.Value;

					// 商品削除
					this.CartList.DeleteProduct(strShopId, strProductId, strVatiationId, addCartKbn, strProductSaleId, productOptionSettingList);
				}
				// セット商品？
				else
				{
					string strProductSetId = whfProductSetId.Value;
					int iProductSetNo = int.Parse(whfProductSetNo.Value);

					// セット商品削除
					this.CartList.DeleteProductSet(strProductSetId, iProductSetNo);
				}
			}
		}

		//------------------------------------------------------
		// 追加カート処理
		//------------------------------------------------------
		foreach (RepeaterItem ri in this.WrProductListBefore.Items)
		{
			// チェックがついていた場合
			if (((Form_Order_CartSelect_ProductItem)ri.FindControl("ucCartSelect_ProductItem") != null)
				&& (((Form_Order_CartSelect_ProductItem)ri.FindControl("ucCartSelect_ProductItem")).Checked))
			{
				var whfIsSetItem = GetWrappedControl<WrappedHiddenField>(ri, "hfIsSetItem", "");
				var whfCartId = GetWrappedControl<WrappedHiddenField>(ri, "hfCartId", string.Empty);
				var whfShopId = GetWrappedControl<WrappedHiddenField>(ri, "hfShopId", "");
				var whfProductId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductId", "");
				var whfVariationId = GetWrappedControl<WrappedHiddenField>(ri, "hfVariationId", "");
				var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(ri, "hfIsFixedPurchase", "false");
				var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSaleId", "");
				var whfProductSetId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSetId", "");
				var whfProductSetNo = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSetNo", "");
				var whfProductOptionSettingList = GetWrappedControl<WrappedHiddenField>(ri, "hfProductOptionSettingList", "");
				var cartId = whfCartId.Value;

				// 通常商品？
				if (bool.Parse(whfIsSetItem.Value) == false)
				{
					string strShopId = whfShopId.Value;
					string strProductId = whfProductId.Value;
					string strVatiationId = whfVariationId.Value;
					bool blIsFixedPurchase = bool.Parse(whfIsFixedPurchase.Value);
					string strProductSaleId = whfProductSaleId.Value;

					// 該当商品データ取得
					foreach (CartProduct cp in this.CartProductsBefore)
					{
						if (cp.IsSetItem == false)
						{
							// カートコピー機能利用時はカートIDも一致しているかを条件に加える
							if ((Constants.CARTCOPY_OPTION_ENABLED ? (cp.CartId == cartId) : true))
							{
								if ((cp.ShopId == strShopId)
									&& (cp.ProductId == strProductId)
									&& (cp.VariationId == strVatiationId)
									&& (cp.IsFixedPurchase == blIsFixedPurchase)
									&& (cp.ProductSaleId == strProductSaleId))
								{
									if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
									{
										var optionTexts = cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues();
										cp.ProductOptionSettingList = new ProductOptionSettingList(strShopId, strProductId);
										cp.ProductOptionSettingList.SetDefaultValueFromProductOptionTexts(optionTexts);
									}
									// 商品追加
									this.CartList.AddProduct(cp);
									break;
								}
							}
						}
					}
				}
				// セット商品
				else
				{
					string strProductSetId = whfProductSetId.Value;
					int iProductSetNo = int.Parse(whfProductSetNo.Value);

					foreach (CartProduct cp in this.CartProductsBefore)
					{
						if (cp.IsSetItem)
						{
							// カートコピー機能利用時はカートIDも一致しているかを条件に加える
							if ((Constants.CARTCOPY_OPTION_ENABLED ? (cp.CartId == cartId) : true))
							{
								if ((cp.ProductSet.ProductSetId == strProductSetId)
									&& (cp.ProductSet.ProductSetNo == iProductSetNo))
								{
									this.CartList.AddProductSet(cp.ProductSet);
									break;
								}
							}
						}
					}
				}
			}
		}

		//------------------------------------------------------
		// 遷移先ページへ（カートが変更された場合は遷移先をショッピングカートへ遷移）
		//------------------------------------------------------
		// 注文同梱画面へ遷移させるため一旦カートリストへ遷移させる
		var pageCartList = (CanUseCartListLp() == false)
			? Constants.PAGE_FRONT_CART_LIST
			: Constants.PAGE_FRONT_CART_LIST_LP;

		Response.Redirect(Constants.PATH_ROOT + pageCartList);
	}

	/// <summary>
	/// オプション価格表示するか
	/// </summary>
	/// <param name="cartProducts">カートプロダクト</param>
	/// <returns>表示非表示</returns>
	protected bool IsDisplayOption(List<CartProduct> cartProducts)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return false;

		var result = cartProducts
			.Any(cartProduct => (cartProduct.ProductOptionSettingList.SelectedOptionTotalPrice != 0)
				|| cartProduct.ProductOptionSettingList.HasOptionPrice);

		return result;
	}

	/// <summary>以前カートに入っていた商品のリスト</summary>
	protected List<CartProduct> CartProductsBefore
	{
		get { return (List<CartProduct>)ViewState["CartProductsBefore"]; }
		set { ViewState["CartProductsBefore"] = value; }
	}
	/// <summary>今カートに入っている商品のリスト</summary>
	protected List<CartProduct> CartProductsNow { get; set; }

	/// <summary>次の画面へのURL</summary>
	string NextUrl
	{
		get { return (string)ViewState["NextUrl"]; }
		set { ViewState["NextUrl"] = value; }
	}
}

