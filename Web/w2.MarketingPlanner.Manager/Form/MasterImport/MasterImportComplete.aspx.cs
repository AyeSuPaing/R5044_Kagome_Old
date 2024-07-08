/*
=========================================================================================================
  Module      : マスタアップロード完了ページ処理(MasterImportComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

public partial class Form_MasterImport_MasterImportComplete : BasePage
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
	/// アップロード画面へ遷移クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGoToUpload_Click(object sender, System.EventArgs e)
	{
		// アップロード画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_LIST);
	}
}
