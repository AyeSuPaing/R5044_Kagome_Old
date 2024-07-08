/*
=========================================================================================================
  Module      : Data migration page process(DataMigrationPageProcess.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.DataMigration;
using w2.Domain.MenuAuthority.Helper;

/// <summary>
/// Data migration page process
/// </summary>
public class DataMigrationPageProcess : BasePageProcess
{
	/// <summary>
	/// Data migration page process
	/// </summary>
	/// <param name="caller">Caller</param>
	/// <param name="viewState">View state</param>
	/// <param name="context">Context</param>
	public DataMigrationPageProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// Set master items for data migration
	/// </summary>
	/// <param name="xmlPath">File path</param>
	/// <param name="DropDownList">Drop down list master</param>
	/// <param name="masterItem">Master item</param>
	public void SetMasterItemsForDataMigration(string xmlPath, DropDownList ddlMaster, string masterItem)
	{
		DataMigrationUtility.SetMasterItemsForDataMigration(xmlPath, ddlMaster, masterItem);
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
		DataMigrationUtility.ChangedMasterTypeSelectedIndexForDataMigration(
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
		DataMigrationUtility.DisplayFileListForDataMigration(
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
		DataMigrationUtility.SetReplaceTagForButtonsToDataMigration(btnUpload, rExistFiles);
	}

	/// <summary>
	/// Get data migration directory
	/// </summary>
	/// <param name="shopId">Shop id</param>
	/// <param name="masterType">Master type</param>
	/// <param name="dirName">Directory name</param>
	/// <returns>Data migration directory</returns>
	public string GetDataMigrationDirectory(string shopId, string masterType, string dirName)
	{
		return DataMigrationUtility.GetDataMigrationDirectory(shopId, masterType, dirName);
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
		DataMigrationUtility.ImportDataMigrationFile(
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
		DataMigrationUtility.ExecuteBatchForDataMigration(physicalPath, argument);
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
		return DataMigrationUtility.CreateDataMigrationManagerUrl(
			MenuAuthorityHelper.ManagerSiteType.Cs,
			masterType,
			authorizeMode,
			extendedStatus,
			creditExecutionDate);
	}
}