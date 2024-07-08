/*
=========================================================================================================
  Module      : 名称翻訳情報CSV出力ページ処理(NameTranslationSettingExport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.Domain.NameTranslationSetting;

public partial class Form_Common_NameTranslationSettingExport : System.Web.UI.Page
{
	#region ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if ((this.SearchCondition == null) || (this.ExportTargetDataKbn == null))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
		Export();

		this.SearchCondition = null;
		this.ExportTargetDataKbn = null;
	}
	#endregion

	#region -Export エクスポート処理
	/// <summary>
	/// エクスポート処理
	/// </summary>
	private void Export()
	{
		var exportTargetData = GetExportData();
		var csvFormatters = (exportTargetData != null)
			? exportTargetData.Select(s => new NameTranslationCsvOutputFormatter(s)).ToArray()
			: new NameTranslationCsvOutputFormatter[0];

		Response.AppendHeader("Content-Disposition", "attachment; filename="
			+ ValueText.GetValueText(Constants.TABLE_NAMETRANSLATIONSETTING, "export_file_name", string.Empty)
			+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
		Response.Write((csvFormatters.FirstOrDefault() ?? new NameTranslationCsvOutputFormatter()).OutputCsvHeader() + Environment.NewLine);
		Response.Write(string.Join(Environment.NewLine, csvFormatters.Select(csvFormatter => csvFormatter.FormatCsvLine()).ToArray()));
		Response.Flush();
	}
	#endregion

	#region -GetExportData エクスポート対象データ取得
	/// <summary>
	/// エクスポート対象データ取得
	/// </summary>
	/// <returns>エクスポート対象データ</returns>
	private List<NameTranslationSettingModel> GetExportData()
	{
		var exportTargetData = new List<NameTranslationSettingModel>();
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByListPage(this.ExportTargetDataKbn, this.SearchCondition);
		if (translationData != null) exportTargetData.AddRange(translationData);

		return exportTargetData;
	}
	#endregion

	#region プロパティ
	/// <summary>検索条件</summary>
	private string[] SearchCondition
	{
		get { return (string[])Session[Constants.SESSION_KEY_PARAM]; }
		set { Session[Constants.SESSION_KEY_PARAM] = value; }
	}
	/// <summary>出力対象データ区分</summary>
	private string ExportTargetDataKbn
	{
		get { return (string)Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN]; }
		set { Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = value; }
	}
	#endregion
}