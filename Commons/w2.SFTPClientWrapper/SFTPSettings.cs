/*
=========================================================================================================
  Module      : SFTP用の設定クラス (SFTPSettings.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.SFTPClientWrapper
{
	/// <summary>
	/// SFTP用の設定クラス
	/// </summary>
	public class SFTPSettings
	{
		/// <summary>ホスト名</summary>
		public string HostName { get; set; }
		/// <summary>ポート番号</summary>
		public int PortNumber { get; set; }
		/// <summary>ログインユーザ</summary>
		public string LoginUser { get; set; }
		/// <summary>ログインパスワード</summary>
		public string LoginPassword { get; set; }
		/// <summary>ホストキー</summary>
		public string HostKey { get; set; }
		/// <summary>秘密鍵のファイルパス</summary>
		public string PrivateKeyFilePath { get; set; }
		/// <summary>パスフレーズ</summary>
		public string KeyPassphrase { get; set; }
	}
}
