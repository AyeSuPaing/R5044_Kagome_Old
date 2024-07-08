/*
=========================================================================================================
  Module      : Webサニタイザクラス(WebSanitizer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

///*********************************************************************************************
/// <summary>
/// WEBサニタイザクラス
/// </summary>
///*********************************************************************************************
public class WebSanitizer : w2.Common.Web.HtmlSanitizer
{
	/// <summary>
	/// URL属性HTMLエンコード
	/// </summary>
	/// <param name="objSrc">対象URL</param>
	/// <returns></returns>
	public static new string UrlAttrHtmlEncode(object objSrc)
	{
		string strUrl = StringUtility.ToEmpty(objSrc);

		// フレンドリURLであれば通過
		if (FriendlyUrlUtility.CheckFriendlyUrl(strUrl))
		{
			return strUrl;
		}

		return w2.Common.Web.HtmlSanitizer.UrlAttrHtmlEncode(strUrl);
	}
}
