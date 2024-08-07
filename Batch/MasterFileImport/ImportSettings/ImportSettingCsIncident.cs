﻿/*
=========================================================================================================
  Module      : Import setting cs incident (ImportSettingCsIncident.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// Import setting cs incident
	/// </summary>
	public class ImportSettingCsIncident : ImportSettingBase
	{
		/// <summary>Update key field</summary>
		private static List<string> FIELDS_UPDKEY = new List<string>
		{
			Constants.FIELD_CSINCIDENT_DEPT_ID,
			Constants.FIELD_CSINCIDENT_INCIDENT_ID,
		};

		/// <summary>Update prohibited field (SQL automatic creation exclusion field)</summary>
		private static List<string> FIELDS_EXCEPT = new List<string>
		{
			Constants.FIELD_CSINCIDENT_LAST_CHANGED,
		};

		/// <summary>Diff update field ("add_" + real field name "is sent as header)</summary>
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string>();

		/// <summary>Required fields (for insert / update)</summary>
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string>
		{
			Constants.FIELD_CSINCIDENT_DEPT_ID,
			Constants.FIELD_CSINCIDENT_INCIDENT_ID,
		};

		/// <summary>Required field (for delete)</summary>
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string>
		{
			Constants.FIELD_CSINCIDENT_DEPT_ID,
			Constants.FIELD_CSINCIDENT_INCIDENT_ID,
		};

		/// <summary>
		/// Constructor
		/// </summary>
		public ImportSettingCsIncident()
			: base(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.TABLE_CSINCIDENT,
				Constants.TABLE_WORKCSINCIDENT,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
		}

		/// <summary>
		/// Data conversion (various conversions, field joins, fixed value settings, etc.)
		/// </ summary>
		protected override void ConvertData()
		{
			// Data conversion
			foreach (var fieldName in this.HeadersCsv)
			{
				// Trim processing
				this.Data[fieldName] = this.Data[fieldName].ToString().Trim();
			}
		}

		/// <summary>
		/// Check data input
		/// </summary>
		protected override void CheckData()
		{
			var checkKbn = string.Empty;
			var necessaryFields = new List<string>();

			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "CsIncidentInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "CsIncidentDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// Required field check
			var errorMessages = CheckNecessaryFields(necessaryFields);

			// Check the input
			var errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = string.Empty;

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessages += ((errorMessages.Length != 0) ? Environment.NewLine : string.Empty) + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_CSINCIDENT_INCIDENT_ID);
			}

			// Store error message
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}

		/// <summary>
		/// Check necessary fields
		/// </summary>
		/// <param name="fields">Fields</param>
		/// <returns>Error messages</returns>
		protected string CheckNecessaryFields(List<string> fields)
		{
			var errorMessages = new StringBuilder();
			var necessaryFields = new StringBuilder();

			foreach (var keyField in fields)
			{
				if (this.HeadersCsv.Contains(keyField) == false)
				{
					necessaryFields.Append((necessaryFields.Length != 0) ? "," : string.Empty);
					necessaryFields.Append(keyField);
				}
			}

			if (necessaryFields.Length != 0)
			{
				errorMessages.Append((errorMessages.Length != 0) ? Environment.NewLine : string.Empty);
				errorMessages.AppendFormat("該当テーブルの更新にはフィールド「{0}」が必須です。", necessaryFields);
			}

			return errorMessages.ToString();
		}

		/// <summary>
		/// Check data consistency
		/// </summary>
		/// <returns>Error messages</returns>
		public override string CheckDataConsistency()
		{
			return string.Empty;
		}
	}
}
