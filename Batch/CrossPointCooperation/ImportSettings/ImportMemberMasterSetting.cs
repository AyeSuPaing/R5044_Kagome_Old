/*
=========================================================================================================
  Module      : Import Member Master Setting (ImportMemberMasterSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.CrossPointCooperation.ImportSettings
{
	/// <summary>
	/// Import Member Master Setting
	/// </summary>
	public class ImportMemberMasterSetting : ImportSettingBase
	{
		/// <summary>
		/// Convert data
		/// </summary>
		protected override void ConvertData()
		{
			// Data conversion
			foreach (var fieldName in this.HeadersCsv)
			{
				// Trim processing
				this.Data[fieldName] = this.Data[fieldName].ToString().Trim();
			}
		}
	}
}
