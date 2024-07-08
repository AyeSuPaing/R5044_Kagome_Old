/*
=========================================================================================================
  Module      : デフォルト注文方法管理クラス(UserDefaultOrderManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global;
using w2.App.Common.Order;
using w2.Domain.User;
using w2.App.Common.Order.Payment.ECPay;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;

/// <summary>
/// デフォルト注文方法管理クラス
/// </summary>
public class UserDefaultOrderManager
{
	/// <summary>カートリスト</summary>
	private readonly CartObjectList m_cartList = null;
	/// <summary>デフォルト注文方法設定情報</summary>
	private readonly UserDefaultOrderSettingModel m_userDefaultOrderSettingModel = null;
	/// <summary>レコメンド商品が投入されたか？</summary>
	private readonly bool m_isAddRecomendItem = false;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法設定情報</param>
	/// <param name="isAddRecmendItem">レコメンド商品が投入されたか？</param>
	public UserDefaultOrderManager(CartObjectList cartList, UserDefaultOrderSettingModel userDefaultOrderSetting, bool isAddRecmendItem)
	{
		m_cartList = cartList;
		m_userDefaultOrderSettingModel = userDefaultOrderSetting;
		m_isAddRecomendItem = isAddRecmendItem;
	}

	/// <summary>
	/// 画面遷移先とデフォルト注文方法を設定
	/// </summary>
	/// <param name="hasSelectedShippingMethod">選択された配送方法あり</param>
	public void SetNextPageAndDefaultOrderSetting(bool hasSelectedShippingMethod)
	{
		if (Constants.TWOCLICK_OPTION_ENABLE == false ) return; 
		// デフォルト注文方法設定がありレコメンド商品追加時でない場合、デフォルト注文方法をカートに設定し画面遷移先を決定する。
		if ((m_userDefaultOrderSettingModel != null) && (m_isAddRecomendItem == false))
		{
			SetDefaultOrderSetting(hasSelectedShippingMethod);
			m_cartList.CartNextPage
				= GetNextPage(
					m_userDefaultOrderSettingModel.PaymentId,
					m_userDefaultOrderSettingModel.UserShippingNo.ToString(),
					m_userDefaultOrderSettingModel.UserInvoiceNo.ToString());
		}
	}

	/// <summary>
	/// デフォルト注文方法設定
	/// </summary>
	/// <param name="hasSelectedShippingMethod">選択された配送方法あり</param>
	private void SetDefaultOrderSetting(bool hasSelectedShippingMethod)
	{
		m_cartList.SetDefaultOrderSettingForUserDefaultOrder(m_userDefaultOrderSettingModel, hasSelectedShippingMethod);

		m_cartList.UserDefaultOrderSettingParm.IsChangedUserDefaultShipping = false;
		m_cartList.UserDefaultOrderSettingParm.IsChangedUserDefaultPayment = false;
		m_cartList.UserDefaultOrderSettingParm.IsChangedUserDefaultInvoice = false;
	}

	/// <summary>
	/// 画面遷移先URL取得
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <param name="shippingNo">配送先枝番</param>
	/// <param name="invoiceNo">電子発票枝番</param>
	/// <returns>画面遷移先URL</returns>
	private string GetNextPage(string paymentId, string shippingNo, string invoiceNo)
	{
		// デフォルト配送先が設定されているか
		var canDeliverTo = (string.IsNullOrEmpty(shippingNo) == false);
		
		// デフォルト配送先の国が配送可能かを判定する
		if (Constants.GLOBAL_OPTION_ENABLE && canDeliverTo)
		{
			var userShipping = new UserShippingService().Get(m_cartList.UserId, int.Parse(shippingNo));
			var shippingCountryIsoCode = shippingNo.Equals("0")
				? new UserService().Get(m_cartList.UserId).AddrCountryIsoCode
				: ((userShipping != null) ? userShipping.ShippingCountryIsoCode : string.Empty);

			canDeliverTo = ShippingCountryUtil.GetShippingCountryAvailableListAndCheck(shippingCountryIsoCode);

			// When User Default Setting Is Convenience Store Address
			// If Exist Product Not Relate With Service Shipping Convenience Store Then Set Page Order Shipping
			if ((userShipping != null)
				&& (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				&& canDeliverTo)
			{
				canDeliverTo = CheckDataForShippingConvenienceStore(paymentId, userShipping);
			}

			// Check Country Iso Code Can Order With NP After Pay
			if (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED && canDeliverTo)
			{
				canDeliverTo = CheckCountryIsoCodeCanOrderWithNPAfterPay(
					paymentId,
					new UserService().Get(m_cartList.UserId).AddrCountryIsoCode,
					shippingCountryIsoCode);
			}

			if (OrderCommon.DisplayTwInvoiceInfo() && canDeliverTo)
			{
				var hasInvoiceNo = (string.IsNullOrEmpty(invoiceNo) == false);
				canDeliverTo = (canDeliverTo && hasInvoiceNo);
			}

			// Check Owner & Default Shipping For Payment Ec Pay
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			{
				if ((GlobalAddressUtil.IsCountryTw(shippingCountryIsoCode) == false)
					|| (GlobalAddressUtil.IsCountryTw(m_cartList.Owner.AddrCountryIsoCode) == false))
				{
					paymentId = string.Empty;
				}
			}

			// Check Owner & Default Shipping For Payment NewebPay
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				if ((GlobalAddressUtil.IsCountryTw(shippingCountryIsoCode) == false)
					|| (GlobalAddressUtil.IsCountryTw(m_cartList.Owner.AddrCountryIsoCode) == false))
				{
					paymentId = string.Empty;
				}
			}
		}

		// Check vaild Zeus cvs type
		if (OrderCommon.IsPaymentCvsTypeZeus
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (m_userDefaultOrderSettingModel != null)
			&& string.IsNullOrEmpty(m_userDefaultOrderSettingModel.ZeusCvsType))
		{
			paymentId = string.Empty;
		}

		var choseInvoice = ((string.IsNullOrEmpty(invoiceNo) == false)
			|| ((m_cartList.Owner != null) && (m_cartList.Owner.IsAddrTw == false)));

		var hasFixedPurchaseShippingPattern =
			m_cartList.Items.Any(cart => cart.HasFixedPurchase && cart.GetShipping().HasFixedPurchaseSetting);

		var nextPage = GetNextPage(paymentId, canDeliverTo, choseInvoice, hasFixedPurchaseShippingPattern);
		return nextPage;
	}
	/// <summary>
	/// 画面遷移先URL取得
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <param name="canDeliverTo">配送可能</param>
	/// <param name="choseInvoice">電子発票が選択されている</param>
	/// <param name="hasFixedPurchaseShippingPattern">Has Fixed Purchase Shipping Pattern</param>
	/// <returns>画面遷移先URL</returns>
	private string GetNextPage(
		string paymentId,
		bool canDeliverTo,
		bool choseInvoice,
		bool hasFixedPurchaseShippingPattern = false)
	{
		if (SessionSecurityManager.IsEasyUser(HttpContext.Current)) return Constants.PAGE_FRONT_ORDER_SHIPPING;

		// 上からカート画面の遷移順にチェック
		// ギフトOPがONまたは定期購入有りの場合は「注文配送先選択画面」
		// 注文拡張項目オプションがONで必須項目が未入力の場合は「注文配送先選択画面」
		// デフォルト配送先か電子発票が設定されていないか問題がある場合は「注文配送先選択画面」
		// 配送希望日に問題がある場合は「注文配送先選択画面」
		// デフォルト支払方法が設定されていない場合は「注文お支払方法選択画面」
		// 全て設定されており、問題がなければ「注文確認画面」
		var result = (Constants.GIFTORDER_OPTION_ENABLED
			|| (m_cartList.HasFixedPurchase && (hasFixedPurchaseShippingPattern == false))) && (SessionManager.IsTwoClickButton == false)
				? Constants.PAGE_FRONT_ORDER_SHIPPING
				: CheckNextShippingForOrderExtend()
					? Constants.PAGE_FRONT_ORDER_SHIPPING
					: ((canDeliverTo == false) || (Constants.INVOICECSV_ENABLED && (choseInvoice == false)))
						? Constants.PAGE_FRONT_ORDER_SHIPPING
						: CheckNextShippingForOrderShippingDate()
							? Constants.PAGE_FRONT_ORDER_SHIPPING
							: string.IsNullOrEmpty(paymentId)
								? Constants.PAGE_FRONT_ORDER_PAYMENT
								: Constants.PAGE_FRONT_ORDER_CONFIRM;
		return result;
	}

	/// <summary>
	/// 注文拡張項目で配送先画面の表示が必要かチェック
	/// </summary>
	/// <returns>結果</returns>
	private bool CheckNextShippingForOrderExtend()
	{
		// オプションがOFF false;
		if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return false;

		// 必須項目が設定されていない
		var orderExtend = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels
			.Where(m => m.CanUseFront && m.CanUseRegister && m.IsNeecessary).ToArray();

		if ((orderExtend.Any() == false)) return false;

		var result = m_cartList.Items.Any(cart => orderExtend.Any(model => (cart.OrderExtend.ContainsKey(model.SettingId) && string.IsNullOrEmpty(cart.OrderExtend[model.SettingId].Value))));
		return result;
	}

	/// <summary>
	/// 配送希望日によって、配送先画面の表示が必要かどうかチェックする
	/// </summary>
	/// <returns>配送先画面表示の要否</returns>
	private bool CheckNextShippingForOrderShippingDate()
	{
		var result = m_cartList.Items.Any(
			cart => cart.Shippings.Any(
				shipping => (shipping.ShippingDate.HasValue && (OrderCommon.CanCalculateScheduledShippingDate(cart.ShopId, shipping) == false))));

		return result;
	}

	/// <summary>
	/// Check Country Iso Code Can Order With NP After Pay
	/// </summary>
	/// <param name="paymentId">Payment Id</param>
	/// <param name="ownerCountryIsoCode">Owner Country Iso Code</param>
	/// <param name="shippingCountryIsoCode">Shipping Country Iso Code</param>
	/// <returns>True: Can Order, otherwise: false</returns>
	private bool CheckCountryIsoCodeCanOrderWithNPAfterPay(
		string paymentId,
		string ownerCountryIsoCode,
		string shippingCountryIsoCode)
	{
		if (paymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY) return true;

		if ((GlobalAddressUtil.IsCountryJp(ownerCountryIsoCode) == false)
			|| (GlobalAddressUtil.IsCountryJp(shippingCountryIsoCode) == false)) return false;
		return true;
	}

	/// <summary>
	/// Check Data For Shipping Convenience Store
	/// </summary>
	/// <param name="paymentId">Payment Id</param>
	/// <param name="userShipping">User Shipping</param>
	/// <returns>True: Can Order, otherwise: false</returns>
	private bool CheckDataForShippingConvenienceStore(string paymentId, UserShippingModel userShipping)
	{
		var result = m_cartList.IsAllProductRelateWithServiceShippingConvenienceStore;

		if (result
			&& (string.IsNullOrEmpty(paymentId) == false)
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
		{
			result = (ECPayUtility.GetIsCollection(userShipping.ShippingReceivingStoreType)
				== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON);
		}

		if (result
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			result = m_cartList.Items.Any(cart =>
				(cart.Shippings.Any(
					shipping => (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(cart, shipping.ShippingReceivingStoreType) == false))));
		}

		return result;
	}
}
