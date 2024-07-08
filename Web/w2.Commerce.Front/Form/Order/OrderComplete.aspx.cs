/*
=========================================================================================================
  Module      : 注文完了画面処理(OrderComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.RealShop;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Domain;
using w2.Domain.NameTranslationSetting;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.RealShop;
using w2.Domain.TwOrderInvoice;
using w2.Domain.User;

public partial class Form_Order_OrderComplete : OrderPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	
	#region ラップ済みコントロール宣言
	WrappedRepeater WrErrorMesseges { get { return GetWrappedControl<WrappedRepeater>("rErrorMesseges"); } }
	WrappedLinkButton WlbRetryOrder { get { return GetWrappedControl<WrappedLinkButton>("lbRetryOrder"); } }
	WrappedRepeater WrOrderHistory { get { return GetWrappedControl<WrappedRepeater>("rOrderHistory"); } }
	WrappedRepeater WrGoogleAnalytics { get { return GetWrappedControl<WrappedRepeater>("rGoogleAnalytics"); } }
	WrappedControl WpfDocomoPayment { get { return GetWrappedControl<WrappedControl>("pfDocomoPayment"); } }
	WrappedTextBox WtbMobileMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbMobileMailAddr"); } }
	WrappedControl WcCriteo { get { return GetWrappedControl<WrappedControl>("criteo"); } }
	WrappedHtmlGenericControl WhGmoInreviewContent { get { return GetWrappedControl<WrappedHtmlGenericControl>("hGmoInreviewContent"); } }
	#endregion

	//protected List<KeyValuePair<bool, ValueObjectFixedPurchase>> m_lfx = new List<KeyValuePair<bool, ValueObjectFixedPurchase>>();
	//List<ValueObjectFixedPurchase> m_lFixedPurchaseObjects = new List<ValueObjectFixedPurchase>();

	protected List<bool> m_lFixedPurchaseFlgs = new List<bool>();							// 各カートの定期購入判別フラグ
	protected List<string> m_lFixedPurchaseFirstPatternStrings = new List<string>();		// 各カートの定期購入初回配送パターン文字列
	protected List<string> m_lFixedPurchasePatternStrings = new List<string>();				// 各カートの定期購入2回目以降の配送パターン文字列
	protected List<DateTime> m_lFixedPurchaseFirstShippingDates = new List<DateTime>();		// 各カートの定期購入初回配送日
	protected List<DateTime> m_lFixedPurchaseNextShippingDates = new List<DateTime>();		// 各カートの定期購入次回配送日
	protected List<DateTime> m_lFixedPurchaseNextNextShippingDates = new List<DateTime>();	// 各カートの定期購入次々回配送日
	protected List<bool> m_lShippingTimeSetFlgs = new List<bool>();			// 各カートの配送希望時間帯設定可否フラグ
	protected List<string> m_lShippingTimeMessages = new List<string>();	// 各カートの配送希望時間帯表示文字列

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		SessionManager.IsRedirectFromLinePay = false;
		this.DispNum = 0;

		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);

			this.OrderInfos = new List<Hashtable> { new Hashtable { { 0, "" } } };

			m_lFixedPurchaseFlgs.Add(false);
			m_lFixedPurchaseFirstPatternStrings.Add("");
			m_lFixedPurchasePatternStrings.Add("");
			m_lFixedPurchaseFirstShippingDates.Add(DateTime.Now);
			m_lFixedPurchaseNextShippingDates.Add(DateTime.Now);
			m_lFixedPurchaseNextNextShippingDates.Add(DateTime.Now);
			m_lShippingTimeSetFlgs.Add(false);
			m_lShippingTimeMessages.Add("");

			var orderData = Preview.GetDummyOrder(this.ShopId);
			this.OrderItemSerialKeys = new Dictionary<string, DataView> { { orderData[0][Constants.FIELD_ORDER_ORDER_ID] + "0", orderData } };
			this.OrderList = new List<DataView> { orderData };
			this.WrOrderHistory.DataSource = this.OrderList;
			this.WrOrderHistory.DataBind();

			return;
		}

		if (!IsPostBack)
		{
			// Remove session use for display fixed purchase future order from LP
			if (Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING] != null)
			{
				Session.Remove(Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING);
			}

			// LPカートがある場合は復元
			string landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
			if (landingCartSessionKey != null)
			{
				this.CartList = (CartObjectList)Session[landingCartSessionKey];

				// 新LPカートリストページを使用している場合、カートセッションを同期
				if (Constants.CART_LIST_LP_OPTION
					&& landingCartSessionKey.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME))
				{
					SessionManager.CartList = this.CartList;
					SessionManager.CartListLp = this.CartList;
				}
			}

			// 復元が終わったらLPカートのキーを削除
			Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] = null;

			// Delete selection store pickup information
			Session[Constants.SESSION_KEY_REALSHOP_SELECTION_INFO] = null;

			if (Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED)
			{
				SessionManager.IsMovedOnOrderConfirm = false;
			}

			//------------------------------------------------------
			// 画面遷移の正当性チェック
			//------------------------------------------------------
			if (this.OrderInfos == null)
			{
				CheckOrderUrlSession();
			}

			if (this.AliveSessionParameter)
			{
				//------------------------------------------------------
				// パラメタ取得
				//------------------------------------------------------
				Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				if (this.OrderInfos == null)	// デバッグ用
				{
					this.OrderInfos = (List<Hashtable>)htParam["order"];									// 注文情報
				}
				this.DocomoPaymentOrder = (Dictionary<string, CartObject>)htParam["order_docomo"];	// ドコモケータイ払い用のパラメータ

				if (htParam.ContainsKey(DATA_ECPAY))
				{
					var request = (ECPayRequest)htParam[DATA_ECPAY];
					htParam.Remove(DATA_ECPAY);
					var form = ECPayApiFacade.CreateFormPayment(request);

					// Go To Form EcPay
					Response.Clear();
					Response.Write(form);
					Response.End();
				}

				if (htParam.ContainsKey(DATA_NEWEBPAY))
				{
					var request = (NewebPayRequest)htParam[DATA_NEWEBPAY];
					htParam.Remove(DATA_NEWEBPAY);
					var form = NewebPayApiFacade.CreateFormPayment(request);

					// Go To Form NewebPay
					Response.Clear();
					Response.Write(form);
					Response.End();
				}

				//------------------------------------------------------
				// 注文情報取得・データバインド
				//------------------------------------------------------
				this.OrderList = new List<DataView>();

				foreach (var htOrder in this.OrderInfos)
				{
					// Check order id returned from EcPay
					var orderId = StringUtility.ToEmpty(htOrder[Constants.FIELD_ORDER_ORDER_ID]);
					var orderIdRequest = StringUtility.ToEmpty(Request[Constants.FIELD_ORDER_ORDER_ID]);
					if ((string.IsNullOrEmpty(orderIdRequest) == false)
						&& (orderIdRequest != orderId))
					{
						Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = null;
						CheckOrderUrlSession();
					}
					var order = new OrderService().GetOrderHistoryDetailInDataView(
						orderId,
						(string)htOrder[Constants.FIELD_ORDER_USER_ID],
						this.MemberRankId,
						Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent);
					this.OrderList.Add(order);

					// 定期2回目商品切替を利用している場合、配送パターンが設定変更されている可能性がある為取得する
					if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
					{
						m_lFixedPurchaseFirstPatternStrings.Add(StringUtility.ToEmpty(htOrder[Constants.FIXED_PURCHASE_FISRT_PATTERN_STRINGS]));
					}
				}

				// 翻訳名称設定
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					this.OrderList = this.OrderList.Select(SetTranslationData).ToList();
				}

				this.IsDskDeferredAuthResultHold = false;
				var isExistedCvsDefHoldOrder = this.OrderList.Any(
					order => ((string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& ((string)order[0][Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST));

				if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
				{
					this.IsDskDeferredAuthResultHold = isExistedCvsDefHoldOrder;
				}
				else if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
				{
					this.IsScoreDeferredAuthResultHold = isExistedCvsDefHoldOrder;
				}
				else if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
				{
					this.IsVeritransDeferredAuthResultHold = isExistedCvsDefHoldOrder;
				}

				// GMOアトカラ審査中
				this.IsGmoAtokaraAuthResultHold = this.OrderList.Any(
					order => ((string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
						&& ((string)order[0][Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST));

				//------------------------------------------------------
				// 画面全体データバインド
				//------------------------------------------------------
				this.DataBind();

				// 注文情報取得後＆データバインド前なので、このタイミングで読み込む
				this.OrderItemSerialKeys = new Dictionary<string, DataView>();
				foreach (DataView dv in this.OrderList)
				{
					m_lFixedPurchaseFlgs.Add((string)dv[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID] != "");

					// 定期購入商品かどうかで分岐
					bool blFP = m_lFixedPurchaseFlgs[m_lFixedPurchaseFlgs.Count - 1];
					m_lFixedPurchasePatternStrings.Add(blFP ?
						OrderCommon.CreateFixedPurchaseSettingMessage((string)dv[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN], (string)dv[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]) : "");
					DateTime? dtShippingDate = Validator.IsNullEmpty(dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]) ? null : (DateTime?)dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE];

					// 初回配送予定日
					var shippingMethod = dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD].ToString();
					var orderInfo = this.OrderInfos.FirstOrDefault(order => (string)order[Constants.FIELD_ORDER_ORDER_ID] == (string)dv[0][Constants.FIELD_ORDER_ORDER_ID]);
					if (shippingMethod != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL
						&& string.IsNullOrEmpty((string)dv[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]) == false
						&& orderInfo != null
						&& orderInfo[Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE] != null)
					{
						m_lFixedPurchaseFirstShippingDates.Add((DateTime)orderInfo[Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE]);
					}
					else
					{
						var firstShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
							this.ShopId,
							(int)dv[0][Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED],
							dtShippingDate,
							shippingMethod,
							dv[0][Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID].ToString(),
							dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE].ToString(),
							(Constants.TW_COUNTRY_SHIPPING_ENABLE
								&& IsCountryTw(dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE].ToString()))
								? dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2].ToString()
								: dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1].ToString(),
							dv[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP].ToString());

						m_lFixedPurchaseFirstShippingDates.Add(blFP ? firstShippingDate : new DateTime()); // null格納不可なので new Datetime()
					}
					m_lFixedPurchaseNextShippingDates.Add(blFP ? (DateTime)dv[0][Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE] : new DateTime()); // null格納不可なので new Datetime()
					m_lFixedPurchaseNextNextShippingDates.Add(blFP ? (DateTime)dv[0][Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE] : new DateTime()); // null格納不可なので new Datetime()
					m_lShippingTimeSetFlgs.Add((string)dv[0][Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG] == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID);
					m_lShippingTimeMessages.Add(
						Validator.IsNullEmpty(dv[0]["shipping_time_message"])
							? ReplaceTag("@@DispText.shipping_time_list.none@@")
							: (string)dv[0]["shipping_time_message"]);

					// 「購入商品毎 ｘ 数量」に応じたシリアルキーを取得
					foreach (DataRowView drv in dv)
					{
						this.OrderItemSerialKeys.Add((string)drv[Constants.FIELD_ORDER_ORDER_ID] + drv[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO].ToString(), OrderCommon.GetSerialKeyList(drv));
					}
				}
				this.WrOrderHistory.DataSource = this.OrderList;
				this.WrOrderHistory.DataBind();
				this.WrGoogleAnalytics.DataSource = (List<Hashtable>)htParam["googleanaytics_params"];
				this.WrGoogleAnalytics.DataBind();

				// 定期会員フラグセッションを更新する
				if (this.OrderInfos != null)
				{
					var userId = (string)this.OrderList[0][0][Constants.FIELD_ORDER_USER_ID];
					var user = new UserService().Get(userId);
					this.LoginUserFixedPurchaseMemberFlg = user.FixedPurchaseMemberFlg;
				}

				//------------------------------------------------------
				// エラーメッセージ取得・表示
				//------------------------------------------------------
				// エラーメッセージ取得
				List<string> lMessages = new List<string>();
				foreach (string strMessage in (List<string>)htParam["error"])
				{
					if (lMessages.Contains(strMessage) == false)
					{
						lMessages.Add(strMessage);
					}
				}
				foreach (string strMessage in (List<string>)htParam["alert"])
				{
					if (lMessages.Contains(strMessage) == false)
					{
						lMessages.Add(strMessage);
					}
				}

				this.WrErrorMesseges.DataSource = lMessages;
				this.WrErrorMesseges.DataBind();

				this.WrErrorMesseges.Visible = (lMessages.Count != 0);

				// エラーがない場合は再注文リンクを非表示に
				WlbRetryOrder.Visible = (((List<string>)htParam["error"]).Count != 0);

				// show gmo message if result is in review or deposit waiting
				var gmoTransactionResult = (string)htParam["gmo_transaction_result"];
				if (string.IsNullOrEmpty(gmoTransactionResult) == false
					&& ((gmoTransactionResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_INREVIEW) || (gmoTransactionResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_DEPOSIT_WAITING)))
				{
					this.WhGmoInreviewContent.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_GMO_KB_PAYMENT_INREVIEW);
				}
			}

			//------------------------------------------------------
			// ログイン済み ＆ ドコモケータイ払いを行う場合、モバイルアドレスを自動入力
			//------------------------------------------------------
			if ((this.IsLoggedIn) && (this.DocomoPaymentOrder.Count != 0))
			{
				var user = new UserService().Get(this.LoginUserId);
				if (user != null)
				{
					if (Regex.IsMatch(user.MailAddr2, @"^.*@docomo.ne.jp$"))
					{
						// ドコモのアドレスの場合のみ自動入力
						WtbMobileMailAddr.Text = user.MailAddr2;
					}
				}
			}
		}

		//------------------------------------------------------
		// 表示設定
		//------------------------------------------------------
		WpfDocomoPayment.Visible = (this.DocomoPaymentOrder.Count > 0);
		if (this.IsAleadyDisplayed) SetAchievementReportUnvisible();
		this.IsAleadyDisplayed = true;

		if (this.IsAmazonCv2Guest) Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
		{
			this.CartList.AuthenticationCode = string.Empty;
			this.CartList.HasAuthenticationCode = false;
			this.AuthenticationCode = string.Empty;
		}
	}

	/// <summary>
	/// 商品セットのTOPかどうか判断
	/// </summary>
	/// <param name="objOrderItem"></param>
	/// <returns></returns>
	protected bool CheckTopSetItem(object objOrderItem)
	{
		List<DataRowView> lOrerSetItems = GetSetItemList(objOrderItem);
		if (lOrerSetItems == null)
		{
			return false;
		}

		return ((DataRowView)objOrderItem == lOrerSetItems[0]);
	}

	/// <summary>
	/// 商品セット価格小計取得
	/// </summary>
	/// <param name="objOrderItem"></param>
	/// <returns></returns>
	protected decimal GetProductSetPrice(object objOrderItem)
	{
		decimal dSetPrice = 0;
		foreach (DataRowView drvItem in GetSetItemList(objOrderItem))
		{
			dSetPrice += (decimal)drvItem[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]
						* (int)drvItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
		}

		return dSetPrice.ToPriceDecimal().Value;
	}

	/// <summary>
	/// セットアイテム一覧取得
	/// </summary>
	/// <param name="objOrderItem">注文ID</param>
	/// <returns></returns>
	protected List<DataRowView> GetSetItemList(object objOrderItem)
	{
		string strOrderId = (string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_ORDER_ID];
		string strProductSetId = (string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID];
		if (strProductSetId == "")
		{
			return null;
		}
		int iProductSetNo = (int)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO];

		List<DataRowView> lOrderItems = new List<DataRowView>();
		foreach (DataView dvOrder in this.OrderList)
		{
			if ((string)dvOrder[0][Constants.FIELD_ORDER_ORDER_ID] == strOrderId)
			{
				foreach (DataRowView drvOrderItem in dvOrder)
				{
					if (((string)drvOrderItem[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] == strProductSetId)
						&& ((int)drvOrderItem[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] == iProductSetNo))
					{
						lOrderItems.Add(drvOrderItem);
					}
				}
			}
		}
		return lOrderItems;
	}

	/// <summary>
	/// セットプロモーションアイテムリスト取得
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <returns>セットプロモーションアイテムリスト</returns>
	protected List<DataRowView> GetOrderSetPromotionItemList(DataRowView orderItem)
	{
		// セットプロモーション商品でなければnullを返す
		if (orderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == DBNull.Value) return null;

		List<DataRowView> setPromotionItems = new List<DataRowView>();
		string orderId = (string)orderItem[Constants.FIELD_ORDERITEM_ORDER_ID];
		int orderSetPromotionNo = (int)orderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO];

		// 対象注文取得
		DataView targetOrder = this.OrderList.Find(order => (string)order[0][Constants.FIELD_ORDER_ORDER_ID] == orderId);
		if (targetOrder != null)
		{
			// 対象セットプロモーション商品だったらリストに追加
			foreach (DataRowView targetOrderItem in targetOrder)
			{
				if (targetOrderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] != DBNull.Value && (int)targetOrderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == orderSetPromotionNo)
				{
					setPromotionItems.Add(targetOrderItem);
				}
			}
		}

		return setPromotionItems;
	}

	/// <summary>
	/// 注文セットプロモーション情報取得
	/// </summary>
	/// <param name="orderItems"></param>
	/// <returns></returns>
	protected List<Hashtable> GetOrderSetPromotions(DataView orderItems)
	{
		return orderItems.ToHashtableList().FindAll(orderitem =>
			(orderitem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] != DBNull.Value)
			&& ((int)orderitem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] == 1));
	}

	/// <summary>
	/// 失敗した注文をやり直すリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRetryOrder_Click(object sender, EventArgs e)
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;

		// 画面遷移
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM);
	}

	/// <summary>
	/// ドコモケータイ払いメール送信ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bSendDocomoPaymentMail_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// メールアドレス取得＆エラーチェック
		//------------------------------------------------------
		// メールアドレス取得
		StringBuilder sbMessage = new StringBuilder();
		string strMailAddr = WtbMobileMailAddr.Text.Trim() + "@docomo.ne.jp";

		// 入力チェック
		Hashtable htInput = new Hashtable();
		htInput.Add("mail", strMailAddr);
		sbMessage.Append(Validator.Validate("OrderPaymentDocomo", htInput));

		//------------------------------------------------------
		// メール送信
		//------------------------------------------------------
		// 入力値にエラーがなければ送信処理を開始
		if (sbMessage.Length == 0)
		{
			// ドコモケータイ払いを行うカートごとにメールを送信
			int iMailCount = 0;
			int iSkipCount = 0;
			foreach (string strOrderId in this.DocomoPaymentOrder.Keys)
			{
				CartObject coCartObject = (CartObject)this.DocomoPaymentOrder[strOrderId];

				// 今の注文状態が仮注文かチェック（仮注文じゃない場合はメールを送信しない）
				DataView dvOrder = OrderCommon.GetOrder(strOrderId);
				if ((dvOrder.Count == 0)
					|| (StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_ORDER_STATUS]) != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
					|| (StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]) != Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG))
				{
					iSkipCount++;
					continue;
				}

				// メールの送信処理
				if (SendMail(strMailAddr, strOrderId, coCartObject))
				{
					iMailCount++;
				}
			}

			// オールスキップ？
			if ((iSkipCount > 0) && (iMailCount == 0))
			{
				sbMessage.Append(WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_SEND_DOCOMO_PAYMENT_MAIL_SKIP));
			}
			// 成功？
			else
			{
				sbMessage.Append(WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_SEND_DOCOMO_PAYMENT_MAIL_SUCCESS));
			}
		}

		//------------------------------------------------------
		// メッセージのセット
		//------------------------------------------------------
		this.DocomoPaymentErrorMessage = sbMessage.ToString();
	}

	/// <summary>
	/// ドコモケータイ払い用メール送信処理（モバイルへの引継ぎ連絡メール）
	/// </summary>
	/// <param name="strMailAddr">送信先メールアドレス</param>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="coCart">カートオブジェクト</param>
	/// <returns>送信OK/NG</returns>
	private bool SendMail(string strMailAddr, string strOrderId, CartObject coCart)
	{
		//------------------------------------------------------
		// ドコモケータ払い、モバイル引継ぎアドレスの生成
		//------------------------------------------------------
		StringBuilder sbURI = new StringBuilder();
		if (Constants.PATH_MOBILESITE.ToLower().StartsWith("http"))
		{
			// フルパスでモバイルサイトを指定している
			sbURI.Append(Constants.PATH_MOBILESITE.Replace(this.UnsecurePageProtocolAndHost, this.SecurePageProtocolAndHost));
		}
		else
		{
			// フルパス指定ではない場合
			sbURI.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_MOBILESITE);
		}
		sbURI.Append("Form/DCMPmnt/DCMPmntConfirm.aspx?pid=R601_DCMCFM");
		sbURI.Append("&").Append("odid").Append("=").Append(Server.UrlEncode(strOrderId));

		//------------------------------------------------------
		// 購入商品の情報生成
		//------------------------------------------------------
		int iProductNo = 1;
		StringBuilder sbOrderItems = new StringBuilder();
		foreach (CartProduct cp in coCart.Items)
		{
			sbOrderItems.Append(iProductNo).Append(" : ").Append(cp.ProductJointName).Append("\r\n");
			iProductNo++;
		}

		//------------------------------------------------------
		// メール送信
		//------------------------------------------------------
		// メール送信パラメータの格納
		Hashtable htMailTempInput = new Hashtable();
		htMailTempInput.Add("url", sbURI.ToString());					// ドコモケータイ払い用URI
		htMailTempInput.Add("price", coCart.PriceTotal.ToString());	// カートごとの支払い合計金額
		htMailTempInput.Add("order_item", sbOrderItems.ToString());		// 商品情報

		// メール送信(携帯へ送信する)
		using (MailSendUtility msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_DOCOMO_PAYMENT_NEXT, coCart.CartUserId, htMailTempInput, false, Constants.MailSendMethod.Auto, userMailAddress: strMailAddr))
		{
			msMailSend.AddTo(strMailAddr);
			if (msMailSend.SendMail() == false)
			{
				AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// コンビニ判定
	/// </summary>
	/// <param name="strOrderPaymentKbn"></param>
	/// <returns></returns>
	protected bool IsCvs(string strOrderPaymentKbn)
	{
		switch (strOrderPaymentKbn)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
				return true;
		}

		return false;
	}

	/// <summary>
	/// Need to show payment message html
	/// </summary>
	/// <param name="paymentKbn">Payment kbn</param>
	/// <returns>True if need to show payment message html, otherwise false</returns>
	protected bool NeedToShowPaymentMessageHtml(string paymentKbn)
	{
		var validPayment = new string[]
		{
			Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
			Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
			Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET,
			Constants.FLG_PAYMENT_PAYMENT_ID_ATM
		};

		return validPayment.Contains(paymentKbn);
	}

	/// <summary>
	/// ゼウス銀行振込判定
	/// </summary>
	/// <param name="strOrderPaymentKbn"></param>
	/// <returns></returns>
	protected bool IsBank(string strOrderPaymentKbn)
	{
		switch (strOrderPaymentKbn)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF:
				return true;
		}

		return false;
	}

	/// <summary>
	/// 注文付与ポイント合計取得
	/// </summary>
	/// <param name="lOrderHistories"></param>
	/// <returns></returns>
	protected decimal GetOrderPointAddTotal(IList lOrderHistories)
	{
		decimal dOrderPointAddTotal = 0;
		foreach (DataView dvOrderHistory in lOrderHistories)
		{
			if (dvOrderHistory.Count != 0)
			{
				dOrderPointAddTotal += (decimal)dvOrderHistory[0][Constants.FIELD_ORDER_ORDER_POINT_ADD];
			}
		}

		return dOrderPointAddTotal;
	}

	/// <summary>
	/// 注文金額総合計取得
	/// </summary>
	/// <param name="lOrderHistories"></param>
	/// <returns></returns>
	protected decimal GetOrderPriceTotalSummary(IList lOrderHistories)
	{
		decimal dOrderPriceTotalSummary = 0;
		foreach (DataView dvOrderHistory in lOrderHistories)
		{
			if (dvOrderHistory.Count != 0)
			{
				dOrderPriceTotalSummary += (decimal)dvOrderHistory[0][Constants.FIELD_ORDER_ORDER_PRICE_TOTAL];
			}
		}

		return dOrderPriceTotalSummary;
	}

	/// <summary>
	/// 決済金額取得
	/// </summary>
	/// <param name="lOrder">注文</param>
	/// <returns>決済金額合計</returns>
	protected string GetSettlementAmount(object order)
	{
		var settlementAmount = CurrencyManager.ToSettlementCurrencyNotation(
			(decimal)DataBinder.Eval(order, Constants.FIELD_ORDER_SETTLEMENT_AMOUNT),
			(string)DataBinder.Eval(order, Constants.FIELD_ORDER_SETTLEMENT_CURRENCY));
		return settlementAmount;
	}

	/// <summary>
	/// 商品ブランド名を取得
	/// </summary>
	/// <param name="drvOrderItem">注文商品情報</param>
	/// <returns>ブランド名</returns>
	protected string GetProductBrandName(Hashtable htProductInfo)
	{
		DataView dvProduct = GetProduct(
			(string)htProductInfo[Constants.FIELD_ORDERITEM_SHOP_ID],
			(string)htProductInfo[Constants.FIELD_ORDERITEM_PRODUCT_ID],
			(string)htProductInfo[Constants.FIELD_ORDERITEM_VARIATION_ID]);

		if (dvProduct.Count != 0)
		{
			string strBrandId = (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID1];
			return ProductBrandUtility.GetProductBrandName(strBrandId) + " ";
		}

		return "";
	}

	/// <summary>
	/// 商品カテゴリ名を全て取得
	/// </summary>
	/// <param name="drvOrderItem">注文商品情報</param>
	/// <returns>商品カテゴリ１～５に該当するカテゴリ名</returns>
	protected string GetProductCategoryName(Hashtable htProductInfo)
	{
		string[] strCategoryIds = null;

		DataView dvProduct = GetProduct(
			(string)htProductInfo[Constants.FIELD_ORDERITEM_SHOP_ID],
			(string)htProductInfo[Constants.FIELD_ORDERITEM_PRODUCT_ID],
			(string)htProductInfo[Constants.FIELD_ORDERITEM_VARIATION_ID]);

		strCategoryIds = new string[]{
			(string)dvProduct[0][Constants.FIELD_PRODUCT_CATEGORY_ID1],
			(string)dvProduct[0][Constants.FIELD_PRODUCT_CATEGORY_ID2],
			(string)dvProduct[0][Constants.FIELD_PRODUCT_CATEGORY_ID3],
			(string)dvProduct[0][Constants.FIELD_PRODUCT_CATEGORY_ID4],
			(string)dvProduct[0][Constants.FIELD_PRODUCT_CATEGORY_ID5]
		};

		DataView dvResult = null;
		StringBuilder sbCategoryFullName = new StringBuilder();
		for (int iLoop = 0; iLoop < strCategoryIds.Length; iLoop++)
		{
			if (strCategoryIds[iLoop] != "")
			{
				// ------------------------------------------------------
				// カテゴリ名称取得
				// ------------------------------------------------------
				dvResult = ProductCommon.GetParentAndCurrentCategories(
					(string)htProductInfo[Constants.FIELD_ORDERITEM_SHOP_ID],
					strCategoryIds[iLoop],
					this.UserFixedPurchaseMemberFlg);

				// 取得したカテゴリ名を結合する
				foreach (DataRowView drvProductCategory in dvResult)
				{
					sbCategoryFullName.Append(drvProductCategory[Constants.FIELD_PRODUCTCATEGORY_NAME]).Append("-");
				}

				// 末尾のハイフンを削除する
				if (sbCategoryFullName.Length > 0)
				{
					sbCategoryFullName.Remove(sbCategoryFullName.Length - 1, 1);
				}

				if (iLoop != strCategoryIds.Length)
				{
					// 末尾に半角スペースを加える
					if (sbCategoryFullName.Length > 0)
					{
						sbCategoryFullName.Append(" ");
					}
				}
			}
		}

		return sbCategoryFullName.ToString();
	}

	/// <summary>
	/// ギフト判定
	/// </summary>
	/// <param name="dvOrder">注文情報</param>
	/// <returns>ギフト判定</returns>
	protected bool IsGift(object objOrder)
	{
		if (objOrder is DataView)
		{
			return ((string)((DataView)objOrder)[0][Constants.FIELD_ORDER_GIFT_FLG] == Constants.FLG_ORDER_GIFT_FLG_ON);
		}

		return false;
	}

	/// <summary>
	/// 配送先＆商品情報作成（ギフト向け）
	/// </summary>
	/// <param name="objDataView"></param>
	/// <returns></returns>
	protected List<List<DataRowView>> CreteShippingsAndItems(object objDataView)
	{
		List<List<DataRowView>> lShippings = new List<List<DataRowView>>();

		if ((Constants.GIFTORDER_OPTION_ENABLED)
			&& (objDataView is DataView))
		{
			((DataView)objDataView).Sort = Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO;

			int iShippingNo = 0;
			List<DataRowView> lItems = null;
			foreach (DataRowView drv in (DataView)objDataView)
			{
				if (iShippingNo != (int)drv[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO])
				{
					iShippingNo = (int)drv[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO];
					lItems = new List<DataRowView>();
					lShippings.Add(lItems);
				}
				
				DataRowView sameItem = lItems.Find(item =>
					((string)item[Constants.FIELD_ORDERITEM_SHOP_ID] == (string)drv[Constants.FIELD_ORDERITEM_SHOP_ID])
					&& ((string)item[Constants.FIELD_ORDERITEM_PRODUCT_ID] == (string)drv[Constants.FIELD_ORDERITEM_PRODUCT_ID])
					&& ((string)item[Constants.FIELD_ORDERITEM_VARIATION_ID] == (string)drv[Constants.FIELD_ORDERITEM_VARIATION_ID])
					&& ((string)item[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID] == (string)drv[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID])
					&& ((string)item[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS] == (string)drv[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]));

				if (sameItem != null)
				{
					// 同一商品がすでにリストにあれば、数量、価格を加算
					sameItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = (int)sameItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] + (int)drv[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
					sameItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE] = (int)sameItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE] + (int)drv[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE];
					sameItem[Constants.FIELD_ORDERITEM_ITEM_PRICE] = (decimal)sameItem[Constants.FIELD_ORDERITEM_ITEM_PRICE] + (decimal)drv[Constants.FIELD_ORDERITEM_ITEM_PRICE];
					sameItem[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE] = (decimal)sameItem[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE] + (decimal)drv[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE];
				}
				else
				{
					// 同一商品がリストになければ追加
					lItems.Add(drv);
				}
			}
		}

		return lShippings;
	}

	/// <summary>
	/// シルバーエッグ用注文完了商品ID
	/// </summary>
	/// <returns>注文完了商品ID</returns>
	/// <remarks>
	/// ・カンマ区切りで並べる
	/// </remarks>
	protected string GetOrderProductsForSilveregg()
	{
		var orderProducts = string.Join(",", this.OrderList.SelectMany(dv => dv.Cast<DataRowView>().Select(drv => drv[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID])));

		return orderProducts;
	}

	/// <summary>
	/// 最後にカート投入された商品を取得
	/// </summary>
	/// <returns>最後にカート投入された商品の商品ID</returns>
	protected string GetLastAddedCartProduct()
	{
		Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		return "P" + (string)htParam["last_added_cart_product"];
	}

	/// <summary>
	/// 受注達成報告関連のタグを非表示化
	/// </summary>
	private void SetAchievementReportUnvisible()
	{
		this.WrGoogleAnalytics.Visible = false;
		this.WcCriteo.Visible = false;
	}

	/// <summary>
	/// 翻訳情報設定
	/// </summary>
	/// <param name="order">注文情報(DataView)</param>
	/// <returns>翻訳情報設定後注文情報</returns>
	private DataView SetTranslationData(DataView order)
	{
		// 決済種別名
		var payments = order.Cast<DataRowView>().Select(
			drv => new PaymentModel
			{
				PaymentId = (string)drv[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]
			}).ToArray();
		var paymentTranslationSettings = NameTranslationCommon.GetPaymentTranslationSettings(
			payments,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);

		order = SetOrderPaymentKbnNameTranslationDataToDataView(order, paymentTranslationSettings);
		return order;
	}

	/// <summary>
	/// 支払区分名翻訳名称設定
	/// </summary>
	/// <param name="order">注文情報(DataView)</param>
	/// <param name="translationSettings">翻訳設定情報</param>
	/// <returns>翻訳後注文情報</returns>
	public static DataView SetOrderPaymentKbnNameTranslationDataToDataView(DataView order, NameTranslationSettingModel[] translationSettings)
	{
		if (order.Count == 0 || translationSettings.Any() == false) return order;

		for (var i = 0; i < order.Count; i++)
		{
			// 翻訳情報設定
			foreach (var translationSetting in translationSettings)
			{
				if ((translationSetting.MasterId1 != (string)order[i][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])) continue;
				order[i]["order_payment_kbn_name"] = translationSetting.AfterTranslationalName;
			}
		}
		return order;
	}

	/// <summary>
	/// Get Information Uniform Invoice Invoice
	/// </summary>
	/// <param name="uniformInvoice">Uniform Invoice</param>
	/// <param name="uniformInvoiceOption1">Uniform Invoice Option1</param>
	/// <param name="uniformInvoiceOption2">Uniform Invoice Option2</param>
	/// <returns>Uniform Invoice Information</returns>
	protected string GetInformationUniformInvoiceInvoice(string uniformInvoice, string uniformInvoiceOption1, string uniformInvoiceOption2)
	{
		var result = string.Empty;
		switch (uniformInvoice)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				result = ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, uniformInvoice);
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				result = string.Format("{0}<br/>{1}<br/>{2}",
					ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, uniformInvoice),
					uniformInvoiceOption1,
					uniformInvoiceOption2);
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				result = string.Format("{0}<br/>{1}",
					ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, uniformInvoice),
					uniformInvoiceOption1);
				break;
		}

		return result;
	}

	/// <summary>
	/// Get Information Invoice
	/// </summary>
	/// <param name="carryType">Carry Type</param>
	/// <param name="carryTypeOption">Carry Type Option</param>
	/// <returns>Information Invoice</returns>
	protected string GetInformationCarryTypeInvoice(string carryType, string carryTypeOption)
	{
		var result = string.IsNullOrEmpty(carryType)
			? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, carryType)
			: string.Format("{0}<br />{1}",
				ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, carryType).Replace("コード", string.Empty),
				carryTypeOption);

		return result;
	}

	/// <summary>
	/// Get Order Invoice Visible
	/// </summary>
	/// <param name="orderId"></param>
	/// <returns>True When has order invoice: Otherwise return false</returns>
	protected bool GetOrderInvoiceVisible(string orderId)
	{
		var orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(orderId, 1);
		return ((Constants.TWINVOICE_ORDER_INVOICING
			|| Constants.TWINVOICE_ECPAY_ENABLED)
				&& (orderInvoice != null));
	}

	/// <summary>
	/// Get Order Shipping Visible
	/// </summary>
	/// <param name="receivingStoreFlg">Receiving Store Flag</param>
	/// <returns>False When different flag: Otherwise return true</returns>
	protected bool GetOrderShippingVisible(DataView receivingStoreFlg)
	{
		var result = (StringUtility.ToEmpty(receivingStoreFlg[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG])
			== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);

		return result;
	}

	/// <summary>
	/// 注文者名のかなを表示する
	/// </summary>
	/// <returns></returns>
	protected string ShowOwnerNameKana(string isoCode, string kana1, string kana2)
	{
		// 日本以外、またはAmazonPay利用の場合は非表示
		var result = IsCountryJp(isoCode) && (((this.IsLoggedIn == false) && (this.IsAmazonLoggedIn)) == false)
			? string.Format("({0}{1} さま)", kana1, kana2)
			: string.Empty;
		return result;
	}

	/// <summary>
	/// 注文拡張項目の表示内容を取得
	/// </summary>
	/// <param name="dv">注文データ</param>
	/// <returns>表示内容</returns>
	protected OrderExtendItemInput[] GetOrderExtendItemInput(DataView dv)
	{
		if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return new OrderExtendItemInput[]{};

		var inputDictionary = OrderExtendCommon.ConvertOrderExtend(dv[0]);
		var result = new OrderExtendInput(OrderExtendInput.UseType.Register, inputDictionary).OrderExtendItems;
		return result;
	}

	/// <summary>
	/// Get coupon name
	/// </summary>
	/// <param name="orderId">Order id</param>
	/// <returns>Coupon name</returns>
	protected string GetCouponName(string orderId)
	{
		var coupon = new OrderService().GetCoupon(orderId);
		if (coupon == null) return string.Empty;

		var result = string.IsNullOrEmpty(coupon.CouponDispName)
			? coupon.CouponName
			: coupon.CouponDispName;
		return result;
	}

	/// <summary>
	/// 定期２回目商品切り替え後に配送パターンが変わっているか
	/// </summary>
	/// <param name="index">カートリストインデックス</param>
	/// <returns>変わっていればTrue</returns>
	protected bool IsShippingPatternChanged(int index)
	{
		return Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED && ((m_lFixedPurchaseFirstPatternStrings[index] != "") && (m_lFixedPurchaseFirstPatternStrings[index] != m_lFixedPurchasePatternStrings[index]));
	}

	/// <summary>
	/// 定額頒布会コース情報を表示するか
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>表示するであればTRUE</returns>
	protected bool DisplaySubscriptionBoxFixedAmountCourse(DataRowView orderItem)
	{
		// 注文同梱でなければ表示しない
		var order = this.OrderList[0];
		if (string.IsNullOrEmpty((string)order[0][Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS])) return false;

		// 注文同梱であれば、同じ定額頒布会同士での同梱であれば表示しない
		var hasItemsNotFixedAmount = order
			.Cast<DataRowView>()
			.Any(item => IsOrderItemSubscriptionBoxFixedAmount(item) == false);
		if (hasItemsNotFixedAmount) return false;

		var fixedAmountCourseIdCount = order.Cast<DataRowView>()
			.Where(IsOrderItemSubscriptionBoxFixedAmount)
			.GroupBy(item => (string)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX])
			.Count();
		if (fixedAmountCourseIdCount == 1) return false;

		var isFixedAmountItem = IsOrderItemSubscriptionBoxFixedAmount(orderItem);
		return isFixedAmountItem;
	}

	/// <summary>
	/// 注文の全商品が頒布会定額コース商品か
	/// </summary>
	/// <param name="orderIndex">注文項番</param>
	/// <returns>全て頒布会定額コース商品であればTRUE</returns>
	protected bool IsOrderItemsAllSubscriptionBoxFixedAmount(int orderIndex)
	{
		var hasItemsNotFixedAmount = this.OrderList[orderIndex]
			.Cast<DataRowView>()
			.Any(item => IsOrderItemSubscriptionBoxFixedAmount(item) == false);
		return hasItemsNotFixedAmount == false;
	}

	/// <summary>
	/// 注文商品が頒布会定額コース商品か
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>定額頒布会の注文商品であればTRUE</returns>
	protected bool IsOrderItemSubscriptionBoxFixedAmount(DataRowView orderItem)
	{
		if (string.IsNullOrEmpty(
				StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT])))
		{
			return false;
		}

		var result =
			string.IsNullOrEmpty(
				StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX])) == false;
		return result;
	}

	/// <summary>
	/// オプションの合計を取得
	/// </summary>
	/// <param name="orders">オーダーリスト</param>
	/// <returns>合計</returns>
	protected decimal GetTotalOptionPrice(DataView orders)
	{
		var result = 0m;
		foreach (DataRowView order in orders)
		{
			result += ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts(
				(string)(order[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]));
		}
		return result;
	}

	/// <summary>
	/// Get real shop name
	/// </summary>
	/// <param name="realShopId">Real shop id</param>
	/// <returns>Real shop name</returns>
	protected string GetRealShopName(string realShopId)
	{
		var realShopName = string.Empty;

		if (string.IsNullOrEmpty(realShopId) == false)
		{
			var realShop = new RealShopService().Get(realShopId);
			realShopName = (realShop != null)
				? realShop.Name
				: string.Empty;
		}

		return realShopName;
	}

	/// <summary>
	/// Get real shop opening hours
	/// </summary>
	/// <param name="realShopId">Real shop id</param>
	/// <returns>Real shop opening hours</returns>
	protected string GetRealShopOpeningHours(string realShopId)
	{
		var realShopOpeningHours = string.Empty;

		if (string.IsNullOrEmpty(realShopId) == false)
		{
			var realShop = new RealShopService().Get(realShopId);
			realShopOpeningHours = (realShop != null)
				? realShop.OpeningHours
				: string.Empty;
		}

		return realShopOpeningHours;
	}

	/// <summary>
	/// Is display payment message text
	/// </summary>
	/// <param name="orderPaymentKbn">Order payment kbn</param>
	/// <returns>True: display payment message text, otherwise false</returns>
	protected bool IsDisplayPaymentMessageText(string orderPaymentKbn)
	{
		switch (orderPaymentKbn)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
			case Constants.FLG_PAYMENT_PAYMENT_ID_ATM:
				return true;
		}

		return false;
	}

	/// <summary>ドコモ決済注文情報</summary>
	Dictionary<string, CartObject> DocomoPaymentOrder
	{
		get { return (Dictionary<string, CartObject>)ViewState["DocomoPaymentOrder"]; }
		set { ViewState["DocomoPaymentOrder"] = value; }
	}
	/// <summary>注文情報</summary>
	protected List<Hashtable> OrderInfos
	{
		get { return (List<Hashtable>)ViewState["Orders"]; }
		set { ViewState["Orders"] = value; }
	}
	/// <summary>当画面を一度表示した事があるか判断</summary>
	protected bool IsAleadyDisplayed
	{
		get { return (bool?)this.OrderInfos[0]["AleadyDisplayed"] ?? false; }
		set { this.OrderInfos[0]["AleadyDisplayed"] = value; }
	}
	/// <summary>表示用番号</summary>
	protected int DispNum { get; set; }
	/// <summary>注文情報</summary>
	protected List<DataView> OrderList { get; set; }
	/// <summary>注文商品毎のシリアルキー情報</summary>
	protected Dictionary<string, DataView> OrderItemSerialKeys { get; set; }
	/// <summary>ドコモ決済エラーメッセージ</summary>
	protected string DocomoPaymentErrorMessage { get; set; }
	/// <summary>コンビニ後払い(DSK)の与信結果がHOLDか</summary>
	protected bool IsDskDeferredAuthResultHold { get; set; }
	/// <summary>コンビニ後払い(スコア)の与信結果がHOLDか</summary>
	protected bool IsScoreDeferredAuthResultHold { get; set; }
	/// <summary>コンビニ後払い(ベリトランス)の与信結果がHOLDか</summary>
	protected bool IsVeritransDeferredAuthResultHold { get; set; }
	/// <summary>GMOアトカラの与信結果が審査中か</summary>
	protected bool IsGmoAtokaraAuthResultHold { get; set; }
}
