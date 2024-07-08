/*
=========================================================================================================
  Module      : URL検証クラス(UrlValidator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Web
{
	/// <summary>
	/// URL検証クラス
	/// </summary>
	public class UrlValidator
	{
		#region +GetAltUrlIfOtherHostUsed URLがホストと異なるホストを利用してた場合に別のURLを取得
		/// <summary>
		/// URLがホストと異なるホストを利用してた場合に別のURLを取得
		/// </summary>
		/// <param name="host">比較元ホスト名</param>
		/// <param name="url">チェックしたいURL</param>
		/// <param name="altUrl">別URL</param>
		/// <returns>URLがホストと同一サイトであればそのURL、でなければ別URL</returns>
		public static string GetAltUrlIfOtherHostUsed(string host, string url, string altUrl)
		{
			return IsSameHostUsed(host, url) ? url : altUrl;
		}
		#endregion

		#region +IsSameHostUsed URLがホストが同じホストを利用しているか判定
		/// <summary>
		/// URLがホストが同じホストを利用しているか判定
		/// </summary>
		/// <param name="host">比較元ホスト名</param>
		/// <param name="url">チェックしたいURL</param>
		/// <returns>同一サイトか</returns>
		public static bool IsSameHostUsed(string host, string url)
		{
			// オープンリダイレクトアタックに対策する。
			// 絶対指定パスであることを保証してホスト名を確実に取得し、比較を行う。
			// "//"や"/\/"で始まるものもすべて補足できる
			Uri nextUri;
			if (Uri.TryCreate(url, UriKind.Absolute, out nextUri))
			{
				if (nextUri.Host != host) return false;
				return true;
			}
			// 相対パスか判定
			else if (Uri.TryCreate(url, UriKind.Relative, out nextUri))
			{
				return true;
			}
			// 同一でないと判定
			else
			{
				return false;
			}
		}
		#endregion
	}
}
