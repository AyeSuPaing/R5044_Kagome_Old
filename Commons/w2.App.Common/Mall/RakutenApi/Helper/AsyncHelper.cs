/*
=========================================================================================================
  Module      : 非同期メソッドを同期処理するヘルパー (AsyncHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace w2.App.Common.Mall.RakutenApi.Helper
{
	/// <summary>
	/// 非同期メソッドを同期処理する
	/// </summary>
	internal static class AsyncHelper
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
		public static TResult RunSync<TResult>(Func<Task<TResult>> function)
		{
			return m_taskFactory.StartNew<Task<TResult>>(function).Unwrap<TResult>().GetAwaiter().GetResult();
		}
		/// <summary>
		/// 非同期メソッドを同期処理で呼び出す
		/// </summary>
		/// <param name="function">実行対象</param>
		public static void RunSync(Func<Task> function)
		{
			m_taskFactory.StartNew<Task>(function).Unwrap().GetAwaiter().GetResult();
		}
	}
}
