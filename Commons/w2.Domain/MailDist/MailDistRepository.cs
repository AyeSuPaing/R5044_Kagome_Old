/*
=========================================================================================================
  Module      : メール配信リポジトリ (MailDistRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MailDist
{
	/// <summary>
	/// メール配信リポジトリ
	/// </summary>
	internal class MailDistRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MailDist";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MailDistRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MailDistRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetText メール配信文章取得
		/// <summary>
		/// メール配信文章取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <returns>モデル</returns>
		internal MailDistTextModel GetText(string deptId, string mailtextId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, mailtextId},
			};
			var dv = Get(XML_KEY_NAME, "GetText", ht);
			if (dv.Count == 0) return null;
			return new MailDistTextModel(dv[0]);
		}
		#endregion

		#region ~GetTextAll メール配信文章取得
		/// <summary>
		/// メール配信文章取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		internal MailDistTextModel[] GetTextAll(string deptId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXT_DEPT_ID, deptId},
			};
			var dv = Get(XML_KEY_NAME, "GetTextAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new MailDistTextModel(drv)).ToArray();
		}
		#endregion

		#region ~GetMailDistTextContainGlobalSetting グローバル設定を含めてメール配信文章情報取得
		/// <summary>
		/// グローバル設定を含めてメール配信文章情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <returns>モデル</returns>
		internal MailDistTextModel[] GetMailDistTextContainGlobalSetting(string deptId, string mailtextId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, mailtextId},
			};
			var dv = Get(XML_KEY_NAME, "GetMailDistTextContainGlobalSetting", ht);
			return dv.Cast<DataRowView>().Select(drv => new MailDistTextModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTextDataViewByLanguageCode メール配信文章DataView取得(言語コード指定)
		/// <summary>
		/// メール配信文章DataView取得(言語コード指定)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		internal DataView GetTextDataViewByLanguageCode(string deptId, string mailtextId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, mailtextId},
				{Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetTextByLanguageCode", ht);
			return dv;
		}
		#endregion

		#region ~GetTextGlobalSettingDataView メール配信文章グローバル設定DataView取得
		/// <summary>
		/// メール配信文章グローバル設定DataView取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		internal DataView GetTextGlobalSettingDataView(string deptId, string mailtextId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, mailtextId},
				{Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetTextGlobalSetting", ht);
			return dv;
		}
		#endregion

		#region ~InsertTextGlobalSetting メール配信文章グローバル設定登録
		/// <summary>
		/// メール配信文章グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertTextGlobalSetting(MailDistTextGlobalModel model)
		{
			Exec(XML_KEY_NAME, "InsertTextGlobalSetting", model.DataSource);
		}
		#endregion

		#region ~UpdateTextGlobalSetting メール配信文章グローバル設定更新
		/// <summary>
		/// メール配信文章グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateTextGlobalSetting(MailDistTextGlobalModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateTextGlobalSetting", model.DataSource);
			return result;
		}
		#endregion

		#region ~CheckLanguageCodeDupulication 言語コード重複登録チェック
		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		internal MailDistTextModel[] CheckLanguageCodeDupulication(string deptId, string mailtextId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, mailtextId},
				{Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetMailDistTextContainGlobalSetting", ht);
			return dv.Cast<DataRowView>().Select(drv => new MailDistTextModel(drv)).ToArray();
		}
		#endregion

		#region ~DeleteTextGlobalSetting メール配信文章グローバル設定削除
		/// <summary>
		/// メール配信文章グローバル設定削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール文章ID</param>
		internal int DeleteTextGlobalSetting(string deptId, string mailtextId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTTEXTGLOBAL_DEPT_ID, deptId},
				{Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_ID, mailtextId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteTextGlobalSetting", ht);
			return result;
		}
		#endregion
	}
}
