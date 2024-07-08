/*
=========================================================================================================
  Module      : Interface Import Setting (IImportSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;

namespace w2.Commerce.Batch.CrossPointCooperation.ImportSettings
{
	/// <summary>
	/// Interface import setting
	/// </summary>
	public interface IImportSetting
	{
		/// <summary>
		/// Convert and check
		/// </summary>
		/// <param name="data">Data</param>
		void ConvertAndCheck(Hashtable data);

		/// <summary>
		/// Set field names
		/// </summary>
		/// <param name="headers">Headers</param>
		void SetFieldNames(List<string> headers);

		/// <summary>Headers csv</summary>
		List<string> HeadersCsv { get; set; }
		/// <summary>Data</summary>
		Hashtable Data { get; set; }
		/// <summary>Error messages</summary>
		string ErrorMessages { get; set; }
		/// <summary>Error occurred id info</summary>
		string ErrorOccurredIdInfo { get; set; }
	}
}
