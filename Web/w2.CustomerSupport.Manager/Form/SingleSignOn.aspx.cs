/*
=========================================================================================================
  Module      : シングルサインオンページ処理(SingleSignOn.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Manager;
using w2.Domain.MenuAuthority.Helper;

public partial class Form_SingleSignOn : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			MenuAuthorityHelper.ManagerSiteType managerSiteType;
			if (Enum.TryParse(Request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_MANAGER_SITE_TYPE], out managerSiteType) == false)
			{
				managerSiteType = MenuAuthorityHelper.ManagerSiteType.Mp;
			}

			var info = new SingleSignOnInfoCreator().Create(
				this.LoginOperatorShopId,
				this.LoginOperatorId,
				managerSiteType);

			string errorMessage = null;
			switch (info.ErrorKbn)
			{
				case SingleSignOnInfoCreator.ErrorKbn.LoginError:
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOP_OPERATOR_LOGIN_ERROR);
					break;

				case SingleSignOnInfoCreator.ErrorKbn.LoginLimitedCountError:
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR);
					break;

				case SingleSignOnInfoCreator.ErrorKbn.OperatorUnaccessable:
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR);
					break;
			}
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// シングルサインオンHTML出力
			string html = CreateSingleSignOnHtml(
				Constants.PROTOCOL_HTTPS + Request.Url.Authority + info.LoginPageUrl,
				this.LoginOperatorShopId,
				info.ShopOperator.LoginId,
				info.ShopOperator.Password,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MANAGER_NEXTURL]));
			Response.Write(html);
		}
	}

	/// <summary>
	/// シングルサインオンHTML作成
	/// </summary>
	/// <param name="loginUrl">ログインURL</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="loginId">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns>シングルサインオンHTML</returns>
	private string CreateSingleSignOnHtml(string loginUrl, string shopId, string loginId, string password, string nextUrl)
	{
		return "<html>\r\n"
			+ "<head><title></title></head>\r\n"
			+ "<body onload=\"javascript:document.form1.submit();\">\r\n"
			+ "<form name=\"form1\" method=\"post\" action=\"" + WebSanitizer.UrlAttrHtmlEncode(loginUrl) + "\">\r\n"
			+ "<input type=\"hidden\" name=\"" + WebSanitizer.HtmlEncode(Constants.REQUEST_KEY_SHOP_ID) + "\" value=\"" + WebSanitizer.HtmlEncode(shopId) + "\" />\r\n"
			+ "<input type=\"hidden\" name=\"" + WebSanitizer.HtmlEncode(Constants.REQUEST_KEY_MANAGER_LOGIN_ID) + "\" value=\"" + WebSanitizer.HtmlEncode(loginId) + "\" />\r\n"
			+ "<input type=\"hidden\" name=\"" + WebSanitizer.HtmlEncode(Constants.REQUEST_KEY_MANAGER_PASSWORD) + "\" value=\"" + WebSanitizer.HtmlEncode(password) + "\" />\r\n"
			+ "<input type=\"hidden\" name=\"" + WebSanitizer.HtmlEncode(Constants.REQUEST_KEY_MANAGER_LOGIN_FLG) + "\" value=\"1\" />\r\n"
			+ "<input type=\"hidden\" name=\"" + WebSanitizer.HtmlEncode(Constants.REQUEST_KEY_MANAGER_NEXTURL) + "\" value=\"" + WebSanitizer.HtmlEncode(nextUrl) + "\" />\r\n"
			+ "</form>\r\n"
			+ "</body>\r\n"
			+ "</html>\r\n";
	}
}