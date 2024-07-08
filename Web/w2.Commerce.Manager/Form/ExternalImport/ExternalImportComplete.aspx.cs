/*
=========================================================================================================
  Module      : 外部ファイル取込完了ページ処理(ExternalImportComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Form_ExternalImport_ExternalImportComplete : BasePage
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}
	protected void btnGoToUpload_Click(object sender, EventArgs e)
	{
		// アップロード画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXTERNALIMPORT_LIST);
	}
}
