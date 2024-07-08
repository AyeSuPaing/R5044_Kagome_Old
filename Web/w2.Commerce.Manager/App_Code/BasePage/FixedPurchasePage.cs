/*
=========================================================================================================
  Module      : 定期購入情報共通ページ(FixedPurchasePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Register;
using w2.App.Common.User.UpdateAddressOfUserandFixedPurchase;
using w2.Common.Web;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.User;

/// <summary>
/// FixedPurchasePage の概要の説明です
/// </summary>
public class FixedPurchasePage : OrderPage
{
	protected const string REQUEST_KEY_ACTION_KBN = "action_kbn";
	protected const string ACTION_KBN_PAYMENT = "payment";
	protected const string ACTION_KBN_SHIPPING = "shipping";
	protected const string ACTION_KBN_PATTERN = "pattern";
	protected const string ACTION_KBN_ITEM = "item";
	protected const string ACTION_KBN_ORDER = "order";
	protected const string REQUEST_KEY_REINPUT_KBN = "reinput";
	protected const string REINPUT_KBN_ON = "1";
	protected const string ACTION_KBN_INVOICE = "invoice";

	/// <summary>
	/// 定期購入詳細URL作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="isPopup">ポップアップ表示?</param>
	/// <returns>定期購入詳細URL</returns
	public static string CreateFixedPurchaseDetailUrl(string fixedPurchaseId, bool isPopup = false)
	{
		return CreateFixedPurchaseDetailBaseUrl(fixedPurchaseId, Constants.ACTION_STATUS_DETAIL, isPopup);
	}

	/// <summary>
	/// 定期購入確認URL作成
	/// </summary>
	/// <returns>定期購入確認URL</returns>
	protected string CreateFixedPurchaseConfirmUrl()
	{
		return CreateFixedPurchaseDetailBaseUrl(this.RequestFixedPurchaseId, Constants.ACTION_STATUS_CONFIRM, false)
			+ "&" + REQUEST_KEY_ACTION_KBN + "=" + HttpUtility.UrlEncode(this.RequestActionKbn);
	}

	/// <summary>
	/// 定期購入完了URL作成
	/// </summary>
	/// <returns>定期購入完了URL</returns
	protected string CreateFixedPurchaseCompleteUrl()
	{
		return CreateFixedPurchaseDetailBaseUrl(this.RequestFixedPurchaseId, Constants.ACTION_STATUS_COMPLETE, false);
	}

	/// <summary>
	/// 定期購入詳細URL作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="isPopup">ポップアップ表示?</param>
	/// <returns>定期購入詳細URL</returns>
	public static string CreateFixedPurchaseDetailBaseUrl(string fixedPurchaseId, string actionStatus, bool isPopup = false)
	{
		return Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM
			+ "?" + Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID + "=" + HttpUtility.UrlEncode(fixedPurchaseId)
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + HttpUtility.UrlEncode(actionStatus)
			+ (isPopup ? "&" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP) : "");
	}

	/// <summary>
	/// 頒布会詳細ポップURL作成
	/// </summary>
	/// <param name="courseId">頒布会ID</param>
	/// <returns>頒布会詳細URL</returns>
	public static string CreateSubscriptionBoxRegisterUrl(string courseId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_REGISTER)
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, courseId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, "update")
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// 定期購入ステータス表示用CSS-CLASS取得
	/// </summary>
	/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
	/// <returns>CSS-CLASS</returns>
	public static string GetFixedPurchaseStatusCssClass(string fixedPurchaseStatus)
	{
		switch (fixedPurchaseStatus)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL:
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_COMPLETE:
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL:
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP:
				return "";

			default:
				return "notice";
		}
	}

	/// <summary>
	/// 決済ステータス表示用CSS-CLASS取得
	/// </summary>
	/// <param name="paymentStatus">決済ステータス</param>
	/// <returns>CSS-CLASS</returns>
	public static string GetPaymentStatusCssClass(string paymentStatus)
	{
		switch (paymentStatus)
		{
			case Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL:
				return "";

			default:
				return "notice";
		}
	}

	/// <summary>
	/// カート情報リストの作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <returns>カート情報リスト</returns>
	public static CartObjectList CreateSimpleCartListForFixedPurchase(string userId, string fixedPurchaseId)
	{
		var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
		var user = new UserService().Get(userId);
		if (user == null) return null;

		var orderRegister = new OrderRegisterFixedPurchaseInner(
			user.IsMember,
			Constants.FLG_LASTCHANGED_USER,
			false,
			fixedPurchaseId,
			null);
		var cartObject = new OrderRegisterFixedPurchase(Constants.FLG_LASTCHANGED_USER, false, false, null)
			.CreateCartList(fixedPurchase, user, orderRegister);
		return cartObject;
	}

	#region プロパティ
	/// <summary>リクエスト：定期購入ID</summary>
	protected string RequestFixedPurchaseId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID]).Trim(); }
	}
	/// <summary>リクエスト：アクション区分</summary>
	protected string RequestActionKbn
	{
		get { return StringUtility.ToEmpty(Request[REQUEST_KEY_ACTION_KBN]); }
	}
	/// <summary>リクエスト：アクション区分</summary>
	protected bool IsReinput
	{
		get { return (Request[REQUEST_KEY_REINPUT_KBN] == REINPUT_KBN_ON); }
	}
	/// <summary>リクエスト：ウィンドウ区分</summary>
	protected string RequestWindowKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_WINDOW_KBN]); }
	}
	/// <summary>定期購入情報（表示用）</summary>
	protected FixedPurchaseContainer FixedPurchaseContainer
	{
		get { return (FixedPurchaseContainer)ViewState["FixedPurchaseContainer"]; }
		set { ViewState["FixedPurchaseContainer"] = value; }
	}
	/// <summary>配送種別情報</summary>
	protected ShopShippingModel ShopShipping
	{
		get { return (ShopShippingModel)ViewState["ShopShipping"]; }
		set { ViewState["ShopShipping"] = value; }
	}
	/// <summary>配送会社情報</summary>
	protected DeliveryCompanyModel DeliveryCompany
	{
		get { return (DeliveryCompanyModel)ViewState["deliveryCompany"]; }
		set { ViewState["deliveryCompany"] = value; }
	}
	/// <summary>定期購入情報（編集用）</summary>
	protected FixedPurchaseInput ModifyInfo
	{
		get { return (Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASE_MODIFY_INFO] != null) ? (FixedPurchaseInput)Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASE_MODIFY_INFO] : new FixedPurchaseInput(this.FixedPurchaseContainer); }
		set { Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASE_MODIFY_INFO] = value; }
	}
	/// <summary>定期購入一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get
		{
			return (Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASE_SEARCH_INFO] != null)
				? (SearchValues)Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASE_SEARCH_INFO]
				: new SearchValues(
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					1,
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"");
		}
		set { Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASE_SEARCH_INFO] = value; }
	}
	/// <summary>今すぐ注文時のエラーメッセージ</summary>
	protected string ErrorMessages
	{
		get { return StringUtility.ToEmpty(Session["ErrorMessages"]); }
		set { Session["ErrorMessages"] = value; }
	}
	/// <summary>配送先情報反映対象</summary>
	protected string[] AddressUpdatePattern
	{
		get { return (string[])Session[Constants.SESSION_KEY_ADDRESSUPDATE_PATTERN]; }
		set { Session[Constants.SESSION_KEY_ADDRESSUPDATE_PATTERN] = value; }
	}
	/// <summary>更新する対象のリスト</summary>
	protected IEnumerable<IUpdated> DoUpdateTargets
	{
		get { return (IEnumerable<IUpdated>)Session[Constants.SESSION_KEY_ADDRESSUPDATE_DO_UPDATE_TARGETS]; }
		set { Session[Constants.SESSION_KEY_ADDRESSUPDATE_DO_UPDATE_TARGETS] = value; }
	}
	/// <summary>更新した対象のリスト</summary>
	protected IEnumerable<IUpdated> UpdatedTargets
	{
		get { return (IEnumerable<IUpdated>)Session[Constants.SESSION_KEY_ADDRESSUPDATE_UPDATE_TARGETS]; }
		set { Session[Constants.SESSION_KEY_ADDRESSUPDATE_UPDATE_TARGETS] = value; }
	}
	/// <summary>Taiwan FixedPurchase Invoice</summary>
	protected TwFixedPurchaseInvoiceInput TwModifyInfo
	{
		get
		{
			return Session[Constants.SESSIONPARAM_KEY_TWFIXEDPURCHASEINVOICE_MODIFY_INFO] != null
				? (TwFixedPurchaseInvoiceInput)Session[Constants.SESSIONPARAM_KEY_TWFIXEDPURCHASEINVOICE_MODIFY_INFO]
				: null;
		}
		set { Session[Constants.SESSIONPARAM_KEY_TWFIXEDPURCHASEINVOICE_MODIFY_INFO] = value; }
	}
	/// <summary>定期商品が存在するか</summary>
	protected bool IsItemsExist
	{
		get { return (this.FixedPurchaseContainer.Shippings[0].Items.Length > 0); }
	}
	/// <summary>Is not payment at convenience store EcPay</summary>
	protected bool IsNotPaymentAtConvenienceStoreEcPay
	{
		get
		{
			var result = ((Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
				|| (this.FixedPurchaseContainer.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
				|| (ECPayUtility.GetIsCollection(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType)
					== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF));
			return result;
		}
	}
	#endregion

	#region +検索値格納クラス
	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="userName">注文者名</param>
		/// <param name="orderCountFrom">購入回数(注文基準)（開始）</param>
		/// <param name="orderCountTo">購入回数(注文基準)（終了）</param>
		/// <param name="shippedCountFrom">購入回数(出荷基準)（開始）</param>
		/// <param name="shippedCountTo">購入回数(出荷基準)（終了）</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="orderPaymentKbn">決済種別</param>
		/// <param name="paymentStatus">決済ステータス</param>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="shipping">配送先</param>
		/// <param name="productId">商品ID</param>
		/// <param name="managementMemoFlg">管理メモフラグ（ブランク or あり or なし）</param>
		/// <param name="managementMemo">管理メモ</param>
		/// <param name="shippingMemoFlg">配送メモフラグ（ブランク or あり or なし）</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="dateType">日付</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="extendStatus">拡張ステータス</param>
		/// <param name="sortKbn">並び順</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <param name="pageNum">ページ番号</param>
		/// <param name="updateDateExtendStatus">拡張ステータス更新日・ステータス</param>
		/// <param name="orderExtendName">注文拡張項目 項目名</param>
		/// <param name="orderExtendFlg">注文拡張項目 ありなし</param>
		/// <param name="orderExtendType">注文拡張項目 入力方式</param>
		/// <param name="orderExtendLikeEscaped">注文拡張項目 入力内容</param>
		/// <param name="updateExtendStatusDateFrom">Update extend status date from</param>
		/// <param name="updateExtendStatusDateTo">Update extend status date to</param>
		/// <param name="updateExtendStatusTimeFrom">Update extend status time from</param>
		/// <param name="updateExtendStatusTimeTo">Update extend status time to</param>
		/// <param name="dateFrom">Date from</param>
		/// <param name="dateTo">Date to</param>
		/// <param name="timeFrom">Time from</param>
		/// <param name="timeTo">Time to</param>
		/// <param name="receiptFlg">領収書希望フラグ</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="isSubscriptionBox">頒布会か</param>
		/// <param name="subscriptionBoxOrderCountFrom">頒布会購入回数FROM</param>
		/// <param name="subscriptionBoxOrderCountTo">頒布会購入回数TO</param>
		public SearchValues(string fixedPurchaseId,
			string fixedPurchaseStatus,
			string userId,
			string userName,
			string orderCountFrom,
			string orderCountTo,
			string shippedCountFrom,
			string shippedCountTo,
			string orderKbn,
			string orderPaymentKbn,
			string paymentStatus,
			string fixedPurchaseKbn,
			string shipping,
			string productId,
			string managementMemoFlg,
			string managementMemo,
			string shippingMemoFlg,
			string shippingMemo,
			string dateType,
			string extendStatusNo,
			string extendStatus,
			string sortKbn,
			string shippingMethod,
			int pageNum,
			string updateDateExtendStatus,
			string orderExtendName,
			string orderExtendFlg,
			string orderExtendType,
			string orderExtendLikeEscaped,
			string updateExtendStatusDateFrom,
			string updateExtendStatusDateTo,
			string updateExtendStatusTimeFrom,
			string updateExtendStatusTimeTo,
			string dateFrom,
			string dateTo,
			string timeFrom,
			string timeTo,
			string subscriptionBoxCourseId,
			string isSubscriptionBox,
			string subscriptionBoxOrderCountFrom,
			string subscriptionBoxOrderCountTo,
			string receiptFlg = "")
		{
			this.FixedPurchaseId = fixedPurchaseId;
			this.FixedPurchaseStatus = fixedPurchaseStatus;
			this.UserId = userId;
			this.UserName = userName;
			this.OrderCountFrom = orderCountFrom;
			this.OrderCountTo = orderCountTo;
			this.ShippedCountFrom = shippedCountFrom;
			this.ShippedCountTo = shippedCountTo;
			this.OrderKbn = orderKbn;
			this.OrderPaymentKbn = orderPaymentKbn;
			this.PaymentStatus = paymentStatus;
			this.FixedPurchaseKbn = fixedPurchaseKbn;
			this.Shipping = shipping;
			this.ProductId = productId;
			this.ManagementMemoFlg = managementMemoFlg;
			this.ManagementMemo = managementMemo;
			this.ShippingMemoFlg = shippingMemoFlg;
			this.ShippingMemo = shippingMemo;
			this.DateType = dateType;
			this.ExtendStatusNo = extendStatusNo;
			this.ExtendStatus = extendStatus;
			this.SortKbn = sortKbn;
			this.ShippingMethod = shippingMethod;
			this.PageNum = pageNum;
			this.UpdateDateExtendStatus = updateDateExtendStatus;
			this.UpdateExtendStatusDateFrom = updateExtendStatusDateFrom;
			this.UpdateExtendStatusDateTo = updateExtendStatusDateTo;
			this.UpdateExtendStatusTimeFrom = updateExtendStatusTimeFrom;
			this.UpdateExtendStatusTimeTo = updateExtendStatusTimeTo;
			this.DateFrom = dateFrom;
			this.DateTo = dateTo;
			this.TimeFrom = timeFrom;
			this.TimeTo = timeTo;
			this.ReceiptFlg = receiptFlg;
			this.OrderExtendName = orderExtendName;
			this.OrderExtendFlg = orderExtendFlg;
			this.OrderExtendType = orderExtendType;
			this.OrderExtendLikeExcaped = orderExtendLikeEscaped;
			this.SubscriptionBoxCourseId = subscriptionBoxCourseId;
			this.IsSubscriptionBox = isSubscriptionBox;
			this.SubscriptionBoxOrderCountFrom = subscriptionBoxOrderCountFrom;
			this.SubscriptionBoxOrderCountTo = subscriptionBoxOrderCountTo;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 定期購入一覧URL作成
		/// </summary>
		/// <param name="addPageNo">ページを付けるかどうかフラグ（ページャ作成の際はfalse）</param>
		/// <returns>定期購入一覧URL</returns>
		public string CreateFixedPurchaseListUrl(bool addPageNo = true)
		{
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_LIST)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID, this.FixedPurchaseId)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, this.FixedPurchaseStatus)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_USER_ID, this.UserId)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_USER_NAME, this.UserName)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_COUNT_FROM, this.OrderCountFrom)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_COUNT_TO, this.OrderCountTo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPED_COUNT_FROM, this.ShippedCountFrom)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPED_COUNT_TO, this.ShippedCountTo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_KBN, this.OrderKbn)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_PAYMENT_KBN, this.OrderPaymentKbn)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_PAYMENT_STATUS, this.PaymentStatus)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_KBN, this.FixedPurchaseKbn)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING, this.Shipping)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_PRODUCT_SEARCH_VALUE, this.ProductId)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_MANAGEMENT_MEMO_FLG, this.ManagementMemoFlg)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_MANAGEMENT_MEMO, this.ManagementMemo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING_MEMO_FLG, this.ShippingMemoFlg)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING_MEMO, this.ShippingMemo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_TYPE, this.DateType)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_NO, this.ExtendStatusNo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS, this.ExtendStatus)
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, this.SortKbn)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING_METHOD, this.ShippingMethod)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_NO_UPDATE_DATE, this.UpdateDateExtendStatus)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_DATE_FROM, this.UpdateExtendStatusDateFrom)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_DATE_TO, this.UpdateExtendStatusDateTo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_TIME_FROM, this.UpdateExtendStatusTimeFrom)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_TIME_TO, this.UpdateExtendStatusTimeTo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_FROM, this.DateFrom)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_TO, this.DateTo)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_FROM, this.TimeFrom)
				.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_TO, this.TimeTo);

			// 領収書希望フラグ
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_RECEIPT_FLG, this.ReceiptFlg);
			}

			if (Constants.ORDER_EXTEND_OPTION_ENABLED)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_NAME, this.OrderExtendName);
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_FLG, this.OrderExtendFlg);
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_TYPE, this.OrderExtendType);
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_TEXT, this.OrderExtendLikeExcaped);
			}

			if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, this.SubscriptionBoxCourseId);
				urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX, this.IsSubscriptionBox);
				urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM, this.SubscriptionBoxOrderCountFrom);
				urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO, this.SubscriptionBoxOrderCountTo);
			}

			if (addPageNo)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, this.PageNum.ToString());
			}

			var url = urlCreator.CreateUrl();
			return url;
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>定期購入ステータス</summary>
		public string FixedPurchaseStatus { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>注文者名</summary>
		public string UserName { get; set; }
		/// <summary>購入回数(注文基準)（開始）</summary>
		public string OrderCountFrom { get; set; }
		/// <summary>購入回数(注文基準)（終了）</summary>
		public string OrderCountTo { get; set; }
		/// <summary>購入回数(出荷基準)（開始）</summary>
		public string ShippedCountFrom { get; set; }
		/// <summary>購入回数(出荷基準)（終了）</summary>
		public string ShippedCountTo { get; set; }
		/// <summary>注文区分</summary>
		public string OrderKbn { get; set; }
		/// <summary>決済種別</summary>
		public string OrderPaymentKbn { get; set; }
		/// <summary>決済ステータス</summary>
		public string PaymentStatus { get; set; }
		/// <summary>定期購入区分</summary>
		public string FixedPurchaseKbn { get; set; }
		/// <summary>配送先</summary>
		public string Shipping { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>管理メモフラグ（ブランク or あり or なし）</summary>
		public string ManagementMemoFlg { get; set; }
		/// <summary>管理メモ</summary>
		public string ManagementMemo { get; set; }
		/// <summary>配送メモフラグ（ブランク or あり or なし）</summary>
		public string ShippingMemoFlg { get; set; }
		/// <summary>配送メモ</summary>
		public string ShippingMemo { get; set; }
		/// <summary>日付</summary>
		public string DateType { get; set; }
		/// <summary>拡張ステータスNo</summary>
		public string ExtendStatusNo { get; set; }
		/// <summary>拡張ステータス</summary>
		public string ExtendStatus { get; set; }
		/// <summary>拡張ステータス更新日・ステータス</summary>
		public string UpdateDateExtendStatus { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
		/// <summary>配送方法</summary>
		public string ShippingMethod { get; set; }
		/// <summary>領収書希望フラグ</summary>
		public string ReceiptFlg { get; set; }
		/// <summary></summary>
		public string OrderExtendName { get; set; }
		/// <summary></summary>
		public string OrderExtendFlg { get; set; }
		/// <summary></summary>
		public string OrderExtendType { get; set; }
		/// <summary></summary>
		public string OrderExtendLikeExcaped { get; set; }
		/// <summary>Update extend status date from</summary>
		public string UpdateExtendStatusDateFrom { get; set; }
		/// <summary>Update extend status date to</summary>
		public string UpdateExtendStatusDateTo { get; set; }
		/// <summary>Update extend status time from</summary>
		public string UpdateExtendStatusTimeFrom { get; set; }
		/// <summary>Update extend status time to</summary>
		public string UpdateExtendStatusTimeTo { get; set; }
		/// <summary>Date from</summary>
		public string DateFrom { get; set; }
		/// <summary>Date to</summary>
		public string DateTo { get; set; }
		/// <summary>Time from</summary>
		public string TimeFrom { get; set; }
		/// <summary>Time to</summary>
		public string TimeTo { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>頒布会か</summary>
		public string IsSubscriptionBox { get; set; }
		/// <summary>頒布会購入回数FROM</summary>
		public string SubscriptionBoxOrderCountFrom { get; set; }
		/// <summary>頒布会購入回数To</summary>
		public string SubscriptionBoxOrderCountTo { get; set; }
		#endregion
	}
	#endregion
}