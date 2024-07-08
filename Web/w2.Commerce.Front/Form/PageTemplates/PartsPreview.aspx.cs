/*
=========================================================================================================
  Module      : パーツプレビューページ(PartsPreview.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.Common.Logger;

/// <summary>
/// パーツプレビューページ
/// </summary>
public partial class Form_PageTemplates_Preview : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		try
		{
			long partsId;
			if (string.IsNullOrEmpty(this.Request[Constants.REQUEST_KEY_PARTS_PREVIEW_PARTS_ID]) 
				|| (long.TryParse(this.Request[Constants.REQUEST_KEY_PARTS_PREVIEW_PARTS_ID],
				out partsId) == false))
			{
				FileLogger.WriteError("パラメータが不正");
				return;
			}

			var model = DataCacheControllerFacade.GetPartsDesignCacheController().CacheData.FirstOrDefault(m => m.PartsId == partsId);
			if (model == null)
			{
				FileLogger.WriteError(string.Format("PartsId {0}:対象パーツIDのパーツ情報が存在しません", partsId.ToString()));
				return;
			}

			var spRootPath = "";
			var targetSetting = SmartPhoneUtility.SmartPhoneSiteSettings.FirstOrDefault();
			if (targetSetting != null) spRootPath = targetSetting.RootPath;

			var partsPath = Path.Combine((this.IsSmartPhone) ? spRootPath : "~/", model.FileDirPath, model.FileName);
			var physicalPath = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, partsPath.Replace("~/", ""));
			if (File.Exists(physicalPath))
			{
				var uc = Page.LoadControl(partsPath);
				this.PartsPanel.Controls.Add(uc);
			}
			else
			{
				FileLogger.WriteError(string.Format("PartsId {0}:対象パーツIDの実ファイルが存在しません。", partsId.ToString()));
				return;
			}
		}
		catch (ArgumentNullException ex)
		{
			FileLogger.WriteError("対象パーツが存在しません", ex);
		}
	}
}