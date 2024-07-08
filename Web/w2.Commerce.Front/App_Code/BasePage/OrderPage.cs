/*
=========================================================================================================
  Module      : 注文系基底ページ(OrderPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.App.Common.Global.Config;
using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.Process;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.App.Common;
using w2.Domain.SubscriptionBox;
using System.Collections.Generic;
using w2.Common.Extensions;

///*********************************************************************************************
/// <summary>
/// 注文系基底ページ
/// </summary>
///*********************************************************************************************
public class OrderPage : ProductPage
{
	/// <summary>Data ECPay</summary>
	public const string DATA_ECPAY = "data_ec_pay";

	/// <summary>Data NewebPay</summary>
	public const string DATA_NEWEBPAY = "data_neweb_pay";

	/// <summary>
	/// ページ初期化
	/// </summary>
	protected new void Page_Init(object sender, EventArgs e)
	{
		this.Process.Page_Init(sender, e);
	}

	/// <summary>
	/// HTTPS通信チェック（ＮＧの場合ショッピングカートへ遷移）
	/// </summary>
	public override void CheckHttps()
	{
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 利用ポイントチェック
	/// </summary>
	/// <param name="coTarget">対象カートオブジェクト</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckUsePoint(CartObject coTarget)
	{
		return this.Process.CheckUsePoint(coTarget);
	}

	/// <summary>
	/// カート存在チェック
	/// </summary>
	/// <remarks>
	/// カートが存在しない場合、エラーページへ遷移
	/// </remarks>
	public void CheckCartExists()
	{
		this.Process.CheckCartExists();
	}

	/// <summary>
	/// カート存在チェック
	/// </summary>
	/// <param name="messages">エラーメッセージ</param>
	/// <remarks>
	/// カートが存在しない場合、エラーページへ遷移
	/// </remarks>
	public void CheckCartExists(string messages)
	{
		this.Process.CheckCartExists(messages);
	}

	/// <summary>
	/// カート注文者存在チェック
	/// </summary>
	/// <remarks>
	/// 注文者が存在しない場合カート一覧ページへ飛ぶ
	/// </remarks>
	public void CheckCartOwnerExists()
	{
		this.Process.CheckCartOwnerExists();
	}

	/// <summary>
	/// カート配送先存在チェック
	/// </summary>
	/// <remarks>
	/// 注文者が存在しない場合カート一覧ページへ飛ぶ
	/// </remarks>
	public void CheckCartShippingExists()
	{
		this.Process.CheckCartShippingExists();
	}

	/// <summary>
	/// カート注文者チェック
	/// </summary>
	/// <remarks>
	/// メールアドレスが登録されていない OR ログイン状態とカート注文者情報が不整合の場合配送先ページへ飛ぶ
	/// </remarks>
	public void CheckCartOwner()
	{
		this.Process.CheckCartOwner();
	}

	/// <summary>
	/// カート配送先エリアチェック
	/// </summary>
	protected void CheckCartOwnerShippingArea()
	{
		this.Process.CheckCartOwnerShippingArea();
	}

	/// <summary>
	/// URLセッションチェック(注文時)
	/// </summary>
	/// <param name="hssSession">HttpSessionState</param>
	/// <param name="hrResponse">HttpResponse</param>
	/// <param name="strCurrentPage">現在のページパス</param>
	/// <remarks>
	/// 画面遷移の正当性をチェックしたいときに使用する。
	///
	/// １．遷移元で画面遷移する際、セッションパラメタに
	///		 > htParam.Add(Constants.HASH_KEY_NEXT_PAGE_FOR_CHECK, Constants.PAGE_FRONT_USER_MODIFY_CONFIRM);
	///		 > Session[Constants.SESSION_KEY_PARAM] = htParam;
	///		 のように次ページのURLを格納する。
	///		
	/// ２．遷移先のページでこのメソッドをコールすることにより、遷移元のから遷移であるかを確認する。
	///		> SessionManager.CheckUrlSession(Session, Response, this.RawUrl);
	///
	///	３. 注文時のみ使用。URLが存在しない場合はカート画面を表示、URLが一致しない場合はセッションに格納されているURLへ遷移
	///
	/// ※主に、入力データを確認画面で正しく受け取るための制御に使用する。
	/// </remarks>
	public void CheckOrderUrlSession()
	{
		this.Process.CheckOrderUrlSession();
	}

	/// <summary>
	/// 郵便番号文字列作成
	/// </summary>
	/// <param name="objCartInfo">カート情報（注文者or配送先）</param>
	/// <returns>郵便番号文字列</returns>
	public string CreateZipString(object objCartInfo)
	{
		return this.Process.CreateZipString(objCartInfo);
	}
	/// <summary>
	/// 郵便番号文字列作成
	/// </summary>
	/// <param name="strZip1">郵便番号1</param>
	/// <param name="strZip2">郵便番号2</param>
	/// <returns>郵便番号文字列</returns>
	public string CreateZipString(string strZip1, string strZip2)
	{
		return this.Process.CreateZipString(strZip1, strZip2);
	}

	/// <summary>
	/// 電話番号文字列作成
	/// </summary>
	/// <param name="objCartInfo">カート情報（注文者or配送先）</param>
	/// <returns>電話番号文字列</returns>
	public string CreateTelString(object objCartInfo)
	{
		return this.Process.CreateTelString(objCartInfo);
	}
	/// <summary>
	/// 電話番号文字列作成
	/// </summary>
	/// <param name="strTel1">電話番号1</param>
	/// <param name="strTel2">電話番号2</param>
	/// <param name="strTel3">電話番号3</param>
	/// <returns>電話番号文字列</returns>
	public string CreateTelString(string strTel1, string strTel2, string strTel3)
	{
		return this.Process.CreateTelString(strTel1, strTel2, strTel3);
	}

	/// <summary>
	/// クーポンコード取得
	/// </summary>
	/// <param name="ccCoupon">クーポン情報</param>
	/// <returns>クーポンコード</returns>
	public string GetCouponCode(CartCoupon ccCoupon)
	{
		return this.Process.GetCouponCode(ccCoupon);
	}

	/// <summary>
	/// 表示用クーポン名取得
	/// </summary>
	/// <param name="ccCoupon">クーポン情報</param>
	/// <returns>表示用クーポン名取得</returns>
	public string GetCouponDispName(CartCoupon ccCoupon)
	{
		return this.Process.GetCouponCode(ccCoupon);
	}

	/// <summary>
	/// 商品が有効か?
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>有効：true、無効：false</returns>
	public bool IsProductValid(DataRowView orderItem)
	{
		return this.Process.IsProductValid(orderItem);
	}

	/// <summary>
	/// 商品詳細リンク有効か?
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>有効：true、無効：false</returns>
	public bool IsProductDetailLinkValid(DataRowView orderItem)
	{
		return this.Process.IsProductDetailLinkValid(orderItem);
	}

	/// <summary>
	/// ギフト購入したもののカート商品と配送先情報に紐づく商品情報の整合性チェック
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	/// <returns>整合性チェック結果</returns>
	public bool IsConsistentGiftItemsAndShippings(CartObjectList cartList)
	{
		return this.Process.IsConsistentGiftItemsAndShippings(cartList);
	}

	/// <summary>
	/// 新しいセッションを生成し、配送先入力画面へ遷移
	/// </summary>
	protected void RedirectToOrderShippingWithNewSession()
	{
		this.Process.RedirectToOrderShippingWithNewSession();
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
				? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, shipping.CarryType)
				: string.Format("{0}<br/>{1}",
					ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, shipping.CarryType).Replace("コード", string.Empty),
					shipping.CarryTypeOptionValue);
		}
		else
		{
			switch (shipping.UniformInvoiceType)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					result = ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, shipping.UniformInvoiceType);
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					result = string.Format("{0}<br/>{1}<br/>{2}",
						ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, shipping.UniformInvoiceType),
						shipping.UniformInvoiceOption1,
						shipping.UniformInvoiceOption2);
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					result = string.Format("{0}<br/>{1}",
						ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, shipping.UniformInvoiceType),
						shipping.UniformInvoiceOption1);
					break;
			}
		}

		return result;
	}

	#region カート表示用
	/// <summary>
	/// 商品セットアイテムRowSpan取得
	/// </summary>
	/// <param name="cpProduct"></param>
	/// <returns></returns>
	public static int GetProductSetRowspan(CartProduct cpProduct)
	{
		if (cpProduct.IsSetItem == false)
		{
			return 1;
		}

		return cpProduct.ProductSet.Items.Count;
	}

	/// <summary>
	/// 商品セット数取得
	/// </summary>
	/// <param name="cpProduct"></param>
	/// <returns></returns>
	public static int GetProductSetCount(CartProduct cpProduct)
	{
		if (cpProduct.IsSetItem == false)
		{
			return 0;
		}

		return cpProduct.ProductSet.ProductSetCount;
	}

	/// <summary>
	/// 商品セット小計取得
	/// </summary>
	/// <param name="cpProduct"></param>
	/// <returns></returns>
	public static decimal GetProductSetPriceSubtotal(CartProduct cpProduct)
	{
		if (cpProduct.IsSetItem == false)
		{
			return 0;
		}
		var result = cpProduct.ProductSet.ProductSetPriceSubtotal
			+ (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && (cpProduct.ProductOptionSettingList != null)
				? cpProduct.ProductOptionSettingList.SelectedOptionTotalPrice
				: 0m);

		return result;
	}

	/// <summary>
	/// 商品セットID取得
	/// </summary>
	/// <param name="cpProduct"></param>
	/// <returns></returns>
	public static string GetProductSetId(CartProduct cpProduct)
	{
		if (cpProduct.IsSetItem == false)
		{
			return "";
		}

		return cpProduct.ProductSet.ProductSetId;
	}

	/// <summary>
	/// 商品セットNO取得
	/// </summary>
	/// <param name="cpProduct"></param>
	/// <returns></returns>
	public static int GetProductSetNo(CartProduct cpProduct)
	{
		if (cpProduct.IsSetItem == false)
		{
			return 0;
		}

		return cpProduct.ProductSet.ProductSetNo;
	}

	/// <summary>
	/// 商品セット名取得
	/// </summary>
	/// <param name="cpProduct"></param>
	/// <returns></returns>
	public static string GetProductSetName(CartProduct cpProduct)
	{
		if (cpProduct.IsSetItem == false)
		{
			return "";
		}

		return cpProduct.ProductSet.ProductSetName + cpProduct.ProductSet.ProductSetNo.ToString();
	}
	#endregion

	#region #GetOrderItemProductTranslationName 注文商品翻訳名取得
	/// <summary>
	/// 注文商品翻訳名取得
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <returns>注文商品翻訳名</returns>
	protected string GetOrderItemProductTranslationName(object orderItem)
	{
		var orderItemProductName = (string)ProductCommon.GetKeyValue(orderItem, Constants.FIELD_ORDERITEM_PRODUCT_NAME);
		if (Constants.GLOBAL_OPTION_ENABLE == false) return orderItemProductName;

		var productId = (string)ProductCommon.GetKeyValue(orderItem, Constants.FIELD_ORDERITEM_PRODUCT_ID);
		var variationId = (string)ProductCommon.GetKeyValue(orderItem, Constants.FIELD_ORDERITEM_VARIATION_ID);

		var productTranslationName =
			NameTranslationCommon.GetOrderItemProductTranslationName(
				orderItemProductName,
				productId,
				variationId,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		return productTranslationName;
	}
	#endregion

	#region #GetOrderSetPromotionDispNameTranslationName 注文セットプロモーション表示用セットプロモーション名翻訳名取得
	/// <summary>
	/// 注文セットプロモーション表示用セットプロモーション名翻訳名取得
	/// </summary>
	/// <param name="orderSetPromotionItem">注文セットプロモーション情報</param>
	/// <returns>表示用セットプロモーション名翻訳名</returns>
	protected string GetOrderSetPromotionDispNameTranslationName(object orderSetPromotionItem)
	{
		var setPromotionDispName = (string)ProductCommon.GetKeyValue(orderSetPromotionItem, Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME);
		if (Constants.GLOBAL_OPTION_ENABLE == false) return setPromotionDispName;

		var setPromotionId = (string)ProductCommon.GetKeyValue(orderSetPromotionItem, Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID);
		var setPromotionDispNameTranslationName = NameTranslationCommon.GetTranslationName(
			setPromotionId,
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME,
			setPromotionDispName);
		return setPromotionDispNameTranslationName;
	}
	#endregion

	/// <summary>
	/// Add Product Grant Novelty When Auto Additional Flag is ON
	/// </summary>
	/// <param name="cartNoveltyList">Cart Novelty List</param>
	protected void AddProductGrantNovelty(CartNoveltyList cartNoveltyList)
	{
		this.Process.AddProductGrantNovelty(cartNoveltyList);
	}

	/// <summary>プロセス</summary>
	protected new OrderFlowProcess Process
	{
		get { return (OrderFlowProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new OrderFlowProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	/// <summary>カートリスト</summary>
	protected CartObjectList CartList
	{
		get { return this.Process.CartList; }
		set { this.Process.CartList = value; }
	}
	/// <summary>セッションパラメタが生きているか</summary>
	public bool AliveSessionParameter
	{
		get { return (Session[Constants.SESSION_KEY_PARAM] != null); }
	}
	/// <summary>配送会社リスト</summary>
	public DeliveryCompanyModel[] AllDeliveryCompanyList
	{
		get
		{
			if (m_allDeliveryComapnyList == null)
			{
				this.m_allDeliveryComapnyList = new DeliveryCompanyService().GetAll();
			}
			return m_allDeliveryComapnyList;
		}
	}
	public DeliveryCompanyModel[] m_allDeliveryComapnyList;
	/// <summary>レコメンド商品が投入されたか？</summary>
	protected bool IsAddRecmendItem
	{
		get { return this.Process.IsAddRecmendItem; }
	}
	/// <summary>出荷予定日を表示するか</summary>
	protected bool DisplayScheduledShippingDate
	{
		get { return (GlobalConfigUtil.UseLeadTime() && Constants.SCHEDULED_SHIPPING_DATE_VISIBLE); }
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
	/// <summary>
	/// Is valid modify front
	/// </summary>
	/// <param name="paymentId">Payment id</param>
	/// <param name="paymentIdsCanChange">Payment ids can change</param>
	/// <returns>True if payment is valid</returns>
	public bool IsValidModifyFront(string paymentId, string[] paymentIdsCanChange)
	{
		if (IsNotSetCanChangePayment(paymentIdsCanChange)) return true;

		var result = paymentIdsCanChange.Contains(paymentId);
		return result;
	}

	/// <summary>
	/// Is not set can change payment
	/// </summary>
	/// <param name="paymentIdsCanChange">Payment ids can change</param>
	/// <returns>True: if not set can change payment</returns>
	private bool IsNotSetCanChangePayment(string[] paymentIdsCanChange)
	{
		var result = ((paymentIdsCanChange.Length == 0)
			|| string.IsNullOrEmpty(paymentIdsCanChange[0]));

		return result;
	}

	/// <summary>
	/// カートにて注文拡張項目を表示できるか
	/// </summary>
	/// <returns>表示できるかどうか</returns>
	protected bool IsDisplayOrderExtend()
	{
		return this.Process.IsDisplayOrderExtend();
	}

	/// <summary>
	/// Get coupon name
	/// </summary>
	/// <param name="target">Target</param>
	/// <returns>Coupon name</returns>
	protected string GetCouponName(object target)
	{
		return this.Process.GetCouponName(target);
	}

	///*********************************************************************************************
	/// <summary>
	/// カートエラーメッセージを格納する
	/// </summary>
	/// <remarks>
	/// カート上では入力箇所に対してのエラーを出力する必要があるため、
	/// カートINDEX、アイテムINDEX（またはポイントorクーポンのエラー区分）に対しての
	/// エラーメッセージを格納します。
	/// 画面に対してのエラーを格納する場合は双方-1を格納します。
	/// </remarks>
	///*********************************************************************************************
	public class CartErrorMessages
	{
		Hashtable m_htErrorMessages = new Hashtable();

		/// <summary>エラー区分</summary>
		public enum ErrorKbn
		{
			/// <summary>ポイント</summary>
			Point = -1,
			/// <summary>クーポン</summary>
			Coupon = -2
		}

		/// <summary>
		/// エラーメッセージ追加（ページ全体用）
		/// </summary>
		/// <param name="strErrorMessage">エラーメッセージ</param>
		public void Add(string strErrorMessage)
		{
			this.Add(-1, -1, strErrorMessage);
		}
		/// <summary>
		/// エラーメッセージ追加（ポイント・クーポン用）
		/// </summary>
		/// <param name="iCartIndex">カートindex</param>
		/// <param name="ecErrorKbn">エラー区分</param>
		/// <param name="strErrorMessage">エラーメッセージ</param>
		public void Add(int iCartIndex, ErrorKbn ecErrorKbn, string strErrorMessage)
		{
			this.Add(iCartIndex, (int)ecErrorKbn, strErrorMessage);
		}
		/// <summary>
		/// エラーメッセージ追加（カートアイテム用）
		/// </summary>
		/// <param name="iCartIndex">カートindex</param>
		/// <param name="iItemIndex">カートアイテムindex</param>
		/// <param name="strErrorMessage">エラーメッセージ</param>
		public void Add(int iCartIndex, int iItemIndex, string strErrorMessage)
		{
			if (strErrorMessage != "")
			{
				m_htErrorMessages[iCartIndex + " " + iItemIndex] += strErrorMessage + "\r\n";
			}
		}
		/// <summary>
		/// エラーメッセージ追加（セットプロモーションアイテム用）
		/// </summary>
		/// <param name="cartIndex">カートindex</param>
		/// <param name="setPromotionIndex">セットプロモーションindex</param>
		/// <param name="setPromotionItemIndex">セットプロモーションアイテムindex</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		public void Add(int cartIndex, int setPromotionIndex, int setPromotionItemIndex, string errorMessage)
		{
			if (errorMessage != "")
			{
				m_htErrorMessages[cartIndex + " " + setPromotionIndex + " " + setPromotionItemIndex] += errorMessage + "\r\n";
			}
		}

		/// <summary>
		/// エラーメッセージ取得（ページ全体用）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Get()
		{
			return this.Get(-1, -1);
		}
		/// <summary>
		/// エラーメッセージ取得（ポイント・クーポン用）
		/// </summary>
		/// <param name="iCartIndex">カートindex</param>
		/// <param name="ecErrorKbn">エラー区分</param>
		/// <returns>エラーメッセージ</returns>
		public string Get(int iCartIndex, ErrorKbn ecErrorKbn)
		{
			return this.Get(iCartIndex, (int)ecErrorKbn);
		}
		/// <summary>
		/// エラーメッセージ取得（カートアイテム用）
		/// </summary>
		/// <param name="iCartIndex">カートindex</param>
		/// <param name="iItemIndex">カートアイテムindex</param>
		/// <returns>エラーメッセージ</returns>
		public string Get(int iCartIndex, int iItemIndex)
		{
			return StringUtility.ToEmpty(m_htErrorMessages[iCartIndex + " " + iItemIndex]);
		}
		/// <summary>
		/// エラーメッセージ取得（セットプロモーションアイテム用）
		/// </summary>
		/// <param name="cartIndex">カートindex</param>
		/// <param name="setPromotionIndex">セットプロモーションindex</param>
		/// <param name="setPromotionItemIndex">セットプロモーションアイテムindex</param>
		/// <returns>エラーメッセージ</returns>
		public string Get(int cartIndex, int setPromotionIndex, int setPromotionItemIndex)
		{
			return StringUtility.ToEmpty(m_htErrorMessages[cartIndex + " " + setPromotionIndex + " " + setPromotionItemIndex]);
		}

		/// <summary>
		/// エラーメッセージが存在するか
		/// </summary>
		/// <param name="iCartIndex">カートindex</param>
		/// <param name="ecErrorKbn">エラー区分</param>
		/// <returns>エラーメッセージが存在するか</returns>
		public bool HasMessages(int iCartIndex, ErrorKbn ecErrorKbn)
		{
			return HasMessages(iCartIndex, (int)ecErrorKbn);
		}
		/// <summary>
		/// エラーメッセージが存在するか
		/// </summary>
		/// <param name="iCartIndex">カートindex</param>
		/// <param name="iItemIndex">カートアイテムindex</param>
		/// <returns>エラーメッセージが存在するか</returns>
		public bool HasMessages(int iCartIndex, int iItemIndex)
		{
			return m_htErrorMessages.Contains(iCartIndex + " " + iItemIndex);
		}
		/// <summary>
		/// エラーメッセージが存在するか
		/// </summary>
		/// <param name="cartIndex">カートindex</param>
		/// <param name="setPromotionIndex">セットプロモーションindex</param>
		/// <param name="setPromotionItemIndex">セットプロモーションアイテムindex</param>
		/// <returns>エラーメッセージが存在するか</returns>
		public bool HasMessages(int cartIndex, int setPromotionIndex, int setPromotionItemIndex)
		{
			return m_htErrorMessages.Contains(cartIndex + " " + setPromotionIndex + " " + setPromotionItemIndex);
		}

		/// <summary>エラーメッセージ数</summary>
		public int Count
		{
			get { return m_htErrorMessages.Keys.Count; }
		}
	}
	/// <summary>
	/// ポイント戻し処理できるかどうか
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>ポイント戻しできるか</returns>
	protected static bool CheckCanRevokeGrantedUserPoint(OrderModel order, SqlAccessor accessor)
	{
		var orderAddedPoint = order.OrderPointAdd;
		if ((Constants.W2MP_POINT_OPTION_ENABLED == false) || (orderAddedPoint == 0))
		{
			return true;
		}

		// 付与ポイント情報取得
		var pointService = new PointService();
		var userPoint = pointService.GetUserPoint(order.UserId, string.Empty, accessor);
		var userPointHistory = pointService.GetUserPointHistoryByOrderId(order.UserId, order.OrderId, accessor);

		// 仮ポイントの場合
		if (userPoint.Any(x => (x.OrderId == order.OrderId) && x.IsPointTypeTemp))
		{
			return true;
		}

		// 利用ポイント復元情報取得
		bool isBeforeMigration;
		var usedUserPointHistory = pointService.GetUserPointHistoriesForRestore(
			order.UserId,
			order.OrderId,
			"",
			out isBeforeMigration,
			accessor);

		var errorMessage = (CheckCanRevokePoint(userPointHistory, usedUserPointHistory, userPoint));
		errorMessage += (CheckCanRevokeLimitedTermPoint(userPointHistory, usedUserPointHistory, userPoint));

		return string.IsNullOrEmpty(errorMessage);
	}

	/// <summary>
	/// 本ポイントを戻せるか
	/// </summary>
	/// <param name="userPointHistory">ユーザーポイント履歴</param>
	/// <param name="usedUserPointHistory">利用ポイント履歴</param>
	/// <param name="userPoint">ユーザーポイント情報</param>
	/// <returns>エラーメッセージ</returns>
	private static string CheckCanRevokePoint(
		UserPointHistoryModel[] userPointHistory,
		UserPointHistoryModel[] usedUserPointHistory,
		UserPointModel[] userPoint)
	{
		var errorMessage = string.Empty;
		var sumBaseUserPoint = 0m;
		var actionExecutedDatetime = DateTime.Now;

		// 本ポイント
		var baseUserPointHistory =
			userPointHistory.Where(x => x.IsBasePoint && x.IsPointTypeComp && x.IsAddedPoint).ToArray();
		if (baseUserPointHistory.Any())
		{
			sumBaseUserPoint += usedUserPointHistory
				.Where(x => x.IsBasePoint && x.IsPointTypeComp && (actionExecutedDatetime <= x.UserPointExp))
				.Sum(x => (x.PointInc * -1));
			sumBaseUserPoint += userPoint.Where(x => x.IsBasePoint && x.IsPointTypeComp).Sum(x => x.Point);
		}

		// 本ポイントを戻せるか
		var addedBasePoint = baseUserPointHistory.Sum(x => x.PointInc);
		if (sumBaseUserPoint < addedBasePoint)
		{
			errorMessage = string.Format(
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_CANCEL_ADD_POINT_ERROR),
				sumBaseUserPoint,
				(addedBasePoint * -1),
				(sumBaseUserPoint - addedBasePoint)).Replace(
				"@@ point_kbn_name @@",
				ValueText.GetValueText(
					Constants.TABLE_USERPOINT,
					Constants.FIELD_USERPOINT_POINT_KBN,
					Constants.FLG_USERPOINT_POINT_KBN_BASE));
		}
		return errorMessage;
	}

	/// <summary>
	/// 期間限定ポイントを戻せるか
	/// </summary>
	/// <param name="userPointHistory">ユーザーポイント履歴</param>
	/// <param name="usedUserPointHistory">利用ポイント履歴</param>
	/// <param name="userPoint">ユーザーポイント情報</param>
	/// <returns>エラーメッセージ</returns>
	private static string CheckCanRevokeLimitedTermPoint(
		UserPointHistoryModel[] userPointHistory,
		UserPointHistoryModel[] usedUserPointHistory,
		UserPointModel[] userPoint)
	{
		var errorMessage = string.Empty;
		var sumLimitedTermPoint = 0m;
		var actionExecutedDatetime = DateTime.Now;

		// 期間限定ポイント
		var limitedTermPointHistory = userPointHistory.Where(x => x.IsLimitedTermPoint && x.IsAddedPoint).ToArray();
		if (limitedTermPointHistory.Any())
		{
			sumLimitedTermPoint += usedUserPointHistory
				.Where(x => x.IsLimitedTermPoint && (actionExecutedDatetime <= x.UserPointExp))
				.Sum(x => (x.PointInc * -1));
			sumLimitedTermPoint += userPoint.Where(x => x.IsLimitedTermPoint).Sum(x => x.Point);
		}

		// 期間限定ポイントを戻せるか
		var addedLimitedTermPoint = limitedTermPointHistory.Sum(x => x.PointInc);
		if (sumLimitedTermPoint < addedLimitedTermPoint)
		{
			errorMessage = string.Format(
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_CANCEL_ADD_POINT_ERROR),
				sumLimitedTermPoint,
				(addedLimitedTermPoint * -1),
				(sumLimitedTermPoint - addedLimitedTermPoint)).Replace(
				"@@ point_kbn_name @@",
				ValueText.GetValueText(
					Constants.TABLE_USERPOINT,
					Constants.FIELD_USERPOINT_POINT_KBN,
					Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT));
		}

		return errorMessage;
	}

	/// <summary>
	/// 頒布会表示名取得
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>頒布会表示名</returns>
	protected string GetSubscriptionBoxDisplayName(string subscriptionBoxCourseId)
	{
		var result = this.Process.GetSubscriptionBoxDisplayName(subscriptionBoxCourseId);
		return result;
	}

	/// <summary>
	/// 配送予定日取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>配送予定日</returns>
	protected string GetScheduledShippingDate(string orderId)
	{
		var scheduledShippingDateText = string.Join("\n", new OrderService().Get(orderId).Shippings.Select(data =>
		{
			if (data.ScheduledShippingDate == null)
			{
				return ReplaceTag("@@DispText.shipping_date_list.none@@");
			}
			else
			{
				return (data != null)
					? DateTimeUtility.ToStringFromRegion(data.ScheduledShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay2Letter)
					: string.Empty;
			}
		}));

		return scheduledShippingDateText;
	}

	/// <summary>
	/// 注文IDに紐づく配送日付を取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>配送日付</returns>
	protected string GetShippingDate(string orderId)
	{
		var shippingDateText = string.Join("\n", new OrderService().Get(orderId).Shippings.Select(oim => {
			if (oim.ShippingDate == null)
			{
				return ReplaceTag("@@DispText.shipping_date_list.none@@");
			}
			else
			{
				return (oim != null)
					? DateTimeUtility.ToStringFromRegion(oim.ShippingDate, DateTimeUtility.FormatType.ShortDate2Letter)
					: string.Empty;
			}
		}));
		return shippingDateText;
	}

	/// <summary>
	/// 注文IDに紐づくOrderItemModelリストの取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文IDに紐づくOrderItemModelリスト</returns>
	protected List<OrderItemModel> GetOrderItemModels(string orderId)
	{
		// orderItemModelに画像ヘッダをセットして、複数配送先の商品を一つの配列にまとめる
		var orderModel = new OrderService().Get(orderId);
		var models = orderModel.Shippings.SelectMany(osm => osm.Items.Select(
			item =>
			{
				var dv = ProductCommon.GetProductVariationInfo(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					orderModel.MemberRankId).ToHashtableList();
				if (dv.Count == 0)
				{
					item.ProductVariationImageHead = "";
					item.ProductImageHead = "";
				}
				else
				{
					item.ProductVariationImageHead = (string)dv[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD];
					item.ProductImageHead = (string)dv[0][Constants.FIELD_PRODUCT_IMAGE_HEAD];
				}
				return item;
			}).Where(item => (item.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID)).ToArray()).ToArray();

		// ProductIdとVariationIdが同一の商品は注文個数を加算
		var modelList = new List<OrderItemModel>();
		foreach (var item in models)
		{
			if (modelList.Any(s => s.ProductId == item.ProductId && s.VariationId == item.VariationId) == false)
			{
				modelList.Add(item);
			}
			else
			{
				modelList.Where(s => s.ProductId == item.ProductId && s.VariationId == item.VariationId).Select(i => i.ItemQuantity += item.ItemQuantity).ToArray();
			}
		}

		// 翻訳情報を設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			modelList = SetOrderItemTranslationData(modelList);
		}

		return modelList;
	}

	/// <summary>
	/// 注文商品翻訳情報設定
	/// </summary>
	/// <param name="orderItems">注文商品情報リスト</param>
	/// <returns>翻訳情報設定後注文商品情報リスト</returns>
	protected List<OrderItemModel> SetOrderItemTranslationData(List<OrderItemModel> orderItems)
	{
		var orderItemsTranslationData = orderItems.Select(SetOrderItemTranslationData).ToList();
		return orderItemsTranslationData;
	}

	/// <summary>
	/// 注文商品翻訳情報設定
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <returns>翻訳情報設定後注文商品情報</returns>
	protected OrderItemModel SetOrderItemTranslationData(OrderItemModel orderItem)
	{
		orderItem.ProductName = NameTranslationCommon.GetOrderItemProductTranslationName(
			orderItem.ProductName,
			orderItem.ProductId,
			orderItem.VariationId,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);
		return orderItem;
	}
}
