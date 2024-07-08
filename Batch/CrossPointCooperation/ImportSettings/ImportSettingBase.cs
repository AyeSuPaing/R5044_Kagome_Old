/*
=========================================================================================================
  Module      : Import setting base (ImportSettingBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;

namespace w2.Commerce.Batch.CrossPointCooperation.ImportSettings
{
	/// <summary>
	/// Import setting base
	/// </summary>
	public abstract class ImportSettingBase : IImportSetting
	{
		/// <summary>
		/// Import setting base
		/// </summary>
		protected ImportSettingBase()
		{
		}

		/// <summary>
		/// Convert and check
		/// </summary>
		/// <param name="data">Data</param>
		public void ConvertAndCheck(Hashtable data)
		{
			// Initialize
			this.Data = new Hashtable(data);
			this.ErrorMessages = string.Empty;
			this.ErrorOccurredIdInfo = string.Empty;

			// Data conversion (required to be implemented in each capture setting class)
			ConvertData();
		}

		/// <summary>
		/// Data conversion (various conversions, field joins, fixed value settings, etc.)
		/// </summary>
		/// <remarks>Must be implemented in each capture setting class</remarks>
		protected abstract void ConvertData();

		/// <summary>
		/// Header field settings
		/// </summary>
		/// <param name="headers">Headers</param>
		public void SetFieldNames(List<string> headers)
		{
			// Header field (CSV specification) setting
			this.HeadersCsv = new List<string>(headers);
		}

		/// <summary>
		/// Create id string
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <returns>Id as string</returns>
		protected string CreateIdString(string fieldName)
		{
			if (this.Data.ContainsKey(fieldName) == false) return string.Empty;

			var result = string.Format(
				" {0}={1} ",
				fieldName,
				this.Data[fieldName]);
			return result;
		}

		/// <summary>Headers csv</summary>
		public List<string> HeadersCsv { get; set; }
		/// <summary>Data</summary>
		public Hashtable Data { get; set; }
		/// <summary>Error messages</summary>
		public string ErrorMessages { get; set; }
		/// <summary>Error occurred id info</summary>
		public string ErrorOccurredIdInfo { get; set; }
	}
}
