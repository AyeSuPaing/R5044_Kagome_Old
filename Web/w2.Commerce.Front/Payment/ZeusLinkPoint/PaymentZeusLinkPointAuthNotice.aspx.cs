/*
=========================================================================================================
  Module      : ゼウス（LinkPoint）決済結果取得ページ処理(PaymentZeusLinkPointAuthNotice.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Amazon;
using w2.Common.Extensions;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.SendMail;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UserCreditCard;

public partial class Payment_ZeusLinkPoint_PaymentZeusLinkPointAuthNotice : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var isRedirect = (string.IsNullOrEmpty(this.ActionTypeString));

		if (isRedirect)
		{
			Constants.ActionTypes actionType;

			if (Enum.TryParse(Request[Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE], out actionType) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDITCARD_AUTH_ERROR);
				var urlString = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).CreateUrl();
				Response.Redirect(urlString);
			}
			this.ActionType = actionType;

			// 画面遷移
			ProcessRedirect();
		}
		else
		{
			var actionType =
				(Constants.ActionTypes)
				Enum.Parse(
					typeof(Constants.ActionTypes),
					this.ActionTypeString,
					true);
			this.ActionType = actionType;

			// 通知受け取り
			ProcessReceiveNotice();
		}
	}

	/// <summary>
	/// リダイレクト処理
	/// </summary>
	private void ProcessRedirect()
	{
		var urlString = string.Empty;

		if (this.Result == "OK")
		{
			switch (this.ActionType)
			{
				// 注文登録
				case Constants.ActionTypes.RegisterOrderCreditCard:
					// 決済画面へ
					// 画面遷移の正当性チェックのため遷移先ページURLを設定
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

					urlString = new UrlCreator(
							this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT)
						.AddParam(Constants.REQUEST_KEY_ORDER_ID, Request[Constants.REQUEST_KEY_ORDER_ID])
						.CreateUrl();
					break;

				// クレカ登録
				case Constants.ActionTypes.RegisterUserCreditCard:
					// クレジット一覧画面へ
					urlString = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST).CreateUrl();
					break;

				// 定期
				case Constants.ActionTypes.RegisterFixedPurchaseCreditCard:
					// 定期詳細へ
					urlString = PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]);
					break;

				// 注文変更
				case Constants.ActionTypes.ChangeOrderCreditCard:
					// 注文詳細へ
					urlString = new UrlCreator(
							this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
						.AddParam(Constants.REQUEST_KEY_ORDER_ID, Request[Constants.REQUEST_KEY_ORDER_ID])
						.CreateUrl();
					break;
			}
		}
		else
		{
			switch (this.ActionType)
			{
				// 注文登録
				case Constants.ActionTypes.RegisterOrderCreditCard:
					if (Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED == false)
					{
						ProcessRollbackOrder();
					}
					break;
			}

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDITCARD_AUTH_ERROR);
			urlString = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).CreateUrl();
		}

		// 画面遷移
		Response.Redirect(urlString);
	}

	/// <summary>
	/// 通知受け取り処理
	/// </summary>
	private void ProcessReceiveNotice()
	{
		var responseString = string.Empty;

		if (this.Result == "OK")
		{
			try
			{
				switch (this.ActionType)
				{
					// 注文登録
					case Constants.ActionTypes.RegisterOrderCreditCard:
						ProcessOrder();
						break;

					// クレカ登録
					case Constants.ActionTypes.RegisterUserCreditCard:
						ProcessCregitRgister();
						break;

					// 定期
					case Constants.ActionTypes.RegisterFixedPurchaseCreditCard:
						ProcessFixedPurchase();
						break;

					// 注文変更
					case Constants.ActionTypes.ChangeOrderCreditCard:
						ProcessChangeOrder();
						break;
				}

				responseString = "OK,";
			}
			catch (Exception ex)
			{
				responseString = "NG," + ex.ToString();
			}
		}
		else
		{
			responseString = "NG,";
		}

		Response.Clear();
		Response.Write(responseString);
		Response.End();
	}

	/// <summary>
	/// クレカ登録処理
	/// </summary>
	private void ProcessCregitRgister()
	{
		// クレカ登録
		RegisterCreditCard(true);

		// 登録完了メール送信
		SendMailCommon.SendModifyCreditCardMail(this.UserId);

		// ターゲットページ設定を消去
		this.SessionParamTargetPage = null;
	}

	/// <summary>
	/// 注文処理
	/// </summary>
	private void ProcessOrder()
	{
		var isSuccess = true;
		Hashtable order = null;
		CartObject cart = null;
		var isCardRegister = false;

		OrderRegisterFront register = null;

		// パラメータ取得
		var orderId = this.SendPointValue;

		try
		{
			// ファイルからセッション復元
			isSuccess = RestoreSession(orderId);

			if (isSuccess)
			{
				// 画面遷移の正当性チェック
				CheckOrderUrlSession();

				// セッションからパラメータ取得
				var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
				this.CartList = (landingCartSessionKey == null) ? GetCartObjectList() : (CartObjectList)Session[landingCartSessionKey];
				var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				this.DispErrorMessages = (List<string>)param["error"];

				register = new OrderRegisterFront(this.IsLoggedIn);
				register.SuccessOrders = (List<Hashtable>)param["order"];
				register.ZeusLinkPointPaymentOrders = (List<Hashtable>)param["zeus_linkpoint"];
				register.GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"];

				// 決済処理中の注文情報、カート情報を取得
				order = register.ZeusLinkPointPaymentOrders.Find(o =>
					(string)o[Constants.FIELD_ORDER_ORDER_ID] == orderId);

				cart = this.CartList.Items.Find(c => c.OrderId == orderId);

				if ((order == null) || (cart == null))
				{
					isSuccess = false;
				}
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// 失敗
			isSuccess = false;

			// ログ出力
			AppLogger.WriteError(ex);

			throw new Exception("セッション情報復元エラー");
		}

		// ここまででエラーがあればエラーページへ（仮注文は残す）
		if (isSuccess == false)
		{
			// ログ出力
			AppLogger.WriteError("セッション情報復元エラー：注文ID=" + orderId);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);

			throw new Exception("セッション情報復元エラー");
		}

		// クレカ登録
		RegisterCreditCard(isCardRegister);

		// 支払回数更新処理
		if (isSuccess)
		{
			order[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE] = this.Div;
			order[Constants.FIELD_ORDER_CARD_INSTRUMENTS] = ValueText.GetValueText(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName,
				this.Div);
			cart.Payment.CreditInstallmentsCode = this.Div;
			isSuccess = register.UpdateOrderInstallmentsCode(order, UpdateHistoryAction.DoNotInsert);
		}

		// ３．注文確定処理
		//	・ここを正常通過すれば何があっても注文完了。
		if (isSuccess)
		{
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = this.Ordd;
			// ３－１．注文ステータス更新
			isSuccess = register.UpdateForOrderComplete(order, cart, true, UpdateHistoryAction.DoNotInsert);
		}

		// ４．後処理
		if (isSuccess)
		{
			// 注文完了時処理
			if (this.RegisterUser != null)
			{
				var orderList = new OrderService().GetOrdersByUserId(cart.OrderUserId);
				var isSendMail = orderList.Any(
					userOrder => ((userOrder.OrderId != orderId)
						&& (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)));
				if (isSendMail) order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;
			}

			register.OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);

			// 注文完了後処理
			register.AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.Insert);

			// 注文完了画面用に注文IDを格納
			register.SuccessOrders.Add(order);

			// 注文リストから削除
			if (order != null)
			{
				register.ZeusLinkPointPaymentOrders.Remove(order);
			}
			if (cart != null)
			{
				this.CartList.DeleteCartVurtual(cart);
			}
		}

		// 決済画面へ
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// セッション保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);
	}

	/// <summary>
	/// 注文ロールバック処理
	/// </summary>
	private void ProcessRollbackOrder()
	{
		Hashtable order = null;
		CartObject cart = null;

		// パラメータ取得
		var orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		try
		{
			// ファイルからセッション復元
			var isSuccess = RestoreSession(orderId);

			if (isSuccess)
			{
				// 画面遷移の正当性チェック
				CheckOrderUrlSession();

				// セッションからパラメータ取得
				var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
				this.CartList = (landingCartSessionKey == null) ? GetCartObjectList() : (CartObjectList)Session[landingCartSessionKey];
				var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				this.DispErrorMessages = (List<string>)param["error"];

				var zeusLinkPointPaymentOrders = (List<Hashtable>)param["zeus_linkpoint"];

				// 決済処理中の注文情報、カート情報を取得
				order = zeusLinkPointPaymentOrders.Find(o =>
					((string)o[Constants.FIELD_ORDER_ORDER_ID] == orderId));

				cart = this.CartList.Items.Find(c => (c.OrderId == orderId));

				if ((order == null) || (cart == null))
				{
					isSuccess = false;
				}
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// ログ出力
			AppLogger.WriteError(ex);

			throw new Exception("セッション情報復元エラー");
		}

		this.TransactionName = "2-X.注文ロールバック処理";

		// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
		var isUserDelete = (cart != null)
			&& (((this.IsLoggedIn == false) || (this.RegisterUser != null))
				&& (this.SuccessOrders.Count == 0)
				&& (new OrderService().GetOrdersByUserId(cart.OrderUserId).Length == 1));

		OrderCommon.RollbackPreOrder(order, cart, isUserDelete, (int)order[Constants.FIELD_USERSHIPPING_SHIPPING_NO], this.IsLoggedIn, UpdateHistoryAction.Insert);

		if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
		{
			this.LoginUserId = null;
		}
	}

	/// <summary>
	/// 定期クレカ登録処理
	/// </summary>
	private void ProcessFixedPurchase()
	{
		var isSuccess = true;

		// パラメータ取得
		var fixedPurcaseId = this.SendPointValue;
		var fixedPurchaseOld = new FixedPurchaseService().Get(fixedPurcaseId);

		var isCardRegister = false;
		try
		{
			// ファイルからセッション復元
			isSuccess = RestoreSession(fixedPurcaseId);

			if (isSuccess)
			{
				// 画面遷移の正当性チェック
				CheckOrderUrlSession();

				// セッションからパラメータ取得
				var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				isCardRegister = (bool)param["is_card_register"];
			}
			else
			{
				throw new Exception("セッション情報復元エラー");
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// 失敗
			isSuccess = false;

			// ログ出力
			AppLogger.WriteError(ex);

			throw new Exception("セッション情報復元エラー");
		}

		// クレカ登録
		RegisterCreditCard(isCardRegister);

		// カード枝番とユーザーIDを取得
		int branchNo;
		int.TryParse(this.CreditBranchNoString, out branchNo);

		var installmentsCode = this.Div;
		// クレジットカード決済与信成功更新
		new FixedPurchaseService().UpdateForAuthSuccess(
			fixedPurchaseOld.FixedPurchaseId,
			branchNo,
			installmentsCode,
			Constants.FLG_LASTCHANGED_USER,
			LogCreator.CreateWithUserId(this.UserId),
			UpdateHistoryAction.DoNotInsert);

		// 支払い方法更新
		new FixedPurchaseService().UpdateOrderPayment(
			fixedPurchaseOld.FixedPurchaseId,
			Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
			branchNo,
			installmentsCode,
			"",
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		// 現行のAmazon支払い契約をClosed状態にする
		if (fixedPurchaseOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
		{
			var clba = AmazonApiFacade.CloseBillingAgreement(fixedPurchaseOld.ExternalPaymentAgreementId, "情報が変更され、新たに支払い契約を取り直したため。");
			if (clba.GetSuccess() == false) return;
		}

		// 更新履歴登録
		new UpdateHistoryService().InsertForFixedPurchase(fixedPurchaseOld.FixedPurchaseId, Constants.FLG_LASTCHANGED_USER);
		new UpdateHistoryService().InsertForUser(fixedPurchaseOld.UserId, Constants.FLG_LASTCHANGED_USER);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(
			fixedPurchaseOld.FixedPurchaseId,
			SendMailCommon.FixedPurchaseModify.PaymentMethod);
	}

	/// <summary>
	/// 注文変更処理
	/// </summary>
	private void ProcessChangeOrder()
	{
		var isSuccess = true;

		// パラメータ取得
		var orderId = this.SendPointValue;

		var orderOld = new OrderService().Get(orderId);
		OrderModel orderNew = null;

		var isCardRegister = false;
		var isUpdateFixedPurchase = false;
		try
		{
			// ファイルからセッション復元
			isSuccess = RestoreSession(orderId);

			if (isSuccess)
			{
				// 画面遷移の正当性チェック
				CheckOrderUrlSession();

				// セッションからパラメータ取得
				var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				orderNew = (OrderModel)param["order_new"];
				isCardRegister = (bool)param["is_card_register"];
				isUpdateFixedPurchase = (bool)param["is_update_fixedpurchase"];
				this.DispErrorMessages = (List<string>)param["error"];

				orderNew.CardInstallmentsCode = this.Div;
				orderNew.CardInstruments = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					OrderCommon.CreditInstallmentsValueTextFieldName,
					this.Div);
			}
			else
			{
				throw new Exception("セッション情報復元エラー");
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// 失敗
			isSuccess = false;

			// ログ出力
			AppLogger.WriteError(ex);

			throw new Exception("セッション情報復元エラー");
		}

		var orderItems = new OrderService().GetOrderHistoryDetailInDataView(orderId, this.LoginUserId, this.MemberRankId);
		// 注文セットプロモーション情報セット
		this.OrderSetPromotions = new List<Hashtable>();
		foreach (Hashtable orderItem in orderItems.ToHashtableList()
			.OrderBy(item => (item[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]).ToString()).ToArray())
		{
			if ((StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]) != "")
				&& ((int)orderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] == 1))
			{
				this.OrderSetPromotions.Add(orderItem);
			}
		}

		var errorMessage = string.Empty;
		var externalPaymentApiErrorMassage = string.Empty;

		// カード情報更新
		RegisterCreditCard(false);

		// 注文変更処理
		try
		{
			ExecuteChangeOrder(
				orderOld,
				orderNew,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				isCardRegister,
				isUpdateFixedPurchase,
				out errorMessage,
				out externalPaymentApiErrorMassage);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				throw new Exception(errorMessage);
			}
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);
			throw ex;
		}
	}

	/// <summary>
	/// クレカ登録
	/// </summary>
	/// <param name="isCardRegister">表示するか</param>
	/// <param name="accessor">アクセッサ</param>
	private void RegisterCreditCard(bool isCardRegister, SqlAccessor accessor = null)
	{
		// カード枝番とユーザーIDを取得
		int branchNo;
		int.TryParse(this.CreditBranchNoString, out branchNo);

		// 枝番より、カード情報更新
		var creditCard = new UserCreditCardService().Get(this.UserId, branchNo);
		creditCard.LastFourDigit = this.CardNumber;
		creditCard.ExpirationMonth = this.Yuko.Substring(0, 2);
		creditCard.ExpirationYear = this.Yuko.Substring(2);
		creditCard.CompanyCode = this.CardBrand;

		// クレジット仮登録2
		new UserCreditCardRegister().ExecProvisionalRegistration2(
			creditCard,
			UpdateHistoryAction.DoNotInsert,
			accessor);

		//クレジットカードを登録リストに表示させる
		if (isCardRegister)
		{
			new UserCreditCardService().UpdateDispFlg(
				this.UserId,
				branchNo,
				true,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}
	}

	#region プロパティ
	/// <summary>結果</summary>
	protected string Result
	{
		get { return (string)Request["result"]; }
	}
	/// <summary>IPコード</summary>
	protected string ClientIp
	{
		get { return (string)Request["clientip"]; }
	}
	/// <summary>オーダNO</summary>
	protected string Ordd
	{
		get { return (string)Request["ordd"]; }
	}
	/// <summary>決済金額</summary>
	protected string Money
	{
		get { return (string)Request["money"]; }
	}
	/// <summary>ユーザーの電話番号</summary>
	protected string TelNo
	{
		get { return (string)Request["telno"]; }
	}
	/// <summary>ユーザーのメールアドレス</summary>
	protected string Email
	{
		get { return (string)Request["email"]; }
	}
	/// <summary>フリーパラメータ</summary>
	protected string SendId
	{
		get { return (string)Request["sendid"]; }
	}
	/// <summary>フリーパラメータ</summary>
	protected string SendPoint
	{
		get { return (string)Request["sendpoint"]; }
	}
	/// <summary>カード番号下4桁</summary>
	protected string CardNumber
	{
		get { return (string)Request["cardnumber"]; }
	}
	/// <summary>カードブランド</summary>
	protected string CardBrand
	{
		get { return (string)Request["cardbrand"]; }
	}
	/// <summary>有効期限</summary>
	protected string Yuko
	{
		get { return (string)Request["yuko"]; }
	}
	/// <summary>支払回数</summary>
	protected string Div
	{
		get { return (string)Request["div"]; }
	}
	/// <summary>ユーザーIDの長さ</summary>
	private int UserIdLength { get { return this.SendId.Length - 5; } }
	/// <summary>クレジットカード枝番</summary>
	private string CreditBranchNoString { get { return this.SendId.Substring(this.UserIdLength); } }
	/// <summary>ユーザーID</summary>
	private string UserId { get { return this.SendId.Substring(0, this.UserIdLength); } }
	/// <summary>アクションタイプ文字列</summary>
	private string ActionTypeString
	{
		get { return (this.SendPoint + ":").Split(':')[0]; }
	}
	/// <summary>SendPointの値</summary>
	private string SendPointValue
	{
		get { return (this.SendPoint + ":").Split(':')[1]; }
	}
	/// <summary>アクションタイプ</summary>
	protected Constants.ActionTypes ActionType { get; set; }
	#endregion
}
