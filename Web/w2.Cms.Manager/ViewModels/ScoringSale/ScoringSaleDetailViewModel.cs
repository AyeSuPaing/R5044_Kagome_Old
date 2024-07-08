/*
=========================================================================================================
  Module      : Scoring Sale Detail View Model (ScoringSaleDetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.WebPages.Html;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Domain.ScoringSale;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale detail view model
	/// </summary>
	[Serializable]
	public class ScoringSaleDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleDetailViewModel()
		{
			this.ScoringSaleId = string.Empty;
			this.ScoringSaleTitle = string.Empty;
			this.ProductRecommendConditions = new ScoringSaleProductViewModel[] { new ScoringSaleProductViewModel() };
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scoringSale">Scoring sale model</param>
		public ScoringSaleDetailViewModel(ScoringSaleModel scoringSale)
		{
			this.ScoringSaleId = scoringSale.ScoringSaleId;
			this.ScoringSaleTitle = scoringSale.ScoringSaleTitle;
			this.PublishStatus = scoringSale.PublishStatus;
			this.RangeStartDate = scoringSale.PublicStartDatetime.HasValue
				? DateTimeUtility.ToStringForManager(
					scoringSale.PublicStartDatetime.Value,
					DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime)
				: string.Empty;
			this.RangeStartTime = scoringSale.PublicStartDatetime.HasValue
				? scoringSale.PublicStartDatetime.Value.ToString("HH:mm")
				: string.Empty;
			this.RangeEndDate = scoringSale.PublicEndDatetime.HasValue
				? DateTimeUtility.ToStringForManager(
					scoringSale.PublicEndDatetime.Value,
					DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime)
				: string.Empty;
			this.RangeEndTime = scoringSale.PublicEndDatetime.HasValue
				? scoringSale.PublicEndDatetime.Value.ToString("HH:mm")
				: string.Empty;
			this.ThemeColor = scoringSale.ThemeColor;
			this.TopPageUseFlg = (scoringSale.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON);
			this.TopPageTitle = scoringSale.TopPageTitle;
			this.TopPageSubTitle = scoringSale.TopPageSubTitle;
			this.TopPageBody = scoringSale.TopPageBody;
			this.TopPageImgPath = scoringSale.TopPageImgPath;
			this.TopPageBtnCaption = scoringSale.TopPageBtnCaption;
			this.ResultPageTitle = scoringSale.ResultPageTitle;
			this.ResultPageBodyAbove = scoringSale.ResultPageBodyAbove;
			this.ResultPageBodyBelow = scoringSale.ResultPageBodyBelow;
			this.RadarChartUseFlg = (scoringSale.RadarChartUseFlg == Constants.FLG_SCORINGSALE_RADAR_CHART_USE_FLG_ON);
			this.RadarChartTitle = scoringSale.RadarChartTitle;
			this.ResultPageBtnCaption = scoringSale.ResultPageBtnCaption;
			this.ScoreAxisId = scoringSale.ScoreAxisId;
			this.ScoreAxisTitle = scoringSale.ScoreAxisTitle;
			this.ScoringSaleQuestionPages = scoringSale.ScoringSaleQuestionPages
				.Select(quesstion => new ScoringSaleQuestionPageViewModel(quesstion))
				.OrderBy(item => item.PageNo)
				.ToArray();
			this.ProductRecommend = scoringSale.ScoringSaleProducts
				.Where(item => (item.ScoringSaleResultConditions.Length == 0))
				.Select(model => new ScoringSaleProductViewModel(model))
				.FirstOrDefault();
			this.ProductRecommendConditions = scoringSale.ScoringSaleProducts
				.Where(item => (item.ScoringSaleResultConditions.Length != 0))
				.Select(model => new ScoringSaleProductViewModel(model))
				.ToArray();
		}

		#region +Properties
		/// <summary>Scoring sale Id</summary>
		public string ScoringSaleId { get; set; }
		/// <summary>Scoring sale title</summary>
		public string ScoringSaleTitle { get; set; }
		/// <summary>Publish status</summary>
		public string PublishStatus { get; set; }
		/// <summary>Range start date</summary>
		public string RangeStartDate { get; set; }
		/// <summary>Range start time</summary>
		public string RangeStartTime { get; set; }
		/// <summary>Range end date</summary>
		public string RangeEndDate { get; set; }
		/// <summary>Range end time</summary>
		public string RangeEndTime { get; set; }
		/// <summary>Theme color</summary>
		public string ThemeColor { get; set; }
		/// <summary>Is use top page</summary>
		public bool TopPageUseFlg { get; set; }
		/// <summary>Top page title</summary>
		public string TopPageTitle { get; set; }
		/// <summary>Top page sub title</summary>
		public string TopPageSubTitle { get; set; }
		/// <summary>Top page body</summary>
		public string TopPageBody { get; set; }
		/// <summary>Top page image path</summary>
		public string TopPageImgPath { get; set; }
		/// <summary>Top page button caption</summary>
		public string TopPageBtnCaption { get; set; }
		/// <summary>Result page title</summary>
		public string ResultPageTitle { get; set; }
		/// <summary>Result page body above</summary>
		public string ResultPageBodyAbove { get; set; }
		/// <summary>Result page body below</summary>
		public string ResultPageBodyBelow { get; set; }
		/// <summary>Is use radar chart</summary>
		public bool RadarChartUseFlg { get; set; }
		/// <summary>Radar chart title</summary>
		public string RadarChartTitle { get; set; }
		/// <summary>Result page button caption</summary>
		public string ResultPageBtnCaption { get; set; }
		/// <summary>Score table id</summary>
		public string ScoreAxisId { get; set; }
		/// <summary>Score table title</summary>
		public string ScoreAxisTitle { get; set; }
		/// <summary>Scoring sale question pages</summary>
		public ScoringSaleQuestionPageViewModel[] ScoringSaleQuestionPages { get; set; }
		/// <summary>Products recommends</summary>
		public ScoringSaleProductViewModel ProductRecommend { get; set; }
		/// <summary>Products recommend conditions</summary>
		public ScoringSaleProductViewModel[] ProductRecommendConditions { get; set; }
		/// <summary>Score axis axis no list</summary>
		public SelectListItem[] ScoreAxisAxisNoList { get; set; }
		/// <summary>Image source</summary>
		public string ImageSrc
		{
			get
			{
				if (string.IsNullOrEmpty(this.TopPageImgPath)) return string.Empty;

				var imageSrc = string.Format(
					"{0}{1}",
					Constants.PATH_ROOT_FRONT_PC,
					this.TopPageImgPath);
				return imageSrc;
			}
		}
		#endregion
	}
}
