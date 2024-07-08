/*
=========================================================================================================
  Module      : バッチ処理クラス(BatchProc.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using w2.App.Common;
using w2.Domain.GlobalSMS;

namespace UpdateSMSState
{
	/// <summary>
	/// バッチ処理クラス
	/// </summary>
	public class BatchProc
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Execute()
		{
			// 古い不要なものを削除
			CleaningStatusData();

			// SMSのタイプによって処理振り分け
			if (Constants.GLOBAL_SMS_TYPE == Constants.GLOBAL_SMS_TYPE_MACROKIOSK)
			{
				ExecMacroKiosk();
			}

			// 一定期間超過しているものはエラーポイントを加算して削除
			IncrementSmsErrorPoint();
		}

		/// <summary>
		/// 古い不要なステータスをクリーニング
		/// </summary>
		private void CleaningStatusData()
		{
			var sv = new GlobalSMSService();
			sv.CleaningStatusData(Constants.GLOBAL_SMS_STATUS_CLEAING_DAYS);
		}

		/// <summary>
		/// エラーポイントインクリメント
		/// </summary>
		private void IncrementSmsErrorPoint()
		{
			var sv = new GlobalSMSService();
			sv.IncrementSmsErrorPointTimeOver(Constants.GLOBAL_SMS_STATUS_TIME_OVER_HOURS, Constants.FLG_LASTCHANGED_BATCH);
		}

		/// <summary>
		/// マクロキオスク処理実行
		/// </summary>
		private void ExecMacroKiosk()
		{
			// 取込対象ディレクトリ確認
			if (Directory.Exists(Constants.MACROKIOSK_DN_OUTPUT_DIR_PATH) == false)
			{
				// 無ければ作成
				Directory.CreateDirectory(Constants.MACROKIOSK_DN_OUTPUT_DIR_PATH);
			}

			// 作業、完了ディレクトリ確認
			var workDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "work");
			var compRootDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "comp");
			var compDirPath = Path.Combine(compRootDirPath, DateTime.Now.ToString("yyyyMMdd"));

			if (Directory.Exists(workDirPath) == false)
			{
				Directory.CreateDirectory(workDirPath);
			}

			if (Directory.Exists(compRootDirPath) == false)
			{
				Directory.CreateDirectory(compRootDirPath);
			}

			if (Directory.Exists(compDirPath) == false)
			{
				Directory.CreateDirectory(compDirPath);
			}

			// 取込対象ディレクトリから作業用へコピー
			foreach (var file in Directory.GetFiles(Constants.MACROKIOSK_DN_OUTPUT_DIR_PATH))
			{
				var workFilePath = Path.Combine(workDirPath, new FileInfo(file).Name);
				File.Move(file, workFilePath);
			}

			// 作業用ディレクトリ内のファイルをもとにステータス更新、更新終わったものは完了ディレクトリへ
			var sv = new GlobalSMSService();
			foreach (var file in Directory.GetFiles(workDirPath))
			{
				// ロード
				var data = MacroKioskDNFileData.Load(file);

				// ロード出来たもののみ更新
				if (data != null)
				{
					// ステータスがDELIVEREDの場合は完了とし、ステータスから消す
					if (data.Status == GlobalSMSStatusModel.SMS_STATUS_DELIVERED)
					{
						// 残しても以降使うこともなくパフォーマンスも考慮し、DNはテキストとして残っているので消す
						sv.DeleteState(data.MsgId);
					}
				}

				// 完了ディレクトリへ
				File.Move(file, Path.Combine(compDirPath, new FileInfo(file).Name));
			}
		}
	}
}
