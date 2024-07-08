/*
=========================================================================================================
  Module      : FreeExport DB更新クラス(FreeExportDbUpdate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using ExternalAPI.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using w2.App.Common;
using w2.Common.Sql;
using w2.Domain.UpdateHistory;

namespace ExternalAPI.FreeExport
{
	/// <summary>
	/// FreeExport DB更新クラス
	/// </summary>
	public class FreeExportDbUpdate
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exportSetting">FreeExport 実行設定内容</param>
		public FreeExportDbUpdate(ExportSetting exportSetting)
		{
			this.ExportSetting = exportSetting;
		}

		/// <summary>
		/// DB更新実行
		/// </summary>
		/// <param name="workUpdateDateFilePath">作業ディレクトリ DB更新用ファイルパス</param>
		public void Update(string workUpdateDateFilePath)
		{
			// ファイルが存在しなければ実行なし
			if (File.Exists(workUpdateDateFilePath) == false) return;

			if (this.ExportSetting.CanDbUpdate)
			{
				using (var fs = File.OpenRead(workUpdateDateFilePath))
				using (var sr = new StreamReader(fs, EncodingHelper.GetEncoding(this.ExportSetting.FileSetting.EncodingType)))
				{
					var lineList = new List<string>();
					var count = 0;
					var header = new string[] { };
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							count++;

							if (count == 1)
							{
								// header取得
								header = line.Split(',');
								continue;
							}

							// 実行単位毎にまとめて更新処理
							lineList.Add(line);
							if (lineList.Count == this.ExportSetting.SearchSetting.UpdateSql.ExecUnit)
							{
								UpdateUnit(header, lineList, accessor);
								lineList = new List<string>();
								Thread.Sleep(10);
							}
						}

						// 端数単位をまとめて更新処理
						if (lineList.Count > 0)
						{
							UpdateUnit(header, lineList, accessor);
						}
					}
				}
			}

			// 実行後ファイルの削除
			File.Delete(workUpdateDateFilePath);
		}

		/// <summary>
		/// 実行単位ごとの更新処理
		/// </summary>
		/// <param name="header">header 設定ファイル内のUpdateKey</param>
		/// <param name="lineList">実行内容</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void UpdateUnit(string[] header, List<string> lineList, SqlAccessor accessor)
		{
			// 更新用SQLの作成
			var updateSql = string.Join("\n", lineList.Select(l => GetUpdateSql(header, l.Split(','))));
			using (var statement = new SqlStatement(updateSql))
			{
				// 更新実行
				statement.ExecStatement(accessor);
			}

			// 注文情報 更新履歴の追加
			if ((this.ExportSetting.SearchSetting.UpdateSql.HistoryType != null)
					&& this.ExportSetting.SearchSetting.UpdateSql.HistoryType.Any(ht => ht == HistoryType.Order)
					&& header.Any(h => (h == Constants.FIELD_ORDER_ORDER_ID)))
			{
				foreach (var lineTemp in lineList)
				{
					var index = header.Select(
						(h, i) => new
						{
							key = h,
							index = i
						}).Where(v => (v.key == Constants.FIELD_ORDER_ORDER_ID)).Select(v => v.index).FirstOrDefault();
					var values = lineTemp.Split(',');
					var orderId = values[index];
					new UpdateHistoryService().InsertForOrder(orderId, Constants.FLG_LASTCHANGED_BATCH, accessor);
				}
			}

			// ユーザ情報 更新履歴の追加
			if ((this.ExportSetting.SearchSetting.UpdateSql.HistoryType != null)
				&& this.ExportSetting.SearchSetting.UpdateSql.HistoryType.Any(ht => ht == HistoryType.User)
				&& header.Any(h => h == Constants.FIELD_USER_USER_ID))
			{
				foreach (var lineTemp in lineList)
				{
					var index = header.Select(
						(h, i) => new
						{
							key = h,
							index = i
						}).Where(v => v.key == Constants.FIELD_USER_USER_ID).Select(v => v.index).FirstOrDefault();
					var values = lineTemp.Split(',');
					var userId = values[index];
					new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_BATCH, accessor);
				}
			}

			// 定期台帳 更新履歴の追加
			if ((this.ExportSetting.SearchSetting.UpdateSql.HistoryType != null)
				&& this.ExportSetting.SearchSetting.UpdateSql.HistoryType.Any(ht => ht == HistoryType.FixedPurchase)
				&& header.Any(h => h == Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID))
			{
				foreach (var lineTemp in lineList)
				{
					var index = header.Select(
						(h, i) => new
						{
							key = h,
							index = i
						}).Where(v => v.key == Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID).Select(v => v.index).FirstOrDefault();
					var values = lineTemp.Split(',');
					var fixedPurchaseId = values[index];
					new UpdateHistoryService().InsertForFixedPurchase(
						fixedPurchaseId,
						Constants.FLG_LASTCHANGED_BATCH,
						accessor);
				}
			}
		}

		/// <summary>
		/// 更新用SQLの取得
		/// </summary>
		/// <param name="header">header 設定ファイル内のUpdateKey</param>
		/// <param name="value">1レコード内の各内容</param>
		/// <returns></returns>
		private string GetUpdateSql(string[] header, string[] value)
		{
			var result = "";

			for (int i = 0; i < header.Length; i++)
			{
				result = this.ExportSetting.SearchSetting.UpdateSql.Sql.Replace(
					string.Format("@@ {0} @@", header[i].Replace("'", "\"")),
					value[i].ToString());
			}

			return result;
		}

		/// <summary>
		/// FreeExport 連携ファイル作成クラス
		/// </summary>
		private ExportSetting ExportSetting { get; set; }
	}
}
