/*
=========================================================================================================
  Module      : メール受信 (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.App.Common;

namespace w2.Commerce.MallBatch.PopGet
{
	public class Pop3Mail
	{
		/// <summary>
		/// メール受信
		/// </summary>
		/// [0] POP3サーバー名
		/// [1] POP3サーバーのポート番号
		/// [2] ユーザーID
		/// [3] パスワード
		/// [4] メールを削除するか( -d なら削除する ／ 削除しない場合は-nとでもしておく )
		/// [5] 保存先ディレクトリ
		/// </param>
		public static void Main(string[] args)
		{
			// アプリケーション名設定
			Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

			// アプリケーション共通の設定			
			ConfigurationSetting csSetting
				= new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C300_PopGet);

			FileLogger.WriteInfo("[start PopGet]");
			try
			{
				string saveTo;  // 保存先パス

				if (args.Length < 6)
				{
					FileLogger.WriteError("パラメタ不備により終了しました。");
					return;
				}

				saveTo = args[5];
				if (Directory.Exists(saveTo) == false)
				{
					FileLogger.WriteError("保存先ディレクトリにアクセスできません。");
					return;
				}

				Receive(args[0], int.Parse(args[1]), args[2], args[3], args[4] == "-d", saveTo);
			}
			catch (Exception e)
			{
				FileLogger.WriteError(e);
			}
		}

		/// <summary>
		/// POP3サーバーからメールをすべて受信する
		/// </summary>
		/// <param name="hostName">POP3サーバー名</param>
		/// <param name="portNumber">POP3サーバーのポート番号</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="passWord">パスワード</param>
		/// <param name="deleteMails">メールを削除するか</param>
		public static void Receive(string hostName, int portNumber, string userId, string passWord, bool deleteMails, string saveTo)
		{
			// 削除しないで受信
			IList<Pop3Message> mails = null;
			using (var pop = new Pop3Client(hostName, portNumber, userId, passWord, Common.PopType.Pop))
			{
				pop.Connect();
				mails = pop.GetMessages(false);
			}

			// 保存する
			var deleteList = new HashSet<Pop3Message>();

			int count = 1;
			foreach (Pop3Message mail in mails)
			{
				// ファイル名を作成 ※ファイル名の先頭を「ProjectNo」にしておくこと
				string filename = Constants.PROJECT_NO + "_"
								+ DateTime.Now.ToString("yyyyMMddHHmm")
								+ count.ToString().PadLeft(5, '0') + ".eml";
				count++;

				string path = Path.Combine(saveTo, filename);

				try
				{
					using (var fs = new FileStream(path, FileMode.CreateNew))	// 同名ファイルが存在する場合は例外にする
					using (var sw = new StreamWriter(fs, Encoding.GetEncoding(932)))
					{
						sw.Write(mail.Source);
					}

					deleteList.Add(mail);
				}
				catch
				{
					// 保存に失敗した場合、以降のメールは保存せずに保存済みのメールのみサーバーから削除する
					break;
				}
			}

			// 保存成功分を削除
			if (deleteMails && deleteList.Count > 0)
			{
				using (var pop = new Pop3Client(hostName, portNumber, userId, passWord, Common.PopType.Pop))
				{
					pop.Connect();
					pop.DeleteMessages(deleteList);
				}
			}
		}
	}
}
