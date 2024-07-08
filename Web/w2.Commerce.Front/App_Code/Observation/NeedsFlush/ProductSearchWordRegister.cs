/*
=========================================================================================================
  Module      : 商品検索文字列登録モジュール(ProductSearchWordRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;

///*********************************************************************************************
/// <summary>
/// 商品検索文字列登録モジュール
/// </summary>
///*********************************************************************************************
public class ProductSearchWordRegister
{
	/// <summary>次回更新日時</summary>
	private static DateTime m_dtUpdateDatetime = DateTime.MinValue;
	/// <summary>広告コードキュー</summary>
	private static List<ProductSearchWordRegisterUnit> m_lProductSearchWordRegisterUnits = new List<ProductSearchWordRegisterUnit>();

	/// <summary>キュー最大値</summary>
	private const int COUNT_QUEUE_SIZE_MAX = 2000;
	/// <summary>フラッシュインターバル（秒）</summary>
	private const int FLUSH_INTERVAL_SEC = 60;

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	static ProductSearchWordRegister()
	{
		// 監視モジュールにメソッド登録
		if (Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED)
		{
			ObservationModule.AddMethod(TryFlush);
			ObservationModule.AddFlushMethod(Flush);
		}
	}

	/// <summary>
	/// 検索ワード登録
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="accessKbn">アクセス区分</param>
	/// <param name="word">検索ワード</param>
	/// <param name="hits">ヒット数</param>
	public static void Regist(string shopId, string accessKbn, string word, int hits)
	{
		lock (m_lProductSearchWordRegisterUnits)
		{
			if (m_lProductSearchWordRegisterUnits.Count < COUNT_QUEUE_SIZE_MAX)	// 念のため（無限に増えるとアプリが落ちそう）
			{
				m_lProductSearchWordRegisterUnits.Add(new ProductSearchWordRegisterUnit(shopId, accessKbn, word, hits));
			}
		}
	}

	/// <summary>
	/// 検索ワード情報書き出し試行
	/// </summary>
	/// <returns>登録</returns>
	/// <remarks>監視スレッドなどにより登録</remarks>
	public static void TryFlush()
	{
		lock (m_lProductSearchWordRegisterUnits)
		{
			try
			{
				// 最大数まで達している or 一定時間経った or 日付が変わった
				if ((m_lProductSearchWordRegisterUnits.Count > COUNT_QUEUE_SIZE_MAX)
					|| (DateTime.Now > m_dtUpdateDatetime)
					|| (DateTime.Now.Date != m_dtUpdateDatetime.Date))
				{
					//------------------------------------------------------
					// 次回更新日時セット
					//------------------------------------------------------
					m_dtUpdateDatetime = DateTime.Now.AddSeconds(FLUSH_INTERVAL_SEC);

					//------------------------------------------------------
					// 検索ワード情報書き出し
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
	/// 検索ワード情報書き出し
	/// </summary>
	public static void Flush()
	{
		if (m_lProductSearchWordRegisterUnits.Count != 0)
		{
			//------------------------------------------------------
			// ローカル変数へ移してロック解除
			//------------------------------------------------------
			List<ProductSearchWordRegisterUnit> lSearchWordRegisterUnits = m_lProductSearchWordRegisterUnits;
			m_lProductSearchWordRegisterUnits = new List<ProductSearchWordRegisterUnit>();

			//------------------------------------------------------
			// ＤＢ登録
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatemet = new SqlStatement("ProductSearchWord", "RegistSearchWord"))
			{
				sqlAccessor.OpenConnection();
				foreach (ProductSearchWordRegisterUnit unit in lSearchWordRegisterUnits)
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTSEARCHWORDHISTORY_DEPT_ID, unit.DeptId);
					htInput.Add(Constants.FIELD_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN, unit.AccessKbn);
					htInput.Add(Constants.FIELD_PRODUCTSEARCHWORDHISTORY_SEARCH_WORD, unit.Word);
					htInput.Add(Constants.FIELD_PRODUCTSEARCHWORDHISTORY_HITS, unit.Hits);
					htInput.Add(Constants.FIELD_PRODUCTSEARCHWORDHISTORY_DATE_CREATED, unit.DateCreated);

					sqlStatemet.ExecStatement(sqlAccessor, htInput);
				}
			}
		}
	}

	//*****************************************************************************************
	/// <summary>
	/// 検索ワード登録ユニット
	/// </summary>
	//*****************************************************************************************
	public class ProductSearchWordRegisterUnit
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="accessKbn">アクセス区分</param>
		/// <param name="word">検索ワード</param>
		/// <param name="hits">ヒット数</param>
		public ProductSearchWordRegisterUnit(string deptId, string accessKbn, string word, int hits)
		{
			this.DeptId = deptId;
			this.AccessKbn = accessKbn;
			this.Word = word;
			this.Hits = hits;
			this.DateCreated = DateTime.Now;
		}

		/// <summary>識別ID</summary>
		public string DeptId { get; private set; }
		/// <summary>アクセス区分</summary>
		public string AccessKbn { get; private set; }
		/// <summary>検索ワード</summary>
		public string Word { get; private set; }
		/// <summary>ヒット数</summary>
		public int Hits { get; private set; }
		/// <summary>データ作成日</summary>
		public DateTime DateCreated { get; private set; }
	}
}
