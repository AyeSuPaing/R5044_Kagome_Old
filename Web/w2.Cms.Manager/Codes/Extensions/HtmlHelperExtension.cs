/*
=========================================================================================================
  Module      : HTMLヘルパのエクステンション(HtmlHelperExtention.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;

namespace w2.Cms.Manager.Codes.Extensions
{
	/// <summary>
	/// HTMLヘルパのエクステンション
	/// </summary>
	public static class HtmlHelperExtension
	{
		/// <summary>
		/// デバッグか
		/// </summary>
		/// <param name="htmlHelper">HTMLヘルパ</param>
		/// <returns>デバッグ実行中か</returns>
		public static bool IsDebug(this HtmlHelper htmlHelper)
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}
	}
}