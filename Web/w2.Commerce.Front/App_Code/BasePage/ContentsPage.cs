/*
=========================================================================================================
  Module      : コンテンツページ(ContentsPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using w2.App.Common.DataCacheController;

/// <summary>
/// コンテンツページ
/// </summary>
public class ContentsPage : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType
	{
		get { return PageAccessTypes.Http; }
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Init(object sender, EventArgs e)
	{
		// 基底メソッドコール
		base.Page_Init(sender, e);

		var fileName = StringUtility.ToEmpty(Path.GetFileName(this.AppRelativeVirtualPath)).Replace(Preview.PREVIEW_PAGE_EXTENSION, "");
		var model = DataCacheControllerFacade.GetPageDesignCacheController().CacheData
			.FirstOrDefault(
				m => (m.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM) 
					&& String.Equals(m.FileName, fileName, StringComparison.CurrentCultureIgnoreCase));

		if (model == null) return;

		var accessUser = GetReleaseRangeAccessUser();
		var result = new ReleaseRangePageDesign(model).Check(accessUser);
		ReleaseRangeRedirect(result);
	}
}