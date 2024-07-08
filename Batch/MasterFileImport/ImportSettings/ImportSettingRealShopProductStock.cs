/*
=========================================================================================================
  Module      : リアル店舗商品在庫設定取込クラス(ImportSettingRealShopProductStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.App.Common.Option;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Common.Util.Security;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// リアル店舗商品在庫設定取込
	/// </summary>
	public class ImportSettingRealShopProductStock : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_REALSHOPPRODUCTSTOCK_DATE_CHANGED, Constants.FIELD_REALSHOPPRODUCTSTOCK_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { "add_" + Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingRealShopProductStock(string shopId)
			: base(
			shopId,
			Constants.TABLE_REALSHOPPRODUCTSTOCK,
			Constants.TABLE_WORKREALSHOPPRODUCTSTOCK,
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
					case Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID:
					case Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID:
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
				// 挿入/更新
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "RealShopProductStockInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// 削除
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "RealShopProductStockDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder errorMessages = new StringBuilder();
			StringBuilder necessaryFieldsBuilder = new StringBuilder();
			foreach (string strKeyField in necessaryFields)
			{
				if (this.HeadersCsv.Contains(strKeyField) == false)
				{
					necessaryFieldsBuilder.Append((necessaryFieldsBuilder.Length != 0) ? "," : "");
					necessaryFieldsBuilder.Append(strKeyField);
				}
			}
			if (necessaryFieldsBuilder.Length != 0)
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(necessaryFieldsBuilder.ToString()).Append("」が必須です。");
			}

			// 入力チェック
			string errorMessage = Validator.Validate(checkKbn, this.Data);

			// バリエーションIDチェック
			errorMessage += CheckVariationId(errorMessage);

			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID);
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
