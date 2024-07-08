/*
=========================================================================================================
  Module      : Sale Goal Model (SaleGoalModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.SaleGoal
{
	/// <summary>
	/// Sale goal model
	/// </summary>
	public partial class SaleGoalModel
	{
		#region +Methods
		/// <summary>
		/// Get monthly sale goal
		/// </summary>
		/// <param name="month">The month that need to get monthly sale goal</param>
		/// <returns>Monthly sale goal</returns>
		public decimal GetMonthlySaleGoal(int month)
		{
			switch (month)
			{
				case 1:
					return this.MonthlyGoal1;

				case 2:
					return this.MonthlyGoal2;

				case 3:
					return this.MonthlyGoal3;

				case 4:
					return this.MonthlyGoal4;

				case 5:
					return this.MonthlyGoal5;

				case 6:
					return this.MonthlyGoal6;

				case 7:
					return this.MonthlyGoal7;

				case 8:
					return this.MonthlyGoal8;

				case 9:
					return this.MonthlyGoal9;

				case 10:
					return this.MonthlyGoal10;

				case 11:
					return this.MonthlyGoal11;

				case 12:
					return this.MonthlyGoal12;

				default:
					return 0;
			}
		}
		#endregion
	}
}