/*
=========================================================================================================
  Module      : Maintenance Setting(MaintenanceSetting.ascx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common.RefreshFileManager;

public partial class Form_MaintenanceSetting_MaintenanceSetting : BasePage
{
	#region Constants
	/// <summary>Export File Name</summary>
	protected const string CONST_EXPORT_FILE_NAME = "InMaintenance";
	/// <summary>File Name Maintenace</summary>
	protected const string CONST_FILE_NAME_MAINTENANCE = "maintenance.html";
	#endregion

	#region Methods
	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponents();
		}
		trSaveComplete.Visible = false;
	}

	/// <summary>
	/// Initialize Components
	/// </summary>
	private void InitializeComponents()
	{
		// Read File Maintenance and set to TextBox
		var filePath = Constants.PHYSICALDIRPATH_FRONT_PC + CONST_FILE_NAME_MAINTENANCE;
		if (File.Exists(filePath))
		{
			tbEdit.Text = File.ReadAllText(filePath);
		}

		// Check Valid For Button
		if (File.Exists(Path.Combine(
			Constants.PHYSICALDIRPATH_FRONT_PC,
			CONST_EXPORT_FILE_NAME)))
		{
			lbMaintenance.Visible = true;
			btnEnd.Enabled = true;
			btnStart.Enabled = false;
		}
		else
		{
			lbMaintenance.Visible = false;
			btnEnd.Enabled = false;
			btnStart.Enabled = true;
		}
	}

	/// <summary>
	/// Button Start Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStart_Click(object sender, EventArgs e)
	{
		btnEnd.Enabled = true;
		btnStart.Enabled = false;
		lbMaintenance.Visible = true;
		var filePath = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, CONST_EXPORT_FILE_NAME);
		if (File.Exists(filePath) == false)
		{
			File.WriteAllText(filePath, tbEdit.Text, Encoding.UTF8);
		}
	}

	/// <summary>
	/// Button End Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEnd_Click(object sender, EventArgs e)
	{
		btnEnd.Enabled = false;
		btnStart.Enabled = true;
		lbMaintenance.Visible = false;
		File.Delete(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, CONST_EXPORT_FILE_NAME));

		// メンテナンスリフレッシュファイルを最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Maintenance).CreateUpdateRefreshFile();
	}

	/// <summary>
	/// Button Save
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSave_Click(object sender, EventArgs e)
	{
		var filePath = Constants.PHYSICALDIRPATH_FRONT_PC + CONST_FILE_NAME_MAINTENANCE;
		File.Delete(filePath);
		File.WriteAllText(filePath, tbEdit.Text, Encoding.UTF8);
		trSaveComplete.Visible = true;
	}
	#endregion
}