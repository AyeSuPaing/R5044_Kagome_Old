/*
=========================================================================================================
  Module      : 名称翻訳設定マスタ取込設定クラス(ImportSettingNameTranslationSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingNameTranslationSetting : ImportSettingBase
	{
		#region メンバ変数
		/// <summary>翻訳前名称(マスタアップロードのみ使用)</summary>
		private static string m_beforeTranslationalName = "before_translational_name";
		/// <summary>更新キーフィールド</summary>
		private static List<string> m_fieldsUpdateKey = new List<string>
		{
			Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN,
			Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID3,
			Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE,
			Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID,
		};
		/// <summary>更新禁止フィールド（SQL自動作成除外フィールド）</summary>
		private static List<string> m_fieldsExcept = new List<string>
		{
			Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CHANGED,
			m_beforeTranslationalName,
		};
		/// <summary>差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）</summary>
		private static List<string> m_fieldsIncreasedUpdate = new List<string> { };
		/// <summary>必須フィールド（Insert/Update用）※更新キーフィールドも含めること</summary>
		private static List<string> m_fieldsNecessaryForInsertUpdate = new List<string>
		{
			Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN,
			Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID3,
			Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE,
			Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID,
			Constants.FIELD_NAMETRANSLATIONSETTING_AFTER_TRANSLATIONAL_NAME,
		};
		/// <summary>必須フィールド（Delete用）</summary>
		private static List<string> m_fieldsNecessaryForDelete = new List<string>
		{
			Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN,
			Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2,
			Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID3,
			Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE,
			Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID,
		};
		/// <summary>
		/// 表示HTML区分を利用する項目
		/// </summary>
		private static List<string> m_fieldsUseDisplayKbn = new List<string>
		{
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_OUTLINE,	// 商品概要
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL1,	// 商品詳細1
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL2,	// 商品詳細2
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL3,	// 商品詳細3
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL4,	// 商品詳細4
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION,	// 表示用文言
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE,	// ユーザ拡張項目概要
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NEWS_NEWS_TEXT,	// 本文(新着情報)
		};

		/// <summary>
		/// カラム存在チェック除外フィールド
		/// </summary>
		private static List<string> m_fieldsExclusion = new List<string>
		{
			m_beforeTranslationalName,	// 翻訳前名称
		};
		#endregion

		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingNameTranslationSetting()
			: base(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.TABLE_NAMETRANSLATIONSETTING,
				Constants.TABLE_WORKNAMETRANSLATIONSETTING,
				m_fieldsUpdateKey,
				m_fieldsExcept,
				m_fieldsIncreasedUpdate,
				m_fieldsNecessaryForInsertUpdate,
				m_fieldsNecessaryForDelete)
		{
			this.ExclusionFields = m_fieldsExclusion;
		}
		#endregion

		#region #ConvertData データ変換（各種変換、フィールド結合、固定値設定など）
		///<summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		///</summary>
		protected override void ConvertData()
		{
		}
		#endregion

		#region #CheckData 入力チェック
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
					checkKbn = "NameTranslationSettingInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "NameTranslationSettingDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			var errorMessages = CheckNecessaryFields(necessaryFields);

			// 入力チェック
			var errorMessage = Validate(checkKbn);
			this.ErrorOccurredIdInfo = "";

			if (errorMessage != "")
			{
				errorMessages += ((errorMessages.Length != 0) ? "\r\n" : "") + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID3);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_NAMETRANSLATIONSETTING_AFTER_TRANSLATIONAL_NAME);
			}
			// エラーメッセージ格納
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}
		#endregion

		#region #CheckNecessaryFields 必須フィールドチェック
		/// <summary>
		/// 必須フィールドチェック
		/// </summary>
		/// <param name="fields">必須フィールド</param>
		/// <returns>エラーメッセージ</returns>
		protected string CheckNecessaryFields(List<string> fields)
		{
			var errorMessages = new StringBuilder();
			var necessaryFields = new StringBuilder();
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
		#endregion

		#region #Validate 入力チェック
		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <param name="checkKbn">入力チェック区分</param>
		/// <returns>エラーメッセージ</returns>
		protected string Validate(string checkKbn)
		{
			var input = (IDictionary)this.Data.Clone();

			// HTML区分を利用する項目か
			var dataKbn = (string)this.Data[Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN];
			var translationTargetColumn = (string)this.Data[Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN];
			if (m_fieldsUseDisplayKbn.Any(column => column == translationTargetColumn))
			{
				// 「ユーザ拡張項目概要」の場合は別チェック仕様
				if ((dataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING)
					&& (translationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE))
				{
					input.Add(Constants.FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN + "_for_user_extend", input[Constants.FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN]);
					input.Remove(Constants.FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN);
				}
			}
			else
			{
				// 利用しない場合は入力チェックしない
				input.Remove(Constants.FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN);
			}
			var errorMessage = Validator.Validate(checkKbn, input);
			return errorMessage;
		}
		#endregion

		#region +CheckDataConsistency 整合性チェック
		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";	// 整合性チェックしない
		}
		#endregion
	}
}
