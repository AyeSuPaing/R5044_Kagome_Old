/*
=========================================================================================================
  Module      : Scoring Sale Input (ScoringSaleInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale input
/// </summary>
[Serializable]
public class ScoringSaleInput : InputBase<ScoringSaleModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleInput()
		: base()
	{
		this.TotalScore = new ScoreAxisInput();
		this.QuestionPages = new ScoringSaleQuestionPageInput[0];
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale model</param>
	public ScoringSaleInput(ScoringSaleModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.ScoringSaleTitle = model.ScoringSaleTitle;
		this.PublishStatus = model.PublishStatus;
		this.PublicStartDatetime = model.PublicStartDatetime;
		this.PublicEndDatetime = model.PublicEndDatetime;
		this.ScoreAxisId = model.ScoreAxisId;
		this.ThemeColor = model.ThemeColor;
		this.TopPageUseFlg = model.TopPageUseFlg;
		this.TopPageTitle = model.TopPageTitle;
		this.TopPageSubTitle = model.TopPageSubTitle;
		this.TopPageBody = model.TopPageBody;
		this.TopPageImgPath = model.TopPageImgPath;
		this.TopPageBtnCaption = model.TopPageBtnCaption;
		this.ResultPageTitle = model.ResultPageTitle;
		this.ResultPageBodyAbove = model.ResultPageBodyAbove;
		this.ResultPageBodyBelow = model.ResultPageBodyBelow;
		this.RadarChartUseFlg = model.RadarChartUseFlg;
		this.RadarChartTitle = model.RadarChartTitle;
		this.ResultPageBtnCaption = model.ResultPageBtnCaption;
		this.DateCreated = model.DateCreated;
		this.DateChanged = model.DateChanged;
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
			PublicStartDatetime = this.PublicStartDatetime,
			PublicEndDatetime = this.PublicEndDatetime,
			ScoreAxisId = this.ScoreAxisId,
			ThemeColor = this.ThemeColor,
			TopPageUseFlg = this.TopPageUseFlg,
			TopPageTitle = this.TopPageTitle,
			TopPageSubTitle = this.TopPageSubTitle,
			TopPageBody = this.TopPageBody,
			TopPageImgPath = this.TopPageImgPath,
			TopPageBtnCaption = this.TopPageBtnCaption,
			ResultPageTitle = this.ResultPageTitle,
			ResultPageBodyAbove = this.ResultPageBodyAbove,
			ResultPageBodyBelow = this.ResultPageBodyBelow,
			RadarChartUseFlg = this.RadarChartUseFlg,
			RadarChartTitle = this.RadarChartTitle,
			ResultPageBtnCaption = this.ResultPageBtnCaption,
			DateCreated = this.DateCreated,
			DateChanged = this.DateChanged,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// Load current question page
	/// </summary>
	/// <param name="pageNo">Page no</param>
	public void LoadCurrentQuestionPage(string pageNo)
	{
		if (this.QuestionPages.Length == 0) return;

		this.CurrentQuestionPage = this.QuestionPages.FirstOrDefault(page => (page.PageNo == pageNo));
	}

	/// <summary>
	/// Get axis additional
	/// </summary>
	/// <param name="number">Number</param>
	/// <returns>Axis additional</returns>
	public int GetAxisAdditional(int number)
	{
		var result = (int)
			this.TotalScore.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO + number.ToString()];
		return result;
	}

	/// <summary>
	/// Get all axis additionals
	/// </summary>
	/// <returns>Axis additionals</returns>
	public Dictionary<int, int> GetAllAxisAdditionals()
	{
		var result = this.TotalScore.DataSource.Cast<DictionaryEntry>()
			.ToDictionary(
				item => (string)item.Key,
				item => (object)item.Value)
			.Where(item => item.Key.StartsWith(Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO))
			.ToDictionary(
				item => int.Parse(item.Key.Replace(Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO, string.Empty)),
				item => (int)item.Value);
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
	/// <summary>公開開始日時</summary>
	public DateTime? PublicStartDatetime
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_START_DATETIME] == DBNull.Value) return null;
			return (DateTime?)this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_START_DATETIME];
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_START_DATETIME] = value; }
	}
	/// <summary>公開終了日時</summary>
	public DateTime? PublicEndDatetime
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_END_DATETIME] == DBNull.Value) return null;
			return (DateTime?)this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_END_DATETIME];
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_END_DATETIME] = value; }
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
	public string TopPageUseFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_USE_FLG]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_USE_FLG] = value; }
	}
	/// <summary>トップページタイトル</summary>
	public string TopPageTitle
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE] == DBNull.Value) return null;
			return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE];
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE] = value; }
	}
	/// <summary>トップページサブタイトル</summary>
	public string TopPageSubTitle
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE] == DBNull.Value) return null;
			return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE];
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE] = value; }
	}
	/// <summary>トップページ本文</summary>
	public string TopPageBody
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY] == DBNull.Value) return null;
			return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY];
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY] = value; }
	}
	/// <summary>トップページ画像</summary>
	public string TopPageImgPath
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH] == DBNull.Value) return null;
			return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH];
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH] = value; }
	}
	/// <summary>トップページボタン文言</summary>
	public string TopPageBtnCaption
	{
		get
		{
			if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION] == DBNull.Value) return null;
			return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION];
		}
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
	public string RadarChartUseFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_USE_FLG]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_USE_FLG] = value; }
	}
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
	public DateTime DateCreated
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public DateTime DateChanged
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALE_LAST_CHANGED] = value; }
	}
	/// <summary>Is use top page</summary>
	public bool IsUseTopPage { get { return (this.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON); } }
	/// <summary>Total score</summary>
	public ScoreAxisInput TotalScore { get; set; }
	/// <summary>Question pages</summary>
	public ScoringSaleQuestionPageInput[] QuestionPages { get; set; }
	/// <summary>Current question pages</summary>
	public ScoringSaleQuestionPageInput CurrentQuestionPage { get; set; }
	#endregion
}
