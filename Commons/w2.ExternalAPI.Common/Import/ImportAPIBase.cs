/*
=========================================================================================================
  Module      : インポート処理のための抽象クラス(ImportApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.ExternalAPI.Common.Import
{
	/// <summary>
	///	インポート処理のための抽象クラス
	/// </summary>
	/// <remarks>
	/// ファイル単位でインポートを実行する処理をこのクラスを継承して実装すること
	/// </remarks>
	public abstract class ImportApiBase : ApiBase, IDisposable
	{
		#region メンバ変数
		protected readonly ApiImportCommandBuilder m_commandBuilder;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="executeTarget">ターゲット情報</param>
		///<param name="ApiImportCommandBuilder">インポート行単位で実行したいコマンドビルダー</param>
		protected ImportApiBase(ExecuteTarget executeTarget, ApiImportCommandBuilder commandBuilder)
			: base(executeTarget)
		{
			m_commandBuilder = commandBuilder;
			m_commandBuilder.Properties = executeTarget.Properties;
		}
		#endregion

		#region #ParepareImportFile インポート対象ファイルの準備処理
		/// <summary>
		/// インポート対象ファイルの準備処理
		/// </summary>
		/// <remarks>FTPダウンロードなどインポート対象ファイルの事前準備が必要となる際の処理を実行</remarks>
		/// <param name="importFilepath">準備予定のファイルパス</param>
		public void ParepareImportFile(string importFilepath)
		{
			m_commandBuilder.ParepareImportFile(importFilepath);
		}
		#endregion

		#region #End 終了処理の実装
		/// <summary>
		/// 終了処理の実装
		/// </summary>
		/// <remarks>保持しているインポートコマンドビルダクラスのDisposeを実行する</remarks>
		protected override void End()
		{
			m_commandBuilder.Dispose();
		}
		#endregion
	}
}
