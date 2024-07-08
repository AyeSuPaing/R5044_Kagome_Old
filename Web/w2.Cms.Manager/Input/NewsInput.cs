/*
=========================================================================================================
  Module      : お知らせInputクラス(NewsInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Text;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Domain.News;
using Validator = w2.App.Common.Util.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	///  お知らせInputクラス
	/// </summary>
	public class NewsInput : InputBase<NewsModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NewsInput()
		{

			this.DisplayDateFromYear = DateTime.Now.ToString("yyyy");
			this.DisplayDateFromMonth = DateTime.Now.ToString("MM");
			this.DisplayDateFromDay = DateTime.Now.ToString("dd");
			this.DisplayDateFromHour = DateTime.Now.ToString("HH");
			this.DisplayDateFromMinute = DateTime.Now.ToString("mm");
			this.DisplayDateFromSecond = DateTime.Now.ToString("ss");

			this.DisplayDateToYear = string.Empty;
			this.DisplayDateToMonth = string.Empty;
			this.DisplayDateToDay = string.Empty;
			this.DisplayDateToHour = string.Empty;
			this.DisplayDateToMinute = string.Empty;
			this.DisplayDateToSecond = string.Empty;

			this.NewsTextKbn = Constants.FLG_NEWS_NEWS_TEXT_KBN_HTML;
			this.NewsText = string.Empty;
			this.DispFlg = true;
			this.MobileDispFlg = Constants.FLG_NEWS_MOBILE_DISP_FLG_ALL;
			this.DisplayOrder = string.Empty;
			this.ValidFlg = true;
			this.BrandId = string.Empty;
			this.DateCreated = string.Empty;
			this.DateChanged = string.Empty;
			this.LastChanged = string.Empty;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override NewsModel CreateModel()
		{
			var model = new NewsModel
			{
				ShopId = string.Empty,
				NewsId = this.NewsId,
				DisplayDateFrom = DateTime.Parse(this.DisplayDateFrom),
				DisplayDateTo = this.DisplayDateTo != null ? DateTime.Parse(this.DisplayDateTo) : (DateTime?)null,
				NewsTextKbn = this.NewsTextKbn,
				NewsText = this.NewsText,
				DispFlg = this.DispFlg ? Constants.FLG_NEWS_DISP_FLG_ALL : Constants.FLG_NEWS_DISP_FLG_LIST,
				MobileDispFlg = Constants.FLG_NEWS_MOBILE_DISP_FLG_ALL,
				DisplayOrder = string.IsNullOrEmpty(this.DisplayOrder) == false ? int.Parse(this.DisplayOrder) : 1,
				ValidFlg = this.ValidFlg ? Constants.FLG_NEWS_VALID_FLG_VALID : Constants.FLG_NEWS_VALID_FLG_INVALID,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = this.LastChanged,
				BrandId = StringUtility.ToEmpty(this.BrandId),
				NewsTextKbnMobile = string.Empty,
				NewsTextMobile = string.Empty,
				DelFlg = string.Empty,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(ActionStatus actionStatus)
		{
			var errorMessage = new StringBuilder();

			errorMessage.Append(
				Validator.Validate(
					actionStatus == ActionStatus.Insert ? "NewsRegister" :
					actionStatus == ActionStatus.Update ? "NewsModify" : "",
					this.DataSource));

			if (this.DisplayDateTo != null && Common.Util.Validator.IsDate(this.DisplayDateFrom)
				&& Common.Util.Validator.IsDate(this.DisplayDateTo)
				&& Common.Util.Validator.CheckDateRange(this.DisplayDateFrom, this.DisplayDateTo) == false)
			{
				errorMessage.Append(WebMessages.InputCheckDateRange.Replace("@@ 1 @@", "表示日付"));
			}
			else if (this.ValidDisplayDateTo == false)
			{
				errorMessage.Append(WebMessages.InputCheckDateRange.Replace("@@ 1 @@", "表示日付"));
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// 日付入力値の保持
		/// </summary>
		public void SetDatetime()
		{
			this.DisplayDateFrom = string.Format(
				"{0}-{1}-{2} {3}:{4}:{5}",
				this.DisplayDateFromYear,
				this.DisplayDateFromMonth,
				this.DisplayDateFromDay,
				this.DisplayDateFromHour,
				this.DisplayDateFromMinute,
				this.DisplayDateFromSecond);

			var displayDateTo = string.Format(
				"{0}-{1}-{2} {3}:{4}:{5}",
				this.DisplayDateToYear,
				this.DisplayDateToMonth,
				this.DisplayDateToDay,
				this.DisplayDateToHour,
				this.DisplayDateToMinute,
				this.DisplayDateToSecond);

			DateTime datetime;
			if (DateTime.TryParse(displayDateTo, out datetime)) this.DisplayDateTo = displayDateTo;
		}
		#endregion

		#region プロパティ
		/// <summary>新着ID</summary>
		public string NewsId
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_ID]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_ID] = value; }
		}
		/// <summary>表示日付（From）</summary>
		public string DisplayDateFrom
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_FROM]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_FROM] = value; }
		}
		/// <summary>表示日付（To）</summary>
		public string DisplayDateTo
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO] = value; }
		}
		/// <summary>本文区分</summary>
		public string NewsTextKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN] = value; }
		}
		/// <summary>本文</summary>
		public string NewsText
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT] = value; }
		}
		/// <summary>表示フラグ</summary>
		public bool DispFlg
		{
			get { return (bool)(this.DataSource[Constants.FIELD_NEWS_DISP_FLG] ?? false); }
			set { this.DataSource[Constants.FIELD_NEWS_DISP_FLG] = value; }
		}
		/// <summary>モバイル表示フラグ</summary>
		public string MobileDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_MOBILE_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_NEWS_MOBILE_DISP_FLG] = value; }
		}
		/// <summary>表示順</summary>
		public string DisplayOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public bool ValidFlg
		{
			get { return (bool)(this.DataSource[Constants.FIELD_NEWS_VALID_FLG] ?? false); }
			set { this.DataSource[Constants.FIELD_NEWS_VALID_FLG] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_NEWS_BRAND_ID] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NEWS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NEWS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NEWS_LAST_CHANGED] = value; }
		}
		/// <summary>表示日付 年 From</summary>
		public string DisplayDateFromYear { get; set; }
		/// <summary>表示日付 月 From</summary>
		public string DisplayDateFromMonth { get; set; }
		/// <summary>表示日付 日 From</summary>
		public string DisplayDateFromDay { get; set; }
		/// <summary>表示日付 時 From</summary>
		public string DisplayDateFromHour { get; set; }
		/// <summary>表示日付 分 From</summary>
		public string DisplayDateFromMinute { get; set; }
		/// <summary>表示日付 秒 From</summary>
		public string DisplayDateFromSecond { get; set; }
		/// <summary>表示日付 年 To</summary>
		public string DisplayDateToYear { get; set; }
		/// <summary>表示日付 月 To</summary>
		public string DisplayDateToMonth { get; set; }
		/// <summary>表示日付 日 To</summary>
		public string DisplayDateToDay { get; set; }
		/// <summary>表示日付 時 To</summary>
		public string DisplayDateToHour { get; set; }
		/// <summary>表示日付 分 To</summary>
		public string DisplayDateToMinute { get; set; }
		/// <summary>表示日付 秒 To</summary>
		public string DisplayDateToSecond { get; set; }
		/// <summary>登録か</summary>
		public bool IsInsert { get; set; }
		/// <summary>表示日付 Toに空の要素があるか</summary>
		private bool ValidDisplayDateTo
		{
			get
			{
				var dateValues = new[]
				{
					this.DisplayDateToYear,
					this.DisplayDateToMonth,
					this.DisplayDateToDay,
					this.DisplayDateToHour,
					this.DisplayDateToMinute,
					this.DisplayDateToSecond,
				};
				var isMismatch = (dateValues.All(string.IsNullOrEmpty)
					|| dateValues.All(v => (string.IsNullOrEmpty(v) == false)));

				return isMismatch;
			}
		}
		#endregion
	}
}