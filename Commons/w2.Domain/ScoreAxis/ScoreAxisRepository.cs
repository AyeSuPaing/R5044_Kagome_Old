/*
=========================================================================================================
  Module      : Score Axis Repository (ScoreAxisRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ScoreAxis.Helper;

namespace w2.Domain.ScoreAxis
{
	/// <summary>
	/// Score axis repository
	/// </summary>
	internal class ScoreAxisRepository : RepositoryBase
	{
		/// <summary>XML page name</summary>
		private const string XML_KEY_NAME = "ScoreAxis";

		#region ~Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		internal ScoreAxisRepository()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		internal ScoreAxisRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount
		/// <summary>
		/// Get search hit count
		/// </summary>
		/// <returns>Count</returns>
		internal int GetSearchHitCount()
		{
			var result = Get(XML_KEY_NAME, "GetSearchHitCount");
			return (int)result[0][0];
		}
		#endregion

		#region ~Search
		/// <summary>
		/// Search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Score axis model</returns>
		internal ScoreAxisModel[] Search(string searchWord)
		{
			var param = new Hashtable
			{
				{ "search_word", searchWord }
			};
			var result = Get(XML_KEY_NAME, "Search", param);
			return result.Cast<DataRowView>().Select(row => new ScoreAxisModel(row)).ToArray();
		}
		#endregion

		#region ~SearchAtModal
		/// <summary>
		/// Search
		/// </summary>
		/// <param name="paramModel">Parameter model</param>
		/// <returns>Array of score axis model</returns>
		internal ScoreAxisModel[] SearchAtModal(ScoreAxisSearchParamModel paramModel)
		{
			var input = paramModel.CreateHashtableParams();
			var result = Get(XML_KEY_NAME, "SearchAtModal", input);
			return result.Cast<DataRowView>().Select(item => new ScoreAxisModel(item)).ToArray();
		}
		#endregion

		#region ~Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Score axis model</returns>
		internal ScoreAxisModel Get(string scoreAxisId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCOREAXIS_SCORE_AXIS_ID, scoreAxisId },
			};
			var result = Get(XML_KEY_NAME, "Get", input);
			if (result.Count == 0) return null;
			return new ScoreAxisModel(result[0]);
		}
		#endregion

		#region ~Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Score axis model</param>
		internal void Insert(ScoreAxisModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update
		/// <summary>
		/// Update
		/// </summary>
		/// <param name="model">Score axis model</param>
		/// <returns>Number of affected cases</returns>
		internal int Update(ScoreAxisModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Number of affected cases</returns>
		internal int Delete(string scoreAxisId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SCOREAXIS_SCORE_AXIS_ID, scoreAxisId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}
		#endregion
	}
}
