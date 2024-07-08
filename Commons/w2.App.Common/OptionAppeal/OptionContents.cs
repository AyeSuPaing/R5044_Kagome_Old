/*
=========================================================================================================
  Module      :  オプション訴求コンテンツ(OptionContents.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Text;

namespace w2.App.Common.OptionAppeal
{
	/// <summary>
	/// オプション訴求コンテンツ
	/// </summary>
	public class OptionContents
	{
		/// <summary>URL</summary>
		private readonly string _url = null;
		/// <summary>ロックオブジェクト</summary>
		private readonly object _lockObject = new object();
		/// <summary>データ（キャッシュ用）</summary>
		private string _data = null;
		/// <summary>更新日</summary>
		private DateTime? _updateDate = null;

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OptionContents()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="url">取得先URL</param>
		public OptionContents(string url)
		{
			_url = url;
		}

		/// <summary>
		/// データ取得（取得済みであればキャッシュから取得）
		/// </summary>
		/// <returns>データ</returns>
		public string GetData()
		{
			lock (_lockObject)
			{
				if ((_data == null)
					|| (_updateDate.HasValue == false)
					|| (_updateDate.Value.Date < DateTime.Now.AddMinutes(-5)))
				{
					_data = GetDataFromSite();
					_updateDate = DateTime.Now;
				}
				return _data;
			}
		}

		/// <summary>
		/// サイトからデータ取得
		/// </summary>
		/// <returns>データ</returns>
		private string GetDataFromSite()
		{
			try
			{
				// 1回目はクッキー取得
				var reqForCookie = ((HttpWebRequest)WebRequest.Create(_url));
				reqForCookie.CookieContainer = new CookieContainer();
				using (reqForCookie.GetResponse()) { }

				// 2回目はデータ取得
				var req = ((HttpWebRequest)WebRequest.Create(_url));
				req.CookieContainer = new CookieContainer();
				using (var res = (HttpWebResponse)req.GetResponse())
				using (var st = res.GetResponseStream())
				using (var sr = new StreamReader(st, Encoding.UTF8))
				{
					if (res.StatusCode == HttpStatusCode.OK)
					{
						return sr.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("xmlファイルの読み込みに失敗しました。", ex);
			}
			
			return "";
		}
	}
}
