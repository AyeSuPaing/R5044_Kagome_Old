using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// BasePage の概要の説明です
/// </summary>
public class BasePage : System.Web.UI.Page
{
	/// <summary>モバイルかどうか</summary>
	public bool IsMobile
	{
		get
		{
			string strUserAgent = w2.Common.Util.StringUtility.ToEmpty(Request.UserAgent);

			// モバイル判定
			if (Regex.IsMatch(strUserAgent, @"^DoCoMo/")
				|| Regex.IsMatch(strUserAgent, @"^((KDDI-)?[A-Z]+[A-Z0-9]+\s+)?UP\.Browser/")
				|| Regex.IsMatch(strUserAgent, @"^((KDDI-)?[A-Z]+[A-Z0-9]+\s+)?UP\.Browser/")
				|| Regex.IsMatch(strUserAgent, @"^(J-PHONE|Vodafone|SoftBank)/")
				|| (Request.ServerVariables["HTTP_X_JPHONE_MSNAME"] != null)
#if DEBUG
			|| Regex.IsMatch(strUserAgent, @"^(J-EMULATOR|Vemulator)/")
#endif
)
			{
				return true;
			}

			return false;
		}
	}
}