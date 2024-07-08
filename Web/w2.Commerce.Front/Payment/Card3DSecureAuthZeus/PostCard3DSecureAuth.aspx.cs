/*
=========================================================================================================
  Module      : ゼウス3Dセキュア認証データ送信ページ(PostCard3DSecureAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Text;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

public partial class Payment_Card3DSecureAuthZeus_PostCard3DSecureAuth : BasePage
{
	/// <summary>
	/// セキュアバージョン種別
	/// </summary>
	private enum SecureType
	{
		/// <summary>3Dセキュア1.0</summary>
		[EnumTextName("1")]
		Secure1,
		/// <summary>3Dセキュア2.0</summary>
		[EnumTextName("2")]
		Secure2
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// パラメータ取得
		string strOrderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		PaymentFileLogger.WritePaymentLog(
			null,
			Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
			PaymentFileLogger.PaymentType.Zeus,
			PaymentFileLogger.PaymentProcessingType.Zeus3DSecureAuthResultSend,
			LogCreator.CreateMessage(strOrderId, ""));

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, strOrderId);

		// 受注情報取得
		DataView dvOrder = OrderCommon.GetOrder(strOrderId);

		if (dvOrder.Count != 0)
		{
			// 送信データ作成
			this.Card3DSecureTranId = (string)dvOrder[0][Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID];
			this.Card3DSecureAuthUrl = (string)dvOrder[0][Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL];
			this.Card3DSecureAuthKey = (string)dvOrder[0][Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY];
			this.ReturnUrl = CreateReturnUrl(strOrderId);
			this.Card3DSecure2Flag = SecureType.Secure1.ToText();

			// セキュア2.0対応のため、パラメータ追加
			if (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2)
			{
				this.Card3DSecure2Flag = SecureType.Secure2.ToText();
				this.Card3DSecureIframeUrl = this.Card3DSecureAuthUrl;
				this.Card3DSecureAuthUrl = string.Empty;
			}
		}
	}

	/// <summary>
	/// 認証結果戻し先URL作成
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateReturnUrl(string strOrderId)
	{
		StringBuilder sbReturnUrl = new StringBuilder();
		sbReturnUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT);
		sbReturnUrl.Append(Constants.PAGE_FRONT_PAYMENT_ZEUS_CARD_3DSECURE_GET_RESULT);
		sbReturnUrl.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode(strOrderId));

		return sbReturnUrl.ToString();
	}

	/// <summary>3DセキュアトランザクションID</summary>
	protected string Card3DSecureTranId { get; set; }
	/// <summary>3Dセキュア認証URL</summary>
	protected string Card3DSecureAuthUrl { get; set; }
	/// <summary>カードの認証種別</summary>
	protected string Card3DSecure2Flag { get; set; }
	/// <summary>3Dセキュアインターネットブラウザ情報収集用URL</summary>
	protected string Card3DSecureIframeUrl { get; set; }
	/// <summary>3Dセキュア認証キー</summary>
	protected string Card3DSecureAuthKey { get; set; }
	/// <summary>認証結果戻し先URL</summary>
	protected string ReturnUrl { get; set; }
}