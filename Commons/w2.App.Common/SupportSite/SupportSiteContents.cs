/*
=========================================================================================================
  Module      : サポートサイトコンテンツ(SupportSiteContents.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace w2.App.Common.SupportSite
{
	/// <summary>
	/// サポートサイトコンテンツ（日付が変わると再取得）
	/// </summary>
	public class SupportSiteContents
	{
		/// <summary>URL</summary>
		private readonly string m_url = null;
		/// <summary>ロックオブジェクト</summary>
		private readonly object m_lockObject = new object();
		/// <summary>データ（キャッシュ用）</summary>
		private string m_data = null;
		/// <summary>更新日</summary>
		private DateTime? m_updateDate = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="url">取得先URL</param>
		public SupportSiteContents(string url)
		{
			m_url = url;
		}

		/// <summary>
		/// データ取得（取得済みであればキャッシュから取得）
		/// </summary>
		/// <returns>データ</returns>
		public string GetData()
		{
			lock (m_lockObject)
			{
				if ((m_data == null) || (m_updateDate.HasValue == false) || (m_updateDate.Value.Date < DateTime.Now.Date))
				{
					m_data = GetDataFromSite();
					m_updateDate = DateTime.Now;
				}
				return m_data;
			}
		}

		/// <summary>
		/// サイトからデータ取得
		/// </summary>
		/// <returns>データ</returns>
		private string GetDataFromSite()
		{
			// 1回目はクッキー取得
			var cookieContainer = new CookieContainer();
			var reqForCookie = ((HttpWebRequest)WebRequest.Create(m_url));
			reqForCookie.CookieContainer = cookieContainer;
			using (reqForCookie.GetResponse()) { }

			// 2回目はデータ取得
			var req = ((HttpWebRequest)WebRequest.Create(m_url));
			req.CookieContainer = cookieContainer;
			using (var res = (HttpWebResponse)req.GetResponse())
			using (var st = res.GetResponseStream())
			using (var sr = new StreamReader(st, Encoding.UTF8))
			{
				if (res.StatusCode == HttpStatusCode.OK)
				{
					return sr.ReadToEnd();
				}
			}
			return "";
		}
	}
}
