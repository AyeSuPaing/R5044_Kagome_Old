/*
=========================================================================================================
  Module      : 定期購入メールの送信タイミング管理クラス (FixedPurchaseMailSendTiming.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Domain.FixedPurchase;
using w2.Domain.TaskSchedule;

namespace w2.App.Common.Order.FixedPurchase
{
	/// <summary>
	/// 定期購入メールの送信タイミング管理クラス
	/// 送信NG期間に基づいて送信設定を行う
	/// </summary>
	public class FixedPurchaseMailSendTiming
	{
		/// <summary>送信OK or NG期間ステータスの各パラメータ</summary>
		public enum TimeZoneStatusEnum { Ok, Ng };

		/// <summary>
		/// コンストラクタ
		/// 
		///	<para>送信NG期間</para> 
		///	<para>■即時の場合</para> 
		///	<para>	空文字を指定</para> 
		///	<para>■送信NG期間を指定する場合</para> 
		///	<para>	各期間の始めと終わりを[-(ハイフン)]区切り、</para> 
		///	<para>	各期間を[,(カンマ)]で区切る。</para> 
		///	<para>	[例]</para> 
		///	<para>	"00:00-12:00,15:00-23:59"</para> 
		///	<para>	この場合、00:00-12:00と15:00-23:59が送信NG期間となります</para> 
		/// </summary>
		/// <param name="sendNgTimeZoneString">送信NG期間の文字列</param>
		public FixedPurchaseMailSendTiming(string sendNgTimeZoneString)
		{
			this.SendNgTimeZoneString = sendNgTimeZoneString;
			this.SendNgTimeZoneDatetimeList = ConvertSendNgTimeZoneForDatetimeList(this.SendNgTimeZoneString);
			this.TimeZoneStatus = TimeZoneStatusEnum.Ok;
		}

		/// <summary>
		/// FixedPurchaseBatchMailTmpLogにメール送信用のIDを追加
		/// </summary>
		/// <param name="masterId">注文成功メールの場合は"order_id",失敗メールの場合は"fixed_purchase_id"を指定</param>
		/// <param name="masterType">送信成功の場合は"order",失敗の場合は"error"</param>
		public void InsertFixedPurchaseBatchMailTmpLog(string masterId, string masterType)
		{
			var model = new FixedPurchaseBatchMailTmpLogModel()
			{
				MasterId = masterId,
				MasterType = masterType,
				ActionMasterId = this.ActionMasterId
			};
			new FixedPurchaseService().InsertFixedPurchaseBatchMailTmpLog(model);
		}

		/// <summary>
		/// タスクスケジュールにスケジュールを設定
		/// OK期間の場合はスケジュールは追加されません。
		/// NG期間に実行されるか、OK期間→NG期間に切り替わった場合にスケジュールを追加
		/// </summary>
		/// <param name="actionKbn">実行区分</param>
		public void SettingTaskSchedule(string actionKbn)
		{
			var timeZoneStatus = ChackTimeZoneStatus();

			if (this.TimeZoneStatus == TimeZoneStatusEnum.Ok && timeZoneStatus == TimeZoneStatusEnum.Ng)
			{
				//スケジュール実行日時の確定
				this.ScheduleDateTime = NextTaskScheduleTime();

				this.ActionMasterId = DateTime.Now.ToString("yyyyMMddhhmmssfff");

				var taskModel = new TaskScheduleModel();
				taskModel.ScheduleDate = this.ScheduleDateTime;
				taskModel.DeptId = "";
				taskModel.ActionKbn = actionKbn;
				taskModel.ActionMasterId = this.ActionMasterId;
				taskModel.ActionNo = 1;
				taskModel.PrepareStatus = Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_INIT;
				taskModel.ExecuteStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT;
				taskModel.DateBegin = this.ScheduleDateTime;
				taskModel.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
				taskModel.Progress = "";
				taskModel.StopFlg = "";
				new TaskScheduleService().Insert(taskModel);
			}
			//現在のOKorNG期間ステータスの更新
			this.TimeZoneStatus = timeZoneStatus;
		}


		/// <summary>
		/// 送信NG期間文字列をDictionary["start" or "end", 時間]をValueに持つList配列に変換
		/// </summary>
		/// <param name="sendNgTimeZoneString">送信NG期間の文字列</param>
		/// <returns>送信NG期間の配列</returns>
		private List<Dictionary<string, DateTime>> ConvertSendNgTimeZoneForDatetimeList(string sendNgTimeZoneString)
		{
			var result = new List<Dictionary<string, DateTime>> { };

			if (string.IsNullOrEmpty(sendNgTimeZoneString) == false)
			{
				foreach (string dateTimeString in sendNgTimeZoneString.Split(','))
				{
					var datetime = dateTimeString.Split('-');

					DateTime startTime;
					DateTime endTime;
					if (DateTime.TryParse(datetime[0], out startTime) == false
						|| DateTime.TryParse(datetime[1], out endTime) == false)
					{
						throw new Exception("送信NG期間のパースに失敗しました。");
					}

					var tmp = new Dictionary<string, DateTime>
				{
					{"start", startTime},
					{"end", endTime},
				};
					result.Add(tmp);
				}
			}
			return result;
		}

		/// <summary>
		/// 即時送信が可能か判定
		/// </summary>
		/// <returns>送信可能な場合はTimeZoneStatusEnum.Ok,不可能な場合はTimeZoneStatusEnum.Ng</returns>
		private TimeZoneStatusEnum ChackTimeZoneStatus()
		{
			var timeZoneStatus = TimeZoneStatusEnum.Ok;

			if (string.IsNullOrEmpty(this.SendNgTimeZoneString) == false)
			{
				foreach (var datetime in this.SendNgTimeZoneDatetimeList)
				{
					if (WithInTime(DateTime.Now, datetime["start"], datetime["end"]))
					{
						timeZoneStatus = TimeZoneStatusEnum.Ng;
						break;
					}
				}
			}
			return timeZoneStatus;
		}

		/// <summary>
		/// スケジュール実行日時の算出
		/// </summary>
		/// <returns>スケジュール実行日時</returns>
		private DateTime NextTaskScheduleTime()
		{
			var scheduleDateTime = DateTime.Now;

			//当日分のNG期間判定
			foreach (var datetime in this.SendNgTimeZoneDatetimeList)
			{
				if (WithInTime(DateTime.Now, datetime["start"], datetime["end"]))
				{
					scheduleDateTime = datetime["end"].AddMinutes(1);
					break;
				}
			}

			//明日分のNG期間判定
			foreach (var datetime in this.SendNgTimeZoneDatetimeList)
			{
				if (WithInTime(scheduleDateTime, datetime["start"].AddDays(1), datetime["end"].AddDays(1)))
				{
					scheduleDateTime = datetime["end"].AddDays(1).AddMinutes(1);
					break;
				}
			}
			return scheduleDateTime;
		}

		/// <summary>
		/// 指定日時が期間以内かどうかを判定
		/// </summary>
		/// <param name="compareTime">指定日時</param>
		/// <param name="minTime">期間の最小日時</param>
		/// <param name="maxTime">期間の最大日時</param>
		/// <returns></returns>
		private bool WithInTime(DateTime compareTime, DateTime minTime, DateTime maxTime)
		{
			var result = ((compareTime.CompareTo(minTime) == 1 && compareTime.CompareTo(maxTime) == -1)
				|| compareTime.CompareTo(minTime) == 0
				|| compareTime.CompareTo(maxTime) == 0);
			return result;
		}

		/// <summary>現在のOKorNG期間ステータス</summary>
		public TimeZoneStatusEnum TimeZoneStatus { get; set; }
		/// <summary>スケジュール実行日時</summary>
		public DateTime ScheduleDateTime { get; set; }
		/// <summary>NG期間文字列</summary>
		private string SendNgTimeZoneString { get; set; }
		/// <summary>NG期間の配列</summary>
		private List<Dictionary<string, DateTime>> SendNgTimeZoneDatetimeList { get; set; }
		/// <summary>スケジュールの実行ID</summary>
		private string ActionMasterId { get; set; }
	}
}
