/*
=========================================================================================================
  Module      : データ移行管理(DataMigrationManager.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Web.UI.WebControls;

/// <summary>
/// Data migration manager
/// </summary>
public partial class Form_DataMigration_DataMigrationManager : DataMigrationPage
{
	/// <summary>Data migration directory</summary>
	private static string s_dataMigrationDirectory = string.Empty;

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// Set data migration
		s_dataMigrationDirectory = GetDataMigrationDirectory(
			this.LoginOperatorShopId,
			ddlMaster.SelectedValue,
			Constants.DIRNAME_MASTERIMPORT_UPLOAD);

		if (!IsPostBack)
		{
			SetMasterItemsForDataMigration(
				AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_DATA_MIGRATION_SETTING,
				ddlMaster,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DATA_MIGRATION_MASTER]));

			// Set data migration
			s_dataMigrationDirectory = GetDataMigrationDirectory(
				this.LoginOperatorShopId,
				ddlMaster.SelectedValue,
				Constants.DIRNAME_MASTERIMPORT_UPLOAD);

			// Load file list
			DisplayFileListForDataMigration(
				s_dataMigrationDirectory,
				btnUpload,
				trEnableUploadMessage,
				trDisableUploadMessage,
				rExistFiles);
		}

		// Set replace tag for buttons
		SetReplaceTagForButtonsToDataMigration(btnUpload, rExistFiles);
	}

	/// <summary>
	/// Master selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlMaster_SelectedIndexChanged(object sender, EventArgs e)
	{
		// Set data migration
		s_dataMigrationDirectory = Path.Combine(
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR,
			this.LoginOperatorShopId,
			((DropDownList)sender).SelectedValue,
			Constants.DIRNAME_MASTERIMPORT_UPLOAD);

		// Changed master type selected index
		ChangedMasterTypeSelectedIndexForDataMigration(
			s_dataMigrationDirectory,
			btnUpload,
			trEnableUploadMessage,
			trDisableUploadMessage,
			rExistFiles);
	}

	/// <summary>
	/// Click upload
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpload_Click(object sender, EventArgs e)
	{
		// Is there a file specified?
		if (string.IsNullOrEmpty(fUpload.Value) == false)
		{
			// Create if directory does not exist
			if (Directory.Exists(s_dataMigrationDirectory) == false)
			{
				Directory.CreateDirectory(s_dataMigrationDirectory);
			}

			// Error if not CSV file
			if (fUpload.PostedFile.FileName.EndsWith(".csv") == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_DATA_MIGRATION_FILE_UPLOAD_NOT_CSV);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
			}

			// File upload process
			var filePath = Path.Combine(s_dataMigrationDirectory, Path.GetFileName(fUpload.PostedFile.FileName));
			if (File.Exists(filePath))
			{
				// If the file already exists, go to the error page
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_FILE_ALREADY_EXISTS);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
			}
			else
			{
				// File existence check
				if (fUpload.PostedFile.InputStream.Length == 0)
				{
					// No file error
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_FILE_UNFIND);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
				}
				else
				{
					try
					{
						// File upload execution
						fUpload.PostedFile.SaveAs(filePath);
					}
					catch (UnauthorizedAccessException ex)
					{
						// File upload permission error (also recorded in the log)
						AppLogger.WriteError(ex);
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_FILE_UPLOAD_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
					}
				}
			}

			// To the data migration manager page (hold the selected master)
			Response.Redirect(CreateDataMigrationManagerUrl());
		}
		else
		{
			// Go to "Please select a file" error page
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_NO_FILE_UPLOAD);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// Upload file list repeater command
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rExistFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var fileName = e.CommandArgument.ToString();
		var uploadFilePath = Path.Combine(s_dataMigrationDirectory, fileName);

		// Deletion process
		if (e.CommandName == Constants.FLG_DATA_MIGRATION_COMMAND_NAME_DELETE)
		{
			if (File.Exists(uploadFilePath)) File.Delete(uploadFilePath);

			Response.Redirect(CreateDataMigrationManagerUrl());
		}
		// Import process
		else if (e.CommandName == Constants.FLG_DATA_MIGRATION_COMMAND_NAME_IMPORT)
		{
			// Create processing file directory
			var activeDirectory = GetDataMigrationDirectory(
				this.LoginOperatorShopId,
				ddlMaster.SelectedValue,
				Constants.DIRNAME_MASTERIMPORT_ACTIVE);

			// Import data migration
			ImportDataMigrationFile(uploadFilePath, activeDirectory, fileName);

			// Transition to the import execution completion screen
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_DATA_MIGRATION_COMPLETE);
		}
	}

	/// <summary>
	/// Create data migration manager url
	/// </summary>
	/// <returns>Data migration manager page url</returns>
	private string CreateDataMigrationManagerUrl()
	{
		var url = CreateDataMigrationManagerUrl(ddlMaster.SelectedValue);
		return url;
	}

	#region +Properties
	/// <summary>Maximum size of files that can be uploaded (megabytes)</summary>
	private string MaxRequestLength
	{
		get
		{
			var httpRuntimeSection = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
			var maxRequestLength = (httpRuntimeSection.MaxRequestLength / 1024);
			return maxRequestLength.ToString();
		}
	}
	/// <summary>Message notes data upload</summary>
	protected string MessageNotesDataUpload
	{
		get
		{
			var message = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_NOTES_DATA_UPLOAD_MESSAGE)
				.Replace("@@ 1 @@", this.MaxRequestLength);
			return message;
		}
	}
	#endregion
}
