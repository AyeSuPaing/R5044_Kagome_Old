/*
=========================================================================================================
  Module      : 連携処理ターゲット情報クラス(ExecuteTarget.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace w2.ExternalAPI.Common
{
	/// <summary>
	/// 連携種別（インポートorエクスポート）列挙体
	/// </summary>
	public enum APIType { Import = 0, Export = 1 }

	/// <summary>
	///	連携処理ターゲット情報クラス
	/// </summary>
	/// <remarks>
	/// 連携処理を実行する際に必要な情報を持つクラス
	/// </remarks>
	public class ExecuteTarget
	{
		#region メンバ変数
		private readonly string m_apiId = "";
		private readonly FileInfo m_targetFilePath;
		private readonly FileInfo m_workFilePath;
		private readonly APIType m_apiType;
		private readonly string m_fileType;
		private readonly DateTime m_executedTime;
		private readonly FileInfo m_backupFilePath;
		private readonly FileInfo m_successFilePath;
		private readonly FileInfo m_errorFilePath;
		private readonly BatchProperties m_properties;
		private readonly bool m_writeLog;
		private readonly bool m_useFtpDownload;
		#endregion

		#region コンストラクタ
		protected ExecuteTarget()
		{
			m_executedTime = DateTime.Now;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiId">連携処理を識別するためのID</param>
		/// <param name="targetFilePath">
		/// 連携処理の対象となるファイルのパス
		/// インポートの場合はインポート対象となるファイルのパス
		/// エクスポートの場合は出力先のファイルのパス
		/// </param>
		/// <param name="apiType">連携種別</param>
		/// <param name="fileType">
		/// 連携ファイルの形式、現在は<c>csv</c>のみ指定可能
		/// </param>
		///<param name="workFilePath">
		/// 連携処理のための一時的ファイルのパス
		/// インポートの場合はtargetFilePathからworkFilePathにファイルをコピーして
		/// workFilePathに対して処理を行う
		/// エクスポートの場合はworkFilePathに出力してからtargetFilePathに最終的にコピーする
		/// </param>
		/// <param name="successDirPath">成功したファイルをおくディレクトリパス</param>
		/// <param name="errorDirPath">エラーしたファイルをおくディレクトリパス</param>
		/// <param name="backupDirPath">バックアップをおくディレクトリパス</param>
		/// <param name="props">コマンドライン引数で指定するプロパティ。連携プラグインで利用できる</param>
		/// <param name="writeLog">APIログを出力するか？（true：出力する、 false：出力しない）</param>
		/// <param name="useFtpDownload">FTP連携を利用するか？（true：利用する、 false：利用しない）</param>
		public ExecuteTarget(
			string apiId,
			string targetFilePath,
			APIType apiType,
			string fileType,
			string workFilePath = null,
			string successDirPath = null,
			string errorDirPath = null,
			string backupDirPath = null,
			string props = null,
			bool writeLog = true,
			bool useFtpDownload = false)
			: this()
		{
			m_apiId = apiId;
			m_targetFilePath = new FileInfo(targetFilePath);
			m_apiType = apiType;
			m_fileType = fileType;

			// プロパティの取得
			if (!string.IsNullOrEmpty(props))
			{
				m_properties = new BatchProperties(props);
			}
			else
			{
				m_properties = null;
			}

			// 作業ファイルパス
			m_workFilePath = GetFilePathWithParentAssurance("Work", workFilePath, m_targetFilePath.Name);

			// バックアップファイルパス
			string fileNameWithDateTime = m_executedTime.ToString("yyyyMMddHHmmss") + "_" + m_targetFilePath.Name;
			m_backupFilePath = GetFilePathWithParentAssurance("Backup", backupDirPath, fileNameWithDateTime);

			// インポート時の場合、サクセス・エラーファイルパスを生成
			if (apiType == APIType.Import)
			{
				m_successFilePath = GetFilePathWithParentAssurance("Success", successDirPath, fileNameWithDateTime);
				m_errorFilePath = GetFilePathWithParentAssurance("Error", errorDirPath, fileNameWithDateTime);
			}

			// APIログを出力するか？
			m_writeLog = writeLog;

			// FTPダウンロード機能を利用するかどうか？利用する場合はtrue,しない場合はfalse
			m_useFtpDownload = useFtpDownload;
		}
		/// <summary>
		/// コンストラクタ（後方互換性のために残してある）
		/// </summary>
		/// <param name="apiId">連携処理を識別するためのID</param>
		/// <param name="targetFilePath">
		/// 連携処理の対象となるファイルのパス
		/// インポートの場合はインポート対象となるファイルのパス
		/// エクスポートの場合は出力先のファイルのパス
		/// </param>
		///<param name="workFilePath">
		/// 連携処理のための一時的ファイルのパス
		/// インポートの場合はtargetFilePathからworkFilePathにファイルをコピーして
		/// workFilePathに対して処理を行う
		/// エクスポートの場合はworkFilePathに出力してからtargetFilePathに最終的にコピーする
		/// </param>
		/// <param name="apiType">連携種別</param>
		/// <param name="fileType">
		/// 連携ファイルの形式、現在は<c>csv</c>のみ指定可能
		/// </param>
		public ExecuteTarget(
			string apiId,
			string targetFilePath,
			string workFilePath,
			APIType apiType,
			string fileType)
			: this()
		{
			m_apiId = apiId;
			m_targetFilePath = new FileInfo(targetFilePath);
			m_workFilePath = new FileInfo(workFilePath);
			m_apiType = apiType;
			m_fileType = fileType;

			// バックアップファイルパス
			string fileNameWithDateTime = m_executedTime.ToString("yyyyMMddHHmmss") + "_" + m_targetFilePath.Name;
			m_backupFilePath = GetFilePathWithParentAssurance("Backup", null, fileNameWithDateTime);

			// インポート時の場合、サクセス・エラーファイルパスを生成
			if (apiType == APIType.Import)
			{
				m_successFilePath = GetFilePathWithParentAssurance("Success", null, fileNameWithDateTime);
				m_errorFilePath = GetFilePathWithParentAssurance("Error", null, fileNameWithDateTime);
			}

			// コマンドライン引数指定が旧方式の場合、常にAPIログを出力する
			m_writeLog = true;

			// コマンドライン引数指定が旧方式の場合、FTPダウンロード機能は無効にする
			m_useFtpDownload = false;
		}

		/// <summary>
		/// 指定した親ディレクトリ名（存在保障済）の下にファイルを置く、ファイルパスを取得する
		/// </summary>
		/// <param name="parentDirName"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private FileInfo GetFilePathWithParentAssurance(string parentDirName, string path, string fileName)
		{
			DirectoryInfo res = (!string.IsNullOrEmpty(path))
							? new DirectoryInfo(path)
							: new DirectoryInfo(Path.Combine(m_targetFilePath.Directory.FullName, parentDirName));
			if (!res.Exists) res.Create();
			return new FileInfo(Path.Combine(res.FullName, fileName));
		}

		#endregion

		#region +ToString データを出力する
		/// <summary>
		/// データを出力するToString
		/// </summary>
		/// <returns></returns>
		public new string ToString()
		{
			return string.Format("ApiID:'{0}', ApiType:'{1}', FileType:'{2}', Properties:'{3}'", this.APIID, this.ApiType, this.FileType, this.Properties);
		}
		#endregion

		#region プロパティ

		/// <summary>APIIDプロパティ</summary>
		public string APIID { get { return m_apiId; } }
		/// <summary>対象ファイルパスプロパティ</summary>
		public string TargetFilePath { get { return m_targetFilePath.FullName; } }
		/// <summary>一時ファイルパスプロパティ</summary>
		public string WorkFilePath { get { return m_workFilePath.FullName; } }
		/// <summary>連携種別プロパティ</summary>
		public APIType ApiType { get { return m_apiType; } }
		/// <summary>連携ファイル形式プロパティ</summary>
		public string FileType { get { return m_fileType; } }
		/// <summary>実行した時間</summary>
		public DateTime ExecutedTime { get { return m_executedTime; } }
		/// <summary>バックアップファイルパス</summary>
		public string BackupPath { get { return m_backupFilePath.FullName; } }
		/// <summary>サクセスファイルパス</summary>
		public string SuccessPath { get { return m_successFilePath.FullName; } }
		/// <summary>エラーファイルパス</summary>
		public string ErrorPath { get { return m_errorFilePath.FullName; } }
		/// <summary>実行時指定プロパティ</summary>
		public BatchProperties Properties { get { return m_properties; } }
		/// <summary>APIログ出力プロパティ</summary>
		public bool WriteLog { get { return m_writeLog; } }
		/// <summary>FTPダウンロードの使用プロパティ</summary>
		public bool UseFtpDownload { get { return m_useFtpDownload; } }

		#endregion
	}
}
