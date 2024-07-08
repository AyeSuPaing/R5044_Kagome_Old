/*
=========================================================================================================
  Module      : Sale Goal Service (SaleGoalService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;

namespace w2.Domain.SaleGoal
{
	/// <summary>
	/// Sale goal service
	/// </summary>
	public class SaleGoalService : ServiceBase
	{
		#region +GetCurrentSaleGoal
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="year">The year</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>A sale goal model</returns>
		public SaleGoalModel Get(int year, SqlAccessor accessor = null)
		{
			using (var repository = new SaleGoalRepository(accessor))
			{
				var model = repository.Get(year);
				return model;
			}
		}
		#endregion

		#region +GetSalesRevenue
		/// <summary>
		/// Get sales revenue
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>A sales revenue</returns>
		public decimal GetSalesRevenue(
			DateTime startDate,
			DateTime endDate,
			SqlAccessor accessor = null)
		{
			using (var repository = new SaleGoalRepository(accessor))
			{
				var result = repository.GetSalesRevenue(startDate, endDate);
				return result;
			}
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Sale goal model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of cases affected</returns>
		public int Insert(SaleGoalModel model, SqlAccessor accessor = null)
		{
			using (var repository = new SaleGoalRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Update
		/// <summary>
		/// Update
		/// </summary>
		/// <param name="model">Sale goal model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of cases affected</returns>
		public int Update(SaleGoalModel model, SqlAccessor accessor = null)
		{
			using (var repository = new SaleGoalRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
	}
}