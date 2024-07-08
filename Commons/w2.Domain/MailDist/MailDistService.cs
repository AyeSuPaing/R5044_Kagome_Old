/*
=========================================================================================================
  Module      : メール配信サービス (MailDistService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Transactions;

namespace w2.Domain.MailDist
{
	/// <summary>
	/// メール配信サービス
	/// </summary>
	public class MailDistService : ServiceBase
	{
		#region +GetText メール配信文章取得
		/// <summary>
		/// メール配信文章取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <returns>モデル</returns>
		public MailDistTextModel GetText(string deptId, string mailtextId)
		{
			using (var repository = new MailDistRepository())
			{
				var model = repository.GetText(deptId, mailtextId);
				return model;
			}
		}
		#endregion

		#region +GetTextAll メール配信文章取得(すべて)
		/// <summary>
		/// メール配信文章取得(すべて)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		public MailDistTextModel[] GetTextAll(string deptId)
		{
			using (var repository = new MailDistRepository())
			{
				var result = repository.GetTextAll(deptId);
				return result;
			}
		}
		#endregion

		#region +GetMailDistTextContainGlobalSetting グローバル設定を含めてメール配信文章情報取得
		/// <summary>
		/// グローバル設定を含めてメール配信文章情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <returns>モデル</returns>
		public MailDistTextModel[] GetMailDistTextContainGlobalSetting(string deptId, string mailtextId)
		{
			using (var repository = new MailDistRepository())
			{
				var result = repository.GetMailDistTextContainGlobalSetting(deptId, mailtextId);
				return result;
			}
		}
		#endregion

		#region +GetTextDataViewByLanguageCode メール配信文章DataView取得(言語コード指定)
		/// <summary>
		/// メール配信文章DataView取得(言語コード指定)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		public DataView GetTextDataViewByLanguageCode(string deptId, string mailtextId, string languageCode, string languageLocaleId)
		{
			using (var repository = new MailDistRepository())
			{
				var dv = repository.GetTextDataViewByLanguageCode(deptId, mailtextId, languageCode, languageLocaleId);
				return dv;
			}
		}
		#endregion

		#region +GetTextGlobalSettingDataView メール配信文章グローバル設定DataView取得
		/// <summary>
		/// メール配信文章グローバル設定DataView取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		public DataView GetTextGlobalSettingDataView(string deptId, string mailtextId, string languageCode, string languageLocaleId)
		{
			using (var repository = new MailDistRepository())
			{
				var dv = repository.GetTextGlobalSettingDataView(deptId, mailtextId, languageCode, languageLocaleId);
				return dv;
			}
		}
		#endregion

		#region +InsertTextGlobalSetting メール配信文章グローバル設定登録
		/// <summary>
		/// メール配信文章グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertTextGlobalSetting(MailDistTextGlobalModel model)
		{
			using (var repository = new MailDistRepository())
			{
				repository.InsertTextGlobalSetting(model);
			}
		}
		#endregion

		#region +UpdateTextGlobalSetting メール配信文章グローバル設定更新
		/// <summary>
		/// メール配信文章グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int UpdateTextGlobalSetting(MailDistTextGlobalModel model)
		{
			using (var repository = new MailDistRepository())
			{
				var result = repository.UpdateTextGlobalSetting(model);
				return result;
			}
		}
		#endregion
		
		#region +CheckLanguageCodeDupulication 言語コード重複登録チェック
		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>true:重複登録なし、false:重複登録あり</returns>
		public bool CheckLanguageCodeDupulication(string deptId, string mailtextId, string languageCode, string languageLocaleId)
		{
			using (var repository = new MailDistRepository())
			{
				var result = repository.CheckLanguageCodeDupulication(deptId, mailtextId, languageCode, languageLocaleId);
				return (result.Any() == false);
			}
		}
		#endregion

		#region +DeleteTextGlobalSetting メール配信文章グローバル設定削除
		/// <summary>
		/// メール配信文章グローバル設定削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		public void DeleteTextGlobalSetting(string deptId, string mailtextId)
		{
			using (var repository = new MailDistRepository())
			{
				var result = repository.DeleteTextGlobalSetting(deptId, mailtextId);
			}
		}
		#endregion
	}
}
