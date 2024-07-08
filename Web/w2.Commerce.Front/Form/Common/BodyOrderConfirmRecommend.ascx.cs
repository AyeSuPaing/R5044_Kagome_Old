/*
=========================================================================================================
  Module      : Body Order Confirm Recommend (BodyOrderConfirmRecommend.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Amazon;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.NextEngine;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Product;
using w2.App.Common.Recommend;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.RealShop;
using w2.Domain.Recommend;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// Body order confirm recommend
/// </summary>
public partial class Form_Common_BodyOrderConfirmRecommend : OrderCartUserControl
{
	/// <summary>Wrapped repeater cart list</summary>
	protected WrappedRepeater WrCartList { get { return GetWrappedControl<WrappedRepeater>("rCartList"); } }
	/// <summary>Class display row name</summary>
	private const string CLASS_DISPLAY_ROW_NAME = "bgc";
	/// <summary>Class display price name</summary>
	private const string CLASS_DISPLAY_PRICE_NAME = "minus";
	/// <summary>Prefix display price</summary>
	private const string PREFIX_DISPLAY_PRICE = "-";
	/// <summary>Display reflect memo fixed purchase</summary>
	private const string DISPLAY_REFLECT_MEMO_FIXED_PURCHASE = "※2回目以降の注文メモにも追加する";
	/// <summary>Input value default</summary>
	private const string INPUT_VALUE_DEFAULT = "指定なし";
	/// <summary>String cart index</summary>
	private const string STRING_CART_INDEX = "カート番号";
	/// <summary>Quantity of product set default</summary>
	private const int QUANTITY_OF_PRODUCT_SET_DEFAULT = 1;
	/// <summary>Status credit card do register</summary>
	private const string STATUS_CREDIT_CARD_DO_REGISTER = "する";
	/// <summary>Status credit card already register</summary>
	private const string STATUS_CREDIT_CARD_ALREADY_REGISTER = "済";
	/// <summary>Status credit card do not register</summary>
	private const string STATUS_CREDIT_CARD_DO_NOT_REGISTER = "しない";
	/// <summary>Wrapped link button complete</summary>
	protected WrappedLinkButton WlbComplete { get { return GetWrappedControl<WrappedLinkButton>("lbComplete"); } }

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (Constants.RECOMMEND_OPTION_ENABLED == false) return;

		this.DispNum = 0;
		if (!IsPostBack)
		{
			// カート情報が渡されていないorレコメンド適用済みの場合なにもしない
			if ((this.OrderList == null)
				|| (this.OrderList
					.Any(order => order.Cast<DataRowView>()
						.Any(item => (string.IsNullOrEmpty((string)item[Constants.FIELD_ORDERITEM_RECOMMEND_ID]) == false)))))
			{
				this.Visible = false;
				return;
			}

			// カート情報復元
			CreateCartList();

			// 注文同梱済みorギフト購入or注文ステータスが「注文済み」以外の注文に対してレコメンド表示しない
			if (this.OrderOld.Any(
				order => (string.IsNullOrEmpty(order.CombinedOrgOrderIds) == false)
					|| order.IsGiftOrder
					|| (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)))
			{
				this.Visible = false;
				return;
			}

			if (this.OrderOld.Any(order => (OrderCommon.CheckPaymentYamatoKaSms(order.OrderPaymentKbn))))
			{
				this.Visible = false;
				return;
			}

			SetRecommend();

			var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.Orders = (List<Hashtable>)param["order"];
			if (this.CartList != null)
			{
				//決済金額決定
				foreach (var item in this.CartList.Items)
				{
					item.SettlementCurrency = CurrencyManager.GetSettlementCurrency(item.Payment.PaymentId);
					item.SettlementRate = CurrencyManager.GetSettlementRate(item.SettlementCurrency);
					item.SettlementAmount = CurrencyManager.GetSettlementAmount(
						item.PriceTotal,
						item.SettlementRate,
						item.SettlementCurrency);

					if (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
					{
						var index = Array.IndexOf(this.CartList.Items.ToArray(), item);
						var cvsType = GetCvsType(item, index);

						switch (Constants.PAYMENT_CVS_KBN)
						{
							case Constants.PaymentCvs.SBPS:
								item.Payment.SBPSWebCvsType = (PaymentSBPSTypes.WebCvsTypes)Enum.Parse(typeof(PaymentSBPSTypes.WebCvsTypes), cvsType);
								break;

							case Constants.PaymentCvs.YamatoKwc:
								item.Payment.YamatoKwcCvsType = (YamatoKwcFunctionDivCvs)Enum.Parse(typeof(YamatoKwcFunctionDivCvs), cvsType);
								break;

							case Constants.PaymentCvs.Gmo:
								item.Payment.GmoCvsType = cvsType;
								break;

							case Constants.PaymentCvs.Zeus:
								item.Payment.PaymentObject = new PaymentZeusCvs(cvsType);
								break;
						}
					}
				}

				// デフォルト配送先で再計算
				this.CartList.CalculateAllCart();

				this.WrCartList.DataSource
					= this.CartListRecommend
					= this.CartList;

				// 別タブでカート内を変更された時用に現在のカート内商品情報を格納する
				this.BindingCartList = this.CartList.Items[0];

				// データバインド準備
				this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

				// 定期購入カート + 通常注文の注文同梱用に定期購入設定作成
				CreateFixedPurchaseSettings();

				// データバインド
				this.DataBind();
			}
		}
	}

	/// <summary>
	/// 注文を確定するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbComplete_Click(object sender, System.EventArgs e)
	{
		// 注文ステータスが「注文済み」以外の場合は、レコメンド注文を行わずエラーページに遷移する
		var latestOrders = this.OrderOld.Select(order => DomainFacade.Instance.OrderService.Get(order.OrderId)).ToList();
		if (latestOrders.Any(order => (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)))
		{
			SessionManager.MessageForErrorPage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// 配送方法の見直し
		this.CartListRecommend.Items.ForEach(
			cart => cart.Shippings.ForEach(
				shipping => shipping.UpdateShippingMethod(
					DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.ShippingType))));

		this.CartListRecommend.CalculateAllCart();
		//決済金額決定
		foreach (var item in this.CartListRecommend.Items)
		{
			item.SettlementCurrency = CurrencyManager.GetSettlementCurrency(item.Payment.PaymentId);
			item.SettlementRate = CurrencyManager.GetSettlementRate(item.SettlementCurrency);
			item.SettlementAmount = CurrencyManager.GetSettlementAmount(
				item.PriceTotal,
				item.SettlementRate,
				item.SettlementCurrency);
		}

		// 商品同梱して注文実行
		var registeredOrder = new List<Hashtable>();
		var user = DomainFacade.Instance.UserService.Get(this.OrderUserId);
		var isFirstCart = true;
		var excludeOrderIds = this.OrderOld.Select(order => order.OrderId).ToArray();
		using (var productBundler = new ProductBundler(
			this.CartListRecommend.Items,
			this.OrderUserId,
			SessionManager.AdvCodeFirst,
			SessionManager.AdvCodeNow,
			excludeOrderIds,
			this.LoginUserHitTargetListIds,
			true))
		{
			var bundledCartList = productBundler.CartList.Where(cart => cart.Items.Any()).ToArray();

			// 別カートですでにAtodeneによる再与信を行っているかどうか
			var alreadyOtherCartReauthByAtodene = false;
			// 決済キャンセルを行わない受注ID
			var disablePaymentCancelOrderId = string.Empty;

			foreach (var cart in bundledCartList)
			{
				cart.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];
				var oldOrder = this.OrderOld.FirstOrDefault(order => (order.OrderId == cart.OrderId));
				var newOrder = (oldOrder != null)
					? cart.CreateNewOrder(oldOrder)
					: cart.CreateNewOrder();

				newOrder.AdvcodeFirst = user.AdvcodeFirst;

				foreach (var oderItem in newOrder.Items)
				{
					var cartProduct = cart.Items.FirstOrDefault(item => (oderItem.VariationId == item.VariationId));
					if (cartProduct != null) oderItem.ProductName = cartProduct.ProductJointName;
				}

				//セットプロモーション割引を再計算
				newOrder.RecalculateSetPromotionDiscountAmount();

				if (oldOrder == null)
				{
					cart.OrderId = newOrder.OrderId;
					cart.OrderUserId = user.UserId;

					// For case has Line pay in cart
					if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					{
						cart.IsPreApprovedLinePayPayment = true;
					}

					// 古い注文決済IDを削除しておく
					newOrder.PaymentOrderId = string.Empty;

					// 元の受注からキャンセルされ、なおかつコンビニ後払いのものがあるかどうか確認
					var cvsDefOrder = this.OrderOld.Where(order => (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
					var existOrderIdCart = bundledCartList.Where(item => (string.IsNullOrEmpty(item.OrderId) == false));
					var isAtodeneCancelCart = cvsDefOrder.Any(order => (existOrderIdCart.Any(cartObject => (cartObject.OrderId == order.OrderId)) == false));

					var isAtodeneReauth = OrderCommon.IsAtodeneReauthByNewOrder(
						this.OrderOld.Where(item => (item.OrderId != cart.OrderId)).ToList(),
						newOrder.OrderPaymentKbn);
					if (isAtodeneReauth
						&& isAtodeneCancelCart
						&& (alreadyOtherCartReauthByAtodene == false))
					{
						// 元カートと作成されるカートがAtodeneの場合かつ、元のカートがなくなる場合、再与信を行う
						// キャンセルされるカートの中で最も決済取引金額が高いカートを取得
						var cancelAtodeneOrders = this.OrderOld
							.Where(order => (order.OrderId != cart.OrderId)
								&& (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));

						var maxAmountCancelOrder = cancelAtodeneOrders.OrderByDescending(item => item.LastBilledAmount).FirstOrDefault();
						// アップセル元の商品はひとつのカートのみなので、一回だけでも再与信したら後は無視
						alreadyOtherCartReauthByAtodene = true;

						ExecuteOrderWithReauth(maxAmountCancelOrder, newOrder, cart, user, isFirstCart, out disablePaymentCancelOrderId);
					}
					else
					{
						ExecuteOrder(newOrder, cart, user, isFirstCart);
					}
				}
				else if (newOrder.Items.Any(item => item.IsRecommendItem)
					|| (newOrder.Items.Length != oldOrder.Items.Length)
					|| newOrder.Items.Any(item =>
							(item.ItemQuantitySingle
								!= ((oldOrder.Items.FirstOrDefault(oldItem => ((oldItem.ProductId == item.ProductId)
									&& (oldItem.VariationId == item.VariationId))) != null)
						? oldOrder.Items.First(oldItem => ((oldItem.ProductId == item.ProductId) && (oldItem.VariationId == item.VariationId))).ItemQuantitySingle
						: 0))))
				{
					UpdateOrder(newOrder, user, cart);
				}

				isFirstCart = false;
				registeredOrder.Add(newOrder.DataSource);
			}

			// 商品がなくなったカートがあればキャンセル処理入れておく
			this.OrderOld.Where(item => (bundledCartList.Any(cart => (cart.OrderId == item.OrderId)) == false)).ToList()
				.ForEach(order => CancelOrder(order, user, disablePaymentCancelOrderId));
		}

		DomainFacade.Instance.RecommendService.UpdateBuyOrderedFlg(
			this.Recommend.ShopId,
			this.Recommend.RecommendId,
			this.OrderUserId,
			this.RecommendHistoryNo,
			Constants.FLG_LASTCHANGED_USER);

		Session[Constants.SESSION_KEY_PARAM] = new Hashtable
		{
			{ Constants.FIELD_USER_USER_ID, this.OrderUserId },
			{ "order", registeredOrder },
			{ "error", new List<string>() },
			{ "alert", new List<string>() },
			{ "order_docomo", new Dictionary<string, CartObject>() }
		};

		SessionManager.NextPageForCheck = Constants.PAGE_FRONT_ORDER_COMPLETE;
		var advCodeNow = (string)this.Session[Constants.SESSION_KEY_ADVCODE_NOW];
		Session[Constants.SESSION_KEY_ADVCODE_NOW] = string.Empty;
		Response.Redirect(SessionSecurityManager.GetSecurePageProtocolAndHost()
			+ Constants.PATH_ROOT
			+ Constants.PAGE_FRONT_ORDER_COMPLETE
			+ ((string.IsNullOrEmpty(advCodeNow)
				|| (Constants.ADD_ADVC_TO_REQUEST_PARAMETER_OPTION_ENABLED == false))
					? string.Empty
					: ("?" + Constants.REQUEST_KEY_ADVCODE + "=" + advCodeNow)));
	}

	/// <summary>
	/// Check box default invoice checked changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbDefaultInvoice_CheckedChanged(object sender, EventArgs e)
	{
		WrappedCheckBox wcbDefaultInvoice = null;
		var senderControl = (CheckBox)sender;
		if (senderControl.Checked)
		{
			foreach (RepeaterItem cart in WrCartList.Items)
			{
				var wrCartShippings = GetWrappedControl<WrappedRepeater>(cart, "rCartShippings");
				foreach (RepeaterItem shipping in wrCartShippings.Items)
				{
					wcbDefaultInvoice = GetWrappedControl<WrappedCheckBox>(shipping, "cbDefaultInvoice");
				}
				if (wcbDefaultInvoice.ClientID != senderControl.ClientID)
				{
					wcbDefaultInvoice.Checked = false;
				}
			}
		}
	}

	/// <summary>
	/// レコメンド設定セット
	/// </summary>
	private void SetRecommend()
	{
		// カート情報セット
		if (this.CartList == null) return;
		var targetCartList = this.CartList.Items.ToArray();

		// レコメンド表示をセット
		var buttonId = string.Empty;

		if (SetRecommend(
			targetCartList,
			this.CartList,
			this.OrderUserId,
			buttonId,
			Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE) == false)
		{
			return;
		}

		this.CartList.AddRecommendItem(this.Recommend);
		var cartRecommend = this.CartList.Items
			.SelectMany(cart => cart.Items)
			.Where(item => item.IsRecommendItem)
			.ToArray();

		this.RecommendItemNames = cartRecommend
			.Select(item => item.ProductJointName)
			.ToArray();

		if (cartRecommend.Length == 0) return;

		if (this.Recommend.CanDisplayOrderCompletePage)
		{
			var shippingTypesOld = targetCartList
				.Select(item => item.ShippingType)
				.ToArray();

			var hasPaymentAtoneOrAftee = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					|| (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));

			var isPaymentAtoneOrAfteeAndCrosssellAndOtherShipping = (hasPaymentAtoneOrAftee
				&& this.Recommend.IsCrosssell
				&& cartRecommend.Any(item => (shippingTypesOld.Contains(item.ShippingType) == false)));

			var hasPaymentEcPay = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY));

			var hasPaymentNewebPay = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));

			if (hasPaymentNewebPay
				|| hasPaymentEcPay
				|| isPaymentAtoneOrAfteeAndCrosssellAndOtherShipping)
			{
				this.Recommend = null;
			}
		}
	}

	/// <summary>
	/// レコメンド設定セット
	/// </summary>
	/// <param name="targetCartList">レコメンド商品投入対象カート</param>
	/// <param name="currentCartList">カートオブジェクトリスト</param>
	/// <param name="orderUserId">注文者ユーザーID</param>
	/// <param name="buttonId">商品投入ボタンID</param>
	/// <param name="displayPage">レコメンド表示ページ</param>
	/// <returns>成功したか</returns>
	protected bool SetRecommend(
		CartObject[] targetCartList,
		CartObjectList currentCartList,
		string orderUserId,
		string buttonId,
		string displayPage = Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM)
	{
		// 条件に一致するレコメンド設定を抽出
		var recommend = new RecommendExtractor(orderUserId, this.LoginUserMemberRankId, currentCartList.Items.ToArray())
			.Exec(targetCartList, displayPage);
		// レコメンド設定が存在しなければ処理を抜ける
		if (recommend == null) return false;
		this.Recommend = recommend;

		// レコメンド表示をセット
		this.RecommendDisplay = new RecommendDisplayConverter(recommend, buttonId, this.IsSmartPhone)
			.ExecAndGetRecommendHtml();

		// レコメンド表示履歴枝番を取得
		this.RecommendHistoryNo = DomainFacade
			.Instance
			.RecommendService
			.GetMaxRecommendHistoryNo(
				recommend.ShopId,
				orderUserId,
				recommend.RecommendId);

		return true;
	}

	/// <summary>
	/// カート情報復元
	/// </summary>
	private void CreateCartList()
	{
		var service = DomainFacade.Instance.OrderService;
		this.OrderOld = this.OrderList.Select(order => service.Get((string)order[0][Constants.FIELD_ORDER_ORDER_ID])).ToArray();
		this.OrderUserId = this.LoginUserId ?? this.OrderOld.First().UserId;

		var orderOld = this.OrderOld[0];
		orderOld.PaymentName = DomainFacade.Instance.PaymentService.GetPaymentName(
			orderOld.ShopId,
			orderOld.OrderPaymentKbn);

		this.CartList = CartObjectList.CreateCartObjectListByOrder(
			this.OrderUserId,
			this.IsPc
				? Constants.FLG_ORDER_ORDER_KBN_PC
				: Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
			this.OrderOld);
	}

	/// <summary>
	/// Get Information Invoice
	/// </summary>
	/// <param name="shipping">Shipping</param>
	/// <param name="isCarryType">Is Carry Type</param>
	/// <returns>Information Invoice</returns>
	protected string GetInformationInvoice(CartShipping shipping, bool isCarryType = false)
	{
		var result = string.Empty;
		if (isCarryType)
		{
			result = string.IsNullOrEmpty(shipping.CarryType)
				? ValueText.GetValueText(
					Constants.TABLE_TWORDERINVOICE,
					Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE,
					shipping.CarryType)
				: string.Format(
					"{0}<br/>{1}",
					ValueText.GetValueText(
						Constants.TABLE_TWORDERINVOICE,
						Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE,
						shipping.CarryType).Replace("コード",
						string.Empty),
					shipping.CarryTypeOptionValue);
		}
		else
		{
			switch (shipping.UniformInvoiceType)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					result = ValueText.GetValueText(
						Constants.TABLE_TWORDERINVOICE,
						Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE,
						shipping.UniformInvoiceType);
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					result = string.Format(
						"{0}<br/>{1}<br/>{2}",
						ValueText.GetValueText(
							Constants.TABLE_TWORDERINVOICE,
							Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE,
							shipping.UniformInvoiceType),
						shipping.UniformInvoiceOption1,
						shipping.UniformInvoiceOption2);
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					result = string.Format(
						"{0}<br/>{1}",
						ValueText.GetValueText(
							Constants.TABLE_TWORDERINVOICE,
							Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE,
							shipping.UniformInvoiceType),
						shipping.UniformInvoiceOption1);
					break;
			}
		}

		return result;
	}

	/// <summary>
	/// 注文拡張項目 確認画面の表示内容を取得
	/// </summary>
	/// <param name="cartIndex">カート番号</param>
	/// <returns>表示内容</returns>
	public OrderExtendItemInput[] GetDisplayOrderExtendItemInputs(int cartIndex)
	{
		var result = new OrderExtendInput(OrderExtendInput.UseType.Register, this.CartListRecommend.Items[cartIndex].OrderExtend)
			.OrderExtendItems
			.Select(
				item =>
				{
					item.OrderExtendFixedPUrchaseNextTakeOver = ((this.CartListRecommend.Items[cartIndex].HasFixedPurchase)
						&& item.OrderExtendFixedPUrchaseNextTakeOver);
					return item;
				})
			.ToArray();

		return result;
	}

	/// <summary>
	/// クレジットカード表示文字列（「XXXXXXXXXXXX1234」）取得
	/// </summary>
	/// <param name="payment">カート決済情報</param>
	/// <returns>表示用クレジットカードNO</returns>
	protected string GetCreditCardDispString(CartPayment payment)
	{
		var maskedCardNo = "    " + StringUtility.ToEmpty(payment.CreditCardNo1 + payment.CreditCardNo2 + payment.CreditCardNo3 + payment.CreditCardNo4);
		return maskedCardNo.Substring((maskedCardNo.Length - 4), 4);
	}

	/// <summary>
	/// 商品セット数取得
	/// </summary>
	/// <param name="cartProduct">カート商品</param>
	/// <returns>商品セット数</returns>
	public static int GetProductSetCount(CartProduct cartProduct)
	{
		if (cartProduct.IsSetItem == false) return 0;

		return cartProduct.ProductSet.ProductSetCount;
	}

	/// <summary>
	/// 商品セット小計取得
	/// </summary>
	/// <param name="cartProduct">カート商品</param>
	/// <returns>商品セット小計</returns>
	public static decimal GetProductSetPriceSubtotal(CartProduct cartProduct)
	{
		if (cartProduct.IsSetItem == false) return 0;

		return cartProduct.ProductSet.ProductSetPriceSubtotal;
	}

	/// <summary>
	/// 注文登録
	/// </summary>
	/// <param name="newOrder">注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="isFirstCart">先頭カート？</param>
	private void ExecuteOrder(OrderModel newOrder, CartObject cart, UserModel user, bool isFirstCart)
	{
		var register = new OrderRegisterFront(user.IsMember);
		var result = register.Exec(
			newOrder.DataSource,
			cart,
			false,
			isFirstCart);

		if ((result == OrderRegisterFront.ResultTypes.Fail)
			|| (result == OrderRegisterFront.ResultTypes.Skip))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 再与信を行う注文登録
	/// </summary>
	/// <param name="oldOrder">古い注文情報</param>
	/// <param name="newOrder">新しい注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="isFirstCart">先頭カート？</param>
	/// <param name="disableCancelOrderId">決済キャンｓルを行わない受注ID</param>
	private void ExecuteOrderWithReauth(
		OrderModel oldOrder,
		OrderModel newOrder,
		CartObject cart,
		UserModel user,
		bool isFirstCart,
		out string disableCancelOrderId)
	{
		var register = new OrderRegisterFront(user.IsMember);

		var result = register.ExecReauthAndNewOrder(
			oldOrder,
			newOrder.DataSource,
			cart,
			false,
			isFirstCart,
			false,
			true,
			out disableCancelOrderId,
			newOrder);

		if ((result == OrderRegisterFront.ResultTypes.Fail)
			|| (result == OrderRegisterFront.ResultTypes.Skip))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();

			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="newOrder">注文情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="cart">カート</param>
	private void UpdateOrder(OrderModel newOrder, UserModel user, CartObject cart)
	{
		var oldOrder = this.OrderOld.FirstOrDefault(order => (order.OrderId == newOrder.OrderId));
		var updater = new OrderUpdaterFront(
			user,
			newOrder,
			oldOrder,
			user.UserMemo,
			user.UserManagementLevelId,
			Constants.FLG_LASTCHANGED_USER);

		var transactionName = updater.CreateOrderNew(true, true, UpdateHistoryAction.DoNotInsert, cart);

		if (string.IsNullOrEmpty(transactionName) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = transactionName == OrderUpdaterFront.ResultType.OutOfStock.ToString()
				? WebMessages.GetMessages(WebMessages.ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR)
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();

			Response.Redirect(url);
		}

		// 外部決済連携実行
		var errorMessage = string.Empty;
		var isExecuteExternalPayment = updater.ExecuteExternalPayment(
			ReauthCreatorFacade.ExecuteTypes.System,
			ReauthCreatorFacade.ExecuteTypes.System,
			UpdateHistoryAction.DoNotInsert,
			out errorMessage);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			new OrderService().UpdatePaymentMemo(updater.Order.OrderId, updater.Order.PaymentMemo, updater.LastChanged, UpdateHistoryAction.Insert);
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();

			Response.Redirect(url);
		}

		// 注文情報更新
		var isSuccess = updater.ExecuteUpdateOrderAndRegisterUpdateHistory(
			false,
			false,
			isExecuteExternalPayment,
			true,
			true,
			OrderHistory.ActionType.FrontRecommend,
			UpdateHistoryAction.Insert,
			cart,
			out errorMessage);

		if (isSuccess != OrderUpdaterFront.ResultType.Success)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = (isSuccess == OrderUpdaterFront.ResultType.OutOfStock)
				? WebMessages.GetMessages(WebMessages.ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR)
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();

			Response.Redirect(url);
		}

		// 定期台帳登録・更新
		var newOrderCart = this.CartListRecommend.Items.First(item => (item.OrderId == newOrder.OrderId));
		if (newOrder.Items.Any(item => item.IsFixedPurchaseItem))
		{
			if (oldOrder.IsFixedPurchaseOrder)
			{
				// 更新
				updater.UpdateFixedPurchaseOrderForRecommendAtOrderComplete(newOrderCart, UpdateHistoryAction.DoNotInsert);
			}
			else
			{
				// 新規
				updater.RegisterFixedPurchaseOrder(newOrderCart, UpdateHistoryAction.DoNotInsert);
			}
		}

		// メール送信
		updater.SendOrderUpdateMail(newOrderCart, this.IsLoggedIn, this.RecommendItemNames);

		if (Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging)
		{
			// 「リピートライン」送信
			updater.SendOrderUpdateLineMessage(newOrder.DataSource, newOrderCart, this.IsLoggedIn);
		}
	}

	/// <summary>
	/// 注文キャンセル処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="disablePaymentCancelOrderId">外部決済キャンセルを行わない受注ID</param>
	private void CancelOrder(OrderModel order, UserModel user, string disablePaymentCancelOrderId)
	{
		order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
		var updater = new OrderUpdaterFront(
			user,
			order,
			Constants.FLG_LASTCHANGED_USER);
		var errorMessage = string.Empty;
		var isCancelSuccess = updater.ExecuteCanceOrder(
			true,
			OrderHistory.ActionType.FrontRecommend,
			UpdateHistoryAction.Insert,
			out errorMessage,
			disablePaymentCancelOrderId);

		if (isCancelSuccess)
		{
			if (order.IsFixedPurchaseOrder)
			{
				var fixedPurchaseService = DomainFacade.Instance.FixedPurchaseService;
				var fixedPurchaseContainer = fixedPurchaseService.GetContainer(order.FixedPurchaseId);

				fixedPurchaseService.CancelFixedPurchase(
					fixedPurchaseContainer,
					string.Empty,
					string.Empty,
					Constants.FLG_LASTCHANGED_USER,
					Constants.CONST_DEFAULT_DEPT_ID,
					Constants.W2MP_POINT_OPTION_ENABLED,
					UpdateHistoryAction.DoNotInsert);

				fixedPurchaseService.UpdateInvalidate(
					order.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
			}

			if (Constants.NE_OPTION_ENABLED
				&& Constants.NE_COOPERATION_CANCEL_ENABLED
				&& (OrderCommon.UpdateNextEngineOrderForCancel(order.OrderId, null).Item3 == false))
			{
				NextEngineApi.SendFailureCancelOrderMail(order.OrderId, user.UserId);
			}
		}

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CANCEL_UPSELL_TARGET_ORDER);

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();

			Response.Redirect(url);
		}

		// メール送信
		updater.SendOrderCancelMail(
			CartObject.CreateCartByOrder(order),
			this.IsLoggedIn,
			this.RecommendItemNames);
	}

	/// <summary>
	/// 決済金額取得
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>決済金額合計</returns>
	protected string GetSettlementAmount(CartObject cart)
	{
		var settlementAmount = CurrencyManager.ToSettlementCurrencyNotation(
			cart.SettlementAmount,
			cart.SettlementCurrency);
		return settlementAmount;
	}

	/// <summary>
	/// 商品税抜きのメッセージ表示する判定
	/// </summary>
	/// <param name="orderInfo">注文情報</param>
	/// <returns>メッセージ表示か</returns>
	public bool IsDisplayProductTaxExcludedMessage(object orderInfo)
	{
		// 海外配送時の税込請求、またはグローバル対応なしもしくは注文情報がない場合、
		// メッセージを表示しない
		if (this.ProductIncludedTaxFlg
			|| (Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG
				&& (this.ProductIncludedTaxFlg == false))
			|| (Constants.GLOBAL_OPTION_ENABLE == false)
			|| (orderInfo == null))
		{
			return false;
		}

		// 配送先の国と州をまとめる
		var addressInfo = new List<KeyValuePair<string, string>>();
		if (orderInfo is DataView)
		{
			addressInfo = ((DataView)orderInfo)
				.Cast<DataRowView>()
				.Select(item => new KeyValuePair<string, string>(
					(string)item[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE],
					(string)item[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5]))
				.ToList();
		}

		if (orderInfo is CartObject)
		{
			addressInfo = ((CartObject)orderInfo).Shippings
				.Select(item => new KeyValuePair<string, string>(item.ShippingCountryIsoCode, item.Addr5))
				.ToList();
		}

		// 配送先情報がなければ、メッセージを表示しない
		if (addressInfo.Count == 0) return false;

		// 運用地はアメリカであれば、配送先の国と州を考慮する
		if (Constants.OPERATIONAL_BASE_ISO_CODE == Constants.COUNTRY_ISO_CODE_US)
		{
			return addressInfo.Any(item => ((item.Key != Constants.OPERATIONAL_BASE_ISO_CODE)
				|| (item.Value != Constants.OPERATIONAL_BASE_PROVINCE)));
		}

		return addressInfo.Any(item => (item.Key != Constants.OPERATIONAL_BASE_ISO_CODE));
	}

	/// <summary>
	/// Check payment can save default value
	/// </summary>
	/// <param name="paymentId">Payment id</param>
	/// <returns>True: If can save default value, False: If can not save default value</returns>
	protected bool CheckPaymentCanSaveDefaultValue(string paymentId)
	{
		var paymentsCanNotSaveDefault = new List<string>
		{
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
			Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
			Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
			Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
			Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
		};

		var result = paymentsCanNotSaveDefault.All(item => (item != paymentId));
		return result;
	}

	/// <summary>
	/// 配送パターン入力欄の表示が必要か判定
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>配送パターン入力欄表示有無</returns>
	protected bool IsShowDeliveryPatternInputArea(CartObject cart)
	{
		// 注文同梱されていない場合、表示不要
		if (this.IsOrderCombined == false) return false;

		// 注文同梱前のカートに定期購入商品が含まれ、注文同梱元注文に定期購入商品が含まれない場合、定期購入の配送パターンが表示されないため入力欄を表示
		return ((cart.IsBeforeCombineCartHasFixedPurchase) && (cart.IsCombineParentOrderHasFixedPurchase == false));
	}

	/// <summary>
	/// Get text display
	/// </summary>
	/// <param name="text">Text</param>
	/// <param name="isJpAddress">Is JP address</param>
	/// <param name="textDefault">Text default</param>
	/// <param name="textSuffix">Text suffix</param>
	/// <returns>Text display</returns>
	public string GetTextDisplay(
		string text,
		bool isJpAddress = false,
		string textDefault = "-",
		string textSuffix = "")
	{
		if (string.IsNullOrEmpty(text)
			|| (isJpAddress == false))
		{
			return textDefault;
		}

		var result = string.Format(
			"{0}{1}",
			text,
			textSuffix);

		return result;
	}

	/// <summary>
	/// Get class name for display row
	/// </summary>
	/// <returns>Class name display row</returns>
	public string GetClassNameForDisplayRow()
	{
		var result = (this.DispNum++ % 2 == 0)
			? string.Empty
			: CLASS_DISPLAY_ROW_NAME;
		return result;
	}

	/// <summary>
	/// Get class name for display price
	/// </summary>
	/// <param name="price">Price</param>
	/// <param name="displayIfPriceIsSmallerThanOrEqualToZero">Set a css class even if the price is less than or equal to zero</param>
	/// <returns>Class name for display price</returns>
	public string GetClassNameForDisplayPrice(
		decimal price,
		bool displayIfPriceIsSmallerThanOrEqualToZero = false)
	{
		var result = (price > 0)
			? CLASS_DISPLAY_PRICE_NAME
			: displayIfPriceIsSmallerThanOrEqualToZero
				? CLASS_DISPLAY_PRICE_NAME
				: string.Empty;
		return result;
	}

	/// <summary>
	/// Get prefix for display price
	/// </summary>
	/// <param name="price">Price</param>
	/// <param name="displayIfPriceIsSmallerThanOrEqualToZero">Display a prefix even if the price is less than or equal to zero</param>
	/// <returns>Prefix for display price</returns>
	public string GetPrefixForDisplayPrice(
		decimal price,
		bool displayIfPriceIsSmallerThanOrEqualToZero = false)
	{
		var result = (price > 0)
			? PREFIX_DISPLAY_PRICE
			: displayIfPriceIsSmallerThanOrEqualToZero
				? PREFIX_DISPLAY_PRICE
				: string.Empty;
		return result;
	}

	/// <summary>
	/// Get discount price calculate
	/// </summary>
	/// <param name="discountPrice">Discount price</param>
	/// <returns>Discount price display</returns>
	public string GetDiscountPriceCalculate(decimal discountPrice)
	{
		var priceDiscountFactor = (discountPrice > 0)
			? 1
			: -1;
		return CurrencyManager.ToPrice(discountPrice * priceDiscountFactor);
	}

	/// <summary>
	/// Get reflect memo to fixed purchase
	/// </summary>
	/// <param name="hasReflectMemo">Has reflect memo</param>
	/// <returns>Memo fixed purchase</returns>
	public string GetReflectMemoToFixedPurchase(bool hasReflectMemo)
	{
		var result = hasReflectMemo
			? DISPLAY_REFLECT_MEMO_FIXED_PURCHASE
			: string.Empty;
		return result;
	}

	/// <summary>
	/// Get input text display
	/// </summary>
	/// <param name="inputValue">Input value</param>
	/// <param name="inputText">Input text</param>
	/// <returns>Input text display</returns>
	public string GetInputTextDisplay(string inputValue, string inputText)
	{
		var result = string.IsNullOrEmpty(inputValue)
			? INPUT_VALUE_DEFAULT
			: inputText;
		return result;
	}

	/// <summary>
	/// Get cart index display
	/// </summary>
	/// <param name="indexCart">Index cart</param>
	/// <returns>Cart index</returns>
	public string GetCartIndexDisplay(int indexCart)
	{
		var result = (this.CartListRecommend.Items.Count > 1)
			? string.Format(
				"{0}{1}の",
				STRING_CART_INDEX,
				StringUtility.ToEmpty(indexCart + 1))
			: string.Empty;
		return result;
	}

	/// <summary>
	/// Get product set display
	/// </summary>
	/// <param name="productSet">Product set</param>
	/// <returns>Product set display</returns>
	public List<CartProduct> GetProductSetDisplay(CartProductSet productSet)
	{
		if (productSet == null) return null;

		return productSet.Items;
	}

	/// <summary>
	/// Get count of product set
	/// </summary>
	/// <param name="productSet">Product set</param>
	/// <returns>Count of product set</returns>
	public int GetCountOfProductSet(CartProductSet productSet)
	{
		if (productSet == null) return QUANTITY_OF_PRODUCT_SET_DEFAULT;

		return productSet.Items.Count;
	}

	/// <summary>
	/// Get uniform invoice code option name
	/// </summary>
	/// <param name="uniformInvoiceType">Uniform invoice type</param>
	/// <returns>Invoice code option name</returns>
	public string GetUniformInvoiceCodeOptionName(string uniformInvoiceType)
	{
		var result = (uniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
			? ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@")
			: ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@");
		return result;
	}

	/// <summary>
	/// Get carry type value
	/// </summary>
	/// <param name="carryType">Carry type</param>
	/// <returns>Carry type value</returns>
	public string GetCarryTypeValue(string carryType)
	{
		var result = ValueText.GetValueText(
			Constants.TABLE_TWORDERINVOICE,
			Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE,
			carryType);
		if (string.IsNullOrEmpty(carryType) == false)
		{
			result = result.Replace("コード", string.Empty);
		}

		return result;
	}

	/// <summary>
	/// Get text display user credit card registable
	/// </summary>
	/// <param name="flgCanCreditCard">Flag user can register credit card</param>
	/// <returns>Text display user credit card registable</returns>
	public string GetTextDisplayUserCreditCardRegistable(bool flgCanCreditCard)
	{
		var result = flgCanCreditCard
				? STATUS_CREDIT_CARD_ALREADY_REGISTER
				: STATUS_CREDIT_CARD_DO_NOT_REGISTER;
		return result;
	}

	/// <summary>
	/// Get address display text
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Address display</returns>
	public string GetAddressDisplayText(Object cart)
	{
		var zipText = string.Empty;
		if (cart is CartOwner)
		{
			var cartOwner = (CartOwner)cart;
			zipText = string.Format("〒{0}\r", cartOwner.Zip);
			var result = string.Format(
				"{0}{1} {2}\r{3} {4}\r{5} {6}{7}\r",
				(cartOwner.IsAddrJp
					? zipText
					: string.Empty),
				cartOwner.Addr1,
				cartOwner.Addr2,
				cartOwner.Addr3,
				cartOwner.Addr4,
				cartOwner.Addr5,
				((cartOwner.IsAddrJp == false)
					? zipText
					: string.Empty),
				cartOwner.AddrCountryName);

			return result;
		}

		if (cart is CartShipping)
		{
			var cartShipping = (CartShipping)cart;
			var isAddressJp = IsCountryJp(cartShipping.ShippingCountryIsoCode);
			zipText = string.Format("〒{0}\r", cartShipping.Zip);
			var result = string.Format(
				"{0}{1} {2}\r{3} {4}\r{5} {6}{7}\r",
				(isAddressJp
					? zipText
					: string.Empty),
				cartShipping.Addr1,
				cartShipping.Addr2,
				cartShipping.Addr3,
				cartShipping.Addr4,
				cartShipping.Addr5,
				((isAddressJp == false)
					? zipText
					: string.Empty),
				cartShipping.ShippingCountryName);
			return result;
		}

		return string.Empty;
	}

	/// <summary>
	/// Get cart object
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart object</returns>
	public CartObject GetCartObject(object cart)
	{
		return (CartObject)cart;
	}

	/// <summary>
	/// カートオブジェクトを取得する（注文完了ページレコメンド用）
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>カートオブジェクト</returns>
	protected static CartObject GetCartObjectOrderComplete(object cart)
	{
		var cartObjectOrderRecommend = (CartObject)cart;
		cartObjectOrderRecommend.IsNeedCheckOrderCompleteRecommend = true;
		return cartObjectOrderRecommend;
	}

	/// <summary>
	/// Get cart object list
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart object list</returns>
	public CartObjectList GetCartObjectListByRepeater(object cart)
	{
		return (CartObjectList)cart;
	}

	/// <summary>
	/// Get cart owner
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart owner</returns>
	public CartOwner GetCartOwner(object cart)
	{
		return ((CartObject)cart).Owner;
	}

	/// <summary>
	/// Get cart shipping
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart shipping</returns>
	public CartShipping GetCartShipping(object cart)
	{
		return (CartShipping)cart;
	}

	/// <summary>
	/// Get cart payment
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart shipping</returns>
	public CartPayment GetCartPayment(object cart)
	{
		return ((CartObject)cart).Payment;
	}

	/// <summary>
	/// Get cart product
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart product</returns>
	public CartProduct GetCartProduct(object cart)
	{
		return (CartProduct)cart;
	}

	/// <summary>
	/// Get cart set promotion
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Cart set promotion</returns>
	public CartSetPromotion GetCartSetPromotion(object cart)
	{
		return (CartSetPromotion)cart;
	}

	/// <summary>
	/// Get product option setting
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>Product option setting</returns>
	public ProductOptionSetting GetProductOptionSetting(object data)
	{
		return (ProductOptionSetting)data;
	}

	/// <summary>
	/// Get repeater item
	/// </summary>
	/// <param name="item">Item control</param>
	/// <returns>Repeater item</returns>
	public RepeaterItem GetRepeaterItem(Control item)
	{
		return (RepeaterItem)item;
	}

	/// <summary>
	///  カート情報エリアのリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rCart_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var sPriceControl = (HtmlGenericControl)e.Item.FindControl("sPrice");
		if (sPriceControl == null) return;

		var pPriceControl = (HtmlGenericControl)e.Item.FindControl("pPrice");
		var pSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("pSubscriptionBoxCampaignPrice");
		var sSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPrice");
		var pSubscriptionBoxCampaignPeriodControl = (HtmlGenericControl)e.Item.FindControl("pSubscriptionBoxCampaignPeriod");
		var sSubscriptionBoxCampaignPeriodSinceControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPeriodSince");
		var sSubscriptionBoxCampaignPeriodUntilControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPeriodUntil");
		var cartProduct = (CartProduct)e.Item.DataItem;

		if (cartProduct.IsSubscriptionBox == false) return;

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(cartProduct.SubscriptionBoxCourseId);

		var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == cartProduct.ProductId) && (x.VariationId == cartProduct.VariationId));

		var product = new ProductService().GetProductVariation(this.ShopId, cartProduct.ProductId, cartProduct.VariationId, this.MemberRankId);

		// 頒布会キャンペーン期間の場合キャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem))
		{
			sPriceControl.InnerText = CurrencyManager.ToPrice(
					product.FixedPurchasePrice ?? product.Price);
			pPriceControl.Visible
				= pSubscriptionBoxCampaignPeriodControl.Visible
					= true;
			sSubscriptionBoxCampaignPriceControl.InnerText = CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice);

			if (subscriptionBoxItem.CampaignSince.HasValue)
			{
				sSubscriptionBoxCampaignPeriodSinceControl.InnerText =
					HtmlSanitizer.HtmlEncode(
						StringUtility.ToEmpty(((DateTime)subscriptionBoxItem.CampaignSince).ToString("yyyy年MM月dd日 HH時mm分")));
			}
			if (subscriptionBoxItem.CampaignUntil.HasValue)
			{
				sSubscriptionBoxCampaignPeriodUntilControl.InnerText =
					HtmlSanitizer.HtmlEncode(
						StringUtility.ToEmpty(((DateTime)subscriptionBoxItem.CampaignUntil).ToString("yyyy年MM月dd日 HH時mm分")));
			}
		}

		if (cartProduct.IsSubscriptionBoxFixedAmount())
		{
			pPriceControl.Visible
				= pSubscriptionBoxCampaignPriceControl.Visible
					= pSubscriptionBoxCampaignPeriodControl.Visible
						= false;
		}
	}

	/// <summary>
	/// Get cvs type
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="index">Index</param>
	/// <returns>Cvs type</returns>
	private string GetCvsType(CartObject cart, int index)
	{
		if ((this.Recommend != null)
			&& this.Recommend.IsCrosssell
			&& cart.HasRecommendItem
			&& (this.Orders.Count != this.CartList.Items.Count)
			&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
		{
			var payment = this.CartList.Items
				.First(item => ((item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
					&& (string.IsNullOrEmpty(item.Payment.RakutenCvsType) == false)))
				.Payment;

			return payment.RakutenCvsType;
		}

		if ((this.Recommend != null)
			&& this.Recommend.IsCrosssell
			&& cart.HasRecommendItem
			&& (this.Orders.Count != this.CartList.Items.Count)
			&& OrderCommon.IsPaymentCvsTypeZeus)
		{
			var payment = this.CartList.Items
				.First(item => ((item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
					&& (string.IsNullOrEmpty(item.Payment.GetZeusCvsType()) == false)))
				.Payment;

			return payment.GetZeusCvsType();
		}

		var result = StringUtility.ToEmpty(this.Orders[index][Constants.CONST_PAYMENT_CVS_TYPE]);
		return result;
	}

	/// <summary>
	/// Get real shop name
	/// </summary>
	/// <param name="coIndex">Index</param>
	/// <returns>Real shop name</returns>
	protected string GetRealShopName(int coIndex)
	{
		var realShopId = this.OrderOld[0].Shippings[coIndex].StorePickupRealShopId;
		var realShopName = string.Empty;

		if (string.IsNullOrEmpty(realShopId) == false)
		{
			var realShop = new RealShopService().Get(realShopId);
			realShopName = realShop.Name;
		}

		return realShopName;
	}

	/// <summary>
	/// Get real shop opening hours
	/// </summary>
	/// <param name="coIndex">Index</param>
	/// <returns>Real shop opening hours</returns>
	protected string GetRealShopOpeningHours(int coIndex)
	{
		var realShopId = this.OrderOld[0].Shippings[coIndex].StorePickupRealShopId;
		var realShopOpeningHours = string.Empty;

		if (string.IsNullOrEmpty(realShopId) == false)
		{
			var realShop = new RealShopService().Get(realShopId);
			realShopOpeningHours = realShop.OpeningHours;
		}

		return realShopOpeningHours;
	}

	/// <summary>
	/// Is store pickup displayed
	/// </summary>
	/// <param name="dataItem">Data Item</param>
	/// <param name="isStorePickup">Is store pickup</param>
	/// <returns>True if cart is digital and shipping address kbn is different from store pickup address kbn, otherwise false</returns>
	protected bool IsStorePickupDisplayed(object dataItem, bool isStorePickup = false)
	{
		var cart = FindCart(dataItem);
		var cartShipping = GetCartShipping(dataItem);
		var result = (cart.IsDigitalContentsOnly == false)
			&& cartShipping.IsConvenienceStoreFlagOff
			&& (cartShipping.IsShippingStorePickup == isStorePickup);

		return result;
	}

	/// <summary>Amazon連携かどうか</summary>
	public bool IsAmazonLogin
	{
		get
		{
			return ((Constants.AMAZON_LOGIN_OPTION_ENABLED || Constants.AMAZON_PAYMENT_OPTION_ENABLED)
				&& (Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] != null));
		}
	}
	/// <summary>AmazonPay(CV2)かつゲスト購入か</summary>
	public bool IsAmazonCv2Guest
	{
		get
		{
			return (this.IsAmazonLogin
				&& Constants.AMAZON_PAYMENT_CV2_ENABLED
				&& (this.IsLoggedIn == false));
		}
	}
	/// <remarks>ページ側で設定される</remarks>
	public List<DataView> OrderList { get; set; }
	/// <summary>元注文情報</summary>
	private OrderModel[] OrderOld
	{
		get { return (OrderModel[])ViewState["OrderOld"]; }
		set { ViewState["OrderOld"] = value; }
	}
	/// <summary>注文者ユーザーID</summary>
	private string OrderUserId
	{
		get { return (string)ViewState["OrderUserId"]; }
		set { ViewState["OrderUserId"] = value; }
	}
	/// <summary>レコメンド商品名</summary>
	private string[] RecommendItemNames
	{
		get { return (string[])ViewState["RecommendItemNames"]; }
		set { ViewState["RecommendItemNames"] = value; }
	}
	/// <summary>レコメンド設定</summary>
	protected RecommendModel Recommend
	{
		get { return (RecommendModel)ViewState["Recommend"]; }
		set { ViewState["Recommend"] = value; }
	}
	/// <summary>レコメンド表示</summary>
	protected string RecommendDisplay
	{
		get { return StringUtility.ToEmpty(ViewState["RecommendDisplay"]); }
		set { ViewState["RecommendDisplay"] = value; }
	}
	/// <summary>レコメンド表示履歴枝番</summary>
	protected int RecommendHistoryNo
	{
		get { return (int)ViewState["RecommendHistoryNo"]; }
		set { ViewState["RecommendHistoryNo"] = value; }
	}
	/// <remarks>Cart list recommend</remarks>
	public CartObjectList CartListRecommend
	{
		get { return (CartObjectList)ViewState["CartListRecommend"]; }
		set { ViewState["CartListRecommend"] = value; }
	}
	/// <summary>特商法に基づく記載を表示するか</summary>
	protected bool IsDispCorrespondenceSpecifiedCommericalTransactions
	{
		get
		{
			return Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE
				&& (string.IsNullOrEmpty(ShopMessage.GetMessage("SpecifiedCommercialTransactions")) == false);
		}
	}
	/// <summary>現在のカート情報格納用</summary>
	protected CartObject BindingCartList
	{
		get { return (CartObject)ViewState["bindingCartList"]; }
		set { ViewState["bindingCartList"] = value; }
	}
	/// <summary>Orders</summary>
	protected List<Hashtable> Orders
	{
		get { return (List<Hashtable>)ViewState["Orders"]; }
		set { ViewState["Orders"] = value; }
	}
	/// <summary>Has order history similar shipping</summary>
	protected bool HasOrderHistorySimilarShipping
	{
		get { return this.CartList.HasOrderHistorySimilarShipping; }
	}
	/// <summary>Has card payment</summary>
	protected bool HasCardPayment
	{
		get { return this.CartList.Items.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)); }
	}
	/// <summary>Has cvs def payment</summary>
	protected bool HasCvsDefPayment
	{
		get { return this.CartList.Items.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)); }
	}
	/// <summary>Has paypal payment</summary>
	protected bool HasPayPalPayment
	{
		get { return this.CartList.Items.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)); }
	}
	/// <summary>Has NP After pay payment</summary>
	protected bool HasNPAfterPayPayment
	{
		get
		{
			return this.CartList.Items.Any(cartObject =>
				(cartObject.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
		}
	}
	/// <summary>Hide order button with click</summary>
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
}
