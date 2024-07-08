/*
=========================================================================================================
  Module      : Scoring Sale Question Page Item Input (ScoringSaleQuestionPageItemInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale question page item input
/// </summary>
public class ScoringSaleQuestionPageItemInput : InputBase<ScoringSaleQuestionPageItemModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleQuestionPageItemInput()
	{
		this.BranchNo = Constants.CONST_SCORINGSALE_BRANCH_NO_DEFAULT;
		this.PageNo = Constants.CONST_SCORINGSALE_PAGE_NO_DEFAULT;
	}
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale question page item model</param>
	public ScoringSaleQuestionPageItemInput(ScoringSaleQuestionPageItemModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.PageNo = model.PageNo.ToString();
		this.BranchNo = model.BranchNo.ToString();
		this.QuestionId = model.QuestionId;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale question page item model</returns>
	public override ScoringSaleQuestionPageItemModel CreateModel()
	{
		var model = new ScoringSaleQuestionPageItemModel
		{
			ScoringSaleId = this.ScoringSaleId,
			PageNo = int.Parse(this.PageNo),
			BranchNo = int.Parse(this.BranchNo),
			QuestionId = this.QuestionId,
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <param name="isRegister">Is register</param>
	/// <returns>Error messages</returns>
	public List<string> Validate(bool isRegister)
	{
		var checkkbn = isRegister
			? "ScoringSaleQuestionPageItemRegister"
			: "ScoringSaleQuestionPageItemModify";
		var errorMessageList = Validator.Validate(checkkbn, this.DataSource)
			.Select(keyValue => keyValue.Value)
			.ToList();
		return errorMessageList;
	}
	#endregion

	#region +Properties
	/// <summary>質問ID</summary>
	public string QuestionId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID] = value; }
	}
	/// <summary>スコアリング販売ID</summary>
	public string ScoringSaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID] = value; }
	}
	/// <summary>ページNo</summary>
	public string PageNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO] = value; }
	}
	/// <summary>枝番</summary>
	public string BranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO] = value; }
	}
	#endregion
}
