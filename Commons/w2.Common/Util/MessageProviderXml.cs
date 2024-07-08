/*
=========================================================================================================
  Module      : メッセージ提供クラス（XMLファイルから取得）(MessageProviderXml.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace w2.Common.Util
{
	/// <summary>
	/// メッセージ提供クラス（XMLファイルから取得）
	/// </summary>
	/// <remarks>
	/// GetMessagesでエラーメッセージ取得。
	/// GetMessagesを呼び出すごとにXMLファイルの更新日付をチェックし、
	/// 変更があれば再読み込みを行う。
	/// </remarks>
	public class MessageProviderXml : IMessageProvider
	{
		/// <summary>エラーファイル最終更新日</summary>
		private static DateTime s_dtFileLastUpdate = new DateTime(0);
		/// <summary>エラーメッセージ格納ディクショナリ</summary>
		private static Dictionary<string, string> s_dicMessages = new Dictionary<string, string>();
		/// <summary>ReaderWriterLockSlimオブジェクト</summary>
		private static System.Threading.ReaderWriterLockSlim s_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="replaces">置換パラメータ</param>
		/// <returns>エラーメッセージ</returns>
		public string GetMessages(string messageKey, params string[] replaces)
		{
			// 読み込みロックをかけて処理 排他制御 インフラ調査結果対応（PRODUCT_BASE-1024）
			s_lock.EnterReadLock();
			List<string> errorMessageXmls;
			try
			{
				errorMessageXmls = Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Select(string.Copy).ToList();
			}
			finally
			{
				s_lock.ExitReadLock();
			}

			// XMLが更新されていれば新規に読み込み
			foreach (string strXmlFilePath in errorMessageXmls)
			{
				if (File.Exists(strXmlFilePath))
				{
					if (s_dtFileLastUpdate < File.GetLastWriteTime(strXmlFilePath))
					{
						s_dtFileLastUpdate = File.GetLastWriteTime(strXmlFilePath);

						ReadMessagesXml(Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS);
						break;
					}
				}
			}

			// 読み込みロックをかけて処理
			s_lock.EnterReadLock();
			try
			{
				var message = string.Empty;
				//------------------------------------------------------
				// エラーメッセージ取得
				//------------------------------------------------------
				if (s_dicMessages.ContainsKey(messageKey))
				{
					message = s_dicMessages[messageKey];
				}
				else if (s_dicMessages.ContainsKey(MessageManager.ERRMSG_SYSTEM_ERROR))
				{
					message = s_dicMessages[MessageManager.ERRMSG_SYSTEM_ERROR];
				}

				if (replaces.Any())
				{
					message = replaces
						.Select(
							(value, index) => new
							{
								Value = value,
								Number = index + 1,
							})
						.Aggregate(
							message,
							(before, replace) =>
							{
								return before.Replace(string.Format("@@ {0} @@", replace.Number), replace.Value);
							});
				}

				return message;
			}
			finally
			{
				s_lock.ExitReadLock();
			}
		}

		/// <summary>
		/// メッセージをXMLより取得し、ディクショナリへ設定
		/// </summary>
		/// <param name="lXmlFilePaths">読み込みXML物理パスのリスト</param>
		/// <returns>取得メッセージ</returns>
		private void ReadMessagesXml(List<string> lXmlFilePaths)
		{
			// 書き込みロックをかけてDictionary更新
			s_lock.EnterWriteLock();
			try
			{
				var xdErrorMessages = new XmlDocument();
				foreach (string strXmlPath in lXmlFilePaths)
				{
					xdErrorMessages.Load(strXmlPath);
					foreach (XmlNode xnErrorMessage in xdErrorMessages.SelectSingleNode("ErrorMessages").ChildNodes)
					{
						if (xnErrorMessage.NodeType == XmlNodeType.Comment)
						{
							continue;
						}

						s_dicMessages[xnErrorMessage.Name] = xnErrorMessage.InnerText;
					}
				}
			}
			finally
			{
				s_lock.ExitWriteLock();
			}
		}
	}
}
