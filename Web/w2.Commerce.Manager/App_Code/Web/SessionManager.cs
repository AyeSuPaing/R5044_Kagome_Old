/*
=========================================================================================================
  Module      : セッションマネージャー(SessionManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Web;

/// <summary>
/// セッションマネージャー
/// </summary>
public class SessionManager : w2.App.Common.Web.SessionManager
{
	/// <summary>//ワークフローシナリオ詳細の前ページURL</summary>
	public static string OrderworkflowDetailsUrlOfPreviousPage
	{
		get { return (string)HttpContext.Current.Session[Constants.SESSION_KEY_ORDERWORKFLOWDETAILS_URL_OF_PREVIOUS_PAGE]; }
		set { HttpContext.Current.Session[Constants.SESSION_KEY_ORDERWORKFLOWDETAILS_URL_OF_PREVIOUS_PAGE] = value; }
	}
	/// <summary>コピー元メールID</summary>
	public static string BeforeCopyMailId
	{
		get { return (string)HttpContext.Current.Session[Constants.SESSION_KEY_KEY_BEFORE_COPY_MAIL_ID]; }
		set { HttpContext.Current.Session[Constants.SESSION_KEY_KEY_BEFORE_COPY_MAIL_ID] = value; }
	}
	/// <summary>メール送信エラーメッセージ</summary>
	public static string SendMailErrorMessage
	{
		get { return (string)HttpContext.Current.Session[Constants.SESSION_KEY_SEND_MAIL_ERROR_MESSAGE]; }
		set { HttpContext.Current.Session[Constants.SESSION_KEY_SEND_MAIL_ERROR_MESSAGE] = value; }
	}
	/// <summary>会員番号（クロスポイント用）</summary>
	public static string MemberIdForCrossPoint
	{
		get { return (string)Session[Constants.SESSION_KEY_CROSSPOINT_MEMBER_ID]; }
		set { Session[Constants.SESSION_KEY_CROSSPOINT_MEMBER_ID] = value; }
	}
	/// <summary>PINコード（クロスポイント用）</summary>
	public static string PinCodeForCrossPoint
	{
		get { return (string)Session[Constants.SESSION_KEY_CROSSPOINT_PIN_CODE]; }
		set { Session[Constants.SESSION_KEY_CROSSPOINT_PIN_CODE] = value; }
	}
	/// <summary>店舗カードNO/PIN更新フラグ</summary>
	public static bool UpdatedShopCardNoAndPinFlg
	{
		get { return (bool)Session[Constants.SESSION_KEY_CROSS_POINT_UPDATED_SHOP_CARD_NO_AND_PIN_FLG]; }
		set { Session[Constants.SESSION_KEY_CROSS_POINT_UPDATED_SHOP_CARD_NO_AND_PIN_FLG] = value; }
	}
	/// <summary>Yahoo API "State" コード (CSRF対策)</summary>
	public static string YahooApiMallId
	{
		get { return (string)Session[Constants.SESSION_KEY_YAHOO_API_MALL_ID]; }
		set { Session[Constants.SESSION_KEY_YAHOO_API_MALL_ID] = value; }
	}
	/// <summary>Yahoo API "State" コード (CSRF対策)</summary>
	public static string YahooApiAntiForgeryStateCode
	{
		get { return (string)Session[Constants.SESSION_KEY_YAHOO_API_ANTI_FORGERY_STATE_CODE]; }
		set { Session[Constants.SESSION_KEY_YAHOO_API_ANTI_FORGERY_STATE_CODE] = value; }
	}
}
