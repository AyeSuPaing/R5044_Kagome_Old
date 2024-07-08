/*
=========================================================================================================
  Module      : 注文関連ファイル取込処理(ImportPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// 注文関連ファイル取込処理
	/// </summary>
	public class ImportPayment
	{
		/// <summary>取込ステータス</summary>
		private enum ImportStatus
		{
			/// <summary>ファイル取込完了</summary>
			ReadComplete,
			/// <summary>取込ファイルの列数過不足</summary>
			ColumnCountError,
			/// <summary>取込ファイルの入金日が不正</summary>
			PaymentDateError,
			/// <summary>取込ファイルの項目の型が不正</summary>
			ValueTypeError,
			/// <summary>取込ファイルに項目の不正なし</summary>
			FormatChecked,
			/// <summary>DBに更新対象なし</summary>
			NoRecordError,
			/// <summary>DBに更新対象複数</summary>
			MultiRecordError,
			/// <summary>ファイルとDBの照合時にエラー</summary>
			MatchingError,
			/// <summary>入金ステータスが不正</summary>
			PaymentStatusError,
			/// <summary>取込対象外として判断された行</summary>
			ExcludeRecord,
			/// <summary>ヘッダ行として判断された行</summary>
			HeaderRecord,
			/// <summary>フッタ行として判断された行</summary>
			FooterRecord,
			/// <summary>入金ステータス更新可能（データチェック完了）</summary>
			DataChecked
		}

		// データ型文言定数
		private const string TYPE_TEXT = "text";
		private const string TYPE_NUMBER = "number";
		private const string TYPE_DATE = "date";

		/// <summary>メンバ変数</summary>
		private static ImportSetting m_xmlSetting;

		/// <summary>負荷軽減設定</summary>
		private const int ERROR_DISPLAY_COUNT = 100; // 画面に表示するエラーの上限件数

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="serviceId">取込ファイル種別</param>
		public ImportPayment(string serviceId)
		{
			// 取込定義を取得
			GetImportSetting(serviceId);
		}

		/// <summary>
		/// 指定の取込ファイル種別に従ってクラス変数に取込定義を保存する
		/// </summary>
		/// <param name="serviceId">取込ファイル種別</param>
		public void GetImportSetting(string serviceId)
		{
			// 取込設定
			ImportSetting xmlSetting = new ImportSetting();

			// 設定ファイルの読み込み
			XElement mainElement = XElement.Load(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);
			var serviceNode = from serviceNodes in mainElement.Elements("OrderFile")
							  where serviceNodes.Element("Value").Value == serviceId
							  select new
							  {
								  importFileSetting = serviceNodes.Elements("ImportFileSetting").ToDictionary(node => node.Attribute("key").Value, node => node.Attribute("value").Value),
								  whereConditions = serviceNodes.Element("WhereConditions"),
								  matchConditions = serviceNodes.Element("MatchConditions"),
								  excludeConditions = serviceNodes.Element("ExcludeConditions")
							  };

			// 取込ファイルの基本設定
			Dictionary<string, string> importFileSettings = serviceNode.First().importFileSetting;
			// 入金日の列番号設定はstringをスプリット「,」してint[]へ
			xmlSetting.paymentDateColumnNo = Array.ConvertAll<string, int>(importFileSettings["PaymentDateColumnNo"].Split(','), delegate(string value) { return int.Parse(value); });
			xmlSetting.headerRowCount = int.Parse(importFileSettings["HeaderRowCount"]);
			xmlSetting.footerRowCount = int.Parse(importFileSettings["FooterRowCount"]);
			xmlSetting.columnCount = int.Parse(importFileSettings["ColumnCount"]);
			xmlSetting.pastMonths = int.Parse(importFileSettings["PastMonths"]);
			xmlSetting.allowMultiRecordUpdate = bool.Parse(importFileSettings["AllowMultiRecordUpdate"]);
			xmlSetting.noRecordExclude = bool.Parse(importFileSettings["NoRecordExclude"]);

			// DB検索キー定義（whereConditions）
			List<ImportSetting.ConditionItem> whereCondition = new List<ImportSetting.ConditionItem>();
			foreach (XElement whereElement in serviceNode.First().whereConditions.Elements("Item"))
			{
				ImportSetting.ConditionItem conditionItem = new ImportSetting.ConditionItem();
				conditionItem.name = whereElement.Attribute("name").Value;
				conditionItem.columnNo = int.Parse(whereElement.Attribute("columnNo").Value);
				conditionItem.columnType = whereElement.Attribute("columnType").Value;
				conditionItem.fieldName = whereElement.Attribute("field").Value;
				whereCondition.Add(conditionItem);
			}
			xmlSetting.whereCondition = whereCondition;

			// DB検索キー定義（matchConditions）
			List<ImportSetting.ConditionItem> matchCondition = new List<ImportSetting.ConditionItem>();
			foreach (XElement matchElement in serviceNode.First().matchConditions.Elements("Item"))
			{
				ImportSetting.ConditionItem conditionItem = new ImportSetting.ConditionItem();
				conditionItem.name = matchElement.Attribute("name").Value;
				conditionItem.columnNo = int.Parse(matchElement.Attribute("columnNo").Value);
				conditionItem.columnType = matchElement.Attribute("columnType").Value;
				conditionItem.fieldName = matchElement.Attribute("field").Value;
				matchCondition.Add(conditionItem);
			}
			xmlSetting.matchCondition = matchCondition;

			// 除外設定情報を読込む
			List<ImportSetting.ConditionItem> excludeCondition = new List<ImportSetting.ConditionItem>();
			foreach (XElement excludeElement in serviceNode.First().excludeConditions.Elements("Item"))
			{
				ImportSetting.ConditionItem conditionItem = new ImportSetting.ConditionItem();
				conditionItem.mode = excludeElement.Attribute("mode").Value;
				conditionItem.columnNo = int.Parse(excludeElement.Attribute("columnNo").Value);
				conditionItem.columnType = excludeElement.Attribute("columnType").Value;
				conditionItem.value = excludeElement.Attribute("value").Value;
				excludeCondition.Add(conditionItem);
			}
			xmlSetting.excludeCondition = excludeCondition;

			m_xmlSetting = xmlSetting;
		}

		/// <summary>
		/// ファイル名のチェック
		/// </summary>
		/// <param name="fileNamePattern">ファイル名として有効なパターン</param>
		/// <param name="fileName">ファイル名</param>
		/// <returns>判定結果</returns>
		public static bool CheckFileName(string fileNamePattern, string fileName)
		{
			return (fileNamePattern == "") || Regex.IsMatch(fileName, GetRegexString(fileNamePattern));
		}

		/// <summary>
		/// ファイル取込・入金消込処理
		/// </summary>
		/// <param name="fileStream">csvファイルの中身</param>
		/// <param name="operatorName">ログインオペレータ名</param>
		/// <param name="fileName">取込ファイル名称</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>実行結果</returns>
		public bool Import(StreamReader fileStream, string operatorName, string fileName, UpdateHistoryAction updateHistoryAction)
		{
			this.ErrorList = new List<Dictionary<string, string>>();
			this.ErrorMessage = "";

			// csvファイルからデータ取得
			List<PaymentItem> paymentData = ReadCsvData(fileStream);

			// 入金データとして読み込まれた行があるかチェック
			if (paymentData.Exists(item => (item.status == ImportStatus.ReadComplete)) == false)
			{
				this.ErrorMessage = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_DATA_EMPTY);
				return false;
			}

			// 取込ファイルデータをチェック
			CheckFormat(paymentData);

			// 除外データチェック
			CheckExcludeData(paymentData);

			// 注文データ取得＆データチェック
			CheckData(paymentData);

			// エラー表示データを構成
			CreateErrorDisplayData(paymentData);

			// 除外したレコードをファイル出力
			CreateExcludeRecordsFile(paymentData, fileName);

			// 入金ステータス更新（更新履歴とともに）
			if (this.ErrorList.Count == 0)
			{
				this.UpdatedCount = UpdatePaymentStatus(paymentData, operatorName, updateHistoryAction);
			}
			this.LinesCount = paymentData.Count(
				item => ((item.status != ImportStatus.HeaderRecord)
					&& (item.status != ImportStatus.FooterRecord)));

			return (this.ErrorList.Count == 0);
		}

		/// <summary>
		/// ファイルの内容をファイル取込クラスに格納する
		/// </summary>
		/// <param name="fileStream">ファイルの中身</param>
		/// <returns>取込ファイルデータ</returns>
		private List<PaymentItem> ReadCsvData(StreamReader fileStream)
		{
			List<PaymentItem> paymentData = new List<PaymentItem>();
			int readLineCount = 0;
			while (fileStream.Peek() > -1)
			{
				readLineCount++;

				string[] csvData = StringUtility.SplitCsvLine(fileStream.ReadLine());
				PaymentItem paymentItem = new PaymentItem();
				paymentItem.rowNo = readLineCount;
				paymentItem.csvData = csvData;
				paymentItem.status = (readLineCount > m_xmlSetting.headerRowCount) ? ImportStatus.ReadComplete : ImportStatus.HeaderRecord; // ヘッダ行ならヘッダステータスへ

				paymentData.Add(paymentItem);
			}

			for (int i = Math.Max(0, paymentData.Count - m_xmlSetting.footerRowCount); i < paymentData.Count; i++)
			{
				paymentData[i].status = ImportStatus.FooterRecord; // フッタ行ならフッタステータスへ
			}

			return paymentData;
		}

		/// <summary>
		/// 取込ファイルのみで確認できるエラーのチェックを行う
		/// </summary>
		/// <param name="paymentData">取込ファイルのデータ</param>
		private void CheckFormat(List<PaymentItem> paymentData)
		{
			foreach (PaymentItem paymentItem in paymentData.Where(item => (item.status == ImportStatus.ReadComplete)))
			{
				// 列数の過不足をチェックする
				if (paymentItem.csvData.Length != m_xmlSetting.columnCount)
				{
					paymentItem.status = ImportStatus.ColumnCountError;
					continue;
				}

				// 取込ファイル列数より大きい列番が指定されているかチェックする
				if ((m_xmlSetting.whereCondition.Where(item => item.columnNo > paymentItem.csvData.Length).Count() > 0)
					|| (m_xmlSetting.matchCondition.Where(item => item.columnNo > paymentItem.csvData.Length).Count() > 0))
				{
					paymentItem.status = ImportStatus.ColumnCountError;
					continue;
				}

				// 入金日をフォーマット通りの変換を試みてエラーを検出する
				// 入金日カラムが複数指定されていれば順番に連結
				paymentItem.paymentDateString = string.Join("", Array.ConvertAll<int, string>(m_xmlSetting.paymentDateColumnNo,
					delegate(int value) { return paymentItem.csvData[value - 1]; }));
				if (TryCast(paymentItem.paymentDateString, TYPE_DATE) == false)
				{
					paymentItem.status = ImportStatus.PaymentDateError;
					continue;
				}

				// Where部分、Match部分の型チェックを行う
				if ((m_xmlSetting.whereCondition.Where(item => TryCast(paymentItem.csvData[item.columnNo - 1], item.columnType) == false).Count() > 0)
					|| (m_xmlSetting.matchCondition.Where(item => TryCast(paymentItem.csvData[item.columnNo - 1], item.columnType) == false).Count() > 0))
				{
					paymentItem.status = ImportStatus.ValueTypeError;
					continue;
				}

				paymentItem.status = ImportStatus.FormatChecked;
			}
		}

		/// <summary>
		/// 取込ファイル内から処理対象外の行か判定する
		/// </summary>
		/// <param name="paymentData">取込ファイル内容</param>
		private void CheckExcludeData(List<PaymentItem> paymentData)
		{
			foreach (PaymentItem paymentItem in paymentData.Where(item => (item.status == ImportStatus.FormatChecked)))
			{
				foreach (ImportSetting.ConditionItem excludeCondition in m_xmlSetting.excludeCondition)
				{
					// 除外設定が設定されている場合、除外条件と一致するかどうかチェックする
					switch (excludeCondition.mode)
					{
						case "equal":
							if (paymentItem.csvData[excludeCondition.columnNo - 1].Trim() == excludeCondition.value.Trim())
							{
								paymentItem.status = ImportStatus.ExcludeRecord;
							}
							break;

						case "multi":
							if (paymentData.FindAll(item => (item.csvData[excludeCondition.columnNo - 1] == paymentItem.csvData[excludeCondition.columnNo - 1])).Count() > 1)
							{
								paymentItem.status = ImportStatus.ExcludeRecord;
							}
							break;

						default:
							break;
					}
				}
			}
		}

		/// <summary>
		/// DBからデータを取得し照合する
		/// </summary>
		/// <param name="paymentData">取込ファイル内容</param>
		private void CheckData(List<PaymentItem> paymentData)
		{
			// 追加SQL文とDB検索パラメータを設定し検索を行う
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("PaymentFileImport", "GetOrderShipping"))
			{
				// SQL作成
				StringBuilder selectStatement = new StringBuilder();
				StringBuilder whereStatement = new StringBuilder();
				foreach (ImportSetting.ConditionItem conditionItem in m_xmlSetting.matchCondition)
				{
					selectStatement.Append(", ").Append(conditionItem.fieldName);
				}
				foreach (ImportSetting.ConditionItem conditionItem in m_xmlSetting.whereCondition)
				{
					string paramName = conditionItem.fieldName.Replace('.', '_');
					whereStatement.Append(" AND ").Append(conditionItem.fieldName).Append(" = @").Append(paramName);
					sqlStatement.AddInputParameters(paramName, GetDbFieldType(conditionItem.columnType));
				}
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ select @@", selectStatement.ToString());
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where @@", whereStatement.ToString());

				// エラーを除いたデータでループ
				foreach (PaymentItem paymentItem in paymentData.Where(item => (item.status == ImportStatus.FormatChecked)))
				{
					// パラメータ作成
					Hashtable htParam = new Hashtable();
					foreach (ImportSetting.ConditionItem conditionItem in m_xmlSetting.whereCondition)
					{
						string paramName = conditionItem.fieldName.Replace('.', '_');
						htParam.Add(paramName, CastFieldData(paymentItem.csvData[conditionItem.columnNo - 1], conditionItem.columnType));
					}
					htParam.Add("order_date_from", DateTime.Now.Date.AddMonths(-1 * m_xmlSetting.pastMonths));

					DataView orderData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);

					// 注文データの件数チェック
					if (orderData.Count > 0)
					{
						paymentItem.orderData = orderData;
						// 受注IDをカンマ区切りで格納
						foreach (DataRowView drv in orderData)
						{
							if (paymentItem.orderIdList == null) paymentItem.orderIdList = new List<string>();
							paymentItem.orderIdList.Add((string)drv[Constants.FIELD_ORDER_ORDER_ID]);
						}
						// 複数レコードエラー
						if ((m_xmlSetting.allowMultiRecordUpdate == false) && (orderData.Count > 1))
						{
							paymentItem.status = ImportStatus.MultiRecordError;
						}
					}
					else
					{
						// 0件の場合の除外設定がTRUEなら除外判定、そうでないならレコードなしエラーとして設定
						paymentItem.status = m_xmlSetting.noRecordExclude ? ImportStatus.ExcludeRecord : ImportStatus.NoRecordError;
					}
				}
			}

			// エラーを除いたデータでループ
			foreach (PaymentItem paymentItem in paymentData.Where(item => (item.status == ImportStatus.FormatChecked)))
			{
				// データマッチングチェック
				if (m_xmlSetting.matchCondition.Where(item => CompareData(paymentItem, item) == false).Count() > 0)
				{
					paymentItem.status = ImportStatus.MatchingError;
					continue;
				}

				// 入金ステータスチェック
				if (CheckPaymentStatus(paymentItem) == false)
				{
					paymentItem.status = ImportStatus.PaymentStatusError;
					continue;
				}

				// ひとつもエラーが無い場合
				paymentItem.status = ImportStatus.DataChecked;
			}
		}

		/// <summary>
		/// エラー一覧画面表示用データを構成する
		/// </summary>
		/// <param name="paymentData">取込データ</param>
		private void CreateErrorDisplayData(List<PaymentItem> paymentData)
		{
			// エラーデータでループ（読み取りステータスが右のいずれにも一致しない場合。DataChecked、ExcludeRecord、HeaderRecord、FooterRecord）
			foreach (PaymentItem paymentItem in paymentData.Where(
				item => ((item.status != ImportStatus.DataChecked) && (item.status != ImportStatus.ExcludeRecord) && (item.status != ImportStatus.HeaderRecord) && (item.status != ImportStatus.FooterRecord))))
			{
				// エラー表示件数の限度以上はエラーデータを構成しない
				if (this.ErrorList.Count >= ERROR_DISPLAY_COUNT) break;

				Dictionary<string, string> errorData = new Dictionary<string, string>();
				errorData.Add("行", paymentItem.rowNo.ToString());

				// DB検索条件部分を構成
				foreach (ImportSetting.ConditionItem item in m_xmlSetting.whereCondition)
				{
					errorData.Add(item.name, paymentItem.status != ImportStatus.ColumnCountError ? paymentItem.csvData[item.columnNo - 1] : "");
				}

				// 入金日
				errorData.Add("入金日", paymentItem.status != ImportStatus.ColumnCountError ? paymentItem.paymentDateString : "");

				// エラー内容
				errorData.Add("エラー内容", GetErrorMessage(paymentItem));
				this.ErrorList.Add(errorData);
			}

			// エラーデータがある場合はエラー件数メッセージを作成する
			if (this.ErrorList.Count > 0)
			{
				StringBuilder errorMessage = new StringBuilder();
				errorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ERROR_EXIST).Replace("@@ 1 @@", this.ErrorList.Count.ToString()).Replace("@@ 2 @@", (this.ErrorList.Count == ERROR_DISPLAY_COUNT) ? "以上" : ""));
				// 表示件数オーバーのメッセージ
				if (this.ErrorList.Count == ERROR_DISPLAY_COUNT)
				{
					errorMessage.Append("<br />");
					errorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ERROR_RECORD_OVERLIMIT).Replace("@@ 1 @@", ERROR_DISPLAY_COUNT.ToString()));
				}
				this.ErrorMessage = errorMessage.ToString();
			}
		}

		/// <summary>
		/// ステータスを入金済みに更新
		/// </summary>
		/// <param name="paymentData">取込データ</param>
		/// <param name="operatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数（処理行数）</returns>
		private int UpdatePaymentStatus(List<PaymentItem> paymentData, string operatorName, UpdateHistoryAction updateHistoryAction)
		{
			int resultCount = 0;
			// 入金ステータス更新SQL実行
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				foreach (PaymentItem paymentItem in paymentData.Where(item => item.status == ImportStatus.DataChecked))
				{
					int drvResult = 0;
					// 複数受注の一括更新も考慮
					foreach (DataRowView drv in paymentItem.orderData)
					{
						drvResult += new OrderService().UpdatePaymentStatus(
							(string)drv[Constants.FIELD_ORDER_ORDER_ID],
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
							CastShortDatetime(paymentItem.paymentDateString),
							operatorName,
							updateHistoryAction,
							sqlAccessor);

						// Update FixPurchaseMemberFlag By Settings
						if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE && (drvResult > 0))
						{
							var userOrder = new OrderService().Get((string)drv[Constants.FIELD_ORDER_ORDER_ID], sqlAccessor);
							if (userOrder.IsFixedPurchaseOrder)
							{
								new UserService().UpdateFixedPurchaseMemberFlg(
								userOrder.UserId,
								Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
								operatorName,
								UpdateHistoryAction.DoNotInsert);
							}
						}
					}

					// HACK: 複数の対象を更新する場合、処理行カウント数のほか、UPDATE件数も画面に表示させたい
					// １レコードでも更新したら、更新件数をカウント１する（CSVの対象行が分母になるのに対し、分子の更新行が多くならないようにする調整処理）
					if (drvResult > 0)
					{
						resultCount++;
					}
				}
			}
			return resultCount;
		}

		/// <summary>
		/// ファイル名のフォーマット文字列を作成する
		/// </summary>
		/// <param name="formatString">フォーマット文字列</param>
		/// <returns>正規表現</returns>
		private static string GetRegexString(string formatString)
		{
			StringBuilder regexPattern = new StringBuilder();

			regexPattern.Append("^");
			foreach (char formatChar in formatString)
			{
				// 正規表現の文字に変換
				switch (formatChar)
				{
					case '*': // ワイルドカード
						regexPattern.Append(".*");
						break;

					case '?': // ワイルドカード(1文字)
						regexPattern.Append(".");
						break;

					default:
						regexPattern.Append("[").Append(formatChar).Append("]");
						break;
				}
			}
			regexPattern.Append("$");
			return regexPattern.ToString();
		}

		/// <summary>
		/// エラー内容を画面表示文言にして返す
		/// </summary>
		/// <param name="paymentItem">取込ファイル1行分</param>
		/// <returns>画面表示文言</returns>
		private string GetErrorMessage(PaymentItem paymentItem)
		{
			// 画面表示文言
			StringBuilder displayErrorMessage = new StringBuilder();
			// エラーデータ表示名と値リスト
			List<List<string>> errorValuesList = new List<List<string>>();

			// ステータスに合わせて表示用エラーメッセージを作成する
			// ※DB抽出後に検出したエラーは、DBから抽出した受注のリンクを表示する

			switch (paymentItem.status)
			{
				// 取込ファイルの入金日が不正
				case ImportStatus.PaymentDateError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_DATE_ERROR).Replace("@@ 1 @@", ValueText.GetValueText(Constants.TABLE_ORDER, "update_status", "order_payment_date")));
					break;

				// 取込ファイルの列数過不足
				case ImportStatus.ColumnCountError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_COLUMN_COUNT_ERROR).Replace("@@ 1 @@", m_xmlSetting.columnCount.ToString()).Replace("@@ 2 @@", paymentItem.csvData.Length.ToString()));
					break;

				// 取込ファイルの項目の型が不正
				case ImportStatus.ValueTypeError:
					// エラーの対象となる値を全て検出する
					errorValuesList = GetTypeErrorValues(paymentItem);
					foreach (List<string> errorValue in errorValuesList)
					{
						if (displayErrorMessage.Length != 0) displayErrorMessage.Append("<br />");
						displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_VALUE_TYPE_ERROR).Replace("@@ 1 @@", errorValue[0]).Replace("@@ 2 @@", errorValue[1]).Replace("@@ 3 @@", errorValue[2]));
					}
					break;

				// DBに更新対象なし
				case ImportStatus.NoRecordError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NO_TARGET_ERROR));
					break;

				// DBに更新対象複数
				case ImportStatus.MultiRecordError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MULTI_TARGET_ERROR));

					// 受注のリンクを表示する
					displayErrorMessage.Append("<br /><br />注文ID : ");
					displayErrorMessage.Append(CreatePopUpUrl(paymentItem.orderIdList));
					break;

				// ファイルとDBの照合時にエラー
				case ImportStatus.MatchingError:
					// エラーの対象となる値を全て検出する
					errorValuesList = GetMatchingErrorValues(paymentItem);
					foreach (List<string> errorValues in errorValuesList)
					{
						if (displayErrorMessage.Length != 0) displayErrorMessage.Append("<br />");
						displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MATCHING_ERROR).Replace("@@ 1 @@", errorValues[0]).Replace("@@ 2 @@", errorValues[1]).Replace("@@ 3 @@", errorValues[2]));
					}

					// 受注のリンクを表示する
					displayErrorMessage.Append("<br /><br />注文ID : ");
					displayErrorMessage.Append(CreatePopUpUrl(paymentItem.orderIdList));
					break;

				// 入金ステータスが不正
				case ImportStatus.PaymentStatusError:
					if (m_xmlSetting.allowMultiRecordUpdate == true && paymentItem.orderData.Count > 1)
					{
						// 複数行用の入金ステータスエラー文言
						displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_STATUS_MULTIERROR)
							.Replace("@@ 1 @@", ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)));
					}
					else
					{
						displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_STATUS_ERROR)
							.Replace("@@ 1 @@", ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, paymentItem.orderData[0][Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS]))
							.Replace("@@ 2 @@", ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)));
					}
					// 受注のリンクを表示する
					displayErrorMessage.Append("<br /><br />注文ID : ");
					displayErrorMessage.Append(CreatePopUpUrl(paymentItem.orderIdList));
					break;
			}

			return displayErrorMessage.ToString();
		}

		/// <summary>
		/// 抽出に使う型チェックのエラー内容取得
		/// </summary>
		/// <param name="paymentItem">取込ファイル1行分</param>
		/// <returns>エラーが検出されたデータの表示名と値のリスト</returns>
		private List<List<string>> GetTypeErrorValues(PaymentItem paymentItem)
		{
			List<List<string>> errorValuesList = new List<List<string>>();

			// 検索部分の型チェックを行う
			foreach (ImportSetting.ConditionItem item in m_xmlSetting.matchCondition)
			{
				if (TryCast(paymentItem.csvData[item.columnNo - 1], item.columnType) == false)
				{
					List<string> errorValues = new List<string>();
					errorValues.Add(item.name);
					errorValues.Add(GetTypeNameString(item.columnType));
					errorValues.Add(paymentItem.csvData[item.columnNo - 1]);
					errorValuesList.Add(errorValues);
				}
			}

			// 照合部分の型チェックを行う
			foreach (ImportSetting.ConditionItem item in m_xmlSetting.whereCondition)
			{
				if (TryCast(paymentItem.csvData[item.columnNo - 1], item.columnType) == false)
				{
					List<string> errorValues = new List<string>();
					errorValues.Add(item.name);
					errorValues.Add(GetTypeNameString(item.columnType));
					errorValues.Add(paymentItem.csvData[item.columnNo - 1]);
					errorValuesList.Add(errorValues);
				}
			}
			return errorValuesList;
		}

		/// <summary>
		/// 照合に使う型チェックのエラー内容取得
		/// </summary>
		/// <param name="paymentItem">取込ファイル1行分</param>
		/// <returns>エラーが検出されたデータの表示名と値のリスト</returns>
		private List<List<string>> GetMatchingErrorValues(PaymentItem paymentItem)
		{
			List<List<string>> errorValuesList = new List<List<string>>();

			// ファイルとDBのデータを照合
			foreach (ImportSetting.ConditionItem conditionItem in m_xmlSetting.matchCondition)
			{
				foreach (DataRowView order in paymentItem.orderData)
				{
					if (CompareData(paymentItem.csvData, order, conditionItem) == false)
					{
						List<string> errorValues = new List<string>();
						errorValues.Add(conditionItem.name);
						errorValues.Add(paymentItem.csvData[conditionItem.columnNo - 1]); // ファイル側の値
						errorValues.Add(order[conditionItem.fieldName].ToString()); // DB側の値
						errorValuesList.Add(errorValues);
					}
				}
			}
			return errorValuesList;
		}

		/// <summary>
		/// オブジェクトを指定したデータ型で同一の値か比較して結果を返す
		/// </summary>
		/// <param name="paymentItem">取込ファイル1行分</param>
		/// <param name="conditionItem">XML設定ファイル：DB照合定義1フィールド分</param>
		/// <returns>比較結果</returns>
		private bool CompareData(PaymentItem paymentItem, ImportSetting.ConditionItem conditionItem)
		{
			foreach (DataRowView order in paymentItem.orderData)
			{
				if (CompareData(paymentItem.csvData, order, conditionItem) == false)
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// オブジェクトを指定したデータ型で同一の値か比較して結果を返す
		/// </summary>
		/// <param name="csvData">取込ファイル1行分</param>
		/// <param name="orderData">DBから抽出したデータ</param>
		/// <param name="conditionItem">XML設定ファイル：DB照合定義1フィールド分</param>
		/// <returns>比較結果</returns>
		private bool CompareData(string[] csvData, DataRowView orderData, ImportSetting.ConditionItem conditionItem)
		{
			return (CastFieldData(csvData[conditionItem.columnNo - 1], conditionItem.columnType).Equals
					(CastFieldData(orderData[conditionItem.fieldName].ToString(), conditionItem.columnType)));
		}

		/// <summary>
		/// 入金ステータスが「未入金」かどうか確認
		/// </summary>
		/// <param name="paymentItem">確認対象のPaymentItem</param>
		/// <returns>すべて問題なければTrue</returns>
		private bool CheckPaymentStatus(PaymentItem paymentItem)
		{
			foreach (DataRowView drv in paymentItem.orderData)
			{
				if ((string)drv[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] != Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 型の文言をSQLServerの型データにする
		/// </summary>
		/// <param name="columnType">型名</param>
		/// <returns>SQLServerの型</returns>
		private SqlDbType GetDbFieldType(string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					return SqlDbType.Int;

				case TYPE_DATE:
					return SqlDbType.DateTime;

				case TYPE_TEXT:
					return SqlDbType.NVarChar;

				default:
					return SqlDbType.NVarChar;
			}
		}

		/// <summary>
		/// データ型の日本語名を返す
		/// </summary>
		/// <param name="columnType">型名</param>
		/// <returns>日本語での型名</returns>
		private string GetTypeNameString(string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					return "数値";

				case TYPE_DATE:
					return "日付";

				case TYPE_TEXT:
					return "文字列";

				default:
					return "";
			}
		}

		/// <summary>
		/// XMLで指定した型設定に即した変換を行う
		/// </summary>
		/// <param name="data">変換対象の値</param>
		/// <param name="columnType">変換対象の型名</param>
		/// <returns>型変換後のデータ</returns>
		private object CastFieldData(string data, string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					return Decimal.Parse(data);

				case TYPE_DATE:
					return CastShortDatetime(data);

				case TYPE_TEXT:
					return (string)data;

				default:
					return null;
			}
		}

		/// <summary>
		/// 指定した型に変換できるかチェック
		/// </summary>
		/// <param name="data">変換対象の値</param>
		/// <param name="columnType">変換対象の型名</param>
		/// <returns>チェック結果</returns>
		private bool TryCast(string data, string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					decimal decParseDummy;
					return decimal.TryParse(data, out decParseDummy);

				case TYPE_DATE:
					DateTime dummy;
					return DateTime.TryParse(data, out dummy) || DateTime.TryParseExact(data, "yyyyMMdd", null, DateTimeStyles.AllowLeadingWhite, out dummy);

				case TYPE_TEXT:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// ヘッダ行と除外行をファイルに書き出す
		/// </summary>
		/// <param name="paymentData">処理中の</param>
		/// <param name="originalFileName">元のファイル名称</param>
		private void CreateExcludeRecordsFile(List<PaymentItem> paymentData, string originalFileName)
		{
			if (paymentData.Exists(item => item.status == ImportStatus.ExcludeRecord) == false)
			{
				// 除外レコードが無ければ何もしない
				return;
			}

			// 作成するファイル名
			string fileName = originalFileName + "_exc.csv";

			// 出力先ディレクトリが無ければ作成
			Directory.CreateDirectory(this.PhysicalOutputDirPath);

			// ファイルパスの準備
			string filePath = this.PhysicalOutputDirPath + fileName;

			// CSVファイルの書き出し
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS")))
			{
				// ヘッダ行・フッタ行と除外行を書き出す
				foreach (PaymentItem paymentItem in paymentData.FindAll(item => (item.status == ImportStatus.HeaderRecord) || (item.status == ImportStatus.FooterRecord) || (item.status == ImportStatus.ExcludeRecord)))
				{
					sw.Write(StringUtility.CreateEscapedCsvString(paymentItem.csvData) + "\r\n");
				}
			}

			this.FilePathExcludeRecord = this.OutputDirPath + fileName;
		}

		/// <summary>
		/// 年月日を返す
		/// </summary>
		/// <param name="uncastedDate">日付データ（文字列型・日付型）</param>
		/// <remark>時分秒はデフォルト(00:00:000)にする</remark>
		private DateTime CastShortDatetime(string dateString)
		{
			DateTime result;
			if (DateTime.TryParse(dateString, out result))
			{
				return result.Date;
			}
			// TryParseできなかった場合は形式を指定して再度型変換する
			DateTime.TryParseExact(dateString, "yyyyMMdd", null, DateTimeStyles.AllowLeadingWhite, out result);
			return result.Date;
		}

		/// <summary>
		/// ポップアップリンクを作成する
		/// </summary>
		/// <param name="orderIdList">注文ID</param>
		/// <returns>注文情報詳細URL</returns>
		private string CreatePopUpUrl(List<string> orderIdList)
		{
			if (m_CreatePopupMessageProc == null) { return string.Empty; }

			return m_CreatePopupMessageProc(orderIdList);
		}

		/// <summary>
		/// 取込設定
		/// </summary>
		private class ImportSetting
		{
			/// <summary>入金日の列番号</summary>
			public int[] paymentDateColumnNo;
			/// <summary>取込除外するヘッダ行数</summary>
			public int headerRowCount;
			/// <summary>取込除外するフッタ行数</summary>
			public int footerRowCount;
			/// <summary>取込ファイルの列数</summary>
			public int columnCount;
			/// <summary>過去何ヵ月</summary>
			public int pastMonths;
			/// <summary>複数行の更新の許可設定</summary>
			public bool allowMultiRecordUpdate;
			/// <summary>レコードが存在しない場合の除外の許可設定</summary>
			public bool noRecordExclude;
			/// <summary>抽出の条件</summary>
			public List<ConditionItem> whereCondition;
			/// <summary>突き合せの条件</summary>
			public List<ConditionItem> matchCondition;
			/// <summary>除外対象となる条件</summary>
			public List<ConditionItem> excludeCondition;

			/// <summary>
			/// 各種設定の細かな条件
			/// </summary>
			public class ConditionItem
			{
				/// <summary>項目表示名</summary>
				public string name;
				/// <summary>対象カラム番号</summary>
				public int columnNo;
				/// <summary>カラムの型</summary>
				public string columnType;
				/// <summary>紐づくDBフィールド名</summary>
				public string fieldName;
				/// <summary>除外処理用：判定種別を指定</summary>
				public string mode;
				/// <summary>除外処理用：比較文字列を指定</summary>
				public string value;
			}
		}

		/// <summary>
		/// 取込ファイル1行分のデータ・ステータス
		/// </summary>
		private class PaymentItem
		{
			/// <summary>ファイル内容(1行分)</summary>
			public string[] csvData;
			/// <summary>DB内容(N行分)</summary>
			public DataView orderData;
			/// <summary>入金日</summary>
			public string paymentDateString;
			/// <summary>注文ID</summary>
			public List<string> orderIdList;
			/// <summary>ステータス</summary>
			public ImportStatus status;
			/// <summary>行番号</summary>
			public int rowNo;
		}

		#region +SetCreatePopuoMessageProcポップアップメッセージ生成処理指定
		/// <summary>
		/// ポップアップメッセージ生成処理（EC管理画面のみの利用想定）
		/// </summary>
		private Func<List<string>, string> m_CreatePopupMessageProc;
		/// <summary>
		/// ポップアップメッセージ生成処理指定
		/// </summary>
		/// <param name="createPopupMessageProc">ポップアップメッセージ作成処理</param>
		/// <returns>インスタンス</returns>
		public ImportPayment SetCreatePopupMessageProc(Func<List<string>, string> createPopupMessageProc)
		{
			m_CreatePopupMessageProc = createPopupMessageProc;
			return this;
		}
		#endregion

		/// プロパティ
		/// <summary>処理結果メッセージ</summary>
		public string ErrorMessage { get; private set; }
		/// <summary>更新件数</summary>
		public int UpdatedCount { get; private set; }
		/// <summary>処理行数</summary>
		public int LinesCount { get; private set; }
		/// <summary>エラー情報</summary>
		public List<Dictionary<string, string>> ErrorList { get; private set; }
		/// <summary>出力先ディレクトリパス物理パス</summary>
		private string PhysicalOutputDirPath
		{
			get { return Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + (Constants.PATH_CONTENTS + "Csv/").Replace("/", @"\"); }
		}
		/// <summary>出力先ディレクトリパス</summary>
		private string OutputDirPath
		{
			get { return Constants.PATH_ROOT_EC + (Constants.PATH_CONTENTS + "Csv/"); }
		}
		/// <summary>除外レコードの出力ファイルパス</summary>
		public string FilePathExcludeRecord { get; private set; }

	}
}
