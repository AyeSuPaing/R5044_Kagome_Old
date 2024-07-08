/*
=========================================================================================================
  Module      : Data migration utility(DataMigrationUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using w2.App.Common.Web.Page;
using w2.Common.Web;
using w2.Domain.MenuAuthority.Helper;

namespace w2.App.Common.DataMigration
{
	/// <summary>
	/// Data migration utility
	/// </summary>
	public class DataMigrationUtility
	{
		/// <summary>
		/// Set master items for data migration
		/// </summary>
		/// <param name="xmlPath">File path</param>
		/// <param name="DropDownList">Drop down list master</param>
		/// <param name="masterItem">Master item</param>
		public static void SetMasterItemsForDataMigration(string xmlPath, DropDownList ddlMaster, string masterItem)
		{
			// Add items
			var masterItems = GetMasterItemFromXml(xmlPath);
			ddlMaster.Items.AddRange(masterItems);

			// Set selected value
			if (string.IsNullOrEmpty(masterItem) == false)
			{
				var selectedItem = ddlMaster.Items.FindByValue(masterItem);
				if (selectedItem != null) selectedItem.Selected = true;
			}
		}

		/// <summary>
		/// Get master item from xml
		/// </summary>
		/// <param name="xmlPath">File path</param>
		/// <returns>Master type</returns>
		private static ListItem[] GetMasterItemFromXml(string xmlPath)
		{
			var xDocument = XDocument.Load(xmlPath);
			var infoList = xDocument.Descendants("Master")
				.Select(item =>
					new
					{
						Name = item.Element("Name").Value,
						Directory = item.Element("Directory").Value
					});

			var result = new List<ListItem>();
			foreach (var info in infoList)
			{
				// Judgment by master type
				switch (info.Directory)
				{
					// Data migration table in EC
					case Constants.TABLE_ORDERCOUPON:
						if (Constants.W2MP_COUPON_OPTION_ENABLED)
						{
							result.Add(new ListItem(info.Name, info.Directory));
						}
						break;

					case Constants.TABLE_FIXEDPURCHASE:
					case Constants.TABLE_FIXEDPURCHASEITEM:
					case Constants.TABLE_FIXEDPURCHASESHIPPING:
					case Constants.TABLE_FIXEDPURCHASEHISTORY:
						if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
						{
							result.Add(new ListItem(info.Name, info.Directory));
						}
						break;

					// Data migration table in CS
					case Constants.TABLE_CSINCIDENT:
					case Constants.TABLE_CSMESSAGE:
						result.Add(new ListItem(info.Name, info.Directory));
						break;

					default:
						result.Add(new ListItem(info.Name, info.Directory));
						break;
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Changed master type selected index for data migration
		/// </summary>
		/// <param name="dataMigrationDirectory">Data migration directory</param>
		/// <param name="btnUpload">Button upload</param>
		/// <param name="trEnableUploadMessage">Html table row enable upload message</param>
		/// <param name="trDisableUploadMessage">Html table row disable upload message</param>
		/// <param name="rExistFiles">Repeater exist files</param>
		public static void ChangedMasterTypeSelectedIndexForDataMigration(
			string dataMigrationDirectory,
			Button btnUpload,
			HtmlTableRow trEnableUploadMessage,
			HtmlTableRow trDisableUploadMessage,
			Repeater rExistFiles)
		{
			// Load file list
			DisplayFileListForDataMigration(
				dataMigrationDirectory,
				btnUpload,
				trEnableUploadMessage,
				trDisableUploadMessage,
				rExistFiles);

			// Set replace tag for buttons
			SetReplaceTagForButtonsToDataMigration(btnUpload, rExistFiles);
		}

		/// <summary>
		/// Display file list for data migration
		/// </summary>
		/// <param name="dataMigrationDirectory">Data migration directory</param>
		/// <param name="btnUpload">Button upload</param>
		/// <param name="trEnableUploadMessage">Html table row enable upload message</param>
		/// <param name="trDisableUploadMessage">Html table row disable upload message</param>
		/// <param name="rExistFiles">Repeater exist files</param>
		public static void DisplayFileListForDataMigration(
			string dataMigrationDirectory,
			Button btnUpload,
			HtmlTableRow trEnableUploadMessage,
			HtmlTableRow trDisableUploadMessage,
			Repeater rExistFiles)
		{
			// Acquisition of existing file name & various display settings
			if (Directory.Exists(dataMigrationDirectory))
			{
				var files = Directory.GetFiles(dataMigrationDirectory);
				var existedFile = (files.Length > 0);

				// Display control
				btnUpload.Enabled = (existedFile == false);
				trEnableUploadMessage.Visible = (existedFile == false);
				trDisableUploadMessage.Visible = rExistFiles.Visible = existedFile;

				// Set existing file name to data source
				if (existedFile) rExistFiles.DataSource = files;
			}
			else
			{
				btnUpload.Enabled = trEnableUploadMessage.Visible = true;
				trDisableUploadMessage.Visible = rExistFiles.Visible = false;
			}

			rExistFiles.DataBind();
		}

		/// <summary>
		/// Set replace tag for buttons to data migration
		/// </summary>
		/// <param name="btnUpload">Button upload</param>
		/// <param name="rExistFiles">Repeater exist files</param>
		public static void SetReplaceTagForButtonsToDataMigration(Button btnUpload, Repeater rExistFiles)
		{
			btnUpload.Text = CommonPage.ReplaceTag("@@DispText.data_migration_manager.data_upload.upload_button_text@@");

			foreach (RepeaterItem item in rExistFiles.Items)
			{
				// Button import
				var btnImport = (Button)item.FindControl("btnImport");
				btnImport.OnClientClick = string.Format(
					"return confirm(\"{0}\");",
					CommonPage.ReplaceTag("@@DispText.data_migration_manager.uploaded_files.import_confirm_text@@"));
				btnImport.Text = CommonPage.ReplaceTag("@@DispText.data_migration_manager.uploaded_files.import_button_text@@");

				// Button delete
				var btnDelete = (Button)item.FindControl("btnDelete");
				btnDelete.OnClientClick = string.Format(
					"return confirm(\"{0}\");",
					CommonPage.ReplaceTag("@@DispText.data_migration_manager.uploaded_files.delete_confirm_text@@"));
				btnDelete.Text = CommonPage.ReplaceTag("@@DispText.data_migration_manager.uploaded_files.delete_button_text@@");
			}
		}

		/// <summary>
		/// Get data migration directory
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="masterType">Master type</param>
		/// <param name="dirName">Directory name</param>
		/// <returns>Data migration directory</returns>
		public static string GetDataMigrationDirectory(string shopId, string masterType, string dirName)
		{
			var dataMigrationDirectory = Path.Combine(
				Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR,
				shopId,
				masterType,
				dirName);
			return dataMigrationDirectory;
		}

		/// <summary>
		/// Import data migration file
		/// </summary>
		/// <param name="uploadFilePath">Upload file path</param>
		/// <param name="activeDirectory">Active directory</param>
		/// <param name="fileName">File name</param>
		public static void ImportDataMigrationFile(
			string uploadFilePath,
			string activeDirectory,
			string fileName)
		{
			if (Directory.Exists(activeDirectory) == false)
			{
				Directory.CreateDirectory(activeDirectory);
			}

			// Processing file path
			var activeFilePath = Path.Combine(activeDirectory, fileName);

			if (File.Exists(uploadFilePath))
			{
				// File movement (Because it may take time to create a process, pass the moved file to the batch.)
				File.Move(uploadFilePath, activeFilePath);

				// Process execution (pass the full path of the file after moving as an argument)
				ExecuteBatchForDataMigration(Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE, activeFilePath);
			}
		}

		/// <summary>
		/// Execute batch for data migration
		/// </summary>
		/// <param name="physicalPath">Physical path execute batch</param>
		/// <param name="argument">Argument</param>
		public static void ExecuteBatchForDataMigration(string physicalPath, string argument)
		{
			var exeProcess = new System.Diagnostics.Process
			{
				StartInfo =
				{
					FileName = physicalPath,
					Arguments = argument,
				}
			};

			// Reauth process
			exeProcess.Start();
		}

		/// <summary>
		/// Create data migration manager url
		/// </summary>
		/// <param name="managerSiteType">Manager site type</param>
		/// <param name="masterType">Master type</param>
		/// <param name="authorizeMode">Authorize mode</param>
		/// <param name="extendedStatus">Extended status</param>
		/// <param name="creditExecutionDate">Credit execution date</param>
		/// <returns>Data migration manager page url</returns>
		public static string CreateDataMigrationManagerUrl(
			MenuAuthorityHelper.ManagerSiteType managerSiteType,
			string masterType,
			string authorizeMode,
			string extendedStatus,
			string creditExecutionDate)
		{
			var isManagerSiteEC = (managerSiteType == MenuAuthorityHelper.ManagerSiteType.Ec);
			var pageName = isManagerSiteEC
				? Constants.PAGE_MANAGER_DATA_MIGRATION_MANAGER
				: Constants.PAGE_W2CS_MANAGER_DATA_MIGRATION_MANAGER;
			var url = new UrlCreator(Constants.PATH_ROOT + pageName);

			if (isManagerSiteEC)
			{
				// Authorize mode
				if (string.IsNullOrEmpty(authorizeMode) == false)
				{
					url.AddParam(
						Constants.REQUEST_KEY_CREDIT_MODE,
						authorizeMode);
				}

				// Extended status
				if (string.IsNullOrEmpty(extendedStatus) == false)
				{
					url.AddParam(
						Constants.REQUEST_KEY_CREDIT_EXTENDED_STATUS,
						extendedStatus);
				}

				// Date
				if (string.IsNullOrEmpty(creditExecutionDate) == false)
				{
					url.AddParam(
						Constants.REQUEST_KEY_CREDIT_DATE,
						creditExecutionDate);
				}
			}

			// Data migration type
			url.AddParam(Constants.REQUEST_KEY_DATA_MIGRATION_MASTER, masterType);

			return url.CreateUrl();
		}
	}
}
