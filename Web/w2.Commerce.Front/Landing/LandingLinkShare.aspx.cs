/*
=========================================================================================================
  Module      : リンクシェアランディングページ処理(LandingLinkShare.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Web;

public partial class Landing_LandingLinkShare : System.Web.UI.Page
{
	// ランディングテスト用
	// http://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/Landing/LandingLinkShare.aspx?lstid=test&lsurl=http%3a%2f%2flocalhost%2fV4%2e2%2fWeb%2fw2%2eCommerce%2eFront%2fForm%2fOrder%2fCartList%2easpx

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// キャッシュを無効にする（念のため）
		Response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
		Response.AddHeader("Pragma", "no-cache");

		// リンクシェアアフィリエイト？（上書き）
		if (Constants.AFFILIATE_LINKSHARE_VALID)
		{
			//------------------------------------------------------
			// トラッキングIDが取得できた場合のみ、クッキーに設定
			//------------------------------------------------------
			// トラッキングID取得
			string strLstid = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_LINK_AFFILIATE_LST_ID]);
			var tagId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_LINK_AFFILIATE_TAG_ID]);
			if (strLstid != "")
			{
				// クッキー情報セット
				CookieManager.SetCookie(
					Constants.COOKIE_KEY_AFFILIATE_LINKSHARE,
					new Dictionary<string, string>()
					{
						{ Constants.REQUEST_KEY_LINK_AFFILIATE_LST_ID, strLstid },
						{ Constants.REQUEST_KEY_LINK_AFFILIATE_ARRIVE_DATETIME, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") }, // アクセス日
						{ Constants.REQUEST_KEY_LINK_AFFILIATE_TAG_ID, tagId }
					},
					Constants.PATH_ROOT,
					DateTime.Now.AddDays(Constants.AFFILIATE_LINKSHARE_COOKIE_LIMIT_DAYS + 3));// 有効期限は+3日でセット。（最終的な有効/無効判定は成果報告後にLinkShareで行われる）);

				// 遷移先URLが取得できたら画面遷移
				string strRedirectUrl = StringUtility.ToEmpty(Request["lsurl"]);
				if (strRedirectUrl.StartsWith(Uri.UriSchemeHttp))
				{
					Response.Redirect(strRedirectUrl);
				}
			}

			//------------------------------------------------------
			// TOPページへ（トラッキング無し or 遷移先URLなし）
			//------------------------------------------------------
			Response.Redirect(Constants.PATH_ROOT);
		}
	}
}
