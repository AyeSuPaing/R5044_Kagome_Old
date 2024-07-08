/*
=========================================================================================================
  Module      : お知らせ 登録・編集ビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Cms.Manager.Input;
using w2.Common.Extensions;
using w2.Database.Common;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.News;

namespace w2.Cms.Manager.ViewModels.News
{
	/// <summary>
	/// お知らせ 登録・編集ビューモデル
	/// </summary>
	public class RegisterViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisterViewModel()
		{
			Init();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="m">お知らせモデル</param>
		public RegisterViewModel(NewsModel m)
			: this()
		{
			this.Input.NewsId = m.NewsId;

			this.Input.DisplayDateFromYear = m.DisplayDateFrom.ToString("yyyy");
			this.Input.DisplayDateFromMonth = m.DisplayDateFrom.ToString("MM");
			this.Input.DisplayDateFromDay = m.DisplayDateFrom.ToString("dd");
			this.Input.DisplayDateFromHour = m.DisplayDateFrom.ToString("HH");
			this.Input.DisplayDateFromMinute = m.DisplayDateFrom.ToString("mm");
			this.Input.DisplayDateFromSecond = m.DisplayDateFrom.ToString("ss");

			if (m.DisplayDateTo != null)
			{
				this.Input.DisplayDateToYear = m.DisplayDateTo.Value.ToString("yyyy");
				this.Input.DisplayDateToMonth = m.DisplayDateTo.Value.ToString("MM");
				this.Input.DisplayDateToDay = m.DisplayDateTo.Value.ToString("dd");
				this.Input.DisplayDateToHour = m.DisplayDateTo.Value.ToString("HH");
				this.Input.DisplayDateToMinute = m.DisplayDateTo.Value.ToString("mm");
				this.Input.DisplayDateToSecond = m.DisplayDateTo.Value.ToString("ss");
			}

			this.Input.NewsTextKbn = m.NewsTextKbn;
			this.Input.NewsText = m.NewsText;
			this.Input.DispFlg = m.DispFlg == Constants.FLG_NEWS_DISP_FLG_ALL;
			this.Input.MobileDispFlg = m.MobileDispFlg;
			this.Input.DisplayOrder = m.DisplayOrder.ToString();
			this.Input.ValidFlg = m.ValidFlg == Constants.FLG_NEWS_VALID_FLG_VALID;
			this.Input.BrandId = m.BrandId;
			this.Input.DateCreated = DateTimeUtility.ToStringForManager(
				m.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.Input.DateChanged = DateTimeUtility.ToStringForManager(
				m.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.Input.LastChanged = m.LastChanged;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.NewsTranslationData = GetNewsTranslationData(this.Input.NewsId);
			}
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		private void Init()
		{
			this.Input = new NewsInput();

			this.YearItems = DateTimeUtility.GetBackwardYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.MonthItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.DayItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.HourItems = DateTimeUtility.GetHourListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.MinuteItems = DateTimeUtility.GetMinuteListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.SecondItems = DateTimeUtility.GetSecondListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();

			if (w2.App.Common.Constants.PRODUCT_BRAND_ENABLED)
			{
				this.BrandItems = ProductBrandUtility.GetProductBrandList().ToHashtableList()
					.Select(ht => new SelectListItem
					{
						Value = (string)ht[Constants.FIELD_PRODUCTBRAND_BRAND_ID],
						Text = (string)ht[Constants.FIELD_PRODUCTBRAND_BRAND_ID]
					}).ToArray();
			}
		}

		/// <summary>
		/// 新着情報翻訳設定情報取得
		/// </summary>
		/// <param name="newsId">新着ID</param>
		/// <returns>新着情報翻訳設定情報</returns>
		private NameTranslationSettingModel[] GetNewsTranslationData(string newsId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS,
				MasterId1 = newsId,
				MasterId2 = string.Empty,
				MasterId3 = string.Empty,
			};
			var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
			return translationData;
		}

		/// <summary>お知らせ 入力内容</summary>
		public NewsInput Input { get; private set; }
		/// <summary>更新・登録 成功フラグ</summary>
		public bool UpdateInsertSuccessFlg { get; set; }
		/// <summary>選択肢群 年</summary>
		public SelectListItem[] YearItems { get; private set; }
		/// <summary>選択肢群 月</summary>
		public SelectListItem[] MonthItems { get; private set; }
		/// <summary>選択肢群 日</summary>
		public SelectListItem[] DayItems { get; private set; }
		/// <summary>選択肢群 時</summary>
		public SelectListItem[] HourItems { get; private set; }
		/// <summary>選択肢群 分</summary>
		public SelectListItem[] MinuteItems { get; private set; }
		/// <summary>選択肢群 秒</summary>
		public SelectListItem[] SecondItems { get; private set; }
		/// <summary>選択肢群 ブランドID</summary>
		public SelectListItem[] BrandItems { get; private set; }
		/// <summary>新着情報翻訳設定情報</summary>
		public NameTranslationSettingModel[] NewsTranslationData { get; private set; }
	}
}