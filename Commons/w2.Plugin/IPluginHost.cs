/*
=========================================================================================================
  Module      : プラグインホストインターフェース(IPluginHost.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;

namespace w2.Plugin
{
	/// <summary>
	/// プラグインホストインターフェース
	/// </summary>
	public interface IPluginHost
	{
		/// <summary>
		/// エラーログ書き出し
		/// </summary>
		/// <param name="message">メッセージ</param>
		void WriteErrorLog(string message);
		/// <summary>
		/// エラーログ書き出し
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="ex">例外</param>
		void WriteErrorLog(Exception ex, string message = "");

		/// <summary>
		/// インフォログ書き出し
		/// </summary>
		/// <param name="message">メッセージ</param>
		void WriteInfoLog(string message);
		/// <summary>
		/// インフォログ書き出し
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="ex">例外</param>
		void WriteInfoLog(Exception ex, string message = "");

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="subject">件名</param>
		/// <param name="body">本文</param>
		/// <returns>メール送信結果</returns>
		bool SendMail(string subject, string body);
		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="title">件名</param>
		/// <param name="body">本文</param>
		/// <param name="sendFrom">From</param>
		/// <param name="toList">To</param>
		/// <param name="ccList">Cc</param>
		/// <param name="bccList">Bcc</param>
		/// <returns>メール送信結果</returns>
		bool SendMail(string subject, string body, string from, List<string> toList, List<string> ccList = null, List<string> bccList = null);

		/// <summary>アプリケーション側から渡されたデータ</summary>
		Hashtable Data { get; }
		/// <summary>アプリケーション側から渡されたリクエスト情報</summary>
		HttpContext Context { get; }
	}
}
