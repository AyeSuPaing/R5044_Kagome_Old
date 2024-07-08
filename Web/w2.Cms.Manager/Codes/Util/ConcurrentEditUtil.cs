/*
=========================================================================================================
  Module      : 同時編集回避ユーティリティー(ConcurrentEditUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using w2.Cms.Manager.Codes.Common;

namespace w2.Cms.Manager.Codes.Util
{
	/// <summary>
	/// 同時編集回避ユーティリティー
	/// </summary>
	public class ConcurrentEditUtil
	{
		/// <summary>ロックオブジェクト</summary>
		private static object m_lockObject = new object();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ConcurrentEditUtil()
		{
			this.SessionWrapper = new SessionWrapper();
		}

		/// <summary>
		/// 開かれているか判定（キャッシュ確認）
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>エラー</returns>
		public string CheckOtherOperatorFileOpening(string fileName)
		{
			var cache = MemoryCache.Default;
			var cacheData = cache.Get(fileName);
			if (cacheData == null) return "";

			var kvPair = (KeyValuePair<string, DateTime>)cacheData;
			// 存在したが、ログインオペレータが一致または1分以上更新がない場合は開かれていないと判定
			if ((kvPair.Key == this.SessionWrapper.LoginOperatorName)
				|| (kvPair.Value.AddMinutes(1) < DateTime.Now))
			{
				return "";
			}
			return WebMessages.FileOtherOperatorOpeningError.Replace("@@ 1 @@", kvPair.Key);
		}

		/// <summary>
		/// オペレーターが開いているということを通知（キャッシュ更新）
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>エラー</returns>
		public string NoticeOperatorOpeningFile(string fileName)
		{
			if (Constants.CONCURRENT_EDIT_EXCLUSION_LOGIN_OPERATOR_ID_LIST.Contains(
				this.SessionWrapper.LoginOperator.OperatorId))
			{
				return string.Empty;
			}

			lock (m_lockObject)
			{
				// キャッシュに存在しない場合はfileNameとログインオペレータを入れる
				var cache = MemoryCache.Default;
				var cacheData = cache.Get(fileName);
				if (cacheData == null)
				{
					var policy = new CacheItemPolicy
					{
						AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60.0)
					};
					cache.Set(
						fileName,
						new KeyValuePair<string, DateTime>(this.SessionWrapper.LoginOperatorName, DateTime.Now),
						policy);
					return "";
				}

				// 存在したが、ログインオペレータが一致または1分以上更新がない場合は上書き
				var kvPair = (KeyValuePair<string, DateTime>)cacheData;
				if ((kvPair.Key == this.SessionWrapper.LoginOperatorName)
					|| (kvPair.Value.AddMinutes(1) < DateTime.Now))
				{
					var policy = new CacheItemPolicy
					{
						AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60.0)
					};
					cache.Set(
						fileName,
						new KeyValuePair<string, DateTime>(this.SessionWrapper.LoginOperatorName, DateTime.Now),
						policy);
					return "";
				}
				return WebMessages.FileOtherOperatorOpenError.Replace("@@ 1 @@", kvPair.Key);
			}
		}

		/// <summary>セッションラッパー</summary>
		public SessionWrapper SessionWrapper { get; set; }
	}
}