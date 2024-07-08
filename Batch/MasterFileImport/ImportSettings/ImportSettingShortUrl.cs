/*
=========================================================================================================
  Module      : ショートURL情報取込設定クラス(ImportSettingShortUrl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.ShortUrl;
using w2.App.Common.RefreshFileManager;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.ShortUrl;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingShortUrl : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_SHORTURL_SHOP_ID, Constants.FIELD_SHORTURL_SHORT_URL };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_SHORTURL_SURL_NO, Constants.FIELD_SHORTURL_DATE_CHANGED, Constants.FIELD_SHORTURL_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_SHORTURL_SHOP_ID, Constants.FIELD_SHORTURL_SHORT_URL };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_SHORTURL_SHOP_ID, Constants.FIELD_SHORTURL_SHORT_URL };
		/// <summary> ドメイン付加ショートURLキー </summary>
		private const string FIELD_SHORTURL_SHORT_URL_DOMAIN_ADDED = "short_url_domain_add";
		/// <summary> ドメイン付加ロングURLキー </summary>
		private const string FIELD_SHORTURL_LONG_URL_DOMAIN_ADDED = "long_url_domain_add";


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingShortUrl(string shopId)
			: base(
			shopId,
			Constants.TABLE_SHORTURL,
			Constants.TABLE_WROKSHORTURL,
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
					case Constants.FIELD_SHORTURL_SHOP_ID:
					case Constants.FIELD_SHORTURL_SHORT_URL:
					case Constants.FIELD_SHORTURL_LONG_URL:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
						break;
				}

				// http:// + ドメイン名の削除
				switch (strFieldName)
				{
					case Constants.FIELD_SHORTURL_SHORT_URL:
					case Constants.FIELD_SHORTURL_LONG_URL:
						this.Data[strFieldName] = ShortUrl.RemoveProtocolAndDomain(this.Data[strFieldName].ToString());
						break;
				}
			}

			// チェック用にhttp://ドメイン名を付加したURLを格納する
			SetDomainAddedUrl();
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
					strCheckKbn = "ShortUrlInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "ShortUrlDelete";
					lNecessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder sbErrorMessages = new StringBuilder();
			StringBuilder sbWarningMessages = new StringBuilder();
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

			// ショートURLとロングURLに同一値チェック
			if ((string.IsNullOrEmpty((string)this.Data[Constants.FIELD_SHORTURL_SHORT_URL]) == false)
				&& (string.IsNullOrEmpty((string)this.Data[Constants.FIELD_SHORTURL_LONG_URL]) == false)
			    && ((string)this.Data[Constants.FIELD_SHORTURL_SHORT_URL] == (string)this.Data[Constants.FIELD_SHORTURL_LONG_URL]))
			{
				sbErrorMessages.Append(MessageManager.GetMessages(Constants.INPUTCHECK_SHORTURL_IS_EQUAL_TO_LONGURL));
			}

			// ショートURLとロングURLでの別ドメインチェック
			if ((this.Data.ContainsKey(Constants.FIELD_SHORTURL_LONG_URL))
				&& (IsSameDomain(this.DomainAddedShortUrl, this.DomainAddedLongUrl) == false))
			{
				sbWarningMessages.Append(MessageManager.GetMessages(Constants.INPUTCHECK_SHORTURL_IS_DIFFERENT_DOMAIN_WITH_LONGURL));
			}

			// ショートURLのドメインが正しいかチェック
			if ((this.Data.ContainsKey(Constants.FIELD_SHORTURL_LONG_URL))
				&& (ShortUrl.IsSiteDomain(this.DomainAddedShortUrl) == false))
			{
				sbWarningMessages.Append(MessageManager.GetMessages(Constants.INPUTCHECK_SHORTURL_IS_SITE_DOMAIN).Replace("@@ 1 @@", ShortUrl.URL_TOP));
			}

			// 入力チェック
			string errorMessage = Validator.Validate(strCheckKbn, this.Data);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_SHORTURL_SHORT_URL);
			}

			// エラーメッセージ格納
			if (sbErrorMessages.Length != 0)
			{
				this.ErrorMessages = sbErrorMessages.ToString();
			}
			// 警告メッセージ格納
			if (sbWarningMessages.Length != 0)
			{
				this.WarningMessages = sbWarningMessages.ToString();
			}

			// チェック用に格納したhttp://ドメイン名を付加したURLを削除する
			RemoveDomainAddedUrl();
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			var errorMessage = new StringBuilder();

			// ショートURL重複チェック
			var shortUrlInfo = new ShortUrlService().GetShortUrlForDuplicationShortUrl();
			foreach (var shortUrl in shortUrlInfo)
			{
				errorMessage.Append((errorMessage.Length != 0) ? "\r\n" : "");
				errorMessage.Append("ショートURLが重複しています");
				errorMessage.Append("（");
				errorMessage.Append("ショートURL：").Append(shortUrl.Key);
				errorMessage.Append("、");
				errorMessage.Append("件数：").Append(shortUrl.Value);
				errorMessage.Append("）");
			}
			return errorMessage.ToString();
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
			this.InsertUpdateSql = CreateInsertUpdateSql(this.TableName);
			// Delete文
			this.DeleteSql = CreateDeleteSql(this.TableName);
			// Insert/Update文 （ワークテーブル用）
			this.InsertUpdateWorkSql = CreateInsertUpdateSql(this.WorkTableName);
			// Delete文 （ワークテーブル用）			
			this.DeleteWorkSql = CreateDeleteSql(this.WorkTableName);
		}

		/// <summary>
		/// 完了時実行処理
		/// </summary>
		public override void OnCompleteExecute()
		{
			base.OnCompleteExecute();

			// リフレッシュファイル更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ShortUrl).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// ワークテーブル初期化文作成
		/// </summary>
		/// <remarks>
		/// ・店舗ID,ショートURLだけワークテーブルに移行
		/// ・surl_no(Identity)の項目があるためINSERT SELECTではなくINSERT INTOを使用する。
		/// </remarks>
		/// <returns>ワークテーブル初期化文</returns>
		protected new string CreateInitlializeWorkTableSql()
		{
			StringBuilder sbResult = new StringBuilder();

			// ワークテーブルをTruncate
			sbResult.Append(this.TruncateWorkTableSql).Append(" \n");

			// INSERT INTO での追加カラム準備(更新キー)
			StringBuilder sbUpdateKey = new StringBuilder();
			foreach (string strUpdateKey in this.UpdateKeys)
			{
				if (sbUpdateKey.Length != 0)
				{
					sbUpdateKey.Append(",");
				}
				sbUpdateKey.Append(strUpdateKey);
			}

			// ワークテーブルに対象テーブルの情報をInsert
			sbResult.Append("INSERT INTO ").Append(this.WorkTableName).Append("(").Append(sbUpdateKey.ToString()).Append(")");
			sbResult.Append(" SELECT ").Append(sbUpdateKey.ToString()).Append(" FROM ").Append(this.TableName).Append(" \n");

			return sbResult.ToString();
		}

		/// <summary>
		/// 同一ドメインチェック
		/// </summary>
		/// <param name="strShortUrl">ショートURL</param>
		/// <param name="strLongUrl">ロングURL</param>
		/// <returns>同一可否</returns>
		private bool IsSameDomain(string strShortUrl, string strLongUrl)
		{
			// 値が無い場合はfalse
			if ((strShortUrl + strLongUrl).Length == 0)
			{
				return false;
			}
			// ドメインが異なる場合はfalse
			else if (GetDomain(strShortUrl) != GetDomain(strLongUrl))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// URLからドメインのみ抜き出して返す
		/// </summary>
		private string GetDomain(string strUrl)
		{
			foreach (Match mDomainString in Regex.Matches(strUrl, @":\/\/([^/ ]*)\/?"))
			{
				return mDomainString.Value.ToString();
			}

			// ドメイン部分が見つからなかった場合は空文字を返す
			return "";
		}

		/// <summary>
		/// ドメイン付加URLの追加
		/// </summary>
		protected void SetDomainAddedUrl()
		{
			this.Data[FIELD_SHORTURL_SHORT_URL_DOMAIN_ADDED] = ShortUrl.AddProtocolAndDomainWithReplace(this.Data[Constants.FIELD_SHORTURL_SHORT_URL].ToString());
			this.Data[FIELD_SHORTURL_LONG_URL_DOMAIN_ADDED] = ShortUrl.AddProtocolAndDomainWithReplace(this.Data[Constants.FIELD_SHORTURL_LONG_URL].ToString());
		}

		/// <summary>
		/// ドメイン付加URLの削除
		/// </summary>
		protected void RemoveDomainAddedUrl()
		{
			this.Data.Remove(FIELD_SHORTURL_SHORT_URL_DOMAIN_ADDED);
			this.Data.Remove(FIELD_SHORTURL_LONG_URL_DOMAIN_ADDED);
		}

		/// <summary> ドメイン付加ショートURL </summary>
		protected string DomainAddedShortUrl
		{
			get { return this.Data[FIELD_SHORTURL_SHORT_URL_DOMAIN_ADDED].ToString(); }
		}

		/// <summary> ドメイン付加ロングURL </summary>
		protected string DomainAddedLongUrl
		{
			get { return this.Data[FIELD_SHORTURL_LONG_URL_DOMAIN_ADDED].ToString(); }
		}

	}
}
