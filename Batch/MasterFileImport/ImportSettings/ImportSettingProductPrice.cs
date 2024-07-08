/*
=========================================================================================================
  Module      : 商品価格情報取込設定クラス(ImportSettingProductPrice.cs)
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
	public class ImportSettingProductPrice : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_PRODUCTPRICE_SHOP_ID, Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, Constants.FIELD_PRODUCTPRICE_VARIATION_ID, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_PRODUCTPRICE_DATE_CHANGED, Constants.FIELD_PRODUCTPRICE_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_PRODUCTPRICE_SHOP_ID, Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, Constants.FIELD_PRODUCTPRICE_VARIATION_ID, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_PRODUCTPRICE_SHOP_ID, Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, Constants.FIELD_PRODUCTPRICE_VARIATION_ID, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingProductPrice(string shopId)
			: base(
			shopId,
			Constants.TABLE_PRODUCTPRICE,
			Constants.TABLE_WORKPRODUCTPRICE,
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
					case Constants.FIELD_PRODUCTPRICE_SHOP_ID:
					case Constants.FIELD_PRODUCTPRICE_PRODUCT_ID:
					case Constants.FIELD_PRODUCTPRICE_VARIATION_ID:
					case Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID:
					case Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
						break;
				}
			}

			// 商品IDとバリエーションIDに同じものが指定されている場合、バリエーションは空白にする
			if ((this.Data.ContainsKey(Constants.FIELD_PRODUCTPRICE_PRODUCT_ID))
				&& (this.Data.ContainsKey(Constants.FIELD_PRODUCTPRICE_VARIATION_ID)))
			{
				if (StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCTPRICE_PRODUCT_ID]).ToLower()
					== StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCTPRICE_VARIATION_ID]).ToLower())
				{
					this.Data[Constants.FIELD_PRODUCTPRICE_VARIATION_ID] = string.Empty;
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
					strCheckKbn = "ProductPriceInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "ProductPriceDelete";
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
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTPRICE_PRODUCT_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTPRICE_VARIATION_ID);
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
		}
	}
}
