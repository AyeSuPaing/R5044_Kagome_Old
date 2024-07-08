/*
=========================================================================================================
  Module      : エクスポート処理のための抽象クラス(ExportApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.ExternalAPI.Common.Export
{
	/// <summary>
	/// エクスポート基底クラス
	/// </summary>
	public abstract class ExportApiBase : ApiBase, IDisposable
	{
		#region メンバ変数
		protected readonly ApiExportCommandBuilder m_commandBuilder;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="executeTarget">実行対象の連携処理の情報を持つExecuteTargetクラス</param>
		/// <param name="commandBuilder">エクスポートコマンドビルダ基底クラス</param>
		protected ExportApiBase(ExecuteTarget executeTarget, ApiExportCommandBuilder commandBuilder) : base(executeTarget)
		{
			m_commandBuilder = commandBuilder;
			m_commandBuilder.Properties = executeTarget.Properties;
		}
		#endregion

		#region #End 終了処理の実装
		/// <summary>
		/// 終了処理の実装
		/// </summary>
		/// <remarks>保持しているエクスポートコマンドビルダクラスのDisposeを実行する</remarks>
		protected override void End()
		{
			m_commandBuilder.Dispose();
		}
		#endregion
	}
}
