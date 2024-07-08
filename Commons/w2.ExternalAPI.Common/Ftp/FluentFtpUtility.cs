/*
=========================================================================================================
  Module      : FluentFtpAPI ユーティリティクラス(FluentFtpUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using w2.Common.Logger;

namespace w2.ExternalAPI.Common.Ftp
{
	/// <summary>
	///  FluentFtpAPI ユーティリティクラス
	/// </summary>
	public class FluentFtpUtility
	{
		/// <summary>アップロードタイプ</summary>
		public enum UploadType
		{
			/// <summary>上書き許可</summary>
			OverWrite,
			/// <summary>上書き禁止</summary>
			Append,
			/// <summary>アップロード先に同一ファイルが存在する場合にアップロードをスキップ</summary>
			Skip
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ftpServerHost">FTPサーバのホスト名</param>
		/// <param name="userName">FTPサーバのユーザ</param>
		/// <param name="userPassword">FTPサーバのパスワード</param>
		/// <param name="useActive">アクティブモードはtrue,パッシブモードはfalse</param>
		/// <param name="enableSSL">SSLを利用する場合はtrue, 利用しない場合はfalse</param>
		/// <param name="port">FTP port</param>
		public FluentFtpUtility(
			string ftpServerHost,
			string userName,
			string userPassword,
			bool useActive,
			bool enableSSL = false,
			int? port = null)
		{
			this.FtpServerHost = ftpServerHost;
			this.UserName = userName;
			this.UserPassword = userPassword;
			this.UseActive = useActive;
			this.EnableSsl = enableSSL;
			this.Port = port;
		}

		/// <summary>
		/// FTPアップロード
		/// </summary>
		/// <param name="senderFile">アップロード元ファイルのフルパス</param>
		/// <param name="destinationFile">アップロード先のファイルパス(ftp://[ホスト名]は含まない)</param>
		/// <param name="uploadType">アップロードタイプ デフォルト上書き許可</param>
		/// <returns>FTPアップロードに成功した場合はtrue,失敗した場合はfalse</returns>
		public bool Upload(string senderFile, string destinationFile, UploadType uploadType = UploadType.OverWrite)
		{
			using (var client = InitFtpClient())
			{
				try
				{
					if (File.Exists(senderFile) == false)
					{
						throw new Exception("【FTP外部連携】 アップロード元のファイルが見つかりません。 FTPサーバへのファイルアップロードに失敗しました アップロード元:" + senderFile + " アップロード先" + destinationFile, new Exception(Environment.StackTrace));
					}

					if (CheckFileExist(destinationFile))
					{
						switch (uploadType)
						{
							case UploadType.OverWrite:
								Delete(destinationFile);
								break;

							case UploadType.Append:
								throw new Exception(
									"【FTP外部連携】「上書き禁止」アップロード先に同一名ファイルが存在します。 FTPサーバへのファイルアップロードに失敗しました アップロード元:"
									+ senderFile + " アップロード先" + destinationFile);

							case UploadType.Skip:
								FileLogger.WriteInfo(
									"【FTP外部連携】「スキップ」 アップロード先に同一名ファイルが存在するためスキップしました。アップロード元:"
									+ senderFile + " アップロード先" + destinationFile);
								return true;
						}
					}

					// この処理はlow-levelAPIで実装しています。
					// High-levelAPIで実装した下記の処理と機能的に同一です。
					// client.UploadFile(destinationFile, senderFile, FtpExists.Append);
					using (var openStream = client.OpenWrite(destinationFile))
					using (var fileStream = new FileStream(senderFile, FileMode.Open, FileAccess.Read))
					{
						try
						{
							//8kbづつの読み込みを設定
							var buffer = new byte[8 * 1024];
							int readSize = 0;
							while ((readSize = fileStream.Read(buffer, 0, buffer.Length)) > 0)
							{
								openStream.Write(buffer, 0, readSize);
							}
						}
						finally
						{
							//streamをAppendでオープンした場合は明示的にクローズする必要がある
							openStream.Close();
							//サーバから成功/失敗のメッセージを取得する
							client.GetReply();
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("【FTP外部連携】 FTPサーバへのファイルアップロードに失敗しました アップロード元:" + senderFile + " アップロード先" + destinationFile, ex);
				}
			}
			return true;
		}

		/// <summary>
		/// FTPダウンロード
		/// </summary>
		/// <param name="downloadSource">ダウンロード元ファイルのフルパス</param>
		/// <param name="downloadLocation">ダウンロード先のファイルパス(ftp://[ホスト名]は含まない)</param>
		/// <returns>ダウンロードに成功した場合はtrue,失敗した場合はfalse</returns>
		public bool Download(string downloadSource, string downloadLocation)
		{
			using (var client = InitFtpClient())
			{
				try
				{
					// この処理はlow-levelAPIで実装しています。
					// High-levelAPIで実装した下記の処理と機能的に同一です。
					// client.DownloadFile(downloadLocation, downloadSource);
					using (var openStream = client.OpenRead(downloadSource, FtpDataType.Binary))
					using (var fileStream = new FileStream(downloadLocation, FileMode.Create, FileAccess.Write))
					{
						try
						{
							//8kbづつの読み込みを設定
							var buffer = new byte[8 * 1024];
							int readSize = 0;
							while ((readSize = openStream.Read(buffer, 0, buffer.Length)) > 0)
							{
								fileStream.Write(buffer, 0, readSize);
							}
						}
						finally
						{
							//サーバから成功/失敗のメッセージを取得する
							client.GetReply();
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("【FTP外部連携】 FTPサーバからのファイルダウンロードに失敗しました ダウンロード元:" + downloadSource + " ダウンロード先" + downloadLocation, ex);
				}
			}
			return true;
		}


		/// <summary>
		/// FTPサーバ該当ディレクトリ内のファイル一覧を取得
		/// </summary>
		/// <param name="ftpDirectory">取得対象ののディレクトリパス(ftp://[ホスト名]は含まない)</param>
		/// <returns>ファイル名のリスト(ディレクトリにファイルが存在しない場合は空配列、取得に失敗した場合はNULLを返します)</returns>
		public List<string> FileNameListDownload(string ftpDirectory)
		{
			using (var client = InitFtpClient())
			{
				var fileList = new List<string>();
				try
				{
					foreach (FtpListItem item in client.GetListing(ftpDirectory))
					{
						if (item.Type == FtpFileSystemObjectType.File)
						{
							fileList.Add(item.Name);
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("【FTP外部連携】 ファイル一覧の取得に失敗しました。 検索対象ディレクトリ:" + ftpDirectory, ex);
				}
				return fileList;
			}
		}

		/// <summary>
		/// FTPサーバ上の該当ァイルを削除
		/// </summary>
		/// <param name="deleteLocation">削除対象のファイルパス(ftp://[ホスト名]は含まない)</param>
		/// <returns>削除に成功した場合はtrue, 削除に失敗した場合はfalse</returns>
		public bool Delete(string deleteLocation)
		{
			using (var client = InitFtpClient())
			{
				try
				{
					client.DeleteFile(deleteLocation);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception("【FTP外部連携】 ファイルの削除に失敗しました。 削除対象:" + deleteLocation, ex);
				}
			}
		}

		/// <summary>
		/// FTPサーバ上の該当ファイルを移動
		/// </summary>
		/// <param name="originFile">移動前のファイルパス</param>
		/// <param name="destinationFile">移動後のファイルパス</param>
		/// <param name="overWrite">同名ファイルが存在した場合の処理　上書きの場合はtrue, スキップの場合はfalse</param>
		/// <returns>移動に成功した場合はtrue, 移動前のファイルが存在しない場合もしくは通信に失敗した場合はfalse</returns>
		public bool Move(string originFile, string destinationFile, bool overWrite = true)
		{
			using (var client = InitFtpClient())
			{
				try
				{
					var result = client.MoveFile(originFile, destinationFile, (overWrite) ? FtpExists.Overwrite : FtpExists.Skip);

					if (result == false) throw new Exception("【FTP外部連携】 移動前のファイルが存在しません。移動前:" + originFile + " 移動後" + destinationFile, new Exception(Environment.StackTrace));

					return true;
				}
				catch (Exception ex)
				{
					throw new Exception("【FTP外部連携】 ファイルの移動に失敗しました。移動前:" + originFile + " 移動後" + destinationFile, ex);
				}
			}

		}

		/// <summary>
		/// Is connected
		/// </summary>
		/// <returns>True if connected, else false</returns>
		public bool IsConnected()
		{
			var client = InitFtpClient();
			return client.IsConnected;
		}

		/// <summary>
		/// FtpClientの初期設定
		/// </summary>
		/// <returns>初期設定されたFtpClientを返す</returns>
		private FtpClient InitFtpClient()
		{
			var client = new FtpClient();
			client.Host = this.FtpServerHost;
			client.Credentials = new NetworkCredential(this.UserName, this.UserPassword);
			client.DataConnectionType = (this.UseActive) ? FtpDataConnectionType.AutoActive : FtpDataConnectionType.AutoPassive;
			client.SocketKeepAlive = false;

			if (this.Port.HasValue)
			{
				client.Port = this.Port.Value;
			}

			// タイムアウト(デフォルト:15sec)
			client.ConnectTimeout = 15000;
			client.ReadTimeout = 15000;
			client.DataConnectionConnectTimeout = 15000;
			client.DataConnectionReadTimeout = 15000;

			// ポーリング間隔(デフォルト:15sec)
			client.SocketPollInterval = 15000;

			if (this.EnableSsl)
			{
				client.EncryptionMode = FtpEncryptionMode.Explicit;
				client.SslProtocols = SslProtocols.Tls;
				client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);
			}

			try
			{
				client.Connect();
			}
			catch (Exception ex)
			{
				throw new Exception("【FTP外部連携】 FTPサーバとの接続に失敗しました サーバ名:" + this.FtpServerHost, ex);
			}

			return client;
		}

		/// <summary>
		/// SSL認証
		/// </summary>
		/// <param name="client">接続情報</param>
		/// <param name="e">SSL証明書情報</param>
		private void OnValidateCertificate(FtpClient client, FtpSslValidationEventArgs e)
		{
			e.Accept = true;
		}

		/// <summary>
		/// Check file exist
		/// </summary>
		/// <param name="path">Path</param>
		/// <returns>True : File exist. Otherwisre: False</returns>
		public bool CheckFileExist(string path)
		{
			using (var client = InitFtpClient())
			{
				return client.FileExists(path);
			}
		}

		/// <summary>
		/// Clear log
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="limitValue">Limit value</param>
		public void ClearLog(string path, string limitValue)
		{
			using (var client = InitFtpClient())
			{
				int value = 0;
				if ((client.DirectoryExists(path) == false)
					|| (int.TryParse(limitValue, out value) == false)) return;

				var files = client.GetListing(path);
				foreach (FtpListItem file in files)
				{
					if (((DateTime.Now.Month - file.Created.Month) >= value)
						|| ((DateTime.Now.Month - file.Modified.Month) >= value)
						&& (Path.GetExtension(file.Name) == ".log"))
					{
						client.DeleteFile(Path.Combine(path, file.FullName));
					}
				}
			}
		}

		/// <summary>
		/// Get file size
		/// </summary>
		/// <param name="path">Path</param>
		/// <returns>Total size</returns>
		public long GetFileSize(string path)
		{
			using (var client = InitFtpClient())
			{
				return CalculateItemSize(path, client);
			}
		}

		/// <summary>
		/// Calculate item size
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="ftpClient">Ftp client</param>
		/// <returns>Item size</returns>
		private long CalculateItemSize(string path, FtpClient ftpClient)
		{
			long itemSize = 0;
			var items = ftpClient.GetListing(path);
			foreach (FtpListItem item in items)
			{
				itemSize += (item.Type == FtpFileSystemObjectType.Directory)
					? CalculateItemSize(Path.Combine(path, item.Name), ftpClient)
					: item.Size;
			}

			return itemSize;
		}

		/// <summary>FTP(S)サーバのユーザ名</summary>
		public string UserName { get; set; }
		/// <summary>FTP(S)サーバのパスワード</summary>
		public string UserPassword { get; set; }
		/// <summary>FTP(S)サーバのパスワード</summary>
		public string FtpServerHost { get; set; }
		/// <summary>FTP(S)サーバとアクティブモード通信true, パッシブモード通信false</summary>
		public bool UseActive { get; set; }
		/// <summary>SSL証明を利用する場合true, 利用しない場合false</summary>
		public bool EnableSsl { get; set; }
		/// <summary>FTP Port</summary>
		public int? Port { get; set; }
	}
}
