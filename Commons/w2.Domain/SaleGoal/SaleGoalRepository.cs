/*
=========================================================================================================
  Module      : Sale Goal Repository (SaleGoalRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.SaleGoal
{
	/// <summary>
	/// Sale goal repository
	/// </summary>
	public class SaleGoalRepository : RepositoryBase
	{
		/// <summary>XML key name</summary>
		private const string XML_KEY_NAME = "SaleGoal";

		#region +Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		public SaleGoalRepository()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessor">SQL accessor</param>
		public SaleGoalRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="year">The year</param>
		/// <returns>A sale goal model</returns>
		internal SaleGoalModel Get(int year)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SALEGOAL_YEAR, year.ToString() },
			};
			var result = Get(XML_KEY_NAME, "Get", input);
			return (result.Count != 0)
				? new SaleGoalModel(result[0])
				: null;
		}
		#endregion

		#region +GetSalesRevenue
		/// <summary>
		/// Get sales revenue
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <returns>A sales revenue</returns>
		internal decimal GetSalesRevenue(DateTime startDate, DateTime endDate)
		{
			var input = new Hashtable
			{
				{ "start_date", startDate },
				{ "end_date", endDate }
			};
			var result = Get(XML_KEY_NAME, "GetSalesRevenue", input);
			return (decimal)result[0][0];
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Sale goal model</param>
		/// <returns>Number of cases affected</returns>
		internal int Insert(SaleGoalModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Update
		/// <summary>
		/// Update
		/// </summary>
		/// <param name="model">Sale goal model</param>
		/// <returns>Number of cases affected</returns>
		internal int Update(SaleGoalModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
	}
}