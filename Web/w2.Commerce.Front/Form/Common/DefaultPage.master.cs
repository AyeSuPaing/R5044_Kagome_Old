/*
=========================================================================================================
  Module      : デフォルトページマスタ処理(DefaultPage.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
public partial class Form_Common_DefaultPage : BaseMasterPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsPostBack)
		{
			SetSeoAndOgpTagSetting();
			this.Page.Header.DataBind();
		}

		// フレンドリーURL対応のためリアルURLをセット
		form1.Action = Request.Url.PathAndQuery;

		if (this.IsSmartPhone == false) return;
		foreach (var setting in SmartPhoneUtility.SmartPhoneSiteSettings.Where(s => (s.RootPath != "")))
		{
			var smartPhoneRootPath = Constants.PATH_ROOT + Server.MapPath(setting.RootPath).Replace(Request.PhysicalApplicationPath, string.Empty).TrimEnd('\\');
			if (Request.Url.AbsolutePath.StartsWith(smartPhoneRootPath) == false) continue;

			form1.Action = Constants.PATH_ROOT + Request.Url.PathAndQuery.Substring(smartPhoneRootPath.Length).TrimStart('/');
		}
	}
}
