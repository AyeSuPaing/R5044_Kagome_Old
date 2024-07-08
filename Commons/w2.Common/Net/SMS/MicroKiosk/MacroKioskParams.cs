/*
=========================================================================================================
  Module      : マクロキオスク用SMS連携パラメタ(MacroKioskParams.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Text;
using System.Web;
using w2.Common.Web;

namespace w2.Common.Net.SMS.MicroKiosk
{
	/// <summary>
	/// マクロキオスク用SMS連携パラメタ
	/// </summary>
	public class MacroKioskParams : IHttpApiRequestData
	{
		/// <summary>
		/// タイプの規定値
		/// </summary>
		private string m_typeDefault = "5";


		/// <summary>
		/// POST文字生成
		/// </summary>
		/// <returns>POSTする文字</returns>
		public string CreatePostString()
		{
			var rtn = string.Format(
				"user={0}&pass={1}&type={2}&to={3}&from={4}&text={5}&servid={6}&title={7}",
				HttpUtility.UrlEncode(this.User),
				HttpUtility.UrlEncode(this.Pass),
				HttpUtility.UrlEncode(this.Type),
				HttpUtility.UrlEncode(this.To),
				HttpUtility.UrlEncode(this.From),
				HttpUtility.UrlEncode(this.TextByUCS2),
				HttpUtility.UrlEncode(this.ServId),
				HttpUtility.UrlEncode(this.TitleByUCS2));

			return rtn;
		}

		/// <summary>
		/// ユーザーセット
		/// </summary>
		/// <param name="val">セット内容</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams SetUser(string val)
		{
			this.User = val;
			return this;
		}

		/// <summary>
		/// パスワードセット
		/// </summary>
		/// <param name="val">セット内容</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams SetPass(string val)
		{
			this.Pass = val;
			return this;
		}

		/// <summary>
		/// サービスIDセット
		/// </summary>
		/// <param name="val">セット内容</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams SetServId(string val)
		{
			this.ServId = val;
			return this;
		}

		/// <summary>
		/// SMS送信先セット
		/// </summary>
		/// <param name="val">セット内容</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams SetTo(string val)
		{
			this.To = val;
			return this;
		}

		/// <summary>
		/// SMS送信元セット
		/// </summary>
		/// <param name="val">セット内容</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams SetFrom(string val)
		{
			this.From = val;
			return this;
		}

		/// <summary>
		/// SMS本文セット
		/// </summary>
		/// <param name="val">セット内容</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams SetText(string val)
		{
			this.Text = val;
			return this;
		}

		/// <summary>
		/// UCS2に変換する
		/// </summary>
		/// <param name="val">変換対象</param>
		/// <returns>UCS2に変換したもの</returns>
		public string ToUCS2(string val)
		{
			byte[] ba = Encoding.BigEndianUnicode.GetBytes(val);
			var hex = BitConverter.ToString(ba);
			hex = hex.Replace("-", "");
			return hex;
		}

		/// <summary>
		/// タイプのデフォルト値変更
		/// </summary>
		/// <param name="setVal">変更する値</param>
		/// <returns>パラメタ</returns>
		public MacroKioskParams ChangeTypeDefaultValue(string setVal)
		{
			m_typeDefault = setVal;
			return this;
		}
		
		/// <summary>ユーザー</summary>
		public string User { get; set; }
		/// <summary>パスワード</summary>
		public string Pass { get; set; }
		/// <summary>タイプ</summary>
		public string Type { get { return m_typeDefault; } }
		/// <summary>SMS送信先</summary>
		public string To { get; set; }
		/// <summary>SMS送信元</summary>
		public string From { get; set; }
		/// <summary>SMS本文</summary>
		public string Text { get; set; }
		/// <summary>SMS本文をUCS2に変換したもの</summary>
		public string TextByUCS2
		{
			get { return this.Text == null ? "" : ToUCS2(this.Text); }
		}
		/// <summary>サービスID</summary>
		public string ServId { get; set; }
		/// <summary>タイトル（連携可能だが使わない）、Alphanumeric only</summary>
		public string Title { get; set; }
		/// <summary>タイトルをUCS2に変換したもの</summary>
		public string TitleByUCS2
		{
			get { return this.Title == null ? "" : ToUCS2(this.Title); }
		}
	}
}
