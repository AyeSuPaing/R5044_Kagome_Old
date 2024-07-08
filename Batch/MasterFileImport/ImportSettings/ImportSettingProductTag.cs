/*
=========================================================================================================
  Module      : 商品タグ情報取込設定クラス(ImportSettingProductTag.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using w2.App.Common.ProductDefaultSetting;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingProductTag : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_PRODUCTTAG_PRODUCT_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_PRODUCTTAG_DATE_CHANGED, Constants.FIELD_PRODUCTTAG_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> {Constants.FIELD_PRODUCTTAG_PRODUCT_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_PRODUCTTAG_PRODUCT_ID };
		private static int CONST_SIZE = 100;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingProductTag()
			: base(
			"0",
			Constants.TABLE_PRODUCTTAG,
			Constants.TABLE_WORKPRODUCTTAG,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			SetInputParamDefines();  // SQLパラメータ型定義
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			// Trim処理 & 半角変換
			foreach (string strFieldName in this.HeadersCsv)
			{
				this.Data[strFieldName] = this.Data[strFieldName].ToString().Trim();
				if (strFieldName == Constants.FIELD_PRODUCTTAG_PRODUCT_ID) this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
			}
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			//------------------------------------------------------
			// 該当のアクション・フィールドを取得
			//------------------------------------------------------
			string checkKbn = null;
			List<string> necessaryFields = new List<string>();
			SetActionAndFields(ref checkKbn, ref necessaryFields);

			//------------------------------------------------------
			// 必須フィールドチェック
			//------------------------------------------------------
			StringBuilder errorMessages = CheckNecessaryFields(necessaryFields);

			//------------------------------------------------------
			// 各フィールドチェック
			//------------------------------------------------------
			CheckFields(checkKbn, errorMessages);
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";  // 整合性チェックしない
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			base.CreateSql();  // 共通SQL文作成（※ImportSettingBaseで定義）
		}

		/// <summary>
		/// 商品タグ設定情報取得
		/// </summary>
		private static DataView GetInputParameter()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductTagSetting", "GetTagSettingList"))
			{
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}
		}

		/// <summary>
		/// SQLパラメータ型定義設定
		/// </summary>
		/// <remarks>カラムは設定内容により動的に増減するため最新情報を商品タグ設定から取得する</remarks>
		protected override void SetInputParamTypes()
		{
			// 動的に増減するカラムをインサートするのに必要なパラメータを設定
			this.InputParamTypes = new Dictionary<string, string>();
			base.SetInputParamTypes();
			foreach (DataRowView tagSetting in GetInputParameter())
			{
				this.InputParamTypes.Add((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID], SqlDbType.NVarChar.ToString());
			}
		}

		/// <summary>
		/// SQLパラメータ型定義
		/// </summary>
		/// <remarks>カラムは設定内容により動的に増減するため最新情報を商品タグ設定から取得する</remarks>
		private void SetInputParamDefines()
		{
			foreach (DataRowView tagSetting in GetInputParameter())
			{
				SqlStatement.SqlParam spSqlParam = new SqlStatement.SqlParam();
				spSqlParam.Name = (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID];
				spSqlParam.Type = SqlDbType.NVarChar;
				spSqlParam.Size = CONST_SIZE;

				this.InputParamDefines.Add((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID], spSqlParam);
			}
		}

		/// <summary>
		/// 該当のアクション・フィールドを取得
		/// </summary>
		/// <param name="checkKbn">取込区分</param>
		/// <param name="necessaryFields">必須フィールドリスト</param>
		private void SetActionAndFields(ref string checkKbn, ref List<string> necessaryFields)
		{
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "ProductTagInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "ProductTagDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// Validator用にXMLドキュメントを取得(カラムが増減するため) ※1行毎に読み込ませたくないのでプロパティで保持
			if (this.ValidatorXmlDocument == null) this.ValidatorXmlDocument = new Dictionary<string, XmlDocument>();
			if (this.ValidatorXmlDocument.ContainsKey(checkKbn) == false) this.ValidatorXmlDocument.Add(checkKbn, GetValidatorXml(checkKbn).ToXmlDocument());
		}

		/// <summary>
		/// 必須フィールドチェック
		/// </summary>
		/// <param name="necessaryFields">必須フィールドリスト</param>
		private StringBuilder CheckNecessaryFields(List<string> necessaryFields)
		{
			StringBuilder sbNecessaryFields = new StringBuilder();
			foreach (string strKeyField in necessaryFields)
			{
				if (this.HeadersCsv.Contains(strKeyField) == false)
				{
					sbNecessaryFields.Append((sbNecessaryFields.Length != 0) ? "," : "");
					sbNecessaryFields.Append(strKeyField);
				}
			}
			StringBuilder sbErrorMessages = new StringBuilder();
			if (sbNecessaryFields.Length != 0)
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "");
				sbErrorMessages.Append("該当テーブルの更新にはフィールド「").Append(sbNecessaryFields.ToString()).Append("」が必須です。");
			}
			return sbErrorMessages;
		}

		/// <summary>
		/// 各フィールドチェック
		/// </summary>
		/// <param name="checkKbn">取込区分</param>
		/// <param name="errorMessages">エラーメッセージ</param>
		private void CheckFields(string checkKbn, StringBuilder errorMessages)
		{
			string errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTTAG_PRODUCT_ID);
			}
		}

		/// <summary>
		/// Validator用にXMLドキュメントを取得(カラムが増減するため)
		/// </summary>
		/// <returns></returns>
		private XDocument GetValidatorXml(string checkKbn)
		{
			XDocument validatorXml = Validator.GetValidateXmlDocument(checkKbn).ToXDocument();
			if (checkKbn == "ProductTagInsertUpdate")
			{
				// 複製用ノードを取得する
				XElement targetColumn = validatorXml.Root.Elements().FirstOrDefault(element => element.Attribute("name").Value == "@@ 1 @@");

				// 複製用ノードを各カラム用に変換してXmlへ追加
				foreach (DataRowView tagSetting in GetInputParameter())
				{
					XElement node = new XElement(targetColumn);
					node.SetAttributeValue("name", (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
					node.Element("name").SetValue((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME]);
					validatorXml.Root.Add(node);
				}
			}

			return validatorXml;
		}

		/// <summary>Validator用にXMLドキュメント</summary>
		private Dictionary<string, XmlDocument> ValidatorXmlDocument { get; set; }
	}
}
