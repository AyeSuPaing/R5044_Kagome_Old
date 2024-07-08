/*
=========================================================================================================
  Module      : 検索SQLパラメータ(SearchSqlParameter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.Search
{
	/// <summary>
	/// 検索SQLパラメータ
	/// </summary>
	public class SearchSqlParameter
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sp">検索パラメータ</param>
		public SearchSqlParameter(SearchParameter sp)
		{
			// プロパティ初期化
			this.SqlParams = new List<SqlStatement.SqlParam>();
			this.InputValues = new Hashtable();
			this.WhereStatement = "";

			// Check search parameter
			if (sp == null) return;

			// 指定された追加条件に対応するSQL情報作成
			if (sp.Target != SearchTarget.SearchTop) GetFilteringSqlParams(sp);

			// 指定された検索キーワードに対応するSQL情報作成
			CreateConditionInfo(sp);

			// 指定された追加条件（集計区分値）に対応するSQL情報生成
			if (sp.Target != SearchTarget.SearchTop) CreateSummaryValueInfo(sp);
		}
		#endregion

		#region -CreateConditionInfo 指定された検索項目に対応するSQL情報作成
		/// <summary>
		/// 指定された検索項目に対応するSQL情報作成
		/// </summary>
		/// <param name="sp">検索パラメータ</param>
		private void CreateConditionInfo(SearchParameter sp)
		{
			// 検索キーワード取得
			string[] keywords = GetKeywords(sp);
			if (keywords.Length == 0) return;

			// 検索項目対象カラムリスト取得
			List<SearchTargetColumn> columnList = CreateColumnList(sp);

			// キーワード・カラムに対してSQL情報を作成
			List<string> conditions = new List<string>();
			for (int i = 0; i < keywords.Length; i++)
			{
				string keywordName = "keyword" + i;
				this.InputValues.Add(keywordName, "%" + StringUtility.SqlLikeStringSharpEscape(keywords[i]) + "%");
				this.SqlParams.Add(SqlStatement.CreateSqlParam(keywordName, SqlDbType.NVarChar, "MAX"));

				List<string> keywordConditions = new List<string>();
				foreach (var column in columnList)
				{
					keywordConditions.Add(String.Format("{0}.{1} LIKE @{2} ESCAPE '#'", column.TableName, column.ColumnName, keywordName));
				}
				conditions.Add("(" + string.Join(" OR ", keywordConditions.ToArray()) + ")");
			}
			this.WhereStatement = "AND " + JoinKeywordsConditions(sp, conditions);
		}
		#endregion

		#region -GetKeywords キーワードを分割して取得
		/// <summary>
		/// キーワードを分割して取得
		/// </summary>
		/// <param name="sp">検索パラメータ</param>
		/// <returns>キーワード配列</returns>
		private string[] GetKeywords(SearchParameter sp)
		{
			if (sp.Mode == SearchMode.Exact)
			{
				return new string[] { sp.Keyword };	// 完全一致のときはそのまま格納。
			}
			else
			{
				return sp.Keyword.Split(new string[] { " ", "　" }, StringSplitOptions.RemoveEmptyEntries);
			}
		}
		#endregion

		#region -JoinKeywordsConditions キーワード毎の条件を結合（検索モードによって AND ORで結合）
		/// <summary>
		/// キーワード毎の条件を結合（検索モードによって AND ORで結合）
		/// </summary>
		/// <param name="sp">検索パラメータ</param>
		/// <param name="keywordConditions">キーワード毎の条件</param>
		/// <returns>結合した条件</returns>
		private string JoinKeywordsConditions(SearchParameter sp, List<string> keywordConditions)
		{
			switch (sp.Mode)
			{
				case SearchMode.All:
					return "(" + string.Join(" AND ", keywordConditions.ToArray()) + ")";

				case SearchMode.Any:
					return "(" + string.Join(" OR ", keywordConditions.ToArray()) + ")";

				case SearchMode.Exact:
					return keywordConditions[0];
			}
			throw new Exception("未対応の検索モード:" + sp.Mode.ToString());
		}
		#endregion

		#region -CreateConditionInfo 指定された検索項目に対応するSQL情報作成
		/// <summary>
		/// 指定された検索項目に対応するSQL情報作成
		/// </summary>
		/// <param name="sp">検索パラメータ</param>
		private void CreateSummaryValueInfo(SearchParameter sp)
		{
			// 検索対象カラム取得
			SearchTargetColumn columnSummaryNo = SearchTargetColumn.IncidentSummaryValue.SummaryNo;
			SearchTargetColumn columnSummaryValue = SearchTargetColumn.IncidentSummaryValue.SummaryValue;

			// キーワード・カラムに対してSQL情報を作成
			List<string> conditions = new List<string>();
			foreach (int i in sp.SummaryValues.Keys)
			{
				string param1 = "summary_no" + i;
				string param2 = "value" + i;

				string searchValue;
				switch (sp.SummarySettingTypes[i])
				{
					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN:
					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO:
						searchValue = String.Format("{0}.{1} in (select {2} from {3} where ({4} = @{5} AND {6} = @{7}))",
							SearchTargetColumn.Incident.IncidentId.TableName, SearchTargetColumn.Incident.IncidentId.ColumnName,
							SearchTargetColumn.IncidentSummaryValue.IncidentId.ColumnName, SearchTargetColumn.IncidentSummaryValue.IncidentId.TableName,
							columnSummaryNo.ColumnName, param1, columnSummaryValue.ColumnName, param2);
						this.SqlParams.Add(SqlStatement.CreateSqlParam(param1, columnSummaryNo.ColumnDbType, columnSummaryNo.ColumnSize));
						this.SqlParams.Add(SqlStatement.CreateSqlParam(param2, columnSummaryValue.ColumnDbType, columnSummaryValue.ColumnSize));
						this.InputValues.Add(param1, i);
						this.InputValues.Add(param2, sp.SummaryValues[i]);
						break;

					case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT:
						searchValue = String.Format("{0}.{1} in (select {2} from {3} where ({4} = @{5} AND {6} like '%' + @{7} + '%' ESCAPE '#'))",
							SearchTargetColumn.Incident.IncidentId.TableName, SearchTargetColumn.Incident.IncidentId.ColumnName,
							SearchTargetColumn.IncidentSummaryValue.IncidentId.ColumnName, SearchTargetColumn.IncidentSummaryValue.IncidentId.TableName,
							columnSummaryNo.ColumnName, param1, columnSummaryValue.ColumnName, param2);
						this.SqlParams.Add(SqlStatement.CreateSqlParam(param1, columnSummaryNo.ColumnDbType, columnSummaryNo.ColumnSize));
						this.SqlParams.Add(SqlStatement.CreateSqlParam(param2, columnSummaryValue.ColumnDbType, columnSummaryValue.ColumnSize));
						this.InputValues.Add(param1, i);
						this.InputValues.Add(param2, StringUtility.SqlLikeStringSharpEscape(sp.SummaryValues[i]));
						break;

					default:
						throw new Exception("予期しない集計区分種別が指定されました：" + sp.SummarySettingTypes[i]);
				}

				this.WhereStatement += "AND ( " + searchValue + ")";
			}
		}
		#endregion

		#region -GetFilteringSqlParams 指定された追加条件に対応するSQL情報作成
		/// <summary>
		/// 指定された追加条件に対応するSQL情報作成
		/// </summary>
		/// <param name="sp">検索パラメータ</param>
		private void GetFilteringSqlParams(SearchParameter sp)
		{
			this.InputValues.Add("typefilter", (sp.MessageType == SearchMessageTypes.NoSelection) ? "0" : "1");			// タイプで絞り込むか
			this.InputValues.Add("type_mail_in", sp.MessageType.HasFlag(SearchMessageTypes.Receive) ? "1" : "0");		// タイプ：メール受信
			this.InputValues.Add("type_mail_out", sp.MessageType.HasFlag(SearchMessageTypes.Send) ? "1" : "0");			// タイプ：メール送信
			this.InputValues.Add("type_tel_in", sp.MessageType.HasFlag(SearchMessageTypes.TellIn) ? "1" : "0");			// タイプ：電話受信
			this.InputValues.Add("type_tel_out", sp.MessageType.HasFlag(SearchMessageTypes.TellOut) ? "1" : "0");		// タイプ：電話発信
			this.InputValues.Add("type_others_in", sp.MessageType.HasFlag(SearchMessageTypes.OthersIn) ? "1" : "0");	// タイプ：その他受信
			this.InputValues.Add("type_others_out", sp.MessageType.HasFlag(SearchMessageTypes.OthersOut) ? "1" : "0");	// タイプ：その他発信
			this.InputValues.Add(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS, sp.MessageStatusFilter ?? "");		// メッセージステータス
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_STATUS, sp.StatusFilter ?? "");						// ステータス
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID, sp.CategoryFilter ?? "");		// カテゴリ
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_IMPORTANCE, sp.ImportanceFilter ?? "");				// 重要度
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_VOC_ID, sp.VocFilter ?? "");						// VOC
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_CS_GROUP_ID, sp.GroupFilter ?? "");					// 担当グループ
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_OPERATOR_ID, sp.OperatorFilter ?? "");				// 担当オペレータ
			this.InputValues.Add("inquiry_date_after", (sp.DateKbn == SearchDateKbn.InquiryDate) ? sp.DateFromFilter.ToString() : "");
			this.InputValues.Add("inquiry_date_before", ((sp.DateKbn == SearchDateKbn.InquiryDate) && sp.DateToFilter.HasValue) ? sp.DateToFilter.Value.AddDays(1).ToString() : "");
			this.InputValues.Add("latest_inquiry_date_after", (sp.DateKbn == SearchDateKbn.LatestInquiryDate) ? sp.DateFromFilter.ToString() : "");
			this.InputValues.Add("latest_inquiry_date_before", ((sp.DateKbn == SearchDateKbn.LatestInquiryDate) && sp.DateToFilter.HasValue) ? sp.DateToFilter.Value.AddDays(1).ToString() : "");
			this.InputValues.Add("date_changed_after", (sp.DateKbn == SearchDateKbn.MessageDateChanged) ? sp.DateFromFilter.ToString() : "");
			this.InputValues.Add("date_changed_before", ((sp.DateKbn == SearchDateKbn.MessageDateChanged) && sp.DateToFilter.HasValue) ? sp.DateToFilter.Value.AddDays(1).ToString() : "");
			this.InputValues.Add("created_after", (sp.DateKbn == SearchDateKbn.IncidentDateCreated) ? sp.DateFromFilter.ToString() : "");
			this.InputValues.Add("created_before", ((sp.DateKbn == SearchDateKbn.IncidentDateCreated) && sp.DateToFilter.HasValue) ? sp.DateToFilter.Value.AddDays(1).ToString() : "");
			this.InputValues.Add("completed_after", (sp.DateKbn == SearchDateKbn.IncidentDateCompleted) ? sp.DateFromFilter.ToString() : "");
			this.InputValues.Add("completed_before", ((sp.DateKbn == SearchDateKbn.IncidentDateCompleted) && sp.DateToFilter.HasValue) ? sp.DateToFilter.Value.AddDays(1).ToString() : "");
			this.InputValues.Add(Constants.FIELD_CSINCIDENT_VALID_FLG, sp.IncludeTrash ? "" : Constants.FLG_CSINCIDENT_VALID_FLG_VALID);
		}
		#endregion

		#region -CreateColumnList 指定した検索項目に対応するカラムリストの生成
		/// <summary>
		/// 指定した検索項目に対応するカラムリストの生成
		/// </summary>
		/// <param name="sp">検索項目</param>
		/// <returns>検索対象カラムリスト</returns>
		private static List<SearchTargetColumn> CreateColumnList(SearchParameter sp)
		{
			List<SearchTargetColumn> columnList = new List<SearchTargetColumn>();
			switch (sp.Target)
			{
				case SearchTarget.ContentsAndHeader:
					columnList.Add(SearchTargetColumn.ReceiveMail.Body);
					columnList.Add(SearchTargetColumn.SendMail.Body);
					columnList.Add(SearchTargetColumn.Message.InquiryText);
					columnList.Add(SearchTargetColumn.Message.ReplyText);
					columnList.Add(SearchTargetColumn.ReceiveMail.From);
					columnList.Add(SearchTargetColumn.ReceiveMail.To);
					columnList.Add(SearchTargetColumn.ReceiveMail.Cc);
					columnList.Add(SearchTargetColumn.ReceiveMail.Bcc);
					columnList.Add(SearchTargetColumn.ReceiveMail.Subject);
					columnList.Add(SearchTargetColumn.SendMail.From);
					columnList.Add(SearchTargetColumn.SendMail.To);
					columnList.Add(SearchTargetColumn.SendMail.Cc);
					columnList.Add(SearchTargetColumn.SendMail.Bcc);
					columnList.Add(SearchTargetColumn.SendMail.Subject);
					columnList.Add(SearchTargetColumn.Message.Name1);
					columnList.Add(SearchTargetColumn.Message.Name2);
					columnList.Add(SearchTargetColumn.Message.NameKana1);
					columnList.Add(SearchTargetColumn.Message.NameKana2);
					columnList.Add(SearchTargetColumn.Message.MailAddr);
					columnList.Add(SearchTargetColumn.Message.Tel1);
					columnList.Add(SearchTargetColumn.Message.InquiryTitle);
					break;

				case SearchTarget.Contents:
					columnList.Add(SearchTargetColumn.ReceiveMail.Body);
					columnList.Add(SearchTargetColumn.SendMail.Body);
					columnList.Add(SearchTargetColumn.Message.InquiryText);
					columnList.Add(SearchTargetColumn.Message.ReplyText);
					break;

				case SearchTarget.Header:
					columnList.Add(SearchTargetColumn.ReceiveMail.From);
					columnList.Add(SearchTargetColumn.ReceiveMail.To);
					columnList.Add(SearchTargetColumn.ReceiveMail.Cc);
					columnList.Add(SearchTargetColumn.ReceiveMail.Bcc);
					columnList.Add(SearchTargetColumn.ReceiveMail.Subject);
					columnList.Add(SearchTargetColumn.SendMail.From);
					columnList.Add(SearchTargetColumn.SendMail.To);
					columnList.Add(SearchTargetColumn.SendMail.Cc);
					columnList.Add(SearchTargetColumn.SendMail.Bcc);
					columnList.Add(SearchTargetColumn.SendMail.Subject);
					columnList.Add(SearchTargetColumn.Message.Name1);
					columnList.Add(SearchTargetColumn.Message.Name2);
					columnList.Add(SearchTargetColumn.Message.NameKana1);
					columnList.Add(SearchTargetColumn.Message.NameKana2);
					columnList.Add(SearchTargetColumn.Message.MailAddr);
					columnList.Add(SearchTargetColumn.Message.Tel1);
					columnList.Add(SearchTargetColumn.Message.InquiryTitle);
					break;

				case SearchTarget.IncidentId:
					sp.Mode = SearchMode.All;	// インシデントID検索時は検索モード無効
					columnList.Add(SearchTargetColumn.Incident.IncidentId);
					break;

				case SearchTarget.MessageColumns:
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageFrom))
					{
						columnList.Add(SearchTargetColumn.ReceiveMail.From);
						columnList.Add(SearchTargetColumn.SendMail.From);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageTo))
					{
						columnList.Add(SearchTargetColumn.ReceiveMail.To);
						columnList.Add(SearchTargetColumn.SendMail.To);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageCc))
					{
						columnList.Add(SearchTargetColumn.ReceiveMail.Cc);
						columnList.Add(SearchTargetColumn.SendMail.Cc);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageBcc))
					{
						columnList.Add(SearchTargetColumn.ReceiveMail.Bcc);
						columnList.Add(SearchTargetColumn.SendMail.Bcc);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageSubject))
					{
						columnList.Add(SearchTargetColumn.ReceiveMail.Subject);
						columnList.Add(SearchTargetColumn.SendMail.Subject);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageUserName))
					{
						columnList.Add(SearchTargetColumn.Message.Name1);
						columnList.Add(SearchTargetColumn.Message.Name2);
						columnList.Add(SearchTargetColumn.Message.NameKana1);
						columnList.Add(SearchTargetColumn.Message.NameKana2);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageUserTel))
					{
						columnList.Add(SearchTargetColumn.Message.Tel1);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageUserMailAddr))
					{
						columnList.Add(SearchTargetColumn.Message.MailAddr);
					}
					if (sp.TargetMessageColumns.HasFlag(SearchTargetMessageColumns.MessageInquiryTitle))
					{
						columnList.Add(SearchTargetColumn.Message.InquiryTitle);
					}
					break;

				case SearchTarget.IncidentColumns:
					if (sp.TargetIncidentColumns.HasFlag(SearchTargetIncidentColumns.IncidentTitle))
					{
						columnList.Add(SearchTargetColumn.Incident.IncidentTitle);
					}
					if (sp.TargetIncidentColumns.HasFlag(SearchTargetIncidentColumns.IncidentVocMemo))
					{
						columnList.Add(SearchTargetColumn.Incident.VocMemo);
					}
					if (sp.TargetIncidentColumns.HasFlag(SearchTargetIncidentColumns.IncidentComment))
					{
						columnList.Add(SearchTargetColumn.Incident.Comment);
					}
					if (sp.TargetIncidentColumns.HasFlag(SearchTargetIncidentColumns.IncidentFrom))
					{
						columnList.Add(SearchTargetColumn.Incident.UserName);
						columnList.Add(SearchTargetColumn.Incident.UserContact);
					}
					break;

				case SearchTarget.SearchTop:
					columnList.Add(SearchTargetColumn.Incident.IncidentId);
					columnList.Add(SearchTargetColumn.Incident.IncidentTitle);
					columnList.Add(SearchTargetColumn.Message.Name1);
					columnList.Add(SearchTargetColumn.Message.Name2);
					columnList.Add(SearchTargetColumn.ReceiveMail.Subject);
					columnList.Add(SearchTargetColumn.ReceiveMail.Body);
					columnList.Add(SearchTargetColumn.ReceiveMail.To);
					columnList.Add(SearchTargetColumn.ReceiveMail.From);
					columnList.Add(SearchTargetColumn.ReceiveMail.Cc);
					columnList.Add(SearchTargetColumn.SendMail.Subject);
					columnList.Add(SearchTargetColumn.SendMail.Body);
					columnList.Add(SearchTargetColumn.SendMail.To);
					columnList.Add(SearchTargetColumn.SendMail.From);
					columnList.Add(SearchTargetColumn.SendMail.Cc);
					break;

				default:
					break;
			}
			return columnList;
		}
		#endregion

		#region -ReplaceStatementSortForIncident Replace statement sort for incident
		/// <summary>
		/// Replace statement sort for incident
		/// </summary>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="statement">Sql statement</param>
		public static void ReplaceStatementSortForIncident(string sortField, string sortType, SqlStatement statement)
		{
			var statementCreateTableTemp = new StringBuilder();
			var statementJoinTableTemp = new StringBuilder();

			if (sortField == "EX_InquirySource")
			{
				// Create Statement Create Table Temp For User Kbn
				statementCreateTableTemp.Append(CreateStatementCreateTableTemp(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN));

				// Create Statement Join Table Temp For User Kbn
				statementJoinTableTemp.Append(CreateStatementJoinTableTemp(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN));
			}

			if (sortField == "EX_StatusText")
			{
				// Create Statement Create Table Temp For Incident Status
				statementCreateTableTemp.Append(CreateStatementCreateTableTemp(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS));

				// Create Statement Join Table Temp For Incident Status
				statementJoinTableTemp.Append(CreateStatementJoinTableTemp(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS));
			}

			statement.ReplaceStatement("@@ table_temp @@", statementCreateTableTemp.ToString());
			statement.ReplaceStatement("@@ join_table_temp @@", statementJoinTableTemp.ToString());
			statement.ReplaceStatement("@@ orderby @@", CreateStatementOrderByForIncident(sortField, sortType));
			statement.ReplaceStatement("@@ groupby @@", CreateStatementGroupByForIncident(sortField, sortType));
		}
		#endregion

		#region -CreateStatementOrderByForIncident Create statement order by for incident
		/// <summary>
		/// Create statement order by for incident
		/// </summary>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>Statement order by</returns>
		private static string CreateStatementOrderByForIncident(string sortField, string sortType)
		{
			var fieldOderBy = new StringBuilder();
			switch (sortField)
			{
				// インシデントID
				case "IncidentId":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_INCIDENT_ID));
					break;

				// タイトル
				case "IncidentTitle":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_INCIDENT_TITLE));
					break;

				// 問合せ元
				case "EX_InquirySource":
					// If user id is empty then (if user name of incident is empty then user contact else user name)
					fieldOderBy.Append(string.Format("CASE WHEN {0}.{1} = '' THEN CASE WHEN {0}.{2} = '' THEN {0}.{3} ELSE {0}.{2} END ",
						Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_USER_ID, Constants.FIELD_CSINCIDENT_USER_NAME, Constants.FIELD_CSINCIDENT_USER_CONTACT));

					// Else (if user name not empty then user name)
					fieldOderBy.Append(string.Format("ELSE (CASE WHEN ISNULL({0}.{1},'') <> '' THEN {0}.{1} ", Constants.TABLE_USER, Constants.FIELD_USER_NAME));

					// Else (if user mail addr not empty then user mail addr)
					fieldOderBy.Append(string.Format("ELSE CASE WHEN ISNULL({0}.{1},'') <> '' THEN {0}.{1} ", Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR));

					// Else (if user mail addr2 not empty then user mail addr2)
					fieldOderBy.Append(string.Format("ELSE CASE WHEN ISNULL({0}.{1},'') <> '' THEN {0}.{1} ", Constants.TABLE_USER, Constants.FIELD_USER_MAIL_ADDR2));

					// Else (if user tel1 not empty then user tel1)
					fieldOderBy.Append(string.Format("ELSE CASE WHEN ISNULL({0}.{1},'') <> '' THEN {0}.{1} ", Constants.TABLE_USER, Constants.FIELD_USER_TEL1));

					// Else (if user tel2 not empty then user tel2)
					fieldOderBy.Append(string.Format("ELSE ISNULL({0}.{1},'') END ",
						Constants.TABLE_USER, Constants.FIELD_USER_TEL2));
					fieldOderBy.Append("END END END ");

					// If user id not empty + user kbn text
					fieldOderBy.Append(string.Format(" + ' [' + ISNULL({0}_text_temp,'') + ']') END", Constants.FIELD_USER_USER_KBN));
					break;

				// カテゴリ
				case "EX_IncidentCategoryName":
					fieldOderBy.Append(string.Format("ISNULL({0}.{1},'')", Constants.TABLE_CSINCIDENTCATEGORY, Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME));
					break;

				// ステータス
				case "EX_StatusText":
					fieldOderBy.Append(string.Format("{0}_text_temp", Constants.FIELD_CSINCIDENT_STATUS));
					break;

				// 担当
				case "EX_CsOperatorName":
					fieldOderBy.Append("operator.name");
					break;

				// 受付日時
				case "DateLastReceived":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_DATE_LAST_RECEIVED));
					break;

				// 送受信日時
				case "DateMessageLastSendReceived":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_DATE_MESSAGE_LAST_SEND_RECEIVED));
					break;

				// 更新日時
				case "DateChanged":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_DATE_CHANGED));
					break;

				// グループ
				case "EX_CsGroupName":
					fieldOderBy.Append(Constants.FIELD_CSGROUP_CS_GROUP_NAME);
					break;
			}

			return string.Format("ORDER BY {0} {1}", fieldOderBy.ToString(), sortType);
		}
		#endregion

		#region -CreateStatementGroupByForIncident Create statement group by for incident
		/// <summary>
		/// Create statement order by for incident
		/// </summary>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>Statement order by</returns>
		private static string CreateStatementGroupByForIncident(string sortField, string sortType)
		{
			var fieldGroupBy = new StringBuilder();
			switch (sortField)
			{
				// タイトル
				case "IncidentTitle":
					fieldGroupBy.Append(string.Format(", {0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_INCIDENT_TITLE));
					break;

				// 問合せ元
				case "EX_InquirySource":
					fieldGroupBy.Append(string.Format(", {0}.{1}, {0}.{2}, {0}.{3}, {4}.{5}, {4}.{6}, {4}.{7}, {4}.{8}, {4}.{9}, {10}_text_temp",
						Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_USER_ID, Constants.FIELD_CSINCIDENT_USER_NAME, Constants.FIELD_CSINCIDENT_USER_CONTACT,
						Constants.TABLE_USER, Constants.FIELD_USER_NAME, Constants.FIELD_USER_MAIL_ADDR, Constants.FIELD_USER_MAIL_ADDR2, Constants.FIELD_USER_TEL1,
						Constants.FIELD_USER_TEL2, Constants.FIELD_USER_USER_KBN));
					break;

				// カテゴリ
				case "EX_IncidentCategoryName":
					fieldGroupBy.Append(string.Format(", {0}.{1}", Constants.TABLE_CSINCIDENTCATEGORY, Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME));
					break;

				// ステータス
				case "EX_StatusText":
					fieldGroupBy.Append(string.Format(", {0}_text_temp", Constants.FIELD_CSINCIDENT_STATUS));
					break;

				// 担当
				case "EX_CsOperatorName":
					fieldGroupBy.Append(", operator.name");
					break;

				// 受付日時
				case "DateLastReceived":
					fieldGroupBy.Append(string.Format(", {0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_DATE_LAST_RECEIVED));
					break;

				// 更新日時
				case "DateChanged":
					fieldGroupBy.Append(string.Format(", {0}.{1}", Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_DATE_CHANGED));
					break;
			}

			return fieldGroupBy.ToString();
		}
		#endregion

		#region -ReplaceStatementSortForMessage Replace statement sort for message
		/// <summary>
		/// Replace statement sort for message
		/// </summary>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="statement">Sql statement</param>
		/// <param name="isSetExMail">Is set EX_Mail</param>
		public static void ReplaceStatementSortForMessage(string sortField, string sortType, SqlStatement statement, bool isSetExMail = true)
		{
			var statementCreateTableTemp = new StringBuilder();
			var statementJoinTableTemp = new StringBuilder();

			if (sortField == "EX_MessageStatusName")
			{
				// Create Statement Create Table Temp For Message Status
				statementCreateTableTemp.Append(CreateStatementCreateTableTemp(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_MESSAGE_STATUS));

				// Create Statement Join Table Temp For Message Status
				statementJoinTableTemp.Append(CreateStatementJoinTableTemp(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_MESSAGE_STATUS));
			}

			statement.ReplaceStatement("@@ table_temp @@", statementCreateTableTemp.ToString());
			statement.ReplaceStatement("@@ join_table_temp @@", statementJoinTableTemp.ToString());
			statement.ReplaceStatement("@@ orderby @@", CreateStatementOrderByForMessage(sortField, sortType, isSetExMail));
		}
		#endregion

		#region -CreateStatementOrderByForMessage Create statement order by for message
		/// <summary>
		/// Create statement order by for message
		/// </summary>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="isSetExMail">Is set EX_Mail</param>
		/// <returns>Statement order by</returns>
		private static string CreateStatementOrderByForMessage(string sortField, string sortType, bool isSetExMail)
		{
			var fieldOderBy = new StringBuilder();
			switch (sortField)
			{
				// インシデントID
				case "IncidentId":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_INCIDENT_ID));
					break;

				// 件名
				case "InquiryTitle":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_INQUIRY_TITLE));
					break;

				// 差出人元
				case "EX_Sender":
					if (isSetExMail)
					{
						// If receive mail id not empty then get mail from csmessage table with mail id = receive mail id
						fieldOderBy.Append(string.Format("CASE WHEN {0}.{1} <> '' THEN ReceiveMail.{2} ",
							Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_RECEIVE_MAIL_ID,
							Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM));

						// If send mail id not empty then get mail from csmessage table with mail id = send mail id
						fieldOderBy.Append(string.Format("WHEN {0}.{1} <> '' THEN SendMail.{2} ",
							Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_SEND_MAIL_ID,
							Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM));
					}
					else
					{
						fieldOderBy.Append("CASE ");
					}

					// If receive mail id is empty and send mail id empty then get user name 1 + user name 2
					fieldOderBy.Append(string.Format("WHEN ({0}.{1} + {0}.{2}) <> '' THEN ({0}.{1} + {0}.{2}) ELSE {0}.{3} END ",
						Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_NAME1, Constants.FIELD_CSMESSAGE_USER_NAME2, Constants.FIELD_CSMESSAGE_USER_TEL1));
					break;

				// 対応者/作成者
				case "EX_NameOperator":
					fieldOderBy.Append("CASE WHEN reply_operator.name = '' THEN create_operator.name ELSE reply_operator.name END");
					break;

				// メッセージステータス
				case "EX_MessageStatusName":
					fieldOderBy.Append(string.Format("{0}_text_temp", Constants.FIELD_CSMESSAGE_MESSAGE_STATUS));
					break;

				// 更新日時
				case "DateChanged":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_DATE_CHANGED));
					break;

				// 回答日時
				case "InquiryReplyDate":
					fieldOderBy.Append(string.Format("ISNULL({0}.{1},NULL)", Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE));
					break;

				// 依頼日時
				case "EX_Request":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSMESSAGEREQUEST, Constants.FIELD_CSMESSAGEREQUEST_DATE_CREATED));
					break;

				// 作成日
				case "DateCreated":
					fieldOderBy.Append(string.Format("{0}.{1}", Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_DATE_CREATED));
					break;

				// Reply Changed Date
				case "ReplyChangedDate":
					fieldOderBy.Append("ReplyChangedDate");
					break;

				// Date Send Or Receive
				case "DateSendOrReceive":
					fieldOderBy.Append("DateSendOrReceive");
					break;

				// Operator Name
				case "OperatorName":
					fieldOderBy.Append("operator_name");
					break;
			}

			return string.Format("ORDER BY {0} {1}", fieldOderBy, sortType);
		}
		#endregion

		#region -CreateStatementCreateTableTemp Create statement create table temp
		/// <summary>
		/// Create statement create table temp
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <param name="fieldName">Field name</param>
		/// <returns>Statement create table temp</returns>
		private static StringBuilder CreateStatementCreateTableTemp(string tableName, string fieldName)
		{
			// Create statement for table temporary
			var statementCreateTableStatusTemp = new StringBuilder();
			statementCreateTableStatusTemp.Append(string.Format("DECLARE @table_{0}_temp TABLE({0}_temp nvarchar (10) NOT NULL, {0}_text_temp nvarchar (100) NOT NULL DEFAULT(''))", fieldName));

			// Get value from value text
			foreach (var status in ValueText.GetValueItemArray(tableName, fieldName))
			{
				statementCreateTableStatusTemp.Append(string.Format("INSERT INTO @table_{0}_temp VALUES('{1}', '{2}') ", fieldName, status.Value, status.Text));
			}

			return statementCreateTableStatusTemp.Append(";");
		}
		#endregion

		#region -CreateStatementJoinTableTemp Create statement join table temp
		/// <summary>
		/// Create statement join table temp
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <param name="fieldName">Field name</param>
		/// <returns>Statement create table temp</returns>
		private static string CreateStatementJoinTableTemp(string tableName, string fieldName)
		{
			return string.Format("  LEFT JOIN @table_{0}_temp ON ({0}_temp = {1}.{0})",
				fieldName, tableName);
		}
		#endregion

		/// <summary>
		/// Replace statement search keyword
		/// </summary>
		/// <param name="statement">Statement</param>
		/// <param name="searchParameter">Search parameter</param>
		public void ReplaceStatementSearchKeyWord(SqlStatement statement, Hashtable input, SearchParameter searchParameter)
		{
			if (searchParameter != null)
			{
				foreach (SqlStatement.SqlParam param in this.SqlParams)
				{
					statement.AddInputParameters(param.Name, param.Type, param.Size);
					input.Add(param.Name, this.InputValues[param.Name]);
				}

				statement.ReplaceStatement("@@ keywords_search @@", "[[ SearchMessageKeyword ]]");
				statement.ReplaceStatement("@@ keywords_where @@", this.WhereStatement);
			}
			else
			{
				statement.ReplaceStatement("@@ keywords_search @@", string.Empty);
			}
		}

		#region プロパティ
		/// <summary>SQLステートメント</summary>
		public string WhereStatement { get; private set; }
		/// <summary>SQLパラメタ構造体のリスト</summary>
		public List<SqlStatement.SqlParam> SqlParams { get; private set; }
		/// <summary>SQLパラメタに指定する入力値のリスト</summary>
		public Hashtable InputValues { get; private set; }
		#endregion
	}
}
