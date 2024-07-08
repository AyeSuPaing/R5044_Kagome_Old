using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Web.Page;

/// <summary>
/// Class1 の概要の説明です
/// </summary>
public class GlobalChangeMenuUserControl : CommonUserControl
{
	/// <summary>切り替え方式:言語</summary>
	protected const string MENU_TYPE_LANGUAGE = "L";

	/// <summary>切り替え方式:通過</summary>
	protected const string MENU_TYPE_CURRENCY = "C";

	/// <summary>切り替え方式:言語＋通貨</summary>
	protected const string MENU_TYPE_LANGUAGE_CURRENCY = "LC";
}