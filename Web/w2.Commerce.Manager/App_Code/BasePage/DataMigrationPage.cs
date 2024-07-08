/*
=========================================================================================================
  Module      : Data migration page(DataMigrationPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Web.Process;

/// <summary>
/// Data migration page
/// </summary>
public class DataMigrationPage : BasePage
{
	/// <summary>
	/// Set master items for data migration
	/// </summary>
	/// <param name="xmlPath">File path</param>
	/// <param name="DropDownList">Drop down list master</param>
	/// <param name="masterItem">Master item</param>
	public void SetMasterItemsForDataMigration(string xmlPath, DropDownList ddlMaster, string masterItem)
	{
		this.Process.SetMasterItemsForDataMigration(xmlPath, ddlMaster, masterItem);
	}

	/// <summary>
	/// Changed master type selected index for data migration
	/// </summary>
	/// <param name="dataMigrationDirectory">Data migration directory</param>
	/// <param name="btnUpload">Button upload</param>
	/// <param name="trEnableUploadMessage">Html table row enable upload message</param>
	/// <param name="trDisableUploadMessage">Html table row disable upload message</param>
	/// <param name="rExistFiles">Repeater exist files</param>
	public void ChangedMasterTypeSelectedIndexForDataMigration(
		string dataMigrationDirectory,
		Button btnUpload,
		HtmlTableRow trEnableUploadMessage,
		HtmlTableRow trDisableUploadMessage,
		Repeater rExistFiles)
	{
		this.Process.ChangedMasterTypeSelectedIndexForDataMigration(
			dataMigrationDirectory,
			btnUpload,
			trEnableUploadMessage,
			trDisableUploadMessage,
			rExistFiles);
	}

	/// <summary>
	/// Display file list for data migration
	/// </summary>
	/// <param name="dataMigrationDirectory">Data migration directory</param>
	/// <param name="btnUpload">Button upload</param>
	/// <param name="trEnableUploadMessage">Html table row enable upload message</param>
	/// <param name="trDisableUploadMessage">Html table row disable upload message</param>
	/// <param name="rExistFiles">Repeater exist files</param>
	public void DisplayFileListForDataMigration(
		string dataMigrationDirectory,
		Button btnUpload,
		HtmlTableRow trEnableUploadMessage,
		HtmlTableRow trDisableUploadMessage,
		Repeater rExistFiles)
	{
		this.Process.DisplayFileListForDataMigration(
			dataMigrationDirectory,
			btnUpload,
			trEnableUploadMessage,
			trDisableUploadMessage,
			rExistFiles);
	}

	/// <summary>
	/// Set replace tag for buttons to data migration
	/// </summary>
	/// <param name="btnUpload">Button upload</param>
	/// <param name="rExistFiles">Repeater exist files</param>
	public void SetReplaceTagForButtonsToDataMigration(Button btnUpload, Repeater rExistFiles)
	{
		this.Process.SetReplaceTagForButtonsToDataMigration(btnUpload, rExistFiles);
	}

	/// <summary>
	/// Get data migration directory
	/// </summary>
	/// <param name="shopId">Shop id</param>
	/// <param name="masterType">Master type</param>
	/// <returns>Data migration directory</returns>
	public string GetDataMigrationDirectory(string shopId, string masterType, string dirName)
	{
		return this.Process.GetDataMigrationDirectory(shopId, masterType, dirName);
	}

	/// <summary>
	/// Import data migration file
	/// </summary>
	/// <param name="uploadFilePath">Upload file path</param>
	/// <param name="activeDirectory">Active directory</param>
	/// <param name="fileName">File name</param>
	public void ImportDataMigrationFile(
		string uploadFilePath,
		string activeDirectory,
		string fileName)
	{
		this.Process.ImportDataMigrationFile(
			uploadFilePath,
			activeDirectory,
			fileName);
	}

	/// <summary>
	/// Execute batch for data migration
	/// </summary>
	/// <param name="physicalPath">Physical path execute batch</param>
	/// <param name="argument">Argument</param>
	public void ExecuteBatchForDataMigration(string physicalPath, string argument)
	{
		this.Process.ExecuteBatchForDataMigration(physicalPath, argument);
	}

	/// <summary>
	/// Create data migration manager url
	/// </summary>
	/// <param name="masterType">Master type</param>
	/// <param name="authorizeMode">Authorize mode</param>
	/// <param name="extendedStatus">Extended status</param>
	/// <param name="creditExecutionDate">Credit execution date</param>
	/// <returns>Data migration manager page url</returns>
	public string CreateDataMigrationManagerUrl(
		string masterType,
		string authorizeMode,
		string extendedStatus,
		string creditExecutionDate)
	{
		return this.Process.CreateDataMigrationManagerUrl(
			masterType,
			authorizeMode,
			extendedStatus,
			creditExecutionDate);
	}

	/// <summary>
	/// Check auth execute
	/// </summary>
	/// <param name="authorizeMode">Authorize mode</param>
	/// <param name="extendStatus">Extend status</param>
	/// <param name="date">Date</param>
	/// <returns>Error message</returns>
	public string CheckAuthExecute(string authorizeMode, string extendStatus, string date)
	{
		return this.Process.CheckAuthExecute(authorizeMode, extendStatus, date);
	}

	/// <summary>Process</summary>
	protected new DataMigrationPageProcess Process
	{
		get { return (DataMigrationPageProcess)this.ProcessTemp; }
	}
	/// <summary>Process temp</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new DataMigrationPageProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
}