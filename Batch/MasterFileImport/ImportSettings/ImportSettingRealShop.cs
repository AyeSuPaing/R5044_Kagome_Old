/*
=========================================================================================================
  Module      : リアル店舗設定取込クラス(ImportSettingRealShop.cs)
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
	/// リアル店舗設定取込
	/// </summary>
	public class ImportSettingRealShop : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_REALSHOP_REAL_SHOP_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_REALSHOP_DATE_CHANGED, Constants.FIELD_REALSHOP_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_REALSHOP_REAL_SHOP_ID};
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_REALSHOP_REAL_SHOP_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingRealShop(string shopId)
			: base(
			shopId,
			Constants.TABLE_REALSHOP,
			Constants.TABLE_WORKREALSHOP,
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
					case Constants.FIELD_REALSHOP_ZIP1:
					case Constants.FIELD_REALSHOP_ZIP2:
					case Constants.FIELD_REALSHOP_TEL_1:
					case Constants.FIELD_REALSHOP_TEL_2:
					case Constants.FIELD_REALSHOP_TEL_3:
					case Constants.FIELD_REALSHOP_FAX_1:
					case Constants.FIELD_REALSHOP_FAX_2:
					case Constants.FIELD_REALSHOP_FAX_3:
					case Constants.FIELD_REALSHOP_MAIL_ADDR:
						this.Data[fieldName] = StringUtility.ToHankaku(this.Data[fieldName].ToString());
						break;
				}
			}

			// 郵便番号（前＋後）
			if ((this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ZIP) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ZIP1)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ZIP2))
				)
			{
				// 郵便番号（前＋後）のどちらかが指定がない？
				if (StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ZIP1]) == string.Empty
					&& StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ZIP2]) == string.Empty)
				{
					this.Data[Constants.FIELD_REALSHOP_ZIP] = "";
				}
				// 郵便番号（前＋後）の全て指定がある？
				else
				{
					// フィールド結合
					this.Data[Constants.FIELD_REALSHOP_ZIP] =
						StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ZIP1])
						+ "-"
						+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ZIP2]);
				}

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_REALSHOP_ZIP) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_REALSHOP_ZIP);
				}
			}

			// 住所（都道府県＋市区町村＋番地＋ビル・マンション名）
			if ((this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ADDR) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ADDR1)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ADDR2)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ADDR3)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ADDR4)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_ADDR5)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_COUNTRY_NAME))
				)
			{
				// フィールド結合
				this.Data[Constants.FIELD_REALSHOP_ADDR] =
					StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ADDR1])
					+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ADDR2])
					+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ADDR3])
					+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ADDR4])
					+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_ADDR5])
					+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_COUNTRY_NAME]);

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_REALSHOP_ADDR) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_REALSHOP_ADDR);
				}
			}

			// 電話番号（前＋中＋後）
			if ((this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_TEL) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_TEL_1)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_TEL_2)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_TEL_3))
				)
			{
				// 電話番号（前＋中＋後）ののどちらかが指定がない？
				if (StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_TEL_1]) == string.Empty
					&& StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_TEL_2]) == string.Empty
					&& StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_TEL_3]) == string.Empty)
				{
					this.Data[Constants.FIELD_REALSHOP_TEL] = "";
				}
				// 電話番号（前＋中＋後）の全て指定がある？
				else
				{
					// フィールド結合
					this.Data[Constants.FIELD_REALSHOP_TEL] =
						StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_TEL_1])
						+ "-"
						+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_TEL_2])
						+ "-"
						+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_TEL_3]);
				}

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_REALSHOP_TEL) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_REALSHOP_TEL);
				}
			}

			// FAX（前＋中＋後）
			if ((this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_FAX) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_FAX_1)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_FAX_2)
					|| this.HeadersCsv.Contains(Constants.FIELD_REALSHOP_FAX_3))
				)
			{
				// FAX（前＋中＋後）ののどちらかが指定がない？
				if (StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_FAX_1]) == string.Empty
					&& StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_FAX_2]) == string.Empty
					&& StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_FAX_3]) == string.Empty)
				{
					this.Data[Constants.FIELD_REALSHOP_FAX] = "";
				}
				// FAX（前＋中＋後）の全て指定がある？
				else
				{
					// フィールド結合
					this.Data[Constants.FIELD_REALSHOP_FAX] =
						StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_FAX_1])
						+ "-"
						+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_FAX_2])
						+ "-"
						+ StringUtility.ToEmpty(this.Data[Constants.FIELD_REALSHOP_FAX_3]);
				}

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_REALSHOP_FAX) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_REALSHOP_FAX);
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
					checkKbn = "RealShopInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// 削除
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "RealShopDelete";
					necessaryFields = this.DeleteNecessaryFields;
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
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_REALSHOP_REAL_SHOP_ID);
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
