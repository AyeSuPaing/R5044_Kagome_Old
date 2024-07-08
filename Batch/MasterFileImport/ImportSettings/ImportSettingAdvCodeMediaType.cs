/*
=========================================================================================================
  Module      : Import Setting AdvCode Media Type(ImportSettingAdvCodeMediaType.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingAdvCodeMediaType : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_ADVCODEMEDIATYPE_DATE_CHANGED, Constants.FIELD_ADVCODEMEDIATYPE_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID, Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME, Constants.FIELD_ADVCODEMEDIATYPE_DISPLAY_ORDER };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingAdvCodeMediaType()
			: base(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_ADVCODEMEDIATYPE,
			Constants.TABLE_WORKADVCODEMEDIATYPE,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			// 何もしない
		}

		///<summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		///</summary>
		protected override void ConvertData()
		{
			foreach (string fieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[fieldName] = this.Data[fieldName].ToString().Trim();

				// 半角変換
				switch (fieldName)
				{
					case Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID:
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
					checkKbn = "AdvCodeMediaTypeInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "AdvCodeMediaTypeDelete";
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
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID);
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
			// 共通SQL文作成（※ImportSettingBaseで定義）
			//------------------------------------------------------
			base.CreateSql();
		}
	}
}