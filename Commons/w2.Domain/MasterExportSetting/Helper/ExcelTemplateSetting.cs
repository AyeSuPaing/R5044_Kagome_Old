/*
=========================================================================================================
  Module      : エクセルテンプレート設定(ExcelTemplateSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MasterExportSetting.Helper
{
	/// <summary>
	/// エクセルテンプレート設定
	/// </summary>
	public class ExcelTemplateSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="templatePath">テンプレートファイルパス</param>
		/// <param name="templateSettingPath">テンプレート設定ファイルパス</param>
		public ExcelTemplateSetting(string templatePath, string templateSettingPath)
		{
			this.TemplatePath = templatePath;
			this.TemplateSettingPath = templateSettingPath;
		}

		/// <summary>テンプレートファイルパス</summary>
		public string TemplatePath { get; private set; }
		/// <summary>テンプレート設定ファイルパス</summary>
		public string TemplateSettingPath { get; private set; }
	}
}