/*
=========================================================================================================
  Module      : Score Axis Service Interface (IScoreAxisService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.ScoreAxis.Helper;

namespace w2.Domain.ScoreAxis
{
	/// <summary>
	/// Score axis service interface
	/// </summary>
	public interface IScoreAxisService : IService
	{
		/// <summary>
		/// Get search hit count
		/// </summary>
		/// <returns>Count</returns>
		int GetSearchHitCount();

		/// <summary>
		/// Search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Score axis model</returns>
		ScoreAxisModel[] Search(string searchWord);

		/// <summary>
		/// Search at modal
		/// </summary>
		/// <param name="paramModel">Parameter model</param>
		/// <returns>Array of score axis model</returns>
		ScoreAxisModel[] SearchAtModal(ScoreAxisSearchParamModel paramModel);

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Score axis model</returns>
		ScoreAxisModel Get(string scoreAxisId);

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Score axis model</param>
		void Insert(ScoreAxisModel model);

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="model">Score axis model</param>
		/// <returns>Number of affected cases</returns>
		int Update(ScoreAxisModel model);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Number of affected cases</returns>
		int Delete(string scoreAxisId);
	}
}
