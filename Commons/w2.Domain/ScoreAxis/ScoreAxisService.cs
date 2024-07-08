/*
=========================================================================================================
  Module      : Score Axis Service (ScoreAxisService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.ScoreAxis.Helper;

namespace w2.Domain.ScoreAxis
{
	/// <summary>
	/// Score axis service
	/// </summary>
	public class ScoreAxisService : ServiceBase, IScoreAxisService
	{
		#region +GetSearchHitCount
		/// <summary>
		/// Get search hit count
		/// </summary>
		/// <returns>Count</returns>
		public int GetSearchHitCount()
		{
			using (var repository = new ScoreAxisRepository())
			{
				var count = repository.GetSearchHitCount();
				return count;
			}
		}
		#endregion

		#region +Search
		/// <summary>
		/// Search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Score axis model</returns>
		public ScoreAxisModel[] Search(string searchWord)
		{
			using (var repository = new ScoreAxisRepository())
			{
				var results = repository.Search(searchWord);
				return results;
			}
		}
		#endregion

		#region +SearchAtModal
		/// <summary>
		/// Search at modal
		/// </summary>
		/// <param name="paramModel">Parameter model</param>
		/// <returns>Array of score axis model</returns>
		public ScoreAxisModel[] SearchAtModal(ScoreAxisSearchParamModel paramModel)
		{
			using (var repository = new ScoreAxisRepository())
			{
				var results = repository.SearchAtModal(paramModel);
				return results;
			}
		}
		#endregion

		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Score axis model</returns>
		public ScoreAxisModel Get(string scoreAxisId)
		{
			using (var repository = new ScoreAxisRepository())
			{
				var model = repository.Get(scoreAxisId);
				return model;
			}
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Score axis model</param>
		/// <returns>Number of affected cases</returns>
		public void Insert(ScoreAxisModel model)
		{
			using (var repository = new ScoreAxisRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Score axis model</param>
		/// <returns>Number of affected cases</returns>
		public int Update(ScoreAxisModel model)
		{
			using (var repository = new ScoreAxisRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Number of affected cases</returns>
		public int Delete(string scoreAxisId)
		{
			using (var repository = new ScoreAxisRepository())
			{
				var result = repository.Delete(scoreAxisId);
				return result;
			}
		}
		#endregion
	}
}
