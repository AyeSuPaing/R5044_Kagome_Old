/*
=========================================================================================================
  Module      : Import setting order (ImportSettingOrder.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// Import setting order
	/// </summary>
	public class ImportSettingOrder : ImportSettingBase
	{
		/// <summary>
		/// Fields update key
		/// </summary>
		private static List<string> FIELDS_UPDKEY = new List<string>
		{
			Constants.FIELD_ORDER_ORDER_ID,
		};

		/// <summary>
		/// Fields except
		/// </summary>
		private static List<string> FIELDS_EXCEPT = new List<string>
		{
			Constants.FIELD_ORDER_DATE_CHANGED,
			Constants.FIELD_ORDER_LAST_CHANGED,
		};

		/// <summary>
		/// Fields increased update
		/// </summary>
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string>();

		/// <summary>
		/// Fields necessary for insertupdate
		/// </summary>
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string>
		{
			Constants.FIELD_ORDER_ORDER_ID,
		};

		/// <summary>
		/// Fields necessary for delete
		/// </summary>
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string>
		{
			Constants.FIELD_ORDER_ORDER_ID,
		};

		/// <summary>
		/// Fields exclusion
		/// </summary>
		private static List<string> FIELDS_EXCLUSION = new List<string>();

		/// <summary>
		/// Constructor
		/// </summary>
		public ImportSettingOrder()
			: base(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.TABLE_ORDER,
				string.Empty,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
			this.ExclusionFields = FIELDS_EXCLUSION;
		}

		/// <summary>
		/// Convert data
		/// </summary>
		protected override void ConvertData()
		{
			if (this.FieldNamesForUpdateDelete.Any(field => (field == Constants.FIELD_ORDER_ORDER_ID)) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERCOUPON_COUPON_ID);
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
				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "OrderDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// Check necessary fields
			var errorMessages = CheckNecessaryFields(necessaryFields);

			// Validation input
			var errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = string.Empty;

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessages += ((errorMessages.Length != 0) ? Environment.NewLine : string.Empty) + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_ORDER_ORDER_ID);
			}

			// Error message storage
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}

		/// <summary>
		/// Check necessary fields
		/// </summary>
		/// <param name="fields">Fields</param>
		/// <returns>Error message</returns>
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
		/// <returns>Error message</returns>
		public override string CheckDataConsistency()
		{
			return string.Empty;
		}

		/// <summary>
		/// Create sql
		/// </summary>
		public override void CreateSql()
		{
			base.CreateSql();

			var queryDelete = new StringBuilder();
			queryDelete.Append(base.CreateDeleteSql(Constants.TABLE_ORDERITEM)).AppendLine(";")
				.Append(base.CreateDeleteSql(Constants.TABLE_ORDEROWNER)).AppendLine(";")
				.Append(base.CreateDeleteSql(Constants.TABLE_ORDERSHIPPING)).AppendLine(";")
				.Append(base.CreateDeleteSql(Constants.TABLE_ORDERCOUPON)).AppendLine(";")
				.Append(base.CreateDeleteSql(Constants.TABLE_ORDERPRICEBYTAXRATE)).AppendLine(";");
			this.DeleteSql += string.Format(";{0}{1}", Environment.NewLine, queryDelete);
		}
	}
}
