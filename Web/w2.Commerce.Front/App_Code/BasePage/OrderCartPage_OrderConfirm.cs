/*
=========================================================================================================
  Module      : 注文カート系画面入力基底ページ(OrderCartPage_OrderConfirm.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using w2.App.Common;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchaseCombine;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Register;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Recommend;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;
using w2.Domain;
using w2.Common.Util;

/// <summary>
/// 注文系イベントハンドラ
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void OrderEventHandler(object sender, OrderEventArgs e);

///*********************************************************************************************
/// <summary>
/// 注文表示系ページ
/// </summary>
///*********************************************************************************************
public partial class OrderCartPage : OrderPageDisp
{
	#region 注文確認画面系処理

	/// <summary>
	/// カートリスト全チェック
	/// </summary>
	/// <remarks>エラーの場合OrderExceptionを発生させる</remarks>
	protected void AllCheckCartList()
	{
		//------------------------------------------------------
		// カート内商品チェック
		//------------------------------------------------------
		ProductCheckCartList();

		//------------------------------------------------------
		// 決済手数料設定チェック（エラーはカート画面で表示）
		//------------------------------------------------------
		foreach (CartObject cart in this.CartList.Items)
		{
			if (OrderCommon.CheckSetUpPaymentPrice(cart) != OrderErrorcode.NoError) throw new OrderException();
		}

		// デフォルト注文方法チェック
		var errorMessageKey = this.CartList.CheckDefaultOrderSettingAllCart(this.LoginUserId);
		if (string.IsNullOrEmpty(errorMessageKey) == false)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = CommerceMessages.GetMessages(errorMessageKey);
			Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + (string)Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK]);
		}

		//------------------------------------------------------
		// ユーザポイントチェック（エラーはカート画面で表示）
		// エラーが発生した場合、カート画面へ遷移させる(カート画面でエラー文言を表示させる)
		//------------------------------------------------------
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			if (this.IsLoggedIn)
			{
				// 利用ポイントチェック
				foreach (CartObject cart in this.CartList)
				{
					if (CheckUsePoint(cart) != "") throw new OrderException();
				}

				// 全利用ポイント取得
				var totalUsePoint = this.CartList.Items.Sum(cart => cart.UsePoint);

				// ユーザポイント情報取得
				UserPointObject userPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);

				// 本ポイントがマイナスだとポイントチェックでエラーが発生するので、0補正する
				decimal userPointCorrectedZero = (userPoint.PointUsable) > 0 ? userPoint.PointUsable : 0;

				// 本ポイント < 利用ポイント
				if (userPointCorrectedZero < totalUsePoint)
				{
					// ユーザポイント情報が更新されたため、セッションに格納
					this.LoginUserPoint = userPoint;

					// カート画面へ遷移
					throw new OrderException();
				}
			}
		}

		//------------------------------------------------------
		// クーポン情報チェック
		// エラーが発生した場合、カート画面へ遷移させる(カート画面でエラー文言を表示させる)
		//------------------------------------------------------
		if (this.CartList.CheckCouponUseInfoAllCart(this.LoginUserId, this.LoginUserMail) == false) throw new OrderException();

		// 注文時に定期会員割引の有効性をチェックし、無効であればカートページに遷移する
		if (IsApplyFixedPurchaseMemberDiscountInvaild()) throw new OrderException();
	}

	/// <summary>
	/// カートリスト カート商品チェック
	/// </summary>
	/// <remarks>エラーの場合OrderExceptionを発生させる</remarks>
	protected void ProductCheckCartList()
	{
		//------------------------------------------------------
		// カート内商品の各種チェック
		//------------------------------------------------------
		foreach (CartObject cart in this.CartList.Items)
		{
			// セット商品チェック（エラーはカート画面で表示）
			if (this.CartList.CheckProductSet(false).Length != 0)
			{
				throw new OrderException();
			}

			// 注文同梱の場合、DBに保存されているカートが注文同梱前、処理対象のものが注文同梱後のものであり、
			// 不整合があることは確定しているため、カート整合性のチェックはスキップする
			if ((this.IsOrderCombined == false)
				&& (Session[Constants.SESSION_KEY_STORE_PICKUP_ORDER_COMBINE] == null))
			{
				// カート不整合チェック（別セッションでカート商品が削除されると在庫チェックが行われず購入されてしまう）
				DataView cartItems = cart.GetCartProductsEither();
				foreach (CartProduct cp in cart.Items)
				{
					// カート商品存在チェック
					bool find = false;
					foreach (DataRowView item in cartItems)
					{
						if ((cp.ShopId == (string)item[Constants.FIELD_PRODUCTVARIATION_SHOP_ID])
							&& (cp.ProductId == (string)item[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID])
							&& (cp.VariationId == (string)item[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]))
						{
							find = true;
							break;
						}
					}
					if (find == false)
					{
						// カート商品情報が一致しない場合カート不整合エラーとする。（複数ブラウザ対応）
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_NO_ADJUSTMENT);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}
				}

				// カート商品整合性チェック（エラーはカート画面で表示）
				foreach (DataRowView item in cartItems)
				{
					CartProduct cp = cart.GetProductEither((string)item[Constants.FIELD_PRODUCT_SHOP_ID], (string)item[Constants.FIELD_PRODUCT_PRODUCT_ID], (string)item[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
					if (cp != null)	// 他セッションでカートが削除されていた場合に下記メソッドないでシステムラーが発生するため
					{
						var dvProduct = GetProduct(cp.ShopId, cp.ProductId, cp.VariationId);
						// ※商品マスタの定期購入フラグと比較したいため、GetProductから商品情報を取得する
						if ((dvProduct.Count == 0)
							|| (OrderCommon.CheckCartProduct(cp, dvProduct[0], this.LoginUserId) != OrderErrorcode.NoError))
						{
							throw new OrderException();
						}
						// 商品数量チェック（セッション改ざんの件）
						if ((cp.Count <= 0) || (cp.CountSingle <= 0))
						{
							throw new OrderException();
						}
					}
				}
			}

			Session[Constants.SESSION_KEY_STORE_PICKUP_ORDER_COMBINE] = null;

			// 商品明細金額チェック（セッション改ざんの件）
			foreach (CartProduct cp in cart.Items)
			{
				if ((cp.PriceSubtotal < 0) || (cp.PriceSubtotalSingle < 0))
				{
					throw new OrderException();
				}
			}
		}
	}

	#region イベント

	/// <summary>注文検証イベント</summary>
	public event OrderEventHandler OrderValidateEvent;
	/// <summary>注文完了イベント</summary>
	public event OrderEventHandler OrderCompleteEvent;
	/// <summary>注文失敗イベント</summary>
	public event OrderEventHandler OrderFailedEvent;

	/// <summary>
	/// イベントを紐付ける
	/// </summary>
	private void BindEvent()
	{
		OrderEventBinder binder = new OrderEventBinder();
		this.OrderValidateEvent += binder.OnOrderValidating;
		this.OrderCompleteEvent += binder.OnOrderCompleted;
		this.OrderFailedEvent += binder.OnOrderFailed;
	}

	/// <summary>
	/// 注文検証処理
	/// </summary>
	/// <param name="orderData">注文情報</param>
	/// <param name="errorMessage">メッセージ</param>
	/// <returns></returns>
	private bool OnOrderValidate(Hashtable orderData, List<CartProduct> productList, out string errorMessage)
	{
		errorMessage = "";

		if (this.OrderValidateEvent != null)
		{
			OrderEventArgs e = CreateOrderEventArgs(orderData, productList);
			this.OrderValidateEvent(this, e);
			errorMessage = e.ReturnMessage;
			return e.IsSuccess;
		}
		return true; // プラグインによる失敗では無いため
	}

	/// <summary>
	/// 注文失敗時処理
	/// </summary>
	/// <param name="orderData">注文情報</param>
	/// <param name="productList">カート商品リスト</param>
	private void OnOrderFailed(Hashtable orderData, List<CartProduct> productList)
	{
		if (this.OrderValidateEvent != null)
		{
			OrderEventArgs e = CreateOrderEventArgs(orderData, productList);
			this.OrderFailedEvent(this, e);
		}
	}

	/// <summary>
	/// 注文失敗時処理
	/// </summary>
	/// <param name="orderData">注文情報</param>
	/// <param name="productList">カート商品リスト</param>
	private void OnOrderCompleted(Hashtable orderData, List<CartProduct> productList)
	{
		if (this.OrderValidateEvent != null)
		{
			OrderEventArgs e = CreateOrderEventArgs(orderData, productList);
			this.OrderCompleteEvent(this, e);
		}
	}

	/// <summary>
	/// 注文イベント用引数のインスタンスを生成
	/// </summary>
	/// <param name="cartProductList">カート商品リスト</param>
	/// <param name="orderData">注文情報</param>
	/// <returns>注文イベント用引数</returns>
	private OrderEventArgs CreateOrderEventArgs(Hashtable orderData, List<CartProduct> cartProductList)
	{
		Hashtable orderDataForEvent = (Hashtable)orderData.Clone(); // 操作した内容が反映されない様コピーを利用する
		List<Hashtable> productList = ProductListToHashList(cartProductList);
		orderDataForEvent.Add("OrderProductList", productList);
		return new OrderEventArgs(orderDataForEvent);
	}

	/// <summary>
	/// カート商品リストからハッシュテーブルのリストへ変換
	/// </summary>
	/// <param name="cartProductList"></param>
	/// <returns></returns>
	private List<Hashtable> ProductListToHashList(List<CartProduct> cartProductList)
	{
		List<Hashtable> productList = new List<Hashtable>();

		foreach (CartProduct cp in cartProductList)
		{
			Hashtable product = new Hashtable();
			product.Add("ProductId", cp.ProductId);
			product.Add("VariationId", cp.VariationId);
			product.Add("BrandId", cp.BrandId);
			product.Add("CategoryId1", cp.CategoryId1);
			product.Add("IsFixedPurchase", cp.IsFixedPurchase);
			product.Add("IsSetItem", cp.IsSetItem);
			product.Add("Price", cp.Price);
			product.Add("PriceSubtotal", cp.PriceSubtotal);
			product.Add("SaleId", cp.ProductSaleId);
			productList.Add(product);
		}
		return productList;
	}

	#endregion

	/// <summary>
	/// 注文処理実行
	/// </summary>
	/// <param name="landingCartSessionKey">LPカートセッションキー（通常カートはnull）</param>
	/// <param name="registerUser">注文完了時会員登録のユーザー情報</param>
	/// <param name="landingCartNextPageForCheckSessionKey">チェック用のLPカートの次ページセッションキー（通常カートはnull）</param>
	/// <param name="contentsExecuteLog">Abテスト用コンテンツログモデル(注文実行時)</param>
	protected void ExecOrder(string landingCartSessionKey = null, UserModel registerUser = null, string landingCartNextPageForCheckSessionKey = null, ContentsLogModel contentsExecuteLog = null)
	{
		if (registerUser != null)
		{
			this.CartList.UpdateUserIdToCartDb(registerUser.UserId);
			this.CartList.Owner.OwnerKbn = registerUser.UserKbn;
			this.LoginUserId = registerUser.UserId;
			this.LoginUser = registerUser;
		}

		// 初期化処理
		BindEvent();

		//------------------------------------------------------
		// 決済種別金額範囲チェック
		//------------------------------------------------------
		foreach (CartObject cart in this.CartList)
		{
			string message;
			if (OrderCommon.ValidatePaymentPriceRange(cart, cart.Payment.PaymentId, out message) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = message;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// If credit credit is locked, go to the error page
			if (IsLockedPurchaseByCreditCard(cart))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDIT_AUTH_LOCK);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		foreach (var cart in this.CartList.Items.Where(cart => string.IsNullOrEmpty(cart.DeviceInfo)))
		{
			cart.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];
		}

		// Check error for EcPay
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			&& this.HasShippingConvenienceStore)
		{
			var errorsProductSizeAndTelNoForEcPays = new List<string>();
			// Check tel no for Taiwan
			var cart = this.CartList.Items[0];
			var isOwnerAddressInJapan = IsCountryJp(cart.Owner.AddrCountryIsoCode);
			var ownerTelNo1 = StringUtility.ToEmpty(cart.Owner.Tel1);
			var ownerTelNo2 = StringUtility.ToEmpty(cart.Owner.Tel2);
			var isTelNo1Valid = OrderCommon.CheckValidTelNoTaiwanForEcPay(
				isOwnerAddressInJapan
					? ownerTelNo1.Replace("-", string.Empty)
					: ownerTelNo1);
			var isTelNo2Valid = OrderCommon.CheckValidTelNoTaiwanForEcPay(
				isOwnerAddressInJapan
					? ownerTelNo2.Replace("-", string.Empty)
					: ownerTelNo2);

			if ((isTelNo1Valid == false)
				|| ((string.IsNullOrEmpty(cart.Owner.Tel2) == false)
					&& (isTelNo2Valid == false)))
			{
				errorsProductSizeAndTelNoForEcPays.Add(
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TEL_NO_NOT_TAIWAN));
			}

			foreach (var item in this.CartList.Items)
			{
				var deliveryCompanyId = item.Shippings[0].DeliveryCompanyId;

				// Check valid weight and price
				if (deliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)
				{
					var shippingReceivingStoreType = item.Shippings[0].ShippingReceivingStoreType;
					var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(shippingReceivingStoreType);
					if (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(item, shippingReceivingStoreType))
					{
						errorsProductSizeAndTelNoForEcPays.Add(
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
								.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
								.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString()));
					}
				}

				// Check shipping service: yamato EcPay or home delivery EcPay
				if ((Constants.SHIPPING_SERVICE_YAMATO_FOR_ECPAY.Contains(deliveryCompanyId) == false)
					&& (deliveryCompanyId != Constants.SHIPPING_SERVICE_HOME_DELIVERY_FOR_ECPAY))
				{
					continue;
				}

				// Check error product size
				var sumProductSize = OrderCommon.SumProductSizeForEcPay(item);
				if (sumProductSize > Constants.CONST_PRODUCT_SIZE_LIMIT)
				{
					errorsProductSizeAndTelNoForEcPays.Add(
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TOTAL_SIZE_OF_PRODUCT_MORE_THAN_LIMIT)
							.Replace("@@ 1 @@", sumProductSize.ToString())
							.Replace("@@ 2 @@", Constants.CONST_PRODUCT_SIZE_LIMIT.ToString()));
				}
			}

			if (errorsProductSizeAndTelNoForEcPays.Count > 0)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join(
					Environment.NewLine,
					errorsProductSizeAndTelNoForEcPays.Select(error => error).ToArray());
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		// 最終カート投入商品ID取得（注文処理でカートDBが削除される前に実行）
		// 注文同梱の場合、注文同梱前カート情報から最終カート投入商品IDを取得
		string lastAddedCartProductId = (this.IsOrderCombined == false)
			? GetLastAddedCartProductId(this.CartList)
			: GetLastAddedCartProductId(SessionManager.OrderCombineBeforeCartList);

		// 注文同梱の場合、親注文が注文同梱不可の場合処理中止
		if (this.IsOrderCombined)
		{
			foreach (var cart in this.CartList.Items)
			{
				// 注文同梱時
				var parentOrderId = cart.OrderCombineParentOrderId;
				var errMsg = OrderCombineUtility.IsPossibleToOrderCombine(parentOrderId, true);

				if (string.IsNullOrEmpty(errMsg) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = errMsg;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}

		// Set data for PayPal cooperation info
		if (this.HasPayPalPayment)
		{
			this.CartList.PayPalCooperationInfo = SessionManager.PayPalCooperationInfo;
		}

		//------------------------------------------------------
		// 注文処理
		//------------------------------------------------------
		var register = new OrderRegisterFront(this.IsLoggedIn);

		string userId = (this.IsLoggedIn)
			? this.LoginUserId
			: UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);

		// 元注文の定期情報(定期情報を持っているのは元注文のみと想定)
		var fixedPurchaseId = this.IsOrderCombined
			? this.CartList.Items
				.Select(cart => (cart.FixedPurchase != null) ? cart.FixedPurchase.FixedPurchaseId : string.Empty)
				.FirstOrDefault(fpId => (string.IsNullOrEmpty(fpId) == false)) ?? string.Empty
			: string.Empty;

		var firstCartId = this.CartList.Items[0].CartId;

		// 商品同梱して注文実行
		var disablePaymentCancelOrderId = string.Empty;
		var excludeOrderIds = this.IsOrderCombined ? this.CartList.Items.Select(item => item.OrderCombineParentOrderId).ToArray() : null;
		var successCartForExternalPayment = new Dictionary<string, CartObject>();
		using (var productBundler = new ProductBundler(
			this.CartList.Items,
			userId,
			SessionManager.AdvCodeFirst,
			SessionManager.AdvCodeNow,
			excludeOrderIds,
			this.LoginUserHitTargetListIds,
			true))
		{
			//Check Valid Weight All Cart Has Product Bundler For Convenience Store
			foreach (var cart in productBundler.CartList)
			{
				foreach (var shipping in cart.Shippings.Where(item => (item.ConvenienceStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)))
				{
					var shippingReceivingStoreType = shipping.ShippingReceivingStoreType;
					var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(shippingReceivingStoreType);
					if (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(cart, shippingReceivingStoreType))
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
								.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
								.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString());
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}
				}
			}

			foreach (CartObject cart in productBundler.CartList)
			{
				var orderId = string.Empty;

				// Payment Atone Or Aftee: No Created Order Id Because Created Earlier
				if ((cart.Payment != null)
					&& ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
						|| (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)))
				{
					cart.IsNotUpdateInformationForPaymentAtoneOrAftee = ((string.IsNullOrEmpty(cart.OrderId) == false)
						&& (new OrderService().Get(cart.OrderId) == null));

					if (cart.IsNotUpdateInformationForPaymentAtoneOrAftee) orderId = cart.OrderId;
				}

				cart.CalculatePointFamily();

				Hashtable order = CreateOrderInfo(
					cart,
					string.IsNullOrEmpty(orderId)
						? OrderCommon.CreateOrderId(Constants.CONST_DEFAULT_SHOP_ID)
						: orderId,
					userId,
					landingCartSessionKey);

				OrderRegisterBase.ResultTypes result = OrderRegisterBase.ResultTypes.Success;

				// 検証時イベント処理
				string message;
				if (OnOrderValidate(order, cart.Items, out message) == false)
				{
					result = OrderRegisterBase.ResultTypes.Fail;
					this.DispErrorMessages.Add(message);
				}
				// When shipping Convenience if store not exist => Fail order
				if ((result == OrderRegisterBase.ResultTypes.Success) && Constants.TWOCLICK_OPTION_ENABLE)
				{
					var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
					if (userDefaultOrderSetting != null)
					{
						if (((userDefaultOrderSetting.UserShippingNo) != null)
							&& ((userDefaultOrderSetting.UserShippingNo) > 0))
						{
							foreach (var shipping in cart.Shippings.Where(item => item.ConvenienceStoreFlg
								== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
							{
								if ((OrderCommon.CheckIdExistsInXmlStore(shipping.ConvenienceStoreId) == false)
									&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false))
								{
									result = OrderRegisterBase.ResultTypes.Fail;
									Session[Constants.SESSION_KEY_ERROR_MSG] =
										WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE);
									Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
								}
							}
						}
					}
				}

				// リアルタイム累計購入回数取得
				var orderCount = (this.IsLoggedIn) ? new UserService().Get(userId).OrderCountOrderRealtime : 0;
				order.Add(Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, orderCount);

				// 定期2回目商品切替を利用している場合、配送パターンが切り替わる可能性がある為切替前に配送パターンを保持する
				if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED && cart.HasFixedPurchase)
				{
					var fixedPurchasePattern = cart.GetShipping().GetFixedPurchaseShippingPatternString();
					if (order.ContainsKey(Constants.FIXED_PURCHASE_FISRT_PATTERN_STRINGS))
					{
						order[Constants.FIXED_PURCHASE_FISRT_PATTERN_STRINGS] = fixedPurchasePattern;
					}
					else
					{
						order.Add(Constants.FIXED_PURCHASE_FISRT_PATTERN_STRINGS, fixedPurchasePattern);
					}
				}

				// 注文確認ページレコメンドで注文確定完了場合、レコメンド履歴更新
				if ((result == OrderRegisterBase.ResultTypes.Success)
					&& (cart.HasRecommendItem)
					&& (this.CartList.RecommendOrderConfirm != null)
					&& (this.CartList.RecommendHistoryNoOrderConfirm != 0)
					&& (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDER_ORDER_ID]) == false))
				{
					new RecommendService().UpdateBuyOrderedFlg(
						this.CartList.RecommendOrderConfirm.ShopId,
						this.CartList.RecommendOrderConfirm.RecommendId,
						this.CartList.UserId,
						this.CartList.RecommendHistoryNoOrderConfirm,
						Constants.FLG_LASTCHANGED_USER,
						(string)order[Constants.FIELD_ORDER_ORDER_ID]);

					this.CartList.RecommendHistoryNoOrderConfirm = 0;
					this.CartList.RecommendOrderConfirm = null;
				}

				// 注文実行
				if (result == OrderRegisterBase.ResultTypes.Success)
				{
					// 注文者のリージョンデータを最新に更新
					cart.Owner.UpdateRegion(RegionManager.GetInstance().Region);

					if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE)
					{
						order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] =
							SessionManager.MailForUserRegisterWhenOrderComplete;
					}

					var combinedCancelOrders = new List<OrderModel>();
					var isAtodeneReauth = string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
						? false
						: OrderCommon.IsAtodeneReauthByCombine(
							order,
							cart.OrderCombineParentOrderId.Split(','),
							this.IsOrderCombined,
							out combinedCancelOrders);
					if (isAtodeneReauth)
					{
						// 注文同梱で同梱されるものにAtodeneがあり、なおかつ新しい受注もAtodeneの場合、再与信を行う
						var reauthAtodeneOrder = combinedCancelOrders.OrderByDescending(old => old.LastBilledAmount).FirstOrDefault();
						result = register.ExecReauthAndNewOrder(
							reauthAtodeneOrder,
							order,
							cart,
							(this.IsLoggedIn == false),
							(this.CartList.Items.IndexOf(cart) == 0),
							this.IsOrderCombined,
							true,
							out disablePaymentCancelOrderId);
					}
					else
					{
						result = register.Exec(
							order,
							cart,
							(this.IsLoggedIn == false),
							(this.CartList.Items.IndexOf(cart) == 0),
							this.IsOrderCombined,
							(registerUser != null),
							false,
							contentsExecuteLog);
					}
				}

				// 注文同梱の場合の追加更新
				if ((result != OrderRegisterBase.ResultTypes.Fail) && (this.IsOrderCombined))
				{
					if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
						&& Constants.PAYMENT_SETTING_GMO_3DSECURE)
						break;
					try
					{
						var parentOrderId = cart.OrderCombineParentOrderId;
						var parentOrder = new OrderService().Get(parentOrderId);
						var combineBeforeCartList = SessionManager.OrderCombineBeforeCartList;
						orderId = (string) order[Constants.FIELD_ORDER_ORDER_ID];
						using (var accessor = new SqlAccessor())
						{
							accessor.OpenConnection();
							accessor.BeginTransaction();

							OrderCombineUtility.UpdateOrderCombineFlg(
								orderId,
								true,
								Constants.FLG_LASTCHANGED_USER,
								UpdateHistoryAction.DoNotInsert, accessor);

							// 注文同梱元注文キャンセル失敗フラグ（拡張ステータス46）をオンにする
							OrderCombineUtility.UpdateOrderCombineCancelFailedFlg(
								orderId,
								true,
								Constants.FLG_LASTCHANGED_USER,
								UpdateHistoryAction.DoNotInsert,
								accessor);

							// 親注文の拡張ステータス情報を元に拡張ステータス情報を更新
							var updateResult = OrderCombineUtility.UpdateExtendStatusFromParentOrder(
								orderId,
								parentOrderId,
								Constants.FLG_LASTCHANGED_USER,
								UpdateHistoryAction.DoNotInsert,
								accessor);
							result = (updateResult)
								? OrderRegisterBase.ResultTypes.Success
								: OrderRegisterBase.ResultTypes.Fail;

							if (Constants.GLOBAL_OPTION_ENABLE)
							{
								//グローバル対応 親注文のリージョンデータを引き継ぐ
								var newOrderModel = new OrderService().Get(orderId, accessor);
								newOrderModel.Owner.AccessCountryIsoCode = parentOrder.Owner.AccessCountryIsoCode;
								newOrderModel.Owner.DispCurrencyCode = parentOrder.Owner.DispCurrencyCode;
								newOrderModel.Owner.DispCurrencyLocaleId = parentOrder.Owner.DispCurrencyLocaleId;
								newOrderModel.Owner.DispLanguageCode = parentOrder.Owner.DispLanguageCode;
								newOrderModel.Owner.DispLanguageLocaleId = parentOrder.Owner.DispLanguageLocaleId;
								new OrderService().UpdateOwnerForModify(
									newOrderModel.Owner,
									Constants.FLG_LASTCHANGED_USER,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}

							// 親注文が定期購入の場合
							if (cart.IsCombineParentOrderHasFixedPurchase
								&& (result == OrderRegisterBase.ResultTypes.Success))
							{
								result = ExecFixedPurchaseCombine(
									parentOrderId,
									orderId,
									userId,
									combineBeforeCartList.Items[0],
									cart,
									parentOrder,
									accessor)
										? OrderRegisterBase.ResultTypes.Success
										: OrderRegisterBase.ResultTypes.Fail;
							}

							if (result == OrderRegisterBase.ResultTypes.Success)
							{
								accessor.CommitTransaction();
							}
							else
							{
								accessor.RollbackTransaction();
								var errorMessage = String.Format("{0}{1}{2}", "注文同梱の場合の追加更新がロールバックされました。", "注文ID：",
									orderId);
								FileLogger.WriteError(errorMessage);
							}
							
						}
					}
					catch (Exception exception)
					{
						//エラーログに書き込み、エラーページに遷移させる
						FileLogger.WriteError(exception);
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDERCOMBINE_ERROR);
						this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}
				}

				// Update User Extend
				UserExtendModel userExtend = null;
				var service = new UserService();
				if ((result == OrderRegisterBase.ResultTypes.Success)
					&& (cart.Payment != null))
				{
					switch (cart.Payment.PaymentId)
					{
						case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
						case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
							var usrex = Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID;
							var token = cart.TokenAtone;
							if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
							{
								usrex = Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID;
								token = cart.TokenAftee;
							}

							if (this.IsLoggedIn)
							{
								if (this.LoginUser.UserExtend != null)
								{
									this.LoginUser.UserExtend.UserExtendDataValue[usrex] = token;
									userExtend = this.LoginUser.UserExtend;
								}
							}
							else
							{
								var user = service.Get(userId);
								if (user != null)
								{
									user.UserExtend.UserExtendDataValue[usrex] = token;
									userExtend = user.UserExtend;
								}
							}
							break;
					}

					if (userExtend != null)
					{
						service.UpdateUserExtend(
							userExtend,
							userId,
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.DoNotInsert);
					}
				}

				if (result == OrderRegisterBase.ResultTypes.Success)
				{
					// ログイン AND 最後のカートの場合、デフォルト注文方法登録・更新する
					if (this.IsLoggedIn && Constants.TWOCLICK_OPTION_ENABLE && (this.CartList.Items.IndexOf(cart) == (productBundler.CartList.Count - 1)))
					{
						InsertOrUpdateDefaultOrderSetting(this.CartList);
					}

					OnOrderCompleted(order, cart.Items);

					// 注文同梱の場合、注文同梱前のカート情報を削除
					if (this.IsOrderCombined
						&& (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
					{
						var beforeCartList = SessionManager.OrderCombineBeforeCartList;
						beforeCartList.Items.Select(item => OrderCommon.DeleteCart(item.CartId));
					}

					// LP注文の場合、LP遷移チェックを初期化
					if (string.IsNullOrEmpty(landingCartNextPageForCheckSessionKey) == false)
					{
						Session[landingCartNextPageForCheckSessionKey] = null;
					}

					if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && this.IsLoggedIn)
					{
						SessionManager.MailForUserRegisterWhenOrderComplete = null;
					}

					// ログインユーザー情報更新
					if (this.IsLoggedIn)
					{
						var user = new UserService().Get(userId);
						this.LoginUser = user;
					}
				}
				else if (result == OrderRegisterBase.ResultTypes.Skip)
				{
					// ログイン AND 最後のカートの場合、デフォルト注文方法登録・更新する
					if (this.IsLoggedIn && Constants.TWOCLICK_OPTION_ENABLE && (this.CartList.Items.IndexOf(cart) == (productBundler.CartList.Count - 1)))
					{
						InsertOrUpdateDefaultOrderSetting(this.CartList);
					}

					if (string.IsNullOrEmpty(landingCartNextPageForCheckSessionKey) == false)
					{
						Session[landingCartNextPageForCheckSessionKey] = null;
					}

					// CrossPoint連携のためカート情報を保存
					successCartForExternalPayment.Add((string)order[Constants.FIELD_ORDER_ORDER_ID], cart);

					continue; // スキップ（外部決済など）
				}
				else if (result == OrderRegisterBase.ResultTypes.Fail)
				{
					// Credit credit trial count-1
					if (IsFailByCreditCard(register, cart))
					{
						CreditAuthAttackBlocker.Instance.DecreasePossibleTrialCount(this.LoginUserId, Request.UserHostAddress);
					}

					if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE
						&& (registerUser != null)
						&& (new UserService().Get(this.LoginUserId) == null))
					{
						this.CartList.UpdateUserIdToCartDb("");
						this.CartList.Owner.OwnerKbn = this.IsSmartPhone
							? Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_GUEST
							: Constants.FLG_ORDEROWNER_OWNER_KBN_PC_GUEST;
						this.LoginUserId = null;
					}

					// 失敗イベント処理
					OnOrderFailed(order, cart.Items);
					break;	// 失敗していたらここで終了
				}

				if (cart.HasFixedPurchase
					&& (cart.GetShipping().FirstShippingDate != DateTime.MinValue))
				{
					order[Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE] = cart.GetShipping().FirstShippingDate;
				}
			} // foreach (CartObject coCart in this.CartList)
		}

		//------------------------------------------------------
		// GoogleAnalyticsタグ制御用注文IDをクッキーにセット
		//------------------------------------------------------
		foreach (Hashtable order in register.SuccessOrders)
		{
			CookieManager.SetCookie(
				Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + (string)order[Constants.FIELD_ORDER_ORDER_ID],
				"",
				Constants.PATH_ROOT,
				DateTime.Now.AddHours(1)); // とりあえず1時間
		}

		//------------------------------------------------------
		// パラメタ格納して決済画面へ
		//------------------------------------------------------
		// パラメタ格納
		Hashtable param = new Hashtable();
		param.Add("user_id", userId);
		param.Add("order", register.SuccessOrders);
		param.Add("cancel_orders", new List<Hashtable>());
		param.Add("zeus_order_3dsecure", register.ZeusCard3DSecurePaymentOrders);   // 3Dセキュア認証の注文
		param.Add("zeus_linkpoint", register.ZeusLinkPointPaymentOrders);	// LinkPointの注文
		param.Add("order_docomo", register.DocomoPaymentOrder);					// ドコモケータイ払いの注文
		param.Add("order_sbps_multi", register.SBPSMultiPaymentOrders);         // SBPSマルチ決済
		param.Add("order_linepay", register.LinePayOrders);			// LINEペイ決済
		param.Add("googleanaytics_params", register.GoogleAnalyticsParams);		// GoogleAnalytics用パラメータ
		param.Add("error", register.ErrorMessages);
		param.Add("alert", register.AlertMessages);
		param.Add("last_added_cart_product", lastAddedCartProductId);
		param.Add("order_amazonpay_cv2", register.AmazonPayCv2Orders);
		param.Add("order_yamato_ka_sms_order", register.YamatoKaSmsOrders);
		param.Add("order_rakuten", register.RakutenOrders);
		param.Add(Constants.SESSION_KEY_PAYMENT_CREDIT_ZCOM_ORDER_3DSECURE, register.ZcomCard3DSecurePaymentOrders);
		param.Add("veritrans_order_3dsecure", register.VeriTrans3DSecurePaymentOrders);
		param.Add("paypay_order", register.PaypayOrders);
		param.Add("rakuten_order_3dsecure", register.RakutenCard3DSecurePaymentOrders);   // 楽天3Dセキュア認証の注文
		param.Add("disable_payment_cancel_order_id", disablePaymentCancelOrderId);
		param.Add("gmo_order_3dsecure", register.GmoCard3DSecurePaymentOrders); // 3Dセキュア認証の注文
		param.Add(Constants.SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE, register.YamatoCard3DSecurePaymentOrders);
		param.Add("paygent_order_3dsecure", register.PaygentCard3DSecurePaymentOrders);
		// CrossPoint用にカート情報保持
		param.Add("carts", successCartForExternalPayment);
		param.Add("order_boku", register.BokuPaymentOrders);
		param.Add("gmo_transaction_result", register.GmoTransactionResult); // GMO承認結果
		param.Add("order_gmoatokara", register.GmoAtokaraOrders);
		// Paidy Paygent
		param.Add("paidy_paygent_order", register.PaidyPaygentOrders);

		// Set Data For Payment EcPay
		var isExistsPaymentEcPay = register
			.SuccessCarts
			.Any(currentCart => (currentCart.Payment != null)
				&& (currentCart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY));
		if (isExistsPaymentEcPay)
		{
			var requestEcPay = ECPayUtility.CreateRequestForRegistOrder(register.SuccessCarts[0]);
			param[DATA_ECPAY] = requestEcPay;
		}

		// Set Data For Payment NewebPay
		var isExistsPaymentNewebPay = register
			.SuccessCarts
			.Any(currentCart => (currentCart.Payment != null)
				&& (currentCart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));
		if (isExistsPaymentNewebPay)
		{
			var requestNewebPay = NewebPayUtility.CreateRegistRequest(register.SuccessCarts[0]);
			param[DATA_NEWEBPAY] = requestNewebPay;
		}

		Session[Constants.SESSION_KEY_PARAM] = param;

		// 画面遷移の正当性チェックのため遷移先ページURLを設定し画面遷移（エラーがある場合でも遷移先で判定してエラー画面へ遷移する）
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// 注文完了時会員登録時、ログイン後ページ遷移
		if ((registerUser != null) && this.IsLoggedIn)
		{
			ExecLoginSuccessActionAndGoNextInner(
				registerUser,
				this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT,
				UpdateHistoryAction.Insert);
		}

		if (register.IsExpiredYamatoKwcCredit)
		{
			var paymentPageUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT;
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = paymentPageUrl;

			HttpContext.Current.Session[CommerceMessages.ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED]
				= MessageManager.GetMessages(
					CommerceMessages.ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED,
					new string[1] { new UrlCreator(paymentPageUrl).CreateUrl() });

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// 注文情報取得
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="orderId">注文ID</param>
	/// <param name="userId">ユーザID</param>
	/// <param name="landingCartSessionKey">LPカートセッションキー（通常カートはnull）</param>
	/// <returns>注文情報</returns>
	protected Hashtable CreateOrderInfo(CartObject cart, string orderId, string userId, string landingCartSessionKey = null)
	{
		Hashtable order = OrderCommon.CreateOrderInfo(
			cart,
			orderId,
			userId,
			"00pc",
			"",
			Request.ServerVariables["REMOTE_ADDR"],
			SessionManager.AdvCodeFirst,
			StringUtility.ToEmpty((cart.AdvCodeNew == null) ? Session[Constants.SESSION_KEY_ADVCODE_NOW] : cart.AdvCodeNew),
			Constants.FLG_LASTCHANGED_USER);

		// LPカートセッションキー格納
		order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] = landingCartSessionKey;

		// GMO後払い用HTTPヘッダ情報
		if ((cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo))
		{
			var headers = string.Format("{0};:{1};:{2};:{3};:{4};:{5};:{6};:{7};:{8};:{9};:{10};:{11};:{12};:{13};:{14};:{15};:{16};:",
				this.Request.Headers.AllKeys.Contains("Accept") ? StringUtility.ToEmpty(this.Request.Headers["Accept"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Charset") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Charset"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Encoding") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Encoding"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Language") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Language"]) : "",
				this.Request.Headers.AllKeys.Contains("Client-IP") ? StringUtility.ToEmpty(this.Request.Headers["Client-IP"]) : "",
				this.Request.Headers.AllKeys.Contains("Connection") ? StringUtility.ToEmpty(this.Request.Headers["Connection"]) : "",
				this.Request.Headers.AllKeys.Contains("X-Do-Not-Track") ? StringUtility.ToEmpty(this.Request.Headers["X-Do-Not-Track"])
					: this.Request.Headers.AllKeys.Contains("DNT") ? StringUtility.ToEmpty(this.Request.Headers["DNT"]) : "",
				this.Request.Headers.AllKeys.Contains("Host") ? StringUtility.ToEmpty(this.Request.Headers["Host"]) : "",
				this.Request.Headers.AllKeys.Contains("Referrer") ? StringUtility.ToEmpty(this.Request.Headers["Referrer"]) : "",
				this.Request.Headers.AllKeys.Contains("User-Agent") ? StringUtility.ToEmpty(this.Request.Headers["User-Agent"]) : "",
				this.Request.Headers.AllKeys.Contains("Keep-Alive") ? StringUtility.ToEmpty(this.Request.Headers["Keep-Alive"]) : "",
				this.Request.Headers.AllKeys.Contains("UA-CPU") ? StringUtility.ToEmpty(this.Request.Headers["UA-CPU"]) : "",
				this.Request.Headers.AllKeys.Contains("Via") ? StringUtility.ToEmpty(this.Request.Headers["Via"]) : "",
				this.Request.Headers.AllKeys.Contains("X-Forwarded-For") ? StringUtility.ToEmpty(this.Request.Headers["X-Forwarded-For"]) : "",
				string.Join("::",
					this.Request.Headers.AllKeys
					.Where(x => x != "Accept" && x != "Accept-Charset" && x != "Accept-Encoding" && x != "Accept-Language" && x != "Client-IP" && x != "Connection"
						&& x != "X-Do-Not-Track" && x != "DNT" && x != "Host" && x != "Referrer" && x != "User-Agent" && x != "Keep-Alive" && x != "UA-CPU" && x != "Via" && x != "X-Forwarded-For")
						.Select(x => string.Format("{0}--{1}", x, StringUtility.ToEmpty(this.Request.Headers[x])))
						.ToArray()),
				StringUtility.ToEmpty(this.Request.UserHostAddress),
				"");
			order.Add("gmo_http_headers", headers);
		}

		// スコア後払い用HTTPヘッダ情報
		if ((cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score))
		{
			var headers = string.Format("{0};:{1};:{2};:{3};:{4};:{5};:{6};:{7};:{8};:{9};:{10};:{11};:{12};:{13};:{14};:{15};:{16};:",
				this.Request.Headers.AllKeys.Contains("Accept") ? StringUtility.ToEmpty(this.Request.Headers["Accept"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Charset") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Charset"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Encoding") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Encoding"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Language") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Language"]) : "",
				this.Request.Headers.AllKeys.Contains("Client-IP") ? StringUtility.ToEmpty(this.Request.Headers["Client-IP"]) : "",
				this.Request.Headers.AllKeys.Contains("Connection") ? StringUtility.ToEmpty(this.Request.Headers["Connection"]) : "",
				this.Request.Headers.AllKeys.Contains("X-Do-Not-Track") ? StringUtility.ToEmpty(this.Request.Headers["X-Do-Not-Track"])
					: this.Request.Headers.AllKeys.Contains("DNT") ? StringUtility.ToEmpty(this.Request.Headers["DNT"]) : "",
				this.Request.Headers.AllKeys.Contains("Host") ? StringUtility.ToEmpty(this.Request.Headers["Host"]) : "",
				this.Request.Headers.AllKeys.Contains("Referrer") ? StringUtility.ToEmpty(this.Request.Headers["Referrer"]) : "",
				this.Request.Headers.AllKeys.Contains("User-Agent") ? StringUtility.ToEmpty(this.Request.Headers["User-Agent"]) : "",
				this.Request.Headers.AllKeys.Contains("Keep-Alive") ? StringUtility.ToEmpty(this.Request.Headers["Keep-Alive"]) : "",
				this.Request.Headers.AllKeys.Contains("UA-CPU") ? StringUtility.ToEmpty(this.Request.Headers["UA-CPU"]) : "",
				this.Request.Headers.AllKeys.Contains("Via") ? StringUtility.ToEmpty(this.Request.Headers["Via"]) : "",
				this.Request.Headers.AllKeys.Contains("X-Forwarded-For") ? StringUtility.ToEmpty(this.Request.Headers["X-Forwarded-For"]) : "",
				string.Join("::",
					this.Request.Headers.AllKeys
					.Where(x => x != "Accept" && x != "Accept-Charset" && x != "Accept-Encoding" && x != "Accept-Language" && x != "Client-IP" && x != "Connection"
						&& x != "X-Do-Not-Track" && x != "DNT" && x != "Host" && x != "Referrer" && x != "User-Agent" && x != "Keep-Alive" && x != "UA-CPU" && x != "Via" && x != "X-Forwarded-For")
						.Select(x => string.Format("{0}--{1}", x, StringUtility.ToEmpty(this.Request.Headers[x])))
						.ToArray()),
				StringUtility.ToEmpty(this.Request.UserHostAddress),
				"");
			order.Add("score_http_headers", headers);
		}

		if ((cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO 
				|| cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE))
		{
			var headers = string.Format("{0};:{1};:{2};:{3};:{4};:{5};:{6};:{7};:{8};:{9};:{10};:{11};:{12};:{13};:{14};:{15};:{16};:",
				this.Request.Headers.AllKeys.Contains("Accept") ? StringUtility.ToEmpty(this.Request.Headers["Accept"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Charset") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Charset"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Encoding") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Encoding"]) : "",
				this.Request.Headers.AllKeys.Contains("Accept-Language") ? StringUtility.ToEmpty(this.Request.Headers["Accept-Language"]) : "",
				this.Request.Headers.AllKeys.Contains("Client-IP") ? StringUtility.ToEmpty(this.Request.Headers["Client-IP"]) : "",
				this.Request.Headers.AllKeys.Contains("Connection") ? StringUtility.ToEmpty(this.Request.Headers["Connection"]) : "",
				this.Request.Headers.AllKeys.Contains("X-Do-Not-Track") ? StringUtility.ToEmpty(this.Request.Headers["X-Do-Not-Track"])
					: this.Request.Headers.AllKeys.Contains("DNT") ? StringUtility.ToEmpty(this.Request.Headers["DNT"]) : "",
				this.Request.Headers.AllKeys.Contains("Host") ? StringUtility.ToEmpty(this.Request.Headers["Host"]) : "",
				this.Request.Headers.AllKeys.Contains("Referrer") ? StringUtility.ToEmpty(this.Request.Headers["Referrer"]) : "",
				this.Request.Headers.AllKeys.Contains("User-Agent") ? StringUtility.ToEmpty(this.Request.Headers["User-Agent"]) : "",
				this.Request.Headers.AllKeys.Contains("Keep-Alive") ? StringUtility.ToEmpty(this.Request.Headers["Keep-Alive"]) : "",
				this.Request.Headers.AllKeys.Contains("UA-CPU") ? StringUtility.ToEmpty(this.Request.Headers["UA-CPU"]) : "",
				this.Request.Headers.AllKeys.Contains("Via") ? StringUtility.ToEmpty(this.Request.Headers["Via"]) : "",
				this.Request.Headers.AllKeys.Contains("X-Forwarded-For") ? StringUtility.ToEmpty(this.Request.Headers["X-Forwarded-For"]) : "",
				string.Join("::",
					this.Request.Headers.AllKeys
					.Where(x => x != "Accept" && x != "Accept-Charset" && x != "Accept-Encoding" && x != "Accept-Language" && x != "Client-IP" && x != "Connection"
						&& x != "X-Do-Not-Track" && x != "DNT" && x != "Host" && x != "Referrer" && x != "User-Agent" && x != "Keep-Alive" && x != "UA-CPU" && x != "Via" && x != "X-Forwarded-For")
						.Select(x => string.Format("{0}--{1}", x, StringUtility.ToEmpty(this.Request.Headers[x])))
						.ToArray()),
				StringUtility.ToEmpty(this.Request.UserHostAddress),
				"");
			order.Add("gmo_http_headers", headers);
		}

		// GMO3Dセキュア用連携情報
		if ((cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Gmo)
			&& Constants.PAYMENT_SETTING_GMO_3DSECURE)
		{
			var returnUrl =
				new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE_GET_RESULT)
					.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
					.CreateUrl();
			order.Add(PaymentGmo.RETURN_URL, returnUrl);
		}

		//ベリトランス3Dセキュア用連携情報
		if ((cart.Payment != null && cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.VeriTrans)
			&& Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE)
		{
			order.Add(VeriTransConst.USER_AGENT, this.Request.UserAgent);
			order.Add(VeriTransConst.HTTP_ACCEPT, this.Request.ServerVariables.GetValues("HTTP_ACCEPT").ToString());
			order.Add(VeriTransConst.REDIRECTION_URI_VERITRANS, CreateVeriTransReturnUrl());
		}

		// ペイジェント3Dセキュア用連携情報
		if ((cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Paygent)
			&& Constants.PAYMENT_PAYGENT_CREDIT_3DSECURE)
		{
			order.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, cart.Owner.Addr1);
			order.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, cart.Owner.Addr2);
			order.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3, cart.Owner.Addr3);
			order.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, cart.Owner.Zip);
			order.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, cart.Owner.MailAddr);
			order.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1, cart.Owner.Tel1);
			order.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, cart.Shippings[0].Addr1);
			order.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, cart.Shippings[0].Addr2);
			order.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, cart.Shippings[0].Addr3);
			order.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, cart.Shippings[0].Addr4);
			order.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, cart.Shippings[0].Zip);
			order.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, cart.PriceTotal);
			// 新規カードの場合はトークンを利用する 保存済みカードの場合は顧客カードIDを利用する
			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				order.Add(Constants.CREDIT_CARD_TOKEN, cart.Payment.CreditToken.Token);
			}
			else
			{
				var userCreditCard = new UserCreditCardService().Get(cart.OrderUserId, int.Parse(cart.Payment.CreditCardBranchNo));
				order.Add(Constants.FIELD_USERCREDITCARD_COOPERATION_ID, userCreditCard.CooperationId);
				order.Add(Constants.FIELD_USERCREDITCARD_COOPERATION_ID2, userCreditCard.CooperationId2);
			}
		}

		if ((cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE))
		{
			switch (Constants.PAYMENT_CVS_KBN)
			{
				case Constants.PaymentCvs.SBPS:
					order.Add(Constants.CONST_PAYMENT_CVS_TYPE, cart.Payment.SBPSWebCvsType);
					break;

				case Constants.PaymentCvs.YamatoKwc:
					order.Add(Constants.CONST_PAYMENT_CVS_TYPE, cart.Payment.YamatoKwcCvsType);
					break;

				case Constants.PaymentCvs.Gmo:
					order.Add(Constants.CONST_PAYMENT_CVS_TYPE, cart.Payment.GmoCvsType);
					break;

				case Constants.PaymentCvs.Rakuten:
					order.Add(Constants.CONST_PAYMENT_CVS_TYPE, cart.Payment.RakutenCvsType);
					break;

				case Constants.PaymentCvs.Zeus:
					order.Add(Constants.CONST_PAYMENT_CVS_TYPE, cart.Payment.GetZeusCvsType());
					break;

				case Constants.PaymentCvs.Paygent:
					order.Add(Constants.CONST_PAYMENT_CVS_TYPE, cart.Payment.GetPaygentCvsType());
					break;
			}
		}

		return order;
	}

	/// <summary>
	/// ベリトランス認証結果戻し先URL作成
	/// </summary>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateVeriTransReturnUrl()
	{
		var sbReturnUrl = new StringBuilder();
		sbReturnUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT);
		sbReturnUrl.Append(Constants.PAGE_FRONT_PAYMENT_VERITRANS_CARD_3DSECURE_GET_RESULT);
		return sbReturnUrl.ToString();
	}

	/// <summary>
	/// 成功注文同梱の追加処理
	/// </summary>
	/// <param name="successOrder">成功注文</param>
	/// <param name="errorMessages">エラーメッセージリスト</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
	/// <param name="disablePaymentCancelOrderId">決済をキャンセル外部連携をスキップする受注ID</param>
	public void SuccessCombinedOrderAdditionProcess(
		Hashtable successOrder,
		List<string> errorMessages,
		string userId,
		CartCoupon orderCombineCoupon,
		string disablePaymentCancelOrderId = "")
	{
		// 注文同梱でない・エラーがある・NewebPay決済での注文のいずれかの場合はスキップ
		if ((this.IsOrderCombined == false)
			|| ((errorMessages != null) && errorMessages.Any())
			|| ((string)successOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
		{
			return;
		}

		var beforeCombineCartList = SessionManager.OrderCombineBeforeCartList;
		var combineCartList = SessionManager.OrderCombineCartList;

		// 注文同梱元の親注文をキャンセルする（更新履歴とともに）
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var parentOrderId = (string)successOrder[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS];
			var combinedNewOrderId = (string)successOrder[Constants.FIELD_ORDER_ORDER_ID];
			var errMessage = OrderCombineUtility.OrderCancelForOrderCombine(
				this.ShopId,
				parentOrderId,
				this.LoginUserId,
				true,
				"user",
				UpdateHistoryAction.Insert,
				accessor,
				orderCombineCoupon,
				disablePaymentCancelOrderId);

			// 注文同梱元注文キャンセル失敗フラグ（拡張ステータス46）をオフにする
			OrderCombineUtility.UpdateOrderCombineCancelFailedFlg(
				combinedNewOrderId,
				false,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 注文同梱元親注文の購入回数更新
			var orderCount = new UserService().Get(userId, accessor).OrderCountOrderRealtime;
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, combinedNewOrderId },
				{ Constants.FIELD_ORDER_ORDER_COUNT_ORDER, (orderCount - 1) },
				{ Constants.FIELD_ORDER_LAST_CHANGED, "user" },
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, orderCount},
				{ "cancelCount", 1}
			};

			// 注文同梱元親注文の購入回数更新
			using (var statement = new SqlStatement("Order", "UpdateUserOrderCount"))
			{
				statement.ExecStatement(accessor, ht);
			}
			// ユーザーリアルタイム更新
			OrderCommon.UpdateRealTimeOrderCount(ht, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_COMBINE, accessor);

			var parentOrder = new OrderService().Get(parentOrderId, accessor);

			// 親注文が定期購入かつ、GMO3Dセキュア購入の場合
			if ((combineCartList != null)
				&& (combineCartList.Items.Count != 0)
				&& combineCartList.Items[0].IsCombineParentOrderHasFixedPurchase
				&& (combineCartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
				&& Constants.PAYMENT_SETTING_GMO_3DSECURE)
			{
				ExecFixedPurchaseCombine(
					parentOrderId,
					combinedNewOrderId,
					userId,
					beforeCombineCartList.Items[0],
					combineCartList.Items[0],
					parentOrder,
					accessor);
			}

			// 注文実行対象のカート(注文同梱後のカート)と削除対象のカート(注文同梱前のカート)が異なるため、注文同梱前のカートを別途削除
			beforeCombineCartList.Items.ForEach(beforeCombineCart => { OrderCommon.DeleteCart(beforeCombineCart.CartId); });

			// 注文同梱元の親注文のキャンセルに失敗した場合、管理者へメール送信、ただしフロント画面が正常処理として処理を進める
			if (errMessage != "")
			{
				AppLogger.WriteError("フロント注文同梱の同梱元親注文のキャンセルに失敗しました。親注文ID：" + parentOrderId);

				OrderCombineUtility.SendOrderCombineParentOrderCancelErrorMail(userId, parentOrderId, combinedNewOrderId, errMessage);
				accessor.RollbackTransaction();
			}

			accessor.CommitTransaction();
		}
	}

	/// <summary>
	/// クレジットカード表示文字列（「XXXXXXXXXXXX1234」）取得
	/// </summary>
	/// <param name="payment">カート決済情報</param>
	/// <returns>表示用クレジットカードNO</returns>
	protected string GetCreditCardDispString(CartPayment payment)
	{
		string MaskedCardNo = "    " + StringUtility.ToEmpty(payment.CreditCardNo1 + payment.CreditCardNo2 + payment.CreditCardNo3 + payment.CreditCardNo4);

		return MaskedCardNo.Substring(MaskedCardNo.Length - 4, 4);
	}

	/// <summary>
	/// 最後にカート投入された商品を取得
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	/// <returns>最後に投入された商品の商品ID</returns>
	protected string GetLastAddedCartProductId(CartObjectList cartList)
	{
		StringBuilder cartIds = new StringBuilder();
		foreach (CartObject coCart in cartList)
		{
			cartIds.Append((cartIds.Length == 0) ? "" : ",");
			cartIds.Append("'").Append(coCart.CartId).Append("'");
		}

		DataView products = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Cart", "GetLastAddedCartProduct"))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ cart_ids @@", cartIds.ToString());
			products = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
		}

		return (string)products[0][Constants.FIELD_CART_PRODUCT_ID];
	}

	/// <summary>
	/// デフォルト注文方法登録/更新
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	private void InsertOrUpdateDefaultOrderSetting(CartObjectList cartList)
	{
		var userDefaultOrderSettingParameter = cartList.UserDefaultOrderSettingParm;
		if ((userDefaultOrderSettingParameter.IsUserDefaultPaymentRegister == false)
			&& (userDefaultOrderSettingParameter.IsUserDefaultShippingRegister == false))
		{
			return;
		}

		var userDefaultOrderSettingService = new UserDefaultOrderSettingService();
		var userDefaultOrderSetting = userDefaultOrderSettingService.Get(cartList.UserId);
		var paymentId = (userDefaultOrderSettingParameter.IsUserDefaultPaymentRegister)
			? cartList.Items[userDefaultOrderSettingParameter.UserDefaultPaymentCartNo].Payment.PaymentId
			: (userDefaultOrderSetting != null)
				? userDefaultOrderSetting.PaymentId
				: null;
		var shippingNo = userDefaultOrderSettingParameter.IsUserDefaultShippingRegister
			? SetShippingNo(cartList.Items[userDefaultOrderSettingParameter.UserDefaultShippingCartNo].Shippings[this.CheckedItemNumberForMultipleDeliveryAddresses].ShippingAddrKbn)
			: (userDefaultOrderSetting != null)
				? userDefaultOrderSetting.UserShippingNo
				: null;
		int? creditCardBranchNo = GetUserCreditCardBranchNo(userDefaultOrderSettingParameter.UserDefaultShippingCartNo, paymentId);
		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			paymentId = (creditCardBranchNo != null) ? paymentId : null;
		}

		// Get invoice No
		var isInvoiceNo = ((userDefaultOrderSetting != null)
			&& (userDefaultOrderSetting.UserInvoiceNo != null)
			&& userDefaultOrderSettingParameter.IsUserDefaultInvoiceRegister);
		int? invoiceNo;
		if (isInvoiceNo
			&& (userDefaultOrderSettingParameter.IsChangedUserDefaultInvoice == false))
		{
			invoiceNo = userDefaultOrderSetting.UserInvoiceNo;
		}
		else
		{
			invoiceNo = (userDefaultOrderSettingParameter.IsUserDefaultInvoiceRegister)
				? SetInvoiceNo(cartList.Items[userDefaultOrderSettingParameter.UserDefaultShippingCartNo].Shippings[0])
				: null;
		}

		var isDefaultPaymentSetting = userDefaultOrderSettingParameter.IsChangedUserDefaultPayment;
		var rakutenCvsType = cartList.Items[userDefaultOrderSettingParameter.UserDefaultPaymentCartNo].Payment.RakutenCvsType
			?? string.Empty;
		if (isDefaultPaymentSetting && (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE) && (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
		{
			var oldCvsType = userDefaultOrderSettingService.Get(this.LoginUserId);
			rakutenCvsType = oldCvsType.RakutenCvsType;
		}

		string zeusCvsType = null;
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& OrderCommon.IsPaymentCvsTypeZeus)
		{
			zeusCvsType = userDefaultOrderSettingParameter.IsUserDefaultPaymentRegister
				? cartList.Items[userDefaultOrderSettingParameter.UserDefaultPaymentCartNo].Payment.GetZeusCvsType()
				: (userDefaultOrderSetting != null)
					? userDefaultOrderSetting.ZeusCvsType
					: null;
		}

		string paygentCvsType = null;
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& OrderCommon.IsPaymentCvsTypePaygent)
		{
			paygentCvsType = userDefaultOrderSettingParameter.IsUserDefaultPaymentRegister
				? cartList.Items[userDefaultOrderSettingParameter.UserDefaultPaymentCartNo].Payment.GetPaygentCvsType()
				: (userDefaultOrderSetting != null)
					? userDefaultOrderSetting.PaygentCvsType
					: null;
		}

		// デフォルト注文方法新規登録/更新
		userDefaultOrderSettingService.InsertOrUpdate(
			userId: this.LoginUserId,
			paymentId: paymentId,
			creditCardBranchNo: creditCardBranchNo,
			shippingNo: shippingNo,
			invoiceNo: invoiceNo,
			rakutenCvsType: rakutenCvsType,
			zeusCvsType: zeusCvsType,
			paygentCvsType: paygentCvsType,
			lastChanged: Constants.FLG_LASTCHANGED_USER);

		// デフォルト注文方法初期化
		cartList.UserDefaultOrderSettingParm = null;
	}

	/// <summary>
	/// 配送先枝番セット
	/// </summary>
	/// <param name="shippingAddrKbn">配送先入力区分</param>
	/// <returns>配送先枝番</returns>
	private int? SetShippingNo(string shippingAddrKbn)
	{
		switch (shippingAddrKbn)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				var userShipping = new UserShippingService()
					.GetAllOrderByShippingNoDesc(this.LoginUserId)
					.Where(item => item.ShippingReceivingStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
					.ToArray();

				return (userShipping.Length != 0)
					? (int?)userShipping[0].ShippingNo
					: null;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
				var userShippingStore = new UserShippingService()
					.GetAllOrderByShippingNoDesc(this.LoginUserId)
					.Where(item => item.ShippingReceivingStoreFlg
						== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
					.ToArray();

				return (userShippingStore.Length != 0)
					? (int?)userShippingStore[0].ShippingNo
					: null;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				return 0;

			default:
				return int.Parse(shippingAddrKbn);
		}
	}

	/// <summary>
	/// Set Invoice No
	/// </summary>
	/// <param name="shipping">Shipping</param>
	/// <returns>Invoice No</returns>
	private int? SetInvoiceNo(CartShipping shipping)
	{
		var userInvoice = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.LoginUserId);
		switch (shipping.UniformInvoiceType)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				if (string.IsNullOrEmpty(shipping.UniformInvoiceTypeOption) == false)
				{
					return int.Parse(shipping.UniformInvoiceTypeOption);
				}

				if (userInvoice == null) return (int?)null;

				var invoice = userInvoice.OrderByDescending(item => item.TwInvoiceNo).ToArray();
				return (invoice != null) ? (int?)invoice[0].TwInvoiceNo : (int?)null;

			default:
				if (string.IsNullOrEmpty(shipping.CarryTypeOption) == false)
				{
					return int.Parse(shipping.CarryTypeOption);
				}

				if (userInvoice == null) return (int?)null;

				invoice = userInvoice.OrderByDescending(item => item.TwInvoiceNo).ToArray();
				return (invoice != null) ? (int?)invoice[0].TwInvoiceNo : (int?)null;
		}
	}

	/// <summary>
	/// クレジットカード枝番取得
	/// </summary>
	/// <param name="defaultPaymentCartNo">デフォルト支払方法用カート番号</param>
	/// <param name="paymentId">決済種別ID</param>
	/// <returns>クレジットカード枝番</returns>
	private int? GetUserCreditCardBranchNo(int defaultPaymentCartNo, string paymentId)
	{
		// 新規クレジットカードの場合、枝番の最大値を取得。既存クレジットカードの場合、カート情報から枝番を取得
		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			var newCreditCard = (((this.CartList.Items[defaultPaymentCartNo].Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				|| (this.CartList.Items[defaultPaymentCartNo].Payment.CreditCardBranchNo == string.Empty))
					&& string.IsNullOrEmpty(this.CartList.Items[defaultPaymentCartNo].Payment.UserCreditCardName) == false);
			var existingCreditCard = (this.CartList.Items[defaultPaymentCartNo].Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);
			if (newCreditCard)
			{
				return new UserCreditCardRepository().GetMaxBranchNo(this.LoginUserId);
			}
			else if (existingCreditCard)
			{
				return int.Parse(this.CartList.Items[defaultPaymentCartNo].Payment.CreditCardBranchNo);
			}
		}
		else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			return new UserCreditCardRepository().GetMaxBranchNo(this.LoginUserId);
		}
		else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
		{
			return new UserCreditCardService().GetMaxBranchNoByUserIdAndCooperationType(
				this.LoginUserId,
				Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAIDY);
		}
		else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
		{
			return new UserCreditCardService().GetMaxBranchNoByUserIdAndCooperationType(
				this.LoginUserId,
				Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_ECPAY);
		}
		else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
		{
			return new UserCreditCardService().GetMaxBranchNoByUserIdAndCooperationType(
				this.LoginUserId,
				Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_NEWEBPAY);
		}

		return null;
	}

	/// <summary>
	/// Get landing cart session key
	/// </summary>
	/// <param name="session">Http session state</param>
	/// <returns>A landing cart session key</returns>
	private static string GetLandingCartSessionKey(HttpSessionState session)
	{
		var ladingCartSessionKey = Constants.SESSION_KEY_CART_LIST_LANDING
			+ ((string.IsNullOrEmpty(HttpContext.Current.Request[Constants.REQUEST_KEY_RETURN_URL])
				? string.Format(
					"{0}{1}",
					(string)session[Constants.SESSION_KEY_LANDING_CART_INPUT_ABSOLUTE_PATH],
					(string)session[Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME])
				: HttpContext.Current.Request[Constants.REQUEST_KEY_RETURN_URL]));

		return ladingCartSessionKey;
	}

	/// <summary>
	/// Get cart list
	/// </summary>
	/// <param name="isLandingPage">Is landing cart page</param>
	/// <returns>The cart list</returns>
	protected static CartObjectList GetCartList(bool isLandingPage)
	{
		var session = HttpContext.Current.Session;
		var landingCartSessionKey = GetLandingCartSessionKey(session);

		var cartList = isLandingPage
			? (CartObjectList)session[landingCartSessionKey]
			: SessionSecurityManager.GetCartObjectList(HttpContext.Current, Constants.FLG_ORDER_ORDER_KBN_PC);

		return cartList;
	}

	/// <summary>
	/// Create Data Atone Aftee Token
	/// </summary>
	/// <param name="index">Cart Index</param>
	/// <param name="isAtone">Is Atone or Aftee</param>
	/// <returns>Data Info</returns>
	[WebMethod]
	public static string CreateDataAtoneAfteeToken(string index, bool isAtone)
	{
		var cartIndex = int.Parse(index);
		var cartList =
			SessionSecurityManager.GetCartObjectList(HttpContext.Current, Constants.FLG_ORDER_ORDER_KBN_PC);
		if (cartList == null) return string.Empty;

		var cart = cartList.Items[cartIndex];

		if ((cart.Payment != null)
			&& (string.IsNullOrEmpty(cart.Payment.CardTranId) == false))
		{
			return JsonConvert.SerializeObject(new { data = string.Empty });
		}

		if (string.IsNullOrEmpty(cart.OrderId))
		{
			cart.OrderId = OrderCommon.CreateOrderId(Constants.CONST_DEFAULT_SHOP_ID);
		}
		return OrderCommon.CreateDataForAuthorizingAtoneAftee(cart, isAtone);
	}

	/// <summary>
	/// Set Atone Aftee Transaction Id To Cart
	/// </summary>
	/// <param name="index">Index</param>
	/// <param name="token">Token</param>
	/// <param name="id">Transaction Id</param>
	[WebMethod]
	public static void SetTransactionIdToCart(string index, string token, string id)
	{
		var cartIndex = int.Parse(index);
		var cartList =
			SessionSecurityManager.GetCartObjectList(HttpContext.Current, Constants.FLG_ORDER_ORDER_KBN_PC);
		if (cartList == null) return;

		var cart = cartList.Items[cartIndex];

		cart.Payment.CardTranId = id;

		switch (cart.Payment.PaymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				cart.TokenAtone = token;
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				cart.TokenAftee = token;
				break;
		}
	}

	/// <summary>
	/// Set Transaction Id To Cart Landing
	/// </summary>
	/// <param name="index">Index</param>
	/// <param name="token">Token</param>
	/// <param name="id">iId</param>
	[WebMethod]
	public static void SetTransactionIdToCartLanding(string index, string token, string id)
	{
		var cartIndex = int.Parse(index);
		var session = HttpContext.Current.Session;
		var landingCartInputAbsolutePath = (string)session[Constants.SESSION_KEY_LANDING_CART_INPUT_ABSOLUTEPATH];
		var landingCartConfirmSkipFlgKey = string.Format(
			"{0}{1}",
			Constants.SESSION_KEY_CMS_LANDING_CART_CONFIRM_SKIP_FLG,
			landingCartInputAbsolutePath);
		var isCartListLp = Constants.CART_LIST_LP_OPTION
				&& landingCartInputAbsolutePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME);
		var landingCartSessionKey = (session[landingCartConfirmSkipFlgKey] != null) && isCartListLp
			? Constants.SESSION_KEY_CART_LIST_LANDING
			: OrderCommon.GetLandingCartSessionKey(
				session,
				Constants.SESSION_KEY_CART_LIST_LANDING,
				Constants.SESSION_KEY_LANDING_CART_INPUT_ABSOLUTEPATH,
				isCartListLp ? string.Empty : Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME);
		var cartList = (CartObjectList)session[landingCartSessionKey];
		session[landingCartConfirmSkipFlgKey] = null;
		if (cartList == null) return;

		var cart = cartList.Items[cartIndex];

		cart.Payment.CardTranId = id;
		switch (cart.Payment.PaymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				cart.TokenAtone = token;
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				cart.TokenAftee = token;
				break;
		}
	}

	/// <summary>
	/// クレジットカードによる失敗かどうか
	/// </summary>
	/// <param name="register">フロント側注文登録</param>
	/// <param name="cart">カート</param>
	/// <returns>クレジットカードによる失敗かどうか</returns>
	private bool IsFailByCreditCard(OrderRegisterFront register, CartObject cart)
	{
		if (string.IsNullOrEmpty(register.ApiErrorMessage)) return false;
		if (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return false;
		if (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW) return false;

		return true;
	}

	/// <summary>
	///クレジットカードで購入可能かどうか
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>クレジットカードで購入可能かどうか</returns>
	private bool IsLockedPurchaseByCreditCard(CartObject cart)
	{
		if (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return false;
		if (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW) return false;
		var result = CreditAuthAttackBlocker.Instance.IsLocked(this.LoginUserId, Request.UserHostAddress);
		return result;
	}

	/// <summary>
	/// 定期同梱処理実行
	/// </summary>
	/// <param name="parentOrderId">同梱元注文ID</param>
	/// <param name="combinedNewOrderId">同梱後注文ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="beforeCombineCart">同梱前カート</param>
	/// <param name="combineOrderCart">同梱後カート</param>
	/// <param name="parentOrder">親注文情報</param>
	/// <param name="accessor">SqlAccessor</param>
	/// <returns>結果</returns>
	private bool ExecFixedPurchaseCombine(
		string parentOrderId,
		string combinedNewOrderId,
		string userId,
		CartObject beforeCombineCart,
		CartObject combineOrderCart,
		OrderModel parentOrder,
		SqlAccessor accessor)
	{
		var parentFp = new FixedPurchaseService().Get(parentOrder.FixedPurchaseId, accessor);
		var cart = this.CartList.Items[0];

		// 定期台帳に注文同梱履歴追加
		var addHistoryResult = new FixedPurchaseService().RegistHistoryForOrderCombine(
			parentFp.FixedPurchaseId,
			combinedNewOrderId,
			userId,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert,
			accessor,
			true);
		var result = addHistoryResult;

		// 親注文及び子注文が定期購入の場合（頒布会注文が含まれる場合は更新無し）
		if (beforeCombineCart.HasFixedPurchase
			&& (combineOrderCart.HasSubscriptionBox == false)
			&& result)
		{
			// 定期台帳に商品を追加
			var fpUpdateResult =
				FixedPurchaseCombineUtility.AddFixedPurchaseItemsForOrderCombine(
					parentOrderId,
					userId,
					Constants.FLG_LASTCHANGED_USER,
					beforeCombineCart,
					UpdateHistoryAction.DoNotInsert,
					accessor);

			result = fpUpdateResult;
		}

		// 注文に定期台帳の情報を追加(定期の購入回数は注文同梱元親注文の回数を引き継ぐ)
		if (result)
		{
			var orderService = new OrderService();
			var count = orderService.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
				combinedNewOrderId,
				parentFp.FixedPurchaseId,
				parentOrder.FixedPurchaseOrderCount ?? 1,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 定期商品購入回数は注文同梱元親注文の回数を引き継ぐ
			orderService.UpdateFixedPerchaseItemOrderCountWhenOrderCombine(
				combinedNewOrderId,
				parentOrderId,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			result = count > 0;
		}

		// 更新履歴登録
		new UpdateHistoryService().InsertForFixedPurchase(
			parentOrder.FixedPurchaseId,
			Constants.FLG_LASTCHANGED_USER,
			accessor);
		new UpdateHistoryService().InsertForOrder(
			combinedNewOrderId,
			Constants.FLG_LASTCHANGED_USER,
			accessor);
		new UpdateHistoryService().InsertForUser(
			userId,
			Constants.FLG_LASTCHANGED_USER,
			accessor);

		return result;
	}
	#endregion

	#region プロパティ

	/// <summary>
	/// 外部決済連携時にボタンを隠すか
	/// </summary>
	/// <returns></returns>
	protected bool HideOrderButtonWithClick
	{
		get
		{
			return (this.HasCardPayment
				|| this.HasCvsDefPayment
				|| this.HasPayPalPayment
				|| this.HasNPAfterPayPayment);
		}
	}
	/// <summary>
	/// カード決済を含むか
	/// </summary>
	/// <returns></returns>
	protected bool HasCardPayment
	{
		get { return this.CartList.Items.Any(co => co.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT); }
	}
	/// <summary>
	/// コンビニ後払いを含むか
	/// </summary>
	/// <returns></returns>
	protected bool HasCvsDefPayment
	{
		get { return this.CartList.Items.Any(co => co.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF); }
	}
	/// <summary>
	/// ペイパル決済を含むか
	/// </summary>
	/// <returns></returns>
	protected bool HasPayPalPayment
	{
		get { return this.CartList.Items.Any(co => co.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL); }
	}
	/// <summary>
	/// 後付款(TriLink後払い)を含むか
	/// </summary>
	/// <returns></returns>
	protected bool HasTriLinkAfterPayPayment
	{
		get { return this.CartList.Items.Any(co => co.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY); }
	}
	/// <summary>過去に購入履歴があるか（類似配送先含む）</summary>
	protected bool HasOrderHistorySimilarShipping
	{
		get { return this.CartList.HasOrderHistorySimilarShipping; }
	}
	/// <summary>
	/// Has NP After Pay Payment
	/// </summary>
	/// <returns>True: If cart has NP after pay, otherwise: false</returns>
	protected bool HasNPAfterPayPayment
	{
		get
		{
			return this.CartList.Items.Any(cartObject =>
				cartObject.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
		}
	}
	/// <summary>
	/// 配送先にコンビニ受取が含まれているか
	/// </summary>
	/// <returns></returns>
	protected bool HasShippingConvenienceStore
	{
		get { return this.CartList.Items.Any(cart => cart.IsShippingConvenienceStore); }
	}
	/// <summary>トランザクション名</summary>
	protected string TransactionName { get; set; }
	/// <summary>エラーメッセージ</summary>
	protected List<string> DispErrorMessages { get; set; }
	/// <summary>アラートメッセージ</summary>
	protected List<string> DispAlertMessages { get; set; }
	/// <summary>複数配送先でチェックされた配送先NO</summary>
	protected int CheckedItemNumberForMultipleDeliveryAddresses { get ; set; }
	#endregion
}
