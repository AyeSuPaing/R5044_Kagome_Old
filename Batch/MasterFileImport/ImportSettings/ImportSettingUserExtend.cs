/*
=========================================================================================================
  Module      : ユーザ拡張情報取込設定クラス(ImportSettingUserExtend.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
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
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingUserExtend : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_USEREXTEND_USER_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_USEREXTEND_DATE_CHANGED, Constants.FIELD_USEREXTEND_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_USEREXTEND_USER_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_USEREXTEND_USER_ID };
		private static int CONST_SIZE = 4000;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingUserExtend()
			: base(
			"0",
			Constants.TABLE_USEREXTEND,
			Constants.TABLE_WORKUSEREXTEND,
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
			foreach (string fieldName in this.HeadersCsv)
			{
				this.Data[fieldName] = this.Data[fieldName].ToString().Trim();
				if (fieldName == Constants.FIELD_USEREXTEND_USER_ID)
				{
					this.Data[fieldName] = StringUtility.ToHankaku(this.Data[fieldName].ToString());
				}
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
		/// SQLパラメータ型定義設定
		/// </summary>
		/// <remarks>カラムは設定内容により動的に増減するため最新情報をユーザ拡張項目設定から取得する</remarks>
		protected override void SetInputParamTypes()
		{
			// 動的に増減するカラムをインサートするのに必要なパラメータを設定
			this.InputParamTypes = new Dictionary<string, string>();
			base.SetInputParamTypes();
			var userExtendSettingList = new UserService().GetUserExtendSettingList();
			foreach (var setting in userExtendSettingList.Items)
			{
				this.InputParamTypes.Add(setting.SettingId, SqlDbType.NVarChar.ToString());
			}
		}

		/// <summary>
		/// SQLパラメータ型定義
		/// </summary>
		/// <remarks>カラムは設定内容により動的に増減するため最新情報をユーザ拡張項目設定から取得する</remarks>
		private void SetInputParamDefines()
		{
			var userExtendSettingList = new UserService().GetUserExtendSettingList();
			foreach (var setting in userExtendSettingList.Items)
			{
				SqlStatement.SqlParam spSqlParam = new SqlStatement.SqlParam();
				spSqlParam.Name = setting.SettingId;
				spSqlParam.Type = SqlDbType.NVarChar;
				spSqlParam.Size = CONST_SIZE;

				this.InputParamDefines.Add(setting.SettingId, spSqlParam);
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
					checkKbn = "UserExtendInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "UserExtendDelete";
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
			// 入力チェック
			string errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_USEREXTEND_USER_ID);
			}

			// エラーメッセージ格納
			if (errorMessages.Length != 0) this.ErrorMessages = errorMessages.ToString();
		}

		/// <summary>
		/// Validator用にXMLドキュメントを取得(カラムが増減するため)
		/// </summary>
		/// <returns></returns>
		private XDocument GetValidatorXml(string checkKbn)
		{
			XDocument validatorXml = Validator.GetValidateXmlDocument(checkKbn).ToXDocument();
			if (checkKbn == "UserExtendInsertUpdate")
			{
				// 複製用ノードを取得する
				XElement targetColumn = validatorXml.Root.Elements().FirstOrDefault(element => element.Attribute("name").Value == "@@ 1 @@");

				var userExtendSettingList = new UserService().GetUserExtendSettingList();
				// 複製用ノードを各カラム用に変換してXmlへ追加
				foreach (var setting in userExtendSettingList.Items)
				{
					XElement node = new XElement(targetColumn);
					node.SetAttributeValue("name", setting.SettingId);
					node.Element("name").SetValue(setting.SettingName);
					validatorXml.Root.Add(node);
				}
			}

			return validatorXml;
		}

		/// <summary>Validator用にXMLドキュメント</summary>
		private Dictionary<string, XmlDocument> ValidatorXmlDocument { get; set; }
	}
}
