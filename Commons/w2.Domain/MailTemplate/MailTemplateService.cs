/*
=========================================================================================================
  Module      : メールテンプレートサービス (MailTemplateService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;

namespace w2.Domain.MailTemplate
{
	/// <summary>
	/// メールテンプレートサービス
	/// </summary>
	public class MailTemplateService : ServiceBase, IMailTemplateService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		public MailTemplateModel Get(string shopId, string mailId, string languageCode = null, string languageLocaleId = null)
		{
			using (var repository = new MailTemplateRepository())
			{
				MailTemplateModel model = null;

				// 言語コードの指定が無い場合は、メールテンプレートIDのみ指定して取得
				if (string.IsNullOrEmpty(languageCode) || string.IsNullOrEmpty(languageLocaleId))
				{
					model = repository.Get(shopId, mailId);
				}
				else
				{
					model = GetByLanguageCode(shopId, mailId, languageCode, languageLocaleId)
						?? GetGlobalSetting(shopId, mailId, languageCode, languageLocaleId)
							?? repository.Get(shopId, mailId);
				}
				return model;
			}
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
			using (var repository = new MailTemplateRepository())
			{
				// 取得
				var models = repository.GetAll(shopId);
				return models;
			}
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
			using (var repository = new MailTemplateRepository())
			{
				var count = repository.GetSearchHitCount(param);
				return count;
			}
		}
		#endregion

		#region +GetSearchHitCountExcludeCategory 検索ヒット件数取得(指定カテゴリを除く)
		/// <summary>
		/// 検索ヒット件数取得(指定カテゴリを除く)
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="mailCategorys">メールカテゴリ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCountExcludeCategory(Hashtable param, string[] mailCategorys)
		{
			using (var repository = new MailTemplateRepository())
			{
				var count = repository.GetSearchHitCountExcludeCategory(param, mailCategorys);
				return count;
			}
		}
		#endregion

		#region +Search
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>メールテンプレートリスト</returns>
		public MailTemplateModel[] Search(Hashtable param)
		{
			using (var repository = new MailTemplateRepository())
			{
				return repository.Search(param);
			}
		}
		#endregion

		#region +SearchExcludeCategory(指定購入カテゴリを除く)
		/// <summary>
		/// 検索(指定カテゴリを除く)
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="mailCategorys">メールカテゴリ</param>
		/// <returns>メールテンプレートリスト</returns>
		public MailTemplateModel[] SearchExcludeCategory(Hashtable param, string[] mailCategorys)
		{
			using (var repository = new MailTemplateRepository())
			{
				return repository.SearchExcludeCategory(param, mailCategorys);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MailTemplateModel model)
		{
			using (var repository = new MailTemplateRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(MailTemplateModel model)
		{
			using (var repository = new MailTemplateRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		public void Delete(string shopId, string mailId)
		{
			using (var repository = new MailTemplateRepository())
			{
				repository.Delete(shopId, mailId);
			}
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
			using (var repository = new MailTemplateRepository())
			{
				var models = repository.GetMailTemplateByCategory(shopId, mailCategorys);
				return models;
			}
		}
		#endregion

		#region +GetByLanguageCode 言語コードで取得
		/// <summary>
		/// 言語コードで取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		public MailTemplateModel GetByLanguageCode(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			using (var repository = new MailTemplateRepository())
			{
				var model = repository.GetByLanguageCode(shopId, mailId, languageCode, languageLocaleId);
				return model;
			}
		}
		#endregion

		#region +InsertGlobalSetting グローバル設定登録
		/// <summary>
		/// グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertGlobalSetting(MailTemplateModel model)
		{
			using (var repository = new MailTemplateRepository())
			{
				repository.InsertGlobalSetting(model);
			}
		}
		#endregion

		#region +UpdateGlobalSetting グローバル設定更新
		/// <summary>
		/// グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int UpdateGlobalSetting(MailTemplateModel model)
		{
			using (var repository = new MailTemplateRepository())
			{
				var result = repository.UpdateGlobalSetting(model);
				return result;
			}
		}
		#endregion

		#region +GetGlobalSetting グローバル設定取得
		/// <summary>
		/// グローバル設定取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>メールテンプレートグローバル設定情報</returns>
		public MailTemplateModel GetGlobalSetting(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			using (var repository = new MailTemplateRepository())
			{
				var model = repository.GetGlobalSetting(shopId, mailId, languageCode, languageLocaleId);
				return model;
			}
		}
		#endregion

		#region +DeleteGlobalSetting グローバル設定削除
		/// <summary>
		/// グローバル設定削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		public void DeleteGlobalSetting(string shopId, string mailId)
		{
			using (var repository = new MailTemplateRepository())
			{
				repository.DeleteGlobalSetting(shopId, mailId);
			}
		}
		#endregion

		#region +DeleteSpecifiedLanguageSetting 指定された言語設定を削除
		/// <summary>
		/// 指定された言語設定を削除
		/// </summary>
		/// <param name="model">モデル</param>
		public void DeleteSpecifiedLanguageSetting(MailTemplateModel model)
		{
			using (var repository = new MailTemplateRepository())
			{
				repository.DeleteSpecifiedLanguageSetting(model);
			}
		}
		#endregion

		#region +CheckLanguageCodeDupulication 言語コード重複登録チェック
		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>true:重複登録なし、false:重複登録あり</returns>
		public bool CheckLanguageCodeDupulication(string shopId, string mailId, string languageCode, string languageLocaleId)
		{
			using (var repository = new MailTemplateRepository())
			{
				var models = repository.CheckLanguageCodeDupulication(shopId, mailId, languageCode, languageLocaleId);
				return (models.Any() == false);
			}
		}
		#endregion

		#region +GetMailTemplateContainGlobalSetting グローバル設定を含めてメールテンプレート情報取得
		/// <summary>
		/// グローバル設定を含めてメールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <returns>メールテンプレート情報</returns>
		public MailTemplateGlobalModel[] GetMailTemplateContainGlobalSetting(string shopId, string mailId)
		{
			using (var repository = new MailTemplateRepository())
			{
				var models = repository.GetMailTemplateContainGlobalSetting(shopId, mailId);
				return models;
			}
		}
		#endregion
	}
}
