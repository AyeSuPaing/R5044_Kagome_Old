/*
=========================================================================================================
  Module      : データ移行エラーログ情報出力(DataMigrationOutputLog.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;

/// <summary>
/// Data migration output log
/// </summary>
public partial class Form_DataMigration_DataMigrationOutputLog : BasePage
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// Sanitize and output the contents of the error log
		var outputLog = string.Empty;
		if (GetErrorLog(out outputLog))
		{
			lContent.Text = WebSanitizer.HtmlEncode(outputLog);
		}
		// Error is displayed on the error screen
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = outputLog;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// Get the contents of the error log (No conversion to line feed-> Br is performed)
	/// </summary>
	/// <param name="outputLog">Output the error log. If there is an error, an error message is output</param>
	/// <returns>True with normal termination, False if there is an error.</returns>
	private bool GetErrorLog(out string outputLog)
	{
		// Acquisition of each parameter information
		// * To prevent directory traversal
		//   Deleted "..", "\", and "/" for the shop id and master type (corresponding to the directory).
		//   Deleted "\" and "/" in the file name. (Because ".." can be included in the file name)

		// Shop id
		var shopId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DATA_MIGRATION_SHOP_ID])
			.Replace(@"\", string.Empty)
			.Replace(@"/", string.Empty)
			.Replace(@"..", string.Empty);
		// Master
		var master = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DATA_MIGRATION_MASTER])
			.Replace(@"\", string.Empty)
			.Replace(@"/", string.Empty)
			.Replace(@"..", string.Empty);
		// File name
		var logFileName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DATA_MIGRATION_OUTPUT_LOG_FILE_NAME])
			.Replace(@"\", string.Empty)
			.Replace(@"/", string.Empty);

		// Master type check
		if (CheckValidDataMigrationMasterType(master) == false)
		{
			outputLog = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTER_TYPE_INVALID);
			return false;
		}

		// Error log file specification check
		if (string.IsNullOrEmpty(shopId)
			|| string.IsNullOrEmpty(master)
			|| string.IsNullOrEmpty(logFileName))
		{
			outputLog = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FILE_LOG_DONT_SPECIFIED);
			return false;
		}

		// Error log file existence check
		var outputLogFilePath = Path.Combine(
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR,
			shopId,
			master,
			Constants.DIRNAME_MASTERIMPORT_COMPLETE, logFileName);
		if (File.Exists(outputLogFilePath) == false)
		{
			outputLog = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FILE_LOG_DONT_EXIST);
			return false;
		}

		// Error log file output
		using (var streamReader = new StreamReader(outputLogFilePath, Encoding.UTF8))
		{
			outputLog = streamReader.ReadToEnd();
		}

		return true;
	}

	/// <summary>
	/// Check valid data migration master type
	/// </summary>
	/// <param name="master">Master type</param>
	/// <returns>True: the master upload is used for data migration, otherwise: false</returns>
	private bool CheckValidDataMigrationMasterType(string master)
	{
		// Output control is done depending on whether the main upload is used for data migration
		switch (master)
		{
			case Constants.TABLE_ORDERCOUPON:
				return Constants.W2MP_COUPON_OPTION_ENABLED;

			case Constants.TABLE_FIXEDPURCHASE:
			case Constants.TABLE_FIXEDPURCHASEITEM:
			case Constants.TABLE_FIXEDPURCHASESHIPPING:
			case Constants.TABLE_FIXEDPURCHASEHISTORY:
				return Constants.FIXEDPURCHASE_OPTION_ENABLED;
		}

		return true;
	}
}
