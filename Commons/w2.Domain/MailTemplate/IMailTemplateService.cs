/*
=========================================================================================================
  Module      : メールテンプレートサービスのインターフェース (IMailTemplateService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;

namespace w2.Domain.MailTemplate
{
	/// <summary>
	/// メールテンプレートサービスのインターフェース
	/// </summary>
	public interface IMailTemplateService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		MailTemplateModel Get(
			string shopId,
			string mailId,
			string languageCode = null,
			string languageLocaleId = null);

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		MailTemplateModel[] GetAll(string shopId);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(Hashtable param);

		/// <summary>
		/// 検索ヒット件数取得(指定カテゴリを除く)
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="mailCategories">メールカテゴリ</param>
		/// <returns>件数</returns>
		int GetSearchHitCountExcludeCategory(Hashtable param, string[] mailCategories);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>メールテンプレートリスト</returns>
		MailTemplateModel[] Search(Hashtable param);

		/// <summary>
		/// 検索(指定カテゴリを除く)
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="mailCategories">メールカテゴリ</param>
		/// <returns>メールテンプレートリスト</returns>
		MailTemplateModel[] SearchExcludeCategory(Hashtable param, string[] mailCategories);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(MailTemplateModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(MailTemplateModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		void Delete(string shopId, string mailId);

		/// <summary>
		/// カテゴリからのメールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailCategories">メールカテゴリ列</param>
		/// <returns>メールテンプレートモデル列</returns>
		MailTemplateModel[] GetMailTemplateByCategory(string shopId, string[] mailCategories);

		/// <summary>
		/// 言語コードで取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		MailTemplateModel GetByLanguageCode(
			string shopId,
			string mailId,
			string languageCode,
			string languageLocaleId);

		/// <summary>
		/// グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		void InsertGlobalSetting(MailTemplateModel model);

		/// <summary>
		/// グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		int UpdateGlobalSetting(MailTemplateModel model);

		/// <summary>
		/// グローバル設定取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>メールテンプレートグローバル設定情報</returns>
		MailTemplateModel GetGlobalSetting(
			string shopId,
			string mailId,
			string languageCode,
			string languageLocaleId);

		/// <summary>
		/// グローバル設定削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		void DeleteGlobalSetting(string shopId, string mailId);

		/// <summary>
		/// 指定された言語設定を削除
		/// </summary>
		/// <param name="model">モデル</param>
		void DeleteSpecifiedLanguageSetting(MailTemplateModel model);

		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>true:重複登録なし、false:重複登録あり</returns>
		bool CheckLanguageCodeDupulication(
			string shopId,
			string mailId,
			string languageCode,
			string languageLocaleId);

		/// <summary>
		/// グローバル設定を含めてメールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <returns>メールテンプレート情報</returns>
		MailTemplateGlobalModel[] GetMailTemplateContainGlobalSetting(string shopId, string mailId);
	}
}
