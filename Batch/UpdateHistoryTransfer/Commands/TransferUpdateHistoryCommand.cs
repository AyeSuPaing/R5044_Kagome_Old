/*
=========================================================================================================
  Module      : 更新履歴転送コマンドクラス(TransferUpdateHistoryCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using w2.Common.Logger;
using w2.Common.Sql;

namespace w2.Commerce.Batch.UpdateHistoryTransfer.Commands
{
	/// <summary>
	/// 更新履歴転送コマンドクラス
	/// </summary>
	public class TransferUpdateHistoryCommand : TransferCommandBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TransferUpdateHistoryCommand()
		{
			this.TableName = Constants.TABLE_UPDATEHISTORY;
			this.Action = "更新履歴転送";
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 転送処理実行
		/// </summary>
		protected override void Exec()
		{
			// 対象日（開始・終了）取得
			var targetDateTo = DateTime.Parse(string.Format("{0}/{1}/01", DateTime.Now.Year, DateTime.Now.Month))
				.AddMonths(Constants.UPDATEHISTORY_RETENTION_PERIOD_MONTHS.Value * -1);
			DateTime? targetDateFrom = null;
			var sqlSelectTargetDateFrom =
				string.Format("SELECT MIN(date_created) AS date_created_from FROM {0} WHERE date_created < '{1}'",
					this.TableName.Replace("'", "''"),
					targetDateTo.ToString("yyyy/MM/dd").Replace("'", "''"));
			using (var accessor = new SqlAccessor(Constants.STRING_SQL_CONNECTION))
			using (var statement = new SqlStatement(sqlSelectTargetDateFrom))
			{
				var data = statement.SelectSingleStatementWithOC(accessor);
				if (data.Count != 0)
				{
					if (data[0][Constants.FIELD_UPDATEHISTORY_DATE_CREATED + "_from"] != DBNull.Value)
					{
						targetDateFrom = DateTime.Parse(((DateTime)data[0][Constants.FIELD_UPDATEHISTORY_DATE_CREATED + "_from"]).ToString("yyyy/MM/dd"));
					}
				}
			}

			// 対象日が存在する場合はデータ転送を行う
			var success = true;
			var errorMessages = "";
			var count = 0;
			if (targetDateFrom.HasValue)
			{
				// データ量が多くなることが考えられるため、1日分ずつ転送する
				for (var dateFrom = targetDateFrom.Value; dateFrom < targetDateTo; dateFrom = dateFrom.AddDays(1))
				{
					// SQL作成
					var sqlWhere = string.Format(
						"WHERE date_created >= '{0}' AND date_created < '{1}'",
						dateFrom.ToString("yyyy/MM/dd").Replace("'", "''"),
						dateFrom.AddDays(1).ToString("yyyy/MM/dd").Replace("'", "''"));
					var sqlSelect = string.Format("SELECT * FROM {0} " + sqlWhere, this.TableName);
					var sqlDelete = string.Format("DELETE FROM {0} " + sqlWhere, this.TableName);

					using (SqlAccessor accessor = new SqlAccessor(Constants.STRING_SQL_CONNECTION))
					{
						// トランザクション開始
						accessor.OpenConnection();
						accessor.BeginTransaction();

						try
						{
							// 転送元から更新履歴取得
							DataTable data;
							using (var statement = new SqlStatement(sqlSelect))
							{
								data = statement.SelectStatement(accessor).Tables[0];
							}

							// データが存在する場合は転送処理を行う
							if (data.DefaultView.Count != 0)
							{
								// 転送元に更新履歴削除
								using (var sqlStatement = new SqlStatement(sqlDelete))
								{
									sqlStatement.ExecStatement(accessor);
								}

								// 転送先に更新履歴登録（BulkInsertで実行）
								using (var bulkCopy = new SqlBulkCopy(Constants.STRING_SQL_CONNECTION_TRANSFER))
								{
									// タイムアウト無制限
									bulkCopy.BulkCopyTimeout = 0;
									bulkCopy.DestinationTableName = this.TableName;
									bulkCopy.WriteToServer(data);
								}

								count += data.DefaultView.Count;
							}

							// トランザクションコミット
							accessor.CommitTransaction();
						}
						catch (Exception ex)
						{
							success = false;
							errorMessages = ex.ToString();
							try
							{
								// トランザクションロールバック
								accessor.RollbackTransaction();
							}
							catch (Exception ex2)
							{
								errorMessages += "\r\n" + ex2.ToString();
							}
							AppLogger.WriteError(errorMessages);
							break;
						}
					}
				}
			}
			else
			{
				success = false;
				errorMessages = "転送データが存在しませんでした。";
			}

			var messages = "■データ（処理件数）\r\n"
				+ string.Format("{0}件\r\n", count);
			if (success == false)
			{
				messages += "以下のエラーが発生しました\r\n" + string.Format("{0}\r\n", errorMessages);
			}
			this.Messages = messages;

			Console.WriteLine(this.Messages);
		}
		#endregion
	}
}