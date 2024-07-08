/*
=========================================================================================================
  Module      : ゼウス（LinkPoint）決済処理ページ(PaymentZeusLinkPointAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Payment_ZeusLinkPoint_PaymentZeusLinkPointAuth : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var actionType =
			(Constants.ActionTypes)
			Enum.Parse(
				typeof(Constants.ActionTypes),
				Request[Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE],
				true);
		this.ActionType = actionType;

		switch (this.ActionType)
		{
			case Constants.ActionTypes.RegisterOrderCreditCard:
				ProcessOrder();
				break;

			case Constants.ActionTypes.RegisterUserCreditCard:
				ProcessCregitRgister();
				break;

			case Constants.ActionTypes.RegisterFixedPurchaseCreditCard:
				ProcessFixedPurchase();
				break;

			case Constants.ActionTypes.ChangeOrderCreditCard:
				ProcessChangeOrder();
				break;
		}
	}

	/// <summary>
	/// 注文処理
	/// </summary>
	protected void ProcessOrder()
	{
		var ht = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		var zeusOrder = (List<Hashtable>)ht["zeus_linkpoint"];

		// パラメータ取得
		var orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);

		// 受注情報取得
		var order = new OrderService().Get(orderId);
		var currentZeusOrder = zeusOrder.First(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == orderId);
		this.CartList = GetTargetCartList(currentZeusOrder);
		var cart = this.CartList.Items.Find(c => c.OrderId == orderId);

		// カート内容と注文内容の金額を比較
		if (cart.PriceTotal != order.OrderPriceTotal)
		{
			RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_CHANGED), Constants.PAGE_FRONT_CART_LIST);
		}

		// 送信データ作成
		this.ClientIp = Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP;
		this.Money = order.OrderPriceTotal.ToString("F0");
		this.TelNo = order.Owner.OwnerTel1;
		this.Email = order.Owner.OwnerMailAddr;
		this.SendId = CreateSendId(order.UserId, (int)currentZeusOrder[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]);
		this.SendPoint = CreateSendPoint(orderId);
		this.SuccessUrl = CreateSuccessUrl(Constants.REQUEST_KEY_ORDER_ID, orderId);
		this.SuccessStr = "";
		this.FailureUrl = CreateFailureUrl(Constants.REQUEST_KEY_ORDER_ID, orderId);
		this.FailureStr = "";
	}

	/// <summary>
	/// クレカ登録処理
	/// </summary>
	protected void ProcessCregitRgister()
	{
		var user = new UserService().Get(this.LoginUserId);
		var creditCardInput = (UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM];

		var creditCard = new UserCreditCardRegister().ExecOnlySave(
			creditCardInput,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		this.ClientIp = Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP;
		this.Money = "0";
		this.TelNo = user.Tel1;
		this.Email = user.MailAddr;
		this.SendId = CreateSendId(user.UserId, creditCard.BranchNo);
		this.SendPoint = CreateSendPoint("");
		this.SuccessUrl = CreateSuccessUrl();
		this.SuccessStr = "";
		this.FailureUrl = CreateFailureUrl();
		this.FailureStr = "";
	}

	/// <summary>
	/// 定期処理
	/// </summary>
	protected void ProcessFixedPurchase()
	{
		// パラメータ取得
		var fixedPurchaseId = Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID];
		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		var branchNo = (int?)param[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO];

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, fixedPurchaseId);

		// 情報取得
		var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
		var user = new UserService().Get(this.LoginUserId);

		// 送信データ作成
		this.ClientIp = Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP;
		this.Money = "0";
		this.TelNo = user.Tel1;
		this.Email = user.MailAddr;
		this.SendId = CreateSendId(fixedPurchase.UserId, branchNo);
		this.SendPoint = CreateSendPoint(fixedPurchaseId);
		this.SuccessUrl = CreateSuccessUrl(Constants.REQUEST_KEY_FIXED_PURCHASE_ID, fixedPurchaseId);
		this.SuccessStr = "";
		this.FailureUrl = CreateFailureUrl();
		this.FailureStr = "";
	}

	/// <summary>
	/// 注文変更処理
	/// </summary>
	protected void ProcessChangeOrder()
	{
		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		var orderNew = (OrderModel)param["order_new"];

		// パラメータ取得
		var orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);

		// 送信データ作成
		this.ClientIp = Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP;
		this.Money = "0";
		this.TelNo = orderNew.Owner.OwnerTel1;
		this.Email = orderNew.Owner.OwnerMailAddr;
		this.SendId = CreateSendId(orderNew.UserId, orderNew.CreditBranchNo);
		this.SendPoint = CreateSendPoint(orderId);
		this.SuccessUrl = CreateSuccessUrl(Constants.REQUEST_KEY_ORDER_ID, orderId);
		this.SuccessStr = "";
		this.FailureUrl = CreateFailureUrl();
		this.FailureStr = "";
	}

	/// <summary>
	/// 認証結果戻し先URL作成（成功）
	/// </summary>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateSuccessUrl()
	{
		var url = CreateSuccessUrlCreator().CreateUrl();

		return url;
	}
	/// <summary>
	/// 認証結果戻し先URL作成（成功）
	/// </summary>
	/// <param name="key">キー</param>
	/// <param name="value">値</param>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateSuccessUrl(string key, string value)
	{
		var url = CreateSuccessUrlCreator()
			.AddParam(key, value)
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// 認証結果戻し先UrlCreator作成
	/// </summary>
	/// <returns>UrlCreator</returns>
	private UrlCreator CreateSuccessUrlCreator()
	{
		var urlCreator = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE)
			.AddParam("result", "OK")
			.AddParam(Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE, this.ActionType.ToString());

		return urlCreator;
	}

	/// <summary>
	/// 認証結果戻し先URL作成（失敗）
	/// </summary>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateFailureUrl()
	{
		var url = CreateFailureUrlCreator().CreateUrl();

		return url;
	}
	/// <summary>
	/// 認証結果戻し先URL作成（失敗）
	/// </summary>
	/// <param name="key">キー</param>
	/// <param name="value">値</param>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateFailureUrl(string key, string value)
	{
		var url = CreateFailureUrlCreator()
			.AddParam(key, value)
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// 認証結果戻し先UrlCreator作成
	/// </summary>
	/// <returns>UrlCreator</returns>
	private UrlCreator CreateFailureUrlCreator()
	{
		var urlCreator = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE)
			.AddParam("result", "NG")
			.AddParam(Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE, this.ActionType.ToString());

		return urlCreator;
	}

	/// <summary>
	/// SendId作成
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="branchNo">クレカ枝番</param>
	/// <returns>SendId</returns>
	private string CreateSendId(string userId, int? branchNo)
	{
		return string.Format("{0}{1}", userId, branchNo.ToString().PadLeft(5, '0'));
	}

	/// <summary>
	/// SendPoint作成
	/// </summary>
	/// <param name="value">値</param>
	/// <returns>SendPoint</returns>
	private string CreateSendPoint(string value)
	{
		return string.Format("{0}:{1}", this.ActionType, value);
	}

	#region プロパティ
	/// <summary>IPコード</summary>
	protected string ClientIp { get; set; }
	/// <summary>決済金額</summary>
	protected string Money { get; set; }
	/// <summary>ユーザーの電話番号</summary>
	protected string TelNo { get; set; }
	/// <summary>ユーザーのメールアドレス</summary>
	protected string Email { get; set; }
	/// <summary>フリーパラメータ</summary>
	protected string SendId { get; set; }
	/// <summary>フリーパラメータ</summary>
	protected string SendPoint { get; set; }
	/// <summary>決済完了ページリンク先URL</summary>
	protected string SuccessUrl { get; set; }
	/// <summary>決済完了ページ表示するテキスト</summary>
	protected string SuccessStr { get; set; }
	/// <summary>決済失敗ページリンク先URL</summary>
	protected string FailureUrl { get; set; }
	/// <summary>決済失敗ページ表示するテキスト</summary>
	protected string FailureStr { get; set; }
	/// <summary>ZEUSリンクポイントURL</summary>
	protected string ZeusLinkPointUrl
	{
		get
		{
			return string.IsNullOrEmpty(Constants.PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL_LOCALTEST)
				? Constants.PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL
				: Constants.PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL_LOCALTEST;
		}
	}
	/// <summary>アクションタイプ</summary>
	protected Constants.ActionTypes ActionType { get; set; }
	#endregion
}