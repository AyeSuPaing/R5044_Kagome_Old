/*
=========================================================================================================
  Module      : データキャッシュコントローラ基底クラス(DataCacheControllerBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.RefreshFileManager;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// データキャッシュコントローラ基底クラス
	/// </summary>
	/// <typeparam name="T">キャッシュデータの型</typeparam>
	public abstract class DataCacheControllerBase<T> : IDataCacheController
		where T : class
	{
		/// <summary>ロックオブジェクト</summary>
		private readonly object m_lockObject = new object();
		/// <summary>リフレッシュファイルタイプ</summary>
		private readonly RefreshFileType m_refreshFileType;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="refreshFileType">リフレッシュファイルタイプ</param>
		protected DataCacheControllerBase(RefreshFileType refreshFileType)
		{
			m_refreshFileType = refreshFileType;
		}

		/// <summary>
		/// キャッシュをリフレッシュし、監視セット
		/// </summary>
		private void RefreshCacheDataAndSetOvserver()
		{
			// キャッシュデータリフレッシュ
			RefreshCacheData();

			// リフレッシュファイル監視セット
			RefreshFileManagerProvider.GetInstance(m_refreshFileType).AddObservation(RefreshCacheData);
		}

		/// <summary>キャッシュデータリフレッシュ</summary>
		internal abstract void RefreshCacheData();

		/// <summary>キャッシュデータ</summary>
		public T CacheData
		{
			get
			{
				lock (m_lockObject)
				{
					if (m_cacheData == null) RefreshCacheDataAndSetOvserver();
					return m_cacheData;
				}
			}
			protected set { m_cacheData = value; }
		}
		private T m_cacheData = null;
	}
}
