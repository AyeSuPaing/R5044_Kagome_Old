/*
=========================================================================================================
  Module      : Async Helper (AsyncHelper.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Async helper class
	/// </summary>
	public static class AsyncHelper
	{
		/// <summary>Task factory</summary>
		private static readonly TaskFactory m_taskFactory = new TaskFactory
			(
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskContinuationOptions.None,
				TaskScheduler.Default
			);

		/// <summary>
		/// Run sync
		/// </summary>
		/// <typeparam name="TResult">The type of result data</typeparam>
		/// <param name="function">Function</param>
		/// <returns>The result data</returns>
		internal static TResult RunSync<TResult>(Func<Task<TResult>> function)
		{
			return m_taskFactory
				.StartNew(function)
				.Unwrap()
				.GetAwaiter()
				.GetResult();
		}
	}
}
