/*
=========================================================================================================
  Module      : インポートコマンドビルダー抽象クラス(ApiImportCommandBuilder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Import
{
	/// <summary>
	///	インポートコマンドビルダー抽象クラス
	/// </summary>
	/// <remarks>
	/// インポートしたファイルの1レコード単位で行いたい処理がある場合、
	/// このクラスを継承して処理を実装すること。
	/// </remarks>
	public abstract class ApiImportCommandBuilder : IDisposable
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected ApiImportCommandBuilder()
		{
			Init();
		}
		#endregion

		#region #Init 初期処理
		/// <summary>
		/// 初期処理
		/// </summary>
		protected virtual void Init()
		{
			// 起動ログ
			ApiLogger.Write(LogLevel.info, "コマンドビルダー開始:" + GetType().Name, "");
		}
		#endregion 

		#region #PreImport Import実行前処理
		/// <summary>
		/// 実行前処理
		/// </summary>
		/// <param name="apiEntry"></param>
		protected virtual void PreImport(ApiEntry apiEntry)
		{

		}	
		#endregion

		#region #Importインポート処理実行
		/// <summary>
		/// インポート実行処理
		/// </summary>
		/// <param name="apiEntry"></param>
		protected abstract void Import(ApiEntry apiEntry);
		#endregion

		#region #PostImport Import実行後処理
		/// <summary>
		/// インポート実行後処理
		/// </summary>
		/// <param name="apiEntry"></param>
		protected virtual void PostImport(ApiEntry apiEntry)
		{

		}
		#endregion 

		#region #後処理
		/// <summary>
		/// 後処理
		/// </summary>
		protected virtual void End()
		{
			// 終了ログ
			ApiLogger.Write(LogLevel.info, "コマンドビルダー終了:" + GetType().Name, "");
		}
		#endregion

		#region #PreDo 実行前処理
		/// <summary>
		/// 実行前処理
		/// </summary>
		public virtual void PreDo() { }
		#endregion

		#region +Do 実行する
		/// <summary> 実行する </summary>
		/// <param name="apiEntry">実行時に利用するインポート情報構造体</param>
		/// <remarks>
		/// PreImport→Import→PostImportの順に処理を実行
		/// </remarks>
		public void Do(ApiEntry apiEntry)
		{
			PreImport(apiEntry);
			Import(apiEntry);
			PostImport(apiEntry);
		}
		#endregion

		#region #PostDo 実行後処理
		/// <summary>
		/// 実行後処理
		/// </summary>
		public virtual void PostDo() { }
		#endregion

		#region +Dispose
		/// <summary>
		/// Disposeの実装
		/// </summary>
		public void Dispose()
		{
			End();
		}
		#endregion

		#region #ParepareImportFile インポート対象ファイルの準備処理
		/// <summary>
		/// インポート対象ファイルの準備処理
		/// </summary>
		/// <param name="importFilepath">準備予定のファイルパス</param>
		public virtual void ParepareImportFile(string importFilepath) { }
		#endregion

		/// <summary>コマンドライン引数指定のプロパティ </summary>
		public BatchProperties Properties { get; set; }
	}
}
