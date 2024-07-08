/*
=========================================================================================================
  Module      : 最終同期日時記録クラス (LastUpdatedCheckpointRecorder.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using w2.Common.Logger;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// 最終同期日時記録クラス
	/// </summary>
	public class LastUpdatedCheckpointRecorder
	{
		/// <summary>最終同期日時を記録するファイルのあるディレクトリのパス</summary>
		private static readonly string s_directory =
			Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"Flaps\LastUpdatedTimeStamp");
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="replicationData">同期するデータ</param>
		public LastUpdatedCheckpointRecorder(string replicationData)
		{
			this.ReplicationData = replicationData;
			CreateDirectoryIfNecessary();
		}

		/// <summary>
		/// ディレクトリ作成
		/// </summary>
		private void CreateDirectoryIfNecessary()
		{
			if (Directory.Exists(s_directory) == false)
			{
				Directory.CreateDirectory(s_directory);
			}
		}
		
		/// <summary>
		/// FLAPS最終同期日時を取得する。
		/// </summary>
		/// <returns>最終同期日時</returns>
		/// <remark>管理画面から最終同期日時を取得できるようにアクセス修飾子はあえてpublicとする</remark>
		public string GetLastUpdatedDateTime()
		{
			// 最終同期日時を記録してあるファイル名を取得する
			var initializedLastUpdatedDateTime = DateTime.Parse(Constants.FLG_FLAPS_DEFAULT_CHECKPOINT);
			var productsLastUpdated = FindTimestampFile();
			if (string.IsNullOrEmpty(productsLastUpdated))
			{
				CreateTimestampFile(initializedLastUpdatedDateTime);
				return initializedLastUpdatedDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			}

			// 最終同期日時をファイル名から取り出す
			var timestamp = Path.GetFileName(productsLastUpdated).Replace("_" + this.ReplicationData, "");
			var lastUpdated = initializedLastUpdatedDateTime;
			if (DateTime.TryParseExact(timestamp, "yyyyMMdd-HHmm", null, DateTimeStyles.None, out lastUpdated) == false)
			{
				var msg = "取得した最終同期日時が無効です。'yyyyMMdd-HHmm_XXXX'の形式で入力してください。";
				Console.WriteLine(msg);
				FileLogger.WriteError(msg);
			}
			var checkpoint = lastUpdated.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var checkpointMessage = string.Format("Capture products that were changed after {0}", checkpoint);
			Console.WriteLine(checkpointMessage);
			return checkpoint;
		}

		/// <summary>
		/// 最終同期日時を記録してあるファイル名を取得する
		/// </summary>
		/// <returns>最終同期日時を記録してあるファイル名</returns>
		private string FindTimestampFile()
		{
			var fileEntries = Directory.GetFiles(s_directory);
			var productsLastUpdated = fileEntries.FirstOrDefault(f => f.EndsWith(this.ReplicationData));
			return productsLastUpdated;
		}

		/// <summary>
		/// 最終同期日時を記録するためのファイルを作成する
		/// </summary>
		/// <param name="lastUpdatedAt">最終同期日時</param>
		private void CreateTimestampFile(DateTime lastUpdatedAt)
		{
			var fileName = string.Format("{0:yyyyMMdd-HHmm}_{1}", lastUpdatedAt, this.ReplicationData);
			File.Create(Path.Combine(s_directory, fileName));
		}

		/// <summary>
		/// 最終同期日時を記録する
		/// </summary>
		/// <param name="lastUpdatedAt">最終同期日時</param>
		internal void Record(DateTime lastUpdatedAt)
		{
			// 最終同期日時を記録するためのファイルを取得する
			var productsLastUpdated = FindTimestampFile();
			if (string.IsNullOrEmpty(productsLastUpdated))
			{
				CreateTimestampFile(lastUpdatedAt);
				return;
			}
			
			var newFileName = string.Format("{0:yyyyMMdd-HHmm}_{1}", lastUpdatedAt, this.ReplicationData);
			try
			{
				// 記録する
				File.Move(productsLastUpdated, Path.Combine(s_directory, newFileName));
			}
			catch(Exception exception)
			{
				var msg = string.Format("最終同期日時を更新できませんでした。'{0}'へ手動更新してください。", newFileName);
				FileLogger.WriteError(msg, exception);
			}
		}

		/// <summary>同期するデータ</summary>
		private string ReplicationData { get; set; }
	}
}
