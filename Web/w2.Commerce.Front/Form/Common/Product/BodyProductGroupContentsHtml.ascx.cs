/*
=========================================================================================================
  Module      : 商品グループページ表示内容HTML出力コントローラ処理(BodyProductGroupHtml.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.ProductGroup;

public partial class Form_Common_Product_BodyProductGroupContentsHtml : ProductUserControl
{
	#region メソッド
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 商品グループIDが指定されていない場合はリターン
			if (string.IsNullOrEmpty(this.ProductGroupId)) return;

			// 商品グループページ表示内容取得
			var targetProductGroupId = this.ProductGroupId;
			this.ProductGroupPageContents = GetProductGroupPageContents(targetProductGroupId);
		}
	}

	/// <summary>
	/// 商品グループ表示内容HTML取得
	/// </summary>
	/// <param name="targetProductGroupId">商品グループID</param>
	/// <returns>商品グループページ表示内容HTML</returns>
	private string GetProductGroupPageContents(string targetProductGroupId)
	{
		// 商品グループを取得
		var ApplicableProductGroups = DataCacheControllerFacade.GetProductGroupCacheController().GetApplicableProductGroup();
		var productGroup =
			ApplicableProductGroups
				.Where(grp => grp.ProductGroupId == targetProductGroupId)
				.FirstOrDefault();

		if (productGroup == null) return null;
		var productGroupPageContents = GetProductGroupPageContents(productGroup.ProductGroupPageContentsKbn, productGroup.ProductGroupPageContents);
		return productGroupPageContents;
	}
	/// <summary>
	/// 商品グループ表示内容HTML取得
	/// </summary>
	/// <param name="kbn">HTML区分（0:TEXT、1:HTML）</param>
	/// <param name="contents">商品グループページ表示内容</param>
	/// <returns>商品グループページ表示内容HTML</returns>
	private static string GetProductGroupPageContents(string contentsKbn, string contents)
	{
		var contentsHtml = StringUtility.ToEmpty(contents);
		switch (StringUtility.ToEmpty(contentsKbn))
		{
			case Constants.FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_HTML:
				// 相対パスを絶対パスに置換(aタグ、imgタグのみ）
				MatchCollection relativePath = Regex.Matches(contentsHtml, "(a[\\s]+href=|img[\\s]+src=)([\"|']([^/|#][^\\\"':]+)[\"|'])", RegexOptions.IgnoreCase);
				foreach (Match match in relativePath)
				{
					Uri resourceUri = new Uri(HttpContext.Current.Request.Url, match.Groups[3].ToString());
					contentsHtml = contentsHtml.Replace(match.Groups[2].ToString(), "\"" + resourceUri.PathAndQuery + resourceUri.Fragment + "\"");
				}
				return contentsHtml;

			case Constants.FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_TEXT:
				return WebSanitizer.HtmlEncodeChangeToBr(contentsHtml);

			default:
				return null;
		}
	}
	#endregion

	#region プロパティ
	/// <summary>商品グループページ表示内容</summary>
	protected string ProductGroupPageContents{ get; set; }
	#endregion
}