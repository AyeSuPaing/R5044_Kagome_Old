/*
=========================================================================================================
  Module      : ISFTP client (ISFTPClient.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
namespace w2.SFTPClientWrapper
{
	/// <summary>
	/// SFTPClientのインターフェース
	/// </summary>
	public interface ISFTPClient
	{
		/// <summary>
		/// ファイルGet
		/// </summary>
		/// <param name="getServerFilePath">取得元ファイルパス</param>
		/// <param name="saveToClientFilePath">保存先ファイルパス</param>
		/// <returns>結果文言</returns>
		string GetFile(string getServerFilePath, string saveToClientFilePath);

		/// <summary>
		/// ファイルPut
		/// </summary>
		/// <param name="putServerFilePath">配置先ファイルパス</param>
		/// <param name="fromClientFilePath">取得元ファイルパス</param>
		/// <returns>結果文言</returns>
		string PutFile(string putServerFilePath, string fromClientFilePath);

		/// <summary>
		/// サーバーファイルリネーム
		/// </summary>
		/// <param name="fromServerFilePath">リネーム元ファイルパス</param>
		/// <param name="fromServerFileName">リネーム後ファイルパス</param>
		/// <returns>結果文言</returns>
		string RenameServerFile(string fromServerFilePath, string toServerFilePath);

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="serverFile">チェック対象のファイル</param>
		/// <returns>ファイルが存在しているか</returns>
		bool IsExistsServerFile(string serverFile);

		/// <summary>
		/// ホストキーの取得
		/// </summary>
		/// <returns>ホストキー</returns>
		string ScanFingerprint();

		/// <summary>
		/// 該当ディレクトリ内のファイル一覧を取得
		/// </summary>
		/// <param name="dirPath">探索対象のディレクトリパス</param>
		/// <returns>ディレクトリ内のファイル一覧</returns>
		List<string> ListFileName(string dirPath);

		/// <summary>
		/// サーバファイルの削除
		/// </summary>
		/// <param name="serverFilePath">削除対象のファイルパス</param>
		/// <returns>結果文言</returns>
		string RemoveServerFile(string serverFilePath);
	}
}
