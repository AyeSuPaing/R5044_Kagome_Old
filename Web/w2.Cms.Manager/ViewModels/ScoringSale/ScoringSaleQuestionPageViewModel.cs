/*
=========================================================================================================
  Module      : Scoring Sale Question Page View Model (ScoringSaleQuestionPageViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.ScoringSale;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale detail view model
	/// </summary>
	[Serializable]
	public class ScoringSaleQuestionPageViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleQuestionPageViewModel()
		{
			this.IsSameAsFirstPage = false;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scoringSaleQuestionPage">Scoring sale question page model</param>
		public ScoringSaleQuestionPageViewModel(ScoringSaleQuestionPageModel scoringSaleQuestionPage)
		{
			this.ScoringSaleId = scoringSaleQuestionPage.ScoringSaleId;
			this.PageNo = scoringSaleQuestionPage.PageNo;
			this.PreviousPageBtnCaption = scoringSaleQuestionPage.PreviousPageBtnCaption;
			this.NextPageBtnCaption = scoringSaleQuestionPage.NextPageBtnCaption;
			this.ScoringSaleQuestionItems = scoringSaleQuestionPage.ScoringSaleQuestionPageItems
				.Select(item => new ScoringSaleQuestionPageItemViewModel(item))
				.ToArray();
			this.IsSameAsFirstPage = scoringSaleQuestionPage.IsSameAsFirstPage;
		}

		#region +Properties
		/// <summary>Scoring sale id</summary>
		public string ScoringSaleId { get; set; }
		/// <summary>Page no</summary>
		public int PageNo { get; set; }
		/// <summary>Previous page button caption</summary>
		public string PreviousPageBtnCaption { get; set; }
		/// <summary>Next page button caption</summary>
		public string NextPageBtnCaption { get; set; }
		/// <summary>Date created</summary>
		public string DateCreated { get; set; }
		/// <summary>Date changed</summary>
		public string DateChanged { get; set; }
		/// <summary>Last changed</summary>
		public string LastChanged { get; set; }
		/// <summary>Scoring sale question items</summary>
		public ScoringSaleQuestionPageItemViewModel[] ScoringSaleQuestionItems { get; set; }
		/// <summary>Is same as first page</summary>
		public bool IsSameAsFirstPage { get; set; }
		#endregion

		/// <summary>
		/// Scoring sale detail view model
		/// </summary>
		[Serializable]
		public class ScoringSaleQuestionPageItemViewModel : ViewModelBase
		{
			/// <summary>
			/// Constructor
			/// </summary>
			public ScoringSaleQuestionPageItemViewModel()
			{
			}

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="scoringSaleQuestionPage">Scoring sale question page</param>
			public ScoringSaleQuestionPageItemViewModel(ScoringSaleQuestionPageItemModel scoringSaleQuestionPage)
			{
				this.QuestionId = scoringSaleQuestionPage.QuestionId;
				this.ScoringSaleId = scoringSaleQuestionPage.ScoringSaleId;
				this.PageNo = scoringSaleQuestionPage.PageNo.ToString();
				this.BranchNo = scoringSaleQuestionPage.BranchNo.ToString();
				this.Name = scoringSaleQuestionPage.Name;
				this.Statement = scoringSaleQuestionPage.Statement;
				this.Type = scoringSaleQuestionPage.Type;
				this.UpdateDate = scoringSaleQuestionPage.UpdateDate;
			}

			#region +Properties
			/// <summary>Question id</summary>
			public string QuestionId { get; set; }
			/// <summary>Scoring sale id</summary>
			public string ScoringSaleId { get; set; }
			/// <summary>Page no</summary>
			public string PageNo { get; set; }
			/// <summary>Branch no</summary>
			public string BranchNo { get; set; }
			/// <summary>Name</summary>
			public string Name { get; set; }
			/// <summary>Statement</summary>
			public string Statement { get; set; }
			/// <summary>Type</summary>
			public string Type { get; set; }
			/// <summary>Type text</summary>
			public string TypeText
			{
				get
				{
					var result = ValueText.GetValueText(Constants.TABLE_SCORINGSALEQUESTION, Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE, this.Type);
					return result;
				}
			}
			/// <summary>Update date</summary>
			public string UpdateDate { get; set; }
			#endregion
		}
	}
}
