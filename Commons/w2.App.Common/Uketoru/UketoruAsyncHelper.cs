/*
=========================================================================================================
  Module      : メソッドを同期処理 (UketoruAsyncHelper.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace w2.App.Common.Uketoru
{
	/// <summary>
	/// 非同期メソッドを同期処理する
	/// </summary>
	public static class UketoruAsyncHelper
	{
		/// <summary>タスクファクトリー</summary>
		private static readonly TaskFactory m_taskFactory = new TaskFactory
			(
				CancellationToken.None,
				TaskCreationOptions.None,
				TaskContinuationOptions.None,
				TaskScheduler.Default
			);

		/// <summary>
		/// 非同期メソッドを同期処理で呼び出す
		/// </summary>
		/// <typeparam name="TResult">戻り値の型</typeparam>
		/// <param name="function">実行対象</param>
		/// <returns>メソッドの戻り値</returns>
		public static TResult RunSync<TResult>(Func<Task<TResult>> function)
		{
			return m_taskFactory
				.StartNew(function)
				.Unwrap()
				.GetAwaiter()
				.GetResult();
		}
	}
}
