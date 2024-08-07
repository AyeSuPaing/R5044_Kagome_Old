/*
=========================================================================================================
  Module      : SMSリポジトリ (GlobalSMSRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.GlobalSMS
{
	/// <summary>
	/// SMSリポジトリ
	/// </summary>
	internal class GlobalSMSRepository : RepositoryBase
	{
		/// <returns>クエリ用XML</returns>
		private const string XML_KEY_NAME = "GlobalSMS";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal GlobalSMSRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal GlobalSMSRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		/// <summary>
		/// SMSテンプレートアップサート
		/// </summary>
		/// <param name="model">対象モデル</param>
		/// <returns>処理件数</returns>
		internal int UpsertSMSTemplate(GlobalSMSTemplateModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpsertSMSTemplate", model.DataSource);
			return result;
		}

		/// <summary>
		/// SMSテンプレート取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		/// <returns>SMSテンプレート</returns>
		internal GlobalSMSTemplateModel[] GetSmsTemplates(string shopId, string mailTemplateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID, mailTemplateId},
			};
			var dv = Get(XML_KEY_NAME, "GetSmsTemplates", ht);
			return dv.Cast<DataRowView>().Select(x => new GlobalSMSTemplateModel(x)).ToArray();
		}

		/// <summary>
		/// SMSテンプレート取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		/// <param name="phoneCarrier">キャリア</param>
		/// <returns>SMSテンプレート</returns>
		internal GlobalSMSTemplateModel GetSmsTemplate(string shopId, string mailTemplateId, string phoneCarrier)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID, mailTemplateId},
				{Constants.FIELD_GLOBALSMSTEMPLATE_PHONE_CARRIER,phoneCarrier },
			};
			var dv = Get(XML_KEY_NAME, "GetSmsTemplate", ht);

			if (dv.Count == 0)
			{
				return null;
			}

			return new GlobalSMSTemplateModel(dv[0]);
		}

		/// <summary>
		/// SMSステータス登録
		/// </summary>
		/// <param name="state">登録モデル</param>
		/// <returns>処理件数</returns>
		internal int RegisterState(GlobalSMSStatusModel state)
		{
			var result = Exec(XML_KEY_NAME, "RegisterState", state.DataSource);
			return result;
		}

		/// <summary>
		/// SMSステータス更新
		/// </summary>
		/// <param name="state">更新モデル</param>
		/// <returns>処理件数</returns>
		internal int UpdateState(GlobalSMSStatusModel state)
		{
			var result = Exec(XML_KEY_NAME, "UpdateState", state.DataSource);
			return result;
		}

		/// <summary>
		/// 指定時間経過したSMSステータス取得
		/// </summary>
		/// <param name="timeLimitHours">指定時間</param>
		/// <returns>SMSステータス</returns>
		internal GlobalSMSStatusModel[] GetTimeOverStatus(int timeLimitHours)
		{
			var ht = new Hashtable
			{
				{"time_limit_hours", timeLimitHours},
			};
			var dv = Get(XML_KEY_NAME, "GetTimeOverStatus", ht);
			return dv.Cast<DataRowView>().Select(x => new GlobalSMSStatusModel(x)).ToArray();
		}

		/// <summary>
		/// 指定日数を超過したSMSステータスクリーニング
		/// </summary>
		/// <param name="cleaningDays">指定日数</param>
		/// <returns>処理件数</returns>
		internal int CleaningStatusData(int cleaningDays)
		{
			var ht = new Hashtable
			{
				{"cleaning_days", cleaningDays},
			};

			var result = Exec(XML_KEY_NAME, "CleaningStatusData", ht);
			return result;
		}

		/// <summary>
		/// SMSエラーポイントインクリメント
		/// </summary>
		/// <param name="globalTelNo">インクリメント対象の電話番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>処理件数</returns>
		internal int IncrementSmsErrorPoint(string globalTelNo, string lastChanged)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SMSERRORPOINTGLOBALTELNO_GLOBAL_TEL_NO, globalTelNo},
				{Constants.FIELD_SMSERRORPOINTGLOBALTELNO_LAST_CHANGED, lastChanged},
			};

			var result = Exec(XML_KEY_NAME, "IncrementSmsErrorPoint", ht);
			return result;
		}

		/// <summary>
		/// SMSステータス削除
		/// </summary>
		/// <param name="messageId">メッセージID</param>
		/// <returns>処理件数</returns>
		internal int DeleteStatus(string messageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_GLOBALSMSSTATUS_MESSAGE_ID, messageId},
			};

			var result = Exec(XML_KEY_NAME, "DeleteStatus", ht);
			return result;
		}

		/// <summary>
		/// SMSエラーポイント取得
		/// </summary>
		/// <param name="globalTelNo">電話番号</param>
		/// <returns>SMSエラーポイント</returns>
		internal SMSErrorPointGlobalTelNoModel GetErrorPoint(string globalTelNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SMSERRORPOINTGLOBALTELNO_GLOBAL_TEL_NO, globalTelNo},
			};
			var dv = Get(XML_KEY_NAME, "GetErrorPoint", ht);

			if (dv.Count == 0)
			{
				return null;
			}

			return new SMSErrorPointGlobalTelNoModel(dv[0]);
		}

		/// <summary>
		/// SMS配信文章アップサート
		/// </summary>
		/// <param name="model">SMS配信文章</param>
		/// <returns>処理件数</returns>
		internal int UpsertSMSDistText(GlobalSMSDistTextModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpsertSMSDistText", model.DataSource);
			return result;
		}

		/// <summary>
		/// SMS配信文章取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール配信文章ID</param>
		/// <returns>SMS配信文章</returns>
		internal GlobalSMSDistTextModel[] GetSmsDistTexts(string deptId, string mailtextId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_GLOBALSMSDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID, mailtextId},
			};
			var dv = Get(XML_KEY_NAME, "GetSmsDistTexts", ht);
			return dv.Cast<DataRowView>().Select(x => new GlobalSMSDistTextModel(x)).ToArray();
		}

		/// <summary>
		/// SMSテンプレート削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		/// <returns>処理件数</returns>
		internal int DeleteSmsTemplates(string shopId, string mailTemplateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID, shopId},
				{Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID, mailTemplateId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteSmsTemplates", ht);
			return result;
		}

		/// <summary>
		/// SMS配信文章削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール配信文章ID</param>
		/// <returns>処理件数</returns>
		internal int DeleteSmsDistTexts(string deptId, string mailtextId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_GLOBALSMSDISTTEXT_DEPT_ID, deptId},
				{Constants.FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID, mailtextId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteSmsDistTexts", ht);
			return result;
		}
	}
}
