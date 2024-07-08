/*
=========================================================================================================
  Module      : 商品カテゴリHTML出力コントローラ処理(BodyProductCategoryHtml.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.ProductCategory;

public partial class Form_Common_Product_BodyProductCategoryHtml : ProductUserControl
{
	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WdivCategoryHtml { get { return GetWrappedControl<WrappedHtmlGenericControl>("divCategoryHtml"); } }
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
			// カテゴリHTML表示処理
			//------------------------------------------------------
			// HTMLファイルパス決定
			string defaultPcPath = Constants.PAGE_FRONT_CATEGORY_DEFAULT_HTML;
			string defaultPcFullPath = Server.MapPath(Constants.PATH_ROOT + defaultPcPath);
			string defaultSpPath = SmartPhoneUtility.GetSmartPhonePath(Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent, defaultPcPath);
			string categoryHtmlPhysicalFilePath;

			if (Regex.IsMatch(this.CategoryId, @"^[\w\-.\!\@\#\$\%\^\&\(\)\{\}\[\]_ ]+$", RegexOptions.ECMAScript))
			{
				string catPcPath = Constants.PATH_FRONT_CATEGORY_HTML + this.CategoryId + ".html";
				string catSpPath = SmartPhoneUtility.GetSmartPhonePath(Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent, catPcPath);
				string catPcFullPath = Server.MapPath(Constants.PATH_ROOT + catPcPath);

				// PCサイトにカテゴリファイルが存在ないときは、nullにしてDefaultを使用
				if ((this.CategoryId == "")
					|| (File.Exists(catPcFullPath) == false)
					|| (new ProductCategoryService().CheckCategoryByFixedPurchaseMemberFlg(this.CategoryId, this.UserFixedPurchaseMemberFlg) == false))
				{
					catPcFullPath = null;
				}
				categoryHtmlPhysicalFilePath = catSpPath ?? defaultSpPath ?? catPcFullPath ?? defaultPcFullPath;
			}
			else
			{
				categoryHtmlPhysicalFilePath = defaultSpPath ?? defaultPcFullPath;
			}

			// HTML読み込み＆表示
			using (TextReader trCategory = new StreamReader(categoryHtmlPhysicalFilePath, Encoding.UTF8))
			{
				this.WdivCategoryHtml.InnerHtml = trCategory.ReadToEnd();
			}
		}
	}
}
