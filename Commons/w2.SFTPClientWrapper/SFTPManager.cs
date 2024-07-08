/*
=========================================================================================================
  Module      : SFTP マネージャ (SFTPManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.SFTPClientWrapper.Conc;

namespace w2.SFTPClientWrapper
{
	/// <summary>
	/// SFTP マネージャ
	/// </summary>
	public class SFTPManager
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SFTP設定</param>
		public SFTPManager(SFTPSettings settings)
		{
			this.SftpSettings = settings;
			// ホスト名からホストキーを設定
			this.SftpSettings.HostKey = new WinScpClient(this.SftpSettings).OnScanFingerprint();
		}

		/// <summary>
		/// SFTPクライアント作成
		/// </summary>
		/// <param name="settings">SFTP接続情報</param>
		/// <returns>SFTPクライアント</returns>
		public ISFTPClient CreateSFTPClient()
		{
			return new WinScpClient(this.SftpSettings);
		}

		/// <summary>SFTP設定</summary>
		private SFTPSettings SftpSettings { get; set; }
	}
}
