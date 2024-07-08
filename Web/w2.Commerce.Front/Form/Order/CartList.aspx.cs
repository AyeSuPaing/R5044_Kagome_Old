/*
=========================================================================================================
  Module      : カート画面処理(CartList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.UserDefaultOrderSetting;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.User;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Global.Region.Currency;
using w2.Domain.Coupon.Helper;
using w2.Domain.SubscriptionBox;
using w2.App.Common.DataCacheController;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using System.Web.UI;

public partial class Form_Order_CartList : CartListPage
{
	public WrappedLinkButton WlbTwoClickButton1 { get { return GetWrappedControl<WrappedLinkButton>("lbTwoClickButton1"); } }
	public WrappedLinkButton WlbTwoClickButton2 { get { return GetWrappedControl<WrappedLinkButton>("lbTwoClickButton2"); } }

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		this.DispNum = 0;
		this.HasOrderCombinedReturned = false;

		// セッションOrderCombineFromAmazonPayButtonを初期化する
		SessionManager.OrderCombineFromAmazonPayButton = true;

		//２クリックボタン押下判定初期化
		SessionManager.IsTwoClickButton = false;

		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			// プレビュー時、AmazonRequestが初期化されず例外が発生するのを防ぐ
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForOneTime();
			Reload();
			return;
		}

		if (!IsPostBack)
		{
			// カートリスト情報に決済手数料を紐付け、配送先情報で配送料含め再計算
			foreach (CartObject co in this.CartList.Items)
			{
				if (co.Payment != null)
				{
					// 決済手数料設定
					// 決済手数料は商品合計金額から計算する（ポイント等で割引された金額から計算してはいけない）
					co.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
						co.ShopId,
						co.Payment.PaymentId,
						co.PriceSubtotal,
						co.PriceCartTotalWithoutPaymentPrice);
				}

				// AmazonPay利用時の外部連携メモを削除
				if ((co.RelationMemo != null)
					&& (SessionManager.OrderCombineCartList == null)
					&& co.RelationMemo.Contains(Constants.CONST_RELATIONMEMO_AMAZON_PAY))
				{
					co.RelationMemo = string.Empty;
				}

				// カート配送先情報で再計算
				co.Calculate(false, false, false, false, 0, this.IsOrderCombined);
			}

			if (CanUseCartListLp())
			{
				var url = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST_LP;
				if ((Request[Constants.REQUEST_KEY_CART_ACTION] != null)
					|| (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID] != null))
				{
					var cartListAbsoluteUri = new Uri(Request.Url.AbsoluteUri);
					url += cartListAbsoluteUri.Query;
				}

				Response.Redirect(url);
			}

			if ((this.SubscriputionBoxProductListModify != null) && this.SubscriputionBoxProductListModify.Any())
			{
				foreach (var product in this.SubscriputionBoxProductListModify)
				{
					// 商品情報取得
					var dvProduct = ProductCommon.GetProductVariationInfo(this.ShopId, product.ProductId, product.VariationId, null);
					var index = this.CartList.Items.FindIndex(p => p.SubscriptionBoxCourseId == product.SubscriptionBoxCourseId);
					var info = (SessionManager.ProductOptionSettingList != null)
						? SessionManager.ProductOptionSettingList
						: new ProductOptionSettingList(product.ShopId, product.ProductId);
					if (this.CartList.Items[index].Items.Any(p => p.VariationId == product.VariationId))
					{
						var productIndex = this.CartList.Items[index].Items.FindIndex(p => p.VariationId == product.VariationId);
						if (product.ItemQuantity != this.CartList.Items[index].Items[productIndex].Count)
						{
							this.CartList.Items[index].Items[productIndex].Count = product.ItemQuantity;
							this.CartList.Items[index].Items[productIndex].CountSingle = product.ItemQuantity;
						}
					}
					else
					{
						this.CartList.AddProduct(
							dvProduct[0],
							Constants.AddCartKbn.SubscriptionBox,
							StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
							product.ItemQuantity,
							info,
							null,
							null,
							null,
							this.CartList.Items[index].SubscriptionBoxCourseId);
					}
				}
			}

			SessionManager.SubscriputionBoxProductListForTemp = this.SubscriputionBoxProductList;
			SessionManager.SubscriputionBoxProductListModifyForTemp = this.SubscriputionBoxProductListModify;

			this.SubscriputionBoxProductList = null;
			this.SubscriputionBoxProductListModify = null;
			SessionManager.ProductOptionSettingList = null;

			if (Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED)
			{
				SessionManager.IsMovedOnOrderConfirm = false;
			}

			// 頒布会のカート投入URLが存在する場合
			var subscriptionCourseId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID]);
			if (string.IsNullOrEmpty(subscriptionCourseId) == false
				&& (string.IsNullOrEmpty(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CART_ACTION]))
					|| (Request[Constants.REQUEST_KEY_CART_ACTION] != Constants.KBN_REQUEST_CART_ACTION_ADD_SUBSCRIPTIONBOX)))
			{
				AddProductsToCartListForSubscriptionBox(subscriptionCourseId);

				Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = this.ErrorMessages.Get();
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Adjust point and member rank by Cross Point api
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			}

			var cartObjList = this.CartList.Cast<CartObject>().ToList();
			// 定期会員フラグ設定
			SetFixedPurchaseMemberFlgForCartObject(cartObjList);

			// 定期専用カートがある場合、定期購入あり判定（定期カート分離用の）に更新する
			if (cartObjList.Any(cartObj => cartObj.IsFixedPurchaseOnly) || this.CartList.HasFixedPurchase)
			{
				cartObjList.ForEach(cartObj => cartObj.HasFixedPurchaseForCartSeparation = true);
			}

			//------------------------------------------------------
			// デフォルト配送先で再計算
			//------------------------------------------------------
			this.CartList.CalculateAllCart();

			// 注文同梱後の場合、注文同梱前のカート情報に戻す
			if (SessionManager.OrderCombineCartList != null)
			{
				var isOrderLimit = this.CartList.Items[0].IsOrderLimit;
				var productOrderLimitOrderIds = this.CartList.Items[0].ProductOrderLmitOrderIds;
				this.CartList = SessionManager.OrderCombineBeforeCartList;
				this.CartList.Items.ForEach(co =>
				{
					co.IsOrderLimit = isOrderLimit;
					co.ProductOrderLmitOrderIds = productOrderLimitOrderIds;
				});
				Session[Constants.SESSION_KEY_CART_LIST] = SessionManager.OrderCombineBeforeCartList;
				foreach (var item in this.CartList.Items)
				{
					item.CalculateWithDefaultShipping();
				}
				SessionManager.OrderCombineCartList = null;
				this.HasOrderCombinedReturned = true;
			}
			else
			{
				SessionManager.OrderCombineBeforeCartList = this.CartList;
				this.HasOrderCombinedReturned = false;
			}
			SessionManager.OrderCombinePaymentReselect = false;

			//2クリックボタン表示制御
			SetIsDisplayTwoClickButton();

			//------------------------------------------------------
			// 画面全体データバインド
			//------------------------------------------------------
			this.DataBind();

			//------------------------------------------------------
			// カート投入URLアクション系でリダイレクトしてきた場合
			//------------------------------------------------------
			if (StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART]) != "")
			{
				this.ErrorMessages.Add(StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART]));
				Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = null;
			}
			//------------------------------------------------------
			// カート投入URLアクション
			//------------------------------------------------------
			if (Request[Constants.REQUEST_KEY_CART_ACTION] != null)
			{
				// POST・GETに合わせてリクエスト情報取得
				AddCartHttpRequest addCartHttpRequest = new AddCartHttpRequest(Request, true);
				// リクエスト情報からカートに商品投入
				AddProductToCartFromRequest(addCartHttpRequest);

				// カート投入URLの指定方法に誤りがある場合メッセージ追加
				this.ErrorMessages.Add(addCartHttpRequest.ErrorMessages);

				// カートへ遷移する
				// カートから商品削除後、ブラウザの戻るボタンで再カート投入されないよう回避するためと、一部商品エラーでもパラメタ残さないようにするため。
				Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = this.ErrorMessages.Get();
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
			}

			//------------------------------------------------------
			// カート一覧画面表示
			//------------------------------------------------------
			if (this.CartList.Items.Count != 0)
			{
				//------------------------------------------------------
				// カートチェック
				//------------------------------------------------------
				CheckCartData();

				//------------------------------------------------------
				// 新規登録ポイント表示用ポイント表示
				//------------------------------------------------------
				if ((Constants.W2MP_POINT_OPTION_ENABLED) && (this.IsLoggedIn == false))
				{
					var pointRules = PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER);
					this.UserRegistPoint = 0;
					foreach (var pointRule in pointRules)
					{
						this.UserRegistPoint += pointRule.IncNum;
					}
				}
			}

			// Update LoginUserPoint
			if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn)
			{
				this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);
			}

			// カート情報更新
			Reload();

			if (Constants.TWOCLICK_OPTION_ENABLE)
			{
				var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
				// ログイン済みであれば、デフォルト注文払方法が存在するかの判定を設定する
				if (this.IsLoggedIn) this.CartList.SetDefaultPaymentExistForUserDefaultOrder(userDefaultOrderSetting);
				
				// 次へボタン設定
				NextPageSetting(userDefaultOrderSetting);

				// 配送先入力画面がスキップされ、配送方法が変更となる場合配送料金が合わなくなる為、予め配送方法、配送サービスを再設定する
				// ログインしていない場合は、注文確認画面は下記の処理を走るため、予め配送方法、配送サービスを再設定する必要がない
				if (this.IsLoggedIn) this.Process.CreateShippingMethodListOnDataBind();
			}
			else
			{
				// 次へボタン設定
				NextPageSetting();
			}
		}

		this.AmazonRequest = this.CartList.HasFixedPurchase
			? AmazonCv2Redirect.SignPayloadForReccuring(this.CartList.PriceCartListTotal)
			: AmazonCv2Redirect.SignPayloadForOneTime();
	}

	/// <summary>
	/// リピータイベント呼び出し
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void CallCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		rCartList_ItemCommand(source, e);

		//2クリック購入ボタン表示制御
		var isDisplay = SetIsDisplayTwoClickButton();
		this.WlbTwoClickButton1.Visible = this.WlbTwoClickButton2.Visible = isDisplay;
	}

	/// <summary>
	/// 2クリックボタン表示可否セット
	/// </summary>
	/// <returns>表示するか</returns>
	protected bool SetIsDisplayTwoClickButton()
	{
		if (Constants.TWOCLICK_OPTION_ENABLE
			&& Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
			&& (this.CartList.Items.Any(cart => (cart.HasFixedPurchase || cart.HasSubscriptionBox)) == false))
		{
			var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
			this.IsDisplayTwoClickButton = ((userDefaultOrderSetting == null)
				|| (userDefaultOrderSetting.UserShippingNo != null));

			return this.IsDisplayTwoClickButton;
		}

		return false;
	}

	/// <summary>
	/// 次へボタン設定
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法設定情報</param>
	private void NextPageSetting(UserDefaultOrderSettingModel userDefaultOrderSetting = null)
	{
		// 通常の配送先・支払方法に設定するのチェック状況をクリアする
		this.IsDefaultShippingChecked = null;
		this.IsDefaultPaymentChecked = null;

		// 注文同梱画面へ遷移する際に押下したボタンを初期化
		Session.Remove(Constants.SESSION_KEY_FROM_NEXT_BUTTON);

		// Remove session key real shop selection info
		Session.Remove(Constants.SESSION_KEY_REALSHOP_SELECTION_INFO);

		// 注文同梱済みの場合、注文同梱選択画面へ遷移
		if (SessionManager.OrderCombineCartList != null)
		{
			this.CartList = SessionManager.CartList;
			SessionManager.OrderCombineCartList = null;
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;
			Session[Constants.SESSION_KEY_FROM_NEXT_BUTTON] = true;
			return;
		}

		// 注文同梱対象がある場合、注文同梱選択画面へ遷移
		var isCombinableAmazonPay = (this.IsAmazonLoggedIn && Constants.AMAZON_PAYMENT_OPTION_ENABLED);
		if (Constants.ORDER_COMBINE_OPTION_ENABLED
			&& OrderCombineUtility.ExistCombinableOrder(this.LoginUserId, this.CartList, true, isCombinableAmazonPay))
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;
			Session[Constants.SESSION_KEY_FROM_NEXT_BUTTON] = true;
			return;
		}

		// 確認画面以外からの遷移の場合
		if ((Request.UrlReferrer != null)
			&& (Request.UrlReferrer.ToString().ToLower().Contains(Constants.PAGE_FRONT_ORDER_PAYMENT.ToLower())))
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
		}
		else if ((Request.UrlReferrer != null)
			&& (Request.UrlReferrer.ToString().ToLower().Contains(Constants.PAGE_FRONT_ORDER_CONFIRM.ToLower())))
		{
			this.CartList.CartNextPage = IsFixedPurchaseKbnEmpty()
				? Constants.PAGE_FRONT_ORDER_OWNER_DECISION
				: Constants.PAGE_FRONT_ORDER_CONFIRM;
			// レコメンド商品追加時？
			if (this.IsAddRecmendItem)
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_OWNER_DECISION;
			}

			if (this.CartList.Items.Any(cart => OrderCommon.IsAmazonPayment(cart.Payment.PaymentId)))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_OWNER_DECISION;
				this.CartList.Owner = null;
			}
		}
		else
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_OWNER_DECISION;

			// 画面遷移先とデフォルト注文方法を設定
			new UserDefaultOrderManager(this.CartList, userDefaultOrderSetting, this.IsAddRecmendItem)
				.SetNextPageAndDefaultOrderSetting(this.Process.SelectedShippingMethod != null);
		}
		if ((this.CartList.CartNextPage != Constants.PAGE_FRONT_ORDER_SHIPPING)
			&& (this.CartList.Items.Any(cart => cart.HasFixedPurchase && (cart.GetShipping().HasFixedPurchaseSetting == false)))
			&& (this.LoginUser != null))
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_SHIPPING;
		}
	}

	/// <summary>
	/// カート内の配送パターン空白チェック
	/// </summary>
	/// <returns>値は空白か</returns>
	private bool IsFixedPurchaseKbnEmpty()
	{
		var isEmpty = this.CartList.Items.Any(cart =>
			cart.HasFixedPurchase && string.IsNullOrEmpty(cart.Shippings[0].FixedPurchaseKbn));
		return isEmpty;
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_Changed(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		var ri = (RepeaterItem)useAllPointFlgInputMethod.Parent.Parent;

		this.CartList.Items[ri.ItemIndex].UseAllPointFlg = useAllPointFlgInputMethod.Checked;

		lbRecalculate_Click((useAllPointFlgInputMethod.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグデータバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_DataBinding(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		var ri = (RepeaterItem)useAllPointFlgInputMethod.Parent.Parent;

		useAllPointFlgInputMethod.Checked = this.CartList.Items[ri.ItemIndex].UseAllPointFlg;
	}

	/// <summary>
	/// クーポンリスト変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCouponList_TextChanged(object sender, EventArgs e)
	{
		var couponCodeList = (DropDownList)sender;
		var couponCodeTextbox = ((TextBox)couponCodeList.Parent.FindControl("tbCouponCode"));
		couponCodeTextbox.Text = couponCodeList.SelectedValue;

		lbRecalculate_Click((couponCodeList.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// モーダルクーポンBOX クーポン選択時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponSelect_Click(object sender, EventArgs e)
	{
		var lbCouponSelect = (LinkButton)sender;

		var selectedCouponCode = (HiddenField)lbCouponSelect.Parent.FindControl("hfCouponBoxCouponCode");
		var couponCode = (TextBox)lbCouponSelect.Parent.Parent.Parent.FindControl("tbCouponCode");
		var couponListDdl = (DropDownList)lbCouponSelect.Parent.Parent.Parent.FindControl("ddlCouponList");
		couponCode.Text = selectedCouponCode.Value;
		couponListDdl.SelectedValue = selectedCouponCode.Value;

		var ri = (RepeaterItem)lbCouponSelect.Parent.Parent.Parent.Parent.Parent;
		this.CartList.Items[ri.ItemIndex].CouponBoxVisible = false;

		lbRecalculate_Click((((LinkButton)sender).Parent.Parent.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// モーダルクーポンBOX 閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponBoxClose_Click(object sender, EventArgs e)
	{
		var ri = (RepeaterItem)((LinkButton)sender).Parent.Parent.Parent;
		this.CartList.Items[ri.ItemIndex].CouponBoxVisible = false;

		lbRecalculate_Click((((LinkButton)sender).Parent.Parent.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// クーポン入力方法変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		var ri = (RepeaterItem)couponInputMethod.Parent.Parent.Parent;

		ri.FindControl("ddlCouponList").Visible = (couponInputMethod.Text != CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		ri.FindControl("hgcCouponCodeInputArea").Visible = (couponInputMethod.Text == CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		((TextBox)ri.FindControl("tbCouponCode")).Text = "";
		((DropDownList)ri.FindControl("ddlCouponList")).SelectedValue = "";

		this.CartList.Items[ri.ItemIndex].CouponInputMethod = (StringUtility.ToEmpty(couponInputMethod.Text) == "")
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: couponInputMethod.Text;
		this.CartList.Items[ri.ItemIndex].Coupon = null;

		lbRecalculate_Click((couponInputMethod.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// クーポン入力方法データバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_DataBinding(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		var ri = (RepeaterItem)couponInputMethod.Parent.Parent.Parent;

		couponInputMethod.SelectedValue = (StringUtility.ToEmpty(this.CartList.Items[ri.ItemIndex].CouponInputMethod) == "")
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: this.CartList.Items[ri.ItemIndex].CouponInputMethod;
	}

	/// <summary>
	/// クーポンBOXクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbShowCouponBox_Click(object sender, EventArgs e)
	{
		var ri = (RepeaterItem)((LinkButton)sender).Parent.Parent;
		((TextBox)ri.FindControl("tbCouponCode")).Text = "";
		((DropDownList)ri.FindControl("ddlCouponList")).SelectedValue = "";

		this.CartList.Items[ri.ItemIndex].CouponBoxVisible = true;

		lbRecalculate_Click((((LinkButton)sender).Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// クーポン割引文字列取得
	/// </summary>
	/// <param name="coupon">ユーザークーポン詳細情報</param>
	/// <returns>クーポン割引文字列</returns>
	public static string GetCouponDiscountString(UserCouponDetailInfo coupon)
	{
		var freeShippingMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
		var discountPriceMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);
		if (coupon.DiscountPrice != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + CurrencyManager.ToPrice(coupon.DiscountPrice);
			return CurrencyManager.ToPrice(coupon.DiscountPrice);
		}
		if (coupon.DiscountRate != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%";
			return StringUtility.ToNumeric(coupon.DiscountRate) + "%";
		}
		if (CouponOptionUtility.IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			return freeShippingMessage;
		}
		return "-";
	}

	/// <summary>
	/// Load data to subcription box
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rCartList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var item = e.Item;
		var wddlSubscriptionBox = GetWrappedControl<WrappedDropDownList>(item, "ddlSubscriptionBox");
		if (wddlSubscriptionBox.HasInnerControl)
		{
			// load subcription box
			var wdivsubscriptionBox = GetWrappedControl<WrappedHtmlGenericControl>(item, "dvSubscriptionBox");
			var cart = (CartObject)e.Item.DataItem;
			if (cart == null)//cart null hidden div tag
			{
				wdivsubscriptionBox.Visible = false;
				return;
			}

			if (wdivsubscriptionBox.HasInnerControl == false) return;

			var productId = cart.Items[0].ProductId;
			var variationId = cart.Items[0].VariationId;
			var shopId = cart.Items[0].ShopId;
			var subscriptionBoxes = DataCacheControllerFacade.GetSubscriptionBoxCacheController().GetSubscriptionBoxesByProductId(productId, variationId, shopId);
			wddlSubscriptionBox.Visible = (subscriptionBoxes.Length > 1);
			wddlSubscriptionBox.DataSource = subscriptionBoxes;
			wddlSubscriptionBox.DataBind();
			wddlSubscriptionBox.SelectedValue = this.CartList.Items[0].SubscriptionBoxCourseId;
			wdivsubscriptionBox.Visible = (this.IsUserFixedPurchaseAble && subscriptionBoxes.Length > 0);
		}
	}

	/// <summary>
	/// ペイパル認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		// カート一覧ページ向けペイパル認証完了
		PayPalAuthCompleteForCartList(sender, e);
	}

	/// <summary>
	/// 必須商品判定
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>結果</returns>
	protected bool HasNecessaryProduct(string subscriptionBoxCourseId, string productId)
	{
		var result = this.Process.HasNecessaryProduct(subscriptionBoxCourseId, productId);
		return result;
	}

	/// <summary>
	/// オプション価格を持っているか
	/// </summary>
	/// <param name="cartObject">カートオブジェクト</param>
	/// <returns>オプション価格を持っているか</returns>
	protected bool HasProductOptionPrice(CartObject cartObject)
	{
		var hasProductOptionPrice = ProductOptionSettingHelper.HasProductOptionPriceInCart(cartObject);
		return hasProductOptionPrice;
	}

	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.lbNext.UniqueID + "', '', true, '" + this.lbNext.ValidationGroup + "', '', false, true))"; }
	}

	/// <summary>注文同梱がもとに戻されたか</summary>
	protected bool HasOrderCombinedReturned
	{
		get; set;
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>頒布会商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductList
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST] = value; }
	}
	/// <summary>頒布会更新商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductListModify
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST] = value; }
	}
	/// <summary>２クリックボタンを表示するかどうか</summary>
	public bool IsDisplayTwoClickButton { get; set; }
}
