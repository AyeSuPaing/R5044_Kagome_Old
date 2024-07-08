/*
=========================================================================================================
  Module      : Scoring Sale Input (ScoringSaleInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Domain.ScoringSale;
using Constants = w2.Cms.Manager.Codes.Constants;
using Validator = w2.Common.Util.Validator;

/// <summary>
/// Scoring sale input
/// </summary>
public class ScoringSaleInput : InputBase<ScoringSaleModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleInput()
	{
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Model</param>
	public ScoringSaleInput(ScoringSaleModel model)
		: this()
	{
		this.ScoringSaleQuestionPages = new ScoringSaleQuestionPageInput[] { };
		this.ProductRecommendConditions = new ScoringSaleProductInput[] { };
		this.ScoringSaleId = model.ScoringSaleId;
		this.ScoringSaleTitle = model.ScoringSaleTitle;
		this.PublishStatus = model.PublishStatus;
		this.ScoreAxisId = model.ScoreAxisId;
		this.ThemeColor = model.ThemeColor;
		this.TopPageUseFlg = (model.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON);
		this.TopPageTitle = model.TopPageTitle;
		this.TopPageSubTitle = model.TopPageSubTitle;
		this.TopPageBody = model.TopPageBody;
		this.TopPageImgPath = model.TopPageImgPath;
		this.TopPageBtnCaption = model.TopPageBtnCaption;
		this.ResultPageTitle = model.ResultPageTitle;
		this.ResultPageBodyAbove = model.ResultPageBodyAbove;
		this.ResultPageBodyBelow = model.ResultPageBodyBelow;
		this.RadarChartUseFlg = (model.RadarChartUseFlg == Constants.FLG_SCORINGSALE_RADAR_CHART_USE_FLG_ON);
		this.RadarChartTitle = model.RadarChartTitle;
		this.ResultPageBtnCaption = model.ResultPageBtnCaption;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale model</returns>
	public override ScoringSaleModel CreateModel()
	{
		var model = new ScoringSaleModel
		{
			ScoringSaleId = this.ScoringSaleId,
			ScoringSaleTitle = this.ScoringSaleTitle,
			PublishStatus = this.PublishStatus,
			PublicStartDatetime = ParseDate(this.RangeStartDate, this.RangeStartTime),
			PublicEndDatetime = ParseDate(this.RangeEndDate, this.RangeEndTime),
			ScoreAxisId = this.ScoreAxisId,
			ThemeColor = this.ThemeColor,
			TopPageUseFlg = this.TopPageUseFlg
				? Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON
				: Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_OFF,
			TopPageTitle = this.TopPageTitle,
			TopPageSubTitle = this.TopPageSubTitle,
			TopPageBody = this.TopPageBody,
			TopPageImgPath = this.TopPageImgPath,
			TopPageBtnCaption = this.TopPageBtnCaption,
			ResultPageTitle = this.ResultPageTitle,
			ResultPageBodyAbove = this.ResultPageBodyAbove,
			ResultPageBodyBelow = this.ResultPageBodyBelow,
			RadarChartUseFlg = this.RadarChartUseFlg
				? Constants.FLG_SCORINGSALE_RADAR_CHART_USE_FLG_ON
				: Constants.FLG_SCORINGSALE_RADAR_CHART_USE_FLG_OFF,
			RadarChartTitle = this.RadarChartUseFlg
				? this.RadarChartTitle
				: string.Empty,
			ResultPageBtnCaption = this.ResultPageBtnCaption,
			LastChanged = this.LastChanged,
			ScoringSaleQuestionPages = this.ScoringSaleQuestionPages.Select(item => item.CreateModel()).ToArray(),
			ScoringSaleProducts = this.ProductRecommendConditions.Select(item => item.CreateModel()).ToArray(),
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <param name="isRegister">Is register</param>
	/// <returns>Eror messages</returns>
	public List<string> Validate(bool isRegister)
	{
		var checkScoringSaleKbn = isRegister ? "ScoringSaleRegister" : "ScoringSaleModify";
		var errorMessageList = Validator.Validate(checkScoringSaleKbn, this.DataSource)
			.Select(keyValue => keyValue.Value)
			.ToList();

		var position = 1;
		foreach (var scoringSaleQuestionPage in this.ScoringSaleQuestionPages)
		{
			if (scoringSaleQuestionPage.ScoringSaleQuestionItems == null)
			{
				errorMessageList.Add(
					WebMessages.ScoringsaleQuestionPageItemNoSelectError.Replace("@@ 1 @@", position.ToString()));
			}

			var questionPageErrorMessages = scoringSaleQuestionPage.Validate(isRegister);
			errorMessageList.AddRange(questionPageErrorMessages);

			position++;
		}

		if (this.ProductRecommendConditions.Length == 1)
		{
			errorMessageList.Add(WebMessages.ScoringSaleProductSetOneOrMoreItemError);
		}

		foreach (var productRecommendCondition in this.ProductRecommendConditions)
		{
			var scoringSaleProductErrorMessages = productRecommendCondition.Validate(isRegister);
			errorMessageList.AddRange(scoringSaleProductErrorMessages);

			if (productRecommendCondition.ScoringSaleResultCondition == null) continue;

			foreach (var scoringSaleResultCondition in productRecommendCondition.ScoringSaleResultCondition)
			{
				var scoringSaleResultConditionErrorMessages = scoringSaleResultCondition.Validate(isRegister);

				if ((scoringSaleResultConditionErrorMessages.Count == 0)
					&& (Validator.CheckNumberRange(
						scoringSaleResultCondition.ScoreAxisAxisValueFrom,
						scoringSaleResultCondition.ScoreAxisAxisValueTo) == false))
				{
					errorMessageList.Add(WebMessages.ScoreAxisAdditionValueError);
				}

				errorMessageList.AddRange(scoringSaleResultConditionErrorMessages);
			}
		}

		if (IsValidDatetimeTerm() == false)
		{
			errorMessageList.Add(WebMessages.ReleaseRangeSettingDateRangeError);
		}

		if ((this.IsCopyImage == false)
			&& (this.UploadFile != null))
		{
			var scoringSaleImageRoot = Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				Constants.PATH_SCORING_IMAGE);
			var filePath = string.Format(
				"{0}{1}",
				scoringSaleImageRoot,
				this.UploadFile.FileName);

			if (File.Exists(filePath))
			{
				var imageDulplicateNameErrorMessage = WebMessages.ScoringSaleQuestionImageDulplicateNameError.Replace(
					"@@ 1 @@",
					this.UploadFile.FileName);
				errorMessageList.Add(imageDulplicateNameErrorMessage);
			}
		}

		return errorMessageList;
	}

	/// <summary>
	/// Is valid datetime term
	/// </summary>
	/// <returns>Result</returns>
	private bool IsValidDatetimeTerm()
	{
		var rangeDateString = string.Format(
			"{0}{1}{2}{3}",
			this.RangeStartDate,
			this.RangeStartTime,
			this.RangeEndDate,
			this.RangeEndTime);
		if (string.IsNullOrEmpty(rangeDateString)) return true;

		var startDateTime = ParseDate(this.RangeStartDate, this.RangeStartTime);
		if (IsDatetimeWithinSqldatetimeRange(startDateTime) == false) return false;

		var endDateTime = ParseDate(this.RangeEndDate, this.RangeEndTime);
		if (IsDatetimeWithinSqldatetimeRange(endDateTime) == false) return false;

		if (startDateTime.HasValue
			&& endDateTime.HasValue
			&& (startDateTime > endDateTime))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Is datetime within sql datetime range
	/// </summary>
	/// <param name="target">Target</param>
	/// <returns>Result</returns>
	private bool IsDatetimeWithinSqldatetimeRange(DateTime? target)
	{
		if (target.HasValue == false) return true;

		var result = (((DateTime)SqlDateTime.MinValue <= target.Value)
			&& (target.Value <= (DateTime)SqlDateTime.MaxValue));
		return result;
	}

	/// <summary>
	/// Date and time conversion
	/// </summary>
	/// <param name="date">Date</param>
	/// <param name="time">Time</param>
	/// <returns>Date time after conversion (null on error)</returns>
	private DateTime? ParseDate(string date, string time)
	{
		DateTime temp;

		if (DateTime.TryParse(date, out temp) == false) return null;

		if (string.IsNullOrEmpty(time)) return temp.Date;
		if (DateTime.TryParse(date + " " + time, out temp) == false) return null;

		return temp;
	}

	/// <summary>
	/// Combine error message lists with <br />
	/// </summary>
	/// <param name="errorMessageList">Error message list</param>
	/// <returns>Combined error messages</returns>
	public string CreateErrorJoinMessage(IEnumerable<string> errorMessageList)
	{
		var result = string.Join("<br />", errorMessageList);
		return result;
	}
	#endregion

	#region +Properties
	/// <summary>スコアリング販売ID</summary>
	public string ScoringSaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_ID] = value; }
	}
	/// <summary>スコアリング販売タイトル</summary>
	public string ScoringSaleTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_TITLE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_TITLE] = value; }
	}
	/// <summary>公開状態</summary>
	public string PublishStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_PUBLISH_STATUS]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_PUBLISH_STATUS] = value; }
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
	/// <summary>スコア軸ID</summary>
	public string ScoreAxisId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_SCORE_AXIS_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_SCORE_AXIS_ID] = value; }
	}
	/// <summary>テーマカラー</summary>
	public string ThemeColor
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_THEME_COLOR]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_THEME_COLOR] = value; }
	}
	/// <summary>トップページ利用</summary>
	public bool TopPageUseFlg { get; set; }
	/// <summary>トップページタイトル</summary>
	public string TopPageTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE] = value; }
	}
	/// <summary>トップページサブタイトル</summary>
	public string TopPageSubTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE] = value; }
	}
	/// <summary>トップページ本文</summary>
	public string TopPageBody
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY] = value; }
	}
	/// <summary>トップページ画像</summary>
	public string TopPageImgPath
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH] = value; }
	}
	/// <summary>トップページボタン文言</summary>
	public string TopPageBtnCaption
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION] = value; }
	}
	/// <summary>結果ページタイトル</summary>
	public string ResultPageTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_TITLE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_TITLE] = value; }
	}
	/// <summary>結果ページ本文HTML(上)</summary>
	public string ResultPageBodyAbove
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_ABOVE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_ABOVE] = value; }
	}
	/// <summary>結果ページ本文HTML(下)</summary>
	public string ResultPageBodyBelow
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_BELOW]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_BELOW] = value; }
	}
	/// <summary>レーダーチャート利用</summary>
	public bool RadarChartUseFlg { get; set; }
	/// <summary>レーダーチャートタイトル</summary>
	public string RadarChartTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_TITLE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_TITLE] = value; }
	}
	/// <summary>結果ページボタン文言</summary>
	public string ResultPageBtnCaption
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BTN_CAPTION]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BTN_CAPTION] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_LAST_CHANGED] = value; }
	}
	/// <summary>Scoring sale question pages</summary>
	public ScoringSaleQuestionPageInput[] ScoringSaleQuestionPages { get; set; }
	/// <summary>Product recommend conditions</summary>
	public ScoringSaleProductInput[] ProductRecommendConditions { get; set; }
	/// <summary>Is copy image</summary>
	public bool IsCopyImage { get; set; }
	/// <summary>アップロードファイル</summary>
	public HttpPostedFileBase UploadFile { get; set; }
	#endregion
}
