/*
=========================================================================================================
  Module      : Amazon Pay履歴詳細画面処理(AmazonPayHistoryPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;

/// <summary>
/// AmazonPayHistoryPage の概要の説明です
/// </summary>
public class AmazonPayHistoryPage : HistoryPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }

	/// <summary>ViewStateキー：定期購入情報</summary>
	private const string VIEW_STATE_KEY_FIXED_PURCHASE_MODEL = "FixedPurchaseModel";
	/// <summary>ViewStateキー：注文情報</summary>
	private const string VIEW_STATE_KEY_ORDER_MODEL = "OrderModel";

	/// <summary>
	/// 初期化
	/// </summary>
	protected void Initialize()
	{
		// 定期購入情報の取得
		if (string.IsNullOrEmpty(this.RequestFixedPurchaseId) == false)
		{
			this.FixedPurchaseModel = new FixedPurchaseService().GetContainer(this.RequestFixedPurchaseId);
		}

		// 注文情報の取得
		if (string.IsNullOrEmpty(this.RequestOrderId) == false)
		{
			this.OrderModel = new OrderService().Get(this.RequestOrderId);
		}
	}

	/// <summary>
	/// 注文情報取得
	/// </summary>
	/// <param name="orderReferenceIdOrBillingAgreementId">注文リファレンスIDor支払い契約ID</param>
	/// <param name="orderType">注文種別</param>
	/// <returns>エラーメッセージ</returns>
	[WebMethod]
	public static string GetAmazonAddress(string orderReferenceIdOrBillingAgreementId, string orderType)
	{
		// トークン取得
		var session = HttpContext.Current.Session;
		var amazonModel = (AmazonModel)session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
		var token = amazonModel.Token;

		// 注文種別、住所種別
		AmazonConstants.OrderType eOrderType;
		var isValidOrderType = Enum.TryParse<AmazonConstants.OrderType>(orderType, out eOrderType);
		var errorMessage = string.Empty;
		if (string.IsNullOrEmpty(orderReferenceIdOrBillingAgreementId)
			|| string.IsNullOrEmpty(token)
			|| (isValidOrderType == false))
		{
			errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_INVALID_NAME_FOR_AMAZON_ADDRESS_WIDGET);
			return JsonConvert.SerializeObject(new { Error = errorMessage });
		}

		// ウィジェットから住所情報取得
		AmazonAddressInput input = null;
		if (eOrderType == AmazonConstants.OrderType.OneTime)
		{
			var res = AmazonApiFacade.GetOrderReferenceDetails(orderReferenceIdOrBillingAgreementId, token);
			input = new AmazonAddressInput(res);
		}
		else
		{
			var res = AmazonApiFacade.GetBillingAgreementDetails(orderReferenceIdOrBillingAgreementId, token);
			input = new AmazonAddressInput(res);
		}

		// 入力チェック
		errorMessage = input.Validate();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG] = errorMessage;
			return JsonConvert.SerializeObject(new { Error = errorMessage });
		}

		session.Remove(AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG);
		return JsonConvert.SerializeObject(new { Error = errorMessage });
	}

	#region プロパティ
	/// <summary>定期購入情報</summary>
	protected FixedPurchaseModel FixedPurchaseModel
	{
		get { return (FixedPurchaseModel)ViewState[VIEW_STATE_KEY_FIXED_PURCHASE_MODEL]; }
		set { ViewState[VIEW_STATE_KEY_FIXED_PURCHASE_MODEL] = value; }
	}
	/// <summary>注文情報</summary>
	protected OrderModel OrderModel
	{
		get { return (OrderModel)ViewState[VIEW_STATE_KEY_ORDER_MODEL]; }
		set { ViewState[VIEW_STATE_KEY_ORDER_MODEL] = value; }
	}
	/// <summary>リクエスト：定期購入ID</summary>
	protected string RequestFixedPurchaseId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]).Trim(); }
	}
	/// <summary>リクエスト：注文ID</summary>
	protected string RequestOrderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]).Trim(); }
	}
	/// <summary>通常注文</summary>
	protected bool HasOrder
	{
		get { return (this.OrderModel != null); }
	}
	/// <summary>定期注文</summary>
	protected new bool HasFixedPurchase
	{
		get { return (this.FixedPurchaseModel != null); }
	}
	#endregion
}