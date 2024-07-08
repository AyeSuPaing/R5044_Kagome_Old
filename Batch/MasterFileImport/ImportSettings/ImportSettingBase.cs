/*
=========================================================================================================
  Module      : マスタ取込設定クラス(ImportSettingBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using System.Xml;
using w2.Common.Util;
using w2.Common.Sql;
using w2.App.Common.ProductDefaultSetting;
using w2.App.Common;
using System.Xml.Linq;
using w2.App.Common.Product;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public abstract class ImportSettingBase
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected ImportSettingBase()
		{
			// 何もしない
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="tableName">テーブル名</param>
		/// <param name="workTableName">ワークテーブル名</param>
		/// <param name="updateKeys">更新キーリスト</param>
		/// <param name="exceptionFields">禁止フィールドリスト</param>
		/// <param name="increaseFields">差分更新フィールドリスト</param>
		/// <param name="insertUpdateNecessaryFields">新規・更新時必須フィールドリスト</param>
		/// <param name="deleteNecessaryFields">削除時必須フィールドリスト</param>
		protected ImportSettingBase(
			string shopId,
			string tableName,
			string workTableName,
			List<string> updateKeys,
			List<string> exceptionFields,
			List<string> increaseFields,
			List<string> insertUpdateNecessaryFields,
			List<string> deleteNecessaryFields)
		{
			//------------------------------------------------------
			// 初期化
			//------------------------------------------------------
			this.ShopId = shopId;
			this.TableName = tableName;
			this.WorkTableName = workTableName;
			this.UpdateKeys = updateKeys;
			this.ExceptionFields = exceptionFields;
			this.IncreaseFields = increaseFields;
			this.InsertUpdateNecessaryFields = insertUpdateNecessaryFields;
			this.DeleteNecessaryFields = deleteNecessaryFields;

			// 初期設定情報
			this.ProductDefaultSetting = new ProductDefaultSetting();
			this.ProductDefaultSetting.LoadSetting(this.ShopId);

			// SQLパラメータ型定義（データ型チェック用）
			this.SetInputParamTypes();

			// SQLパラメータ型定義（SqlStatement用）
			var paramXml = XDocument.Load(Constants.PHYSICALDIRPATH_PARAMETERSFILE);
			this.InputParamDefines = SqlStatement.GetParamDefinition(paramXml.Root, "./" + tableName + "/Input");
			if (this.InputParamDefines.Any(param => param.Key == Constants.FIELD_IMPORT_LAST_CHANGED) == false)
			{
				this.InputParamDefines.Add(
					Constants.FIELD_IMPORT_LAST_CHANGED,
					new SqlStatement.SqlParam
					{
						Name = Constants.FIELD_IMPORT_LAST_CHANGED,
						Type = SqlDbType.NVarChar,
						Size = 20
					});
			}

			//送信メール設定
			this.IsSendMail = true;
		}

		/// <summary>
		/// データ変換＆入力チェック
		/// </summary>
		/// <param name="htData">データ</param>
		/// <param name="strTableName">マスタ名（ディレクトリ名）</param>
		public void ConvertAndCheck(Hashtable htData)
		{
			// 初期化
			this.Data = new Hashtable(htData);
			this.ErrorMessages = "";
			this.WarningMessages = "";
			this.ErrorOccurredIdInfo = "";

			// 画像削除以外の場合、テーブルカラム存在チェックを行う
			if (this.TableName != Constants.ACTION_KBN_DELETE_PRODUCT_IMAGE)
			{
				// テーブルカラム存在チェック
				ExistsColumn();
				// エラーの場合
				if (this.ErrorMessages != "")
				{
					return;	// 処理を抜ける
				}
			}

			// データ変換（各取込設定クラスで実装必須）
			ConvertData();

			// Remove fields which enable flag is false
			// (Execute after ConvertData())
			RemoveData();

			// グローバル対応の場合、日付のバリデータのため、一時的カルチャーを変更
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			if (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
			}

			// 入力チェック（各取込設定クラスで実装必須）
			CheckData();
			// カルチャーを復元
			Thread.CurrentThread.CurrentCulture = currentCulture;

			// エラーの場合
			if (this.ErrorMessages != "")
			{
				return;	// 処理を抜ける
			}

			//------------------------------------------------------
			// 後処理
			//------------------------------------------------------
			// デフォルト値設定
			SetDefaultValue();

			// 空文字入力が許されない値（datetime, intなど）のnull変換
			foreach (string strFieldName in ((Hashtable)this.Data.Clone()).Keys)	// foreach内でデータ書換が発生するためコピー
			{
				if ((StringUtility.ToEmpty(this.Data[strFieldName]) == "")
					&& (Constants.FIELD_NULLABLE_COLUMNS.Contains((string)this.InputParamTypes[strFieldName])))
				{
					this.Data[strFieldName] = DBNull.Value;
				}

				// InputParamTypesに含まれていない場合はスル―
				if (this.InputParamTypes.ContainsKey(strFieldName) == false) { continue; }

				// 「yyyy/MM/dd HH:mm:ss」フォーマットで日付変換
				if (Constants.GLOBAL_OPTION_ENABLE
					&& (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
					&& (strFieldName != Constants.IMPORT_KBN)
					&& (StringUtility.ToEmpty(this.Data[strFieldName]) != "")
					&& (this.InputParamTypes[strFieldName] == "datetime"))
				{
					this.Data[strFieldName] = DateTime.Parse(
						(string)this.Data[strFieldName],
						new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)).ToString("yyyy/MM/dd HH:mm:ss");
				}
			}

			// last_changed列が未指定の場合は既定値
			if (this.Data.ContainsKey(Constants.FIELD_IMPORT_LAST_CHANGED) == false)
			{
				this.Data.Add(Constants.FIELD_IMPORT_LAST_CHANGED, Constants.IMPORT_LAST_CHANGED);
			}
		}

		/// <summary>
		/// Variation id is start with product id
		/// </summary>
		/// <param name="errorMessage">Error massage</param>
		protected string CheckVariationId(string errorMessage)
		{
			if (this.Data.ContainsKey(Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)
				&& this.Data.ContainsKey(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)
				&& (this.Data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString().StartsWith(this.Data[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID].ToString()) == false))
			{
				return string.Format("{0}{1}", (string.IsNullOrEmpty(errorMessage) ? string.Empty : "\r\n"), "商品バリエーションIDの先頭が商品IDではありません。");
			}

			return string.Empty;
		}

		/// <summary>
		/// テーブルカラム存在チェック
		/// </summary>
		private void ExistsColumn()
		{
			// テーブルカラム取得
			DataView tableColumns;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("SELECT * FROM sys.columns WHERE object_id = object_id('" + this.TableName + "')"))
			{
				tableColumns = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}
			List<string> columns = new List<string>();
			foreach (DataRowView column in tableColumns) columns.Add(column["name"].ToString());

			// 存在しないカラムの場合、エラーメッセージをセット
			List<string> error = new List<string>();
			foreach (string column in this.Data.Keys)
			{
				// 取込区分の場合、スルー
				if (Constants.IMPORT_KBN == column) continue;

				// 差分更新フィールドの場合、スルー
				if (this.IncreaseFields.Contains(column) == true) continue;

				// カラム存在チェック除外フィールドの場合、スルー
				if ((this.ExclusionFields != null)
					&& (this.ExclusionFields.Any(field => field == column))) continue;

				if (columns.Contains(column) == false)
				{
					error.Add("該当テーブルにフィールド「" + column + "」は存在しません。");
				}
			}
			if (error.Count != 0)
			{
				this.ErrorMessages = string.Join("\r\n", error.ToArray());
			}
		}

		/// <summary>
		/// Remove fields which enable flag is false
		/// </summary>
		private void RemoveData()
		{
			switch (this.TableName)
			{
				// 商品情報
				case Constants.TABLE_PRODUCT:
					RemoveData(MasterFieldSetting.GetProductFieldsDisable());
					break;

				// 商品バリエーション情報
				case Constants.TABLE_PRODUCTVARIATION:
					RemoveData(MasterFieldSetting.GetProductVariationFieldsDisable());
					break;

				// 商品在庫情報
				case Constants.TABLE_PRODUCTSTOCK:
					RemoveData(MasterFieldSetting.GetProductStockFieldsDisable());
					break;

				// 商品カテゴリ情報
				case Constants.TABLE_PRODUCTCATEGORY:
					RemoveData(MasterFieldSetting.GetProductCategoryFieldsDisable());
					break;

				// ユーザー情報
				case Constants.TABLE_USER:
					RemoveData(MasterFieldSetting.GetUserFieldsDisable());
					break;

				// 商品価格マスタ
				case Constants.TABLE_PRODUCTPRICE:
					RemoveData(MasterFieldSetting.GetProductPriceFieldsDisable());
					break;

				// リアル店舗
				case Constants.TABLE_REALSHOP:
					RemoveData(MasterFieldSetting.GetRealShopFieldsDisable());
					break;
			}
		}

		/// <summary>
		/// Remove fields which enable flag is false from data
		/// </summary>
		/// <param name="fieldRemoves"> List field remove</param>
		private void RemoveData(List<string> fieldRemoves)
		{
			foreach (var field in fieldRemoves)
			{
				RemoveFieldData(field);
			}
		}

		/// <summary>
		/// Remove field which enable flag is false from data
		/// </summary>
		/// <param name="fieldName"> Field Name remove</param>
		private void RemoveFieldData(string fieldName)
		{
			this.FieldNamesForUpdateDelete.Remove(fieldName);	// Remove from FieldNamesForUpdateDelete
			this.Data.Remove(fieldName);						// Remove from Data
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		/// <remarks>各取込設定クラスで実装必須</remarks>
		protected abstract void ConvertData();

		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <remarks>各取込設定クラスで実装必須</remarks>
		protected abstract void CheckData();

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>各取込設定クラスで実装必須</remarks>
		public abstract string CheckDataConsistency();

		/// <summary>
		/// SQLパラメータ型定義設定
		/// </summary>
		protected virtual void SetInputParamTypes()
		{
			//------------------------------------------------------
			// SQLパラメータ設定ファイル読込
			//------------------------------------------------------
			try
			{
				XDocument paramXml = XDocument.Load(Constants.PHYSICALDIRPATH_PARAMETERSFILE);
				var inputParamTypes = from param in paramXml.Descendants("Input")
									  where param.Parent.Name == this.TableName
									  select new
									  {
										  name = param.Attribute("Name").Value,
										  type = param.Attribute("Type").Value
									  };

				this.InputParamTypes = inputParamTypes.ToDictionary(input => input.name, input => input.type);
			}
			catch (Exception ex)
			{
				throw new System.ApplicationException("Parameters.xmlファイルの読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// デフォルト値設定
		/// </summary>
		private void SetDefaultValue()
		{
			//------------------------------------------------------
			// デフォルト値設定処理
			//------------------------------------------------------
			if (this.ProductDefaultSetting.Tables.ContainsKey(this.TableName))
			{
				foreach (KeyValuePair<string, ProductDefaultSettingField> kvpSettingField in this.ProductDefaultSetting.Tables[this.TableName].Fields)
				{
					var strFieldName = kvpSettingField.Key;

					// 管理側のみで使用している商品付帯情報の設定処理は行わない
					var productOptionSettingKey = ProductOptionSettingHelper.GetAllProductOptionSettingKeys();
					if (productOptionSettingKey.Any(posName => posName.Equals(strFieldName)))
					{
						continue;
					}

					// デフォルト値を設定する対象フィールドのみ、値を変更する
					if (this.Data.Contains(strFieldName) == false)
					{
						var strDefaultValue = kvpSettingField.Value.Default;

						// 対象フィールドのデフォルト値を利用しない場合は何もしない
						if (strDefaultValue != null)
						{
							// datetime, intなど空文字入力が許されない値の時はnullを格納
							if ((strDefaultValue == "")
								&& (Constants.FIELD_NULLABLE_COLUMNS.Contains(StringUtility.ToEmpty(this.InputParamTypes[strFieldName]))))
							{
								this.Data[strFieldName] = DBNull.Value;
							}
							else
							{
								this.Data[strFieldName] = strDefaultValue;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// ヘッダフィールド設定
		/// </summary>
		public void SetFieldNames(List<string> lHeaders)
		{
			// ※テーブル操作（TBL）削除
			List<string> lTempHeaders = new List<string>(lHeaders);
			lTempHeaders.Remove(Constants.IMPORT_HEADER_FIELD);
			// ヘッダフィールド（CSV指定）設定
			this.HeadersCsv = new List<string>(lTempHeaders);
			// ヘッダフィールド設定
			this.FieldNamesForUpdateDelete = new List<string>(lTempHeaders);
		}

		/// <summary>
		/// Insert用ヘッダフィールド設定
		/// </summary>
		public void SetFieldNamesForInsert()
		{
			// ヘッダフィールドをコピー
			this.FieldNamesForInsert = new List<string>(this.FieldNamesForUpdateDelete);

			//------------------------------------------------------
			// ヘッダフィールド追加
			//------------------------------------------------------
			// 対象テーブルがデフォルト値設定情報を保持している？
			if (this.ProductDefaultSetting.Tables.ContainsKey(this.TableName))
			{
				foreach (KeyValuePair<string, ProductDefaultSettingField> kvpSettingField in this.ProductDefaultSetting.Tables[this.TableName].Fields)
				{
					var strFieldName = kvpSettingField.Key;

					// ヘッダフィールドに指定されていない場合？
					if (this.FieldNamesForInsert.Contains(strFieldName) == false)
					{
						// 管理側のみで使用している商品付帯情報の設定処理は行わない
						var productOptionSettingKey = ProductOptionSettingHelper.GetAllProductOptionSettingKeys();
						if (productOptionSettingKey.Any(posName => posName.Equals(strFieldName)))
						{
							continue;
						}

						// デフォルト値が設定されていればフィールドを追加
						if (kvpSettingField.Value.Default != null)
						{
							this.FieldNamesForInsert.Add(strFieldName);
						}
					}
				}
			}
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		/// <remarks>各取込設定クラスでコールしています</remarks>
		public virtual void CreateSql()
		{
			//------------------------------------------------------
			// 共通SQL文作成
			//------------------------------------------------------
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
		/// Insert文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert文</returns>
		private string CreateInsertSql(string strTableName)
		{
			StringBuilder sbResult = new StringBuilder();
			StringBuilder sbSqlSelect = new StringBuilder();

			//------------------------------------------------------
			// Insert文作成
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
			sbResult.Append(CreateInsertSqlRaw(strTableName));
			sbResult.Append("   END ");

			return sbResult.ToString();
		}

		/// <summary>
		/// Insert文作成（※共通処理）
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert文</returns>
		protected virtual string CreateInsertSqlRaw(string strTableName)
		{
			StringBuilder sbResult = new StringBuilder();
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
					// Check if data of date_created is not empty
					if (strFieldName == Constants.FIELD_IMPORT_DATE_CREATED
						&& string.IsNullOrEmpty(this.Data[strFieldName].ToString()))
					{
						continue;
					}
					if (sbSqlFields.Length != 0)
					{
						sbSqlFields.Append(",");
						sbSqlValues.Append(",");
					}

					// 差分更新フィールド？
					if (this.IncreaseFields.Contains(strFieldName))
					{
						string strRealField = strFieldName.Substring("add_".Length);
						sbSqlFields.Append(strRealField);
						sbSqlValues.Append("@").Append(strFieldName);
					}
					else
					{
						sbSqlFields.Append(strFieldName);
						sbSqlValues.Append("@").Append(strFieldName);
					}
				}
			}

			// 更新者設定（※更新禁止フィールドに"last_changed"があれば更新）
			if (this.ExceptionFields.Contains(Constants.FIELD_IMPORT_LAST_CHANGED))
			{
				sbSqlFields.AppendFormat(",{0}", Constants.FIELD_IMPORT_LAST_CHANGED);
				sbSqlValues.AppendFormat(",@{0}", Constants.FIELD_IMPORT_LAST_CHANGED);
			}

			// Insert文組み立て
			sbResult.Append("INSERT ").Append(strTableName);
			sbResult.Append(" (");
			sbResult.Append(sbSqlFields.ToString());
			sbResult.Append(" ) VALUES ( ");
			sbResult.Append(sbSqlValues.ToString());
			sbResult.Append(" ) ");

			return sbResult.ToString();
		}

		/// <summary>
		/// Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Update文</returns>
		protected virtual string CreateUpdateSql(string strTableName)
		{
			StringBuilder sbResult = new StringBuilder();
			StringBuilder sbSqlFields = new StringBuilder();

			//------------------------------------------------------
			// Update文
			//------------------------------------------------------
			foreach (string strFieldName in this.FieldNamesForUpdateDelete)
			{
				if (strFieldName.Length == 0 || this.UpdateKeys.Contains(strFieldName))
				{
					continue;
				}

				// 更新禁止フィールドではない？
				if (!this.ExceptionFields.Contains(strFieldName))
				{
					// Check if data of date_created is not empty
					if (strFieldName == Constants.FIELD_IMPORT_DATE_CREATED
						&& string.IsNullOrEmpty(this.Data[strFieldName].ToString()))
					{
						continue;
					}
					if (sbSqlFields.Length != 0)
					{
						sbSqlFields.Append(",");
					}

					// 差分更新フィールド？
					if (this.IncreaseFields.Contains(strFieldName))
					{
						string strRealField = strFieldName.Substring("add_".Length);
						sbSqlFields.Append(strRealField).Append(" = ").Append(strRealField).Append(" + @").Append(strFieldName);
					}
					else
					{
						sbSqlFields.Append(strFieldName).Append(" = @").Append(strFieldName);
					}
				}
			}

			// 更新日（※更新禁止フィールドに"date_changed"があれば更新）
			if (this.ExceptionFields.Contains(Constants.FIELD_IMPORT_DATE_CHANGED))
			{
				sbSqlFields.Append((sbSqlFields.Length > 0) ? "," : string.Empty)
					.Append("date_changed =  GETDATE()");
			}

			// 更新者設定（※更新禁止フィールドに"last_changed"があれば更新）
			if (this.ExceptionFields.Contains(Constants.FIELD_IMPORT_LAST_CHANGED))
			{
				sbSqlFields.Append((sbSqlFields.Length > 0) ? "," : string.Empty)
					.AppendFormat("{0} = @{0}", Constants.FIELD_IMPORT_LAST_CHANGED);
			}

			// Update文組み立て
			sbResult.Append(" UPDATE ").Append(strTableName);
			sbResult.Append(" SET ").Append(sbSqlFields.ToString());
			sbResult.Append(CreateWhere());

			return sbResult.ToString();
		}

		/// <summary>
		/// Insert/Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected virtual string CreateInsertUpdateSql(string strTableName)
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
			sbResult.Append(CreateInsertSqlRaw(strTableName));
			sbResult.Append("   END ");
			sbResult.Append(" ELSE ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(CreateUpdateSql(strTableName));
			sbResult.Append("   END ");

			return sbResult.ToString();
		}

		/// <summary>
		/// Delete文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Delete文</returns>
		protected string CreateDeleteSql(string strTableName)
		{
			StringBuilder sbResult = new StringBuilder();

			//------------------------------------------------------
			// Delete文作成
			//------------------------------------------------------
			if (this.UpdateKeys.Count != 0)
			{
				// Delete文組み立て
				sbResult.Append(" DELETE ").Append(strTableName);
				sbResult.Append(CreateWhere());
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// ワークテーブル初期化文作成
		/// </summary>
		/// <returns>ワークテーブル初期化文</returns>
		protected string CreateInitlializeWorkTableSql()
		{
			StringBuilder sbResult = new StringBuilder();

			// ワークテーブルをTrancate
			sbResult.Append(this.TruncateWorkTableSql).Append(" \n");
			// ワークテーブルに対象テーブルの情報をInsert
			sbResult.Append("INSERT ").Append(this.WorkTableName).Append(" SELECT * FROM ").Append(this.TableName).Append(" \n");

			return sbResult.ToString();
		}

		/// <summary>
		/// パラメータ付きのWHERE文生成
		/// </summary>
		/// <returns>WHERE文</returns>
		protected string CreateWhere()
		{
			StringBuilder sbResult = new StringBuilder();

			foreach (string strFieldName in this.UpdateKeys)
			{
				if (sbResult.Length != 0)
				{
					sbResult.Append(" AND ");
				}
				sbResult.Append(strFieldName).Append(" = @").Append(strFieldName);
			}
			// 先頭にWHERE付加
			sbResult.Insert(0, " WHERE ");

			return sbResult.ToString();
		}


		/// <summary>
		/// ID文字列作成
		/// </summary>
		/// <param name="filedName"></param>
		/// <returns></returns>
		protected string CreateIdString(string filedName)
		{
			if (this.Data.ContainsKey(filedName) == false) return "";
			return string.Format(" {0}={1} ", filedName, this.Data[filedName]);
		}

		/// <summary>
		/// 実行前処理
		/// </summary>
		public virtual void OnBeforeExecute(string filePath)
		{
			this.ExecuteDateTimeBegin = DateTime.Now;
		}

		/// <summary>
		/// 完了時実行処理
		/// </summary>
		public virtual void OnCompleteExecute()
		{
			this.ExecuteDateTimeComplete = DateTime.Now;
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>対象テーブル名</summary>
		public string TableName { get; set; }
		/// <summary>対象ワークテーブル名</summary>
		public string WorkTableName { get; set; }
		/// <summary>ヘッダフィールド（CSV指定）</summary>
		public List<string> HeadersCsv { get; set; }
		/// <summary>ヘッダフィールド（Update/Delete用）</summary>
		public List<string> FieldNamesForUpdateDelete { get; set; }
		/// <summary>ヘッダフィールド（Insert用）</summary>
		public List<string> FieldNamesForInsert { get; set; }
		/// <summary>更新キーフィールド</summary>
		public List<string> UpdateKeys { get; set; }
		/// <summary>更新禁止フィールド</summary>
		public List<string> ExceptionFields { get; set; }
		/// <summary>差分更新フィールド</summary>
		public List<string> IncreaseFields { get; set; }
		/// <summary>必須フィールド（Insert/Update用）</summary>
		public List<string> InsertUpdateNecessaryFields { get; set; }
		/// <summary>必須フィールド（Delete用）</summary>
		public List<string> DeleteNecessaryFields { get; set; }
		/// <summary>カラム存在チェック除外フィールド</summary>
		public List<string> ExclusionFields { get; set; }
		/// <summary>SQLパラメータ型定義（intやdatetime型が空の場合にDbNull変換を行うため）</summary>
		/// <remarks>
		/// (Hashtable)[User]
		///		+-- [login_id](string)  "nvarchar"
		///		+-- [password](string)  "nvarchar"
		///		|
		/// </remarks>
		public Dictionary<string, string> InputParamTypes { get; set; }
		/// <summary>SQLパラメータ型定義（SqlStatementより取得）</summary>
		public Dictionary<string, SqlStatement.SqlParam> InputParamDefines { get; set; }
		/// <summary>初期値設定情報</summary>
		private ProductDefaultSetting ProductDefaultSetting { get; set; }
		/// <summary>Insert/Update文</summary>
		public string InsertUpdateSql { get; set; }
		/// <summary>Delete文</summary>
		public string DeleteSql { get; set; }
		/// <summary>Insert/Update文（ワークテーブル用）</summary>
		public string InsertUpdateWorkSql { get; set; }
		/// <summary>Delete文（ワークテーブル用）</summary>
		public string DeleteWorkSql { get; set; }
		/// <summary>ワークテーブル初期化文</summary>
		public string InitlializeWorkTableSql { get; set; }
		/// <summary>Truncate work table sql</summary>
		public string TruncateWorkTableSql
		{
			get
			{
				// Truncate work table query
				return string.Format("TRUNCATE TABLE {0}", this.WorkTableName);
			}
		}
		/// <summary>マスタデータ</summary>
		public Hashtable Data { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessages { get; set; }
		/// <summary>警告メッセージ</summary>
		public string WarningMessages { get; set; }
		/// <summary>エラー発生ID情報</summary>
		public string ErrorOccurredIdInfo { get; set; }
		/// <summary>警告メールを送信します。</summary>
		public bool IsSendMail { get; set; }
		/// <summary>実行開始日時</summary>
		public DateTime? ExecuteDateTimeBegin { get; set; }
		/// <summary>実行完了日時</summary>
		public DateTime? ExecuteDateTimeComplete { get; set; }
	}
}
