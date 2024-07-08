/*
=========================================================================================================
  Module      : 監視モジュール(ObservationModule.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Threading;

///*********************************************************************************************
/// <summary>
/// 監視モジュール
/// </summary>
///*********************************************************************************************
public class ObservationModule
{
	/// <summary>監視スレッドスリープインターバル</summary>
	private const int SLEEP_MSEC = 5000;

	/// <summary>実行メソッドデリゲート</summary>
	public delegate void ExecMethods();
	/// <summary>実行メソッド</summary>
	static ExecMethods m_emMethods = null;
	/// <summary>書き出しメソッド（アプリケーション終了時用）</summary>
	static ExecMethods m_emFlushMethods = null;

	/// <summary>
	/// 定期実行メソッド登録
	/// </summary>
	/// <param name="dTryFlushMethods">TryFlushメソッド</param>
	public static void AddMethod(ExecMethods emMethod)
	{
		m_emMethods += emMethod;
	}
	/// <summary>
	/// 書き出しメソッド登録
	/// </summary>
	/// <param name="emMethod">書き出しメソッド</param>
	public static void AddFlushMethod(ExecMethods emMethod)
	{
		m_emFlushMethods += emMethod;
	}

	/// <summary>
	/// 監視スレッド作成
	/// </summary>
	public static void CreateObserverThread()
	{
		Thread th = new Thread(new ThreadStart(new Observer().Work));
		th.Priority = ThreadPriority.BelowNormal;
		th.Start();
	}

	//*****************************************************************************************
	/// <summary>
	/// 監視スレッド
	/// </summary>
	//*****************************************************************************************
	private class Observer
	{
		/// <summary>
		/// 監視ルーチン
		/// </summary>
		public void Work()
		{
			try
			{
				while (true)
				{
					//------------------------------------------------------
					// 各メソッド呼び出し
					// （集約エラーハンドラでキャッチできないのでtry-catch）
					//------------------------------------------------------
					try
					{
						if (m_emMethods != null)
						{
							m_emMethods();
						}
					}
					catch (Exception ex)
					{
						try
						{
							// ログを落としてスルー
							AppLogger.WriteError(ex);
						}
						catch
						{
							// なにもしない
						}
					}

					//------------------------------------------------------
					// スリープ
					//------------------------------------------------------
					Thread.Sleep(SLEEP_MSEC);
				}
			}
			finally
			{
				//------------------------------------------------------
				// スレッド終了時は強制書きだし
				//------------------------------------------------------
				try
				{
					if (m_emFlushMethods != null)
					{
						m_emFlushMethods();
					}
				}
				catch (Exception ex)
				{
					try
					{
						// ログを落としてスルー
						AppLogger.WriteError(ex);
					}
					catch
					{
						// なにもしない
					}
				}
			}
		}
	}
}
