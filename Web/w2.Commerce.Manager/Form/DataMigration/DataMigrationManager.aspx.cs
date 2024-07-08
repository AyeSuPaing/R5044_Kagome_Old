/*
=========================================================================================================
  Module      : データ移行管理(DataMigrationManager.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using w2.Common.Util;

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
		s_dataMigrationDirectory = Path.Combine(
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR,
			this.LoginOperatorShopId,
			ddlMaster.SelectedValue,
			Constants.DIRNAME_MASTERIMPORT_UPLOAD);

		if (!IsPostBack)
		{
			Initialize();
			ddlMaster_SelectedIndexChanged(ddlMaster, e);
		}

		// Set authorize mode enabled
		SetAuthorizeModeEnabled();

		// Set replace tag for buttons
		btnAuthExecute.Text = ReplaceTag("@@DispText.data_migration_manager.credit_execution.auth_execute_button_text@@");
		SetReplaceTagForButtonsToDataMigration(btnUpload, rExistFiles);

		// Display message
		divComp.Visible = this.CreditComplete;
		this.CreditComplete = false;
		btnAuthExecute.Enabled = (divComp.Visible == false);
	}

	/// <summary>
	/// Initialize
	/// </summary>
	private void Initialize()
	{
		// Credit execution information
		SetAuthorizeModes();
		SetDate();
		SetExtendedStatus();

		// Data upload for migration information
		SetMasterItemsForDataMigration(
			AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_DATA_MIGRATION_SETTING,
			ddlMaster,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DATA_MIGRATION_MASTER]));
	}

	/// <summary>
	/// Set authorize modes
	/// </summary>
	private void SetAuthorizeModes()
	{
		// Add items
		var authorizeModes = ValueText.GetValueItemArray(
			Constants.VALUETEXT_PARAM_DATA_MIGRATION_MANAGER,
			Constants.VALUETEXT_PARAM_CREDIT_MODE_TEXT);
		foreach (var item in authorizeModes)
		{
			rblAuthorizeMode.Items.Add(item);
		}

		// Set selected value
		var creditMode = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CREDIT_MODE]);

		if (string.IsNullOrEmpty(creditMode))
		{
			creditMode = this.AuthorizeModeKbn;
			this.AuthorizeModeKbn = string.Empty;
		}

		var authorizeMode = (string.IsNullOrEmpty(creditMode) == false)
			? creditMode
			: Constants.FLG_CREDIT_MODE_KBN_AUTH_ONLY;

		var selectedItem = rblAuthorizeMode.Items.FindByValue(authorizeMode);
		if (selectedItem != null) selectedItem.Selected = true;
	}

	/// <summary>
	/// Set date
	/// </summary>
	private void SetDate()
	{
		// Set selected value
		var dateString = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CREDIT_DATE]);
		if (string.IsNullOrEmpty(dateString) == false)
		{
			var date = DateTime.Parse(dateString);
			ucCreditExecutionDate.SetStartDate(date);
		}
	}

	/// <summary>
	/// Set extended status
	/// </summary>
	private void SetExtendedStatus()
	{
		// Add items
		ddlExtendedStatus.Items.Add(new ListItem(string.Empty, string.Empty));

		var orderExtendSettingList = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);
		foreach (DataRowView item in orderExtendSettingList)
		{
			var itemText = string.Format(
				"{0}：{1}",
				item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString(),
				item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME].ToString());
			ddlExtendedStatus.Items.Add(
				new ListItem(
					itemText,
					item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString()));
		}

		// Set selected value
		var extendedStatusItem = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CREDIT_EXTENDED_STATUS]);
		if (string.IsNullOrEmpty(extendedStatusItem) == false)
		{
			var selectedItem = ddlExtendedStatus.Items.FindByValue(extendedStatusItem);
			if (selectedItem != null) selectedItem.Selected = true;
		}
	}

	/// <summary>
	/// Click auth execute
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAuthExecute_Click(object sender, EventArgs e)
	{
		var authorizeMode = rblAuthorizeMode.SelectedValue;
		var errorMessage = CheckAuthExecute(
			authorizeMode,
			ddlExtendedStatus.SelectedValue,
			ucCreditExecutionDate.StartDateString);

		var today = ucCreditExecutionDate.HfStartDate.Value;

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.AuthorizeModeKbn = authorizeMode;
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Create argument
		var args = string.Format("-m:{0} ", authorizeMode);
		args += (authorizeMode == Constants.FLG_CREDIT_MODE_KBN_AUTH_ONLY)
			? string.Format("-e:{0}", ddlExtendedStatus.SelectedValue)
			: string.Format("-d:{0}", today);

		ExecuteBatchForDataMigration(Constants.PHYSICALDIRPATH_REAUTH_EXE, args);
		this.CreditComplete = true;

		// To the data migration manager page
		Response.Redirect(CreateDataMigrationManagerUrl());
	}

	/// <summary>
	/// Change authorize mode
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblAuthorizeMode_SelectedIndexChanged(object sender, EventArgs e)
	{
		// To the data migration manager page (hold the selected master)
		Response.Redirect(CreateDataMigrationManagerUrl());
	}

	/// <summary>
	/// Set authorize mode enabled
	/// </summary>
	private void SetAuthorizeModeEnabled()
	{
		var selected = rblAuthorizeMode.SelectedValue;
		var isAuthOnly = (selected == Constants.FLG_CREDIT_MODE_KBN_AUTH_ONLY);

		// Disable date input
		ucCreditExecutionDate.Disabled = isAuthOnly;
		// Disable extended status select
		ddlExtendedStatus.Enabled = isAuthOnly;
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
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// File upload process
			var filePath = Path.Combine(s_dataMigrationDirectory, Path.GetFileName(fUpload.PostedFile.FileName));
			if (File.Exists(filePath))
			{
				// If the file already exists, go to the error page
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_FILE_ALREADY_EXISTS);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			else
			{
				// File existence check
				if (fUpload.PostedFile.InputStream.Length == 0)
				{
					// No file error
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_FILE_UNFIND);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
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
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
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
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// Master selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlMaster_SelectedIndexChanged(object sender, EventArgs e)
	{
		// Set data migration directory
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
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_DATA_MIGRATION_COMPLETE);
		}
	}

	/// <summary>
	/// Create data migration manager url
	/// </summary>
	/// <returns>Data migration manager page url</returns>
	private string CreateDataMigrationManagerUrl()
	{
		var url = CreateDataMigrationManagerUrl(
			ddlMaster.SelectedValue,
			rblAuthorizeMode.SelectedValue,
			ddlExtendedStatus.SelectedValue,
			ucCreditExecutionDate.StartDateString);
		return url;
	}

	#region +Properties
	/// <summary>Authorize mode kbn</summary>
	protected string AuthorizeModeKbn
	{
		get { return StringUtility.ToEmpty(Session["AuthorizeModeKbn"]); }
		set { Session["AuthorizeModeKbn"] = value; }
	}
	/// <summary>Credit complete</summary>
	protected bool CreditComplete
	{
		get { return (bool)(Session["CreditComplete"] ?? false); }
		set { Session["CreditComplete"] = value; }
	}
	/// <summary>Message notes credit execution</summary>
	protected string MessageNotesCreditExecution
	{
		get
		{
			// Get date current
			var today = DateTimeUtility.ToStringFromRegion(
				DateTime.Today,
				DateTimeUtility.FormatType.ShortDate2Letter);

			// Get messsage
			var message = WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_NOTES_CREDIT_EXECUTION_MESSAGE)
				.Replace("@@ 1 @@", "10")
				.Replace("@@ 2 @@", today);
			return message;
		}
	}
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
