/*
=========================================================================================================
  Module      : Scoring Sale Question Choice Input (ScoringSaleQuestionChoiceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.ScoringSale;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// Scoring sale question choice input
	/// </summary>
	public class ScoringSaleQuestionChoiceInput : InputBase<ScoringSaleQuestionChoiceModel>
	{
		#region +Method
		/// <summary>
		/// Create model
		/// </summary>
		/// <returns>Model</returns>
		public override ScoringSaleQuestionChoiceModel CreateModel()
		{
			var model = new ScoringSaleQuestionChoiceModel
			{
				QuestionId = this.QuestionId,
				BranchNo = int.Parse(this.BranchNo),
				QuestionChoiceStatement = this.QuestionChoiceStatement,
				QuestionChoiceStatementImgPath = this.QuestionChoiceStatementImgPath,
				AxisAdditional1 = int.Parse(this.AxisAdditional1),
				AxisAdditional2 = int.Parse(this.AxisAdditional2),
				AxisAdditional3 = int.Parse(this.AxisAdditional3),
				AxisAdditional4 = int.Parse(this.AxisAdditional4),
				AxisAdditional5 = int.Parse(this.AxisAdditional5),
				AxisAdditional6 = int.Parse(this.AxisAdditional6),
				AxisAdditional7 = int.Parse(this.AxisAdditional7),
				AxisAdditional8 = int.Parse(this.AxisAdditional8),
				AxisAdditional9 = int.Parse(this.AxisAdditional9),
				AxisAdditional10 = int.Parse(this.AxisAdditional10),
				AxisAdditional11 = int.Parse(this.AxisAdditional11),
				AxisAdditional12 = int.Parse(this.AxisAdditional12),
				AxisAdditional13 = int.Parse(this.AxisAdditional13),
				AxisAdditional14 = int.Parse(this.AxisAdditional14),
				AxisAdditional15 = int.Parse(this.AxisAdditional15),
			};

			return model;
		}

		/// <summary>
		/// Validate
		/// </summary>
		/// <param name="isRegister">Is register</param>
		/// <param name="indexQuestionChoice">Index question choice</param>
		/// <returns>Error message</returns>
		public string Validate(bool isRegister, int indexQuestionChoice)
		{
			var scoringSaleQuestionChoiceInput = this.DataSource;
			var checkKbn = isRegister
				? "ScoringSaleQuestionChoiceRegister"
				: "ScoringSaleQuestionChoiceModify";

			var errorMessageList = Validator.Validate(checkKbn, scoringSaleQuestionChoiceInput)
				.Select(item => {
					var errorMessage = (string.IsNullOrEmpty(item.Value) == false)
						? string.Format(
							"{0}つ目の選択肢の{1}",
							indexQuestionChoice,
							item.Value)
						: string.Empty;

					return errorMessage;
				})
				.ToList();

			return CreateErrorJoinMessage(errorMessageList);
		}

		/// <summary>
		/// Create error join message
		/// </summary>
		/// <param name="errorMessageList">Error message list</param>
		/// <returns>Error message</returns>
		private string CreateErrorJoinMessage(IEnumerable<string> errorMessageList)
		{
			var result = string.Join("<br />", errorMessageList);
			return result;
		}
		#endregion

		#region +Properties
		/// <summary>質問ID</summary>
		public string QuestionId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID]); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID] = value; }
		}
		/// <summary>枝番</summary>
		public string BranchNo { get; set; }
		/// <summary>選択肢文</summary>
		public string QuestionChoiceStatement
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT]); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT] = value; }
		}
		/// <summary>選択肢画像</summary>
		public string QuestionChoiceStatementImgPath
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH] = value; }
		}
		/// <summary>軸加算値１</summary>
		public string AxisAdditional1
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1] = value; }
		}
		/// <summary>軸加算値２</summary>
		public string AxisAdditional2
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2] = value; }
		}
		/// <summary>軸加算値３</summary>
		public string AxisAdditional3
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3] = value; }
		}
		/// <summary>軸加算値４</summary>
		public string AxisAdditional4
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4] = value; }
		}
		/// <summary>軸加算値５</summary>
		public string AxisAdditional5
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5] = value; }
		}
		/// <summary>軸加算値６</summary>
		public string AxisAdditional6
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6] = value; }
		}
		/// <summary>軸加算値７</summary>
		public string AxisAdditional7
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7] = value; }
		}
		/// <summary>軸加算値８</summary>
		public string AxisAdditional8
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8] = value; }
		}
		/// <summary>軸加算値９</summary>
		public string AxisAdditional9
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9] = value; }
		}
		/// <summary>軸加算値１０</summary>
		public string AxisAdditional10
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10] = value; }
		}
		/// <summary>軸加算値１１</summary>
		public string AxisAdditional11
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11] = value; }
		}
		/// <summary>軸加算値１２</summary>
		public string AxisAdditional12
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12] = value; }
		}
		/// <summary>軸加算値１３</summary>
		public string AxisAdditional13
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13] = value; }
		}
		/// <summary>軸加算値１４</summary>
		public string AxisAdditional14
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14] = value; }
		}
		/// <summary>軸加算値１５</summary>
		public string AxisAdditional15
		{
			get { return StringUtility.ToValue(this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15], "0").ToString(); }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15] = value; }
		}
		/// <summary>Is copy image</summary>
		public bool IsCopyImage { get; set; }
		/// <summary>アップロードファイル</summary>
		public HttpPostedFileBase UploadFile { get; set; }
		#endregion
	}
}