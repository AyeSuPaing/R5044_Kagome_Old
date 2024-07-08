/*
=========================================================================================================
  Module      : プロセスユーティリティモジュール(ProcessUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace w2.App.Common.Util
{
	///*********************************************************************************************
	/// <summary>
	/// プロセスユーティリティ
	/// </summary>
	///*********************************************************************************************
	public class ProcessUtility
	{
		/// <summary>
		///  Mutexでプロセス二重実行防止しながらメソッド実行
		/// </summary>
		/// <param name="method">実行メソッド</param>
		/// <returns>実行できたか</returns>
		public static bool ExcecWithProcessMutex(Action method)
		{
			return ExecWithMutex(GenerateMutexName(Assembly.GetEntryAssembly().Location, "Process", true, true), method);
		}

		/// <summary>
		///  Mutexで二重実行防止しながらメソッド実行
		/// </summary>
		/// <param name="mutexName">Mutex名</param>
		/// <param name="method">実行メソッド</param>
		/// <returns>実行できたか</returns>
		public static bool ExecWithMutex(string mutexName, Action method)
		{
			// すべてのユーザからこの名前付きMutexにアクセスできるよう、Windowsアクセス制御セキュリティを定義
			var mutexSecurity = new MutexSecurity();
			mutexSecurity.AddAccessRule(
				new MutexAccessRule(
					new SecurityIdentifier(WellKnownSidType.WorldSid, null),
					MutexRights.Synchronize | MutexRights.Modify,
					AccessControlType.Allow));

			// Mutexを生成
			var created = false;
			using (Mutex mutex = new Mutex(false, mutexName, out created, mutexSecurity))
			{
				// Mutexハンドルを取得しているか
				var hasHandle = false;
				try
				{
					try
					{
						hasHandle = mutex.WaitOne(0, false);

						// ハンドルを取得できなかった場合
						if (hasHandle == false) return false;
					}
					catch (AbandonedMutexException)
					{
						// 前のプロセスがMutexハンドルを開放せずに終了してしまった場合はここに来る。
						// この場合でも、ハンドル自体は取得できているので取得扱いで処理する。
						// 参考: KERNEL32 -> WaitForSingleObjectEx
						// https://msdn.microsoft.com/ja-jp/library/windows/desktop/ms687036%28v=vs.85%29.aspx
						hasHandle = true;
						Console.Error.WriteLine("Abandoned Mutex Exception");
					}

					// 処理実行
					method.Invoke();

					return true;
				}
				finally
				{
					// Mutexハンドル開放
					if (hasHandle) mutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// Mutex名 を生成する
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="postfix">Postfix（用途/アクセス種別等）</param>
		/// <param name="global">Globalスコープにする（ログオンセッション間で共有可能）</param>
		/// <param name="lower">ファイルパスは小文字にする</param>
		/// <returns>Mutex名</returns>
		public static string GenerateMutexName(string filePath, string postfix, bool global, bool lower)
		{
			// Mutexカーネルオブジェクトの前に付加する文字列
			var PREFIX_W2_MUTEX = "wwMTX:";

			// ファイルパス部分生成
			var filePathEscaped = string.Empty;

			// 200文字を超えていたらハッシュ
			if (filePath.Length > 200)
			{
				// 必要に応じて小文字化 -> Unicode(UTF-16LE) -> SHA256(大文字)
				var hasher = new System.Security.Cryptography.SHA256CryptoServiceProvider();
				var bytes = Encoding.Unicode.GetBytes(lower ? filePath.ToLower() : filePath);
				filePathEscaped = string.Join(string.Empty, hasher.ComputeHash(bytes).Select(item => item.ToString("X2")));
			}
			else
			{
				// 必要に応じて小文字化 -> バックスラッシュはスラッシュに
				filePathEscaped = (lower
					? filePath.Replace(@"\", "/").ToLower()
					: filePath.Replace(@"\", "/"));
			}

			// Mutex名生成
			return string.Format(@"{0}\{1}:{2}.{3}",
				(global ? @"Global" : @"Local"),	// カーネル名前空間Prefix
				PREFIX_W2_MUTEX,	// 衝突防止
				filePathEscaped,
				postfix);
		}
	}
}
