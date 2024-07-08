/*
=========================================================================================================
  Module      : 商品在庫情報取込設定クラス処理(ImportSettingProductStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;
using w2.Common.Sql;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingProductStock : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_PRODUCTSTOCK_SHOP_ID, Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, Constants.FIELD_PRODUCTSTOCK_VARIATION_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_PRODUCTSTOCK_DATE_CHANGED, Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { "add_" + Constants.FIELD_PRODUCTSTOCK_STOCK, "add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK, "add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, "add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, "add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_PRODUCTSTOCK_SHOP_ID, Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, Constants.FIELD_PRODUCTSTOCK_VARIATION_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_PRODUCTSTOCK_SHOP_ID, Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, Constants.FIELD_PRODUCTSTOCK_VARIATION_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingProductStock(string shopId)
			: base(
			shopId,
			Constants.TABLE_PRODUCTSTOCK,
			Constants.TABLE_WORKPRODUCTSTOCK,
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
			foreach (string strFieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[strFieldName] = this.Data[strFieldName].ToString().Trim();

				// 半角変換
				switch (strFieldName)
				{
					case Constants.FIELD_PRODUCTSTOCK_SHOP_ID:
					case Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID:
					case Constants.FIELD_PRODUCTSTOCK_VARIATION_ID:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
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
			string strCheckKbn = null;
			List<string> lNecessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					strCheckKbn = "ProductStockInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "ProductStockDelete";
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
			string errorMessage = Validator.Validate(strCheckKbn, this.Data);

			// バリエーションIDチェック
			errorMessage += CheckVariationId(errorMessage);

			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID);
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
			// 共通SQL文作成（※ImportSettingBaseで定義）
			//------------------------------------------------------
			base.CreateSql();

			//------------------------------------------------------
			// 商品在庫履歴SQL文
			//------------------------------------------------------
			this.ProductStockHistoryInsertSql = CreateProductStockHistoryInsertSql();
			this.ProductStockHistoryDeleteSql = CreateProductStockHistoryDeleteSql();
		}

		/// <summary>
		/// 商品在庫履歴Insert文作成
		/// </summary>
		/// <returns>商品在庫履歴Insert文</returns>
		private string CreateProductStockHistoryInsertSql()
		{
			StringBuilder sbFields = new StringBuilder();
			StringBuilder sbValues = new StringBuilder();

			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_PRODUCTSTOCK_STOCK))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_STOCK).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_PRODUCTSTOCK_REALSTOCK))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_REALSTOCK).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains("add_" + Constants.FIELD_PRODUCTSTOCK_STOCK))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK).Append(",");
				sbValues.Append("@").Append("add_" + Constants.FIELD_PRODUCTSTOCK_STOCK).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK).Append(",");
				sbValues.Append("@").Append("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B).Append(",");
				sbValues.Append("@").Append("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C).Append(",");
				sbValues.Append("@").Append("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED).Append(",");
				sbValues.Append("@").Append("add_" + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO))
			{
				sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO).Append(",");
			}

			if (sbFields.Length == 0)
			{
				return null;
			}

			StringBuilder sbResult = new StringBuilder();
			sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID).Append(",");
			sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_SHOP_ID).Append(",");

			sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID).Append(",");
			sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID).Append(",");

			sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID).Append(",");
			sbValues.Append("@").Append(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID).Append(",");

			sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS).Append(",");
			sbValues.Append("'").Append(Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_MASTER_IMPORT).Append("',");

			sbFields.Append(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED);
			sbValues.Append("'").Append(Constants.IMPORT_LAST_CHANGED).Append("'");

			sbResult.Append("INSERT INTO " + Constants.TABLE_PRODUCTSTOCKHISTORY);
			sbResult.Append("(").Append(sbFields.ToString()).Append(")");
			sbResult.Append(" VALUES ");
			sbResult.Append("(").Append(sbValues.ToString()).Append(")");

			return sbResult.ToString();
		}

		/// <summary>
		/// 商品在庫履歴Delete文作成
		/// </summary>
		/// <returns>商品在庫履歴Delete文</returns>
		private string CreateProductStockHistoryDeleteSql()
		{
			StringBuilder sbResult = new StringBuilder();

			sbResult.Append("DELETE ").Append(Constants.TABLE_PRODUCTSTOCKHISTORY);
			sbResult.Append(" WHERE ").Append(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID).Append(" = ").Append("@").Append(Constants.FIELD_PRODUCTSTOCK_SHOP_ID);
			sbResult.Append("   AND ").Append(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID).Append(" = ").Append("@").Append(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID);
			sbResult.Append("   AND ").Append(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID).Append(" = ").Append("@").Append(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID);

			return sbResult.ToString();
		}

		/// <summary>商品在庫履歴Insert文</summary>
		public string ProductStockHistoryInsertSql { get; set; }
		/// <summary>商品在庫履歴Delete文</summary>
		public string ProductStockHistoryDeleteSql { get; set; }

	}
}
