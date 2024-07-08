/*
=========================================================================================================
  Module      : Scoring Sale Question Input (ScoringSaleQuestionInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.ScoringSale;
using WebMessages = w2.Cms.Manager.Codes.Common.WebMessages;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// Scoring sale question input
	/// </summary>
	[Serializable]
	public class ScoringSaleQuestionInput : InputBase<ScoringSaleQuestionModel>
	{
		#region +Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		public ScoringSaleQuestionInput()
		{
			this.ScoringSaleQuestionChoiceList = new List<ScoringSaleQuestionChoiceInput>();
		}
		#endregion

		#region +Method
		/// <summary>
		/// Create model
		/// </summary>
		/// <returns>Model</returns>
		public override ScoringSaleQuestionModel CreateModel()
		{
			var model = new ScoringSaleQuestionModel
			{
				QuestionId = this.QuestionId,
				QuestionTitle = this.QuestionTitle,
				ScoreAxisId = this.ScoreAxisId,
				AnswerType = this.AnswerType,
				QuestionStatement = this.QuestionStatement,
				ScoringSaleQuestionChoiceList = this.ScoringSaleQuestionChoiceList
					.Select(choice => choice.CreateModel())
					.ToList(),
			};

			return model;
		}

		/// <summary>
		/// Validate
		/// </summary>
		/// <param name="isRegister">Is register</param>
		/// <returns>Error message</returns>
		public string Validate(bool isRegister)
		{
			var checkKbn = isRegister ? "ScoringSaleQuestionRegister" : "ScoringSaleQuestionModify";
			var errorMessageList = Validator.Validate(checkKbn, this.DataSource)
				.Select(item => item.Value)
				.ToList();

			if (this.ScoringSaleQuestionChoiceList.Count == 0)
			{
				errorMessageList.Add(WebMessages.ScoringSaleQuestionChoiceAmountMinError);
			}

			foreach (var scoringSaleQuestionChoice in this.ScoringSaleQuestionChoiceList)
			{
				var indexQuestionChoice = this.ScoringSaleQuestionChoiceList.IndexOf(scoringSaleQuestionChoice) + 1;
				var errorMessage = scoringSaleQuestionChoice.Validate(isRegister, indexQuestionChoice);
				errorMessageList.Add(errorMessage);
			}

			var imageFileNameList = this.ScoringSaleQuestionChoiceList
				.Where(scoringSaleQuestionChoice => ((scoringSaleQuestionChoice.IsCopyImage == false)
					&& (scoringSaleQuestionChoice.UploadFile != null)))
				.Select(scoringSaleQuestionChoice => scoringSaleQuestionChoice.UploadFile.FileName)
				.ToArray();

			var distinctImageFileNameList = imageFileNameList.Distinct().ToList();
			var scoringSaleImageRoot = Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				Constants.PATH_SCORING_QUESTION_IMAGE);

			foreach (var distinctImageFileName in distinctImageFileNameList)
			{
				var isExists = File.Exists(string.Format(
					"{0}{1}",
					scoringSaleImageRoot,
					distinctImageFileName));

				if (isExists)
				{
					errorMessageList.Add(
						WebMessages.ScoringSaleQuestionImageDulplicateNameError.Replace("@@ 1 @@", distinctImageFileName));
				}
			}

			return CreateErrorJoinMessage(errorMessageList);
		}

		/// <summary>
		/// Create error join message
		/// </summary>
		/// <param name="errorMessageList">Error message list</param>
		/// <returns><Error message</returns>
		private string CreateErrorJoinMessage(IEnumerable<string> errorMessageList)
		{
			var result = string.Join(
				"<br />",
				errorMessageList.Where(errorMessage => (string.IsNullOrEmpty(errorMessage) == false)));
			return result;
		}
		#endregion

		#region +Properties
		/// <summary>質問ID</summary>
		public string QuestionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID] = value; }
		}
		/// <summary>質問タイトル</summary>
		public string QuestionTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_TITLE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_TITLE] = value; }
		}
		/// <summary>スコア軸ID</summary>
		public string ScoreAxisId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID] = value; }
		}
		/// <summary>回答タイプ</summary>
		public string AnswerType
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE] = value; }
		}
		/// <summary>質問文</summary>
		public string QuestionStatement
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT] = value; }
		}
		/// <summary>Scoring sale question choice list</summary>
		public List<ScoringSaleQuestionChoiceInput> ScoringSaleQuestionChoiceList { get; set; }
		#endregion
	}
}
