/*
=========================================================================================================
  Module      : シリアルキー情報取込設定クラス(ImportSettingSerialKey.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;
using w2.Common.Sql;
using System.Data;
using w2.Common.Util.Security;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.User;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingSerialKey : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_SERIALKEY_SERIAL_KEY, Constants.FIELD_SERIALKEY_PRODUCT_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_SERIALKEY_DATE_CHANGED, Constants.FIELD_SERIALKEY_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_SERIALKEY_SERIAL_KEY };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_SERIALKEY_SERIAL_KEY };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingSerialKey(string shopId)
			: base(
			shopId,
			Constants.TABLE_SERIALKEY,
			Constants.TABLE_WORKSERIALKEY,
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
			this.Data.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId);
			//------------------------------------------------------
			// データ変換
			//------------------------------------------------------
			foreach (string FieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[FieldName] = this.Data[FieldName].ToString().Trim();

				// シリアルキーの場合暗号化
				if ((FieldName == Constants.FIELD_SERIALKEY_SERIAL_KEY)
					&& (StringUtility.ToEmpty(this.Data[FieldName]) != ""))
				{
					RijndaelCrypto crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
					this.Data[FieldName] = crypto.Encrypt(StringUtility.ToHankaku(this.Data[FieldName].ToString()));
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
			string CheckKbn = null;
			List<string> lNecessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				case Constants.IMPORT_KBN_INSERT_UPDATE: // Insert/Update
					CheckKbn = "SerialKeyInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				case Constants.IMPORT_KBN_DELETE: // Delete
					CheckKbn = "SerialKeyDelete";
					lNecessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder NecessaryFields = new StringBuilder();
			foreach (string strKeyField in lNecessaryFields)
			{
				if (this.HeadersCsv.Contains(strKeyField) == false)
				{
					NecessaryFields.Append((NecessaryFields.Length != 0) ? "," : "");
					NecessaryFields.Append(strKeyField);
				}
			}

			StringBuilder errorMessages = new StringBuilder();
			if (NecessaryFields.Length != 0)
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(NecessaryFields.ToString()).Append("」が必須です。");
			}

			// 入力チェック用にデコード
			var input = (Hashtable)this.Data.Clone();
			if (this.Data.ContainsKey(Constants.FIELD_SERIALKEY_SERIAL_KEY))
			{
				var crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
				input[Constants.FIELD_SERIALKEY_SERIAL_KEY] =
					crypto.Decrypt(this.Data[Constants.FIELD_SERIALKEY_SERIAL_KEY].ToString());
			}
			// 入力チェック
			string errorMessage = Validator.Validate(CheckKbn, input);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_SERIALKEY_SERIAL_KEY);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_SERIALKEY_PRODUCT_ID);
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
			var errorMessages = new StringBuilder();

			// 商品チェック
			if (this.Data.ContainsKey(Constants.FIELD_SERIALKEY_PRODUCT_ID))
			{
				var count = new ProductService().CountProductView(
					(string)this.Data[Constants.FIELD_PRODUCT_SHOP_ID],
					(string)this.Data[Constants.FIELD_SERIALKEY_PRODUCT_ID],
					(string)this.Data[Constants.FIELD_SERIALKEY_VARIATION_ID]);
				if (count == 0)
				{
					errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
					errorMessages.Append("商品情報:" + this.Data[Constants.FIELD_SERIALKEY_PRODUCT_ID] + " が見つかりませんでした。");
				}
			}
			// ユーザチェック
			if (this.Data.ContainsKey(Constants.FIELD_SERIALKEY_USER_ID) && ((string)this.Data[Constants.FIELD_SERIALKEY_USER_ID] != ""))
			{
				var count = new UserService().Count((string)this.Data[Constants.FIELD_SERIALKEY_USER_ID]);
				if (count == 0)
				{
					errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
					errorMessages.Append("ユーザ情報:" + this.Data[Constants.FIELD_SERIALKEY_USER_ID] + " が見つかりませんでした");
				}
			}
			// 注文チェック
			if (this.Data.ContainsKey(Constants.FIELD_SERIALKEY_ORDER_ID) && ((string)this.Data[Constants.FIELD_SERIALKEY_ORDER_ID] != ""))
			{
				var count = new OrderService().Count((string)this.Data[Constants.FIELD_SERIALKEY_ORDER_ID]);
				if (count == 0)
				{
					errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
					errorMessages.Append("注文情報:" + this.Data[Constants.FIELD_SERIALKEY_ORDER_ID] + " が見つかりませんでした。");
				}
			}

			return errorMessages.ToString();
		}


		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			base.CreateSql(); // 共通SQL文作成（※ImportSettingBaseで定義）
		}
	}
}
