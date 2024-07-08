/*
=========================================================================================================
  Module      : WinScp client (WinScpClient.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Text;
using System.Collections.Generic;
using WinSCP;

namespace w2.SFTPClientWrapper.Conc
{
	/// <summary>
	/// WINSCPを利用したSFTPClientの実装
	/// </summary>
	internal class WinScpClient : BaseSFTPClient
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sftpSetting">SFTP設定</param>
		internal WinScpClient(SFTPSettings sftpSetting)
			: base(sftpSetting)
		{
			this.SessionOption = new SessionOptions
			{
				Protocol = Protocol.Sftp,
				HostName = this.SftpSettings.HostName,
				PortNumber = this.SftpSettings.PortNumber,
				UserName = this.SftpSettings.LoginUser,
				Password = this.SftpSettings.LoginPassword,
				SshHostKeyFingerprint = this.SftpSettings.HostKey,
				SshPrivateKeyPath = this.SftpSettings.PrivateKeyFilePath,
				PrivateKeyPassphrase = this.SftpSettings.KeyPassphrase
			};

		}
		#endregion

		#region +OnGetFile OnGetFileの実装
		/// <summary>
		/// OnGetFileの実装
		/// </summary>
		/// <param name="getServerFilePath">取得元ファイルパス</param>
		/// <param name="saveToClientFilePath">保存先ファイルパス</param>
		/// <returns>結果文言</returns>
		protected override string OnGetFile(string getServerFilePath, string saveToClientFilePath)
		{
			var sb = new StringBuilder();

			using (var session = new Session())
			{
				// Connect
				session.Open(this.SessionOption);
				var transferResult = session.GetFiles(getServerFilePath, saveToClientFilePath, false, null);

				// Throw on any error
				transferResult.Check();

				// Print results
				transferResult.Transfers.ToList().ForEach(
					transfer => sb.AppendLine(string.Format("DownLoad of {0} succeeded", transfer.FileName)));
			}

			return sb.ToString();
		}
		#endregion

		#region OnPutFileの実装
		/// <summary>
		/// OnPutFileの実装
		/// </summary>
		/// <param name="putServerFilePath">配置先ファイルパス</param>
		/// <param name="fromClientFilePath">取得元ファイルパス</param>
		/// <returns>結果文言</returns>
		protected override string OnPutFile(string putServerFilePath, string fromClientFilePath)
		{
			var sb = new StringBuilder();

			using (var session = new Session())
			{
				session.Open(this.SessionOption);
				var transferResult = session.PutFiles(fromClientFilePath, putServerFilePath, false, null);

				// Throw on any error
				transferResult.Check();

				transferResult.Transfers.ToList().ForEach(
					transfer => sb.AppendLine(string.Format("Upload of {0} succeeded", transfer.FileName)));
			}

			return sb.ToString();
		}
		#endregion

		#region OnRenameServerFileの実装
		/// <summary>
		/// OnRenameServerFileの実装
		/// </summary>
		/// <param name="fromServerFilePath">リネーム元ファイルパス</param>
		/// <param name="toServerFilePath">リネーム後ファイルパス</param>
		/// <returns>結果文言</returns>
		public override string OnRenameServerFile(string fromServerFilePath, string toServerFilePath)
		{
			var sb = new StringBuilder();

			using (var session = new Session())
			{
				// Connect
				session.Open(this.SessionOption);

				session.MoveFile(fromServerFilePath, toServerFilePath);

				sb.AppendLine(string.Format("Rename of {0} succeeded", toServerFilePath));

			}

			return sb.ToString();
		}
		#endregion

		#region OnExistsServerFileの実装
		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="serverFile">チェック対象のファイル</param>
		/// <returns>存在する:True 存在しない:False</returns>
		public override bool OnExistsServerFile(string serverFile)
		{
			using (var session = new Session())
			{
				// Connect
				session.Open(this.SessionOption);

				var transferOptions = new TransferOptions();
				transferOptions.TransferMode = TransferMode.Binary;

				// Check exists file
				return session.FileExists(serverFile);
			}
		}
		#endregion

		#region OnScanFingerprintの実装
		/// <summary>
		/// OnScanFingerprintの実装
		/// </summary>
		/// <returns>ホストキー</returns>
		public override string OnScanFingerprint()
		{
			var hostKey = string.Empty;
			using (var session = new Session())
			{
				hostKey = session.ScanFingerprint(this.SessionOption, "SHA256");
			}
			return hostKey;
		}
		#endregion

		#region OnListFileNameの実装
		/// <summary>
		/// OnListFileNameの実装
		/// </summary>
		/// <param name="dirPath">探索対象のディレクトリパス</param>
		/// <returns>ディレクトリ内のファイル一覧</returns>
		public override List<string> OnListFileName(string dirPath)
		{
			using (var session = new Session())
			{
				session.Open(this.SessionOption);
				var listFileName = session.ListDirectory(dirPath).Files.Where(f => (f.IsDirectory == false)).Select(f => f.Name).ToList();
				return listFileName;
			}
		}
		#endregion

		#region OnRemoveServerFileの実装
		/// <summary>
		/// OnRemoveServerFileの実装
		/// </summary>
		/// <param name="serverFilePath">削除対象のファイルパス</param>
		/// <returns>結果文言</returns>
		public override string OnRemoveServerFile(string serverFilePath)
		{
			var sb = new StringBuilder();

			using (var session = new Session())
			{
				// Connect
				session.Open(this.SessionOption);

				session.RemoveFiles(serverFilePath);

				sb.AppendLine(string.Format("Remove of {0} succeeded", serverFilePath));

			}
			return sb.ToString();
		}
		#endregion

		/// <summary>SFTPコネクション設定</summary>
		private SessionOptions SessionOption { get; set; }
	}
}
