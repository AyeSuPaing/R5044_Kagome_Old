/*
=========================================================================================================
  Module      : ターゲットリスト情報取込設定クラス(ImportSettingTargetListData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.TargetList;
using w2.Common.Util;
using w2.Common.Sql;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// ターゲットリスト情報取込設定
	/// </summary>
	public class ImportSettingTargetListData : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_TARGETLISTDATA_USER_ID, Constants.FIELD_TARGETLISTDATA_MAIL_ADDR, Constants.FIELD_TARGETLISTDATA_DEPT_ID, Constants.FIELD_TARGETLISTDATA_MASTER_ID, Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_TARGETLISTDATA_USER_ID, Constants.FIELD_TARGETLISTDATA_MAIL_ADDR, Constants.FIELD_TARGETLISTDATA_DEPT_ID, Constants.FIELD_TARGETLISTDATA_MASTER_ID, Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingTargetListData()
			: base(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_TARGETLISTDATA,
			Constants.TABLE_WORKTARGETLISTDATA,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			// 何もしない
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			//------------------------------------------------------
			// データ変換
			//------------------------------------------------------
			foreach (string fieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[fieldName] = this.Data[fieldName].ToString().Trim();

				// 半角変換
				switch (fieldName)
				{
					case Constants.FIELD_TARGETLISTDATA_USER_ID:
					case Constants.FIELD_TARGETLISTDATA_MAIL_ADDR:
					case Constants.FIELD_TARGETLISTDATA_DEPT_ID:
					case Constants.FIELD_TARGETLISTDATA_MASTER_ID:
					case Constants.FIELD_TARGETLISTDATA_TARGET_KBN:
					case Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN:
						this.Data[fieldName] = StringUtility.ToHankaku(this.Data[fieldName].ToString());
						break;
				}
			}
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
					checkKbn = "TargetListDataInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder errorMessages = new StringBuilder();
			StringBuilder necessaryFieldsBuilder = new StringBuilder();
			foreach (string keyField in necessaryFields)
			{
				if (this.HeadersCsv.Contains(keyField) == false)
				{
					necessaryFieldsBuilder.Append((necessaryFieldsBuilder.Length != 0) ? "," : "");
					necessaryFieldsBuilder.Append(keyField);
				}
			}
			if (necessaryFieldsBuilder.Length != 0)
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(necessaryFieldsBuilder.ToString()).Append("」が必須です。");
			}

			// 入力チェック
			string errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_TARGETLISTDATA_TARGET_KBN);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_TARGETLISTDATA_MASTER_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_TARGETLISTDATA_DATA_NO);
			}

			// エラーメッセージ格納
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";	//整合性チェックしない
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			//------------------------------------------------------
			// 共通SQL文作成（※ImportSettingBaseで定義）
			//------------------------------------------------------
			base.CreateSql();
			StringBuilder customSqlBuilder = new StringBuilder();
			// ワークテーブルをTrancate
			customSqlBuilder.Append(this.TruncateWorkTableSql);

			this.InitlializeWorkTableSql = customSqlBuilder.ToString();
		}

		/// <summary>
		/// Update文作成
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <returns>Update文</returns>
		protected override string CreateUpdateSql(string tableName)
		{
			StringBuilder sqlResult = new StringBuilder();
			StringBuilder sqlFields = new StringBuilder();

			//------------------------------------------------------
			// Update文
			//------------------------------------------------------
			foreach (string fieldName in this.FieldNamesForUpdateDelete)
			{
				if (fieldName.Length == 0)
				{
					continue;
				}

				// 更新禁止フィールドではない？
				if (!this.ExceptionFields.Contains(fieldName))
				{
					if (sqlFields.Length != 0)
					{
						sqlFields.Append(",");
					}

					// 差分更新フィールド？
					if (this.IncreaseFields.Contains(fieldName))
					{
						string strRealField = fieldName.Substring("add_".Length);
						sqlFields.Append(strRealField).Append(" = ").Append(strRealField).Append(" + @").Append(fieldName);
					}
					else
					{
						sqlFields.Append(fieldName).Append(" = @").Append(fieldName);
					}
				}
			}

			// Update文組み立て
			sqlResult.Append(" UPDATE ").Append(tableName);
			sqlResult.Append(" SET ").Append(sqlFields);
			sqlResult.Append(CreateWhere());

			return sqlResult.ToString();
		}

		/// <summary>
		/// Insert/Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected override string CreateInsertUpdateSql(string strTableName)
		{
			// Insertのみ
			return CreateInsertSqlRaw(strTableName);
		}

		/// <summary>
		/// 実行前処理
		/// </summary>
		public override void OnBeforeExecute(string filePath)
		{
			this.IsSendMail = true;
			using (var fileReader = new StreamReader(filePath))
			{
				fileReader.ReadLine();
				var line = fileReader.ReadLine();
				if (string.IsNullOrEmpty(line) == false)
				{
					this.m_targetListId = StringUtility.SplitCsvLine(line)[4];
					this.m_deptId = StringUtility.SplitCsvLine(line)[3];
				}
			}
		}

		/// <summary>
		/// 完了時実行処理
		/// </summary>
		public override void OnCompleteExecute()
		{
			base.OnCompleteExecute();

			using (var sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				// ターゲットリスト件数を更新
				TargetListUtility.UpdateTargetListDataCountByTargetListData(this.m_deptId, this.m_targetListId, sqlAccessor);
				// ターゲットリストをMP管理画面へ表示
				TargetListUtility.UpdateTargetListToVisible(this.m_deptId, this.m_targetListId, sqlAccessor);
			}
		}

		///<summary>ターゲットID</summary>
		private string m_targetListId;
		///<summary>ターゲットID</summary>
		private string m_deptId;
	}
}
