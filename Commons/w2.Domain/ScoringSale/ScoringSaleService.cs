/*
=========================================================================================================
  Module      : Scoring Sale Service (ScoringSaleService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ScoringSale.Helper;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// Scoring sale service
	/// </summary>
	public class ScoringSaleService : ServiceBase, IScoringSaleService
	{
		#region ~ScoringSale
		#region +SearchScoringSale
		/// <summary>
		/// Search scoring sale
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <param name="publicDateKbn">Public date kbn</param>
		/// <param name="publicStatus">Public status</param>
		/// <returns>Array of scoring sale model</returns>
		public ScoringSaleModel[] SearchScoringSale(string searchWord, string publicDateKbn, string publicStatus)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var results = repository.SearchScoringSale(searchWord, publicDateKbn, publicStatus);
				return results;
			}
		}
		#endregion

		#region +GetScoringSale
		/// <summary>
		/// Get scoring sale
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale model</returns>
		public ScoringSaleModel GetScoringSale(string scoringSaleId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSale(scoringSaleId);
				return model;
			}
		}
		#endregion

		#region +GetPageData
		/// <summary>
		/// Get page data
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale model</returns>
		public ScoringSaleModel GetPageData(string scoringSaleId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSale(scoringSaleId);
				var questionPages = repository.GetScoringSaleQuestionPages(scoringSaleId);
				var productPages = repository.GetScoringSaleProducts(scoringSaleId, Constants.CONST_DEFAULT_SHOP_ID);
				var questionPageItem = repository.GetScoringSaleQuestionPageItems(scoringSaleId);
				var scoringSaleResultCondition = repository.GetScoringSaleResultConditions(scoringSaleId);

				model.ScoringSaleQuestionPages = questionPages;
				model.ScoringSaleProducts = productPages;

				foreach (var questionPage in model.ScoringSaleQuestionPages)
				{
					questionPage.ScoringSaleQuestionPageItems = questionPageItem.Where(item => item.ScoringSaleId == questionPage.ScoringSaleId
						&& item.PageNo == questionPage.PageNo).ToArray();
				}

				foreach (var product in model.ScoringSaleProducts)
				{
					product.ScoringSaleResultConditions = scoringSaleResultCondition.Where(item => item.BranchNo == product.BranchNo
						&& item.ScoringSaleId == product.ScoringSaleId).ToArray();
				}

				return model;
			}
		}
		#endregion

		#region +InsertScoringSale
		/// <summary>
		/// Insert scoring sale
		/// </summary>
		/// <param name="model">Scoring sale model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSale(ScoringSaleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				repository.InsertScoringSale(model);
			}
		}
		#endregion

		#region +UpdateScoringSale
		/// <summary>
		/// Update scoring sale
		/// </summary>
		/// <param name="model">Scoring sale model</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateScoringSale(ScoringSaleModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ScoringSaleRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var result = repository.UpdateScoringSale(model);

				DeleteScoringSaleQuestionPage(model.ScoringSaleId, accessor);
				DeleteScoringSaleQuestionPageItem(model.ScoringSaleId, accessor);
				DeleteScoringSaleProduct(model.ScoringSaleId, accessor);
				DeleteScoringSaleResultCondition(model.ScoringSaleId, accessor);

				foreach (var scoringSaleQuestionPage in model.ScoringSaleQuestionPages)
				{
					InsertScoringSaleQuestionPage(scoringSaleQuestionPage, accessor);
					foreach (var item in scoringSaleQuestionPage.ScoringSaleQuestionPageItems)
					{
						InsertScoringSaleQuestionPageItem(item, accessor);
					}
				}

				foreach (var scoringSalePageProduct in model.ScoringSaleProducts)
				{
					InsertScoringSaleProduct(scoringSalePageProduct, accessor);
					foreach (var condition in scoringSalePageProduct.ScoringSaleResultConditions)
					{
						InsertScoringSaleResultCondition(condition, accessor);
					}
				}
				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion

		#region +DeleteScoringSale
		/// <summary>
		/// Delete ScoringSale
		/// </summary>
		/// <param name="ScoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSale(string ScoringSaleId, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.DeleteScoringSale(ScoringSaleId);
				return result;
			}
		}
		#endregion

		#region +DeletePage
		/// <summary>
		/// Delete page data
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		public void DeletePageData(string scoringSaleId)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ScoringSaleRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				DeleteScoringSale(scoringSaleId, accessor);
				DeleteScoringSaleQuestionPage(scoringSaleId, accessor);
				DeleteScoringSaleQuestionPageItem(scoringSaleId, accessor);
				DeleteScoringSaleProduct(scoringSaleId, accessor);
				DeleteScoringSaleResultCondition(scoringSaleId, accessor);

				accessor.CommitTransaction();
			}
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
		public int GetSearchHitCountScoringSaleQuestion(string scoreAxisId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var result = repository.GetSearchHitCountScoringSaleQuestion(scoreAxisId);
				return result;
			}
		}
		#endregion

		#region +SearchScoringSaleQuestion
		/// <summary>
		/// Search scoring sale question
		/// </summary>
		/// <param name="searchParamModel">Scoring sale question search param model</param>
		/// <returns>Array of scoring sale question model</returns>
		public ScoringSaleQuestionModel[] SearchScoringSaleQuestion(ScoringSaleQuestionSearchParamModel searchParamModel)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var results = repository.SearchScoringSaleQuestion(searchParamModel);
				return results;
			}
		}
		#endregion

		#region +GetScoringSaleQuestion
		/// <summary>
		/// Get scoring sale question
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Scoring sale question model</returns>
		public ScoringSaleQuestionModel GetScoringSaleQuestion(string questionId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestion(questionId);
				return model;
			}
		}
		#endregion

		#region +GetScoringSaleQuestions
		/// <summary>
		/// Get scoring sale questions
		/// </summary>
		/// <param name="questionIds">Array of question id</param>
		/// <returns>Array of scoring sale question model</returns>
		public ScoringSaleQuestionModel[] GetScoringSaleQuestions(string[] questionIds)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var results = repository.GetScoringSaleQuestions(questionIds);
				return results;
			}
		}
		#endregion

		#region +InsertScoringSaleQuestion
		/// <summary>
		/// Insert scoring sale question
		/// </summary>
		/// <param name="model">Scoring sale question model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSaleQuestion(ScoringSaleQuestionModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				if (repository.Accessor.Transaction == null)
				{
					repository.Accessor.BeginTransaction();
				}

				// Insert scoring sale question
				repository.InsertScoringSaleQuestion(model);

				// Insert scoring sale question choices
				foreach (var scoringSaleQuestionChoice in model.ScoringSaleQuestionChoiceList)
				{
					scoringSaleQuestionChoice.QuestionId = model.QuestionId;
					repository.InsertScoringSaleQuestionChoice(scoringSaleQuestionChoice);
				}

				repository.Accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateScoringSaleQuestion
		/// <summary>
		/// Update scoring sale question
		/// </summary>
		/// <param name="model">Scoring sale question model</param>
		/// <param name="accessor">Sql accessor</param>
		public void UpdateScoringSaleQuestion(ScoringSaleQuestionModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				if (repository.Accessor.Transaction == null)
				{
					repository.Accessor.BeginTransaction();
				}

				// Update scoring sale question
				repository.UpdateScoringSaleQuestion(model);
				// Delete scoring sale question choices
				repository.DeleteScoringSaleQuestionChoices(model.QuestionId);

				// Insert scoring sale question choices
				foreach (var scoringSaleQuestionChoice in model.ScoringSaleQuestionChoiceList)
				{
					scoringSaleQuestionChoice.QuestionId = model.QuestionId;
					repository.InsertScoringSaleQuestionChoice(scoringSaleQuestionChoice);
				}

				repository.Accessor.CommitTransaction();
			}
		}
		#endregion

		#region +DeleteScoringSaleQuestion
		/// <summary>
		/// Delete scoring sale question
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="accessor">Sql accessor</param>
		public void DeleteScoringSaleQuestion(string questionId, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				if (repository.Accessor.Transaction == null)
				{
					repository.Accessor.BeginTransaction();
				}

				// Delete scoring sale question
				repository.DeleteScoringSaleQuestion(questionId);
				// Delete scoring sale question choices
				repository.DeleteScoringSaleQuestionChoices(questionId);

				repository.Accessor.CommitTransaction();
			}
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
		/// <returns>Scoring sale question choice model</returns>
		public ScoringSaleQuestionChoiceModel GetScoringSaleQuestionChoice(string questionId, int branchNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionChoice(questionId, branchNo);
				return model;
			}
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
			using (var repository = new ScoringSaleRepository())
			{
				var results = repository.GetScoringSaleQuestionChoices(questionId);
				return results;
			}
		}
		#endregion

		#region +InsertScoringSaleQuestionChoice
		/// <summary>
		/// Insert scoring sale question choice
		/// </summary>
		/// <param name="model">Scoring sale question choice model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSaleQuestionChoice(ScoringSaleQuestionChoiceModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				repository.InsertScoringSaleQuestionChoice(model);
			}
		}
		#endregion

		#region +UpdateScoringSaleQuestionChoice
		/// <summary>
		/// Update scoring sale question choice
		/// </summary>
		/// <param name="model">Scoring sale question choice model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateScoringSaleQuestionChoice(ScoringSaleQuestionChoiceModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.UpdateScoringSaleQuestionChoice(model);
				return result;
			}
		}
		#endregion

		#region +DeleteScoringSaleQuestionChoice
		/// <summary>
		/// Delete scoring sale question choice
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <param name="branchNo">Branch no</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSaleQuestionChoice(string questionId, int branchNo, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.DeleteScoringSaleQuestionChoice(questionId, branchNo);
				return result;
			}
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
		public ScoringSaleQuestionPageModel GetScoringSaleQuestionPage(string scoringSaleId, string pageNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionPage(scoringSaleId, pageNo);
				return model;
			}
		}
		#endregion

		#region +GetScoringSaleQuestionPages
		/// <summary>
		/// Get scoring sale question pages
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Array of scoring sale question page model</returns>
		public ScoringSaleQuestionPageModel[] GetScoringSaleQuestionPages(string scoringSaleId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionPages(scoringSaleId);
				return model;
			}
		}
		#endregion

		#region +GetScoringSaleQuestionPageName
		/// <summary>
		/// Get scoring sale question page name
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale question page model</returns>
		public ScoringSaleQuestionPageModel[] GetScoringSaleQuestionPageName(string scoringSaleId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionPageName(scoringSaleId);
				return model;
			}
		}
		#endregion

		#region +InsertScoringSaleQuestionPage
		/// <summary>
		/// Insert scoring sale question page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSaleQuestionPage(ScoringSaleQuestionPageModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				repository.InsertScoringSaleQuestionPage(model);
			}
		}
		#endregion

		#region +UpdateScoringSaleQuestionPage
		/// <summary>
		/// Update scoring sale question page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateScoringSaleQuestionPage(ScoringSaleQuestionPageModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.UpdateScoringSaleQuestionPage(model);
				return result;
			}
		}
		#endregion

		#region +DeleteScoringSaleQuestionPage
		/// <summary>
		/// Delete scoring sale question choice
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSaleQuestionPage(string scoringSaleId, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.DeleteScoringSaleQuestionPage(scoringSaleId);
				return result;
			}
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
		public ScoringSaleQuestionPageItemModel GetScoringSaleQuestionPageItem(
			string questionId,
			string scoringSaleId,
			string pageNo,
			int branchNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionPageItem(
					questionId,
					scoringSaleId,
					pageNo,
					branchNo);
				return model;
			}
		}
		#endregion

		#region +GetScoringSaleQuestionPageItems
		/// <summary>
		/// Get scoring sale question page items
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Array of item scoring sale question page items</returns>
		public ScoringSaleQuestionPageItemModel[] GetScoringSaleQuestionPageItems(string scoringSaleId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionPageItems(scoringSaleId);
				return model;
			}
		}
		#endregion

		#region GetScoringSaleQuestionPageItems
		/// <summary>
		/// Get scoring sale question page items
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="pageNo">Page no</param>
		/// <returns>Scoring sale question page item models</returns>
		public ScoringSaleQuestionPageItemModel[] GetScoringSaleQuestionPageItems(string scoringSaleId, int pageNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleQuestionPageItems(scoringSaleId);
				return model.Where(item => (item.PageNo == pageNo)).ToArray();
			}
		}
		#endregion

		#region +InsertScoringSaleQuestionPageItem
		/// <summary>
		/// Insert scoring sale question page item
		/// </summary>
		/// <param name="model">Scoring sale question page item model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSaleQuestionPageItem(ScoringSaleQuestionPageItemModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				repository.InsertScoringSaleQuestionPageItem(model);
			}
		}
		#endregion

		#region +DeleteScoringSaleQuestionPageItem
		/// <summary>
		/// Delete scoring sale question page item
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSaleQuestionPageItem(
			string scoringSaleId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.DeleteScoringSaleQuestionPageItem(scoringSaleId);
				return result;
			}
		}
		#endregion
		#endregion

		#region ~ScoringSaleProduct
		#region +GetScoringSaleProduct
		/// <summary>
		/// Get scoring sale product
		/// </summary>
		/// <param name="scoringSaleId">Scoring Sale Id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale page product model</returns>
		public ScoringSaleProductModel GetScoringSaleProduct(string scoringSaleId, int branchNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleProduct(scoringSaleId, branchNo);
				return model;
			}
		}
		#endregion

		#region +GetScoringSaleProducts
		/// <summary>
		/// Get scoring sale page products
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Array scoring sale page product model</returns>
		public ScoringSaleProductModel[] GetScoringSaleProducts(string scoringSaleId, string shopId)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var result = repository.GetScoringSaleProducts(scoringSaleId, shopId);
				return result;
			}
		}
		#endregion

		#region +InsertScoringSaleProduct
		/// <summary>
		/// Insert scoring sale page product
		/// </summary>
		/// <param name="model">Scoring sale page product model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSaleProduct(ScoringSaleProductModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				repository.InsertScoringSaleProduct(model);
			}
		}
		#endregion

		#region +UpdateScoringSaleProduct
		/// <summary>
		/// Update scoring sale product
		/// </summary>
		/// <param name="model">Scoring sale page product model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateScoringSaleProduct(ScoringSaleProductModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.UpdateScoringSaleProduct(model);
				return result;
			}
		}
		#endregion

		#region +DeleteScoringSaleProduct
		/// <summary>
		/// Delete scoring sale product
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSaleProduct(string scoringSaleId, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.DeleteScoringSaleProduct(scoringSaleId);
				return result;
			}
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
		public ScoringSaleResultConditionModel GetScoringSaleResultCondition(string scoringSaleId, int branchNo, int conditionBranchNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleResultCondition(scoringSaleId, branchNo, conditionBranchNo);
				return model;
			}
		}
		#endregion

		#region
		/// <summary>
		/// Get scoring sale result conditions
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="branchNo">Branch no</param>
		/// <returns>Scoring sale result condition model</returns>
		public ScoringSaleResultConditionModel[] GetScoringSaleResultConditions(string scoringSaleId, int branchNo)
		{
			using (var repository = new ScoringSaleRepository())
			{
				var model = repository.GetScoringSaleResultConditions(scoringSaleId);
				return model.Where(item => (item.BranchNo == branchNo)).ToArray();
			}
		}
		#endregion

		#region +InsertScoringSaleResultCondition
		/// <summary>
		/// Insert scoring sale result condition
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		/// <param name="accessor">Sql accessor</param>
		public void InsertScoringSaleResultCondition(ScoringSaleResultConditionModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				repository.InsertScoringSaleResultCondition(model);
			}
		}
		#endregion

		#region +UpdateScoringSaleResultCondition
		/// <summary>
		/// Update scoring sale result condition
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateScoringSaleResultCondition(ScoringSaleProductModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.UpdateScoringSaleResultCondition(model);
				return result;
			}
		}
		#endregion

		#region +DeleteScoringSaleResultCondition
		/// <summary>
		/// Delete scoring sale result condition
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int DeleteScoringSaleResultCondition(
			string scoringSaleId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ScoringSaleRepository(accessor))
			{
				var result = repository.DeleteScoringSaleResultCondition(scoringSaleId);
				return result;
			}
		}
		#endregion
		#endregion
	}
}
