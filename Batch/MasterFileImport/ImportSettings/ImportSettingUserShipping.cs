/*
=========================================================================================================
  Module      : ユーザー配送先情報取込設定クラス(ImportSettingUserShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;
using w2.App.Common;
using w2.App.Common.Util;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingUserShipping : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_USERSHIPPING_USER_ID, Constants.FIELD_USERSHIPPING_SHIPPING_NO };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_USERSHIPPING_DATE_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_USERSHIPPING_USER_ID,
			Constants.FIELD_USERSHIPPING_SHIPPING_NO, Constants.FIELD_USERSHIPPING_NAME, Constants.FIELD_USERSHIPPING_SHIPPING_ZIP,
			Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3,
			Constants.FIELD_USERSHIPPING_SHIPPING_TEL1, Constants.FIELD_USERSHIPPING_SHIPPING_NAME1, Constants.FIELD_USERSHIPPING_SHIPPING_NAME2,
			Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1, Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2 };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_USERSHIPPING_USER_ID, Constants.FIELD_USERSHIPPING_SHIPPING_NO };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingUserShipping(string shopId)
			: base(
			shopId,
			Constants.TABLE_USERSHIPPING,
			Constants.TABLE_WORKUSERSHIPPING,
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
					case Constants.FIELD_USERSHIPPING_USER_ID:
					case Constants.FIELD_USERSHIPPING_SHIPPING_NO:
					case Constants.FIELD_USERSHIPPING_SHIPPING_ZIP:
					case Constants.FIELD_USERSHIPPING_SHIPPING_TEL1:
						this.Data[fieldName] = StringUtility.ToHankaku(this.Data[fieldName].ToString());
						break;
				}

				// 全角変換
				switch (fieldName)
				{
					case Constants.FIELD_USERSHIPPING_SHIPPING_NAME:
					case Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1:
					case Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2:
					case Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3:
					case Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4:
					case Constants.FIELD_USERSHIPPING_SHIPPING_NAME1:
					case Constants.FIELD_USERSHIPPING_SHIPPING_NAME2:
					case Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME:
					case Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME:
						this.Data[fieldName] = StringUtility.ToZenkaku(this.Data[fieldName].ToString());
						break;
				}

				// 全角変換
				switch (fieldName)
				{
					case Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA:
					case Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1:
					case Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2:
						this.Data[fieldName] = DataInputUtility.ConvertToFullWidthKanaBySetting(this.Data[fieldName].ToString());
						break;
				}
			}

			//------------------------------------------------------
			// 動的に生成フィールドの追加
			//------------------------------------------------------
			if (this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NO) == false)
			{
				this.HeadersCsv.Add(Constants.FIELD_USERSHIPPING_SHIPPING_NO);
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERSHIPPING_SHIPPING_NO);
				this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NO] = "";
			}

			//------------------------------------------------------
			// フィールド結合
			// ■メモ
			// １．結合フィールドが指定されている場合、指定されているデータを優先する
			//     ＜例＞
			//     氏名（姓名）に指定があった場合、氏名（姓名）を優先
			// ２．結合対象のフィールドが１つでも指定がある場合、フィールド結合を行う
			//     ＜例＞
			//     氏名（姓）のみ指定があった場合、氏名（姓名）には氏名（姓）が登録される
			// ３．ヘッダフィールドに結合フィールドが存在しない場合、追加する
			//------------------------------------------------------
			// 配送先氏名（姓＋名）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME2))
				)
			{
				// フィールド結合
				this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NAME] =
					this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NAME1].ToString() + this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NAME2].ToString();

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERSHIPPING_SHIPPING_NAME);
				}
			}

			// 配送先氏名かな（姓＋名）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2))
				)
			{
				// フィールド結合
				this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA] =
					this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1].ToString() + this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2].ToString();

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA);
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
			string strCheckKbn = null;
			string importKbn = this.Data[Constants.IMPORT_KBN].ToString();
			List<string> lNecessaryFields = new List<string>();
			switch (importKbn)
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					strCheckKbn = "UserShippingInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "UserShippingDelete";
					lNecessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder sbErrorMessages = new StringBuilder();
			StringBuilder sbNecessaryFields = new StringBuilder();
			foreach (string strKeyField in lNecessaryFields)
			{
				if (this.HeadersCsv.Contains(strKeyField) == false)
				{
					sbNecessaryFields.Append((sbNecessaryFields.Length != 0) ? "," : "");
					sbNecessaryFields.Append(strKeyField);
				}
			}
			if (sbNecessaryFields.Length != 0)
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "");
				sbErrorMessages.Append("該当テーブルの更新にはフィールド「").Append(sbNecessaryFields.ToString()).Append("」が必須です。");
			}

			// 入力チェック
			var errorMessage = Validator.ChangeToDisplay(Validator.Validate(
				strCheckKbn,
				this.Data,
				"",
				(Constants.GLOBAL_OPTION_ENABLE
					? StringUtility.ToEmpty(this.Data[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE])
					: "")));
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID);
			}

			// エラーメッセージ格納
			if (sbErrorMessages.Length != 0)
			{
				this.ErrorMessages = sbErrorMessages.ToString();
			}
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
			// ワークテーブル初期化文
			this.InitlializeWorkTableSql = base.CreateInitlializeWorkTableSql();
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
		/// Insert/Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected new string CreateInsertUpdateSql(string strTableName)
		{
			StringBuilder sbResult = new StringBuilder();
			StringBuilder sbSqlSelect = new StringBuilder();

			//------------------------------------------------------
			// Insert/Update文作成
			//------------------------------------------------------
			// Select文作成
			if (this.UpdateKeys.Count != 0)
			{
				// Select文組み立て
				sbSqlSelect.Append(" SELECT @SELECT_COUNTS = COUNT(*)");
				sbSqlSelect.Append(" FROM ").Append(strTableName);
				sbSqlSelect.Append(CreateWhere());
			}

			// Insert/Update文組み立て
			sbResult.Append(" DECLARE @SELECT_COUNTS int");
			sbResult.Append(sbSqlSelect.ToString());
			sbResult.Append(" IF @SELECT_COUNTS = 0 ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(this.CreateInsertSql(strTableName));
			sbResult.Append("   END ");
			sbResult.Append(" ELSE ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(base.CreateUpdateSql(strTableName));
			sbResult.Append("   END ");

			return sbResult.ToString();
		}

		/// <summary>
		/// Insert文作成（※共通処理）
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert文</returns>
		protected string CreateInsertSql(string strTableName)
		{
			StringBuilder sbResult = new StringBuilder();
			StringBuilder sbMaxShippingNo = new StringBuilder();
			StringBuilder sbSqlFields = new StringBuilder();
			StringBuilder sbSqlValues = new StringBuilder();

			//------------------------------------------------------
			// Insert文作成
			//------------------------------------------------------
			foreach (string strFieldName in this.FieldNamesForInsert)
			{
				if (strFieldName.Length == 0)
				{
					continue;
				}

				// 更新禁止フィールドではない？
				if (this.ExceptionFields.Contains(strFieldName) == false)
				{
					if (sbSqlFields.Length != 0)
					{
						sbSqlFields.Append(",");
						sbSqlValues.Append(",");
					}

					// フィールド設定
					sbSqlFields.Append(strFieldName);
					sbSqlValues.Append("@");
					sbSqlValues.Append((strFieldName == Constants.FIELD_USERSHIPPING_SHIPPING_NO) ? "MAX_SHIPPING_NO" : strFieldName);
				}
			}

			// Insert文組み立て
			sbMaxShippingNo.Append(" SELECT  @MAX_SHIPPING_NO = ISNULL(MAX(shipping_no), 0) + 1");
			sbMaxShippingNo.Append(" FROM ").Append(strTableName);
			sbMaxShippingNo.Append(" WHERE user_id =");
			sbMaxShippingNo.Append(" @").Append(Constants.FIELD_USERSHIPPING_USER_ID);

			sbResult.Append(" DECLARE @MAX_SHIPPING_NO int");
			sbResult.Append(" SET @MAX_SHIPPING_NO =");
			sbResult.Append(" @").Append(Constants.FIELD_USERSHIPPING_SHIPPING_NO);
			sbResult.Append(" IF (@MAX_SHIPPING_NO is null)");
			sbResult.Append("   BEGIN ");
			sbResult.Append(sbMaxShippingNo.ToString());
			sbResult.Append("   END ");
			sbResult.Append(" INSERT ").Append(strTableName);
			sbResult.Append(" ( ");
			sbResult.Append(sbSqlFields.ToString());
			sbResult.Append(" ) VALUES ( ");
			sbResult.Append(sbSqlValues.ToString());
			sbResult.Append(" ) ");

			return sbResult.ToString();
		}
	}
}
