/*
=========================================================================================================
  Module      : HTMLプレビュー用ページ処理(HtmlPreviewForm.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_HtmlPreviewForm : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var previewNo = 1;
			int.TryParse(StringUtility.ToEmpty(Request.QueryString[Constants.HTML_PREVIEW_NO]), out previewNo);

			this.HtmlForPreview = (this.HtmlForPreviewList != null)
				? this.HtmlForPreviewList[previewNo - 1]
				: string.Empty;
		}
	}

	/// <summary>プレビュー対象HTML</summary>
	protected string HtmlForPreview { get; set; }
}