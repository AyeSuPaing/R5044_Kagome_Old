/*
=========================================================================================================
  Module      : SMSサービス (GlobalSMSService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

//using w2.Domain.GlobalSMSStatus.Helper;

namespace w2.Domain.GlobalSMS
{
	/// <summary>
	/// SMSサービス
	/// </summary>
	public class GlobalSMSService : ServiceBase
	{
		/// <summary>
		/// SMSテンプレートアップサート
		/// </summary>
		/// <param name="model">対象モデル</param>
		/// <returns>処理件数</returns>
		public int UpsertSMSTemplate(GlobalSMSTemplateModel model)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.UpsertSMSTemplate(model);
			}
		}

		/// <summary>
		/// SMSテンプレート取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		/// <returns>SMSテンプレート</returns>
		public GlobalSMSTemplateModel[] GetSmsTemplates(string shopId, string mailTemplateId)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.GetSmsTemplates(shopId, mailTemplateId);
			}
		}

		/// <summary>
		/// SMSテンプレート取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		/// <param name="phoneCarrier">キャリア</param>
		/// <returns>SMSテンプレート</returns>
		public GlobalSMSTemplateModel GetSmsTemplate(string shopId, string mailTemplateId, string phoneCarrier)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.GetSmsTemplate(shopId, mailTemplateId, phoneCarrier);
			}
		}

		/// <summary>
		/// SMSステータス登録
		/// </summary>
		/// <param name="state">登録モデル</param>
		/// <returns>処理件数</returns>
		public int RegisterState(GlobalSMSStatusModel state)
		{
			using (var repository = new GlobalSMSRepository())
			{
				var result = repository.RegisterState(state);
				return result;
			}
		}

		/// <summary>
		/// SMSステータス更新
		/// </summary>
		/// <param name="state">更新モデル</param>
		/// <returns>処理件数</returns>
		public int UpdateState(GlobalSMSStatusModel state)
		{
			using (var repository = new GlobalSMSRepository())
			{
				var result = repository.UpdateState(state);
				return result;
			}
		}

		/// <summary>
		/// 指定日数を超過したSMSステータスクリーニング
		/// </summary>
		/// <param name="cleaningDays">指定日数</param>
		/// <returns>処理件数</returns>
		public int CleaningStatusData(int cleaningDays)
		{
			using (var repository = new GlobalSMSRepository())
			{
				var result = repository.CleaningStatusData(cleaningDays);
				return result;
			}
		}

		/// <summary>
		/// SMSステータス削除
		/// </summary>
		/// <param name="messageId">メッセージID</param>
		/// <returns>処理件数</returns>
		public int DeleteState(string messageId)
		{
			using (var repository = new GlobalSMSRepository())
			{
				var result = repository.DeleteStatus(messageId);
				return result;
			}
		}

		/// <summary>
		/// 一定時間経過したもののSMSエラーポイントインクリメント
		/// </summary>
		/// <param name="timeLimitHours">指定時間</param>
		/// <param name="lastChanged">最終更新者</param>
		public void IncrementSmsErrorPointTimeOver(int timeLimitHours, string lastChanged)
		{
			using (var repository = new GlobalSMSRepository())
			{
				var target = repository.GetTimeOverStatus(timeLimitHours);
				foreach (var state in target)
				{
					repository.IncrementSmsErrorPoint(state.GlobalTelNo, lastChanged);
					repository.DeleteStatus(state.MessageId);
				}
			}
		}

		/// <summary>
		/// SMSエラーポイント取得
		/// </summary>
		/// <param name="globalTelNo">電話番号</param>
		/// <returns>SMSエラーポイント</returns>
		public SMSErrorPointGlobalTelNoModel GetErrorPoint(string globalTelNo)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.GetErrorPoint(globalTelNo);
			}
		}

		/// <summary>
		/// SMS配信文章アップサート
		/// </summary>
		/// <param name="model">SMS配信文章</param>
		/// <returns>処理件数</returns>
		public int UpsertSMSDistText(GlobalSMSDistTextModel model)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.UpsertSMSDistText(model);
			}
		}

		/// <summary>
		/// SMS配信文章取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール配信文章ID</param>
		/// <returns>SMS配信文章</returns>
		public GlobalSMSDistTextModel[] GetSmsDistTexts(string deptId, string mailtextId)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.GetSmsDistTexts(deptId, mailtextId);
			}
		}

		/// <summary>
		/// SMSテンプレート削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		/// <returns>処理件数</returns>
		public int DeleteSmsTemplates(string shopId, string mailTemplateId)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.DeleteSmsTemplates(shopId, mailTemplateId);
			}
		}

		/// <summary>
		/// SMS配信文章削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailtextId">メール配信文章ID</param>
		/// <returns>処理件数</returns>
		public int DeleteSmsDistTexts(string deptId, string mailtextId)
		{
			using (var repository = new GlobalSMSRepository())
			{
				return repository.DeleteSmsDistTexts(deptId, mailtextId);
			}
		}
	}
}
