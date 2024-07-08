/*
=========================================================================================================
  Module      : 店舗管情報取込設定クラス (ImportSettingShopOperator.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Common.Logger;
using w2.App.Common.DataCacheController;
using w2.Domain.RealShop;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// 店舗管情報取込設定クラス
	/// </summary>
	public class ImportSettingShopOperator : ImportSettingBase
	{
		/// <summary>Area ID max length</summary>
		private const int AREA_ID_MAXLENGTH = 30;
		/// <summary>Real shop ID max length</summary>
		private const int REAL_SHOP_ID_MAXLENGTH = 30;
		/// <summary>更新キーフィールド</summary>
		private static readonly List<string> FIELDS_UPDKEY =
			new List<string>
			{
				Constants.FIELD_SHOPOPERATOR_SHOP_ID,
				Constants.FIELD_SHOPOPERATOR_OPERATOR_ID,
			};
		/// <summary>更新禁止フィールド（SQL自動作成除外フィールド）</summary>
		private static readonly List<string> FIELDS_EXCEPT =
			new List<string>
			{
				Constants.FIELD_SHOPOPERATOR_DATE_CHANGED,
				Constants.FIELD_SHOPOPERATOR_LAST_CHANGED,
				Constants.FIELD_REALSHOP_AREA_ID,
				Constants.FIELD_REALSHOP_REAL_SHOP_ID
			};
		/// <summary>差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）</summary>
		private static readonly List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		/// <summary>※更新キーフィールドも含めること</summary>
		private static readonly List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE =
			new List<string>
			{
				Constants.FIELD_SHOPOPERATOR_SHOP_ID,
				Constants.FIELD_SHOPOPERATOR_OPERATOR_ID
			};
		/// <summary>必須フィールド（Delete用）</summary>
		private static readonly List<string> FIELDS_NECESSARY_FOR_DELETE =
			new List<string>
			{
				Constants.FIELD_SHOPOPERATOR_SHOP_ID,
				Constants.FIELD_SHOPOPERATOR_OPERATOR_ID
			};
		/// <summary>カラム存在チェック除外フィールド</summary>
		private static readonly List<string> FIELDS_EXCLUSION =
			new List<string>
			{
				Constants.FIELD_REALSHOP_AREA_ID,
				Constants.FIELD_REALSHOP_REAL_SHOP_ID
			};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingShopOperator(string shopId)
			: base(
				shopId,
				Constants.TABLE_SHOPOPERATOR,
				Constants.TABLE_WORKSHOPOPERATOR,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
			this.ExclusionFields = FIELDS_EXCLUSION;
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return string.Empty;
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			var checkKbn = string.Empty;
			var necessaryFields = new List<string>();
			switch (StringUtility.ToEmpty(this.Data[Constants.IMPORT_KBN]))
			{
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "ShopOperatorInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "ShopOperatorDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			var necessaryFieldsString = new StringBuilder();
			foreach (var keyField in necessaryFields)
			{
				if (this.HeadersCsv.Contains(keyField)) continue;
				
				if (necessaryFieldsString.Length != 0) necessaryFieldsString.Append(",");
				necessaryFieldsString.Append(keyField);
			}

			var errorMessages = new StringBuilder();
			if (necessaryFieldsString.Length != 0)
			{
				if (errorMessages.Length != 0) errorMessages.AppendLine();
				errorMessages.AppendFormat(
					"該当テーブルの更新にはフィールド「{0}」が必須です",
					necessaryFieldsString.ToString());
			}

			// 入力チェック
			var errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = string.Empty;

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				if (errorMessages.Length != 0) errorMessages.AppendLine();
				errorMessages.Append(errorMessage);

				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID);
			}

			// Validate area ID
			var areaIdErrorMessages = ValidateAreaId();
			if (string.IsNullOrEmpty(areaIdErrorMessages) == false)
			{
				if (errorMessages.Length != 0) errorMessages.AppendLine();
				errorMessages.Append(areaIdErrorMessages);
			}
			// Validate real shop ID
			var realShopIdErrorMessages = ValidateRealShopId();
			if (string.IsNullOrEmpty(realShopIdErrorMessages) == false)
			{
				if (errorMessages.Length != 0) errorMessages.AppendLine();
				errorMessages.Append(realShopIdErrorMessages);
			}

			// エラーメッセージ格納
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			FileLogger.WriteDebug(string.Join("," , this.HeadersCsv));
			foreach (string strFieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[strFieldName] = this.Data[strFieldName].ToString().Trim();
				FileLogger.WriteDebug(strFieldName + "：" + this.Data[strFieldName]);
			}
			FileLogger.WriteDebug("------");

			AddRealShopIdWhenImportShopOperator();
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			// ワークテーブル初期化文
			this.InitlializeWorkTableSql = CreateInitlializeWorkTableSql();
			// Insert/Update文
			this.InsertUpdateSql = CreateInsertUpdateSql(this.TableName);
			// Delete文
			this.DeleteSql = CreateOperatorDeleteSql(this.TableName);
			// Insert/Update文 （ワークテーブル用）
			this.InsertUpdateWorkSql = CreateInsertUpdateSql(this.WorkTableName);
			// Delete文 （ワークテーブル用）
			this.DeleteWorkSql = CreateOperatorDeleteSql(this.WorkTableName);
		}

		/// <summary>
		/// Insert/Update文作成
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected override string CreateInsertUpdateSql(string tableName)
		{
			var sqlInsertUpdateBuilder = new StringBuilder();
			var insertOperatorAuthorityFields = new List<string>
			{
				Constants.FIELD_OPERATORAUTHORITY_SHOP_ID,
				Constants.FIELD_OPERATORAUTHORITY_OPERATOR_ID,
				Constants.FIELD_OPERATORAUTHORITY_CONDITION_TYPE,
				Constants.FIELD_OPERATORAUTHORITY_PERMISSION,
				Constants.FIELD_OPERATORAUTHORITY_CONDITION_VALUE
			};
			var hasRealShopIdData = string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_REAL_SHOP_ID])) == false;

			// Insert/Update文組み立て
			sqlInsertUpdateBuilder.Append(" DECLARE @SELECT_COUNTS int");
			sqlInsertUpdateBuilder.Append(" DECLARE @SELECT_COUNTS_TABLE_OPERATORAUTHORITY int");
			sqlInsertUpdateBuilder.AppendFormat(" DECLARE @delimiter CHAR(1) = '{0}'", Constants.MASTERFILEIMPORT_DELIMITER);

			// Select文作成
			if (this.UpdateKeys.Count != 0)
			{
				// Select文組み立て
				sqlInsertUpdateBuilder.Append(" SELECT @SELECT_COUNTS = COUNT(*)");
				sqlInsertUpdateBuilder.Append(" FROM ").Append(tableName);
				sqlInsertUpdateBuilder.Append(CreateWhere());
				if (hasRealShopIdData)
				{
					sqlInsertUpdateBuilder.Append(" SELECT @SELECT_COUNTS_TABLE_OPERATORAUTHORITY = COUNT(*)");
					sqlInsertUpdateBuilder.Append(" FROM ").Append(Constants.TABLE_OPERATORAUTHORITY);
					sqlInsertUpdateBuilder.Append(CreateWhere());
				}
			}

			sqlInsertUpdateBuilder.Append(" IF @SELECT_COUNTS = 0 ");
			sqlInsertUpdateBuilder.Append("   BEGIN ");
			sqlInsertUpdateBuilder.Append(CreateInsertSqlRaw(tableName));
			sqlInsertUpdateBuilder.Append("   END ");
			sqlInsertUpdateBuilder.Append(" ELSE ");
			sqlInsertUpdateBuilder.Append("   BEGIN ");
			sqlInsertUpdateBuilder.Append(CreateUpdateSql(tableName));
			sqlInsertUpdateBuilder.Append("   END ");

			// Insert update operator authority table
			if (hasRealShopIdData)
			{
				sqlInsertUpdateBuilder.Append(" DELETE ").Append(Constants.TABLE_OPERATORAUTHORITY);
				sqlInsertUpdateBuilder.Append(CreateWhere());

				sqlInsertUpdateBuilder.Append(" INSERT ");
				sqlInsertUpdateBuilder.Append(Constants.TABLE_OPERATORAUTHORITY);
				sqlInsertUpdateBuilder.Append(" (");
				sqlInsertUpdateBuilder.Append(string.Join(",", insertOperatorAuthorityFields));
				sqlInsertUpdateBuilder.Append(" ) SELECT ");
				sqlInsertUpdateBuilder.AppendFormat("@{0},@{1},", Constants.FIELD_SHOPOPERATOR_SHOP_ID, Constants.FIELD_SHOPOPERATOR_OPERATOR_ID);
				sqlInsertUpdateBuilder.Append("'REAL_SHOP','1',");
				sqlInsertUpdateBuilder.AppendFormat("LTRIM(RTRIM(SUBSTRING(@{0}, Number, CHARINDEX(@{1}, @{0} + @{1}, Number) - Number)))", Constants.FIELD_REALSHOP_REAL_SHOP_ID, "delimiter");
				sqlInsertUpdateBuilder.Append("  FROM  (SELECT  ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS Number FROM  master.dbo.spt_values) AS Numbers ");
				sqlInsertUpdateBuilder.AppendFormat("WHERE  Number <= LEN(@{0}) + 1 AND  SUBSTRING(@{1} + @{0}, Number, 1) = @{1}", Constants.FIELD_REALSHOP_REAL_SHOP_ID, "delimiter");

			}
			else if (hasRealShopIdData == false && string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_AREA_ID])))
			{
				sqlInsertUpdateBuilder.Append(" DELETE ").Append(Constants.TABLE_OPERATORAUTHORITY);
				sqlInsertUpdateBuilder.Append(CreateWhere());
			}
			FileLogger.WriteDebug(sqlInsertUpdateBuilder.ToString());
			return sqlInsertUpdateBuilder.ToString();
		}

		/// <summary>
		/// Delete文作成
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <returns>Delete文</returns>
		protected string CreateOperatorDeleteSql(string tableName)
		{
			if (this.UpdateKeys.Count == 0) return string.Empty;

			// Delete文組み立て
			var sqlDeleteBuilder = new StringBuilder();

			sqlDeleteBuilder.Append(" DELETE ").Append(tableName);
			sqlDeleteBuilder.Append(CreateWhere());
			sqlDeleteBuilder.Append(" DELETE ").Append(Constants.TABLE_OPERATORAUTHORITY);
			sqlDeleteBuilder.Append(CreateWhere());

			return sqlDeleteBuilder.ToString();
		}

		/// <summary>
		/// Add real shop ID when import shop operator
		/// </summary>
		private void AddRealShopIdWhenImportShopOperator()
		{
			var areaIdString = StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_AREA_ID]);
			if (Constants.REALSHOP_OPTION_ENABLED == false
				|| this.TableName != Constants.TABLE_SHOPOPERATOR
				|| string.IsNullOrEmpty(areaIdString))
			{
				return;
			}

			// Get real shop list
			var service = new RealShopService();
			var realShopList = new List<RealShopModel>();
			if (areaIdString.Contains(Constants.MASTERFILEIMPORT_DELIMITER))
			{
				var areaIds = areaIdString.Split(Constants.MASTERFILEIMPORT_DELIMITER[0]);
				foreach (var areaId in areaIds)
				{
					realShopList.AddRange(service.GetRealShops(areaId, null));
				}
			}
			else
			{
				realShopList = service.GetRealShops(areaIdString, null).ToList();
			}

			if (realShopList.Any() == false) return;

			// Get real shop IDs
			var realShopIdString = StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_REAL_SHOP_ID]);
			var realShopIds = realShopList
				.Select(realShopItem => realShopItem.RealShopId)
				.ToArray();

			realShopIdString += string.IsNullOrEmpty(realShopIdString)
				? string.Join(Constants.MASTERFILEIMPORT_DELIMITER, realShopIds)
				: Constants.MASTERFILEIMPORT_DELIMITER + string.Join(Constants.MASTERFILEIMPORT_DELIMITER, realShopIds);

			this.Data[Constants.FIELD_REALSHOP_REAL_SHOP_ID] =
				string.Join(
					Constants.MASTERFILEIMPORT_DELIMITER,
					realShopIdString.Split(Constants.MASTERFILEIMPORT_DELIMITER[0])
						.Distinct()
						.ToArray());
		}

		/// <summary>
		/// Validate area ID
		/// </summary>
		/// <returns>Error messages</returns>
		private string ValidateAreaId()
		{
			var areaIdString = StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_AREA_ID]);
			if (string.IsNullOrEmpty(areaIdString))
			{
				return string.Empty;
			}

			var errorMessages = new StringBuilder();
			var areaIds = areaIdString.Split(Constants.MASTERFILEIMPORT_DELIMITER[0]);
			var validRealShopAreaList = DataCacheControllerFacade
				.GetRealShopAreaCacheController()
				.GetRealShopAreaList()
				.Select(item => item.AreaId)
				.ToList();

			foreach (var areaId in areaIds)
			{
				if (areaId.Length > AREA_ID_MAXLENGTH)
				{
					errorMessages.AppendLine(
						MessageManager.GetMessages(
							MessageManager.INPUTCHECK_LENGTH_MAX,
							string.Format("エリアID（{0}）", areaId),
							AREA_ID_MAXLENGTH.ToString()));
				}
				else if (validRealShopAreaList.Contains(areaId) == false)
				{
					errorMessages.AppendFormat(
						"エリアID（{0}）は有効ではありません。 有効なエリアIDを入力して下さい。（{1}）",
						areaId,
						string.Join("、", validRealShopAreaList));
				}
			}

			return errorMessages.ToString();
		}

		/// <summary>
		/// Validate real shop ID
		/// </summary>
		/// <returns>Error messages</returns>
		private string ValidateRealShopId()
		{
			var realShopIdString = StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_REAL_SHOP_ID]);
			if (string.IsNullOrEmpty(realShopIdString))
			{
				return string.Empty;
			}

			var errorMessages = new StringBuilder();
			var realShopIds = realShopIdString.Split(Constants.MASTERFILEIMPORT_DELIMITER[0]);
			foreach (var realShopId in realShopIds)
			{
				if (realShopId.Length <= REAL_SHOP_ID_MAXLENGTH) continue;

				errorMessages.AppendLine(
					MessageManager.GetMessages(
						MessageManager.INPUTCHECK_LENGTH_MAX,
						string.Format("リアル店舗ID（{0}）", realShopId),
						REAL_SHOP_ID_MAXLENGTH.ToString()));
			}

			return errorMessages.ToString();
		}
	}
}
