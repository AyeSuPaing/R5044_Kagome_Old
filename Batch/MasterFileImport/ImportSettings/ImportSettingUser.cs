/*
=========================================================================================================
  Module      : ユーザー情報取込設定クラス(ImportSettingUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Config;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Common.Util.Security;
using w2.Domain.User;
using w2.Domain.User.Helper;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingUser : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_USER_USER_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_USER_DATE_CHANGED, Constants.FIELD_USER_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_USER_USER_ID, Constants.FIELD_USER_USER_KBN };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_USER_USER_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingUser(string shopId, bool checkFlg = true)
			: base(
			shopId,
			Constants.TABLE_USER,
			Constants.TABLE_WORKUSER,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			this.CheckFlg = checkFlg;

			// 会員ランクID一覧設定（会員ランク変更履歴設定用）
			this.MemberRankIds = MemberRankOptionUtility.GetMemberRankList()
				.Select(memberRank => memberRank.MemberRankId)
				.ToList();
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
					case Constants.FIELD_USER_USER_ID:
					case Constants.FIELD_USER_USER_KBN:
					case Constants.FIELD_USER_MAIL_ADDR:
					case Constants.FIELD_USER_MAIL_ADDR2:
					case Constants.FIELD_USER_BIRTH_YEAR:
					case Constants.FIELD_USER_BIRTH_MONTH:
					case Constants.FIELD_USER_BIRTH_DAY:
					case Constants.FIELD_USER_SEX:
					case Constants.FIELD_USER_ZIP:
					case Constants.FIELD_USER_ZIP1:
					case Constants.FIELD_USER_ZIP2:
					case Constants.FIELD_USER_TEL1:
					case Constants.FIELD_USER_TEL1_1:
					case Constants.FIELD_USER_TEL1_2:
					case Constants.FIELD_USER_TEL1_3:
					case Constants.FIELD_USER_TEL2:
					case Constants.FIELD_USER_TEL2_1:
					case Constants.FIELD_USER_TEL2_2:
					case Constants.FIELD_USER_TEL2_3:
					case Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE:
					case Constants.FIELD_USER_MAIL_FLG:
					case Constants.FIELD_USER_LOGIN_ID:
					case Constants.FIELD_USER_PASSWORD:
					case Constants.FIELD_USER_REMOTE_ADDR:
					case Constants.FIELD_USER_MOBILE_UID:
					case Constants.FIELD_USER_ADVCODE_FIRST:
					case Constants.FIELD_USER_DEL_FLG:
					case Constants.FIELD_USER_MEMBER_RANK_ID:
					case Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID:
						this.Data[fieldName] = StringUtility.ToHankaku(this.Data[fieldName].ToString());
						break;
				}

				// 全角変換
				if ((Constants.GLOBAL_OPTION_ENABLE == false) || (Constants.HALFWIDTH_CHARACTER_INPUTABLE == false))
				{
				switch (fieldName)
				{
					case Constants.FIELD_USER_NAME:
					case Constants.FIELD_USER_NAME1:
					case Constants.FIELD_USER_NAME2:
					case Constants.FIELD_USER_ADDR:
					case Constants.FIELD_USER_ADDR1:
					case Constants.FIELD_USER_ADDR2:
					case Constants.FIELD_USER_ADDR3:
					case Constants.FIELD_USER_ADDR4:
					case Constants.FIELD_USER_COMPANY_NAME:
					case Constants.FIELD_USER_COMPANY_POST_NAME:
						this.Data[fieldName] = StringUtility.ToZenkaku(this.Data[fieldName].ToString());
						break;
				}
				}

				// 全角ひらがな変換
				switch (fieldName)
				{
					case Constants.FIELD_USER_NAME_KANA:
					case Constants.FIELD_USER_NAME_KANA1:
					case Constants.FIELD_USER_NAME_KANA2:
						this.Data[fieldName] = DataInputUtility.ConvertToFullWidthKanaBySetting(this.Data[fieldName].ToString());
						break;
				}

				// 生年月日（月、日）の０詰め削除変換
				switch (fieldName)
				{
					case Constants.FIELD_USER_BIRTH_MONTH:
					case Constants.FIELD_USER_BIRTH_DAY:
						int iValue;
						if (int.TryParse(this.Data[fieldName].ToString(), out iValue))
						{
							this.Data[fieldName] = iValue.ToString();
						}
						break;
				}

				// 暗号化
				switch (fieldName)
				{
					case Constants.FIELD_USER_PASSWORD:
						// ユーザマスタ(非入力チェック)の場合でもパスワードの暗号化を行う
						// フィールドがパスワードで値が空文字以外の場合
						if (StringUtility.ToEmpty(this.Data[Constants.FIELD_USER_PASSWORD]) != "")
						{
							RijndaelCrypto crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);

							this.Data[Constants.FIELD_USER_PASSWORD] = crypto.Encrypt((string)this.Data[Constants.FIELD_USER_PASSWORD]);
						}
						break;
				}
			}

			//------------------------------------------------------
			// フィールド結合
			// ■メモ
			// １．結合フィールドが指定されている場合、指定されているデータを優先する
			//     ＜例＞
			//     氏名（姓名）に指定があった場合、氏名（姓名）を優先
			// ２．結合対象のフィールドが１つでも指定がある場合、フィールド結合を行う
			//     ＜例＞
			//     氏名（姓）のみ指定があった場合、氏名（姓名）には氏名（姓）が登録される
			// ３．ヘッダフィールドに結合フィールドが存在しない場合、追加する
			//------------------------------------------------------
			// 氏名（姓＋名）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_NAME) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_NAME1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_NAME2))
				)
			{
				// フィールド結合
				this.Data[Constants.FIELD_USER_NAME] =
					this.Data[Constants.FIELD_USER_NAME1].ToString() + this.Data[Constants.FIELD_USER_NAME2].ToString();

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_NAME) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_NAME);
				}
			}
			// 氏名かな（姓＋名）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_NAME_KANA) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_NAME_KANA1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_NAME_KANA2))
				)
			{
				// フィールド結合
				this.Data[Constants.FIELD_USER_NAME_KANA] =
					this.Data[Constants.FIELD_USER_NAME_KANA1].ToString() + this.Data[Constants.FIELD_USER_NAME_KANA2].ToString();

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_NAME_KANA) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_NAME_KANA);
				}
			}
			// 生年月日（年＋月＋日）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_BIRTH) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_BIRTH_YEAR)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_BIRTH_MONTH)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_BIRTH_DAY))
				)
			{
				// 年・月・日の指定がない？
				if (this.Data[Constants.FIELD_USER_BIRTH_YEAR].ToString() == ""
					&& this.Data[Constants.FIELD_USER_BIRTH_MONTH].ToString() == ""
					&& this.Data[Constants.FIELD_USER_BIRTH_DAY].ToString() == "")
				{
					this.Data[Constants.FIELD_USER_BIRTH] = "";	// 生年月日にはnullが格納
				}
				// 年月日の指定がある？
				else
				{
					// フィールド結合
					this.Data[Constants.FIELD_USER_BIRTH] =
						this.Data[Constants.FIELD_USER_BIRTH_YEAR].ToString()
						+ "/"
						+ this.Data[Constants.FIELD_USER_BIRTH_MONTH].ToString()
						+ "/"
						+ this.Data[Constants.FIELD_USER_BIRTH_DAY].ToString();
				}

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_BIRTH) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_BIRTH);
				}
			}
			// 郵便番号（前＋後）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_ZIP) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_ZIP1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_ZIP2))
				)
			{
				// 郵便番号（前＋後）のどちらかが指定がない？
				if (this.Data[Constants.FIELD_USER_ZIP1].ToString() == ""
					|| this.Data[Constants.FIELD_USER_ZIP2].ToString() == "")
				{
					this.Data[Constants.FIELD_USER_ZIP] = "";
				}
				// 郵便番号（前＋後）の全て指定がある？
				else
				{
					// フィールド結合
					this.Data[Constants.FIELD_USER_ZIP] =
						this.Data[Constants.FIELD_USER_ZIP1].ToString()
						+ "-"
						+ this.Data[Constants.FIELD_USER_ZIP2].ToString();
				}

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_ZIP) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_ZIP);
				}
			}
			// 住所（都道府県＋市区町村＋番地＋ビル・マンション名）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR2)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR3)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR4)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR5)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_ADDR_COUNTRY_NAME))
				)
			{
				// フィールド結合
				this.Data[Constants.FIELD_USER_ADDR] =
					this.Data[Constants.FIELD_USER_ADDR1].ToString()
					+ this.Data[Constants.FIELD_USER_ADDR2].ToString()
					+ this.Data[Constants.FIELD_USER_ADDR3].ToString()
					+ this.Data[Constants.FIELD_USER_ADDR4].ToString()
					+ ((Constants.GLOBAL_OPTION_ENABLE)
						? (this.Data[Constants.FIELD_USER_ADDR5].ToString()
							+ this.Data[Constants.FIELD_USER_ADDR_COUNTRY_NAME].ToString())
						: "");

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_ADDR) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_ADDR);
				}
			}
			// 電話番号（前＋中＋後）
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_TEL1) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_TEL1_1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_TEL1_2)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_TEL1_3))
				)
			{
				// 電話番号（前＋中＋後）ののどちらかが指定がない？
				if (this.Data[Constants.FIELD_USER_TEL1_1].ToString() == ""
					|| this.Data[Constants.FIELD_USER_TEL1_2].ToString() == ""
					|| this.Data[Constants.FIELD_USER_TEL1_3].ToString() == "")
				{
					this.Data[Constants.FIELD_USER_TEL1] = "";
				}
				// 電話番号（前＋中＋後）の全て指定がある？
				else
				{
					// フィールド結合
					this.Data[Constants.FIELD_USER_TEL1] = UserService.CreatePhoneNo(this.Data[Constants.FIELD_USER_TEL1_1].ToString(),
																					 this.Data[Constants.FIELD_USER_TEL1_2].ToString(),
																					 this.Data[Constants.FIELD_USER_TEL1_3].ToString());
				}

				// ヘッダーフィールド追加
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_TEL1) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_TEL1);
				}
			}

			// Import for telephone 2
			if ((this.HeadersCsv.Contains(Constants.FIELD_USER_TEL2) == false)
				&& (this.HeadersCsv.Contains(Constants.FIELD_USER_TEL2_1)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_TEL2_2)
					|| this.HeadersCsv.Contains(Constants.FIELD_USER_TEL2_3))
				)
			{
				// Create telephone 2
				this.Data[Constants.FIELD_USER_TEL2] = UserService.CreatePhoneNo(this.Data[Constants.FIELD_USER_TEL2_1].ToString(),
																				 this.Data[Constants.FIELD_USER_TEL2_2].ToString(),
																				 this.Data[Constants.FIELD_USER_TEL2_3].ToString());
				// Add field name for telephone 2
				if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_TEL2) == false)
				{
					this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USER_TEL2);
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
					strCheckKbn = this.GetValidationStatement;
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "UserDelete";
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
			/*
			// エラーメッセージ末尾に登録値を付与するため、１データ毎にチェック
			foreach (string strFieldName in Validator.GetValidatorColumns(strCheckKbn))
			{
				// 入力データがなければ、以下のチェックを行わない
				if (this.Data.ContainsKey(strFieldName) == false) continue;

				// 入力チェック
				Hashtable htInput = new Hashtable();
				string strValue = this.Data[strFieldName].ToString();

				// パスワードの場合は復号化
				if ((strFieldName == Constants.FIELD_USER_PASSWORD) && (StringUtility.ToEmpty(strValue) != ""))
				{
					RijndaelCrypto crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
					strValue = crypto.Decrypt(strValue);
				}

				htInput.Add(strFieldName, strValue);
				string strErrorMessages = Validator.Validate(strCheckKbn, htInput);

				// 会員ランクIDの存在確認(ユーザーランクが空の場合はチェックを行わない)
				if ((strFieldName == Constants.FIELD_USER_MEMBER_RANK_ID)
					&& (strValue != "")
					&& (this.MemberRankIds.Contains(strValue) == false))
				{
					strErrorMessages += MessageManager.GetMessages(Constants.INPUTCHECK_NOT_EXIST_MEMBER_RANK_ID);
				}

				// 入力エラー？
				if (strErrorMessages != "")
				{
					sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "");
					sbErrorMessages.Append(strErrorMessages).Append("：").Append(strValue);
				}
			}*/

			if (this.CheckFlg == false)
			{
				if ((string)this.Data[Constants.FIELD_USER_USER_ID] == "")
				{
					this.ErrorMessages = Validator.GetErrorMessage("CHECK_NECESSARY", Constants.FIELD_USER_USER_ID);
				}
				return;
			}

			var input = (Hashtable)this.Data.Clone();
			// 入力チェック用にデコード
			if ((this.Data.ContainsKey(Constants.FIELD_USER_PASSWORD))
				&& ((string)this.Data[Constants.FIELD_USER_PASSWORD] != ""))
			{
				var crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
				input[Constants.FIELD_USER_PASSWORD] = crypto.Decrypt(this.Data[Constants.FIELD_USER_PASSWORD].ToString());
			}
			// 入力チェック
			var errorMessage = Validator.ChangeToDisplay(Validator.Validate(
				strCheckKbn,
				input,
				(Constants.GLOBAL_OPTION_ENABLE
					? StringUtility.ToEmpty(this.Data[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID])
					: ""),
				(Constants.GLOBAL_OPTION_ENABLE
					? StringUtility.ToEmpty(this.Data[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE])
					: "")));

			// グローバル対応チェック処理
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var globalError = CheckGlobalData();
				if (globalError != "") errorMessage += "\r\n" + globalError;
			}

			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_USER_USER_ID);
			}

			// エラーメッセージ格納
			if (sbErrorMessages.Length != 0)
			{
				this.ErrorMessages = sbErrorMessages.ToString();
			}
		}

		/// <summary>
		/// グローバル対応 入力値チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckGlobalData()
		{
			var errorMessage = new StringBuilder();

			// アクセス国ISOコード検証
			if ((this.Data.ContainsKey(Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE))
				&& ((string)this.Data[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE] != "")
				&& (GlobalConfigUtil.CheckCountryPossible((string)this.Data[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE]) == false))
			{
				errorMessage = GetGlobalErrorMessage(
					errorMessage,
					Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE,
					(string)this.Data[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE]);
				}

			// 表示言語コード＆表示言語ロケールID検証
			if ((this.Data.ContainsKey(Constants.FIELD_USER_DISP_LANGUAGE_CODE))
				&& ((string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_CODE] != "")
				&& (this.Data.ContainsKey(Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID))
				&& ((string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID] != ""))
			{
				if (GlobalConfigUtil.CheckLanguagePossible((string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_CODE]) == false)
				{
					errorMessage = GetGlobalErrorMessage(
						errorMessage,
						Constants.FIELD_USER_DISP_LANGUAGE_CODE,
						(string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_CODE]);
				}

				if (GlobalConfigUtil.CheckLanguageLocaleIdPossible(
					(string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_CODE],
					(string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]) == false)
				{
					errorMessage = GetGlobalErrorMessage(
						errorMessage,
						Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID,
						(string)this.Data[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]);
				}
			}

			// 表示通貨コード＆表示通貨ロケールID検証
			if ((this.Data.ContainsKey(Constants.FIELD_USER_DISP_CURRENCY_CODE))
				&& ((string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_CODE] != "")
				&& (this.Data.ContainsKey(Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID))
				&& ((string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID] != ""))
			{
				if (GlobalConfigUtil.CheckCurrencyPossible((string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_CODE]) == false)
				{
					errorMessage = GetGlobalErrorMessage(errorMessage, Constants.FIELD_USER_DISP_CURRENCY_CODE, (string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_CODE]);
				}

				if (GlobalConfigUtil.CheckCurrencyLocaleIdPossible(
					(string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_CODE],
					(string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID]) == false)
				{
					errorMessage = GetGlobalErrorMessage(
						errorMessage,
						Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID,
						(string)this.Data[Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID]);
				}
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// グローバル対応 エラーメッセージ生成
		/// </summary>
		/// <param name="errorMessage">入力先エラーメッセージ</param>
		/// <param name="columnName">カラム名</param>
		/// <param name="columnValue">カラム値</param>
		/// <returns></returns>
		private StringBuilder GetGlobalErrorMessage(StringBuilder errorMessage, string columnName, string columnValue)
		{
			errorMessage
				.Append((errorMessage.Length != 0) ? "\r\n" : "")
				.Append(
					string.Format("{0}にてエラーが発生しました。{1}は対応していません。"
					, columnName
					, columnValue));
			return errorMessage;
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			StringBuilder sbResult = new StringBuilder();

			//------------------------------------------------------
			// ログインID重複チェック
			// ※ワークテーブル参照
			//------------------------------------------------------
			// ユーザー情報取得
			var userList = new UserService().GetDuplicationLoginIdList();
			// ログインID重複件数分ループ
			foreach (DuplicationLoginId data in userList)
			{
				sbResult.Append((sbResult.Length != 0) ? "\r\n" : "");
				sbResult.Append("ログインIDが重複しています");
				sbResult.Append("（");
				sbResult.Append("ログインID：").Append(data.LoginId);
				sbResult.Append("、");
				sbResult.Append("件数：").Append(data.Count);
				sbResult.Append("）");
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			//------------------------------------------------------
			// 共通SQL文作成
			//------------------------------------------------------
			// ワークテーブル初期化文
			this.InitlializeWorkTableSql = CreateInitlializeWorkTableSql();
			// Insert/Update文 
			this.InsertUpdateSql = CreateInsertUpdateSql();
			// Delete文
			this.DeleteSql = CreateDeleteSql(this.TableName);
			// Insert/Update文 （ワークテーブル用）
			this.InsertUpdateWorkSql = CreateInsertUpdateSql(this.WorkTableName);
			// Delete文 （ワークテーブル用）			
			this.DeleteWorkSql = CreateDeleteSql(this.WorkTableName);
		}

		/// <summary>
		/// Insert/Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected string CreateInsertUpdateSql()
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
				sbSqlSelect.Append(" FROM ").Append(Constants.TABLE_USER);
				sbSqlSelect.Append(CreateWhere());
			}

			// Insert/Update文組み立て
			sbResult.Append(" DECLARE @SELECT_COUNTS int");
			sbResult.Append(sbSqlSelect.ToString());
			sbResult.Append(" IF @SELECT_COUNTS = 0 ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(CreateInsertSqlRaw(Constants.TABLE_USER));
			sbResult.Append("   END ");
			sbResult.Append(" ELSE ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(CreateUpdateSql());
			sbResult.Append("   END ");

			return sbResult.ToString();
		}

		/// <summary>
		/// Update文作成
		/// </summary>
		/// <returns>Update文</returns>
		private string CreateUpdateSql()
		{
			//------------------------------------------------------
			// 会員ランク履歴変更文作成
			//------------------------------------------------------
			// 会員ランクフィールドが更新対象の場合、会員ランク変更履歴用SQL文生成
			StringBuilder sbResult = new StringBuilder();
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USER_MEMBER_RANK_ID))
			{
				sbResult.Append("DECLARE  @SELECT_MEMBER_RANK_ID nvarchar(30)");
				sbResult.Append("\r\n");
				sbResult.Append(" SELECT  @SELECT_MEMBER_RANK_ID = w2_User.member_rank_id");
				sbResult.Append("\r\n");
				sbResult.Append("   FROM  w2_User");
				sbResult.Append("\r\n");
				sbResult.Append("  WHERE  w2_User.user_id = @user_id");
				sbResult.Append("\r\n");
				sbResult.Append(" INSERT  INTO  ").Append(Constants.TABLE_USERMEMBERRANKHISTORY);
				sbResult.Append("\r\n");
				sbResult.Append(" VALUES(@user_id,@SELECT_MEMBER_RANK_ID,@member_rank_id,'','Batch',GETDATE())");
			}

			//------------------------------------------------------
			// Update文
			//------------------------------------------------------
			StringBuilder sbSqlFields = new StringBuilder();
			foreach (string strFieldName in this.FieldNamesForUpdateDelete)
			{
				if (strFieldName.Length == 0)
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

			// 更新日設定
			sbSqlFields.Append(",date_changed =  GETDATE()");

			// 更新者設定（※更新禁止フィールドに"last_changed"があれば更新）
			if (this.ExceptionFields.Contains(Constants.FIELD_IMPORT_LAST_CHANGED))
			{
				sbSqlFields.Append((sbSqlFields.Length > 0) ? "," : string.Empty)
					.AppendFormat("{0} = @{0}", Constants.FIELD_IMPORT_LAST_CHANGED);
			}

			// Update文組み立て
			sbResult.Append(" UPDATE ").Append(Constants.TABLE_USER);
			sbResult.Append(" SET ").Append(sbSqlFields.ToString());
			sbResult.Append(CreateWhere());

			w2.Common.Logger.AppLogger.WriteInfo(sbResult.ToString());

			return sbResult.ToString();
		}

		/// <summary>会員ランクID一覧</summary>
		protected List<string> MemberRankIds { get; set; }
		/// <summary>チェックフラグ</summary>
		protected bool CheckFlg { get; set; }
		/// <summary> バリデーション区分取得 </summary>
		private string GetValidationStatement
		{
			get
			{
				// グローバル対応なし、半角入力不可能もしくは日本国の住所の場合、日本向けのバリデーション区分を返す
				if ((Constants.GLOBAL_OPTION_ENABLE == false)
					|| (Constants.HALFWIDTH_CHARACTER_INPUTABLE == false)
					|| ((this.Data.ContainsKey(Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE))
						&& ((string)this.Data[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE] == Constants.COUNTRY_ISO_CODE_JP)))
				{
					return "UserInsertUpdate";
				}
				// グローバル向かいバリデーション区分を返す
				return "UserInsertUpdateGlobal";
			}
		}
	}
}
