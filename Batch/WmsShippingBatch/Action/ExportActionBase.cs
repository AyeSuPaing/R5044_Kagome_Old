/*
=========================================================================================================
  Module      : Export Action Base (ExportActionBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.Commerce.Batch.WmsShippingBatch.Util;
using w2.Common.Logger;
using w2.Domain;

namespace w2.Commerce.Batch.WmsShippingBatch.Action
{
	/// <summary>
	///  Export action base
	/// </summary>
	public abstract class ExportActionBase : ActionBase
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ExportActionBase()
		{
			this.ExportSetting = ExportSettingUtility.GetExportSetting();
			this.DeferredPayment = ExportSettingUtility.GetDeferredPayment();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Execute action
		/// </summary>
		public override void Execute()
		{
			ExportData();
		}

		/// <summary>
		/// Get export data
		/// </summary>
		/// <returns>A array of export models</returns>
		public abstract IModel[] GetExportData();

		/// <summary>
		/// Export data
		/// </summary>
		protected void ExportData()
		{
			try
			{
				var models = GetExportData();
				if (models.Count() == 0) return;

				// Export data
				var exportData = ConvertExportData(models);
				var exportFileName = string.Format(this.ExportSetting.FileNameFormat, DateTime.Now);

				ExportData(exportData, exportFileName);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Convert export data
		/// </summary>
		/// <param name="models">The export models</param>
		/// <returns>A list of export data</returns>
		protected string[][] ConvertExportData(IModel[] models)
		{
			var result = models.Select(model =>
				this.ExportSetting.FieldSettings.Select(setting =>
					CsvUtility.ConvertField(model.DataSource, setting)).ToArray()).ToArray();
			return result;
		}

		/// <summary>
		/// Export data
		/// </summary>
		/// <param name="exportData">A list of export data</param>
		/// <param name="exportFileName">Export file name</param>
		protected void ExportData(string[][] exportData, string exportFileName)
		{
			var headerFields = this.ExportSetting.FieldSettings
				.Select(setting => setting.ExportHeaderName)
				.ToList();
			var header = string.Join(",", headerFields.Select(title => string.Format("\"{0}\"", title)));
			var csvBuilder = new StringBuilder();
			csvBuilder.Append(header).AppendLine();

			// Create line data
			foreach (var lineData in exportData)
			{
				var line = string.Join(",", lineData.Select(cellData => string.Format("\"{0}\"", cellData)));
				csvBuilder.AppendLine(line);
			}

			// Directory existence check (create if it does not exist)
			if (Directory.Exists(Constants.DIR_PATH_WAITING_FOR_PROCESSING) == false)
			{
				Directory.CreateDirectory(Constants.DIR_PATH_WAITING_FOR_PROCESSING);
			}

			// Write all text on the output path
			var outputPath = Path.Combine(Constants.DIR_PATH_WAITING_FOR_PROCESSING, exportFileName);
			File.WriteAllText(outputPath, csvBuilder.ToString(), Encoding.GetEncoding(932));
		}
		#endregion

		#region Properties
		/// <summary>Export setting</summary>
		protected ExportSetting ExportSetting { get; private set; }
		/// <summary>後払い決済区分ID</summary>
		protected List<string> DeferredPayment { get; private set; }
		#endregion
	}
}
