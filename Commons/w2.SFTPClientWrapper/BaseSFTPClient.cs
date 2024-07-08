/*
=========================================================================================================
  Module      : SFTP client base (BaseSFTPClient.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.SFTPClientWrapper
{
	/// <summary>
	/// SFTPClientの基底クラス
	/// </summary>
	public abstract class BaseSFTPClient : ISFTPClient
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sftpSetting">SFTP接続設定</param>
		internal BaseSFTPClient(SFTPSettings sftpSetting)
		{
			this.SftpSettings = sftpSetting;
		}
		#endregion

		/// <summary>
		/// ファイル取得
		/// </summary>
		/// <param name="getServerFilePath">取得元ファイルパス</param>
		/// <param name="saveToClientFilePath">保存先ファイルパス</param>
		/// <returns>結果文言</returns>
		public string GetFile(string getServerFilePath, string saveToClientFilePath)
		{
			return OnGetFile(getServerFilePath, saveToClientFilePath);
		}

		/// <summary>
		/// ファイルGetの抽象メソッド
		/// </summary>
		/// <param name="getServerFilePath">取得元ファイルパス</param>
		/// <param name="saveToClientFilePath">保存先ファイルパス</param>
		/// <returns>結果文言</returns>
		protected abstract string OnGetFile(string getServerFilePath, string saveToClientFilePath);

		/// <summary>
		/// ファイルアップロード
		/// </summary>
		/// <param name="putServerFilePath">配置先ファイルパス</param>
		/// <param name="fromClientFilePath">取得元ファイルパス</param>
		/// <returns>結果文言</returns>
		public string PutFile(string putServerFilePath, string fromClientFilePath)
		{
			return OnPutFile(putServerFilePath, fromClientFilePath);
		}

		/// <summary>
		/// ファイルアップロードの抽象メソッド
		/// </summary>
		/// <param name="putServerFilePath">配置先ファイルパス</param>
		/// <param name="fromClientFilePath">取得元ファイルパス</param>
		/// <returns>結果文言</returns>
		protected abstract string OnPutFile(string putServerFilePath, string fromClientFilePath);

		/// <summary>
		/// ファイルのリネーム
		/// </summary>
		/// <param name="fromServerFilePath">リネーム元ファイルパス</param>
		/// <param name="toServerFilePath">リネーム後ファイルパス</param>
		/// <returns>結果文言</returns>
		public string RenameServerFile(string fromServerFilePath, string toServerFilePath)
		{
			return OnRenameServerFile(fromServerFilePath, toServerFilePath);
		}

		/// <summary>
		/// ファイルのリネームの抽象メソッド
		/// </summary>
		/// <param name="fromServerFilePath">リネーム元ファイルパス</param>
		/// <param name="toServerFilePath">リネーム後ファイルパス</param>
		/// <returns>結果文言</returns>
		public abstract string OnRenameServerFile(string fromServerFilePath, string toServerFilePath);

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="serverFile">チェック対象のファイル</param>
		/// <returns>ファイルが存在しているか</returns>
		public bool IsExistsServerFile(string serverFile)
		{
			return OnExistsServerFile(serverFile);
		}

		/// <summary>
		/// ファイル存在チェックの抽象メソッド
		/// </summary>
		/// <param name="serverFile">チェック対象のファイル</param>
		/// <returns>ファイルが存在しているか</returns>
		public abstract bool OnExistsServerFile(string serverFile);

		/// <summary>
		/// ホストキーの取得
		/// </summary>
		/// <returns>ホストキー</returns>
		public string ScanFingerprint()
		{
			return OnScanFingerprint();
		}

		/// <summary>
		/// ホストキーの取得の抽象メソッド
		/// </summary>
		/// <returns>ホストキー</returns>
		public abstract string OnScanFingerprint();

		/// <summary>
		/// 該当ディレクトリ内のファイル名一覧を取得
		/// </summary>
		/// <param name="dirPath">探索対象のディレクトリパス</param>
		/// <returns>ディレクトリ内のファイル一覧</returns>
		public List<string> ListFileName(string dirPath)
		{
			return OnListFileName(dirPath);
		}

		/// <summary>
		/// 該当ディレクトリ内のファイル名一覧を取得の抽象メソッド
		/// </summary>
		/// <param name="dirPath">探索対象のディレクトリパス</param>
		/// <returns>ディレクトリ内のファイル一覧</returns>
		public abstract List<string> OnListFileName(string dirPath);

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="serverFilePath">削除対象のファイルパス</param>
		/// <returns>結果文言</returns>
		public string RemoveServerFile(string serverFilePath)
		{
			return OnRemoveServerFile(serverFilePath);
		}

		/// <summary>
		/// ファイル削除の抽象メソッド
		/// </summary>
		/// <param name="serverFilePath">削除対象のファイルパス</param>
		/// <returns>結果文言</returns>
		public abstract string OnRemoveServerFile(string serverFilePath);

		/// <summary>SFTP設定</summary>
		protected SFTPSettings SftpSettings { get; set; }
	}
}
