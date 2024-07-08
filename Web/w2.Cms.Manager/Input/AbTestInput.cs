/*
=========================================================================================================
  Module      : ABテスト入力クラス (AbTestInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Database.Common;
using w2.Domain.AbTest;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// ABテスト入力クラス
	/// </summary>
	public class AbTestInput : InputBase<AbTestModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AbTestInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AbTestInput(AbTestModel model)
			: this()
		{
			this.AbTestId = model.AbTestId;
			this.AbTestTitle = model.AbTestTitle;
			this.PublicStatus = model.PublicStatus;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override AbTestModel CreateModel()
		{
			var model = new AbTestModel
			{
				AbTestId = this.AbTestId,
				AbTestTitle = this.AbTestTitle,
				PublicStatus = this.PublicStatus,
				PublicStartDatetime = ParseDate(this.RangeStartDate) ?? DateTime.Now,
				PublicEndDatetime = ParseDate(this.RangeEndDate),
				Items = this.Items.Select(item => item.CreateModel()).ToArray(),
				LastChanged = "",
			};
			return model;
		}

		/// <summary>
		/// 日時変換
		/// </summary>
		/// <param name="date">年月日</param>
		/// <param name="time">時分秒</param>
		/// <returns>変換後DateTime（エラー時はnull）</returns>
		private DateTime? ParseDate(string date, string time = null)
		{
			DateTime result;
			if (DateTime.TryParse(date, out result) == false) return null;

			if (string.IsNullOrEmpty(time)) return result.Date;

			if (DateTime.TryParse(date + " " + time, out result) == false) return null;

			return result;
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <param name="isRegister">登録か</param>
		/// <returns>エラーメッセージ/空</returns>
		public string Validate(bool isRegister)
		{
			var errorMessages = new List<string>();
			if (string.IsNullOrEmpty(this.AbTestTitle))
			{
				errorMessages.Add(MessageManager.GetMessages(MessageManager.INPUTCHECK_NECESSARY)
					.Replace("@@ 1 @@", "ABテスト管理名"));
			}
			else if (this.AbTestTitle.Length > 255)
			{
				errorMessages.Add(MessageManager.GetMessages(MessageManager.INPUTCHECK_LENGTH_MAX)
				.Replace("@@ 1 @@", "ABテスト管理名")
				.Replace("@@ 2 @@", "255"));
			}

			if (string.IsNullOrEmpty(this.AbTestId))
			{
				errorMessages.Add(MessageManager.GetMessages(MessageManager.INPUTCHECK_NECESSARY)
					.Replace("@@ 1 @@", "ABテストID(URL)"));
			}
			else
			{
				if (Regex.IsMatch(this.AbTestId, @"[^(a-zA-Z0-9)\-_\.\!\*\'\(\)]+")) errorMessages.Add(WebMessages.AbTestIdFormatError);

				if (this.AbTestId.Length > 30)
					errorMessages.Add(MessageManager.GetMessages(MessageManager.INPUTCHECK_LENGTH_MAX)
						.Replace("@@ 1 @@", "ABテストID(URL)")
						.Replace("@@ 2 @@", "30"));
				if (isRegister)
				{
					var abTest = new AbTestService().Get(this.AbTestId);
					if (abTest != null) errorMessages.Add(WebMessages.AbTestIdDuplicate);
				}
			}

			if (this.Items == null)
			{
				errorMessages.Add(WebMessages.AbTestLandingPageNoSelectError);
			}
			else
			{
				if (this.Items.Length > 100) errorMessages.Add(WebMessages.AbTestLandingPageTooManyError);

				var totalRate = 0;
				foreach (var item in this.Items)
				{
					int rate;
					if ((int.TryParse(item.DistributionRate, out rate)==false) || (rate < 0))
					{
						errorMessages.Add(WebMessages.AbTestDistributionFormatError);
						break;

					}
					totalRate += rate;
				}

				if (totalRate != 100) errorMessages.Add(WebMessages.AbTestDistributionRateError);
			}

			if (IsValidDate() == false)
			{
				errorMessages.Add(WebMessages.InputCheckDateRange.Replace("@@ 1 @@", "ABテスト実施期間"));
			}

			var errorMessage = string.Join("<br />", errorMessages);

			return errorMessage;
		}

		/// <summary>
		/// 日付入力が正しいか
		/// </summary>
		/// <returns>可否</returns>
		private bool IsValidDate()
		{
			DateTime startDate;
			DateTime endDate;
			var isValid = true;

			if ((string.IsNullOrEmpty(this.RangeStartDate))
				|| (DateTime.TryParse(this.RangeStartDate, out startDate) == false))
			{
				return false;
			}

			if (string.IsNullOrEmpty(this.RangeEndDate)) return true;

			if (DateTime.TryParse(this.RangeEndDate, out endDate) == false) return false;

			if ((endDate < DateTime.Today) || (endDate < startDate)) return false;

			return true;
		}
		#endregion

		#region プロパティ
		/// <summary>ABテストID</summary>
		public string AbTestId
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_AB_TEST_ID]; }
			set { this.DataSource[Constants.FIELD_ABTEST_AB_TEST_ID] = value; }
		}
		/// <summary>ABテストタイトル</summary>
		public string AbTestTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_AB_TEST_TITLE]; }
			set { this.DataSource[Constants.FIELD_ABTEST_AB_TEST_TITLE] = value; }
		}
		/// <summary>公開状態</summary>
		public string PublicStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_PUBLIC_STATUS]; }
			set { this.DataSource[Constants.FIELD_ABTEST_PUBLIC_STATUS] = value; }
		}
		/// <summary>公開開始日</summary>
		public string RangeStartDate
		{
			get { return StringUtility.ToEmpty(this.DataSource["start_date"]); }
			set { this.DataSource["start_date"] = value; }
		}
		/// <summary>公開開始時間</summary>
		public string RangeStartTime
		{
			get { return StringUtility.ToEmpty(this.DataSource["start_time"]); }
			set { this.DataSource["start_time"] = value; }
		}
		/// <summary>公開終了日</summary>
		public string RangeEndDate
		{
			get { return StringUtility.ToEmpty(this.DataSource["end_date"]); }
			set { this.DataSource["end_date"] = value; }
		}
		/// <summary>公開終了時間</summary>
		public string RangeEndTime
		{
			get { return StringUtility.ToEmpty(this.DataSource["end_time"]); }
			set { this.DataSource["end_time"] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ABTEST_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ABTEST_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ABTEST_LAST_CHANGED] = value; }
		}
		/// <summary>ABテストアイテム入力値</summary>
		public AbTestItemInput[] Items { get; set; }
		#endregion
	}
}
