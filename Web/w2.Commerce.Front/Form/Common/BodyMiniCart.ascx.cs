/*
=========================================================================================================
  Module      : ミニカート出力コントローラ処理(BodyMiniCart.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.Common.Extensions;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_BodyMiniCart : ProductUserControl
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrMiniCartList { get { return GetWrappedControl<WrappedRepeater>("rMiniCartList"); } }
	WrappedHtmlGenericControl WdivMiniCart { get { return GetWrappedControl<WrappedHtmlGenericControl>("divMiniCart"); } }
	WrappedHtmlGenericControl WdivMiniCartEmpty { get { return GetWrappedControl<WrappedHtmlGenericControl>("divMiniCartEmpty"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 削除ボタンのイベントを発生させるために必要
		Reload();
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var hfCartId = (HiddenField)BasePageHelper.GetParentRepeaterItemControl((Repeater)source, "hfCartId");
		var cartId = (hfCartId != null) ? hfCartId.Value : "";

		if (e.CommandName == "DeleteProduct")
		{
			DeleteProduct(cartId, e);
		}
		else if (e.CommandName == "DeleteProductSet")
		{
			DeleteProductSet(cartId, e);
		}
		ReloadScreen();
	}

	/// <summary>
	/// 商品情報削除
	/// </summary>
	/// <param name="cartId">カートID</param>
	/// <param name="e">RepeaterCommandEventArgs</param>
	private void DeleteProduct(string cartId, RepeaterCommandEventArgs e)
	{
		#region ラップ済みコントロール宣言
		var whfShopId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfShopId", "");
		var whfProductId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductId", "");
		var whfVariationId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfVariationId", "");
		var whfSubscriptionBoxCourseId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfSubscriptionBoxCourseId", "");
		var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(e.Item, "hfIsFixedPurchase", "false");	// 旧バージョンはこちらを見る
		var whfhfAddCartKbn = GetWrappedControl<WrappedHiddenField>(e.Item, "hfAddCartKbn", "Normal");
		var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductSaleId", "");
		var whfProductOptionValue = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductOptionValue", "");
		#endregion

		// 対象カート取得
		var targetCart = this.CartList.GetCart(cartId);

		// 削除対象商品取得
		if (targetCart != null)
		{
			var shopId = whfShopId.Value;
			var productId = whfProductId.Value;
			var variationId = whfVariationId.Value;
			var subscriptionBoxCourseId = whfSubscriptionBoxCourseId.Value;
			var productSaleId = whfProductSaleId.Value;
			var productOptionValue = whfProductOptionValue.Value;
			var targetCartProduct = targetCart.GetProduct(
				shopId,
				productId,
				variationId,
				false,
				bool.Parse(whfIsFixedPurchase.Value),
				productSaleId,
				productOptionValue,
				"",
				subscriptionBoxCourseId);

			if (targetCartProduct != null)
			{
				var addCartKbn = Constants.AddCartKbn.Normal;
				if (whfhfAddCartKbn.InnerControl != null)
				{
					Enum.TryParse(whfhfAddCartKbn.Value, out addCartKbn);
				}
				else
				{
					addCartKbn = (Constants.FIXEDPURCHASE_OPTION_ENABLED
						&& bool.Parse(whfIsFixedPurchase.Value)) ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal;
				}

				// カート商品削除（商品数が0になればカート削除）
				this.CartList.DeleteProduct(
					cartId,
					shopId,
					productId,
					variationId,
					addCartKbn,
					productSaleId,
					productOptionValue);
			}
		}
	}

	/// <summary>
	/// 商品セット情報削除
	/// </summary>
	/// <param name="cartId">カートID</param>
	/// <param name="e">RepeaterCommandEventArgs</param>
	private void DeleteProductSet(string cartId, RepeaterCommandEventArgs e)
	{
		#region ラップ済みコントロール宣言
		var whfProductSetId = GetWrappedControl<WrappedHiddenField>(e.Item.Parent.Parent, "hfProductSetId", "");
		var whfProductSetNo = GetWrappedControl<WrappedHiddenField>(e.Item.Parent.Parent, "hfProductSetNo", "");
		#endregion

		// カート商品削除（商品数が0になればカート削除）
		// HACK: デザイン側のhfCartIdを削除されるとカートIDが取得できない
		this.CartList.DeleteProductSet(cartId, whfProductSetId.Value, int.Parse(whfProductSetNo.Value));
	}

	/// <summary>
	/// 画面再読み込み
	/// </summary>
	private void ReloadScreen()
	{
		// カート一覧ページの場合のみページ再読み込み
		if ((Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_CART_LIST, true, null))
			|| (Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_CART_LIST_LP, true, null)))
		{
			var originalStringUrl = Request.Url.OriginalString.RemoveQueryStringByKey(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID);
			Response.Redirect(originalStringUrl);
		}
		else
		{
			Reload();
		}
	}

	/// <summary>
	/// 画面リロード処理（HTML描画時に実行される）
	/// </summary>
	protected void Reload()
	{
		// ノベルティ関連処理
		if (Constants.NOVELTY_OPTION_ENABLED)
		{
			var cartNoveltyList = new CartNoveltyList(this.CartList);
			this.CartList.RemoveNoveltyGrantItem(cartNoveltyList);
			cartNoveltyList.RemoveCartNovelty(this.CartList);
		}

		// ミニカート表示
		var cartObjectList = GetCartObjectList();
		if (cartObjectList.Items.Count != 0)
		{
			// データバインド
			this.WrMiniCartList.DataSource = cartObjectList;
			this.WrMiniCartList.DataBind();

			this.TotalPrice = cartObjectList.ItemTotalPrice.ToPriceDecimal().Value;
			this.ProductCount = cartObjectList.ItemTotalCount;

			// カート商品あり表示
			this.WdivMiniCart.Visible = true;
			this.WdivMiniCartEmpty.Visible = false;

			// 新LPカートリストページを使用している場合、LPカートセッションを再セット
			if (Constants.CART_LIST_LP_OPTION
				&& this.LadingCartSessionKey.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME))
			{
				Session[this.LadingCartSessionKey] = cartObjectList;
			}
		}
		else
		{
			// カート商品なし表示
			this.WdivMiniCart.Visible = false;
			this.WdivMiniCartEmpty.Visible = true;
		}
	}

	// 商品削除可能かどうか
	protected bool CanDeleteProduct()
	{
		return ((this.IsOrderPage == false) && (this.IsLandingConfirmFromCartListLp == false));
	}

	/// <summary>カートリスト</summary>
	protected CartObjectList CartList
	{
		get
		{ 
			var cartList = GetCartObjectList();
			return cartList;
		}
	}

	/// <summary>
	/// 商品詳細URL表示できるかどうか処理
	/// </summary>
	/// <param name="cartItem">カートアイテム</param>
	protected bool CanDisplayProductDetailUrl(CartProduct cartItem)
	{
		if (cartItem == null) return false;
		
		var result = cartItem.IsProductDetailLinkValid()
			|| cartItem.IsProductNovelty
			|| Constants.REPEATPLUSONE_OPTION_ENABLED;
		
		return result;
	}

	/// <summary>合計金額</summary>
	protected decimal TotalPrice
	{
		get { return (decimal?)ViewState["TotalPrice"] ?? 0; }
		private set { ViewState["TotalPrice"] = value; }
	}
	/// <summary>カート内商品数</summary>
	protected int ProductCount
	{
		get { return (int?)ViewState["ProductCount"] ?? 0; }
		private set { ViewState["ProductCount"] = value; }
	}
}
