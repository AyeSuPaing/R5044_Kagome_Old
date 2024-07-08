/*
=========================================================================================================
  Module      : メールテンプレートリポジトリ (MailTemplateRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MailTemplate
{
	/// <summary>
	/// メールテンプレートリポジトリ
	/// </summary>
	public class MailTemplateRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MailTemplate";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MailTemplateRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <returns>モデル</returns>
		public MailTemplateModel Get(string shopId, string mailId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATE_MAIL_ID, mailId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new MailTemplateModel(dv[0]);
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public MailTemplateModel[] GetAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);

			return dv.Cast<DataRowView>().Select(drv => new MailTemplateModel(drv)).ToArray();
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(Hashtable param)
		{
			var dv = base.Get(XML_KEY_NAME, "GetSearchHitCount", param);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetSearchHitCountExcludeCategory 検索ヒット件数取得(特定のカテゴリーを除く)
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="mailCategorys">メールカテゴリ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCountExcludeCategory(Hashtable param, string[] mailCategorys)
		{
			var replace = new KeyValuePair<string, string>(
				"@@ mail_categorys @@", string.Join(",", mailCategorys.Select(category => string.Format("'{0}'", category)).ToArray()));

			var dv = Get(XML_KEY_NAME, "GetSearchHitCountExcludeCategory", param, replaces: replace);
			return (int)dv[0][0];
		}
		#endregion

		#region +Search
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>メールテンプレートリスト</returns>
		internal MailTemplateModel[] Search(Hashtable param)
		{
			var dv = Get(XML_KEY_NAME, "Search", param);
			return dv.Cast<DataRowView>().Select(drv => new MailTemplateModel(drv)).ToArray();
		}
		#endregion

		#region +SearchExcludeCategory
		/// <summary>
		/// 検索(特定のカテゴリーを除く)
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="mailCategorys">メールカテゴリ</param>
		/// <returns>メールテンプレートリスト</returns>
		internal MailTemplateModel[] SearchExcludeCategory(Hashtable param, string[] mailCategorys)
		{
			if (mailCategorys.Length == 0) return new MailTemplateModel[0];

			var replace = new KeyValuePair<string, string>(
				"@@ mail_categorys @@", string.Join(",", mailCategorys.Select(category => string.Format("'{0}'", category)).ToArray()));

			var dv = Get(XML_KEY_NAME, "SearchExcludeCategory", param, replaces: replace);

			return dv.Cast<DataRowView>().Select(drv => new MailTemplateModel(drv)).ToArray();

		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MailTemplateModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(MailTemplateModel model)
		{
			return Exec(XML_KEY_NAME, "Update", model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		public int Delete(string shopId, string mailId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATE_MAIL_ID, mailId},
			};
			return Exec(XML_KEY_NAME, "Delete", ht);
		}
		#endregion

		#region +GetMailTemplateByCategory カテゴリからのメールテンプレート情報取得
		/// <summary>
		/// カテゴリからのメールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailCategorys">メールカテゴリ列</param>
		/// <returns>メールテンプレートモデル列</returns>
		public MailTemplateModel[] GetMailTemplateByCategory(string shopId, string[] mailCategorys)
		{
			if (mailCategorys.Length == 0) return new MailTemplateModel[0];

			var replace = new KeyValuePair<string, string>(
				"@@ mail_categorys @@", string.Join(",", mailCategorys.Select(category => string.Format("'{0}'", category)).ToArray()));

			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId}
			};
			var dv = Get(XML_KEY_NAME, "GetMailTemplateByCategory", ht, replaces: replace);

			return dv.Cast<DataRowView>().Select(drv => new MailTemplateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetMailTemplateContainsGlobalSettingByCategory カテゴリからのメールテンプレート情報取得(グローバル設定含む)
		/// <summary>
		/// カテゴリからのメールテンプレート情報取得(グローバル設定含む)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailCategorys">メールカテゴリ列</param>
		/// <returns>メールテンプレートモデル列</returns>
		internal MailTemplateModel[] GetMailTemplateContainsGlobalSettingByCategory(string shopId, string[] mailCategorys)
		{
			if (mailCategorys.Length == 0) return new MailTemplateModel[0];

			var replace = new KeyValuePair<string, string>(
				"@@ mail_categorys @@", string.Join(",", mailCategorys.Select(category => string.Format("'{0}'", category)).ToArray()));

			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId}
			};
			var dv = Get(XML_KEY_NAME, "GetMailTemplateContainsGlobalSettingByCategory", ht, replaces: replace);

			return dv.Cast<DataRowView>().Select(drv => new MailTemplateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetByLanguageCode 言語コードで取得
		/// <summary>
		/// 言語コードで取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		internal MailTemplateModel GetByLanguageCode(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATE_MAIL_ID, mailId},
				{Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILTEMPLATE_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetByLanguageCode", ht);
			if (dv.Count == 0) return null;
			return new MailTemplateModel(dv[0]);
		}
		#endregion

		#region ~InsertGlobalSetting グローバル設定登録
		/// <summary>
		/// グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertGlobalSetting(MailTemplateModel model)
		{
			Exec(XML_KEY_NAME, "InsertGlobalSetting", model.DataSource);
		}
		#endregion

		#region ~UpdateGlobalSetting グローバル設定更新
		/// <summary>
		/// グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateGlobalSetting(MailTemplateModel model)
		{
			return Exec(XML_KEY_NAME, "UpdateGlobalSetting", model.DataSource);
		}
		#endregion

		#region ~DeleteGlobalSetting グローバル設定削除
		/// <summary>
		/// グローバル設定削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		internal int DeleteGlobalSetting(string shopId, string mailId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATE_MAIL_ID, mailId},
			};
			return Exec(XML_KEY_NAME, "DeleteGlobalSetting", ht);
		}
		#endregion

		#region ~DeleteSpecifiedLanguageSetting 指定された言語設定を削除
		/// <summary>
		/// 指定された言語設定を削除
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteSpecifiedLanguageSetting(MailTemplateModel model)
		{
			return Exec(XML_KEY_NAME, "DeleteSpecifiedLanguageSetting", model.DataSource);
		}
		#endregion

		#region ~CheckLanguageCodeDupulication 言語コード重複登録チェック
		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>メールテンプレート情報</returns>
		internal MailTemplateModel[] CheckLanguageCodeDupulication(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ID, mailId},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetMailTemplateContainGlobalSetting", ht);
			return dv.Cast<DataRowView>().Select(drv => new MailTemplateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetGlobalSetting グローバル設定取得
		/// <summary>
		/// グローバル設定取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		internal MailTemplateModel GetGlobalSetting(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ID, mailId},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetGlobalSetting", ht);
			if (dv.Count == 0) return null;
			return new MailTemplateModel(dv[0]);
		}
		#endregion

		#region ~GetMailTemplateContainGlobalSetting グローバル設定を含めてメールテンプレート情報取得
		/// <summary>
		/// グローバル設定を含めてメールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <returns>メールテンプレート情報</returns>
		internal MailTemplateGlobalModel[] GetMailTemplateContainGlobalSetting(string shopId, string mailId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ID, mailId},
			};
			var dv = Get(XML_KEY_NAME, "GetMailTemplateContainGlobalSetting", ht);
			return dv.Cast<DataRowView>().Select(drv => new MailTemplateGlobalModel(drv)).ToArray();
		}
		#endregion

		#region ~GetMailTemplateWithLanguageCode メールテンプレート情報取得(言語コード)
		/// <summary>
		/// メールテンプレート情報取得(言語コード)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>メールテンプレート情報</returns>
		/// <remarks>言語コードのドロップダウン作成用のやつっぽい</remarks>
		internal MailTemplateModel GetMailTemplateWithLanguageCode(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_MAILTEMPLATE_MAIL_ID, mailId},
				{Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILTEMPLATE_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetMailTemplateWithLanguageCode", ht);
			if (dv.Count == 0) return null;
			return new MailTemplateModel(dv[0]);
		}
		#endregion
	}
}
