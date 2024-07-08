/*
=========================================================================================================
  Module      : GMO3Dセキュア認証データ送信ページ(PostCard3DSecureAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.Order;

/// <summary>
/// GMO3Dセキュア認証データ送信ページ
/// </summary>
public partial class Payment_GMO_PostCard3DSecureAuth : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		// パラメータ取得
		var orderId = this.Request[Constants.REQUEST_KEY_ORDER_ID];

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(this.Session, orderId);

		// 送信データ作成
		CreatePostData(orderId);
	}

	/// <summary>
	/// 注文情報取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文情報</returns>
	private OrderModel GetOrder(string orderId)
	{
		var orderDv = OrderCommon.GetOrder(orderId);
		if (orderDv.Count == 0)
		{
			// 注文が見つからない場合はエラーページへ
			this.Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var order = new OrderModel(orderDv[0]);
		return order;
	}

	/// <summary>
	/// 送信データ作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void CreatePostData(string orderId)
	{
		var order = GetOrder(orderId);
		var idAndPass = order.Card_3dsecureTranId.Split(' ');
		if (idAndPass.Any() && (string.IsNullOrEmpty(idAndPass[0]) == false))
		{
			this.Card3DSecureTranId = idAndPass[0];
			this.Card3DSecureAuthUrl = order.Card_3dsecureAuthUrl;
			this.Card3DSecureAuthKey = order.Card_3dsecureAuthKey;
			this.ReturnUrl = CreateReturnUrl(orderId);
		}
		else
		{
			// 取引IDが見つからない場合はエラーページへ
			this.Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// 認証結果戻し先URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateReturnUrl(string orderId)
	{
		var returnUrl = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE_GET_RESULT)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();
		return returnUrl;
	}

	/// <summary>3DセキュアトランザクションID</summary>
	protected string Card3DSecureTranId { get; set; }
	/// <summary>3Dセキュア認証URL</summary>
	protected string Card3DSecureAuthUrl { get; set; }
	/// <summary>3Dセキュア認証キー</summary>
	protected string Card3DSecureAuthKey { get; set; }
	/// <summary>認証結果戻し先URL</summary>
	protected string ReturnUrl { get; set; }
}