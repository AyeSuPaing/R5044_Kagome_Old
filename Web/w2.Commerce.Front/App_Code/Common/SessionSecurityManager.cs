/*
=========================================================================================================
  Module      : セッションセキュリティ管理クラス(SessionSecurityManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain.TempDatas;

///**************************************************************************************
/// <summary>
/// セッションセキュリティ管理クラス
/// </summary>
///**************************************************************************************
public class SessionSecurityManager
{
	/// <summary>
	/// 未発行の場合、ユーザーに認証キー発行
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <remarks>
	/// [発行して良いかの判断]
	/// ○初回：クッキー、セッションともに空
	/// ○セッション切れ：クッキーだけ残ってセッションが空
	/// ○ログオフしてログイン：クッキーだけ残ってセッションが空
	/// ×ハイジャック：セッションだけあってクッキーが空
	/// ×発行済：クッキー、セッションともに残ってる
	///   →以上により、セッションが空だったら発行OK
	/// </remarks>
	public static void PublishAuthKeyIfUnpublished(HttpContext context)
	{
		if (context.Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION] == null)
		{
			string authKey = Guid.NewGuid().ToString();

			context.Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION] = authKey;

			CookieManager.SetCookie(Constants.COOKIE_KEY_AUTH_KEY, authKey, Constants.PATH_ROOT, secure: true);
		}
	}

	/// <summary>
	/// 認証キーが正しいかチェック
	/// レスポンスにセットされたばかりの可能性もあるため、リクエスト＆レスポンスどちらか一致するかチェック（レスポンスクッキーにセットされたばかりの可能性もあるため、リクエストクッキーORレスポンスクッキーどちらかと一致するかチェック）
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <returns>セッションの認証キーがクッキーと一致するか</returns>
	public static bool HasCorrectAuthKey(HttpContext context)
	{
		var requestCookieAuthKey = CookieManager.GetValue(Constants.COOKIE_KEY_AUTH_KEY);
		var responseCookieAuthKey = CookieManager.GetResponseValue(Constants.COOKIE_KEY_AUTH_KEY);

		var hasAuthKey = (((string)context.Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION] == requestCookieAuthKey)
			|| ((string)context.Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION] == responseCookieAuthKey));
		return hasAuthKey;
	}

	/// <summary>
	/// ログインチェック
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <returns>ログインの有無</returns>
	public static bool IsLoggedIn(HttpContext context)
	{
		return (context.Session[Constants.SESSION_KEY_LOGIN_USER_ID] != null);
	}

	/// <summary>
	/// かんたん会員かどうか
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <returns>true:かんたん会員、false:通常会員</returns>
	public static bool IsEasyUser(HttpContext context)
	{
		return ((string)context.Session[Constants.SESSION_KEY_LOGIN_USER_EASY_REGISTER_FLG] == Constants.FLG_USER_EASY_REGISTER_FLG_EASY);
	}

	/// <summary>
	/// ログインユーザーID取得
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <returns>ログインユーザーID</returns>
	public static string GetLoginUserId(HttpContext context)
	{
		return (string)context.Session[Constants.SESSION_KEY_LOGIN_USER_ID];
	}

	/// <summary>
	/// カートオブジェクト取得(無ければ生成）
	/// </summary>
	/// <param name="context">httpContext</param>
	/// <param name="orderKbn">注文区分</param>
	/// <returns>カートオブジェクト</returns>
	public static CartObjectList GetCartObjectList(HttpContext context, string orderKbn)
	{
		if (context.Session[Constants.SESSION_KEY_CART_LIST] == null)
		{
			context.Session[Constants.SESSION_KEY_CART_LIST] = CartCookieManager.GetCartListFromCookie(GetLoginUserId(context), orderKbn);
		}

		var cartList = (SessionManager.OrderCombineCartList != null)
			? SessionManager.OrderCombineCartList
			: SessionManager.CartList;

		if (Constants.CART_LIST_LP_OPTION
			&& (SessionManager.CartListLp != null))
		{
			var cartListLp = SessionManager.CartListLp;
			if (cartListLp.Items.Any()
				&& cartListLp.Items.All(cart => cart.IsOrderDone))
			{
				return cartList;
			}

			cartList = cartListLp;
		}

		return cartList;
	}

	/// <summary>
	/// LPカートオブジェクトリスト取得
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <returns>LPカートオブジェクトリストの配列</returns>
	public static CartObjectList[] GetLPCartObjectLists(HttpContext context)
	{
		return context.Session.Keys.Cast<string>()
			.Where(key => key.StartsWith(Constants.SESSION_KEY_CART_LIST_LANDING + "/"))
			.Select(key => (CartObjectList)context.Session[key]).ToArray();
	}

	/// <summary>
	/// セッションID変更向けセッション情報ＤＢ保存
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <param name="response">レスポンス</param>
	/// <param name="session">セッション</param>
	public static void SaveSesstionContetnsToDatabaseForChangeSessionId(
		HttpRequest request,
		HttpResponse response,
		HttpSessionState session
		)
	{
		// セッションデータのキャッシュ格納用キーを作成
		string sessionDataKey = "ASP.NET_SessionIdTmp " + session.SessionID;

		// セッションデータをＤＢ保存
		SaveSesstionContetnsToDatabase(session, TempDatasService.TempType.ChangeSessionId, sessionDataKey);

		// クッキーにキャッシュキー格納（セッション復元用） ※クライアントPCの時計に影響されるためセッションクッキーとする
		CookieManager.SetCookie(
			Constants.COOKIE_KEY_SESSION_DATA_KEY,
			sessionDataKey,
			Constants.PATH_ROOT,
			secure: request.IsSecureConnection);

		// セッションID破棄：クライアント側（ブラウザ）クッキーを過去の有効期限に設定し、削除するように誘導
		CookieManager.RemoveCookie(Constants.SESSION_COOKIE_NAME, "/");

		// ログイン前のセッションを破棄
		session.Abandon();
	}

	/// <summary>
	/// 外部サイト遷移向け セッション情報ＤＢ保存
	/// </summary>
	/// <param name="session">セッション</param>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	public static void SaveSesstionContetnsToDatabaseForGoToOtherSite(
		HttpSessionState session,
		string tempDataKey)
	{
		SaveSesstionContetnsToDatabase(
			session,
			TempDatasService.TempType.GoToOtherSite,
			tempDataKey);
	}

	/// <summary>
	/// セッション情報ＤＢ保存
	/// </summary>
	/// <param name="session">セッション</param>
	/// <param name="tempType">テンポラリデータタイプ</param>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	private static void SaveSesstionContetnsToDatabase(
		HttpSessionState session,
		TempDatasService.TempType tempType,
		string tempDataKey)
	{
		var sessionDatas = new Dictionary<string, object>();
		foreach (string key in session.Keys)
		{
			sessionDatas[key] = session[key];
		}
		new TempDatasService().Save(tempType, tempDataKey, sessionDatas);
	}

	/// <summary>
	///  セッションID変更向けＤＢセッション情報復元
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public static void RestoreSessionFromDatabaseForChangeSessionId(HttpContext context)
	{
		// クッキーからセッションデータのキャッシュキー取得
		var sessionDataKeyCookie = CookieManager.Get(Constants.COOKIE_KEY_SESSION_DATA_KEY);
		// クッキー自体存在しなければエラー（クッキー削除も、データベース削除もしない）
		if (sessionDataKeyCookie == null) throw new Exception("SessionDataKey Cookie 無しで呼び出されました。");

		var sessionDataKey = sessionDataKeyCookie.Value;

		//セッションデータキャッシュキーのクッキー削除（作成し直ささないといけない）
		CookieManager.RemoveCookie(Constants.COOKIE_KEY_SESSION_DATA_KEY, Constants.PATH_ROOT);

		// ＤＢからセッション情報復元
		RestoreSessionFromDatabase(context.Session, TempDatasService.TempType.ChangeSessionId, sessionDataKey, true);
	}

	/// <summary>
	/// 外部サイト遷移向け ＤＢセッション情報復元
	/// </summary>
	/// <param name="session">復元先セッション</param>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	/// <param name="delete">削除</param>
	public static bool RestoreSessionFromDatabaseForGoToOtherSite(HttpSessionState session, string tempDataKey, bool delete)
	{
		return RestoreSessionFromDatabase(session, TempDatasService.TempType.GoToOtherSite, tempDataKey, delete);
	}

	/// <summary>
	/// 外部サイト遷移向け ＤＢセッション情報復元
	/// </summary>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	/// <param name="delete">削除</param>
	public static Dictionary<string, object> RestoreSessionFromDatabaseForGoToOtherSite(string tempDataKey, bool delete)
	{
		return RestoreSessionFromDatabase(TempDatasService.TempType.GoToOtherSite, tempDataKey, delete);
	}

	/// <summary>
	/// ＤＢセッション情報復元
	/// </summary>
	/// <param name="session">セッション</param>
	/// <param name="tempType">テンポラリデータタイプ</param>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	/// <param name="delete">削除</param>
	private static bool RestoreSessionFromDatabase(HttpSessionState session, TempDatasService.TempType tempType, string tempDataKey, bool delete)
	{
		var sessionDatas = RestoreSessionFromDatabase(tempType, tempDataKey, delete);
		if (sessionDatas == null) return false;

		foreach (var key in sessionDatas.Keys)
		{
			session[key] = sessionDatas[key];
		}
		return true;
	}
	/// <summary>
	/// ＤＢセッション情報復元
	/// </summary>
	/// <param name="tempType">テンポラリデータタイプ</param>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	/// <param name="delete">削除</param>
	private static Dictionary<string, object> RestoreSessionFromDatabase(TempDatasService.TempType tempType, string tempDataKey, bool delete)
	{
		// データ取得
		var model = new TempDatasService().Resotre(tempType, tempDataKey);
		var sessionDatas = (Dictionary<string, object>)model.TempDataDeserialized;

		// 削除
		if (delete) DeleteSessionFromDatabase(tempType, tempDataKey);

		return sessionDatas;
	}

	/// <summary>
	/// 外部サイト遷移向け ＤＢセッション情報削除
	/// </summary>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	public static void DeleteSessionFromDatabaseForGoToOtherSite(string tempDataKey)
	{
		DeleteSessionFromDatabase(TempDatasService.TempType.GoToOtherSite, tempDataKey);
	}

	/// <summary>
	/// ＤＢセッション情報削除
	/// </summary>
	/// <param name="tempType">テンポラリデータタイプ</param>
	/// <param name="tempDataKey">テンポラリデータキー</param>
	private static void DeleteSessionFromDatabase(TempDatasService.TempType tempType, string tempDataKey)
	{
		new TempDatasService().Delete(tempType, tempDataKey);
	}

	/// <summary>
	/// セッションが重要情報を持っているかどうか
	/// </summary>
	/// <param name="context">HttpContext</param>
	/// <returns>セッションが重要情報を持っているか</returns>
	public static bool HasCriticalInformation(HttpContext context)
	{
		bool isSmartPhone = (Constants.SMARTPHONE_OPTION_ENABLED) && (SmartPhoneUtility.CheckSmartPhone(context.Request.UserAgent));
		return (IsLoggedIn(context)
			|| (GetCartObjectList(context, isSmartPhone ? Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE : Constants.FLG_ORDER_ORDER_KBN_PC).Owner != null)
			|| (GetLPCartObjectLists(context).Any(cartObjectList => cartObjectList.Owner != null)));
	}

	/// <summary>
	/// セキュアページプロトコル取得
	/// </summary>
	/// <returns>セキュアURL</returns>
	public static string GetSecurePageProtocolAndHost()
	{
		return Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN;
	}

	/// <summary>
	/// 非セキュアページプロトコル取得
	/// </summary>
	/// <param name="context">HTTPコンテキスト</param>
	/// <returns>非セキュアURL</returns>
	public static string GetUnsecurePageProtocolAndHost(HttpContext context)
	{
		if (Constants.ALLOW_HTTP_AFTER_LOGGEDIN)
		{
			return Constants.PROTOCOL_HTTP + Constants.SITE_DOMAIN;
		}
		else
		{
			return ((HasCriticalInformation(context)) ? Constants.PROTOCOL_HTTPS : Constants.PROTOCOL_HTTP) + Constants.SITE_DOMAIN;
		}
	}
}
