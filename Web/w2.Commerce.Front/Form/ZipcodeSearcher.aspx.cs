/*
=========================================================================================================
  Module      : 複数住所検索クラス (ZipcodeSearcher.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using w2.Domain.Zipcode;

/// <summary>
/// 複数住所検索クラス
/// </summary>
public partial class Form_ZipcodeSearcher : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// 複数町域にまたがる住所を取得
	/// </summary>
	/// <param name="zipcode1">郵便番号1</param>
	/// <param name="zipcode2">郵便番号2</param>
	/// <returns>複数住所検索結果JSON</returns>
	[WebMethod]
	public static string GetAddrJson(string zipcode1, string zipcode2)
	{
		var addrArray = new ZipcodeService().GetByZipcode(zipcode1 + zipcode2);
		return new JavaScriptSerializer().Serialize(addrArray);
	}
}