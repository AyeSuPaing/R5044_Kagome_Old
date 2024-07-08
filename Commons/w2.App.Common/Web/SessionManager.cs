/*
=========================================================================================================
  Module      : セッションマネージャー(SessionManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Web;
using System.Web.SessionState;

namespace w2.App.Common.Web
{
	/// <summary>
	/// セッションマネージャー
	/// </summary>
	public class SessionManager
	{
		/// <summary>
		/// セッションクリア
		/// </summary>
		public static void Clear()
		{
			HttpContext.Current.Session.Clear();
		}

		/// <summary>ZEUS決済タブレット利用</summary>
		public static bool UsePaymentTabletZeus
		{
			get
			{
				if (HttpContext.Current == null) return false;
				if (HttpContext.Current.Session == null) return false;
				return (bool)(HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_USE_PAYMENT_TABLET_ZEUS] ?? false);
			}
			set { HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_USE_PAYMENT_TABLET_ZEUS] = value; }
		}
		/// <summary> クレカ連携ID2（外部レスポンスから更新用 更新後空に）</summary>
		public static string UserCreditCooperationId2
		{
			get { return (string)Session["userCreditCardCooperationId2"]; }
			set { Session["userCreditCardCooperationId2"] = value; }
		}
		/// <summary>セッション</summary>
		public static HttpSessionState Session
		{
			get { return HttpContext.Current.Session; }
		}

		/// <summary>表示用商品リスト</summary>
		public static bool[] DisplayProductList
		{
			get { return (bool[])Session["display_product_list"]; }
			set { Session["display_product_list"] = value; }
		}
		/// <summary> PayTgモック用レスポンス </summary>
		public static Hashtable PayTgMockResponse
		{
			get { return (Hashtable)Session["payTgMockResponse"]; }
			set { Session["payTgMockResponse"] = value; }
		}
	}
}
