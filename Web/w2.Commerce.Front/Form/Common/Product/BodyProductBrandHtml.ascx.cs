/*
=========================================================================================================
  Module      : ブランドHTML出力コントローラ処理(BodyProductBrandHtml.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_Product_BodyProductBrandHtml : ProductUserControl
{
	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WdivBrandHtml { get { return GetWrappedControl<WrappedHtmlGenericControl>("divBrandHtml"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ブランドHTML表示処理
			//------------------------------------------------------
			// HTMLファイルパス決定
			string brandHtmlPhysicalFilePath = AppDomain.CurrentDomain.BaseDirectory + Constants.PATH_FRONT_BRAND_HTML + this.FolderName + "/" + this.BrandId + ".html";
			if ((this.BrandId == "")
				|| (File.Exists(brandHtmlPhysicalFilePath) == false))
			{
				brandHtmlPhysicalFilePath = AppDomain.CurrentDomain.BaseDirectory + Constants.PATH_FRONT_BRAND_HTML + this.FolderName + "/" + Constants.PAGE_FRONT_BRAND_DEFAULT_HTML_FILE;
			}

			// HTML読み込み＆表示
			using (TextReader trBrand = new StreamReader(brandHtmlPhysicalFilePath, Encoding.UTF8))
			{
				this.WdivBrandHtml.InnerHtml = trBrand.ReadToEnd();
			}
		}
	}

	/// <summary>フォルダ名</summary>
	public string FolderName { get; set; }
}
