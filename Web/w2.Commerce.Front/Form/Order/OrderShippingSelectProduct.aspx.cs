/*
=========================================================================================================
  Module      : 注文配送先商品選択画面処理(OrderShippingSelectProduct.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.UserDefaultOrderSetting;

public partial class Form_Order_OrderShippingSelectProduct : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	WrappedLinkButton WlbBack { get { return GetWrappedControl<WrappedLinkButton>("lbBack"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			RefreshAll(e);
			return;
		}

		RedirectOrderShipping();
		RedirectOrderShippingSelect();

		//------------------------------------------------------
		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// カートチェック（カートが存在しない場合、エラーページへ）
		//------------------------------------------------------
		CheckCartExists();

		//------------------------------------------------------
		// カート注文者存在チェック
		//------------------------------------------------------
		CheckCartOwnerExists();

		//------------------------------------------------------
		// カート配送先存在チェック
		//------------------------------------------------------
		CheckCartShippingExists();

		// Load data for shipping method
		this.Process.CreateShippingMethodListOnDataBind();

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 注全画面リフレッシュ
			//------------------------------------------------------
			RefreshAll(e);
			// レコメンド商品投入時は、次へボタンクリックを行いエラーを表示させる
			if (this.IsAddRecmendItem)
			{
				lbNext_Click(sender, e);
			}
		}

		// デフォルト支払方法の設定があり、レコメンド商品追加時でない場合、画面遷移先を注文確認ページに設定
		var userDefaultOrderSetting = Constants.TWOCLICK_OPTION_ENABLE ? new UserDefaultOrderSettingService().Get(this.LoginUserId) : null;
		if ((userDefaultOrderSetting != null) && (this.IsAddRecmendItem == false))
		{
			if (userDefaultOrderSetting.PaymentId != null)
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_CONFIRM;
			}
		}
	}

	/// <summary>
	/// 全画面リフレッシュ
	/// </summary>
	/// <param name="e"></param>
	private void RefreshAll(EventArgs e)
	{
		//----------------------------------------------------
		// 配送情報入力画面初期処理（共通）
		//----------------------------------------------------
		InitComponentsOrderShipping();

		//------------------------------------------------------
		// データバインド準備
		//------------------------------------------------------
		this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

		// 定期購入設定作成
		CreateFixedPurchaseSettings();

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();

		//------------------------------------------------------
		// データバインド後処理
		//------------------------------------------------------
		// 各種表示初期化(データバインド後に実行する必要がある)
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// 配送先リフレッシュ
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
			foreach (RepeaterItem riShipping in wrCartShippings.Items)
			{
				InitComponentsDispOrderShipping(
					riShipping,
					e);

				// 注文メモ欄デザイン設定
				SetOrderMemoDesign(riCart);

				// 配送変更表示初期処理
				var wddlShippingMethod = CreateWrappedDropDownListShippingMethod(riShipping);
				if (wddlShippingMethod.InnerControl != null)
				{
					wddlShippingMethod.SelectedValue =
						this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex].ShippingMethod;
					ddlShippingMethodList_OnSelectedIndexChanged(wddlShippingMethod, e);
				}

				// 配送サービス選択初期化
				SelectDeliveryCompany(riShipping, this.CartList.Items[riCart.ItemIndex], riShipping.ItemIndex);
			}

			// 商品部分リフレッシュ
			RefreshCartProducts(riCart);
		}
	}

	/// <summary>
	/// カート商品部分表示更新
	/// </summary>
	/// <param name="riShipping"></param>
	private void RefreshCartProducts(RepeaterItem riCart)
	{
		CartObject cartObject = this.CartList.Items[riCart.ItemIndex];

		// 割り当て済み商品更新
		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		foreach (RepeaterItem riShipping in wrCartShippings.Items)
		{
			// 割り当て済み商品一覧バインド
			var wrAllocatedProducts = GetWrappedControl<WrappedRepeater>(riShipping, "rAllocatedProducts");
			wrAllocatedProducts.Visible = (this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex].ProductCounts.Count > 0);
			wrAllocatedProducts.DataSource = this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex].ProductCounts;
			wrAllocatedProducts.DataBind();

			if (cartObject.IsGift)
			{
				// テキストボックス変更イベント作成
				foreach (RepeaterItem riProduct in wrAllocatedProducts.Items)
				{
					var wlbRecalculateCart = GetWrappedControl<WrappedLinkButton>(riProduct, "lbRecalculateCart");
					var wtbProductCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount", "1");

					// テキストボックス向けイベント作成
					TextBoxEventScriptCreator eventCreator = new TextBoxEventScriptCreator(wlbRecalculateCart);
					eventCreator.RegisterInitializeScript(this);

					eventCreator.AddScriptToControl(wtbProductCount);
				}

				// 商品リストボックス初期化
				var wlbProducts = GetWrappedControl<WrappedListBox>(riShipping, "lbProducts");
				var whfProductListFormat = GetWrappedControl<WrappedHiddenField>(riShipping, "hfProductListFormat", "{0} ￥{1}");
				if (wlbProducts.Items.Count == 0)
				{
					wlbProducts.Items.Clear();
					foreach (CartProduct cp in cartObject.Items)
					{
						var priceText = string.Empty;
						priceText = Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && cp.ProductOptionSettingList.HasOptionPrice
							? CurrencyManager.ToPrice(cp.Price) + " + " + CurrencyManager.ToPrice(cp.TotalOptionPrice)
							: CurrencyManager.ToPrice(cp.Price);
						wlbProducts.Items.Add(
							new ListItem(
								string.Format(whfProductListFormat.Value, cp.ProductJointName, priceText, cp.VariationId),
								cartObject.Items.IndexOf(cp).ToString()));
					}
					wlbProducts.SelectedIndex = 0;
				}

				// 商品選択ドロップダウン＆リンク表示制御
				var whcAllocateProductToShipping = GetWrappedControl<WrappedHtmlGenericControl>(riShipping, "hcAllocateProductToShipping");
				whcAllocateProductToShipping.Visible = true;

				// 商品未割当メッセージ表示制御
				var whcNoAlloacatedProductMessage = GetWrappedControl<WrappedHtmlGenericControl>(riShipping, "hcNoAlloacatedProductMessage");
				whcNoAlloacatedProductMessage.Visible = (cartObject.IsGift && (cartObject.Shippings[riShipping.ItemIndex].ProductCounts.Count == 0));
			}
		}
	}

	/// <summary>
	/// 配送先部分以下リフレッシュ
	/// </summary>
	/// <param name="riCart"></param>
	/// <param name="lCartShippings"></param>
	/// <param name="e"></param>
	private void RefreshCartShippings(RepeaterItem riCart, List<CartShipping> lCartShippings, EventArgs e)
	{
		InitComponentsOrderShipping();

		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		wrCartShippings.DataSource = lCartShippings;
		wrCartShippings.DataBind();
	}

	/// <summary>
	/// 注文送り主区分文言取得
	/// </summary>
	/// <param name="cartShipping"></param>
	/// <param name="strNewStr"></param>
	/// <param name="strOwnerStr"></param>
	/// <returns></returns>
	protected string GetOrderSenderAddrKbnString(CartShipping cartShipping, string strNewStr, string strOwnerStr)
	{
		switch (cartShipping.SenderAddrKbn)
		{
			case CartShipping.AddrKbn.New:
				return strNewStr;

			case CartShipping.AddrKbn.Owner:
				return strOwnerStr;

			default:
				return "";
		}
	}

	/// <summary>
	/// 注文配送先区分文言取得
	/// </summary>
	/// <param name="cartShipping"></param>
	/// <param name="strNewStr"></param>
	/// <param name="strOwnerStr"></param>
	/// <returns></returns>
	protected string GetOrderShippingAddrKbnString(CartShipping cartShipping, string strNewStr, string strOwnerStr)
	{
		switch (cartShipping.ShippingAddrKbn)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				return strNewStr;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				return strOwnerStr;

			default:
				return cartShipping.UserShippingName;
		}
	}

	/// <summary>
	/// 商品に割り当てリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAllocateProductToShipping_Click(object sender, System.EventArgs e)
	{
		RepeaterItem riShipping = (RepeaterItem)((LinkButton)sender).Parent.Parent;
		RepeaterItem riCart = (RepeaterItem)((LinkButton)sender).Parent.Parent.Parent.Parent;

		var whcErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(riShipping, "hcErrorMessage");
		var wlbProducts = GetWrappedControl<WrappedListBox>(riShipping, "lbProducts");

		if (string.IsNullOrEmpty(wlbProducts.SelectedValue) == false)
		{
			CartObject cartObject = this.CartList.Items[riCart.ItemIndex];
			CartProduct cpAdd = cartObject.Items[int.Parse(wlbProducts.SelectedValue)];

			if ((cartObject.Items.Find(product => (product == cpAdd)).Count + 1 <= cpAdd.ProductMaxSellQuantity))
			{
				whcErrorMessage.Visible = false;

				var find = cartObject.Shippings[riShipping.ItemIndex].ProductCounts.Find(pc => (pc.Product == cpAdd));
				if (find != null)
				{
					find.Count++;
				}
				else
				{
					cartObject.Shippings[riShipping.ItemIndex].ProductCounts.Add(new CartShipping.ProductCount(cpAdd, 1));
				}
			}
			else
			{
				whcErrorMessage.Visible = true;
				whcErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OVER_MAXSELLQUANTITY)
					.Replace("@@ 1 @@", cartObject.Items[wlbProducts.SelectedIndex].ProductName)
					.Replace("@@ 2 @@", cartObject.Items[wlbProducts.SelectedIndex].ProductMaxSellQuantity.ToString("F0"));
			}
		}

		// 商品数再計算
		RecalculateProductCount(riCart);

		// カート商品リフレッシュ
		RefreshCartProducts(riCart);
	}

	/// <summary>
	/// 商品割り当て解除リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lReleaseProductAllocation_Click(object sender, System.EventArgs e)
	{
		LinkButton lReleaseProductAllocation = (LinkButton)sender;
		RepeaterItem riProduct = (RepeaterItem)lReleaseProductAllocation.Parent.Parent.Parent;
		RepeaterItem riShipping = (RepeaterItem)riProduct.Parent.Parent;
		RepeaterItem riCart = (RepeaterItem)riShipping.Parent.Parent;

		// 商品をはずす
		this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex].ProductCounts.RemoveAt(riProduct.ItemIndex);

		// 商品数再計算
		RecalculateProductCount(riCart);

		// カート商品リフレッシュ
		RefreshCartProducts(riCart);
	}

	/// <summary>
	/// 再計算リンククリック（実際はテキストボックスのフォーカス外れ）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRecalculateCart_Click(object sender, System.EventArgs e)
	{
		RepeaterItem riProduct = GetOuterControl((LinkButton)sender, typeof(RepeaterItem));
		RepeaterItem riShipping = GetOuterControl(riProduct, typeof(RepeaterItem));
		RepeaterItem riCart = GetOuterControl(riShipping, typeof(RepeaterItem));

		var whcErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(riShipping, "hcErrorMessage");
		var wtbCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount");

		CartObject cartObject = this.CartList.Items[riCart.ItemIndex];
		CartShipping.ProductCount targetProductCount = cartObject.Shippings[riShipping.ItemIndex].ProductCounts[riProduct.ItemIndex];

		// 入力商品数取得
		int iCount;
		if (int.TryParse(StringUtility.ToHankaku(wtbCount.Text), out iCount) == false)
		{
			iCount = targetProductCount.Count;
		}
		iCount = (iCount > 0) ? iCount : 1;

		// カート内の対象商品合計取得
		int iProductCountTotal = cartObject.Items.Find(product => (product == targetProductCount.Product)).Count - targetProductCount.Count + iCount;

		// xxx商品購入制限チェック
		if (iProductCountTotal > targetProductCount.Product.ProductMaxSellQuantity)
		{
			this.ErrorMessages.Add(riCart.ItemIndex, riShipping.ItemIndex, OrderCommon.GetErrorMessage(OrderErrorcode.MaxSellQuantityError));
		}

		// 問題なければ更新
		if (this.ErrorMessages.Count != 0)
		{
			whcErrorMessage.Visible = true;
			whcErrorMessage.InnerText = this.ErrorMessages.Get(riCart.ItemIndex, riShipping.ItemIndex)
				.Replace("@@ 1 @@", cartObject.Shippings[riShipping.ItemIndex].ProductCounts[riProduct.ItemIndex].Product.ProductName)
				.Replace("@@ 2 @@", cartObject.Shippings[riShipping.ItemIndex].ProductCounts[riProduct.ItemIndex].Product.ProductMaxSellQuantity.ToString("F0"));
		}
		else
		{
			whcErrorMessage.Visible = false;
			targetProductCount.Count = iCount;
		}

		// 商品数再計算
		RecalculateProductCount(riCart);

		// 表示更新
		RefreshCartProducts(riCart);
	}

	/// <summary>
	/// 商品数再計算
	/// </summary>
	/// <param name="cartControl">カート一覧のコントロール</param>
	private void RecalculateProductCount(RepeaterItem cartControl)
	{
		var cartObject = this.CartList.Items[cartControl.ItemIndex];
		foreach (var cartProduct in cartObject.Items)
		{
			// cartProductをもつProductCountリスト取得＆数の合計計算
			var productCounts = cartObject.Shippings.Select(shipping => shipping.ProductCounts.Find(pcount => pcount.Product == cartProduct)).Where(pcount => pcount != null);
			int sum = productCounts.Sum(pcount => pcount.Count);
			// DBには保存したくないのでオブジェクトのみ更新
			cartProduct.CountSingle = sum;
			cartProduct.Calculate();
		}
		// 再計算処理
		cartObject.CalculateWithCartShipping();
	}

	/// <summary>
	/// 配送先変更リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>削除禁止（P0004_Endepaで利用）</remarks>
	protected void lbShippingModify_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING;

		// 次のページへ遷移
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING);
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, System.EventArgs e)
	{
		this.lbShippingModify_Click(sender, e);
	}

	/// <summary>
	/// 次ページへ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 配送先商品未割り当てチェック
		//------------------------------------------------------
		foreach (RepeaterItem riCart in WrCartList.Items)
		{
			if (this.CartList.Items[riCart.ItemIndex].IsGift)
			{
				var wrCartShipping = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
				foreach (RepeaterItem riShipping in wrCartShipping.Items)
				{
					if (this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex].ProductCounts.Count == 0)
					{
						var wlbProducts = GetWrappedControl<WrappedListBox>(riShipping, "lbProducts");
						wlbProducts.Focus();
						return;
					}
				}
			}
		}

		//------------------------------------------------------
		// カート商品未割り当てチェック・削除
		//------------------------------------------------------
		foreach (CartObject co in this.CartList.Items)
		{
			if (co.IsGift)
			{
				List<CartProduct> removeList = new List<CartProduct>();
				foreach (CartProduct cp in co.Items)
				{
					if (co.Shippings.Exists(shipping => shipping.ProductCounts.Exists(pcount => pcount.Product == cp)) == false)
					{
						removeList.Add(cp);
					}
				}
				co.Items.RemoveAll(cp => removeList.Contains(cp));
				co.CalculateWithCartShipping();
			}
		}

		//------------------------------------------------------
		// カートチェック
		//------------------------------------------------------
		CheckCartData();
		if (this.ErrorMessages.Count != 0)
		{
			hcErrorMessage.InnerText = this.ErrorMessages.Get();
			this.MaintainScrollPositionOnPostBack = false;
			return;
		}

		//------------------------------------------------------
		// 注文配送情報入力画面次へリンククリック処理（共通）
		//------------------------------------------------------
		if (lbNext_Click_OrderShipping_Others(sender, e) == false)
		{
			return;
		}

		// Display shipping date error message if exising
		if (DisplayShippingDateErrorMessage())
		{
			return;
		}

		// 熨斗・包装格納
		GetGiftWrappingPaperAndBagInput();

		//------------------------------------------------------
		// すべてのカートを再計算
		//------------------------------------------------------
		foreach (RepeaterItem riCart in WrCartList.Items)
		{
			// 商品数再計算
			RecalculateProductCount(riCart);

			// カート商品リフレッシュ
			RefreshCartProducts(riCart);
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;

		// 次のページへ遷移
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage);
	}

	/// <summary>戻るイベント格納用</summary>
	protected string BackEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + WlbBack.UniqueID + "', '', true, '', '', false, true))"; }
	}
	/// <summary>戻るonclick</summary>
	protected string BackOnClick
	{
		get { return "return true"; }
	}
	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.lbNext.UniqueID + "', '', true, '" + this.lbNext.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>確認画面へ遷移するか</summary>
	protected bool GotoConfirm
	{
		get { return (bool)ViewState["GotoConfirm"]; }
		set { ViewState["GotoConfirm"] = value; }
	}
}
