/*
=========================================================================================================
  Module      : WEBリクエスト送信バッチ処理(WebRequestSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace WebRequestSender
{
	/// <summary>
	/// WebRequestSender の概要の説明です。
	/// </summary>
	class WebRequestSender
	{
		static string m_strBaseDir = AppDomain.CurrentDomain.BaseDirectory;

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		static void Main(string[] args)
		{
			try
			{
				XmlDocument xdTargets = new XmlDocument();
				xdTargets.Load(m_strBaseDir + "Targets.xml");
				foreach (XmlNode xnUrl in xdTargets.SelectNodes("/Targets/Url"))
				{
					try
					{
						using (WebResponse res = ((HttpWebRequest)WebRequest.Create(xnUrl.InnerText)).GetResponse())
						{
							// なにもしない
						}
					}
					catch (Exception ex)
					{
						// エラーメッセージ表示
						WriteCMSLog(ex.Message);
					}
				}
			}
			catch (Exception ex)
			{
				// エラーメッセージ表示
				WriteCMSLog(ex.Message);
			}
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		private static  void WriteCMSLog(string strMessage)
		{
			//------------------------------------------------------------------
			// 書き込みログファイル名決定
			//------------------------------------------------------------------
			// ログファイルパス決定
			string strLogFilePath = AppDomain.CurrentDomain.BaseDirectory + "errors_" + DateTime.Now.ToString("yyyyMM") + ".log";

			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (System.Threading.Mutex mtx = new System.Threading.Mutex(false, strLogFilePath.Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (StreamWriter sw = new StreamWriter(strLogFilePath, true, System.Text.Encoding.Default))
						{
							// ファイル書き込み
							sw.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + strMessage + "\r\n");
							sw.Close();
						}
					}
					finally
					{
						mtx.ReleaseMutex();
					}
				}
			}
			catch
			{
				// エラーはスルー
			}
		}
	}
}
