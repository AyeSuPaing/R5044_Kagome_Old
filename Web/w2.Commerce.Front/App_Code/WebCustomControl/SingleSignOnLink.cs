/*
=========================================================================================================
  Module      : シングルサインオンリンク（Webカスタムコントロール）(SingleSignOnLink.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.UI;

namespace w2.Commerce.Front.WebCustomControl
{
	/// <summary>
	/// SingleSignOnLink の概要の説明です
	/// </summary>
	[ToolboxData("<{0}:SingleSignOnLink runat=server></{0}:SingleSignOnLink>")]
	public class SingleSignOnLink : w2.App.Common.Web.WebCustomControl.SingleSignOnLink
	{
		/// <summary>
		/// クリックイベント
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			string userId = (string)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_USER_ID];
			Redirect(userId);
		}
	}
}