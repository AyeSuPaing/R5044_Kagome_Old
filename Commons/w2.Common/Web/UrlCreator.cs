/*
=========================================================================================================
  Module      : URL生成クラス (UrlCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using w2.Common.Extensions;

namespace w2.Common.Web
{
	/// <summary>
	/// URL生成クラス
	/// </summary>
	public class UrlCreator
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="urlPath">ページパス</param>
		public UrlCreator(string urlPath)
		{
			this.UrlPath = urlPath;
			this.Parameters = new List<KeyValuePair<string, string>>();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="urlPath">ページパス</param>
		/// <param name="parseBaseUrlParameters">urlPathを解析する</param>
		public UrlCreator(string urlPath, bool parseBaseUrlParameters)
		{
			this.Parameters = new List<KeyValuePair<string, string>>();
			if (parseBaseUrlParameters == false)
			{
				this.UrlPath = urlPath;
				return;
			}

			this.UrlPath = urlPath.RemoveRight("?");
			this.UrlFragment = urlPath.RemoveLeft("#");

			var query = urlPath.Split('?').ElementAtOrDefault(1);
			if (string.IsNullOrEmpty(query) == false)
			{
				this.Parameters = query
					.RemoveRight("#")
					.Split('&')
					.Select(s => s.Split('='))
					.Select(s => new KeyValuePair<string, string>(s[0], HttpUtility.UrlDecode(s.ElementAtOrDefault(1))))
					.ToList();
			}
		}

		/// <summary>
		/// クエリパラメータ追加
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		public UrlCreator AddParam(string key, string value)
		{
			this.Parameters.Add(new KeyValuePair<string, string>(key, value));
			return this;
		}

		/// <summary>
		/// クエリパラメータ置換
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		/// <returns>小インスタンス</returns>
		public UrlCreator ReplaceParam(string key, string value)
		{
			this.Parameters.RemoveAll(p => p.Key == key);
			this.Parameters.Add(new KeyValuePair<string, string>(key, value));

			return this;
		}

		/// <summary>
		/// URLフラグメント追加
		/// </summary>
		/// <param name="urlFragment">URLフラグメント</param>
		/// <returns>このインスタンス</returns>
		public UrlCreator WithUrlFragment(string urlFragment)
		{
			this.UrlFragment = urlFragment;
			return this;
		}

		/// <summary>
		/// URL作成
		/// </summary>
		/// <param name="encode">Encode</param>
		/// <returns>作成したURL</returns>
		public string CreateUrl(Encoding encode = null)
		{
			var builder = new StringBuilder(this.UrlPath);
			if (this.Parameters.Any())
			{
				builder
					.Append(this.UrlPath.Contains('?') ? "&" : "?")
					.Append(
						this.Parameters
							.Select(item =>
								string.Format(
									"{0}={1}",
									item.Key,
									HttpUtility.UrlEncode(item.Value, encode ?? Encoding.UTF8)))
							.JoinToString("&"));
			}

			if (string.IsNullOrWhiteSpace(this.UrlFragment) == false)
			{
				builder.AppendFormat("#{0}", HttpUtility.UrlEncode(this.UrlFragment.TrimStart('#')));
			}

			return builder.ToString();
		}

		/// <summary>
		/// クエリパラメータ変更
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		/// <returns>自オブジェクト</returns>
		public UrlCreator ChangeParam(string key, string value)
		{
			if (string.IsNullOrEmpty(key)) return this;

			this.Parameters = this.Parameters.Where(param => (param.Key != key)).ToList();
			AddParam(key, value);
			return this;
		}

		/// <summary>
		/// クエリパラメータ追加
		/// </summary>
		/// <param name="param">パラメータリスト</param>
		/// <returns>このインスタンス</returns>
		public UrlCreator AddRangeParam(IEnumerable<KeyValuePair<string, string>> param)
		{
			this.Parameters.AddRange(param);
			return this;
		}

		/// <summary>URLフラグメント</summary>
		private string UrlFragment { get; set; }
		/// <summary>URLパス</summary>
		private string UrlPath { get; set; }
		/// <summary>パラメータリスト</summary>
		private List<KeyValuePair<string, string>> Parameters { get; set; }
	}
}
