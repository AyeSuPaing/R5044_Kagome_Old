/*
=========================================================================================================
  Module      : 広告コード情報取込設定クラス(ImportSettingAdvCode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingAdvCode : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_ADVCODE_DEPT_ID, Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_ADVCODE_ADVCODE_NO, Constants.FIELD_ADVCODE_DATE_CHANGED, Constants.FIELD_ADVCODE_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_ADVCODE_DEPT_ID, Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE,	Constants.FIELD_ADVCODE_MEDIA_NAME,	Constants.FIELD_ADVCODE_VALID_FLG };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_ADVCODE_DEPT_ID, Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingAdvCode()
			: base(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_ADVCODE,
			Constants.TABLE_WORKADVCODE,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			this.InsertUpdateNecessaryFields.Add(Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID);
		}

		///<summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		///</summary>
		protected override void ConvertData()
		{
			// 何もしない
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			//------------------------------------------------------
			// 入力チェック
			//------------------------------------------------------
			string checkKbn = null;
			List<string> necessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "AdvCodeInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "AdvCodeDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			string errorMessages = CheckNecessaryFields(necessaryFields);

			// 入力チェック
			string errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = "";

			if (errorMessage != "")
			{
				errorMessages += ((errorMessages.Length != 0) ? "\r\n" : "") + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_ADVCODE_MEDIA_NAME);
			}
			// エラーメッセージ格納
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}

		/// <summary>
		/// 必須フィールドチェック
		/// </summary>
		/// <param name="fields">必須フィールド</param>
		/// <returns>エラーメッセージ</returns>
		protected string CheckNecessaryFields(List<string> fields)
		{
			StringBuilder errorMessages = new StringBuilder();
			StringBuilder necessaryFields = new StringBuilder();
			foreach (string keyField in fields)
			{
				if (this.HeadersCsv.Contains(keyField) == false)
				{
					necessaryFields.Append((necessaryFields.Length != 0) ? "," : "");
					necessaryFields.Append(keyField);
				}
			}
			if (necessaryFields.Length != 0)
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(necessaryFields.ToString()).Append("」が必須です。");
			}
			return errorMessages.ToString();
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";	// 整合性チェックしない
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			//------------------------------------------------------
			// 共通SQL文作成
			//------------------------------------------------------
			// 広告コードワークテーブル初期化文
			this.InitlializeWorkTableSql = CreateInitlializeWorkTableSql();
			// Insert/Update文 
			this.InsertUpdateSql = CreateInsertUpdateSql(this.TableName);
			// Delete文
			this.DeleteSql = CreateDeleteSql(this.TableName);
			// Insert/Update文 （ワークテーブル用）
			this.InsertUpdateWorkSql = CreateInsertUpdateSql(this.WorkTableName);
			// Delete文 （ワークテーブル用）			
			this.DeleteWorkSql = CreateDeleteSql(this.WorkTableName);
		}

		/// <summary>
		/// 広告コードワークテーブル初期化文作成
		/// </summary>
		/// <remarks>
		/// advcode_no(Identity)の項目があるため、INSERT SELECTではなくINSERT INTOを使用する
		/// </remarks>
		/// <returns>広告コードワークテーブル初期化文</returns>
		protected new string CreateInitlializeWorkTableSql()
		{
			StringBuilder result = new StringBuilder();

			// ワークテーブルをTruncate
			result.Append(this.TruncateWorkTableSql).Append(" \n");

			// INSERT INTO での追加カラム準備(更新キー)
			string updateKey = SetNecessaryKey();

			// ワークテーブルに対象テーブルの情報をInsert
			result.Append("INSERT INTO ").Append(this.WorkTableName).Append("(").Append(updateKey.ToString()).Append(")");
			result.Append(" SELECT ").Append(updateKey.ToString()).Append(" FROM ").Append(this.TableName).Append(" \n");

			return result.ToString();
		}

		/// <summary>
		/// 更新キーの格納
		/// </summary>
		/// <returns>更新キー</returns>
		protected string SetNecessaryKey()
		{
			StringBuilder updateKeys = new StringBuilder();

			foreach (string updateKey in FIELDS_NECESSARY_FOR_INSERTUPDATE)
			{
				if (updateKeys.Length != 0)
				{
					updateKeys.Append(",");
				}
				updateKeys.Append(updateKey);
			}
			return updateKeys.ToString() ;
		}
	}
}