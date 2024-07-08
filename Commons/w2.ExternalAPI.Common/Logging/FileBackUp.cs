/*
=========================================================================================================
  Module      : CSVファイルのバックアップクラス(CsvFileBackUp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.IO;

namespace w2.ExternalAPI.Common.Logging
{
	/// <summary>
	///	CSVファイルのバックアップクラス
	/// </summary>
	class FileBackUp : BackUp
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="executeTarget">パック対象ファイルパス等をもつターゲット情報</param>
		public FileBackUp(ExecuteTarget executeTarget) : base(executeTarget)
		{
		}
		#endregion

		#region #Execute バックアップ実行処理の実装
		/// <summary>
		/// バックアップ実行処理の実装
		/// </summary>
		/// <param name="backupPath"></param>
		protected override void Execute(string backupPath)
		{
			// ファイルコピー
			File.Copy(base.ExecuteTarget.TargetFilePath, backupPath);
		}
		#endregion 

	}
}
