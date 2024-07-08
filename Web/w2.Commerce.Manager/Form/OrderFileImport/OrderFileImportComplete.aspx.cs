/*
=========================================================================================================
  Module      : 注文関連ファイル取込完了ページ処理(OrderFileImportComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;

public partial class Form_OrderFileImport_OrderFileImportComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
	}

	/// <summary>
	///注文関連ファイル取込画面へ遷移クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGoToUpload_Click(object sender, System.EventArgs e)
	{
		// アップロード画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST);
	}

	/// <summary>Message can recredit</summary>
	protected string MessageCanRecredit
	{
		get
		{
			if ((Constants.DATAMIGRATION_OPTION_ENABLED == false)
				|| (DateTime.Now > Constants.DATAMIGRATION_END_DATETIME))
			{
				return string.Empty;
			}

			// Create url
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_DATA_MIGRATION_MANAGER)
				.CreateUrl();

			var result = WebMessages.GetMessages(WebMessages.ERRMSG_CAN_RE_CREDIT_FROM_HERE_MESSAGE)
				.Replace("@@ 1 @@", url);

			return result;
		}
	}
}
