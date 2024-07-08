/*
=========================================================================================================
  Module      : DM発送履歴情報取込設定クラス(ImportSettingDmShippingHistory.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.Domain.User;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// DM発送履歴情報取込設定クラス
	/// </summary>
	public class ImportSettingDmShippingHistory : ImportSettingBase
	{
		/// <summary>
		/// 更新キーフィールド
		/// </summary>
		private static readonly List<string> m_fieldsUpdateKey = new List<string>
		{
			Constants.FIELD_DMSHIPPINGHISTORY_USER_ID,
			Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE,
		};
		/// <summary>
		/// 更新禁止フィールド（SQL自動作成除外フィールド）
		/// </summary>
		private static readonly List<string> m_fieldsExcept = new List<string>
		{
			Constants.FIELD_DMSHIPPINGHISTORY_DATE_CHANGED,
			Constants.FIELD_DMSHIPPINGHISTORY_LAST_CHANGED,
		};
		/// <summary>
		/// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		/// </summary>
		private static readonly List<string> m_fieldsIncreasedUpdate = new List<string>();
		/// <summary>
		/// 必須フィールド（Insert/Update用）※更新キーフィールドも含めること
		/// </summary>
		private static readonly List<string> m_fieldsNecessaryForInsertUpdate = new List<string>
		{
			Constants.FIELD_DMSHIPPINGHISTORY_USER_ID,
			Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE,
			Constants.FIELD_DMSHIPPINGHISTORY_DM_SHIPPING_DATE,
		};
		/// <summary>
		/// 必須フィールド（Delete用）
		/// </summary>
		private static readonly List<string> m_fieldsNecessaryForDelete = new List<string>
		{
			Constants.FIELD_DMSHIPPINGHISTORY_USER_ID,
			Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE,
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingDmShippingHistory()
			: base(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_DMSHIPPINGHISTORY,
			null,
			m_fieldsUpdateKey,
			m_fieldsExcept,
			m_fieldsIncreasedUpdate,
			m_fieldsNecessaryForInsertUpdate,
			m_fieldsNecessaryForDelete)
		{
		}

		///<summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		///</summary>
		protected override void ConvertData()
		{
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			// 入力チェック
			string checkKbn = null;
			var necessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "DmShippingHistoryInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "DmShippingHistoryDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// エラーメッセージ格納
			var errorMessages = CheckNecessaryFields(necessaryFields);
			var errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = string.Empty;
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessages += ((errorMessages.Length > 0) ? "\r\n" : string.Empty) + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_DMSHIPPINGHISTORY_USER_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE);
			}
			if (errorMessages.Length > 0)
			{
				this.ErrorMessages = errorMessages;
			}
		}

		/// <summary>
		/// 必須フィールドチェック
		/// </summary>
		/// <param name="fields">必須フィールド</param>
		/// <returns>エラーメッセージ</returns>
		protected string CheckNecessaryFields(List<string> fields)
		{
			var errorMessages = new StringBuilder();
			var necessaryFields = new StringBuilder();
			foreach (var keyField in fields)
			{
				if (this.HeadersCsv.Contains(keyField) == false)
				{
					necessaryFields.Append((necessaryFields.Length > 0) ? "," : string.Empty);
					necessaryFields.Append(keyField);
				}
			}
			if (necessaryFields.Length > 0)
			{
				errorMessages.Append((errorMessages.Length > 0) ? "\r\n" : string.Empty);
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(necessaryFields).Append("」が必須です。");
			}
			return errorMessages.ToString();
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			// ユーザー存在チェック
			var errorMessage = CheckUserExist();
			return errorMessage;
		}

		/// <summary>
		/// ユーザー存在チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckUserExist()
		{
			var userModel = new UserService().Get(this.Data[Constants.FIELD_USER_USER_ID].ToString());
			if (userModel == null)
			{
				return "ユーザーID:" + this.Data[Constants.FIELD_USER_USER_ID] + " の会員が見つかりませんでした";
			}
			return string.Empty;
		}
	}
}