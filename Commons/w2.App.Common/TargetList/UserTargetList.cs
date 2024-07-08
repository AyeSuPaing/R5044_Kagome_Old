/*
=========================================================================================================
  Module      : User Target List(UserTargetList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Order;
using w2.App.Common.OrderExtend;
using w2.App.Common.Web.Page;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.ProductTaxCategory;
using w2.Domain.User;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// User target list
	/// </summary>
	public static class UserTargetList
	{
		/// <summary>SELECT句のフィールドフォーマット</summary>
		private const string SELECT_FIELDS_FORMAT = "{0}.{1},{2}.{3},{4}.{5},{6}.{7}";

		/// <summary>
		/// データの入力チェックします。
		/// </summary>
		/// <param name="targetListName">Target list name</param>
		/// <returns>Error message</returns>
		public static string CheckInputData(string targetListName)
		{
			// ターゲットリスト名
			var errorMessage = new StringBuilder(Validator.CheckNecessaryError(CommonPage.ReplaceTag("@@DispText.target_list.TargetListName@@"), targetListName))
				.Append(Validator.CheckByteLengthMaxError(CommonPage.ReplaceTag("@@DispText.target_list.TargetListName@@"), targetListName, 60));
			return errorMessage.ToString();
		}

		/// <summary>
		/// ターゲット リストのデータ カウントを更新します。
		/// </summary>
		/// <param name="newTargetId">New target id</param>
		/// <param name="totalDataCount">Total data count</param>
		/// <param name="loginOperatorDeptId">Login operator dept id</param>
		public static void UpdateTargetListDataCount(
			string newTargetId,
			int totalDataCount,
			string loginOperatorDeptId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TARGETLIST_DEPT_ID, loginOperatorDeptId },
				{ Constants.FIELD_TARGETLIST_TARGET_ID, newTargetId },
				{ Constants.FIELD_TARGETLIST_DATA_COUNT, totalDataCount }
			};

			// データ数を更新します。
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("TargetList", "UpdateTargetListDataCount"))
			{
				statement.ExecStatementWithOC(accessor, input);
			}
		}

		/// <summary>
		/// 新規ターゲットリストを挿入
		/// </summary>
		/// <param name="targetListType">Target list type</param>
		/// <param name="targetListName">Target list name</param>
		/// <param name="loginOperatorDeptId">Login operator dept id</param>
		/// <param name="loginOperatorName">Login operator name</param>
		/// <returns>ターゲットリストID</returns>
		public static string CreateNewTargetList(
			string targetListType,
			string targetListName,
			string loginOperatorDeptId,
			string loginOperatorName)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TARGETLIST_DEPT_ID, loginOperatorDeptId },
				{
					Constants.FIELD_TARGETLIST_TARGET_ID,
					NumberingUtility.CreateKeyId(loginOperatorDeptId, Constants.NUMBER_KEY_TARGET_LIST_ID, 10)
				},
				{ Constants.FIELD_TARGETLIST_TARGET_NAME, targetListName },
				{ Constants.FIELD_TARGETLIST_TARGET_TYPE, targetListType },
				{ Constants.FIELD_TARGETLIST_TARGET_CONDITION, string.Empty },
				{ Constants.FIELD_TARGETLIST_LAST_CHANGED, loginOperatorName },
				{ Constants.FIELD_TARGETLIST_EXEC_TIMING, string.Empty },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_KBN, string.Empty },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK, string.Empty },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_YEAR, null },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_MONTH, null },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_DAY, null },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_HOUR, null },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE, null },
				{ Constants.FIELD_TARGETLIST_SCHEDULE_SECOND, null },
				{ Constants.FIELD_TARGETLIST_DEL_FLG, Constants.FLG_TARGETLIST_DEL_FLG_VALID },
			};

			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				try
				{
					// ターゲットリスト追加（ターゲットID取得のためSELECT）
					using (var statement = new SqlStatement("TargetList", "InsertTargetList"))
					{
						statement.ExecStatement(accessor, input);
					}

					// コミット
					accessor.CommitTransaction();
					return StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_TARGET_ID]);
				}
				catch
				{
					// ロールバック
					accessor.RollbackTransaction();
					throw;
				}
			}
		}

		/// <summary>
		/// ターゲットリストの件数をカウントします。
		/// </summary>
		/// <param name="param">Param</param>
		/// <returns>User count</returns>
		public static int GetUserCount(Hashtable param)
		{
			var inputParameters = (Hashtable)param[Constants.TABLE_USER];
			inputParameters["mail_send_both_pc_and_mobile_enabled"] = Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED;

			var queryFile = string.Empty;
			var queryStatement = string.Empty;
			var selectFields = string.Empty;
			switch ((string)param[Constants.FIELD_TARGETLIST_TARGET_TYPE])
			{
				case Constants.FLG_TARGETLIST_TARGET_TYPE_USER_LIST:
					queryFile = "User";
					queryStatement = "GetUserMasterCount";
					selectFields = string.Format(
						SELECT_FIELDS_FORMAT,
						Constants.TABLE_USER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST:
					queryFile = "Order";
					queryStatement = "GetOrderMasterCount";
					selectFields = string.Format(
						SELECT_FIELDS_FORMAT,
						Constants.TABLE_ORDER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST:
					queryFile = "Order";
					queryStatement = "GetOrderWorkflowMasterCount";
					selectFields = string.Format(
						SELECT_FIELDS_FORMAT,
						Constants.TABLE_ORDER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST:
					queryFile = "FixedPurchase";
					queryStatement = "GetFixedPurchaseMasterCount";
					selectFields = string.Format(
						SELECT_FIELDS_FORMAT,
						Constants.TABLE_FIXEDPURCHASE,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW_LIST:
					queryFile = "FixedPurchase";
					queryStatement = "GetFixedPurchaseWorkflowMasterCount";
					selectFields = string.Format(
						SELECT_FIELDS_FORMAT,
						Constants.TABLE_FIXEDPURCHASE,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_REPEAT_ANALYSIS_REPORT:
					queryFile = "FixedPurchaseRepeatAnalysis";
					queryStatement = "GetFixedPurchaseRepeatAnalysisMasterCount";
					selectFields = string.Format(
						SELECT_FIELDS_FORMAT,
						Constants.TABLE_USER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					break;
			}

			var count = GetUserCount(
				queryFile,
				queryStatement,
				selectFields,
				inputParameters);
			return count;
		}

		/// <summary>
		/// ターゲットリストの件数をカウントします。
		/// </summary>
		/// <param name="queryFile">ステートメントページ名</param>
		/// <param name="queryStatement">ステートメント名</param>
		/// <param name="selectFields">SELECT対象のフィールド</param>
		/// <param name="inputParameters">入力パラメータ</param>
		/// <returns>User count</returns>
		public static int GetUserCount(
			string queryFile,
			string queryStatement,
			string selectFields,
			Hashtable inputParameters)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(queryFile, queryStatement))
			{
				statement.Statement = statement.Statement.Replace(
					"@@ where @@",
					StringUtility.ToEmpty(inputParameters["@@ where @@"]));

				statement.Statement = statement.Statement.Replace(
					"@@ fields @@",
					selectFields);

				statement.ReplaceStatement(
					"@@ multi_order_id @@",
					OrderCommon.GetOrderSearchMultiOrderId(inputParameters));

				statement.Statement = ReplaceUserExtendFieldName(
					statement.Statement,
					(string)inputParameters["user_extend_name"]);

				if ((queryFile == "Order") || (queryFile == "FixedPurchase"))
				{
					var orderExtendTable = (queryFile == "Order")
						? Constants.TABLE_ORDER
						: Constants.TABLE_FIXEDPURCHASE;

					statement.Statement = OrderExtendCommon
						.ReplaceOrderExtendFieldName(
							statement.Statement,
							orderExtendTable,
							StringUtility.ToEmpty(inputParameters[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
				}

				statement.ReplaceStatement(
					"@@ real_shop_ids_condition @@",
					string.Empty);

				return (int)statement.SelectSingleStatementWithOC(accessor, inputParameters)[0][0];
			}
		}

		/// <summary>
		/// 各検索条件からユーザー情報取得
		/// </summary>
		/// <param name="sessionParameters">Session parameters</param>
		/// <returns>ユーザー情報</returns>
		public static IEnumerable<UserModel> GetUserInfos(Hashtable sessionParameters)
		{
			var inputParameters = (Hashtable)(sessionParameters)[Constants.TABLE_USER];
			var queryFile = string.Empty;
			var queryStatement = string.Empty;
			switch ((string)(sessionParameters)[Constants.FIELD_TARGETLIST_TARGET_TYPE])
			{
				case Constants.FLG_TARGETLIST_TARGET_TYPE_USER_LIST:
					queryFile = "User";
					queryStatement = "GetUserMaster";
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST:
					queryFile = "Order";
					queryStatement = "GetOrderMaster";
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST:
					queryFile = "Order";
					queryStatement = "GetOrderWorkflowMaster";
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST:
					queryFile = "FixedPurchase";
					queryStatement = "GetFixedPurchaseMaster";
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW_LIST:
					queryFile = "FixedPurchase";
					queryStatement = "GetFixedPurchaseWorkflowMaster";
					break;

				case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_REPEAT_ANALYSIS_REPORT:
					queryFile = "FixedPurchaseRepeatAnalysis";
					queryStatement = "GetFixedPurchaseRepeatAnalysisMaster";
					break;
			}

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(queryFile, queryStatement))
			{
				var fields = string.Empty;
				if (queryStatement == "GetOrderWorkflowMaster")
				{
					fields = string.Format(
						"{0}.{1},{2}.{3},{4}.{5},{6}.{7}",
						Constants.TABLE_ORDER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					statement.Statement = statement.Statement.Replace("@@ where @@", (string)inputParameters["@@ where @@"]);
				}

				if (queryStatement == "GetFixedPurchaseWorkflowMaster")
				{
					fields = string.Format(
						"{0}.{1},{2}.{3},{4}.{5},{6}.{7}",
						Constants.TABLE_USER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
					statement.Statement = statement.Statement.Replace("@@ where @@", (string)inputParameters["@@ where @@"]);
				}

				if (queryStatement == "GetOrderMaster")
				{
					fields = string.Format(
						"{0}.{1},{2}.{3},{4}.{5},{6}.{7}",
						Constants.TABLE_ORDER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
				}

				if (queryStatement == "GetUserMaster")
				{
					fields = string.Format(
						"{0}.{1},{2}.{3},{4}.{5},{6}.{7}",
						Constants.TABLE_USER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
				}

				if (queryStatement == "GetFixedPurchaseMaster")
				{
					fields = string.Format(
						"{0}.{1},{2}.{3},{4}.{5},{6}.{7}",
						Constants.TABLE_USER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
				}

				if (queryStatement == "GetFixedPurchaseRepeatAnalysisMaster")
				{
					fields = string.Format(
						"{0}.{1},{2}.{3},{4}.{5},{6}.{7}",
						Constants.TABLE_USER,
						Constants.FIELD_USER_USER_ID,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR,
						Constants.TABLE_USER,
						Constants.FIELD_USER_MAIL_ADDR2,
						Constants.TABLE_USER,
						Constants.FIELD_USER_DEL_FLG);
				}

				// 税率毎価格情報項目設定
				var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
				var taxFields = string.Join(",", taxFieldNames.Select(taxfieldName => string.Format("[{0}]", taxfieldName)));

				statement.Statement = statement.Statement.Replace("@@ fields @@", fields);

				// @@ orderby @@ などを置換
				statement.ReplaceStatement("@@ orderby @@", OrderCommon.GetOrderSearchOrderByStringForOrderListAndWorkflow((string)inputParameters["sort_kbn"]));

				// Replace @@ multi_order_id @@ with conditional statement
				statement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(inputParameters));

				// @@ user_extend_field_name @@を条件文に置換
				statement.Statement = ReplaceUserExtendFieldName(
					statement.Statement,
					(string)inputParameters["user_extend_name"]);

				if ((queryFile == "Order") || (queryFile == "FixedPurchase"))
				{
					var orderExtendTable = (queryFile == "Order")
						? Constants.TABLE_ORDER
						: Constants.TABLE_FIXEDPURCHASE;
					statement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(
						statement.Statement, orderExtendTable,
						StringUtility.ToEmpty(inputParameters[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
				}

				using (var reader = new SqlStatementDataReader(accessor, statement, inputParameters, true))
				{
					while (reader.Read())
					{
						var dict = Enumerable.Range(0, reader.FieldCount).ToDictionary(
							i => reader.GetName(i),
							i => ((reader[i] == DBNull.Value) ? null : reader[i]));
						var ht = new Hashtable(dict);
						yield return new UserModel(ht);
					}
				}
			}
		}

		/// <summary>
		/// @@ user_extend_field_name @@を条件文に置換
		/// </summary>
		/// <param name="sqlStatement">SQL本文</param>
		/// <param name="userExtendName">ユーザー拡張名</param>
		/// <returns>Replaced user extend field name</returns>
		private static string ReplaceUserExtendFieldName(string sqlStatement, string userExtendName)
		{
			var result = sqlStatement.Replace(
				"@@ user_extend_field_name @@",
					string.Format(
						"{0}.{1}",
						Constants.TABLE_USEREXTEND,
						string.IsNullOrEmpty(userExtendName)
							? Constants.FIELD_USEREXTEND_USER_ID
							: userExtendName));
			return result;
		}
	}
}
