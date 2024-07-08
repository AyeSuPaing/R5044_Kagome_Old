/*
=========================================================================================================
  Module      : Shipping Receiving Store Setting Import Action(ShippingReceivingStoreSettingImportAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using w2.App.Common;
using w2.Commerce.Batch.ShippingReceivingStoreSettingImportBatch.Util;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Ftp;

namespace w2.Commerce.Batch.ShippingReceivingStoreSettingImportBatch.Action
{
	/// <summary>
	/// Shipping Receiving Store Setting ImportAction
	/// </summary>
	public class ShippingReceivingStoreSettingImportAction
	{
		#region Fields
		/// <summary>File Name</summary>
		private const string FILE_NAME = "TwPelicanAllCvs";
		/// <summary>File Name Download</summary>
		private const string FILE_NAME_DOWNLOAD = "F01ALLCVS";
		/// <summary>File Extension</summary>
		private const string FILE_EXTENSION = ".xml";
		/// <summary>Format Date</summary>
		private const string FORMAT_DATE = "yyyyMMdd";
		/// <summary>Format Date Time</summary>
		private const string FORMAT_DATETIME = "yyyy/MM/dd HH:mm:ss";
		/// <summary>Folder Download In PC</summary>
		private const string FOLDER_DOWNLOAD_IN_PC = "Contents";
		#endregion

		#region Constructor
		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="ftp">FTP</param>
		public ShippingReceivingStoreSettingImportAction(FluentFtpUtility ftp)
		{
			this.DownloadDirectory = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, FOLDER_DOWNLOAD_IN_PC);
			this.Ftp = ftp;
		}
		#endregion

		#region Flow Excute Design
		/// <summary>
		/// Execute Action
		/// </summary>
		public void OnExecute()
		{
			// Download all import files from server
			DownloadImportFiles();
		}
		#endregion

		#region Method
		/// <summary>
		/// Download import files
		/// </summary>
		protected void DownloadImportFiles()
		{
			var isResultImport = true;
			var startDateTime = DateTime.Now;

			try
			{
				var files = this.Ftp.FileNameListDownload(string.Empty).Where(
					file => (file == string.Format(
						"{0}{1}{2}",
						FILE_NAME_DOWNLOAD,
						DateTime.Now.ToString(FORMAT_DATE),
						FILE_EXTENSION)));
				foreach (var file in files)
				{
					this.Ftp.Download(
						file,
						Path.Combine(this.DownloadDirectory, file));

					if (File.Exists(
						Path.Combine(
							this.DownloadDirectory,
							string.Format("{0}{1}", FILE_NAME, FILE_EXTENSION))))
					{
						File.Delete(
							Path.Combine(
								this.DownloadDirectory,
								string.Format("{0}{1}", FILE_NAME, FILE_EXTENSION)));
					}
					File.Move(
						Path.Combine(this.DownloadDirectory, file),
						Path.Combine(this.DownloadDirectory, string.Format("{0}{1}", FILE_NAME, FILE_EXTENSION)));
				}
			}
			catch(Exception ex)
			{
				isResultImport = false;
				FileLogger.WriteError(ex);
			}

			// Send Mail
			new MailSendUtil().SendMail(
				isResultImport,
				startDateTime.ToString(FORMAT_DATETIME),
				DateTime.Now.ToString(FORMAT_DATETIME));
		}
		#endregion

		#region Property
		/// <summary>The Download Directory</summary>
		public string DownloadDirectory { get; set; }
		/// <summary>The File Transfer Protocol object</summary>
		protected FluentFtpUtility Ftp { get; private set; }
		#endregion
	}
}
