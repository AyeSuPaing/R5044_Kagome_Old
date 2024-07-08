/*
=========================================================================================================
  Module      : コーディネートアイテム取込設定クラス(ImportSettingCoordinateItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.Domain.Coordinate;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// コーディネートアイテム取込設定クラス
	/// </summary>
	public class ImportSettingCoordinateItem : ImportSettingBase
	{
		/// <summary>
		/// 更新キーフィールド
		/// </summary>
		private static readonly List<string> m_fieldsUpdateKey = new List<string>
		{
			Constants.FIELD_COORDINATEITEM_COORDINATE_ID,
			Constants.FIELD_COORDINATEITEM_ITEM_KBN,
			Constants.FIELD_COORDINATEITEM_ITEM_ID,
		};
		/// <summary>
		/// 更新禁止フィールド（SQL自動作成除外フィールド）
		/// </summary>
		private static readonly List<string> m_fieldsExcept = new List<string>
		{
			Constants.FIELD_COORDINATEITEM_ITEM_NO,
			Constants.FIELD_COORDINATEITEM_DATE_CHANGED,
			Constants.FIELD_COORDINATEITEM_LAST_CHANGED,
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
			Constants.FIELD_COORDINATEITEM_COORDINATE_ID,
			Constants.FIELD_COORDINATEITEM_ITEM_KBN,
			Constants.FIELD_COORDINATEITEM_ITEM_ID,
		};
		/// <summary>
		/// 必須フィールド（Delete用）
		/// </summary>
		private static readonly List<string> m_fieldsNecessaryForDelete = new List<string>
		{
			Constants.FIELD_COORDINATEITEM_COORDINATE_ID,
			Constants.FIELD_COORDINATEITEM_ITEM_KBN,
			Constants.FIELD_COORDINATEITEM_ITEM_ID,
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingCoordinateItem()
			: base(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_COORDINATEITEM,
			Constants.TABLE_WORKCOORDINATEITEM,
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
					checkKbn = "CoordinateItemInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "CoordinateItemDelete";
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
			string errorMessage = string.Empty;
			// 登録更新の場合、コーディネート存在チェック
			if (this.Data[Constants.IMPORT_KBN].ToString() == Constants.IMPORT_KBN_INSERT_UPDATE)
			{
				errorMessage = CheckCoordinateExist();
			}
			return errorMessage;
		}

		/// <summary>
		/// ユーザー存在チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckCoordinateExist()
		{
			var coordinate = new CoordinateService().Get(this.Data[Constants.FIELD_COORDINATE_COORDINATE_ID].ToString());
			if (coordinate == null)
			{
				return "コーディネートID:" + this.Data[Constants.FIELD_COORDINATE_COORDINATE_ID] + " のコーディネートが見つかりませんでした";
			}
			return string.Empty;
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			// 共通SQL文作成
			// ワークテーブル初期化文
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
		/// コーディネートアイテムワークテーブル初期化文作成
		/// </summary>
		/// <remarks>
		/// item_no(Identity)の項目があるため、INSERT SELECTではなくINSERT INTOを使用する
		/// </remarks>
		/// <returns>コーディネートアイテムワークテーブル初期化文</returns>
		protected new string CreateInitlializeWorkTableSql()
		{
			var result = new StringBuilder();

			// ワークテーブルをTruncate
			result.Append(this.TruncateWorkTableSql).Append(" \n");

			// INSERT INTO での追加カラム準備(更新キー)
			var updateKey = SetNecessaryKey();

			// ワークテーブルに対象テーブルの情報をInsert
			result.Append("INSERT INTO ").Append(this.WorkTableName).Append("(").Append(updateKey).Append(")");
			result.Append(" SELECT ").Append(updateKey).Append(" FROM ").Append(this.TableName).Append(" \n");

			return result.ToString();
		}

		/// <summary>
		/// 更新キーの格納
		/// </summary>
		/// <returns>更新キー</returns>
		protected string SetNecessaryKey()
		{
			var updateKeys = new StringBuilder();

			foreach (var updateKey in m_fieldsNecessaryForInsertUpdate)
			{
				if (updateKeys.Length != 0)
				{
					updateKeys.Append(",");
				}
				updateKeys.Append(updateKey);
			}
			return updateKeys.ToString();
		}
	}
}
