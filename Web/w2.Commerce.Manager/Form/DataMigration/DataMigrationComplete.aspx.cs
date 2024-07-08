/*
=========================================================================================================
  Module      : データ移行完了(DataMigrationComplete.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// Data migration complete
/// </summary>
public partial class Form_DataMigration_DataMigrationComplete : BasePage
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		btnGoToDataMigration.Text = ReplaceTag("@@DispText.data_migration_complete.back_button_text@@");
	}

	/// <summary>
	/// Click go to data migration
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGoToDataMigration_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_DATA_MIGRATION_MANAGER);
	}
}
