/*
=========================================================================================================
  Module      : データキャッシュマネージャ(DataCacheManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.Caching;

namespace w2.App.Common.Util
{
	/// <summary>
	/// データキャッシュマネージャ
	/// </summary>
	public class DataCacheManager
	{
		/// <summary>インスタンス</summary>
		private static readonly DataCacheManager m_instance = new DataCacheManager();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private DataCacheManager()
		{
		}

		/// <summary>
		/// インスタンス取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static DataCacheManager GetInstance()
		{
			return m_instance;
		}

		/// <summary>
		/// キャッシュまたはメソッドからデータ取得
		/// </summary>
		/// <param name="cacheKey">キャッシュキー</param>
		/// <param name="expireMinutes">キャッシュの有効期限(分)（0：キャッシュしない）</param>
		/// <param name="method">データ取得用メソッド</param>
		/// <param name="cacheDependency">キャッシュデータのファイル依存関係またはキャッシュ キー依存関係</param>
		/// <returns>取得データ</returns>
		public T GetData<T>(string cacheKey, int expireMinutes, Func<T> method, CacheDependency cacheDependency = null)
			where T : class
		{
			// コンテキストを取得できない場合(オンライン以外からの呼び出し)は、キャッシュに格納しない
			var context = HttpContext.Current;
			if (context == null) return method();

			var cache = context.Cache;
			lock (cacheKey)
			{
				// キャッシュから取得できればそれを返す
				var data = (cache != null) ? (T)cache[cacheKey] : null;
				if (data != null) return data;

				// 取得
				data = method();

				// キャッシング
				if ((cache != null) && (expireMinutes != 0))
				{
					// 0秒台は負荷が高まる可能性があるので3秒台でクリアする。
					var expireDate = DateTime.Parse(DateTime.Now.AddMinutes(expireMinutes).ToString("yyyy/MM/dd HH:mm:03"));
					cache.Insert(cacheKey, data, cacheDependency, expireDate, Cache.NoSlidingExpiration);
				}
				return data;
			}
		}
	}
}