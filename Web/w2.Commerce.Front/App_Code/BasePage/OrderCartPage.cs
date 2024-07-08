/*
=========================================================================================================
  Module      : 注文カート系基底ページ(OrderCartPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Domain.ContentsLog;
using w2.Domain.Coupon.Helper;

///*********************************************************************************************
/// <summary>
/// 注文カート系基底ページ
/// </summary>
///*********************************************************************************************
public partial class OrderCartPage : OrderPageDisp
{
	/// <summary>
	/// 商品カート投入
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="iProductCount">商品数</param>
	/// <param name="lProductOptionSelectedValues">付帯情報</param>
	/// <param name="contentsLog">コンテンツログ</param>
	protected string AddProductToCart(
		string strShopId,
		string strProductId,
		string strVariationId,
		Constants.AddCartKbn addCartKbn,
		int iProductCount,
		List<string> lProductOptionSelectedValues,
		ContentsLogModel contentsLog = null)
	{
		return this.Process.AddProductToCart(
			strShopId,
			strProductId,
			strVariationId,
			addCartKbn,
			iProductCount,
			lProductOptionSelectedValues,
			contentsLog);
	}

	/// <summary>
	/// 商品カート投入
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="productCount">商品数</param>
	/// <param name="productOptionSelectedValues">付帯情報</param>
	/// <param name="contentsLog">コンテンツログ</param>
	protected string AddProductToLandingCart(
		string shopId,
		string productId,
		string variationId,
		Constants.AddCartKbn addCartKbn,
		int productCount,
		List<string> productOptionSelectedValues,
		ContentsLogModel contentsLog = null)
	{
		return this.Process.AddProductToLandingCart(
			shopId,
			productId,
			variationId,
			addCartKbn,
			productCount,
			productOptionSelectedValues,
			contentsLog);
	}

	/// <summary>
	/// 商品セットカート投入
	/// </summary>
	/// <param name="strShopId">商品ID</param>
	/// <param name="strProductSetId">商品セットID</param>
	/// <param name="lProductSetItems">商品セットアイテム</param>
	protected string AddProductSetToCart(
		string strShopId,
		string strProductSetId,
		List<Dictionary<string, string>> lProductSetItems)
	{
		return this.Process.AddProductSetToCart(strShopId, strProductSetId, lProductSetItems);
	}

	/// <summary>
	/// ポイント情報セット（再計算は呼び出し元で行う）
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	protected void SetUsePointData(WrappedRepeater wrCartList)
	{
		this.Process.SetUsePointData(wrCartList);
	}

	/// <summary>
	/// クーポン情報セット（再計算は呼び出し元で行う）
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	protected void SetUseCouponData(WrappedRepeater wrCartList)
	{
		this.Process.SetUseCouponData(wrCartList);
	}

	/// <summary>
	/// カート内容一括チェック
	/// </summary>
	/// <remarks>
	/// カート内容をチェックします。
	/// 初回読み込み時、および各イベント入力チェック＆データセット後に実行すべし
	/// </remarks>
	protected void CheckCartData()
	{
		this.Process.CheckCartData();
	}

	/// <summary>
	/// 過去に購入した商品がある場合カートから削除する
	/// </summary>
	/// <returns>カート商品削除数</returns>
	protected void ProductOrderLimitItemDelete()
	{
		this.Process.ProductOrderLimitItemDelete();
	}

	/// <summary>
	/// 紹介コードチェック
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	protected void CheckReferralCode(string userId)
	{
		this.Process.CheckReferralCode(userId);
	}

	/// <summary>
	/// シルバーエッグ用カート内商品ID取得
	/// </summary>
	/// <returns>カート内商品ID</returns>
	/// <remarks>
	/// ・カンマ区切りで並べる
	/// </remarks>
	protected string GetCartProductsForSilveregg()
	{
		return string.Join(",", this.CartList.Items.SelectMany(x => x.Items.Select(y => y.RecommendProductId)));
	}

	#region レコナイズ削除予定記述
	/// <summary>
	/// レコナイズ用カート内商品ID取得
	/// </summary>
	/// <returns>カート内商品ID</returns>
	/// <remarks>
	/// ・商品IDの頭に"P"をつける
	/// ・カンマ区切りで並べる
	/// ・最大10件
	/// </remarks>
	protected string GetCartProductsForReconize()
	{
		StringBuilder sbCartProducts = new StringBuilder();
		int iCount = 0;

		foreach (CartObject coCart in this.CartList)
		{
			foreach (CartProduct cpCartProduct in coCart)
			{
				sbCartProducts.Append((sbCartProducts.Length == 0) ? "" : ",");
				sbCartProducts.Append("P").Append(cpCartProduct.ProductId);
				iCount++;

				if (iCount >= Constants.CONST_RECONIZE_REQUEST_MAX_ITEM_CODE_COUNT)
				{
					return sbCartProducts.ToString();
				}
			}
		}
		return sbCartProducts.ToString();
	}
	#endregion

	/// <summary>
	/// カート装飾文字表示
	/// </summary>
	/// <param name="objCartObject">カートオブジェクト</param>
	/// <param name="strParams">装飾文字（[0]:ギフトのみ, [1]:デジタルコンテンツのみ）</param>
	/// <returns>カート装飾文字</returns>
	/// <remarks>たとえば「(ギフト)」とかの文字を出力する</remarks>
	protected string DispCartDecolationString(object objCartObject, params string[] strParams)
	{
		CartObject co = (CartObject)objCartObject;

		if (co.IsGift) return strParams[0];
		if (co.IsDigitalContentsOnly) return strParams[1];

		return "";
	}

	/// <summary>
	/// カートオブジェクト検索
	/// </summary>
	/// <param name="objTarget">ターゲットオブジェクト</param>
	/// <returns>カートオブジェクト</returns>
	protected CartObject FindCart(object objTarget)
	{
		return this.Process.FindCart(objTarget);
	}

	/// <summary>
	/// カートオブジェクトの定期会員フラグ設定
	/// </summary>
	/// <param name="cartObjList">カートオブジェクトリスト</param>
	protected void SetFixedPurchaseMemberFlgForCartObject(List<CartObject> cartObjList)
	{
		this.Process.SetFixedPurchaseMemberFlgForCartObject(cartObjList);
	}

	/// <summary>
	/// 頒布会コースIDからカートに投入する商品を取得
	/// </summary>
	/// <param name="courseId">コースID</param>
	protected void AddProductsToCartListForSubscriptionBox(string courseId)
	{
		this.Process.AddProductsToCartListForSubscriptionBox(courseId);
	}

	/// <summary>
	/// 定期会員割引が無効であるか判定
	/// </summary>
	/// <returns>true：定期会員割引無効、false：定期会員割引有効</returns>
	private bool IsApplyFixedPurchaseMemberDiscountInvaild()
	{
		return this.Process.IsApplyFixedPurchaseMemberDiscountInvaild();
	}

	/// <summary>
	/// クーポン入力方法取得
	/// </summary>
	/// <returns>クーポン入力方法</returns>
	protected ListItemCollection GetCouponInputMethod()
	{
		return CouponOptionUtility.GetCouponInputMethod();
	}

	/// <summary>
	/// カートに対し利用可能なクーポン情報取得
	/// </summary>
	/// <param name="cart">カートオブジェクト</param>
	/// <returns>クーポン情報</returns>
	protected UserCouponDetailInfo[] GetUsableCoupons(CartObject cart)
	{
		string languageCode = null;
		string languageLocaleId = null;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			languageCode = RegionManager.GetInstance().Region.LanguageCode;
			languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
		}

		CheckReferralCode(this.LoginUserId);

		var usableCoupons = CouponOptionUtility.GetUsableCoupons(
			this.LoginUserId,
			this.LoginUserMail,
			cart,
			languageCode,
			languageLocaleId,
			Constants.INTRODUCTION_COUPON_OPTION_ENABLED
				? SessionManager.ReferralCode
				: string.Empty);
		
		// ユーザー表示用のクーポン名が空である場合、管理用のクーポン名を表示するようにする
		foreach (var cp in usableCoupons)
		{
			if (string.IsNullOrEmpty(cp.CouponDispName) == false) continue;
			cp.CouponDispName = cp.CouponName;
		}

		return usableCoupons;
	}

	/// <summary>
	/// 利用可能回数表示文字列取得
	/// </summary>
	/// <param name="coupon">ユーザークーポン詳細情報</param>
	/// <returns>利用可能回数表示</returns>
	protected string GetCouponCount(UserCouponDetailInfo coupon)
	{
		return CouponOptionUtility.GetCouponCount(coupon);
	}

	/// <summary>
	/// クーポンの初期設定
	/// </summary>
	/// <param name="repeaterItem">親リピーターアイテム</param>
	protected void InitializeCouponComponents(RepeaterItem repeaterItem)
	{
		this.Process.InitializeCouponComponents(repeaterItem);
	}

	/// <summary>
	/// Check Payment Can Save Default Value
	/// </summary>
	/// <param name="paymentId">Payment Id</param>
	/// <returns>True: If Can Save Default Value Or False: If Can Not Save Default Value</returns>
	protected bool CheckPaymentCanSaveDefaultValue(string paymentId)
	{
		var paymentsCanNotSaveDefault = new List<string>
		{
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
			Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
			Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
			Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
			Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT
		};
		var result = paymentsCanNotSaveDefault.All(item => item != paymentId);
		return result;
	}

	/// <summary>
	/// 配送種別未設定エラーか確認
	/// </summary>
	protected void CheckGlobalShippingPriceCalcError()
	{
		foreach (var cart in this.CartList.Items)
		{
			cart.CheckGlobalShippingPriceCalcError();
		}
	}

	/// <summary>
	/// 配送種別未設定エラーが出ているか
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>エラーが出ているならtrue</returns>
	protected bool IsGlobalShippingPriceCalcError(CartObject cart)
	{
		var result = (cart.Shippings.Where(shipping => shipping.IsGlobalShippingPriceCalcError).ToList().Count > 0);
		return result;
	}

	/// <summary>
	/// 注文拡張項目 確認画面の表示内容を取得
	/// </summary>
	/// <param name="cartIndex">カート番号</param>
	/// <returns>表示内容</returns>
	public OrderExtendItemInput[] GetDisplayOrderExtendItemInputs(int cartIndex)
	{
		var result = new OrderExtendInput(OrderExtendInput.UseType.Register, this.CartList.Items[cartIndex].OrderExtend)
			.OrderExtendItems
			.Select(
				i =>
				{
					i.OrderExtendFixedPUrchaseNextTakeOver = (this.CartList.Items[cartIndex].HasFixedPurchase) && i.OrderExtendFixedPUrchaseNextTakeOver;
					return i;
				})
			.ToArray();
		
		return result;
	}

	/// <summary>
	/// カート情報セッションの同梱商品を削除
	/// </summary>
	public void DeleteBundledProduct()
	{
		var cartList = SessionManager.CartList.Items.Select(item => item.Items);
		foreach (var cartItem in cartList.Select((value, index) => new { value, index }))
		{
			SessionManager.CartList.Items.ElementAt(cartItem.index).Items.RemoveAll(x => x.IsBundle);
		}
	}

	/// <summary>
	/// Get use point
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Use point</returns>
	protected decimal GetUsePoint(CartObject cart)
	{
		if (cart.CanUsePointForPurchase == false)
		{
			cart.SetUsePoint(0, 0);
		}
		return cart.UsePoint;
	}

	/// <summary>
	/// Get price can purchase use point
	/// </summary>
	/// <param name="purchasePriceTotal">Purchase price total</param>
	/// <returns>Price can purchase use point</returns>
	protected string GetPriceCanPurchaseUsePoint(decimal purchasePriceTotal)
	{
		var result = CurrencyManager.ToPrice(Constants.POINT_MINIMUM_PURCHASEPRICE - purchasePriceTotal);
		return result;
	}

	/// <summary>
	/// Redirect order shipping
	/// </summary>
	protected void RedirectOrderShipping()
	{
		if (Constants.GIFTORDER_OPTION_ENABLED == false)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
		}
	}

	/// <summary>
	/// Redirect order shipping select
	/// </summary>
	protected void RedirectOrderShippingSelect()
	{
		if (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT);
		}
	}

	/// <summary>
	/// メール便かつ決済種別が代引きではないかチェック
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	protected void CheckPaymentAvailableShipping()
	{
		var errorMesseage = "";
		foreach (var cart in this.CartList.Items)
		{
			if ((cart.Shippings[0].ShippingMethod == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL)
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
			{
				errorMesseage = errorMesseage + MessageManager.GetMessages(WebMessages.ERRMSG_FRONT_CART_LIMITED_PAYMENT).Replace("@@ 1 @@", cart.Payment.PaymentName);
			}
		}
		if　(errorMesseage.Any())
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMesseage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>入力チェック用カートID</summary>
	protected int CurrentCartIndex
	{
		get { return this.Process.CurrentCartIndex; }
		set { this.Process.CurrentCartIndex = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage
	{
		get { return this.Process.ErrorMessage; }
		set { this.Process.ErrorMessage = value; }
	}
	/// <summary>カートエラーメッセージ</summary>
	protected CartErrorMessages ErrorMessages
	{
		get { return this.Process.ErrorMessages; }
	}
	/// <summary> 注文同梱有無 </summary>
	protected bool IsOrderCombined
	{
		get { return this.Process.IsOrderCombined; }
	}
	/// <summary> 注文同梱後 決済再選択有無 </summary>
	protected bool IsPaymentReselect
	{
		get { return this.Process.IsPaymentReselect; }
	}
	/// <summary>領収書希望選択肢リスト</summary>
	protected ListItemCollection ReceiptFlgListItems
	{
		get { return this.Process.ReceiptFlgListItems; }
	}
	/// <summary>利用可能ポイントの定期継続利用が利用可能か</summary>
	protected bool CanUseAllPointFlg
	{
		get
		{
			return (Constants.W2MP_POINT_OPTION_ENABLED
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE);
		}
	}
}
