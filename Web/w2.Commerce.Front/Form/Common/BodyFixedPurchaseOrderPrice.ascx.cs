/*
=========================================================================================================
  Module      : 定期注文購入金額出力コントローラ処理(BodyFixedPurchaseOrderPrice.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;

public partial class Form_Common_BodyFixedPurchaseOrderPrice : BaseUserControl
{
	#region ラップ済コントロール宣言
	/// <summary>divSubscriptionBoxAutomaticReset</summary>
	protected WrappedWebControl WdivSubscriptionBoxAutomaticReset { get { return GetWrappedControl<WrappedWebControl>("divSubscriptionBoxAutomaticReset"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack
			|| ((this.Cart == null)
				&& (Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING] != null)
				&& (((CartObject[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING]).Length > 0)))
		{
			if (IsPostBack)
			{
				this.Cart = ((CartObject[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING])[0];
				Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING] = ((CartObject[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING])
					.Where(co => co.CartId != this.Cart.CartId)
					.ToArray();
			}

			WrappedWebControl wDivSubCartList = GetWrappedControl<WrappedWebControl>("divSubCartList");
			wDivSubCartList.Visible = this.IsExistedFixedPurchaseItemNotOrderCombine;

			this.WdivSubscriptionBoxAutomaticReset.Visible = false;

			// 今回の注文商品に定期商品が含まれていない、または表示回数に0が指定されている場合は非表示
			if ((this.IsExistedFixedPurchaseItemNotOrderCombine == false) || (this.DisplayMaxCount == 0)) return;

			// 頒布会表示
			// 注文同梱の場合、子注文が通常や定期であれば処理を抜ける
			if (((this.Cart.IsOrderCombined == false)
					&& this.Cart.IsSubscriptionBox)
				|| (this.Cart.IsOrderCombined
					&& (this.Cart.IsShouldRegistSubscriptionForCombine
						|| this.Cart.IsOrderCombinedWithSameSubscriptionBoxCourse())))
			{
				// 頒布会コースが1回のみの場合、2回目以降の注文内容は表示しない
				var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.Cart.SubscriptionBoxCourseId);
				this.DisplayMaxCount =
					subscriptionBox.DefaultOrderProducts.Max(defaultProductCount => defaultProductCount.Count ?? 0);
				this.IsIndefinitePeriod = subscriptionBox.IsIndefinitePeriod;
				if (this.DisplayMaxCount == 1)
				{
					this.FixedPurchasePriceExchange = 0;
					this.FixedPurchasePriceShipping = 0;
					this.SummaryOfFixedPurchasePriceTotal = this.Cart.PriceTotal;
					return;
				}

				DisplayFixedPurchaseOrderPrice(subscriptionBox);
				return;
			}

			// 定期注文金額表示
			DisplayFixedPurchaseOrderPrice();

			// 定期購入解約可能回数（数値大きいの方適用）
			foreach (CartProduct cp in this.Cart)
			{
				if (this.CancelableCount == 0 || this.CancelableCount < cp.FixedPurchaseCancelableCount)
				{
					this.CancelableCount = cp.FixedPurchaseCancelableCount;
				}
			}
		}
	}

	/// <summary>
	/// 定期注文金額表示
	/// </summary>
	private void DisplayFixedPurchaseOrderPrice()
	{
		// カート内の定期商品の価格を、2回目以降の価格に更新する
		var tempCart = this.Cart.Copy(false).CloneCart();

		// 頒布会を含む注文同梱の場合、頒布会商品は除いておく
		if (this.Cart.IsOrderCombinedWithSubscriptionBox) tempCart.ExcludeSubscriptionBoxItem();

		tempCart.SetFixedPurchasePrice();

		// 各注文回数ごとの金額を計算してListに格納
		var fixedPurchaseList = new List<CartObject>();
		var fixedPurchaseOrderCount = 1;

		// 2回目以降の注文金額を計算する
		for (var displayCount = 1; displayCount <= this.DisplayMaxCount; displayCount++)
		{
			var cart = tempCart.Copy(false).CloneCart();

			// CartShippingのCartObjectを最新化する
			foreach (var shipping in cart.Shippings)
			{
				shipping.CartObject = cart;
			}

			if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
			{
				CalculateFixedPurchaseNextShippingProduct(cart);
			}

			cart.SetFixedPurchasePrice();
			cart.SubscriptionTimes = displayCount + 1;
			cart.DuplicatedSubscriptionTimesTo = displayCount + 1;

			// クーポン、ポイント情報は2回目以降の金額には含めないため削除する
			cart.Coupon = null;
			cart.UsePoint = 0;
			cart.UsePointPrice = 0;

			// 定期商品購入回数設定
			for (var idx = 0; idx < cart.Items.Count; idx++)
			{
				// 定期商品でない場合、スキップ
				if (cart.Items[idx].IsFixedPurchase == false) continue;

				var isChangeProduct = (cart.Items[idx].ProductId != tempCart.Items[idx].ProductId)
					|| (cart.Items[idx].VariationId != tempCart.Items[idx].VariationId);
				cart.Items[idx].FixedPurchaseItemOrderCount = isChangeProduct ? displayCount : (displayCount + 1);
			}

			cart.Calculate(
				false,
				isPaymentChanged: true,
				fixedPurchaseOrderCount: fixedPurchaseOrderCount,
				isSecndFixedPurchase: true,
				fixedPurchaseDisplayCount: displayCount + 1,
				isFixedPurchaseOrderPrice: true);

			cart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
				cart.ShopId,
				cart.Payment.PaymentId,
				cart.PriceSubtotal,
				cart.PriceCartTotalWithoutPaymentPrice);

			fixedPurchaseList.Add(cart);
			fixedPurchaseOrderCount++;
		}

		this.FixedPurchasePriceExchange = fixedPurchaseList.First().Payment.PriceExchange;
		this.FixedPurchasePriceShipping = fixedPurchaseList.First().PriceShipping;
		rFixedPurchaseOrderPriceList.DataSource = fixedPurchaseList;
		rFixedPurchaseOrderPriceList.DataBind();
		this.TotalDisplayMaxCount = fixedPurchaseList.Count(d => d.PriceSubtotal > 0);
		this.SummaryOfFixedPurchasePriceTotal = this.Cart.PriceTotal + fixedPurchaseList.Where(d => d.PriceSubtotal > 0).Sum(o => o.PriceTotal);
		this.SubscriptionTimes = fixedPurchaseList[0].SubscriptionTimes;
		this.DuplicatedSubscriptionTimesTo = fixedPurchaseList[0].DuplicatedSubscriptionTimesTo;
	}
	/// <summary>
	/// 頒布会注文金額表示
	/// </summary>
	/// <param name="subscriptionBox">頒布会情報</param>
	private void DisplayFixedPurchaseOrderPrice(SubscriptionBoxModel subscriptionBox)
	{
		if (subscriptionBox == null) return;

		// 異なる頒布会での注文同梱の場合、子注文の頒布会コース商品のみに絞り込んでセットする
		var tempCart = this.Cart.Copy(false).CloneCart();
		if (this.Cart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false)
		{
			tempCart.SetCartSubscriptionBoxCourseItem();
		}

		var beforeCart = tempCart.Copy(false).CloneCart();

		// カート内の定期商品の価格を、2回目以降の価格に更新する
		tempCart.SetFixedPurchasePrice();

		// 各注文回数ごとの金額を計算してListに格納
		var fixedPurchaseList = new List<CartObject>();
		// 表示用カートのリスト
		// 以下はイメージ
		// 1回目の注文内容
		// 2回目の注文内容
		// 3～5回目まで2回目と同じ注文内容でお届けします。
		// 6回目の注文内容
		// 7～10回目まで6回目と同じ注文内容でお届けします。
		var displayFixedPurchaseList = new List<CartObject>();
		var cartShipping = tempCart.GetShipping();

		// 商品決定方法が「期間」の場合
		if (subscriptionBox.IsDeterminationTypePeriod)
		{
			var calculator = new FixedPurchaseShippingDateCalculator();
			var count = 0;

			// 次回配送日を計算する
			var tempDate = calculator.CalculateFirstShippingDate(
				cartShipping.ShippingDate ?? cartShipping.GetFirstShippingDate(),
				cartShipping.FixedPurchaseDaysRequired);

			// 配送対象の商品を取得
			var products = subscriptionBox.GetDefaultOrderProduct(tempDate, 0);

			// 配送回数を計算する(対象商品の中で最も終了期限が遠い日付と比較)
			if (products.Any() && cartShipping.HasFixedPurchaseSetting)
			{
				while (tempDate < products.Last().TermUntil)
				{
					tempDate = calculator.CalculateFollowingShippingDate(
					cartShipping.FixedPurchaseKbn,
					cartShipping.FixedPurchaseSetting,
					tempDate,
					cartShipping.FixedPurchaseMinSpan,
					NextShippingCalculationMode.Earliest);

					count++;
				}
			}

			// 配送予定日取得
			var tempShippingDate = calculator.CalculateFirstShippingDate(
				cartShipping.ShippingDate ?? cartShipping.GetFirstShippingDate(),
				cartShipping.FixedPurchaseDaysRequired);

			for (var displayCount = 1; displayCount <= count; displayCount++)
			{
				var cart = tempCart.Copy(false).CloneCart();
				cart.UpdateCartDb = false;

				foreach (var product in products)
				{
					var dvDefaultOrderProducts = ProductCommon.GetProductVariationInfo(
						cart.ShopId,
						product.ProductId,
						product.VariationId,
						null);

					// 配送日を期間に含む商品か判断
					if ((tempShippingDate > product.TermSince) && (tempShippingDate < product.TermUntil))
					{
						cart.AddVirtural(
						new CartProduct(
							dvDefaultOrderProducts[0],
							Constants.AddCartKbn.SubscriptionBox,
							"",
							product.ItemQuantity,
							false,
							new ProductOptionSettingList(),
							subscriptionBoxCourseId: product.SubscriptionBoxCourseId));
					}
				}

				// 配送予定日更新
				tempShippingDate = calculator.CalculateFollowingShippingDate(
					cartShipping.FixedPurchaseKbn,
					cartShipping.FixedPurchaseSetting,
					tempShippingDate,
					cartShipping.FixedPurchaseMinSpan,
					NextShippingCalculationMode.Earliest);

				foreach (var product in tempCart.Items)
				{
					cart.RemoveProduct(
						cart.ShopId,
						product.ProductId,
						product.VariationId,
						product.AddCartKbn,
						product.ProductSaleId,
						(product.ProductOptionSettingList != null)
							? product.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()
							: "");
				}

				// CartShippingのCartObjectを最新化する
				foreach (var shipping in cart.Shippings)
				{
					shipping.CartObject = cart;
				}

				cart.SetFixedPurchasePrice();

				// クーポン、ポイント情報は2回目以降の金額には含めないため削除する
				cart.Coupon = null;
				cart.UsePoint = 0;
				cart.UsePointPrice = 0;

				// 1回目注文の割引の適用などが引き継がれないように再計算する
				if (cartShipping.HasFixedPurchaseSetting)
				{
					cart.Calculate(
						isDefaultShipping: false,
						isSecndFixedPurchase: true,
						fixedPurchaseDisplayCount: displayCount + 1,
						isFixedPurchaseOrderPrice: true);
				}

				cart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					cart.ShopId,
					cart.Payment.PaymentId,
					cart.PriceSubtotal,
					cart.PriceCartTotalWithoutPaymentPrice);

				cart.SubscriptionTimes = cart.DuplicatedSubscriptionTimesTo = displayCount + 1;

				fixedPurchaseList.Add(cart);
			}
		}
		else
		{
			// 2回目以降の注文金額を計算する
			for (var displayCount = 1; displayCount <= this.DisplayMaxCount; displayCount++)
			{
				var cart = tempCart.Copy(false).CloneCart();

				this.IsSubscriptionBoxAutomaticReset = subscriptionBox.IsAutoRenewal;
				var defaultOrderProducts = subscriptionBox.DefaultOrderProducts
					.Where(defaultItem => defaultItem.Count == (displayCount + 1)).ToArray();

				var canTakeOver = (defaultOrderProducts.Length == 0)
					|| defaultOrderProducts.Any(product => string.IsNullOrEmpty(product.ProductId));
				if (canTakeOver)
				{
					cart = beforeCart;
					cart.IsSubscriptionBoxTakeOver = true;
				}
				else
				{
					cart.UpdateCartDb = false;
					foreach (var product in defaultOrderProducts)
					{
						var dvDefaultOrderProducts = ProductCommon.GetProductVariationInfo(
							cart.ShopId,
							product.ProductId,
							product.VariationId,
							null);
						cart.AddVirtural(
							new CartProduct(
								dvDefaultOrderProducts[0],
								Constants.AddCartKbn.SubscriptionBox,
								"",
								product.ItemQuantity,
								false,
								new ProductOptionSettingList(),
								subscriptionBoxCourseId: product.SubscriptionBoxCourseId));
					}

					foreach (var product in tempCart.Items)
					{
						cart.RemoveProduct(
							cart.ShopId,
							product.ProductId,
							product.VariationId,
							product.AddCartKbn,
							product.ProductSaleId,
							(product.ProductOptionSettingList != null)
								? product.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()
								: "");
					}
				}

				// CartShippingのCartObjectを最新化する
				foreach (var shipping in cart.Shippings)
				{
					shipping.CartObject = cart;
				}

				cart.SetFixedPurchasePrice();

				// クーポン、ポイント情報は2回目以降の金額には含めないため削除する
				cart.Coupon = null;
				cart.UsePoint = 0;
				cart.UsePointPrice = 0;

				// 1回目注文の割引の適用などが引き継がれないように再計算する
				if (cartShipping.HasFixedPurchaseSetting)
				{
					cart.Calculate(
						false,
						isSecndFixedPurchase: true,
						fixedPurchaseDisplayCount: displayCount + 1,
						isFixedPurchaseOrderPrice: true);
				}
				cart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					cart.ShopId,
					cart.Payment.PaymentId,
					cart.PriceSubtotal,
					cart.PriceCartTotalWithoutPaymentPrice);

				cart.SubscriptionTimes = displayCount + 1;

				if (displayCount > 1)
				{
					// 前回と同じ商品の場合であるかを判定
					if (cart.Items.Select(cp => cp.VariationId).ToList()
							.All(p => beforeCart.Items.Any(bcp => bcp.VariationId == p))
						&& cart.Items.Count == beforeCart.Items.Count)
					{
						cart.IsSameProducts = true;
					}
					else
					{
						cart.IsSameProducts = false;
					}
				}
				else
				{
					cart.IsSameProducts = false;
				}

				// 頒布会の回数と画面表示の比較(1回目配送商品は対象外)
				if ((displayCount <= (this.DisplayMaxCount - 1)) || (subscriptionBox.IsNumberTime == false))
				{
					fixedPurchaseList.Add(cart);
					if (cart.IsSameProducts == false)
					{
						displayFixedPurchaseList.Add(cart);
					}
				}

				beforeCart = cart.Copy(false).CloneCart();
			}
		}

		if (displayFixedPurchaseList.Count > 0)
		{
			var tmp = 0;
			foreach (var co in displayFixedPurchaseList.OrderByDescending(c => c.SubscriptionTimes).ToList()) 
			{
				if (co.SubscriptionTimes < tmp)
				{
					co.DuplicatedSubscriptionTimesTo = tmp - 1;
				}
				else
				{
					co.DuplicatedSubscriptionTimesTo = this.DisplayMaxCount == 0 ? co.SubscriptionTimes : this.DisplayMaxCount;
				}
				tmp = co.SubscriptionTimes;
			}
		}

		this.FixedPurchasePriceExchange = fixedPurchaseList.Count > 0
			? fixedPurchaseList.First().Payment.PriceExchange
			: tempCart.Payment.PriceExchange;
		this.FixedPurchasePriceShipping = fixedPurchaseList.Count > 0
			? fixedPurchaseList.First().PriceShipping
			: tempCart.PriceShipping;

		rFixedPurchaseOrderPriceList.DataSource = subscriptionBox.IsNumberTime 
			? displayFixedPurchaseList.Count > 0 ? displayFixedPurchaseList : fixedPurchaseList 
			: fixedPurchaseList;
		rFixedPurchaseOrderPriceList.DataBind();
		this.WdivSubscriptionBoxAutomaticReset.Visible = this.IsSubscriptionBoxAutomaticReset || this.IsIndefinitePeriod;

		this.TotalDisplayMaxCount = fixedPurchaseList.Count(d => d.PriceSubtotal > 0);
		this.SummaryOfFixedPurchasePriceTotal = this.Cart.PriceTotal + fixedPurchaseList.Where(d=>d.PriceSubtotal > 0).Sum(o => o.PriceTotal);
		this.SubscriptionTimes = fixedPurchaseList.Count > 0
			? fixedPurchaseList[0].SubscriptionTimes
			: tempCart.SubscriptionTimes;
		this.DuplicatedSubscriptionTimesTo = fixedPurchaseList.Count > 0
			? fixedPurchaseList[0].DuplicatedSubscriptionTimesTo
			: tempCart.DuplicatedSubscriptionTimesTo;
	}

	/// <summary>
	/// Calculate Fixed Purchase Next Shipping Product
	/// </summary>
	/// <param name="cart">Cart</param>
	private void CalculateFixedPurchaseNextShippingProduct(CartObject cart)
	{
		for (int index = 0; index < cart.Items.Count; index++)
		{
			var product = new ProductService().GetProductVariationAtDataRowView(
				cart.Items[index].ShopId,
				cart.Items[index].FixedPurchaseNextShippingProductId,
				cart.Items[index].FixedPurchaseNextShippingVariationId,
				cart.MemberRankId);

			if (product != null)
			{
				var cartProduct = new CartProduct(
					product,
					Constants.AddCartKbn.FixedPurchase,
					cart.Items[index].ProductSaleId,
					(cart.Items[index].FixedPurchaseNextShippingItemQuantity * cart.Items[index].Count),
					false);
				cart.Items[index] = cartProduct;
			}
		}
	}

	/// <summary>
	/// 各配送予定日収得
	/// </summary>
	/// <param name="itemIndex"></param>
	/// <returns>配送予定日</returns>
	protected DateTime? GetShippingDate(int itemIndex)
	{
		var shipping = this.Cart.GetShipping();
		var shippingDate = DateTime.MinValue;

		if (this.Cart.CartId == shipping.CartObject.CartId)
		{
			shippingDate = shipping.NextShippingDate;
			var timeSpace = shipping.NextNextShippingDate - shipping.NextShippingDate;

			if (itemIndex == 0) return shippingDate;
			for (var index = 0; index < itemIndex; index++)
			{
				shippingDate += timeSpace;
			}
		}
		return shippingDate;
	}

	/// <summary>
	/// 配送パターン収得
	/// </summary>
	/// <returns>配送パターン</returns>
	protected string GetFixedPurchaseShippingPatternString()
	{
		var shipping = this.Cart.GetShipping();
		var fixedPurchaseShippingPatternString = string.Empty;

		if (this.Cart.CartId == shipping.CartObject.CartId)
		{
			fixedPurchaseShippingPatternString = this.IsFixedPurchaseShippingPatternChanged
				? "配送パターン：" + shipping.GetNextShippingItemFixedPurchaseShippingPattern()
				: string.Empty;
		}
		return fixedPurchaseShippingPatternString;
	}
	#region プロパティ
	/// <summary>最大表示回数</summary>
	protected int DisplayMaxCount { get; set; }
	/// <summary>Total display Count</summary>
	protected int TotalDisplayMaxCount { get; set; }
	/// <summary>カート（ページ側で設定される）</summary>
	public CartObject Cart { get; set; }
	/// <summary>注文同梱以外の定期商品が含まれているか</summary>
	protected bool IsExistedFixedPurchaseItemNotOrderCombine
	{
		get
		{
			var fixedPurchaseItemNotOrderCombine = this.Cart.Items.Count(item =>
				(((string.IsNullOrEmpty(item.OrderCombineOrgOrderId)) && (item.IsFixedPurchase))
					|| ((item.IsFixedPurchase) && (item.AddedQuantitySingleByOrderCombine > 0))));

			var subscriptionBoxItemNotOrderCombine = this.Cart.Items.Count(item =>
				(((string.IsNullOrEmpty(item.OrderCombineOrgOrderId)) && (item.IsSubscriptionBox))
					|| ((item.IsSubscriptionBox) && (item.AddedQuantitySingleByOrderCombine > 0))));

			return (fixedPurchaseItemNotOrderCombine > 0) || (subscriptionBoxItemNotOrderCombine > 0);
		}
	}
	/// <summary>2回目以降配送料金</summary>
	protected decimal FixedPurchasePriceShipping
	{
		get { return (decimal)(ViewState["FixedPurchasePriceShipping"] ?? 0m); }
		set { ViewState["FixedPurchasePriceShipping"] = value; }
	}
	/// <summary>2回目以降決済手数料</summary>
	protected decimal FixedPurchasePriceExchange
	{
		get { return (decimal)(ViewState["FixedPurchasePriceExchange"] ?? 0m); }
		set { ViewState["FixedPurchasePriceExchange"] = value; }
	}
	/// <summary>総合計の合算</summary>
	protected decimal SummaryOfFixedPurchasePriceTotal
	{
		get { return (decimal)(ViewState["SummaryOfFixedPurchasePriceTotal"] ?? 0m); }
		set { ViewState["SummaryOfFixedPurchasePriceTotal"] = value; }
	}
	/// <summary>定期購入解約可能回数</summary>
	protected int CancelableCount { get; set; }
	/// <returns>二回目の配送間隔が変わるかどうか</returns>
	protected bool IsFixedPurchaseShippingPatternChanged
	{
		get
		{
			var shipping = this.Cart.GetShipping();
			var isFixedPurchaseShippingPatternChanged = true;

			if (this.Cart.CartId == shipping.CartObject.CartId)
			{
				// EC管理画面の商品情報で設定された定期購入2回目以降商品の定期配送パターン
				var itemFixedPurchaseSetting = shipping.GetNextShippingItemFixedPurchaseShippingPattern();
				// フロントで設定された定期配送パターン
				var fixedPurchaseSetting = shipping.GetFixedPurchaseShippingPatternString();
				isFixedPurchaseShippingPatternChanged = (itemFixedPurchaseSetting != fixedPurchaseSetting);
			}
			return isFixedPurchaseShippingPatternChanged;
		}
	}
	/// <summary>頒布会で自動繰り返し設定になっているか</summary>
	protected bool IsSubscriptionBoxAutomaticReset { get; set; }
	/// <summary>Use all point flag</summary>
	protected bool UseAllPointFlg
	{
		get
		{
			var result = Constants.W2MP_POINT_OPTION_ENABLED
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE
				&& (this.Cart != null)
				&& this.Cart.UseAllPointFlg;
			return result;
		}
	}
	/// <summary>無期限設定フラグか</summary>
	protected bool IsIndefinitePeriod { get; set; }
	/// <summary>頒布会・定期何回目</summary>
	protected int SubscriptionTimes
	{
		get { return (int)ViewState["SubscriptionTimes"]; }
		set { ViewState["SubscriptionTimes"] = value; }
	}
	/// <summary>頒布会何回目まで商品重複されている</summary>
	protected int DuplicatedSubscriptionTimesTo
	{
		get { return (int)ViewState["DuplicatedSubscriptionTimesTo"]; }
		set { ViewState["DuplicatedSubscriptionTimesTo"] = value; }
	}
	#endregion
}
