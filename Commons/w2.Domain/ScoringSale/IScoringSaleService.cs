/*
=========================================================================================================
  Module      : Scoring Sale Service Interface (IScoringSaleService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.ScoringSale.Helper;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// Scoring sale service interface
	/// </summary>
	public interface IScoringSaleService : IService
	{
		#region ~ScoringSale
		/// <summary>
		/// Search scoring sale
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <param name="publicDateKbn">Public date kbn</param>
		/// <param name="publicStatus">Public status</param>
		/// <returns>Array of Scoring sale model</returns>
		ScoringSaleModel[] SearchScoringSale(string searchWord, string publicDateKbn, string publicStatus);

		/// <summary>
		/// Get scoring sale
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale model</returns>
		ScoringSaleModel GetScoringSale(string scoringSaleId);

		/// <summary>
		/// Get page data
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale model</returns>
		ScoringSaleModel GetPageData(string scoringSaleId);

		/// <summary>
		/// Insert scoring sale
		/// </summary>
		/// <param name="model">Scoring sale model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		void InsertScoringSale(ScoringSaleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update scoring sale
		/// </summary>
		/// <param name="model">Scoring sale model</param>
		/// <returns>Number of affected cases</returns>
		int UpdateScoringSale(ScoringSaleModel model);

		/// <summary>
		/// Delete scoring sale
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int DeleteScoringSale(string scoringSaleId, SqlAccessor accessor = null);

		/// <summary>
		/// Delete page data
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		void DeletePageData(string scoringSaleId);
		#endregion

		#region ~ScoringSaleQuestion
		/// <summary>
		/// Get search hit count scoring sale question
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Number</returns>
		int GetSearchHitCountScoringSaleQuestion(string scoreAxisId);

		/// <summary>
		/// Search scoring sale question
		/// </summary>
		/// <param name="searchParamModel">Scoring sale question search param model</param>
		/// <returns>Array of scoring sale question model</returns>
		ScoringSaleQuestionModel[] SearchScoringSaleQuestion(ScoringSaleQuestionSearchParamModel searchParamModel);

		/// <summary>
		/// Get scoring sale question
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Scoring sale question model</returns>
		ScoringSaleQuestionModel GetScoringSaleQuestion(string questionId);

		/// <summary>
		/// Get scoring sale questions
		/// </summary>
		/// <param name="questionIds">Array of question id</param>
		/// <returns>Array of scoring sale question model</returns>
		ScoringSaleQuestionModel[] GetScoringSaleQuestions(string[] questionIds);

		/// <summary>
		/// Insert scoring sale question
		/// </summary>
		/// <param name="model">Scoring sale question model</param>
		/// <param name="accessor">Sql accessor</param>
		void InsertScoringSaleQuestion(ScoringSaleQuestionModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update scoring sale question
		/// </summary>
		/// <param name="model">Scoring sale question model</param>
		/// <param name="accessor">Sql accessor</param>
		void UpdateScoringSaleQuestion(ScoringSaleQuestionModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete scoring sale question
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="accessor">Sql accessor</param>
		void DeleteScoringSaleQuestion(string questionId, SqlAccessor accessor = null);
		#endregion

		#region ~ScoringSaleQuestionChoice
		/// <summary>
		/// Get scoring sale question choice
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale question choice model</returns>
		ScoringSaleQuestionChoiceModel GetScoringSaleQuestionChoice(string questionId, int branchNo);

		/// <summary>
		/// Get scoring sale question choices
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Array of scoring sale question choice model</returns>
		ScoringSaleQuestionChoiceModel[] GetScoringSaleQuestionChoices(string questionId);

		/// <summary>
		/// Insert scoring sale question choice
		/// </summary>
		/// <param name="model">Scoring sale question choice model</param>
		/// <param name="accessor">Sql accessor</param>
		void InsertScoringSaleQuestionChoice(ScoringSaleQuestionChoiceModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update scoring sale question choice
		/// </summary>
		/// <param name="model">Scoring sale question choice model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpdateScoringSaleQuestionChoice(ScoringSaleQuestionChoiceModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete scoring sale question choice
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="branchNo">Branch no</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int DeleteScoringSaleQuestionChoice(string questionId, int branchNo, SqlAccessor accessor = null);
		#endregion

		#region ~ScoringSaleQuestionPage
		/// <summary>
		/// Get scoring sale question page
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="pageNo">Page no</param>
		/// <returns>scoring sale question page model</returns>
		ScoringSaleQuestionPageModel GetScoringSaleQuestionPage(string scoringSaleId, string pageNo);

		/// <summary>
		/// Get scoring sale question pages
		/// </summary>
		/// <param name="scoringSaleId">Scoring Sale id</param>
		/// <returns>Scoring sale question page model</returns>
		ScoringSaleQuestionPageModel[] GetScoringSaleQuestionPages(string scoringSaleId);

		/// <summary>
		/// Get scoring sale question page name
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale question page model</returns>
		ScoringSaleQuestionPageModel[] GetScoringSaleQuestionPageName(string scoringSaleId);

		/// <summary>
		/// Insert scoring sale question page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <param name="accessor">Sql accessor</param>
		void InsertScoringSaleQuestionPage(ScoringSaleQuestionPageModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update scoring sale question page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpdateScoringSaleQuestionPage(ScoringSaleQuestionPageModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete scoring sale question choice
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int DeleteScoringSaleQuestionPage(string scoringSaleId, SqlAccessor accessor = null);
		#endregion

		#region ~ScoringSaleQuestionPageItem
		/// <summary>
		/// Get scoring sale question page item
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="pageNo">Page no</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale question page item model</returns>
		ScoringSaleQuestionPageItemModel GetScoringSaleQuestionPageItem(
			string questionId,
			string scoringSaleId,
			string pageNo,
			int branchNo);

		/// <summary>
		/// Get scoring sale question page items 
		/// </summary>
		/// <param name="scoringsaleId">Scoring Sale id</param>
		/// <returns>Scoring sale question page item model</returns>
		ScoringSaleQuestionPageItemModel[] GetScoringSaleQuestionPageItems(string scoringSaleId);

		/// <summary>
		/// Get scoring sale question page items
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="pageNo">Page no</param>
		/// <returns>Scoring sale question page item models</returns>
		ScoringSaleQuestionPageItemModel[] GetScoringSaleQuestionPageItems(string scoringSaleId, int pageNo);

		/// <summary>
		/// Insert scoring sale question page item
		/// </summary>
		/// <param name="model">Scoring sale question page item model</param>
		/// <param name="accessor">Sql accessor</param>
		void InsertScoringSaleQuestionPageItem(ScoringSaleQuestionPageItemModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete scoring sale question page item
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int DeleteScoringSaleQuestionPageItem(
			string scoringSaleId,
			SqlAccessor accessor = null);
		#endregion

		#region ~ScoringSaleProduct
		/// <summary>
		/// Get scoring sale product
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale product model</returns>
		ScoringSaleProductModel GetScoringSaleProduct(string scoringSaleId, int branchNo);

		/// <summary>
		/// Get scoring sale products
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="shopId">Shop id</param>
		/// <returns>Array scoring sale page product model</returns>
		ScoringSaleProductModel[] GetScoringSaleProducts(string scoringSaleId, string shopId);

		/// <summary>
		/// Insert scoring sale page product
		/// </summary>
		/// <param name="model">Scoring sale product model</param>
		/// <param name="accessor">Sql accessor</param>
		void InsertScoringSaleProduct(ScoringSaleProductModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update scoring sale page product
		/// </summary>
		/// <param name="model">Scoring sale product model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpdateScoringSaleProduct(ScoringSaleProductModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete scoring sale page product
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int DeleteScoringSaleProduct(string scoringSaleId, SqlAccessor accessor = null);
		#endregion

		#region ~ScoringSaleResultCondition
		/// <summary>
		/// Get scoring sale result condition
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="branchNo">Branch no</param>
		/// <param name="conditionBranchNo">Condition branch no</param>
		/// <returns>Scoring sale result condition model</returns>
		ScoringSaleResultConditionModel GetScoringSaleResultCondition(string scoringSaleId, int branchNo, int conditionBranchNo);

		/// <summary>
		/// Get scoring sale result conditions
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale result condition model</returns>
		ScoringSaleResultConditionModel[] GetScoringSaleResultConditions(string scoringSaleId, int branchNo);

		/// <summary>
		/// Insert scoring sale result condition
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		/// <param name="accessor">Sql accessor</param>
		void InsertScoringSaleResultCondition(ScoringSaleResultConditionModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update scoring sale result condition
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpdateScoringSaleResultCondition(ScoringSaleProductModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete scoring sale result condition
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int DeleteScoringSaleResultCondition(
			string scoringSaleId,
			SqlAccessor accessor = null);
		#endregion
	}
}
