<%--
=========================================================================================================
  Module      : インターコム用HTTPSからHTTPへ行く場合のセッション引継ぎページ(DownloadLinkSender.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Front
  BaseVersion : V5.0
  Author      : M.Ochiai
  email       : product@w2solution.co.jp
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
  URL         : http://www.w2solution.co.jp/
=========================================================================================================
PKG-V5.0[PF0001] 2010/09/10 M.Ochiai        v5.0用に分離
--%>

<%@ Page Title="無名のページ" Language="C#" Inherits="BasePage" %>
<%@ Import Namespace="System.Net" %>
<script runat="server">
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{

		// セッションクッキーをNONセキュアに設定
		HttpCookie hcSessionId = Request.Cookies[Constants.SESSION_COOKIE_NAME];
		if (hcSessionId != null)
		{
			hcSessionId.Value = Session.SessionID;	// セッション切替のタイミングでValueが取得できないことがあるので明示的にセット
			hcSessionId.Secure = false;
			hcSessionId.HttpOnly = true;
			hcSessionId.Path = Constants.PATH_ROOT;	// セッションクッキーは自動で「/」がパスに設定されるため指定しない
			Response.Cookies.Remove(Constants.SESSION_COOKIE_NAME);
			Response.Cookies.Add(hcSessionId);
		}

		//デバッグ用
		//Response.Redirect("http://direct.intercom.co.jp/Form/User/testpage.htm?a=2");

		//HTTPにするので上手いことセッション引き継ぐ
		SessionSecurityManager.SaveSesstionContetnsToDatabaseAndClearSession(Request, Response, Session);
		Response.Redirect("https://direct.intercom.co.jp/Form/RestoreSession.aspx" + "?" + Constants.REQUEST_KEY_NEXT_URL + "="
		+ HttpUtility.UrlEncode("http://direct.intercom.co.jp/Form/User/DownloadLink.aspx"
		));
		
	}
</script>
