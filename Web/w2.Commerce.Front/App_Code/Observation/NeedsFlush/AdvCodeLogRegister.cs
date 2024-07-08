/*
=========================================================================================================
  Module      : 広告コードログ登録モジュール(AdvCodeLogRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;

///*********************************************************************************************
/// <summary>
/// 広告コードログ登録モジュール
/// </summary>
///*********************************************************************************************
public class AdvCodeLogRegister
{
	/// <summary>次回更新日時</summary>
	private static DateTime m_dtUpdateDatetime = DateTime.Now;
	/// <summary>広告コードキュー</summary>
	private static List<AdvCodeLogRegisterUnit> m_lAdvCodeLogRegisterUnits = new List<AdvCodeLogRegisterUnit>();

	/// <summary>キュー最大値</summary>
	private const int COUNT_QUEUE_SIZE_MAX = 2000;
	/// <summary>フラッシュインターバル（秒）</summary>
	private const int FLUSH_INTERVAL_SEC = 60;

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	static AdvCodeLogRegister()
	{
		// 監視モジュールにメソッド登録
		if (Constants.W2MP_AFFILIATE_OPTION_ENABLED)
		{
			ObservationModule.AddMethod(TryFlush);
			ObservationModule.AddFlushMethod(Flush);
		}
	}

	/// <summary>
	/// 広告コード存在チェック
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="strAdvCode">広告コード</param>
	public static bool IsValid(string strDeptId, string strAdvCode)
	{
		bool blIsValid = false;

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatemet = new SqlStatement("AdvCode", "GetAdvCodeCount"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_ADVCODE_DEPT_ID, strDeptId);
			htInput.Add(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, strAdvCode);

			blIsValid = ((int)sqlStatemet.SelectSingleStatementWithOC(sqlAccessor, htInput)[0][0] != 0);
		}

		return blIsValid;
	}

	/// <summary>
	/// 広告コードログ登録
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="strAdvCode">広告コード</param>
	/// <param name="strCareerId">キャリアID</param>
	/// <param name="strAccessUserId">アクセスユーザID</param>
	public static void Regist(string strDeptId, string strAdvCode, string strCareerId, string strAccessUserId)
	{
		lock (m_lAdvCodeLogRegisterUnits)
		{
			if (m_lAdvCodeLogRegisterUnits.Count < COUNT_QUEUE_SIZE_MAX)	// 念のため（無限に増えるとアプリが落ちそう）
			{
				m_lAdvCodeLogRegisterUnits.Add(new AdvCodeLogRegisterUnit(strDeptId, strAdvCode, strCareerId, strAccessUserId));
			}
		}
	}

	/// <summary>
	/// 広告コードログ書き出し試行
	/// </summary>
	/// <remarks>監視スレッドなどにより登録</remarks>
	public static void TryFlush()
	{
		lock (m_lAdvCodeLogRegisterUnits)
		{
			try
			{
				// 最大数まで達している or 一定時間経った
				if ((m_lAdvCodeLogRegisterUnits.Count > COUNT_QUEUE_SIZE_MAX)
					|| (DateTime.Now > m_dtUpdateDatetime)
					|| (DateTime.Now.Date != m_dtUpdateDatetime.Date))
				{
					//------------------------------------------------------
					// 次回更新日時セット
					//------------------------------------------------------
					m_dtUpdateDatetime = DateTime.Now.AddSeconds(FLUSH_INTERVAL_SEC);

					//------------------------------------------------------
					// 広告コードログ書き出し
					//------------------------------------------------------
					Flush();
				}
			}
			catch (Exception ex)
			{
				// ログを落としてスルー
				AppLogger.WriteError(ex);
			}
		}
	}

	/// <summary>
	/// 広告コードログ書き出し
	/// </summary>
	public static void Flush()
	{
		if (m_lAdvCodeLogRegisterUnits.Count != 0)
		{
			//------------------------------------------------------
			// ローカル変数へ移してロック解除
			//------------------------------------------------------
			List<AdvCodeLogRegisterUnit> lAdvCodeLogRegisterUnits = m_lAdvCodeLogRegisterUnits;
			m_lAdvCodeLogRegisterUnits = new List<AdvCodeLogRegisterUnit>();

			//------------------------------------------------------
			// ＤＢ登録
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatemet = new SqlStatement("AdvCode", "InsertAdvCodeLog"))
			{
				sqlAccessor.OpenConnection();

				foreach (AdvCodeLogRegisterUnit unit in lAdvCodeLogRegisterUnits)
				{
					sqlStatemet.ExecStatement(sqlAccessor, unit.InputParam);
				}
			}
		}
	}

	//*****************************************************************************************
	/// <summary>
	/// 広告コード登録ユニット
	/// </summary>
	//*****************************************************************************************
	public class AdvCodeLogRegisterUnit
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strAdvCode">広告コード</param>
		/// <param name="strCareerId">キャリアID</param>
		/// <param name="strAccessUserId">アクセスユーザID</param>
		public AdvCodeLogRegisterUnit(string strDeptId, string strAdvCode, string strCareerId, string strAccessUserId)
		{
			// 広告コード検索用
			this.InputParam = new Hashtable();
			this.InputParam.Add(Constants.FIELD_ADVCODE_DEPT_ID, strDeptId);

			// 広告コードログ登録用
			this.InputParam.Add(Constants.FIELD_ADVCODELOG_ACCESS_DATE, DateTime.Now.ToString("yyyy/MM/dd"));
			this.InputParam.Add(Constants.FIELD_ADVCODELOG_ACCESS_TIME, DateTime.Now.ToString("HH:mm:ss"));
			this.InputParam.Add(Constants.FIELD_ADVCODELOG_ADVERTISEMENT_CODE, strAdvCode);
			this.InputParam.Add(Constants.FIELD_ADVCODELOG_CAREER_ID, strCareerId);
			this.InputParam.Add(Constants.FIELD_ADVCODELOG_MOBILE_UID, "");
			this.InputParam.Add(Constants.FIELD_ADVCODELOG_ACCESS_USER_ID, strAccessUserId);
		}

		/// <summary>入力パラメタ</summary>
		public Hashtable InputParam { get; private set; }
	}
}
