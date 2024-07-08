/*
=========================================================================================================
  Module      : ターゲットリスト抽出クラス(TargetListUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.App.Common.TargetList
{
	public class TargetListUtility
	{
		/// <summary>
		/// ターゲットリスト比較クラス
		/// </summary>
		private class TargetListComparer : IEqualityComparer<UserModel>
		{
			/// <summary>
			/// 指定したオブジェクトが等しいかどうかを返す
			/// </summary>
			/// <param name="x">ユーザマスタモデル</param>
			/// <param name="y">ユーザマスタモデル</param>
			/// <returns>指定したオブジェクトが等しいかどうか</returns>
			public bool Equals(UserModel x, UserModel y)
			{
				return ((x.UserId == y.UserId)
					&& (x.MailAddr == y.MailAddr)
					&& (x.MailAddr2 == y.MailAddr2));
			}

			/// <summary>
			/// ハッシュコードを取得
			/// </summary>
			/// <param name="u">ユーザマスタモデル</param>
			/// <returns>ハッシュコード</returns>
			public int GetHashCode(UserModel u)
			{
				return u.UserId.GetHashCode();
			}
		}

		/// <summary>
		/// 抽出実行
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetListId">ターゲットリストID</param>
		public static void ExecuteExtractTargetListData(string deptId, string targetListId)
		{
			// ターゲットリストデータ削除
			using (var sqlAccessor = new SqlAccessor())
			{
				try
				{
					sqlAccessor.OpenConnection();
					sqlAccessor.BeginTransaction();

					var targetList = GetTargetListBatchNotUsing(deptId, targetListId);
					switch ((string)targetList[Constants.FIELD_TARGETLIST_TARGET_TYPE])
					{
						//------------------------------------------------------
						// ユーザリストのターゲットリスト
						//------------------------------------------------------
						case Constants.FLG_TARGETLIST_TARGET_TYPE_MANUAL:
							// ターゲットリストデータを削除 
							DeleteTargetListData(deptId, targetListId, sqlAccessor);

							// ターゲットリストデータ作成
							CreateTargetListData(deptId, targetListId, (string)targetList[Constants.FIELD_TARGETLIST_TARGET_CONDITION], sqlAccessor);

							// ターゲット リストのデータ カウントを更新します。
							UpdateTargetListDataCountByTargetListData(deptId, targetListId, sqlAccessor);

							sqlAccessor.CommitTransaction();
							break;
					}
				}
				catch (Exception)
				{
					sqlAccessor.RollbackTransaction();
					throw;
				}
			}
		}

		/// <summary>
		/// ターゲットリスト取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetListId">ターゲットリストID</param>
		/// <returns>ターゲットリスト</returns>
		private static DataRowView GetTargetListBatchNotUsing(string deptId, string targetListId)
		{
			var targetListData = GetTargetList(deptId, targetListId);
			if (targetListData.Count == 0) return null;

			switch ((string)targetListData[0][Constants.FIELD_TARGETLIST_STATUS])
			{
				case Constants.FLG_TARGETLIST_STATUS_NORMAL:
					// 次へ（ステータスは既に抽出中となっている）
					return targetListData[0];

				case Constants.FLG_TARGETLIST_STATUS_EXTRACT:
				case Constants.FLG_TARGETLIST_STATUS_USING:
				case Constants.FLG_TARGETLIST_STATUS_ERROR:
					throw new Exception("「通常」ステータスのターゲットリストを抽出できませんでした。抽出実行中か、失敗している可能性があります。");

				default:
					return null;
			}

		}

		/// <summary>
		/// 必要なフィールドを使用して新しいCSVファイルを作成
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="listUserInfo">ユーザー情報</param>
		/// <returns>CSVデータ</returns>
		private static IEnumerable<string> BuildCsvContent(string deptId, string masterId, IEnumerable<UserModel> listUserInfo)
		{
			// ヘッダ部生成
			yield return BuildCsvHeader();

			// ボディ部生成
			foreach (var user in listUserInfo)
			{
				var bodies = BuildCsvBody(
					deptId,
					masterId,
					user.UserId,
					user.MailAddr,
					user.MailAddr2);

				foreach (var body in bodies)
				{
					yield return body;
				}
			}
		}

		/// <summary>
		/// ヘッダ部CSVの生成
		/// </summary>
		/// <returns>ヘッダ部CSV</returns>
		public static string BuildCsvHeader()
		{
			var headers = new[] 
			{ 
				"TBL",
				Constants.FIELD_TARGETLISTDATA_USER_ID,
				Constants.FIELD_TARGETLISTDATA_MAIL_ADDR,
				Constants.FIELD_TARGETLISTDATA_DEPT_ID,
				Constants.FIELD_TARGETLISTDATA_MASTER_ID,
				Constants.FIELD_TARGETLISTDATA_TARGET_KBN,
				Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN
			};
			var csvHeader = StringUtility.CreateEscapedCsvString(headers);
			return csvHeader;
		}

		/// <summary>
		/// 行ごとのボディ部CSVの生成
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailAddr2">メールアドレス2</param>
		/// <returns>行ごとのボディ部CSV</returns>
		public static IEnumerable<string> BuildCsvBody(
			string deptId,
			string masterId,
			string userId,
			string mailAddr,
			string mailAddr2)
		{
			var mailAddrKbn = string.Empty;
			var targetMailAddr = string.Empty;
			if (string.IsNullOrEmpty(mailAddr) == false)
			{
				targetMailAddr = mailAddr;
				mailAddrKbn = Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC;
			}
			else if (string.IsNullOrEmpty(mailAddr)
				&& (string.IsNullOrEmpty(mailAddr2) == false))
			{
				targetMailAddr = mailAddr2;
				mailAddrKbn = Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE;
			}
			var dataColumns = new[] 
			{
				"IU",
				userId,
				targetMailAddr,
				deptId,
				masterId,
				Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST,
				mailAddrKbn
			};
			yield return StringUtility.CreateEscapedCsvString(dataColumns);

			// ターゲットリストデータ作成(双方送信用)
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED
				&& (string.IsNullOrEmpty(mailAddr) == false)
				&& (string.IsNullOrEmpty(mailAddr2) == false))
			{
				targetMailAddr = mailAddr2;
				mailAddrKbn = "MB";
				dataColumns = new[] 
				{
					"IU",
					userId,
					targetMailAddr,
					deptId,
					masterId,
					Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST,
					mailAddrKbn
				};
				yield return StringUtility.CreateEscapedCsvString(dataColumns);
			}
		}

		/// <summary>
		/// 必要なフィールドを使用して新しいCSVファイルを作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="listUserInfo">ユーザー情報</param>
		/// <param name="fileName">ファイル名</param>
		/// <param name="totalDataCount">前回抽出件数</param>
		/// <returns>CSVファイルパス</returns>
		public static string CreateImportCsvToActiveDirectory(
			string shopId,
			string deptId,
			string masterId,
			IEnumerable<UserModel> listUserInfo,
			string fileName,
			out int totalDataCount)
		{
			// CSV生成
			var tempCsvFilePath = Path.GetTempFileName();

			var csvContent = BuildCsvContent(deptId, masterId, listUserInfo);
			totalDataCount = WriteCsvContent(tempCsvFilePath, csvContent);

			// アクティブディレクトリへファイル移動
			var activeDirectoryPath = Path.Combine(
				Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR,
				shopId,
				Constants.TABLE_TARGETLISTDATA,
				Constants.DIRNAME_MASTERIMPORT_ACTIVE);
			if (Directory.Exists(activeDirectoryPath) == false) Directory.CreateDirectory(activeDirectoryPath);

			var activeFilePath = Path.Combine(activeDirectoryPath, fileName + ".csv");
			File.Delete(activeFilePath);
			File.Move(tempCsvFilePath, activeFilePath);

			return activeFilePath;
		}

		/// <summary>
		/// CSVファイルを書き出します。
		/// </summary>
		/// <param name="outputFilePath">出力先ファイルパス</param>
		/// <param name="csvContent">CSV出力内容</param>
		/// <returns>CSV行数（ヘッダ除く）</returns>
		private static int WriteCsvContent(string outputFilePath, IEnumerable<string> csvContent)
		{
			var count = 0;
			using (var sw = new StreamWriter(outputFilePath, true, Encoding.GetEncoding("Shift_JIS")))
			{
				foreach (var line in csvContent)
				{
					sw.WriteLine(line);
					count++;
				}
			}
			return count - 1;
		}

		/// <summary>
		/// ターゲットリスト存在チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>	
		/// <returns>存在するか</returns>
		public static bool IsTargetListExists(string deptId, string targetId)
		{
			return (GetTargetList(deptId, targetId).Count > 0);
		}

		/// <summary>
		/// ターゲットリストを取得します
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ターゲットリスト</returns>
		public static DataView GetTargetList(string deptId, string targetId)
		{
			using (var sqlAccessor = new SqlAccessor())
			{
				return GetTargetList(deptId, targetId, sqlAccessor);
			}
		}
		/// <summary>
		/// ターゲットリストを取得します
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ターゲット リスト </returns>
		public static DataView GetTargetList(string deptId, string targetId, SqlAccessor sqlAccessor)
		{
			using (var sqlStatement = new SqlStatement("TargetList", "GetTargetList"))
			{
				var sqlParams = new Hashtable
				{
					{ Constants.FIELD_TARGETLIST_DEPT_ID, deptId },
					{ Constants.FIELD_TARGETLIST_TARGET_ID, targetId }
				};
				return sqlStatement.SelectSingleStatement(sqlAccessor, sqlParams);
			}
		}

		/// <summary>
		/// ターゲット名取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットID</param>
		/// <returns>ターゲット名取得</returns>
		public static string GetTargetName(string deptId, string targetId)
		{
			var targetList = GetTargetList(deptId, targetId);
			var targetName = (targetList.Count != 0)
				? (string)targetList[0][Constants.FIELD_TARGETLIST_TARGET_NAME]
				: string.Empty;

			return targetName;
		}

		/// <summary>
		/// ターゲットリストデータを削除 
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void DeleteTargetListData(string deptId, string targetId, SqlAccessor sqlAccessor)
		{
			var sqlParams = new Hashtable
			{
				{Constants.FIELD_TARGETLISTDATA_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST},
				{Constants.FIELD_TARGETLISTDATA_MASTER_ID, targetId}
			};
			using (var sqlStatement = new SqlStatement("TargetListData", "DeleteTargetListData") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				sqlStatement.ExecStatement(sqlAccessor, sqlParams);
			}
		}

		/// <summary>
		/// 新規ターゲットリストを挿入
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorName">更新者名</param>
		/// <param name="targetListName">ターゲットリスト名</param>
		/// <returns>ターゲットリストID</returns>
		public static string InsertNewTargetList(string shopId, string deptId, string operatorName, string targetListName)
		{
			//------------------------------------------------------
			// 値セット
			//------------------------------------------------------
			var sqlParams = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, NumberingUtility.CreateKeyId(shopId, Constants.NUMBER_KEY_TARGET_LIST_ID, 10)},
				{Constants.FIELD_TARGETLIST_TARGET_NAME, targetListName},
				{Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_CSV},
				{Constants.FIELD_TARGETLIST_TARGET_CONDITION, string.Empty},
				{Constants.FIELD_TARGETLIST_LAST_CHANGED, operatorName},
				{Constants.FIELD_TARGETLIST_EXEC_TIMING, string.Empty},
				{Constants.FIELD_TARGETLIST_SCHEDULE_KBN, string.Empty},
				{Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK, string.Empty},
				{Constants.FIELD_TARGETLIST_SCHEDULE_YEAR, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_MONTH, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_DAY, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_HOUR, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_SECOND, null},
				{Constants.FIELD_TARGETLIST_DEL_FLG, "1"}
			};

			using (var sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					//------------------------------------------------------
					// ターゲットリスト追加（ターゲットID取得のためSELECT）
					//------------------------------------------------------
					using (var sqlStatement = new SqlStatement("TargetList", "InsertTargetList"))
					{
						sqlStatement.ExecStatement(sqlAccessor, sqlParams);
					}

					// コミット
					sqlAccessor.CommitTransaction();
					return (string)sqlParams[Constants.FIELD_TARGETLIST_TARGET_ID];
				}
				catch
				{
					// ロールバック
					sqlAccessor.RollbackTransaction();
					throw;
				}
			}
		}

		/// <summary>
		/// ターゲットリストデータ作成
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="targetlistTargetCondition">抽出条件(XMLデータ)</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="commandTimeout">SQL実行タイムアウト</param>
		/// <returns>取得した件数</returns>
		public static int CreateTargetListData(
			string deptId,
			string targetId,
			string targetlistTargetCondition,
			SqlAccessor sqlAccessor,
			int? commandTimeout = null)
		{
			// 条件作成
			var targetListConditionList = TargetListConditionRelationXml.CreateTargetListConditionList(targetlistTargetCondition);

			// ANDかOR判定して条件を変更する
			var conditionType = targetListConditionList.TargetConditionList[0]
				.GetConditionType(targetListConditionList.TargetConditionList[0]);

			var manualSql = string.Empty;
			if (conditionType == Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_AND)
			{
				manualSql += CreateSqlAndCondition(targetListConditionList);
			}
			else
			{
				manualSql += CreateSqlOrCondition(targetListConditionList);
			}

			var sqlParams = new Hashtable
			{
				{Constants.FIELD_TARGETLISTDATA_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST},
				{Constants.FIELD_TARGETLISTDATA_MASTER_ID, targetId}
			};
			// SQLタイムアウト値設定
			using (var sqlStatement = new SqlStatement(
				"TargetListData",
				(Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED
					? "CreateTargetListDataBothPcAndMobile"
					: "CreateTargetListData")) { CommandTimeout = commandTimeout ?? Constants.SQL_COMMAND_TIMEOUT })
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ param @@", manualSql);
				return sqlStatement.ExecStatement(sqlAccessor, sqlParams);
			}
		}

		/// <summary>
		/// AND条件のSQL作成
		/// </summary>
		/// <param name="targetListConditionList">条件</param>
		/// <returns>作成したSQL</returns>
		private static string CreateSqlAndCondition(TargetListConditionList targetListConditionList)
		{
			var sql = new StringBuilder();
			sql.Append("			(\n");

			var groupSql = new StringBuilder();
			var nonGroupConditions = new TargetListConditionGroup();

			foreach (var targetGroup in targetListConditionList.TargetConditionList)
			{
				// グループ化されていない条件はすべて合わせて1つのグループ条件として扱う
				if (targetGroup is TargetListCondition)
				{
					((TargetListCondition)targetGroup).GroupConditionType =
						Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_AND;
					nonGroupConditions.TargetGroup.Add((TargetListCondition)targetGroup);
				}
				else
				{
					if (string.IsNullOrEmpty(groupSql.ToString()) == false)
					{
						groupSql.Append("			" + Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_AND + "\n");
					}
					groupSql.Append("			(\n");
					groupSql.Append(new CreateTargetListSql().CreateManualSql(targetGroup));
					groupSql.Append("			)\n");
				}
			}
			// SQL作成
			if (nonGroupConditions.TargetGroup.Count != 0)
			{
				var createTargetListSql = new CreateTargetListSql();
				sql.Append(createTargetListSql.CreateManualSql(nonGroupConditions));
				if (string.IsNullOrEmpty(groupSql.ToString()) == false)
				{
					sql.Append("			" + Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_AND + "\n");
				}
			}
			if (string.IsNullOrEmpty(groupSql.ToString()) == false)
			{
				sql.Append(groupSql);
			}

			sql.Append("			)\n");
			return sql.ToString();
		}

		/// <summary>
		/// OR条件のSQL作成
		/// </summary>
		/// <param name="targetListConditionList">条件</param>
		/// <returns>作成したSQL</returns>
		private static string CreateSqlOrCondition(TargetListConditionList targetListConditionList)
		{
			var sql = new StringBuilder();
			foreach (var targetGroup in targetListConditionList.TargetConditionList)
			{
				if (targetGroup != targetListConditionList.TargetConditionList.First())
				{
					sql.Append("			" + targetGroup.GetConditionType(targetGroup) + "\n");
				}
				sql.Append("			(\n");
				sql.Append(new CreateTargetListSql().CreateManualSql(targetGroup));
				sql.Append("			)\n");
			}

			return sql.ToString();
		}

		/// <summary>
		/// ターゲット リストのデータ カウントを更新します。
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetListId">ターゲットリストID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void UpdateTargetListDataCountByTargetListData(string deptId, string targetListId, SqlAccessor sqlAccessor)
		{
			var sqlParams = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetListId},
				{Constants.FIELD_TARGETLISTDATA_MASTER_ID, targetListId}
			};
			using (var sqlStatement = new SqlStatement("TargetList", "UpdateTargetListDataCountByTargetListData"))
			{
				sqlStatement.ExecStatement(sqlAccessor, sqlParams);
			}
		}

		/// <summary>
		/// ターゲットリストのデータ カウントを更新します。
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="totalDataCount">前回抽出件数</param>
		/// <param name="deptId">識別ID</param>
		public static void UpdateTargetListDataCount(string targetId, int totalDataCount, string deptId)
		{
			// データ数を更新します。
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("TargetList", "UpdateTargetListDataCount"))
			{
				var sqlParam = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
				{Constants.FIELD_TARGETLIST_DATA_COUNT, totalDataCount}
			};
				sqlStatement.ExecStatementWithOC(sqlAccessor, sqlParam);
			}
		}

		/// <summary>
		/// ターゲットリストをMP管理画面へ表示(del_flgの更新)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetListId">ターゲットリストID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void UpdateTargetListToVisible(string deptId, string targetListId, SqlAccessor sqlAccessor)
		{
			var sqlParams = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetListId},
			};
			using (var sqlStatement = new SqlStatement("TargetList", "UpdateTargetListDelFlgToVisible"))
			{
				sqlStatement.ExecStatement(sqlAccessor, sqlParams);
			}
		}

		/// <summary>
		/// 有効なユーザ（削除フラグが通常）を取得し、
		/// ユーザ、メールアドレス、メールアドレス2で一意なデータを抽出します。
		/// </summary>
		/// <param name="listUserInfo">ユーザー情報</param>
		/// <returns>ユーザ情報リスト</returns>
		public static IEnumerable<UserModel> GetAvailableDistinctTargetList(IEnumerable<UserModel> listUserInfo)
		{
			return listUserInfo.Where(user =>
				(user.DelFlg == Constants.FLG_USER_DELFLG_UNDELETED))
				.Distinct(new TargetListComparer());
		}
	}
}
