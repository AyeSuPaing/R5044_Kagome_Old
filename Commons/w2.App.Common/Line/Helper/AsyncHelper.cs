/*
=========================================================================================================
  Module      : メソッドを同期処理(AsyncHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace w2.App.Common.Line.Helper
{
	/// <summary>
	/// 非同期メソッドを同期処理する
	/// </summary>
	public static class AsyncHelper
	{
		/// <summary>タスクファクトリー</summary>
		private static readonly TaskFactory m_taskFactory = new TaskFactory(
			CancellationToken.None,
			TaskCreationOptions.None,
			TaskContinuationOptions.None,
			TaskScheduler.Default);

		/// <summary>
		/// 非同期メソッドを同期処理で呼び出す
		/// </summary>
		/// <typeparam name="TResult">戻り値の型</typeparam>
		/// <param name="function">実行対象</param>
		/// <returns>メソッドの戻り値</returns>
		internal static TResult RunSync<TResult>(Func<Task<TResult>> function)
		{
			return m_taskFactory.StartNew(function).Unwrap().GetAwaiter().GetResult();
		}
		/// <summary>
		/// 非同期メソッドを同期処理で呼び出す
		/// </summary>
		/// <param name="function">実行対象</param>
		internal static void RunSync(Func<Task> function)
		{
			m_taskFactory.StartNew(function).Unwrap().GetAwaiter().GetResult();
		}
	}
}
