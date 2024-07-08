/*
=========================================================================================================
  Module      : コーディネート情報取込設定クラス(ImportSettingCoordinate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// コーディネート情報取込設定クラス
	/// </summary>
	public class ImportSettingCoordinate : ImportSettingBase
	{
		/// <summary>
		/// 更新キーフィールド
		/// </summary>
		private static readonly List<string> m_fieldsUpdateKey = new List<string>
		{
			Constants.FIELD_COORDINATE_COORDINATE_ID,
		};
		/// <summary>
		/// 更新禁止フィールド（SQL自動作成除外フィールド）
		/// </summary>
		private static readonly List<string> m_fieldsExcept = new List<string>
		{
			Constants.FIELD_COORDINATE_DATE_CHANGED,
			Constants.FIELD_COORDINATE_LAST_CHANGED,
		};
		/// <summary>
		/// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		/// </summary>
		private static readonly List<string> m_fieldsIncreasedUpdate = new List<string>();
		/// <summary>
		/// 必須フィールド（Insert/Update用）※更新キーフィールドも含めること
		/// </summary>
		private static readonly List<string> m_fieldsNecessaryForInsertUpdate = new List<string>
		{
			Constants.FIELD_COORDINATE_COORDINATE_ID,
		};
		/// <summary>
		/// 必須フィールド（Delete用）
		/// </summary>
		private static readonly List<string> m_fieldsNecessaryForDelete = new List<string>
		{
			Constants.FIELD_COORDINATE_COORDINATE_ID,
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingCoordinate()
			: base(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_COORDINATE,
			Constants.TABLE_WORKCOORDINATE,
			m_fieldsUpdateKey,
			m_fieldsExcept,
			m_fieldsIncreasedUpdate,
			m_fieldsNecessaryForInsertUpdate,
			m_fieldsNecessaryForDelete)
		{
		}

		///<summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		///</summary>
		protected override void ConvertData()
		{
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			// 入力チェック
			string checkKbn = null;
			var necessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "CoordinateInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "CoordinateDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// エラーメッセージ格納
			var errorMessages = CheckNecessaryFields(necessaryFields);
			var errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = string.Empty;
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessages += ((errorMessages.Length > 0) ? "\r\n" : string.Empty) + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_COORDINATE_COORDINATE_ID);
			}
			if (errorMessages.Length > 0)
			{
				this.ErrorMessages = errorMessages;
			}
		}

		/// <summary>
		/// 必須フィールドチェック
		/// </summary>
		/// <param name="fields">必須フィールド</param>
		/// <returns>エラーメッセージ</returns>
		protected string CheckNecessaryFields(List<string> fields)
		{
			var errorMessages = new StringBuilder();
			var necessaryFields = new StringBuilder();
			foreach (var keyField in fields)
			{
				if (this.HeadersCsv.Contains(keyField) == false)
				{
					necessaryFields.Append((necessaryFields.Length > 0) ? "," : string.Empty);
					necessaryFields.Append(keyField);
				}
			}
			if (necessaryFields.Length > 0)
			{
				errorMessages.Append((errorMessages.Length > 0) ? "\r\n" : string.Empty);
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(necessaryFields).Append("」が必須です。");
			}
			return errorMessages.ToString();
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";
		}
	}
}
