/*
=========================================================================================================
  Module      : Csvファイル用エクスポート処理(CsvFileExportApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.IO;
using w2.ExternalAPI.Common.Extension;
using w2.ExternalAPI.Common.FrameWork.File;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Export
{
	/// <summary>
	/// CSVエクスポートクラス
	/// </summary>
	public class CsvFileExportApi : ExportApiBase
	{
		#region メンバ変数
		private readonly BackUp m_backUp;
		private readonly CsvSetting m_csvSetting;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="executeTarget">実行対象の連携処理の情報を持つExecuteTargetクラス</param>
		/// <param name="apiExportCommandBuilder">エクスポートコマンドビルダ基底クラス</param>
		/// <param name="csvSetting">CSV情報構造体 </param>
		public CsvFileExportApi(ExecuteTarget executeTarget, ApiExportCommandBuilder apiExportCommandBuilder, CsvSetting csvSetting)
			:base(executeTarget, apiExportCommandBuilder)
		{
			m_backUp = new FileBackUp(executeTarget);
			m_csvSetting = csvSetting;
		}
		#endregion

		#region #PreExecute 実行前処理の実装
		/// <summary>
		/// 実行前処理の実装
		/// </summary>
		protected override void PreExecute()
		{
			// コマンドビルダー実行前処理
			m_commandBuilder.PreDo();
		}
		#endregion

		#region #Execute 出力処理
		/// <summary>出力処理</summary>
		protected override bool Execute()
		{
			bool result = true;

			DataTableReader reader = new DataTableReader(m_commandBuilder.GetDataTable());

			// ワークファイル出力
			using (CsvWriter writer = CsvStreamBuilder.BuildCsvWriter(m_executeTarget.WorkFilePath, m_csvSetting))
			{
				int lineNumber = 0;
				while (reader.Read())
				{
					object[] data = null;

					try
					{
						lineNumber++;
						data = m_commandBuilder.Do(reader);

						// ファイル書き込み情報ログ
						ApiLogger.Write(LogLevel.info,
							string.Format("CSVファイルExport{0}行目",
							lineNumber.ToString())
							, string.Format("読み込みデータ内容:'{0}'", (data == null) ? "Null" :string.Join(",", data)));

						if (data != null)
						{
							writer.WriteLine(data);
							writer.Flush();
						}

                        m_commandBuilder.PostExport(data);
					}
					catch (Exception ex)
					{
						// エラーをCatchしログを書く
						// 上位にはthrowしない
						ApiLogger.Write(LogLevel.error, ex.Message,string.Format("書き込みデータ内容:'{0}'", (data == null) ? "Null" :string.Join(",", data)), ex);

						result = false;
					}
				}
			}

			return result;
		}
		#endregion

		#region #PostExecute 出力後処理
		/// <summary>出力後処理</summary>
		/// <param name="resultExecute">Execute実行結果（成功：true、失敗：false）</param>
		protected override void PostExecute(bool resultExecute)
		{
			// コマンドビルダー実行後処理
			m_commandBuilder.PostDo();

			// ワークファイルをターゲットに移動
			new System.IO.FileInfo(base.m_executeTarget.TargetFilePath).ForceDelete();
			File.Copy(m_executeTarget.WorkFilePath, m_executeTarget.TargetFilePath);

			// バックアップ
			m_backUp.Do();

			// ワークファイルは削除
			new FileInfo(base.m_executeTarget.WorkFilePath).ForceDelete();

			// コマンドビルダー　ターゲットファイル生成が完了した際の処理
			m_commandBuilder.EndExport(m_executeTarget.TargetFilePath);
		}
		#endregion
	}
}