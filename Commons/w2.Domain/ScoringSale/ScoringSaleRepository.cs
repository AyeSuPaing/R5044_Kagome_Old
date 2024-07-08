/*
=========================================================================================================
  Module      : Scoring Sale Repository (ScoringSaleRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ScoringSale.Helper;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// Scoring sale repository
	/// </summary>
	internal class ScoringSaleRepository : RepositoryBase
	{
		/// <summary>XML page name</summary>
		private const string XML_KEY_NAME = "ScoringSale";

		#region ~Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		internal ScoringSaleRepository()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		internal ScoringSaleRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~ScoringSale
		#region +SearchScoringSale
		/// <summary>
		/// Search scoring sale
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <param name="publicDateKbn">Public date kbn</param>
		/// <param name="publicStatus">Public status</param>
		/// <returns>Array of scoring sale model</returns>
		internal ScoringSaleModel[] SearchScoringSale(string searchWord, string publicDateKbn, string publicStatus)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALE_SCORING_SALE_TITLE, searchWord },
				{ Constants.VALUETEXT_PARAM_PUBLIC_DATE_KBN, publicDateKbn },
				{ Constants.FIELD_SCORINGSALE_PUBLISH_STATUS, publicStatus },
			};
			var data = Get(XML_KEY_NAME, "SearchScoringSale", input);

			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +GetScoringSale
		/// <summary>
		/// Get scoring sale
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale model</returns>
		internal ScoringSaleModel GetScoringSale(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALE_SCORING_SALE_ID, scoringSaleId },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSale", input);
			if (result.Count == 0) return null;

			return new ScoringSaleModel(result[0]);
		}
		#endregion

		#region +InsertScoringSale
		/// <summary>
		/// Insert scoring sale
		/// </summary>
		/// <param name="model">Scoring sale model</param>
		internal void InsertScoringSale(ScoringSaleModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSale", model.DataSource);
		}
		#endregion

		#region +UpdateScoringSale
		/// <summary>
		/// Update scoring sale
		/// </summary>
		/// <param name="model">Scoring sale model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpdateScoringSale(ScoringSaleModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateScoringSale", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteScoringSale
		/// <summary>
		/// Delete scoring sale
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSale(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALE_SCORING_SALE_ID, scoringSaleId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSale", input);
			return result;
		}
		#endregion
		#endregion

		#region ~ScoringSaleQuestion
		#region +GetSearchHitCountScoringSaleQuestion
		/// <summary>
		/// Get search hit count scoring sale question
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Number</returns>
		internal int GetSearchHitCountScoringSaleQuestion(string scoreAxisId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID, scoreAxisId },
			};
			var result = Get(XML_KEY_NAME, "GetSearchHitCountScoringSaleQuestion", input);
			return (int)result[0][0];
		}
		#endregion

		#region +SearchScoringSaleQuestion
		/// <summary>
		/// Search scoring sale question
		/// </summary>
		/// <param name="searchParamModel">Scoring sale question search param model</param>
		/// <returns>Array of scoring sale question model</returns>
		internal ScoringSaleQuestionModel[] SearchScoringSaleQuestion(ScoringSaleQuestionSearchParamModel searchParamModel)
		{
			var hasNotSearchWord = (searchParamModel.BeginRowNumber.HasValue && searchParamModel.EndRowNumber.HasValue);
			var input = searchParamModel.CreateHashtableParams();
			var data = Get(
				XML_KEY_NAME,
				hasNotSearchWord
					? "SearchScoringSaleQuestion"
					: "SearchScoringSaleQuestionBySearchWord",
				input);

			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleQuestionModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +GetScoringSaleQuestion
		/// <summary>
		/// Get scoring sale question
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Scoring sale question model</returns>
		internal ScoringSaleQuestionModel GetScoringSaleQuestion(string questionId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID, questionId },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleQuestion", input);
			if (result.Count == 0) return null;

			return new ScoringSaleQuestionModel(result[0]);
		}
		#endregion

		#region ~GetScoringSaleQuestions
		/// <summary>
		/// Get scoring sale questions
		/// </summary>
		/// <param name="questionIds">Array of question id</param>
		/// <returns>Array of scoring sale question mode</returns>
		internal ScoringSaleQuestionModel[] GetScoringSaleQuestions(string[] questionIds)
		{
			var param = string.Join(",", questionIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ question_ids @@", param);
			var data = Get(XML_KEY_NAME, "GetScoringSaleQuestions", replaces: replace);

			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleQuestionModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +InsertScoringSaleQuestion
		/// <summary>
		/// Insert scoring sale question
		/// </summary>
		/// <param name="model">Scoring sale question model</param>
		internal void InsertScoringSaleQuestion(ScoringSaleQuestionModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSaleQuestion", model.DataSource);
		}
		#endregion

		#region +UpdateScoringSaleQuestion
		/// <summary>
		/// Update scoring sale question
		/// </summary>
		/// <param name="model">Scoring sale question model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpdateScoringSaleQuestion(ScoringSaleQuestionModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateScoringSaleQuestion", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteScoringSaleQuestion
		/// <summary>
		/// Delete scoring sale question
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSaleQuestion(string questionId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID, questionId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleQuestion", input);
			return result;
		}
		#endregion
		#endregion

		#region ~ScoringSaleQuestionChoice
		#region +GetScoringSaleQuestionChoice
		/// <summary>
		/// Get scoring sale question choice
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale choice model</returns>
		internal ScoringSaleQuestionChoiceModel GetScoringSaleQuestionChoice(string questionId, int branchNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID, questionId },
				{ Constants.FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO, branchNo },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleQuestionChoice", input);
			if (result.Count == 0) return null;

			return new ScoringSaleQuestionChoiceModel(result[0]);
		}
		#endregion

		#region +GetScoringSaleQuestionChoices
		/// <summary>
		/// Get scoring sale question choices
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Array of scoring sale question choice model</returns>
		public ScoringSaleQuestionChoiceModel[] GetScoringSaleQuestionChoices(string questionId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID, questionId},
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleQuestionChoices", input);
			return result.Cast<DataRowView>().Select(item => new ScoringSaleQuestionChoiceModel(item)).ToArray();
		}
		#endregion

		#region +InsertScoringSaleQuestionChoice
		/// <summary>
		/// Insert scoring sale question choice
		/// </summary>
		/// <param name="model">Scoring sale question choice model</param>
		internal void InsertScoringSaleQuestionChoice(ScoringSaleQuestionChoiceModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSaleQuestionChoice", model.DataSource);
		}
		#endregion

		#region +UpdateScoringSaleQuestionChoice
		/// <summary>
		/// Update scoring sale question choice
		/// </summary>
		/// <param name="model">Scoring sale question choice model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpdateScoringSaleQuestionChoice(ScoringSaleQuestionChoiceModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateScoringSaleQuestionChoice", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteScoringSaleQuestionChoice
		/// <summary>
		/// Delete scoring sale question choice
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSaleQuestionChoice(string questionId, int branchNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID, questionId },
				{ Constants.FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO, branchNo },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleQuestionChoice", input);
			return result;
		}
		#endregion

		#region +DeleteScoringSaleQuestionChoices
		/// <summary>
		/// Delete scoring sale question choices
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSaleQuestionChoices(string questionId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID, questionId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleQuestionChoices", input);
			return result;
		}
		#endregion
		#endregion

		#region ~ScoringSaleQuestionPage
		#region +GetScoringSaleQuestionPage
		/// <summary>
		/// Get scoring sale question page
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="pageNo">Page no</param>
		/// <returns>Scoring sale question page model</returns>
		internal ScoringSaleQuestionPageModel GetScoringSaleQuestionPage(string scoringSaleId, string pageNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID, scoringSaleId },
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGE_PAGE_NO, pageNo },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleQuestionPage", input);
			if (result.Count == 0) return null;

			return new ScoringSaleQuestionPageModel(result[0]);
		}
		#endregion

		#region +GetScoringSaleQuestionPages
		/// <summary>
		/// Get scoring sale question pages
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale question page model</returns>
		internal ScoringSaleQuestionPageModel[] GetScoringSaleQuestionPages(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID, scoringSaleId },
			};
			var data = Get(XML_KEY_NAME, "GetScoringSaleQuestionPages", input);
			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleQuestionPageModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +GetScoringSaleQuestionPageName
		/// <summary>
		/// Get scoring sale question page
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale question page model</returns>
		internal ScoringSaleQuestionPageModel[] GetScoringSaleQuestionPageName(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALE_SCORING_SALE_ID, scoringSaleId },
			};
			var data = Get(XML_KEY_NAME, "GetScoringSaleQuestionPageName", input);
			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleQuestionPageModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +InsertScoringSaleQuestionPage
		/// <summary>
		/// Insert scoring sale question page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <returns>Number of affected cases</returns>
		internal void InsertScoringSaleQuestionPage(ScoringSaleQuestionPageModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSaleQuestionPage", model.DataSource);
		}
		#endregion

		#region +UpdateScoringSaleQuestionPage
		/// <summary>
		/// Update scoring sale question page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpdateScoringSaleQuestionPage(ScoringSaleQuestionPageModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateScoringSaleQuestionPage", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteScoringSaleQuestionPage
		/// <summary>
		/// Delete scoring sale question choice
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSaleQuestionPage(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID, scoringSaleId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleQuestionPage", input);
			return result;
		}
		#endregion
		#endregion

		#region ~ScoringSaleQuestionPageItem
		#region +GetScoringSaleQuestionPageItem
		/// <summary>
		/// Get scoring sale question page item
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="pageNo">Page no</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale question page item model</returns>
		internal ScoringSaleQuestionPageItemModel GetScoringSaleQuestionPageItem(
			string questionId,
			string scoringSaleId,
			string pageNo,
			int branchNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID, questionId },
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID, scoringSaleId },
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO, pageNo },
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO, branchNo },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleQuestionPageItem", input);
			if (result.Count == 0) return null;

			return new ScoringSaleQuestionPageItemModel(result[0]);
		}
		#endregion

		#region +GetScoringSaleQuestionPageItems
		/// <summary>
		/// Get scoring sale question page item
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Array of item scoring sale question page items</returns>
		internal ScoringSaleQuestionPageItemModel[] GetScoringSaleQuestionPageItems(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID, scoringSaleId },
			};
			var data = Get(XML_KEY_NAME, "GetScoringSaleQuestionPageItems", input);
			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleQuestionPageItemModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +InsertScoringSaleQuestionPageItem
		/// <summary>
		/// Insert scoring question page item
		/// </summary>
		/// <param name="model">Scoring sale question page item model</param>
		internal void InsertScoringSaleQuestionPageItem(ScoringSaleQuestionPageItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSaleQuestionPageItem", model.DataSource);
		}
		#endregion

		#region +DeleteScoringSaleQuestionPageItem
		/// <summary>
		/// Delete scoring sale question page item
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSaleQuestionPageItem(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID, scoringSaleId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleQuestionPageItem", input);
			return result;
		}
		#endregion
		#endregion

		#region ~ScoringSaleProduct
		#region +GetScoringSaleProduct
		/// <summary>
		/// Get scoring sale product
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale product model</returns>
		internal ScoringSaleProductModel GetScoringSaleProduct(string scoringSaleId, int branchNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID, scoringSaleId },
				{ Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO, branchNo },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleProduct", input);
			if (result.Count == 0) return null;

			return new ScoringSaleProductModel(result[0]);
		}
		#endregion

		#region +GetScoringSaleProducts
		/// <summary>
		/// Get scoring sale products
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="shopId">Shop id</param>
		/// <returns>Array scoring sale page product model</returns>
		public ScoringSaleProductModel[] GetScoringSaleProducts(string scoringSaleId, string shopId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID, scoringSaleId },
				{ Constants.FIELD_SCORINGSALEPRODUCT_SHOP_ID, shopId }
			};
			var data = Get(XML_KEY_NAME, "GetScoringSaleProducts", input);
			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleProductModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +InsertScoringSaleProduct
		/// <summary>
		/// Insert scoring sale product
		/// </summary>
		/// <param name="model">Scoring sale product model</param>
		/// <returns>Number of affected cases</returns>
		internal void InsertScoringSaleProduct(ScoringSaleProductModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSaleProduct", model.DataSource);
		}
		#endregion

		#region +UpdateScoringSaleProduct
		/// <summary>
		/// Update scoring sale product
		/// </summary>
		/// <param name="model">Scoring sale product model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpdateScoringSaleProduct(ScoringSaleProductModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateScoringSaleProduct", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteScoringSaleProduct
		/// <summary>
		/// Delete scoring sale product
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSaleProduct(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID, scoringSaleId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleProduct", input);
			return result;
		}
		#endregion
		#endregion

		#region ~ScoringSaleResultCondition
		#region +GetScoringSaleResultCondition
		/// <summary>
		/// Get scoring sale result condition
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="branchNo">Branch no</param>
		/// <param name="conditionBranchNo">Condition branch no</param>
		/// <returns>Scoring sale result condition model</returns>
		internal ScoringSaleResultConditionModel GetScoringSaleResultCondition(string scoringSaleId, int branchNo, int conditionBranchNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID, scoringSaleId },
				{ Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO, branchNo },
				{ Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO, conditionBranchNo },
			};
			var result = Get(XML_KEY_NAME, "GetScoringSaleResultCondition", input);
			if (result.Count == 0) return null;

			return new ScoringSaleResultConditionModel(result[0]);
		}
		#endregion

		#region +GetScoringSaleResultCondition
		/// <summary>
		/// Get scoring sale result conditions
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Array item of scoring sale result condition model</returns>
		internal ScoringSaleResultConditionModel[] GetScoringSaleResultConditions(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID, scoringSaleId },
			};
			var data = Get(XML_KEY_NAME, "GetScoringSaleResultConditions", input);
			var result = data.Cast<DataRowView>().Select(item => new ScoringSaleResultConditionModel(item)).ToArray();
			return result;
		}
		#endregion

		#region +InsertScoringSaleResultCondition
		/// <summary>
		/// Insert scoring sale result condition
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		/// <returns>Number of affected cases</returns>
		internal void InsertScoringSaleResultCondition(ScoringSaleResultConditionModel model)
		{
			Exec(XML_KEY_NAME, "InsertScoringSaleResultCondition", model.DataSource);
		}
		#endregion

		#region +UpdateScoringSaleResultCondition
		/// <summary>
		/// Update scoring sale result condition
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpdateScoringSaleResultCondition(ScoringSaleProductModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateScoringSaleResultCondition", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteScoringSaleResultCondition
		/// <summary>
		/// Delete scoring sale result condition
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Number of affected cases</returns>
		internal int DeleteScoringSaleResultCondition(string scoringSaleId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID, scoringSaleId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteScoringSaleResultCondition", input);
			return result;
		}
		#endregion
		#endregion
	}
}
